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

namespace PowerCalibration
{
    enum BoardTypes { Hornshark, Mudshark, Humpback, Hooktooth, Milkshark, Zebrashark };

    public partial class Form_Main : Form, ICalibrationService
    {
        MultiMeter _meter = null; // The multimeter controler
        RelayControler _relay_ctrl; // The relay controller
        TelnetConnection _telnet_connection; // Telnet connection to ISA3 Adapter
        Ember _ember; // The Ember box
        Calibrate _calibrate = new Calibrate(); // Calibration object
        Stopwatch _stopwatch_running = new Stopwatch();  // Used to measure running tasks
        Stopwatch _stopwatch_idel = new Stopwatch();  // Used to measure idel
        string _calibration_error_msg = null;  //  If set this will indicate the calibration error
        string _coding_error_msg;  //  If set this will indicate the coding error
        CancellationTokenSource _coding_token_src_cancel = new CancellationTokenSource();  // Used to cancel coding
        bool _calibrate_after_code = false;  // Indicates whether to calibrate after cooding completes

        static string _app_data_dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration"); //The app folder where we save most logs, etc
        string _log_file = Path.Combine(_app_data_dir, "runlog.txt"); // Path to the app log file
        string _ember_batchfile_path = Path.Combine(_app_data_dir, "patchit.bat"); // Path to where the Ember programing batch file is created

        delegate void clickCalibrateCallback();  // Clicks Calibrate
        delegate void activateCallback();  // Activates the form
        delegate void setTextCallback(string txt);  // Set object text
        delegate void setTextColorCallback(string txt, Color forecolor, Color backcolor); // Set objects color
        delegate void setEnablementCallback(bool enable, bool isCoding);  // Set enablement


        /// <summary>
        /// The main form constructor
        /// </summary>
        public Form_Main()
        {
            Stream outResultsFile = File.Create("output.txt");
            var textListener = new TextWriterTraceListener(outResultsFile);
            Trace.Listeners.Add(textListener);

            InitializeComponent();
            this.Icon = Properties.Resources.IconPowerCalibration;

            _stopwatch_running.Reset();
            _stopwatch_idel.Start();

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
            this.comboBoxBoardTypes.Items.AddRange(Enum.GetNames(typeof(PowerCalibration.BoardTypes)));
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

            // Ember path
            if (!Directory.Exists(Properties.Settings.Default.Ember_BinPath))
                msg = string.Format("Unable to find Ember bin path \"{0}\"", Properties.Settings.Default.Ember_BinPath);
            else
                msg = string.Format("Ember bin path set at\"{0}\"", Properties.Settings.Default.Ember_BinPath);
            updateOutputStatus(msg);

            // Init the Ember object
            _ember = new Ember();
            _ember.Work_Path = _app_data_dir;
            _ember.Process_ISAChan_Error_Event += p_ember_isachan_ErrorDataReceived;
            _ember.Process_ISAChan_Output_Event += p_ember_isachan_OutputDataReceived;

            // Init the calibration object
            _calibrate.Status_Event += calibrate_Status_event;
            _calibrate.Run_Status_Event += calibrate_Run_Status_Event;
            _calibrate.Relay_Event += calibrate_Relay_Event;

            // Enable the app
            setEnablement(true, false);
        }

        /// <summary>
        /// Initializes the relay controller
        /// </summary>
        void initRelayController()
        {
            string msg;
            RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(
                typeof(RelayControler.Device_Types), Properties.Settings.Default.Relay_Controller_Type);
            try
            {
                _relay_ctrl = new RelayControler(rdevtype);
                initRelayController_Lines();
                msg = string.Format("Relay controler \"{0}\" ready.", rdevtype);
                updateOutputStatus(msg);
            }
            catch (Exception ex)
            {
                msg = string.Format("{0}\r\nTry unplugging and replugging the USB device.\r\nThen try to change relay controller in settings dialog", ex.Message);
                MessageBox.Show(msg);

                msg = string.Format("Unable to init relay controler \"{0}\".  Switching to Manual relay mode", rdevtype);
                updateOutputStatus(msg);

                _relay_ctrl = new RelayControler(RelayControler.Device_Types.Manual);
                initRelayController_Lines();
                Properties.Settings.Default.Relay_Controller_Type = _relay_ctrl.Device_Type.ToString();
                Properties.Settings.Default.Save();

                Form_Settings dlg = new Form_Settings();
                dlg.ShowDialog();

            }
            _relay_ctrl.Open();
            _relay_ctrl.WriteAll(false);
            _relay_ctrl.Close();

        }

