using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;

using System.Runtime.Serialization;
using System.Xml;

using MinimalisticTelnet;
using NationalInstruments;
using NationalInstruments.DAQmx;

namespace powercal
{
    enum BoardTypes { Hornshark, Mudshark, Humpback, Hooktooth, Milkshark, Zebrashark };

    public class Relay_Lines {
        
        static string _key_acPower = "AC Power";
        static string _key_load = "Load";
        static string _key_ember = "Ember";
        static string _key_volts = "Voltmeter";

        public static string Power { get { return _key_acPower; } }
        public static string Load { get { return _key_load; } }
        public static string Ember { get { return _key_ember; } }
        public static string Voltmeter { get { return _key_volts; } }
    }

    public partial class FormMain : Form
    {
        MultiMeter _meter = null;
        RelayControler _relay_ctrl;

        string _key_acPower = "AC Power";
        string _key_load = "Load";
        string _key_ember = "Ember";
        string _key_volts = "Voltmeter";

        /// <summary>
        /// The app folder where we save most logs, etc
        /// </summary>
        static string _app_data_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration");
        /// <summary>
        /// Path to the app log file
        /// </summary>
        string _log_file = Path.Combine(_app_data_dir, "runlog.txt");

        /// <summary>
        /// Voltage and current limits
        /// </summary>
        double _voltage_low_limit = 0.0;
        double _voltage_high_limit = 0.0;
        double _current_high_limit = 0.0;
        double _current_low_limit = 0.0;
        /// <summary>
        /// Voltage and current reference value
        /// </summary>
        int _voltage_reference = 0;
        int _current_reference = 0;
        /// <summary>
        /// Prefix to custom pload and oinfo commands (soon to be auto configured)
        /// </summary>
        string _cmd_prefix = "cs5490"; // UART interface


        /// <summary>
        /// Path to where the Ember programing batch file is created
        /// </summary>
        string _ember_batchfile_path = Path.Combine(_app_data_dir, "patchit.bat");
        /// <summary>
        /// Process used to open Ember box ports (isachan=all)
        /// </summary>
        Process _p_ember_isachan;


        public struct CS_Current_Voltage
        {
            public double Current;
            public double Voltage;

            public CS_Current_Voltage(double i = 0.0, double v = 0.0)
            {
                Current = i;
                Voltage = v;
            }
        }

        delegate void SetTextCallback(string txt);

        public FormMain()
        {

            Stream outResultsFile = File.Create("output.txt");
            var textListener = new TextWriterTraceListener(outResultsFile);
            Trace.Listeners.Add(textListener);

            // All dio port 0 lines to 0
            dio_write(0);

            InitializeComponent();

            // Create the app data folder
            if (!Directory.Exists(_app_data_dir))
            {
                Directory.CreateDirectory(_app_data_dir);
            }
            // Init the log file
            initLogFile();

            // Set the title to match assembly info from About dlg
            AboutBox1 aboutdlg = new AboutBox1();
            this.Text = aboutdlg.AssemblyTitle;
            aboutdlg.Dispose();

            // Init the status text box
            initTextBoxRunStatus();

            // Make sure we have a selection for board types
            this.comboBoxBoardTypes.Items.AddRange(Enum.GetNames(typeof(powercal.BoardTypes)));
            int index = comboBoxBoardTypes.Items.IndexOf(Properties.Settings.Default.Last_Used_Board);
            if (index < 0)
                index = 0;
            if (comboBoxBoardTypes.Items.Count > 0)
                comboBoxBoardTypes.SelectedIndex = index;


            cleanupEmberTempPatchFile();

            // Report COM ports found in system
            string[] ports = SerialPort.GetPortNames();
            string msg = "";
            foreach (string portname in ports)
            {
                msg += string.Format("{0}, ", portname);
            }
            if (msg != "")
            {
                msg = msg.TrimEnd(new char[] { ' ', ',' });
                msg = string.Format("System serial ports: {0}", msg);
                updateOutputStatus(msg);
            }

            // Detect whether meter is connected to one of the ports
            bool detected_meter = autoDetectMeterCOMPort();

            // Init relay controller
            RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(typeof(RelayControler.Device_Types), Properties.Settings.Default.Relay_Controller_Type);
            try
            {
                _relay_ctrl = new RelayControler(rdevtype);
            }
            catch (Exception)
            {
                msg = string.Format("Unable to init relay controler \"{0}\".  Switching to Manual relay mode", rdevtype);
                updateOutputStatus(msg);

                _relay_ctrl = new RelayControler(RelayControler.Device_Types.Manual);
                Properties.Settings.Default.Relay_Controller_Type = _relay_ctrl.Device_Type.ToString();
                Properties.Settings.Default.Save();
            }



            // Ember path
            if (!Directory.Exists(Properties.Settings.Default.Ember_BinPath))
            {
                msg = string.Format("Unable to find Ember bin path \"{0}\"", Properties.Settings.Default.Ember_BinPath);
            }
            else
            {
                msg = string.Format("Ember bin path set at\"{0}\"", Properties.Settings.Default.Ember_BinPath);
            }
            updateOutputStatus(msg);

            kill_em3xx_load();
        }



