using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Deployment.Application;

namespace PowerCalibration
{
    /// <summary>
    /// Creates and runs batch file to patch calibration tokens at specified addresses
    /// </summary>
    class Ember
    {
        public string BatchFilePath { get { return _batch_file; } set { _batch_file = value; } }
        public string EmberBinPath { get { return _ember_bin_path; } set { _ember_bin_path = value; } }
        public string EmberExe { get { return _ember_exe; } set { _ember_exe = value; } }

        public int VoltageAdress { get { return _voltageAddress; } set { _voltageAddress = value; } }
        public int CurrentAdress { get { return _currentAddress; } set { _currentAddress = value; } }
        public int RefereceAdress { get { return _refAddress; } set { _refAddress = value; } }
        public int ACOffsetAdress { get { return _acOffsetAddress; } set { _acOffsetAddress = value; } }

        public int VoltageRefereceValue { get { return _voltageRefValue; } set { _voltageRefValue = value; } }
        public int CurrentRefereceValue { get { return _currentRefValue; } set { _currentRefValue = value; } }

        string _batch_file = "C:\\patchit.bat";
        string _ember_exe = "em3xx_load";
        string _ember_bin_path = "C:\\Program Files (x86)\\Ember\\ISA3 Utilities\\bin";
        static string _ember_work_path;
        public string Work_Path { get { return _ember_work_path; } set { _ember_work_path = value; } }

        public enum Interfaces { USB, IP };
        Interfaces _interface = Interfaces.USB;
        public Interfaces Interface { get { return _interface; } set { _interface = value; } }
        string _interface_address = "localhost";
        public string Interface_Address { get { return _interface_address; } set { _interface_address = value; } }
        //private int _usb_port = 0;

        private int _voltageAddress = 0x08040980;
        private int _currentAddress = 0x08040984;
        private int _refAddress = 0x08040988;
        private int _acOffsetAddress = 0x080409CC;

        // For Humpback:
        //To set the VREF to 240, the patch contains "@08080988=F0 @08080989=00"
        //To set the IREF to 15, the patch contains "@0808098A=0F @0808098B=00"
        private int _voltageRefValue = 0xF0;
        private int _currentRefValue = 0x0F;

        Process _process_ember_isachan;

        public delegate void Process_ISAChan_Error_Handler(object sender, DataReceivedEventArgs e);
        public event Process_ISAChan_Error_Handler Process_ISAChan_Error_Event;

        public delegate void Process_ISAChan_Output_Handler(object sender, DataReceivedEventArgs e);
        public event Process_ISAChan_Output_Handler Process_ISAChan_Output_Event;

        /// <summary>
        /// Starts the process responsible to open the Ember box isa channels
        /// </summary>
        public Process OpenISAChannels()
        {
            Kill_em3xx_load();

            _process_ember_isachan = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(Properties.Settings.Default.Ember_BinPath, "em3xx_load.exe"),

                    Arguments = "--isachan=all",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = false
                }
            };

            if (_interface == Interfaces.IP)
                _process_ember_isachan.StartInfo.Arguments += string.Format(" --ip={0}", _interface_address);
            else if (_interface == Interfaces.USB)
                _process_ember_isachan.StartInfo.Arguments += string.Format(" --usb={0}", _interface_address);

            _process_ember_isachan.EnableRaisingEvents = true;
            _process_ember_isachan.OutputDataReceived += process_isachan_OutputDataReceived;
            _process_ember_isachan.ErrorDataReceived += process_isachan_ErrorDataReceived;

            _process_ember_isachan.Start();

            _process_ember_isachan.BeginOutputReadLine();
            _process_ember_isachan.BeginErrorReadLine();

