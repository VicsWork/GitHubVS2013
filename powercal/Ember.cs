using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace powercal
{
    class Ember
    {
        public string BatchFilePath { get { return _batch_file; } set { _batch_file = value; } }
        public string EmberBinPath { get { return _ember_bin_path; } set { _ember_bin_path = value; } }

        string _batch_file;
        private string _ember_exe = "em3xx_load";
        private string _ember_bin_path = "C:\\Program Files (x86)\\Ember\\ISA3 Utilities\\bin";
        private int _usb_port = 0;
        private int _vAddress = 0x08040980;
        private int _iAddress = 0x08040984;
        private int _acOffsetAddress = 0x080409CC;

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

        public void CreateCalibrationPachBath(int vrms, int irms)
        {
            if (_batch_file == null)
                _batch_file = "C:\\patchit.bat";

            using (StreamWriter writer = File.CreateText(_batch_file))
            {
                string txt = string.Format("pushd \"{0}\"", _ember_bin_path);
                writer.WriteLine(txt);

                txt = string.Format("{0} --usb {1}", _ember_exe, _usb_port);
                writer.WriteLine(txt);

                txt = string.Format("{0} --patch ", _ember_exe);

                // vrms
                int start_addr = _vAddress;
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
                start_addr = _iAddress;
                data = bit24IntToByteArray(irms);
                foreach (byte b in data)
                {
                    txt += string.Format("@{0:X8}=", start_addr);
                    txt += string.Format("{0:X2} ", b);
                    start_addr++;
                }
                txt += string.Format("@{0:X8}=", start_addr);
                txt += string.Format("{0:X2} ", 0); // null

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

        byte[] bit24IntToByteArray(int value)
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