        /// <summary>
        /// Removes any Ember temp files and reports it in output status
        /// </summary>
        void cleanupEmberTempPatchFile()
        {
            string[] ember_temp_files = Ember.CleanupTempPatchFile();
            foreach (string file in ember_temp_files)
            {
                updateOutputStatus(string.Format("Ember temp file found and removed {0}", file));
            }
        }

        /// <summary>
        /// Detects whether the metter is ON and connected to one of the COM ports
        /// If one is found, the serial port setting is changed automatically
        /// </summary>
        /// <returns>Whether a meter was detected connected to the system</returns>
        bool autoDetectMeterCOMPort()
        {
            bool detected = false;
            string[] ports = SerialPort.GetPortNames();
            foreach (string portname in ports)
            {
                MultiMeter meter = new MultiMeter(portname);
                try
                {
                    meter.WaitForDsrHolding = false;
                    meter.OpenComPort();
                    string idn = meter.IDN();
                    meter.CloseSerialPort();

                    if (idn.StartsWith("HEWLETT-PACKARD,34401A"))
                    {
                        detected = true;
                        Properties.Settings.Default.Meter_COM_Port_Name = portname;
                        Properties.Settings.Default.Save();
                        string msg = string.Format("Multimetter '{0}' comunications port autodetected at {1}", idn.TrimEnd('\n'), Properties.Settings.Default.Meter_COM_Port_Name);
                        updateOutputStatus(msg);
                        break;
                    }

                }
                catch (Exception ex)
                {
                    string msgx = ex.Message;
                }
                meter.CloseSerialPort();
            }
            if (!detected)
            {
                string msg = string.Format("Unable to detect Multimetter comunications port. Using {0}.  Measurements set to manual mode", Properties.Settings.Default.Meter_COM_Port_Name);

                Properties.Settings.Default.Meter_Manual_Measurement = true;
                Properties.Settings.Default.Save();

                updateOutputStatus(msg);
            }

            return detected;

        }