            return _process_ember_isachan;
        }

        /// <summary>
        /// Closes the Ember process that open the isa channels
        /// <seealso cref="openEmberISAChannels"/>
        /// </summary>
        public void CloseISAChannels()
        {
            if (_process_ember_isachan != null)
            {
                try
                {
                    _process_ember_isachan.CancelErrorRead();
                    _process_ember_isachan.CancelOutputRead();
                    if (!_process_ember_isachan.HasExited)
                        _process_ember_isachan.Kill();
                    _process_ember_isachan.Close();
                }
                catch { }
            }
        }

        void process_isachan_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (Process_ISAChan_Error_Event != null)
            {
                Process_ISAChan_Error_Event(sender, e);
            }
        }

        void process_isachan_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (Process_ISAChan_Output_Event != null)
            {
                Process_ISAChan_Output_Event(sender, e);
            }
        }

        /// <summary>
        /// Runs a calibartion batch file
        /// </summary>
        /// <returns></returns>
        public string RunCalibrationPatchBatch()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = _batch_file;
            p.Start();

            int n = 0;
            string error, output;
            while (!p.HasExited)
            {
                Thread.Sleep(1000);
                n++;
                if (n > 10)
                {
                    p.Kill();
                    output = p.StandardOutput.ReadToEnd();
                    error = p.StandardError.ReadToEnd();
                    string msg = string.Format("Timeout running {0}.\r\n", _batch_file);
                    if (output != null && output.Length > 0)
                        msg += string.Format("Output: {0}\r\n", output);
                    if (error != null && error.Length > 0)
                        msg += string.Format("Error: {0}\r\n", error);

                    throw new Exception(msg);
                }
            }

            error = p.StandardError.ReadToEnd();
            output = p.StandardOutput.ReadToEnd();
            int rc = p.ExitCode;
            if (rc != 0)
            {
                string msg = string.Format("Error running {0}.\r\n", _batch_file);
                msg += string.Format("RC: {0}\r\n", rc);
                if (error != null && error.Length > 0)
                    msg += string.Format("Error: {0}\r\n", error);

                throw new Exception(msg);
            }
            return output;
        }

        /// <summary>
        /// Creates a calibration batch file
        /// </summary>
        /// <param name="vrms">Vrms gain</param>
        /// <param name="irms">Irms gain</param>
        public void CreateCalibrationPatchBath(int vrms, int irms)
        {
            using (StreamWriter writer = File.CreateText(_batch_file))
            {
                string txt;
                if (Directory.Exists(_ember_work_path))
                    txt = string.Format("pushd \"{0}\"", _ember_work_path);
                else
                    txt = string.Format("pushd \"{0}\"", AppDomain.CurrentDomain.BaseDirectory);
                //txt = string.Format("pushd \"{0}\"", _ember_bin_path);
                writer.WriteLine(txt);

                //txt = string.Format("{0} --usb {1}", _ember_exe, _usb_port);
                //writer.WriteLine(txt);

                txt = string.Format("\"{0}\"", Path.Combine(_ember_bin_path, _ember_exe));
                //txt = string.Format("{0}", _ember_exe);


                if (_interface == Interfaces.USB)
                    txt += string.Format(" --usb {0}", _interface_address);
                else if (_interface == Interfaces.IP)
                    txt += string.Format(" --ip {0}", _interface_address);
                txt += " --patch ";


                // vrms
                int start_addr = _voltageAddress;
                byte[] data = bit24IntToByteArray(vrms);
                foreach (byte b in data)
                {
                    txt += string.Format("@{0:X8}=", start_addr);
                    txt += string.Format("{0:X2} ", b);
                    start_addr++;
                }
                txt += string.Format("@{0:X8}=", start_addr);
                txt += string.Format("{0:X2} ", 0); // null

                // irms
                start_addr = _currentAddress;
                data = bit24IntToByteArray(irms);
                foreach (byte b in data)
                {
                    txt += string.Format("@{0:X8}=", start_addr);
                    txt += string.Format("{0:X2} ", b);
                    start_addr++;
                }
                txt += string.Format("@{0:X8}=", start_addr);
                txt += string.Format("{0:X2} ", 0); // null

                // referece
                if (_voltageRefValue != 0x0)
                {
                    start_addr = _refAddress;
                    // vref
                    txt += string.Format("@{0:X8}=", start_addr++);
                    txt += string.Format("{0:X2} ", _voltageRefValue);
                    txt += string.Format("@{0:X8}=", start_addr++);
                    txt += string.Format("00 ");

                    // iref
                    txt += string.Format("@{0:X8}=", start_addr++);
                    txt += string.Format("{0:X2} ", _currentRefValue);
                    txt += string.Format("@{0:X8}=", start_addr++);
                    txt += string.Format("00 ");
                }

                // ac offset
                start_addr = _acOffsetAddress;
                data = bit24IntToByteArray(0);
                foreach (byte b in data)
                {
                    txt += string.Format("@{0:X8}=", start_addr);
                    txt += string.Format("{0:X2} ", b);
                    start_addr++;
                }
                txt += string.Format("@{0:X8}=", start_addr);
                txt += string.Format("{0:X2} ", 0); // null

                writer.WriteLine(txt);

                txt = string.Format("popd");
                writer.WriteLine(txt);

                writer.Close();
            }
        }

        /// <summary>
        /// Removes the temp file that is created when a operation such as em3xx_load is initiated
        /// This is helpful when the load failed on a device and the file was not removed.  
        /// The next load will fail if this file is found present
        /// </summary>
        /// <returns>Array with any file found and deleted</returns>
        public static string[] CleanupTempPatchFile()
        {
            List<string> removed_list = new List<string>();
            string file_find_name = "em3xx_load_temp_patch_file_*.s37";

            string path = _ember_work_path;
            removed_list.AddRange(removeFilesFromFolder(path, file_find_name).ToArray());

            path = AppDomain.CurrentDomain.BaseDirectory;
            removed_list.AddRange(removeFilesFromFolder(path, file_find_name).ToArray());

            path = Path.GetTempPath();
            removed_list.AddRange(removeFilesFromFolder(path, file_find_name).ToArray());

            path = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            path = Path.Combine(path, @"VirtualStore\Program Files (x86)\Ember\ISA3 Utilities\bin"); 
            removed_list.AddRange(removeFilesFromFolder(path, file_find_name).ToArray());

            path = Environment.GetEnvironmentVariable("USERPROFILE");
            removed_list.AddRange(removeFilesFromFolder(path, file_find_name).ToArray());

            return removed_list.ToArray();
        }

        static List<string> removeFilesFromFolder(string folder, string file_find_name)
        {
            List<string> removed_list = new List<string>();
            if (Directory.Exists(folder))
            {
                string[] files = Directory.GetFiles(folder, file_find_name);
                foreach (string file in files)
                {
                    File.Delete(file);
                    removed_list.Add(file);
                }
            }

            return removed_list;
        }

        /// <summary>
        /// Kills any em3xx_load process running in the system
        /// </summary>
        public static void Kill_em3xx_load()
        {
            try
            {
                Process[] processes = System.Diagnostics.Process.GetProcessesByName("em3xx_load");
                foreach (Process process in processes)
                {
                    if (!process.HasExited)
                        process.Kill();
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error killing em3xx_load.\r\n{0}", ex.Message);
            }
        }


        /// <summary>
        /// Breaks an int into 3 bytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The last 3 bytes of int value</returns>
        static byte[] bit24IntToByteArray(int value)
        {
            // Converts a 24bit value to a 3 byte array
            // Oreder by LSB to MSB
            byte[] vBytes = new byte[3] { 
                (byte)(value & 0xFF), 
                (byte)( (value >> 8) & 0xFF), 
                (byte)( (value >> 16) & 0xFF) };

            return vBytes;
        }
    }
}
