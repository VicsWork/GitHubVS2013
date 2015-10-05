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
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using System.Runtime.Serialization;
using System.Xml;

using System.ServiceModel;
using System.ServiceModel.Description;

using MinimalisticTelnet;

namespace powercal
{
    enum BoardTypes { Hornshark, Mudshark, Humpback, Hooktooth, Milkshark, Zebrashark };

    public partial class Form_Main : Form, ICalibrationService
    {
        MultiMeter _meter = null;
        RelayControler _relay_ctrl;

        TelnetConnection _telnet_connection;

        Stopwatch _stopwatch_calibration_running = new Stopwatch();
        Stopwatch _stopwatch_calibration_stopped = new Stopwatch();

        string _calibration_error_msg = null;

        /// <summary>
        /// The app folder where we save most logs, etc
        /// </summary>
        static string _app_data_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration");
        /// <summary>
        /// Path to the app log file
        /// </summary>
        string _log_file = Path.Combine(_app_data_dir, "runlog.txt");

        /// <summary>
        /// Path to where the Ember programing batch file is created
        /// </summary>
        string _ember_batchfile_path = Path.Combine(_app_data_dir, "patchit.bat");
        /// <summary>
        /// Process used to open Ember box ports (isachan=all)
        /// </summary>
        Process _p_ember_isachan;

        delegate void SetTextCallback(string txt);
        delegate void SetTextColorCallback(string txt, Color forecolor, Color backcolor);
        delegate void SetEnablementCallback(Boolean enable);
        delegate void UpdateCallback();