        /// <summary>
        /// Inits the log file.  
        /// Creates folder loction...
        /// </summary>
        void initLogFile()
        {
            if (!Directory.Exists(_app_data_dir))
            {
                Directory.CreateDirectory(_app_data_dir);
            }
            if (!File.Exists(_log_file))
            {
                using (StreamWriter sw = File.CreateText(_log_file))
                {
                    sw.WriteLine("{0:G}: Log created", DateTime.Now);
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// Inits the run status text box
        /// </summary>
        void initTextBoxRunStatus()
        {
            this.textBoxRunStatus.BackColor = Color.White;
            this.textBoxRunStatus.ForeColor = Color.Black;
            this.textBoxRunStatus.Clear();
        }

        /// <summary>
        /// Make thread safe call to update status text box
        /// </summary>
        /// <param name="txt"></param>
        private void setOutputStatus(string txt)
        {
            if (this.textBoxOutputStatus.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setOutputStatus);
                try
                {
                    this.Invoke(d, new object[] { txt });
                }
                catch { }
            }
            else
            {
                updateOutputStatus(txt);
            }
        }

        /// <summary>
        /// Updates the output status text box and log file
        /// </summary>
        /// <param name="txt"></param>
        private void updateOutputStatus(string txt)
        {
            string line = string.Format("{0:G}: {1}\r\n", DateTime.Now, txt);

            Trace.WriteLine(line);

            this.textBoxOutputStatus.AppendText(line);
            this.textBoxOutputStatus.Update();

            using (StreamWriter sw = File.AppendText(_log_file))
            {
                sw.WriteLine("{0:G}: {1}", DateTime.Now, txt);
            }
        }

        /// <summary>
        /// Writes to the trace
        /// </summary>
        /// <param name="txt"></param>
        private void traceLog(string txt)
        {
            string line = string.Format("{0:G}: {1}\r\n", DateTime.Now, txt);
            Trace.WriteLine(line);
        }

        /// <summary>
        /// Updates the run status text box
        /// </summary>
        /// <param name="txt"></param>
        private void updateRunStatus(string txt)
        {
            this.textBoxRunStatus.Text = txt;
            this.textBoxRunStatus.Update();
            updateOutputStatus(txt);

            string line = string.Format("{0:G}: {1}", DateTime.Now, txt);
            Trace.WriteLine(line);

        }

        /// <summary>
        /// Invokes Serial test gld
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_Serial(object sender, EventArgs e)
        {
            FormSerialTest dlg = new FormSerialTest();
            //DialogResult result = dlg.ShowDialog();
            dlg.Show();
        }

        /// <summary>
        /// Invokes About dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_About(object sender, EventArgs e)
        {
            AboutBox1 dlg = new AboutBox1();
            DialogResult result = dlg.ShowDialog();
        }

        /// <summary>
        /// Shows dialog with relays sates defined in the RelayControler when in manual mode
        /// </summary>
        /// <param name="relay_ctrl"></param>
        private void relaysSet()
        {

            if (_relay_ctrl.Device_Type == RelayControler.Device_Types.Manual)
            {
                //string msg_dlg = relay_ctrl.ToDlgText();
                string key = "AC Power";
                bool value = _relay_ctrl.ReadLine(key);
                string on_off_str = "OFF";
                if (value)
                    on_off_str = "ON";
                string msg_dlg = string.Format("{0} = {1}\r\n", key, on_off_str);

                key = "Ember";
                value = _relay_ctrl.ReadLine(key);
                on_off_str = "OFF";
                if (value)
                    on_off_str = "ON";
                msg_dlg += string.Format("{0} = {1}\r\n", key, on_off_str);

                key = "Load";
                value = _relay_ctrl.ReadLine(key);
                on_off_str = "OFF";
                if (value)
                    on_off_str = "ON";
                msg_dlg += string.Format("{0} = {1}\r\n", key, on_off_str);

                MessageBox.Show(msg_dlg);
            }
            string status = _relay_ctrl.ToStatusText();
            traceLog(status);
            updateOutputStatus(status);
        }

        /// <summary>
        /// Loags the status of the relays
        /// </summary>
        private void relay_log_status()
        {
            string status = _relay_ctrl.ToStatusText();
            traceLog(status);
            updateOutputStatus(status);
        }

        /// <summary>
        /// Writes to first DIO port if one if found
        /// </summary>
        /// <param name="value">value to write</param>
        private void dio_write(int value)
        {
            try
            {
                string[] channels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External);
                if (channels.Length > 0)
                {
                    using (Task digitalWriteTask = new Task())
                    {
                        digitalWriteTask.DOChannels.CreateChannel(channels[0], "port0", ChannelLineGrouping.OneChannelForAllLines);
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, (UInt32)value);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Kills any em3xx_load process running in the system
        /// </summary>
        private void kill_em3xx_load()
        {
            try
            {
                Process[] processes = System.Diagnostics.Process.GetProcessesByName("em3xx_load");
                foreach (Process process in processes)
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error killing em3xx_load.\r\n{0}", ex.Message);
                updateOutputStatus(msg);
            }
        }

        private void toolStripMenuItem_Click_Clear(object sender, EventArgs e)
        {
            this.textBoxOutputStatus.Clear();
        }

        /// <summary>
        /// Sets values used for calibration for selected board
        /// </summary>
        private void set_board_calibration_values()
        {
            // Default values (USA)
            double voltage_load = 120;
            double voltage_delta = voltage_load * 0.2;
            double current_load = voltage_load / 500; // 500 Ohms
            double current_delta = current_load * 0.3;

            _voltage_reference = 240;
            _current_reference = 15;
            _cmd_prefix = "cs5490"; // UART interface

            powercal.BoardTypes board_type = (powercal.BoardTypes)Enum.Parse(typeof(powercal.BoardTypes), comboBoxBoardTypes.Text);
            switch (board_type)
            {
                case BoardTypes.Humpback:
                    voltage_load = 240;
                    current_load = voltage_load / 2000; // 2K Ohms
                    voltage_delta = voltage_load * 0.3;
                    current_delta = current_load * 0.4;
                    break;
                case BoardTypes.Zebrashark:
                    _cmd_prefix = "cs5480";  // SPI interface
                    break;
                case BoardTypes.Milkshark:
                case BoardTypes.Mudshark:
                    _current_reference = 10;
                    break;
            }

            _voltage_high_limit = voltage_load + voltage_delta;
            _voltage_low_limit = voltage_load - voltage_delta;
            _current_high_limit = current_load + 3 * current_delta;  // On Hornshark current measurement is very high with gain set to 1
            _current_low_limit = current_load - current_delta;

        }

        /// <summary>
        /// Sends a pload command and returns the current and voltage values
        /// </summary>
        /// <param name="tc">Telnet connection to the EMber</param>
        /// <param name="board_type">What board are we using</param>
        /// <returns>Current/Voltage structure values</returns>
        CS_Current_Voltage ember_parse_pinfo_registers(TelnetConnection tc)
        {
            string rawCurrentPattern = "Raw IRMS: ([0-9,A-F]{8})";
            string rawVoltagePattern = "Raw VRMS: ([0-9,A-F]{8})";
            double current_cs = 0.0;
            double voltage_cs = 0.0;

            tc.WriteLine(string.Format("cu {0}_pload", _cmd_prefix));
            Thread.Sleep(500);
            string datain = tc.Read();
            Trace.WriteLine(datain);
            string msg;
            if (datain.Length > 0)
            {
                Match on_off_match = Regex.Match(datain, "Changing OnOff .*");
                if (on_off_match.Success)
                {
                    msg = on_off_match.Value;
                    updateOutputStatus(msg);
                }

                Match match = Regex.Match(datain, rawCurrentPattern);
                if (match.Groups.Count != 2)
                {
                    msg = string.Format("Unable to parse pinfo for current.  Output was:{0}", datain);
                    throw new Exception(msg);
                }

                string current_hexstr = match.Groups[1].Value;
                int current_int = Convert.ToInt32(current_hexstr, 16);
                current_cs = RegHex_ToDouble(current_int);
                current_cs = current_cs * _current_reference / 0.6;

                voltage_cs = 0.0;
                match = Regex.Match(datain, rawVoltagePattern);
                if (match.Groups.Count != 2)
                {
                    msg = string.Format("Unable to parse pinfo for voltage.  Output was:{0}", datain);
                    throw new Exception(msg);
                }

                string voltage_hexstr = match.Groups[1].Value;
                int volatge_int = Convert.ToInt32(voltage_hexstr, 16);
                voltage_cs = RegHex_ToDouble(volatge_int);
                voltage_cs = voltage_cs * _voltage_reference / 0.6;

            }

            CS_Current_Voltage current_voltage = new CS_Current_Voltage(i: current_cs, v: voltage_cs);
            return current_voltage;
        }

        /// <summary>
        /// Converts a 24bit hex (3 bytes) CS register value to a double
        /// </summary>
        /// <example>
        /// byte[] rx_data = new byte[3];
        /// rx_data[2] = 0x5c;
        /// rx_data[1] = 0x28;
        /// rx_data[0] = 0xf6;
        /// Should return midrange =~ 0.36
        /// </example>
        /// <param name="rx_data">data byte array byte[2] <=> MSB ... byte[0] <=> LSB</param>
        /// <returns>range 0 <= value < 1.0</returns>
        private static double RegHex_ToDouble(int data)
        {
            // Maximum 1 =~ 0xFFFFFF
            // Max rms 0.6 =~ 0x999999
            // Half rms 0.36 =~ 0x5C28F6
            double value = ((double)data) / 0x1000000; // 2^24
            return value;
        }

        /// <summary>
        /// Converts a hex string (3 bytes) CS register vaue to a double
        /// </summary>
        /// <param name="hexstr"></param>
        /// <returns>range 0 <= value < 1.0</returns>
        /// <seealso cref="double RegHex_ToDouble(int data)"/>
        private static double RegHex_ToDouble(string hexstr)
        {
            int val_int = Convert.ToInt32(hexstr, 16);
            return RegHex_ToDouble(val_int); ;
        }

        /// <summary>
        /// Handels error data from the em3xx_load process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void p_ember_isachan_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string str = "Error: " + e.Data;
                traceLog(str);
                setOutputStatus(str);
            }
        }

        /// <summary>
        /// Handels output data from the em3xx_load process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void p_ember_isachan_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string str = e.Data;
            //setOutputStatus(str);
            traceLog(str);
        }

        /// <summary>
        /// Patches the flash with calibration tokens
        /// </summary>
        /// <param name="board_type"></param>
        /// <param name="voltage_gain"></param>
        /// <param name="current_gain"></param>
        /// <returns></returns>
        string patch(BoardTypes board_type, int voltage_gain, int current_gain)
        {
            Ember ember = new Ember();
            ember.EmberBinPath = Properties.Settings.Default.Ember_BinPath;
            ember.BatchFilePath = _ember_batchfile_path;
            ember.VoltageRefereceValue = _voltage_reference;
            ember.CurrentRefereceValue = _current_reference;
            switch (board_type)
            {
                case (powercal.BoardTypes.Humpback):
                    ember.VoltageAdress = 0x08080980;
                    ember.CurrentAdress = 0x08080984;
                    ember.RefereceAdress = 0x08080988;
                    ember.ACOffsetAdress = 0x080809CC;
                    break;
                case (powercal.BoardTypes.Zebrashark):
                case (powercal.BoardTypes.Hooktooth):
                case (powercal.BoardTypes.Milkshark):
                    ember.VoltageAdress = 0x08040980;
                    ember.CurrentAdress = 0x08040984;
                    ember.ACOffsetAdress = 0x080409CC;
                    break;
            }
            ember.CreateCalibrationPatchBath(voltage_gain, current_gain);

            bool patchit_fail = false;
            string exception_msg = "";
            string coding_output = "";
            // Retry patch loop if fail
            while (true)
            {
                patchit_fail = false;
                exception_msg = "";
                coding_output = "";
                try
                {
                    string output = ember.RunCalibrationPatchBatch();
                    if (output.Contains("ERROR:"))
                    {
                        patchit_fail = true;
                        exception_msg = "Patching error detected:\r\n";
                        exception_msg += output;
                    }
                    coding_output = output;
                }
                catch (Exception e)
                {
                    patchit_fail = true;
                    exception_msg = "Patching exception detected:\r\n";
                    exception_msg += e.Message;
                }

                if (patchit_fail)
                {
                    string retry_err_msg = exception_msg;
                    int max_len = 1000;
                    if (retry_err_msg.Length > max_len)
                        retry_err_msg = retry_err_msg.Substring(0, max_len) + "...";
                    DialogResult dlg_rc = MessageBox.Show(retry_err_msg, "Patching fail", MessageBoxButtons.RetryCancel);
                    if (dlg_rc == System.Windows.Forms.DialogResult.Cancel)
                        break;
                }
                else
                {
                    break;
                }

                reset_handler(board_type);

            }

            if (patchit_fail)
            {
                throw new Exception(exception_msg);
            }

            return coding_output;
        }

        /// <summary>
        /// Invikes the settings dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_Settings(object sender, EventArgs e)
        {
            FormSettings dlg = new FormSettings();

            // COM ports
            dlg.CheckBoxManualMultiMeter.Checked = Properties.Settings.Default.Meter_Manual_Measurement;
            dlg.TextBoxMeterCOM.Text = Properties.Settings.Default.Meter_COM_Port_Name;

            // Pupulate DIO controller types
            dlg.comboBoxDIOCtrollerTypes.Items.Clear();
            Array relay_types = Enum.GetValues( typeof(RelayControler.Device_Types) );
            foreach (RelayControler.Device_Types relay_type in relay_types)
                dlg.comboBoxDIOCtrollerTypes.Items.Add(relay_type.ToString());
            dlg.comboBoxDIOCtrollerTypes.Text = Properties.Settings.Default.Relay_Controller_Type;

            // DIO line assigment
            Dictionary<string,uint> relay_lines = _relay_ctrl.DicLines_ReadSettings();

            dlg.NumericUpDown_ACPower.Value = relay_lines[_key_acPower];
            dlg.NumericUpDown_Load.Value = relay_lines[_key_load];
            dlg.NumericUpDown_Ember.Value = relay_lines[_key_ember];
            dlg.numericUpDown_Voltmeter.Value = relay_lines[_key_volts];

            // Ember
            dlg.TextBoxEmberBinPath.Text = Properties.Settings.Default.Ember_BinPath;

            DialogResult rc = dlg.ShowDialog();
            if (rc == DialogResult.OK)
            {
                // COM ports
                Properties.Settings.Default.Meter_COM_Port_Name = dlg.TextBoxMeterCOM.Text;
                Properties.Settings.Default.Meter_Manual_Measurement = dlg.CheckBoxManualMultiMeter.Checked;

                // DIO controller type
                Properties.Settings.Default.Relay_Controller_Type = dlg.comboBoxDIOCtrollerTypes.Text;

                // DIO line assigment
                relay_lines[_key_acPower] = (uint)dlg.NumericUpDown_ACPower.Value;
                relay_lines[_key_load] = (uint)dlg.NumericUpDown_Load.Value;
                relay_lines[_key_ember] = (uint)dlg.NumericUpDown_Ember.Value;
                relay_lines[_key_volts] = (uint)dlg.numericUpDown_Voltmeter.Value;
                _relay_ctrl.DicLines_SaveSettings();

                // Ember
                Properties.Settings.Default.Ember_BinPath = dlg.TextBoxEmberBinPath.Text;

                Properties.Settings.Default.Save();
            }
        }

        private void toolStripMenuItem_Click_Calculator(object sender, EventArgs e)
        {
            FormCalculator dlg = new FormCalculator();
            dlg.ShowDialog();
        }

        private void toolStripMenuItem_Click_PowerMeter(object sender, EventArgs e)
        {
            FormPowerMeter dlg = new FormPowerMeter();
            dlg.Show();
        }


        /// <summary>
        /// Starts the process responsible to open the Ember box isa channels
        /// </summary>
        private void openEmberISAChannels()
        {
            _p_ember_isachan = new Process()
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
            _p_ember_isachan.EnableRaisingEvents = true;
            _p_ember_isachan.OutputDataReceived += p_ember_isachan_OutputDataReceived;
            _p_ember_isachan.ErrorDataReceived += p_ember_isachan_ErrorDataReceived;
            _p_ember_isachan.Start();
            _p_ember_isachan.BeginOutputReadLine();
            _p_ember_isachan.BeginErrorReadLine();

        }

        /// <summary>
        /// Closes the Ember process that open the isa channels
        /// <seealso cref="openEmberISAChannels"/>
        /// </summary>
        private void closeEmberISAChannels()
        {
            _p_ember_isachan.CancelErrorRead();
            _p_ember_isachan.CancelOutputRead();
            _p_ember_isachan.Kill();
            _p_ember_isachan.Close();
        }

        /// <summary>
        /// Telnets to the Ember and prints custom commands
        /// Parses command list and tries to find the pload or pinfo comand prefix
        /// It is usually "cs5480_" in the case of SPDI or "cs5490_" in the case of UART comunications
        /// Exception is thrown if not pload command is found after typing "cu"
        /// </summary>
        /// <returns></returns>
        private string get_custom_command_prefix(TelnetConnection telnet_connection)
        {
            string cmd_pre = null;

            int try_count = 0;
            string data = "";

            while (true)
            {
                telnet_connection.WriteLine("cu");
                data += telnet_connection.Read();
                if (data.Contains("pload"))
                    break;
                try_count++;
                if (try_count > 3)
                    break;
            }

            string msg = "";
            if (!data.Contains("pload"))
            {
                msg = string.Format("Unable to get custum command output list from Ember.  Output was: {0}", data);
                throw new Exception(msg);
            }

            string pattern = @"(cs[0-9]{4})_pload\r\n";
            Match match = Regex.Match(data, pattern);
            if (match.Groups.Count != 2)
            {
                msg = string.Format("Unable to parse custom command list for pload.  Output was:{0}", data);
                throw new Exception(msg);
            }

            cmd_pre = match.Groups[1].Value;
            return cmd_pre;
        }

        /// <summary>
        /// Handels a board reset
        /// </summary>
        /// <param name="board_type"></param>
        void reset_handler(BoardTypes board_type)
        {

            switch (board_type)
            {
                // These boards are hanging on reset
                case BoardTypes.Hornshark:
                case BoardTypes.Mudshark:
                    // Force reset by cycle power
                    updateRunStatus("Reset by cycle power");
                    _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, false);
                    relaysSet();
                    _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, true);
                    relaysSet();
                    Thread.Sleep(1000);
                    break;
            }
        }