        void initRelayController_Lines()
        {
            Dictionary<string, uint> dic = _relay_ctrl.DicLines_ReadSettings();
            if (dic.Count == 0)
            {
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Power, 0);
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Load, 1);
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Ember, 2);
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Voltmeter, 3);
                _relay_ctrl.DicLines_SaveSettings();
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
                        string msg = string.Format("Multimetter '{0}' comunications port autodetected at {1}", idn.TrimEnd('\n'),
                            Properties.Settings.Default.Meter_COM_Port_Name);
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
                string msg = string.Format("Unable to detect Multimetter comunications port. Using {0}.  Measurements set to manual mode",
                    Properties.Settings.Default.Meter_COM_Port_Name);

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
                    //sw.Close();
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

        /// <summary>
        /// Set enablement of controls
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="isCoding"></param>
        void setEnablement(bool enable, bool isCoding)
        {
            if (this.InvokeRequired)
            {
                setEnablementCallback d = new setEnablementCallback(setEnablement);
                this.Invoke(d, new object[] { enable, isCoding });
            }
            else
            {
                buttonAll.Enabled = enable;
                buttonCalibrate.Enabled = enable;
                comboBoxBoardTypes.Enabled = enable;
                menuStripMain.Enabled = enable;

                setCodeEnablement(enable, isCoding);
            }
        }

        /// <summary>
        /// Sets the Code button control enablement and label
        /// This buton is also to cancel the operation
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="isCoding"></param>
        void setCodeEnablement(bool enable, bool isCoding)
        {
            if (!enable && isCoding)
            {
                buttonCode.Text = "&Cancel";
                buttonCode.Enabled = true;
            }
            else
            {
                buttonCode.Text = "&Code";
                buttonCode.Enabled = enable;
                buttonAll.Enabled = enable;
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
                setTextCallback d = new setTextCallback(setOutputStatus);
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
        /// Sets the output status text
        /// </summary>
        /// <param name="text"></param>
        void setOutputStatusText(string text)
        {
            if (this.textBoxOutputStatus.InvokeRequired)
            {
                setTextCallback d = new setTextCallback(setOutputStatusText);
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
        /// <param name="text"></param>
        void updateOutputStatus(string text)
        {
            string line = TraceLogger.Log(text);
            using (StreamWriter sw = File.AppendText(_log_file))
            {
                sw.WriteLine(line);
            }

            line = string.Format("{0}\r\n", line);
            setOutputStatusText(line);
        }

        /// <summary>
        /// Sets the run status text
        /// </summary>
        /// <param name="text"></param>
        void setRunStatusText(string text)
        {
            if (this.textBoxRunStatus.InvokeRequired)
            {
                setTextCallback d = new setTextCallback(setRunStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxRunStatus.Text = text;
                this.textBoxRunStatus.Update();
            }
        }

        /// <summary>
        /// Sets the Run status text and backgorind color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        void setRunStatusTextColor(string text, Color forecolor, Color backcolor)
        {
            if (this.textBoxRunStatus.InvokeRequired)
            {
                setTextColorCallback d = new setTextColorCallback(setRunStatusTextColor);
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

        /// <summary>
        /// Updates the Run status text and color settings
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
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
        void toolStripMenuItem_Click_Serial(object sender, EventArgs e)
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
        void toolStripMenuItem_Click_About(object sender, EventArgs e)
        {
            AboutBox1 dlg = new AboutBox1();
            DialogResult result = dlg.ShowDialog();
        }

        /// <summary>
        /// Shows dialog with relays sates defined in the RelayControler when in manual mode
        /// </summary>
        /// <param name="relay_ctrl"></param>
        void relaysSet()
        {
            if (_relay_ctrl == null)
                return;

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
        void relay_log_status()
        {
            string status = _relay_ctrl.ToStatusText();
            updateOutputStatus(status);
        }

        /// <summary>
        /// Clears the Outout status text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_StatusClear(object sender, EventArgs e)
        {
            this.textBoxOutputStatus.Clear();
        }

        /// <summary>
        /// Copys the status selected text or all when nothing selected to
        /// the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_StatusCopy(object sender, EventArgs e)
        {
            string text = textBoxOutputStatus.SelectedText;
            if (text.Length == 0)
                text = textBoxOutputStatus.Text;
            System.Windows.Forms.Clipboard.SetText(text);
        }

        /// <summary>
        /// Invikes the settings dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_Settings(object sender, EventArgs e)
        {
            Form_Settings dlg = new Form_Settings();

            // DIO line assigment
            Dictionary<string, uint> relay_lines = _relay_ctrl.DicLines_ReadSettings();
            dlg.NumericUpDown_ACPower.Value = relay_lines[PowerCalibration.Relay_Lines.Power];
            dlg.NumericUpDown_Load.Value = relay_lines[PowerCalibration.Relay_Lines.Load];
            dlg.NumericUpDown_Ember.Value = relay_lines[PowerCalibration.Relay_Lines.Ember];
            dlg.numericUpDown_Voltmeter.Value = relay_lines[PowerCalibration.Relay_Lines.Voltmeter];

            DialogResult rc = dlg.ShowDialog();
            if (rc == DialogResult.OK)
            {
                // COM ports
                Properties.Settings.Default.Meter_COM_Port_Name = dlg.TextBoxMeterCOM.Text;
                Properties.Settings.Default.Meter_Manual_Measurement = dlg.CheckBoxManualMultiMeter.Checked;

                // DIO controller type
                Properties.Settings.Default.Relay_Controller_Type = dlg.comboBoxDIOCtrollerTypes.Text;
                RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(typeof(RelayControler.Device_Types),
                    Properties.Settings.Default.Relay_Controller_Type);
                _relay_ctrl = new RelayControler(rdevtype);

                // DIO line assigment
                relay_lines = _relay_ctrl.DicLines_ReadSettings();
                relay_lines[PowerCalibration.Relay_Lines.Power] = (uint)dlg.NumericUpDown_ACPower.Value;
                relay_lines[PowerCalibration.Relay_Lines.Load] = (uint)dlg.NumericUpDown_Load.Value;
                relay_lines[PowerCalibration.Relay_Lines.Ember] = (uint)dlg.NumericUpDown_Ember.Value;
                relay_lines[PowerCalibration.Relay_Lines.Voltmeter] = (uint)dlg.numericUpDown_Voltmeter.Value;
                _relay_ctrl.DicLines_SaveSettings();

                // Ember
                Properties.Settings.Default.Ember_Interface = dlg.comboBoxEmberInterface.Text;
                Properties.Settings.Default.Ember_BinPath = dlg.TextBoxEmberBinPath.Text;
                if (dlg.comboBoxEmberInterface.Text == "IP")
                    Properties.Settings.Default.Ember_Interface_IP_Address = dlg.textBoxEmberInterfaceAddress.Text;
                else
                    Properties.Settings.Default.Ember_Interface_USB_Address = dlg.textBoxEmberInterfaceAddress.Text;

                Properties.Settings.Default.Save();

                setEnablement(true, false);
            }
            else
            {
                Properties.Settings.Default.Reload();
            }
        }

        /// <summary>
        /// Opens up a basic calculator for 24bit register values convertions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_Calculator(object sender, EventArgs e)
        {
            Form_Calculator dlg = new Form_Calculator();
            dlg.ShowDialog();
        }

        /// <summary>
        /// Starts the UUT power meter dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_PowerMeter(object sender, EventArgs e)
        {
            try
            {
                _relay_ctrl.Open();
                _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                _relay_ctrl.WriteLine(Relay_Lines.Load, true);
                _relay_ctrl.Close();
                Thread.Sleep(1000);

                Calibrate calibrate = new Calibrate();
                calibrate.BoardType = (BoardTypes)Enum.Parse(typeof(BoardTypes), comboBoxBoardTypes.Text);

                string ember_interface = Properties.Settings.Default.Ember_Interface;
                string ember_address = Properties.Settings.Default.Ember_Interface_IP_Address;
                if (Properties.Settings.Default.Ember_Interface == "USB")
                    ember_address = Properties.Settings.Default.Ember_Interface_USB_Address;

                Form_PowerMeter power_meter_dlg = new Form_PowerMeter(
                    ember_interface, ember_address,
                    calibrate.Voltage_Referencer, calibrate.Current_Referencer);


                power_meter_dlg.ShowDialog();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _relay_ctrl.Open();
                _relay_ctrl.WriteLine(Relay_Lines.Power, false);
                _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
                _relay_ctrl.WriteLine(Relay_Lines.Load, false);
                _relay_ctrl.Close();
            }
        }

        /// <summary>
        /// Invokes the NI DIO test dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_NI(object sender, EventArgs e)
        {
            Form_NIDigitalPortTest dlg = new Form_NIDigitalPortTest();
            dlg.Show();
        }

        /// <summary>
        /// Invokes the FTDI test dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_FT232H(object sender, EventArgs e)
        {
            _relay_ctrl.Close();
            RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(typeof(RelayControler.Device_Types), Properties.Settings.Default.Relay_Controller_Type);
            _relay_ctrl = new RelayControler(rdevtype);
            _relay_ctrl.Open();
            Form_FT232H_DIO_Test dlg = new Form_FT232H_DIO_Test(_relay_ctrl);
            dlg.Show();
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
        /// Remember the last board we used
        /// 

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comboBoxBoardTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Last_Used_Board = this.comboBoxBoardTypes.Text;
            Properties.Settings.Default.Save();

            updateRunStatus("Ready for " + comboBoxBoardTypes.Text);
        }

        /// <summary>
        /// Handles the app shortcut keys
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            switch (keyData)
            {
                case Keys.Space:
                    string action = Properties.Settings.Default.Shortcut_Spacebar_Action;
                    switch (action)
                    {
                        case "All":
                            if (buttonAll.Enabled)
                                buttonAll.PerformClick();
                            break;
                        case "Code":
                            if (buttonCode.Enabled)
                                buttonCode.PerformClick();
                            break;
                        case "Calibrate":
                            if (buttonCalibrate.Enabled)
                                buttonCalibrate.PerformClick();
                            break;
                    }
                    break;
                case Keys.L:
                    if (buttonCalibrate.Enabled)
                        buttonCalibrate.PerformClick();
                    break;
                case Keys.C:
                    if (buttonCode.Enabled)
                        buttonCode.PerformClick();
                    break;
                case Keys.A:
                    if (buttonAll.Enabled)
                        buttonAll.PerformClick();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Handle form closing clean up stuff
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            if (_relay_ctrl != null && _relay_ctrl.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, false);  // AC measure

            // Measure Voltage after power off
            _meter.OpenComPort();
            _meter.SetupForVAC();
            double vac = -1.0;
            int n = 0;
            string msg;
            while (true)
            {
                string voltage_meter_str = _meter.Measure();
                vac = Double.Parse(voltage_meter_str);
                if (vac < 1.0)
                    break;
                if (n > 10)
                {
                    _meter.CloseSerialPort();
                    msg = string.Format("Warning.  Failed to power off. VAC = {0:F8}", vac);
                    throw new Exception(msg);
                }
            }

            if (_relay_ctrl != null && _relay_ctrl.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, true);  // DC Measure
            _meter.SetupForVDC();

            double vdc = -1.0;
            n = 0;
            while (true)
            {
                string voltage_meter_str = _meter.Measure();
                vdc = Double.Parse(voltage_meter_str);
                if (vdc < 1.0)
                    break;
                if (n > 10)
                {
                    _meter.CloseSerialPort();
                    msg = string.Format("Warning DC voltage detected after power off. VDC = {0:F8}", vdc);
                    throw new Exception(msg);
                }
            }

            _meter.CloseSerialPort();

            msg = string.Format("Meter VAC = {0:F8}.  VDC  = {1:F8}.", vac, vdc);
            updateOutputStatus(msg);

            return vac;
        }

        /// <summary>
        /// Verifies the dc voltage is within limits
        /// </summary>
        /// <param name="voltage_dc_low_limit"></param>
        /// <param name="voltage_dc_high_limit"></param>
        void verify_voltage_dc(double voltage_dc_low_limit, double voltage_dc_high_limit)
        {
            if (_meter == null)
                return;

            if (_relay_ctrl != null && _relay_ctrl.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, true);  // DC

            updateOutputStatus("Verify Voltage DC");
            _meter.OpenComPort();
            _meter.SetToRemote();
            _meter.ClearError();
            _meter.SetupForVDC();

            string meter_voltage_str = _meter.Measure();
            double meter_voltage_dc = Double.Parse(meter_voltage_str);
            _meter.SetupForVAC();
            meter_voltage_str = _meter.Measure();
            double meter_voltage_ac = Double.Parse(meter_voltage_str);

            string msg = string.Format("Meter DC Voltage at {0:F8} V.  AC {1:F8}", meter_voltage_dc, meter_voltage_ac);
            updateOutputStatus(msg);

            _meter.CloseSerialPort();

            if (meter_voltage_ac >= 1.0)
            {
                msg = string.Format("AC volatge detected at {0:F8}, DC Voltage {1:F8}", meter_voltage_ac, meter_voltage_dc);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }

            if (meter_voltage_dc < voltage_dc_low_limit || meter_voltage_dc > voltage_dc_high_limit)
            {
                msg = string.Format("Volatge DC is not within limits values: {0:F8} < {1:F8} < {2:F8}", voltage_dc_low_limit, meter_voltage_dc, voltage_dc_high_limit);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Gets the selected board type
        /// </summary>
        /// <returns></returns>
        BoardTypes getSelectedBoardType()
        {
            return (BoardTypes)Enum.Parse(typeof(BoardTypes), comboBoxBoardTypes.Text);
        }

        /// <summary>
        /// Run Calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonClick_Calibrate(object sender, EventArgs e)
        {
            // Clear the error
            _calibration_error_msg = null;

            // If we are not coding after check enablement
            if (!_calibrate_after_code)
            {
                if (!this.buttonCalibrate.Enabled) return;

                // Just in case we look for an invalid calibrate re-run too fast
                if (_stopwatch_idel.Elapsed.TotalMilliseconds < 500)
                {
                    _stopwatch_idel.Restart();
                    return;
                }
            }

            // Disable the app
            setEnablement(false, false);

            // Start the running stopwatch
            _stopwatch_idel.Reset();
            _stopwatch_running.Restart();

            // Init the status text box
            runStatus_Init();
            setRunStatusText("Start Calibration");

            // If we are not coding after
            // Clear output status
            if (!_calibrate_after_code)
                textBoxOutputStatus.Clear();

            // Clear toolstrip
            toolStripStatusLabel.Text = "";
            statusStrip1.Update();

            updateOutputStatus(
                "=============================Start Calibration============================");

            // Start Calibration
            try
            {
                // Init the meter object
                if (Properties.Settings.Default.Meter_Manual_Measurement)
                    _meter = null;
                else
                    _meter = new MultiMeter(Properties.Settings.Default.Meter_COM_Port_Name);

                // Check to see if Ember is to be used as USB and open ISA channel if so
                // Also set the box address
                Ember.Interfaces ember_interface = (Ember.Interfaces)Enum.Parse(
                    typeof(Ember.Interfaces), Properties.Settings.Default.Ember_Interface);
                _ember.Interface = ember_interface;
                if (_ember.Interface == Ember.Interfaces.USB)
                {
                    _ember.Interface_Address = Properties.Settings.Default.Ember_Interface_USB_Address;
                    TraceLogger.Log("Start Ember isachan");
                    _ember.OpenISAChannels();
                }
                else
                {
                    _ember.Interface_Address = Properties.Settings.Default.Ember_Interface_IP_Address;
                }


                // Create a new telnet connection
                TraceLogger.Log("Start telnet");
                // If interface is USB we use localhost
                string telnet_address = "localhost";
                if (_ember.Interface == Ember.Interfaces.IP)
                    telnet_address = _ember.Interface_Address;
                _telnet_connection = new TelnetConnection(telnet_address, 4900);

                // Init the calibration object board type
                _calibrate.BoardType = getSelectedBoardType();

                // Open the relay controller and set the for coding
                // Note, relay controller stays opened until task is done
                _relay_ctrl.Open();
                if (!_calibrate_after_code)
                {
                    if (_relay_ctrl.Device_Type == RelayControler.Device_Types.Manual)
                    {
                        // We don't check dc in manual mode, so just connect the ember and power
                        _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                        _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                        _relay_ctrl.WriteLine(Relay_Lines.Load, true);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        // Check dc before connecting ember
                        _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
                        _relay_ctrl.WriteLine(Relay_Lines.Load, false);
                        _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                        Thread.Sleep(1000);

                        verify_voltage_dc(
                            _calibrate.Voltage_DC_Low_Limit, _calibrate.Voltage_DC_High_Limit);

                        // Should be safe to connect Ember
                        _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                        Thread.Sleep(1000);
                    }
                    relaysSet();
                }
                else
                {
                    _relay_ctrl.WriteLine(Relay_Lines.Power, false);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    Thread.Sleep(1000);
                }

                //clear calibrate after code
                _calibrate_after_code = false;

                // Finish setting up the calibrate object
                _calibrate.Ember = _ember;
                _calibrate.RelayController = _relay_ctrl;
                _calibrate.TelnetConnection = _telnet_connection;
                _calibrate.MultiMeter = _meter;

                // Run the calibration
                Task task_calibrate = new Task(_calibrate.Run);
                task_calibrate.ContinueWith(
                    calibrate_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
                task_calibrate.ContinueWith(
                    calibration_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);
                task_calibrate.Start();

            }
            catch (Exception ex)
            {
                _calibration_error_msg = ex.Message;
                calibration_done();
            }
        }

        /// <summary>
        /// Calibration done handler
        /// </summary>
        void calibration_done()
        {
            // Trun power off
            if (_relay_ctrl != null)
            {
                _relay_ctrl.WriteLine(PowerCalibration.Relay_Lines.Ember, false);
                _relay_ctrl.WriteLine(PowerCalibration.Relay_Lines.Power, false);
                _relay_ctrl.WriteLine(PowerCalibration.Relay_Lines.Load, false);
                _relay_ctrl.Close();
            }

            // Wait for power off
            if (_meter != null)
            {
                _meter.CloseSerialPort();
                wait_for_power_off();
            }

            // Check PASS or FAIL
            if (_calibration_error_msg == null)
            {
                updateRunStatus("PASS", Color.White, Color.Green);

            }
            else
            {
                relaysSet();
                updateRunStatus("FAIL", Color.White, Color.Red);
                updateOutputStatus(_calibration_error_msg);
            }

            TraceLogger.Log("Close Ember isachan");
            if (_ember.Interface == Ember.Interfaces.USB)
                _ember.CloseISAChannels();

            if (_telnet_connection != null)
            {
                TraceLogger.Log("Close telnet");
                _telnet_connection.Close();
            }

            // Stop runing watch and report time lapse
            _stopwatch_running.Stop();
            TimeSpan ts = _stopwatch_running.Elapsed;
            string elapsedTime = String.Format("Elaspsed time {0:00} seconds", ts.TotalSeconds);
            updateOutputStatus(elapsedTime);

            _stopwatch_running.Reset();
            _stopwatch_idel.Restart();

            // Reset the calibrate after coding
            _calibrate_after_code = false;

            setEnablement(true, false);

            updateOutputStatus(
                "==============================End Calibration=============================");

        }

        /// <summary>
        /// Handels when calibration is done
        /// </summary>
        /// <param name="task"></param>
        void calibration_done_handler(Task task)
        {
            calibration_done();
        }

        /// <summary>
        /// Handlels when calibration throws an error
        /// </summary>
        /// <param name="task"></param>
        void calibrate_exception_handler(Task task)
        {
            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;

            _calibration_error_msg = errmsg;
            calibration_done();
        }

        /// <summary>
        /// Handels relay controller event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="relay_controller"></param>
        void calibrate_Relay_Event(object sender, RelayControler relay_controller)
        {
            relaysSet();
        }

        /// <summary>
        /// Handel calibration status events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status_txt"></param>
        void calibrate_Status_event(object sender, string status_txt)
        {
            updateOutputStatus(status_txt);
        }

        /// <summary>
        /// Handels calibration run status events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status_txt"></param>
        void calibrate_Run_Status_Event(object sender, string status_txt)
        {
            updateRunStatus(status_txt);
        }

        /// <summary>
        /// Updates GUI with info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Update_Idle_Tick(object sender, EventArgs e)
        {
            if (_stopwatch_running.Elapsed.Ticks > 0)
            {
                toolStripStatusLabel.Text = string.Format("Running {0:dd\\.hh\\:mm\\:ss}", _stopwatch_running.Elapsed);
            }
            else if (_stopwatch_idel.Elapsed.Ticks > 0)
            {
                toolStripStatusLabel.Text = string.Format("Idel {0:dd\\.hh\\:mm\\:ss}", _stopwatch_idel.Elapsed);
            }
        }

        /// <summary>
        /// Handels when the form is shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Form_Main_Shown(object sender, EventArgs e)
        {
            // Init relay controller
            initRelayController();

            ///buttonCalibrate.Focus();
            buttonAll.Focus();
        }

        /// <summary>
        /// Experiment to control app from external app
        /// </summary>
        /// <param name="boardtype"></param>
        public void Calibrate(string boardtype)
        {
            MessageBox.Show(boardtype);
        }

        /// <summary>
        /// Starts the Coding task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonClick_Code(object sender, EventArgs e)
        {
            // Clear the error
            _coding_error_msg = null;

            // Just in case
            if (!this.buttonCode.Enabled)
                return;

            // Just in case we look for an invalid calibrate re-run too fast
            if (_stopwatch_idel.Elapsed.TotalMilliseconds < 500)
            {
                _stopwatch_idel.Restart();
                return;
            }

            // If the button is labeled "Cancel" then just cancel the task
            if (buttonCode.Text == "&Cancel")
            {
                _coding_token_src_cancel.Cancel();
                return;
            }

            // Disable the app
            setEnablement(false, true);

            // Start the running stopwatch
            _stopwatch_idel.Reset();
            _stopwatch_running.Restart();

            // Init the status text box
            runStatus_Init();
            setRunStatusText("Start Coding");

            // Clear output status
            this.textBoxOutputStatus.Clear();

            // Clear toolstrip
            toolStripStatusLabel.Text = "";
            statusStrip1.Update();

            // Start coding
            updateOutputStatus(
                "===============================Start Coding==============================");
            try
            {
                // Init the meter object
                if (Properties.Settings.Default.Meter_Manual_Measurement)
                    _meter = null;
                else
                    _meter = new MultiMeter(Properties.Settings.Default.Meter_COM_Port_Name);

                // Set the XY poit where to check coding status color
                Coder coder = new Coder(new TimeSpan(0, 2, 0));

                // Init the calibrate object so we can get dv voltage limits
                _calibrate.BoardType = getSelectedBoardType();

                // Open the relay controller and set the for coding
                // Note, relay controller stays opened until task is done
                _relay_ctrl.Open();
                if (_relay_ctrl.Device_Type == RelayControler.Device_Types.Manual)
                {
                    // We don't check dc in manual mode, so just connect the ember and power
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    Thread.Sleep(1000);
                }
                else
                {
                    // Check dc before connecting ember
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    Thread.Sleep(1000);

                    verify_voltage_dc(
                        _calibrate.Voltage_DC_Low_Limit, _calibrate.Voltage_DC_High_Limit);

                    // Should be safe to connect Ember
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                    Thread.Sleep(1000);
                }
                relaysSet();

                // Run coding
                _coding_token_src_cancel = new CancellationTokenSource();
                CancellationToken token = _coding_token_src_cancel.Token;
                Task task = new Task(() => coder.Code(token), token);
                task.ContinueWith(coding_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
                task.ContinueWith(coding_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);
                task.Start();
            }
            catch (Exception ex)
            {
                _coding_error_msg = ex.Message;
                coding_done();
            }
        }

        /// <summary>
        /// Coding done handler
        /// </summary>
        void coding_done()
        {
            // reactivate the window
            activate();

            // Check whether PASS, Cancelled or FAIL
            if (_coding_token_src_cancel.IsCancellationRequested)
            {
                _coding_token_src_cancel = new CancellationTokenSource();
                _calibrate_after_code = false;
                updateRunStatus("Cancelled", Color.Black, Color.Yellow);
            }
            else
            {

                if (_coding_error_msg == null)
                {
                    updateRunStatus("PASS", Color.White, Color.Green);
                }
                else
                {
                    _calibrate_after_code = false;
                    updateRunStatus("FAIL", Color.White, Color.Red);
                    updateOutputStatus(_coding_error_msg);
                }
            }

            // Turn power off if we are not calibrating after this or
            // there was a coding problem
            if (!_calibrate_after_code || _coding_error_msg != null)
            {
                if (_relay_ctrl != null)
                {
                    _relay_ctrl.WriteLine(PowerCalibration.Relay_Lines.Ember, false);
                    _relay_ctrl.WriteLine(PowerCalibration.Relay_Lines.Power, false);
                    _relay_ctrl.Close();
                }
                relaysSet();

                if (_meter != null)
                {
                    _meter.CloseSerialPort();
                    wait_for_power_off();
                }
            }

            // Stop runing watch and report time lapse
            _stopwatch_running.Stop();
            TimeSpan ts = _stopwatch_running.Elapsed;
            string elapsedTime = String.Format("Elaspsed time {0:00} seconds", ts.TotalSeconds);
            updateOutputStatus(elapsedTime);

            _stopwatch_running.Reset();
            _stopwatch_idel.Restart();

            setEnablement(true, true);

            updateOutputStatus(
                "===============================End Coding==============================");

            // Run calibration if eberything is OK
            if (_calibrate_after_code && _coding_error_msg == null &&
                !_coding_token_src_cancel.IsCancellationRequested)
            {
                clickCalibrate();
            }
        }

        /// <summary>
        /// Activates the form
        /// </summary>
        void activate()
        {
            if (InvokeRequired)
            {
                activateCallback d = new activateCallback(activate);
                this.Invoke(d);
            }
            else
            {
                Activate();
            }
        }

        /// <summary>
        /// Clickes the calibrate button
        /// </summary>
        void clickCalibrate()
        {
            if (buttonCalibrate.InvokeRequired)
            {
                clickCalibrateCallback d = new clickCalibrateCallback(clickCalibrate);
                this.Invoke(d);
            }
            else
            {
                buttonCalibrate.PerformClick();
            }
        }

        /// <summary>
        /// Coding done handler
        /// </summary>
        /// <param name="task"></param>
        void coding_done_handler(Task task)
        {
            bool canceled = task.IsCanceled;
            var exception = task.Exception;

            coding_done();
        }

        /// <summary>
        /// Coding error handler
        /// </summary>
        /// <param name="task"></param>
        void coding_exception_handler(Task task)
        {
            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;
            _coding_error_msg = errmsg;

            coding_done();
        }

        /// <summary>
        /// Runs Code + calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonClick_All(object sender, EventArgs e)
        {
            _calibrate_after_code = true;
            buttonClick_Code(sender, e);
        }


    }


    /// <summary>
    /// Defines the relay lines
    /// </summary>
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