        /// <summary>
        /// The main form constructor
        /// </summary>
        public Form_Main()
        {
            Stream outResultsFile = File.Create("output.txt");
            var textListener = new TextWriterTraceListener(outResultsFile);
            Trace.Listeners.Add(textListener);

            InitializeComponent();

            _stopwatch_calibration_running.Reset();
            _stopwatch_calibration_stopped.Start();

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
            runStatus_Init();

            // Make sure we have a selection for board types
            this.comboBoxBoardTypes.Items.AddRange(Enum.GetNames(typeof(powercal.BoardTypes)));
            int index = comboBoxBoardTypes.Items.IndexOf(Properties.Settings.Default.Last_Used_Board);
            if (index < 0)
                index = 0;
            if (comboBoxBoardTypes.Items.Count > 0)
                comboBoxBoardTypes.SelectedIndex = index;


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
            _relay_ctrl.Open();
            _relay_ctrl.WriteAll(false);
            _relay_ctrl.Close();

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
        void runStatus_Init()
        {
            this.textBoxRunStatus.BackColor = Color.White;
            this.textBoxRunStatus.ForeColor = Color.Black;
            this.textBoxRunStatus.Clear();
            this.textBoxRunStatus.Update();
        }

        void setEnablement(Boolean enable)
        {
            if (this.InvokeRequired)
            {
                SetEnablementCallback d = new SetEnablementCallback(setEnablement);
                this.Invoke(d, new object[] { enable });
            }
            else
            {
                this.buttonRun.Enabled = enable;
                this.comboBoxBoardTypes.Enabled = enable;
                this.menuStripMain.Enabled = enable;

                if (this.buttonRun.Enabled)
                {
                    UseWaitCursor = false;
                    this.Cursor = this.DefaultCursor;
                    //this.Update();
                }
                else
                {
                    this.UseWaitCursor = true;
                }

            }
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

        void setOutputStatusText(string text)
        {
            if (this.textBoxOutputStatus.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setOutputStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxOutputStatus.AppendText(text);
                this.textBoxOutputStatus.Update();
            }
        }

        /// <summary>
        /// Updates the output status text box and log file
        /// </summary>
        /// <param name="txt"></param>
        void updateOutputStatus(string txt)
        {
            string line = TraceLogger.Log(txt);
            using (StreamWriter sw = File.AppendText(_log_file))
            {
                sw.WriteLine(line);
            }

            line = string.Format("{0}\r\n", line);
            setOutputStatusText(line);
        }

        void setRunStatusText(string text)
        {
            if (this.textBoxRunStatus.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setRunStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxRunStatus.Text = text;
                this.textBoxRunStatus.Update();
            }
        }

        void setRunStatusTextColor(string text, Color forecolor, Color backcolor)
        {
            if (this.textBoxRunStatus.InvokeRequired)
            {
                SetTextColorCallback d = new SetTextColorCallback(setRunStatusTextColor);
                this.Invoke(d, new object[] { text, forecolor, backcolor });
            }
            else
            {
                this.textBoxRunStatus.Text = text;
                this.textBoxRunStatus.ForeColor = forecolor;
                this.textBoxRunStatus.BackColor = backcolor;
                this.textBoxRunStatus.Update();
            }
        }

        /// <summary>
        /// Updates the run status text box
        /// </summary>
        /// <param name="txt"></param>
        void updateRunStatus(string txt)
        {
            setRunStatusText(txt);
            setOutputStatus(txt);
        }

        void updateRunStatus(string txt, Color forecolor, Color backcolor)
        {
            setRunStatusTextColor(txt, forecolor, backcolor);
            setOutputStatus(txt);
        }

        /// <summary>
        /// Invokes Serial test gld
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_Serial(object sender, EventArgs e)
        {
            Form_SerialTest dlg = new Form_SerialTest();
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
            updateOutputStatus(status);
        }

        /// <summary>
        /// Loags the status of the relays
        /// </summary>
        private void relay_log_status()
        {
            string status = _relay_ctrl.ToStatusText();
            updateOutputStatus(status);
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
                    if (!process.HasExited)
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
        /// Handels error data from the em3xx_load process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void p_ember_isachan_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string str = "Error: " + e.Data;
                TraceLogger.Log(str);
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
            TraceLogger.Log(str);
        }

        /// <summary>
        /// Invikes the settings dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_Settings(object sender, EventArgs e)
        {
            Form_Settings dlg = new Form_Settings();

            // COM ports
            dlg.CheckBoxManualMultiMeter.Checked = Properties.Settings.Default.Meter_Manual_Measurement;
            dlg.TextBoxMeterCOM.Text = Properties.Settings.Default.Meter_COM_Port_Name;

            // Pupulate DIO controller types
            dlg.comboBoxDIOCtrollerTypes.Items.Clear();
            Array relay_types = Enum.GetValues(typeof(RelayControler.Device_Types));
            foreach (RelayControler.Device_Types relay_type in relay_types)
                dlg.comboBoxDIOCtrollerTypes.Items.Add(relay_type.ToString());
            dlg.comboBoxDIOCtrollerTypes.Text = Properties.Settings.Default.Relay_Controller_Type;

            // DIO line assigment
            Dictionary<string, uint> relay_lines = _relay_ctrl.DicLines_ReadSettings();
            dlg.NumericUpDown_ACPower.Value = relay_lines[powercal.Relay_Lines.Power];
            dlg.NumericUpDown_Load.Value = relay_lines[powercal.Relay_Lines.Load];
            dlg.NumericUpDown_Ember.Value = relay_lines[powercal.Relay_Lines.Ember];
            dlg.numericUpDown_Voltmeter.Value = relay_lines[powercal.Relay_Lines.Voltmeter];

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
                RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(typeof(RelayControler.Device_Types), Properties.Settings.Default.Relay_Controller_Type);
                _relay_ctrl = new RelayControler(rdevtype);

                // DIO line assigment
                relay_lines = _relay_ctrl.DicLines_ReadSettings();
                relay_lines[powercal.Relay_Lines.Power] = (uint)dlg.NumericUpDown_ACPower.Value;
                relay_lines[powercal.Relay_Lines.Load] = (uint)dlg.NumericUpDown_Load.Value;
                relay_lines[powercal.Relay_Lines.Ember] = (uint)dlg.NumericUpDown_Ember.Value;
                relay_lines[powercal.Relay_Lines.Voltmeter] = (uint)dlg.numericUpDown_Voltmeter.Value;
                _relay_ctrl.DicLines_SaveSettings();


                // Ember
                Properties.Settings.Default.Ember_BinPath = dlg.TextBoxEmberBinPath.Text;

                Properties.Settings.Default.Save();
            }
        }

        private void toolStripMenuItem_Click_Calculator(object sender, EventArgs e)
        {
            Form_Calculator dlg = new Form_Calculator();
            dlg.ShowDialog();
        }

        private void toolStripMenuItem_Click_PowerMeter(object sender, EventArgs e)
        {
            Form_PowerMeter dlg = new Form_PowerMeter();
            dlg.Show();
        }

        /// <summary>
        /// Starts the process responsible to open the Ember box isa channels
        /// </summary>
        private void openEmberISAChannels()
        {
            Ember ember = new Ember();
            ember.Process_ISAChan_Error_Event += p_ember_isachan_ErrorDataReceived;
            ember.Process_ISAChan_Output_Event += p_ember_isachan_OutputDataReceived;
            _p_ember_isachan = ember.OpenEmberISAChannels();
        }

        /// <summary>
        /// Closes the Ember process that open the isa channels
        /// <seealso cref="openEmberISAChannels"/>
        /// </summary>
        private void closeEmberISAChannels()
        {
            if (_p_ember_isachan != null)
            {
                _p_ember_isachan.CancelErrorRead();
                _p_ember_isachan.CancelOutputRead();
                if (!_p_ember_isachan.HasExited)
                    _p_ember_isachan.Kill();
                _p_ember_isachan.Close();
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
                    updateRunStatus(string.Format("Set UUT Relay {0}", value.ToString()));
                    if (value)
                        telnet_connection.WriteLine("write 1 6 0 1 0x10 {01}");
                    else
                        telnet_connection.WriteLine("write 1 6 0 1 0x10 {00}");
                    break;
            }

        }

        /// <summary>
        /// Invokes the NI DIO test dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_NI(object sender, EventArgs e)
        {
            Form_NIDigitalPortTest dlg = new Form_NIDigitalPortTest();
            dlg.Show();
        }

        /// <summary>
        /// Invokes the FTDI test dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click_FT232H(object sender, EventArgs e)
        {
            _relay_ctrl.Close();
            RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(typeof(RelayControler.Device_Types), Properties.Settings.Default.Relay_Controller_Type);
            _relay_ctrl = new RelayControler(rdevtype);
            _relay_ctrl.Open();
            Form_FT232H_DIO_Test dlg = new Form_FT232H_DIO_Test(_relay_ctrl);
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

            updateRunStatus("Ready for " + comboBoxBoardTypes.Text);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                if (this.buttonRun.Enabled)
                    this.buttonClick_Run(this, EventArgs.Empty);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _relay_ctrl.Close();

            Trace.Flush();
            Trace.Close();
        }

        /// <summary>
        /// Wait for VAC power to be off
        /// </summary>
        /// <returns></returns>
        double wait_for_power_off()
        {
            if (_meter == null)
                return -1;

            // Measure Voltage after power off
            _meter.OpenComPort();
            _meter.SetupForVAC();

            double voltage_meter = -1.0;
            int n = 0;
            while (true)
            {
                string voltage_meter_str = _meter.Measure();
                voltage_meter = Double.Parse(voltage_meter_str);
                if (voltage_meter < 1.0)
                    break;
                if (n > 100)
                {
                    _meter.CloseSerialPort();
                    string msg = string.Format("Power not off. VAC = {0:F8}", voltage_meter);
                    throw new Exception(msg);
                }
            }

            _meter.CloseSerialPort();

            return voltage_meter;
        }

        /// <summary>
        /// Run Calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClick_Run(object sender, EventArgs e)
        {
            // Clear the error
            _calibration_error_msg = null;

            if (!this.buttonRun.Enabled)
                return;

            // Just in case we look for an invalid calibrate re-run too fast
            if (_stopwatch_calibration_stopped.Elapsed.TotalMilliseconds < 500)
            {
                _stopwatch_calibration_stopped.Restart();
                return;
            }

            // Disable the app
            setEnablement(false);

            _stopwatch_calibration_running.Restart();

            runStatus_Init();
            setRunStatusText("Start Calibration");

            this.textBoxOutputStatus.Clear();
            
            toolStripStatusLabel.Text = "";
            statusStrip1.Update();
            
            kill_em3xx_load();
            _p_ember_isachan = null;

            try
            {
                bool manual_measure = Properties.Settings.Default.Meter_Manual_Measurement;
                if (manual_measure)
                {
                    _meter = null;
                }
                else
                {
                    _meter = new MultiMeter(Properties.Settings.Default.Meter_COM_Port_Name);
                }


                TraceLogger.Log("Start Ember isachan");
                openEmberISAChannels();

                // Create a new telnet connection
                TraceLogger.Log("Start telnet");
                _telnet_connection = new TelnetConnection("localhost", 4900);

                powercal.BoardTypes board_type = (powercal.BoardTypes)Enum.Parse(typeof(powercal.BoardTypes), comboBoxBoardTypes.Text);

                Calibrate calibrate = new Calibrate(board_type, _relay_ctrl, _telnet_connection, _meter);
                calibrate.Status_Event += calibrate_Status_event;
                calibrate.Run_Status_Event += calibrate_Run_Status_Event;
                calibrate.Relay_Event += calibrate_Relay_Event;
                Task task = new Task(calibrate.Run);
                task.ContinueWith(calibrate_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
                task.ContinueWith(calibration_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);
                task.Start();

            }
            catch (Exception ex)
            {
                _calibration_error_msg = ex.Message;
                calibration_done();
            }

        }

        void calibration_done()
        {
            if (_relay_ctrl != null)
            {
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Ember, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Load, false);
                _relay_ctrl.Close();
            }

            if (_meter != null)
            {
                _meter.CloseSerialPort();
                double voltage_meter = wait_for_power_off();
                string msg = string.Format("Meter VAC = {0:F8}", voltage_meter);
                TraceLogger.Log(msg);
                updateOutputStatus(msg);
            }

            if (_calibration_error_msg == null || _calibration_error_msg == "")
            {
                updateRunStatus("PASS", Color.White, Color.Green);

            }
            else
            {
                updateRunStatus("FAIL", Color.White, Color.Red);
                updateOutputStatus(_calibration_error_msg);
            }

            TraceLogger.Log("Close Ember isachan");
            closeEmberISAChannels();

            if (_telnet_connection != null)
            {
                TraceLogger.Log("Close telnet");
                _telnet_connection.Close();
            }

            kill_em3xx_load();

            _stopwatch_calibration_running.Stop();
            TimeSpan ts = _stopwatch_calibration_running.Elapsed;
            string elapsedTime = String.Format("Elaspsed time {0:00} seconds", ts.TotalSeconds);
            updateOutputStatus(elapsedTime);

            _stopwatch_calibration_running.Reset();
            _stopwatch_calibration_stopped.Restart();

            setEnablement(true);

        }

        void calibration_done_handler(Task task)
        {
            calibration_done();
        }

        void calibrate_exception_handler(Task task)
        {
            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;

            _calibration_error_msg = errmsg;
            calibration_done();
        }

        void calibrate_Relay_Event(object sender, RelayControler relay_controller)
        {
            relaysSet();
        }

        void calibrate_Status_event(object sender, string status_txt)
        {
            updateOutputStatus(status_txt);
        }

        void calibrate_Run_Status_Event(object sender, string status_txt)
        {
            updateRunStatus(status_txt);
        }

        private void timer_Update_Idle_Tick(object sender, EventArgs e)
        {
            if (_stopwatch_calibration_running.Elapsed.Ticks > 0)
            {
                this.toolStripStatusLabel.Text = string.Format("Running {0:dd\\.hh\\:mm\\:ss}", _stopwatch_calibration_running.Elapsed);
            }
            else if (_stopwatch_calibration_stopped.Elapsed.Ticks > 0)
            {
                this.toolStripStatusLabel.Text = string.Format("Idel {0:dd\\.hh\\:mm\\:ss}", _stopwatch_calibration_stopped.Elapsed);
            }
        }

        private void Form_Main_Shown(object sender, EventArgs e)
        {
            buttonRun.Focus();
        }

        public void Calibrate(string boardtype)
        {
            MessageBox.Show(boardtype);
        }

    }


    public class Relay_Lines
    {

        static string _key_acPower = "AC Power";
        static string _key_load = "Load";
        static string _key_ember = "Ember";
        static string _key_volts = "Voltmeter";

        public static string Power { get { return _key_acPower; } }
        public static string Load { get { return _key_load; } }
        public static string Ember { get { return _key_ember; } }
        public static string Voltmeter { get { return _key_volts; } }
    }

}