        /// <summary>
        /// Closes the board releay using custom command
        /// </summary>
        /// <param name="board_type"></param>
        /// <param name="telnet_connection"></param>
        void set_board_relay(BoardTypes board_type, TelnetConnection telnet_connection, bool value)
        {
            switch (board_type)
            {
                // These boards have relays
                case BoardTypes.Hooktooth:
                case BoardTypes.Hornshark:
                case BoardTypes.Humpback:
                case BoardTypes.Zebrashark:
                    updateRunStatus( string.Format("Set UUT Relay {0}", value.ToString() ) );
                    if(value)
                        telnet_connection.WriteLine("write 1 6 0 1 0x10 {01}");
                    else
                        telnet_connection.WriteLine("write 1 6 0 1 0x10 {00}");
                    break;
            }

        }

        /// <summary>
        /// Calibrates using just the Ember
        /// Voltage and Current register values are gathered using custom commands
        /// </summary>
        private void calibrate()
        {
            // Remember to set power to external on ember

            // Set values depending on board type tested
            set_board_calibration_values();

            bool manual_measure = Properties.Settings.Default.Meter_Manual_Measurement;
            powercal.BoardTypes board_type = (powercal.BoardTypes)Enum.Parse(typeof(powercal.BoardTypes), comboBoxBoardTypes.Text);

            string msg;
            updateOutputStatus("===============================Start Calibration==============================");

            // Trun AC ON
            //_relay_ctrl.ResetDevice();
            _relay_ctrl.WriteLine(powercal.Relay_Lines.Ember, true);
            _relay_ctrl.WriteLine(powercal.Relay_Lines.Load, false);
            _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, true);
            _relay_ctrl.WriteLine(powercal.Relay_Lines.Load, true);
            relaysSet();
            Thread.Sleep(3000);

            // Open Ember isa channels
            updateRunStatus("Start Ember isachan");
            openEmberISAChannels();

            // Create a new telnet connection
            updateRunStatus("Start telnet");
            TelnetConnection telnet_connection = new TelnetConnection("localhost", 4900);
            string datain = telnet_connection.Read();

            // Patch gain to 1
            updateRunStatus("Patch Gain to 1");
            msg = patch(board_type, 0x400000, 0x400000);
            traceLog(msg);

            Thread.Sleep(3000);
            datain = telnet_connection.Read();
            traceLog(datain);

            // Force reset by cycle power
            reset_handler(board_type);

            // Close UUT relay
            set_board_relay(board_type, telnet_connection, true);

            Thread.Sleep(1000);
            datain = telnet_connection.Read();
            traceLog(datain);


            // Setup multi-meter
            // Take a measurement with load on
            string meterPortName = Properties.Settings.Default.Meter_COM_Port_Name;
            _meter = new MultiMeter(meterPortName);
            if (!manual_measure)
            {
                updateRunStatus("Setup multi-meter");
                _meter.OpenComPort();
                _meter.SetToRemote();
                _meter.ClearError();
                _meter.SetupForVAC();

                string meter_load_voltage_str = _meter.Measure();
                meter_load_voltage_str = _meter.Measure();
                double meter_load_voltage = Double.Parse(meter_load_voltage_str);
                msg = string.Format("Meter Load Voltage at {0:F8} V", meter_load_voltage);
                updateOutputStatus(msg);

                _meter.CloseSerialPort();
                //string idn = _meter.IDN();

                if (meter_load_voltage < _voltage_low_limit || meter_load_voltage > _voltage_high_limit)
                {
                    msg = string.Format("Meter measured Vrms before calibration is not within limits values: {0:F8} < {1:F8} < {2:F8}", _voltage_low_limit, meter_load_voltage, _voltage_high_limit);
                    traceLog(msg);
                    throw new Exception(msg);
                }
            }

            string cmd_prefix = get_custom_command_prefix(telnet_connection);

            // Get UUT currect/voltage values
            updateRunStatus("Get UUT values");
            CS_Current_Voltage cv = ember_parse_pinfo_registers(telnet_connection);
            msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}", cv.Current, cv.Voltage, cv.Current * cv.Voltage);
            traceLog(msg);
            cv = ember_parse_pinfo_registers(telnet_connection);
            msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}", cv.Current, cv.Voltage, cv.Current * cv.Voltage);
            updateOutputStatus(msg);

            if (cv.Voltage < _voltage_low_limit || cv.Voltage > _voltage_high_limit)
            {
                msg = string.Format("Cirrus voltage before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", _voltage_low_limit, cv.Voltage, _voltage_high_limit);
                throw new Exception(msg);
            }
            //if (cv.Current < _current_low_limit || cv.Current > _current_high_limit)
            if (cv.Current < 0.01 || cv.Current > 1)
            {
                //msg = string.Format("Cirrus current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", _current_low_limit, cv.Current, _current_high_limit);
                msg = string.Format("Cirrus current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", 0.01, cv.Current, 1);
                throw new Exception(msg);
            }

            /// The meter measurements
            updateRunStatus("Meter measurements");
            _meter.OpenComPort();
            _meter.SetupForIAC();

            string current_meter_str = _meter.Measure();
            current_meter_str = _meter.Measure();
            double current_meter = Double.Parse(current_meter_str);
            msg = string.Format("Meter I = {0:F8}", current_meter);
            traceLog(msg);
            current_meter_str = _meter.Measure();
            current_meter = Double.Parse(current_meter_str);

            _meter.SetupForVAC();
            string voltage_meter_str = _meter.Measure();
            voltage_meter_str = _meter.Measure();
            double voltage_meter = Double.Parse(voltage_meter_str);
            msg = string.Format("Meter V = {0:F8}", voltage_meter);
            traceLog(msg);
            voltage_meter_str = _meter.Measure();
            voltage_meter = Double.Parse(voltage_meter_str);

            _meter.CloseSerialPort();

            msg = string.Format("Meter I = {0:F8}, V = {1:F8}, P = {2:F8}", current_meter, voltage_meter, current_meter * voltage_meter);
            updateOutputStatus(msg);

            if (voltage_meter < _voltage_low_limit || voltage_meter > _voltage_high_limit)
            {
                msg = string.Format("Meter voltage before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", _voltage_low_limit, voltage_meter, _voltage_high_limit);
                throw new Exception(msg);
            }
            //if (current_meter < _current_low_limit || current_meter > _current_high_limit)
            if (current_meter < 0.01 || current_meter > 1)
            {
                //msg = string.Format("Meter current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", _current_low_limit, current_meter, _current_high_limit);
                msg = string.Format("Meter current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", 0.01, current_meter, 1);
                throw new Exception(msg);
            }

            // Gain calucalation
            updateRunStatus("Gain calucalation");
            double current_gain = current_meter / cv.Current;
            //double current_gain = current_meter / current_cs;
            int current_gain_int = (int)(current_gain * 0x400000);
            msg = string.Format("Current Gain = {0:F8} (0x{1:X})", current_gain, current_gain_int);
            updateOutputStatus(msg);

            double voltage_gain = voltage_meter / cv.Voltage;
            int voltage_gain_int = (int)(voltage_gain * 0x400000);
            msg = string.Format("Voltage Gain = {0:F8} (0x{1:X})", voltage_gain, voltage_gain_int);
            updateOutputStatus(msg);

            // Patch new gain
            updateRunStatus("Patch Gain");
            msg = patch(board_type, voltage_gain_int, current_gain_int);

            Thread.Sleep(3000);
            datain = telnet_connection.Read();
            traceLog(datain);

            // Force reset by cycle power
            reset_handler(board_type);

            datain = telnet_connection.Read();
            traceLog(datain);

            telnet_connection.WriteLine(string.Format("cu {0}_pinfo", _cmd_prefix));
            Thread.Sleep(500);
            datain = telnet_connection.Read();
            traceLog(datain);

            // Get UUT currect/voltage values
            updateRunStatus("Get UUT calibrated values");
            cv = ember_parse_pinfo_registers(telnet_connection);
            msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}", cv.Current, cv.Voltage, cv.Current * cv.Voltage);
            traceLog(msg);
            cv = ember_parse_pinfo_registers(telnet_connection);
            msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}", cv.Current, cv.Voltage, cv.Current * cv.Voltage);
            updateOutputStatus(msg);

            // Open UUT relay
            //set_board_relay(board_type, telnet_connection, false);
            //Thread.Sleep(1000);
            //datain = telnet_connection.Read();
            //traceLog(datain);

            _p_ember_isachan.CancelOutputRead();
            _p_ember_isachan.CancelErrorRead();

            updateRunStatus("Close telnet");
            telnet_connection.Close();

            updateRunStatus("Close Ember isachan");
            closeEmberISAChannels();

            // Disconnect Power
            _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, false);
            _relay_ctrl.WriteLine(powercal.Relay_Lines.Ember, false);
            relaysSet();

            // Check calibration
            double delta = voltage_meter * 0.3;
            double high_limit = voltage_meter + delta;
            double low_limit = voltage_meter - delta;
            if (cv.Voltage < low_limit || cv.Voltage > high_limit)
            {
                msg = string.Format("Voltage after calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", low_limit, cv.Voltage, high_limit);
                Trace.WriteLine(msg);
                throw new Exception(msg);
            }
            delta = current_meter * 0.3;
            high_limit = current_meter + delta;
            low_limit = current_meter - delta;
            if (cv.Current < low_limit || cv.Current > high_limit)
            {
                msg = string.Format("Current after calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", low_limit, cv.Current, high_limit);
                Trace.WriteLine(msg);
                throw new Exception(msg);
            }

            // Measure Voltage after power off
            _meter.OpenComPort();
            _meter.SetupForVAC();
            voltage_meter_str = _meter.Measure();
            _meter.CloseSerialPort();
            voltage_meter = Double.Parse(voltage_meter_str);
            msg = string.Format("Meter V = {0:F8}", voltage_meter);
            traceLog(msg);
            updateOutputStatus(msg);

            this.textBoxRunStatus.BackColor = Color.Green;
            this.textBoxRunStatus.ForeColor = Color.White;
            this.textBoxRunStatus.Text = "PASS";

            updateOutputStatus("================================End Calibration===============================");

        }

        /// <summary>
        /// Run Calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRun_Click(object sender, EventArgs e)
        {
            //this.buttonRun.Enabled = false;

            this.textBoxOutputStatus.Clear();
            initTextBoxRunStatus();

            kill_em3xx_load();

            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                cleanupEmberTempPatchFile();

                calibrate();

                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;
                // Format and display the TimeSpan value. 
                string elapsedTime = String.Format("Elaspsed time {0:00} seconds", ts.TotalSeconds);
                
                updateOutputStatus(elapsedTime);

                if (_meter != null)
                    _meter.CloseSerialPort();

            }
            catch (Exception ex)
            {
                this.textBoxRunStatus.BackColor = Color.Red;
                this.textBoxRunStatus.ForeColor = Color.White;
                updateRunStatus("FAIL");
                updateOutputStatus(ex.Message);

                _relay_ctrl.WriteLine(powercal.Relay_Lines.Ember, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Load, false);
                relaysSet();

                if (_meter != null)
                    _meter.CloseSerialPort();
            }

            kill_em3xx_load();
            cleanupEmberTempPatchFile();

            this.buttonRun.Enabled = true;
        }

        /// <summary>
        /// Invokes the NI DIO test dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_NI(object sender, EventArgs e)
        {
            FormNIDigitalPortTest dlg = new FormNIDigitalPortTest();
            dlg.Show();
        }

        /// <summary>
        /// Invokes the FTDI test dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_FT232H(object sender, EventArgs e)
        {
            FormFT232H_DIO_Test dlg = new FormFT232H_DIO_Test();
            dlg.Show();
        }

        /// <summary>
        /// Remember the last board we used
        /// 

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxBoardTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Last_Used_Board = this.comboBoxBoardTypes.Text;
            Properties.Settings.Default.Save();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _relay_ctrl.Close();
        }

    }

}
