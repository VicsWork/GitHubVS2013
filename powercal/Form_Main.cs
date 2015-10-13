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
        Ember _ember;
        Calibrate _calibrate = new Calibrate();

        Stopwatch _stopwatch_running = new Stopwatch();
        Stopwatch _stopwatch_stopped = new Stopwatch();

        string _calibration_error_msg = null;

        /// <summary>
        /// The app folder where we save most logs, etc
        /// </summary>
        static string _app_data_dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration");
        /// <summary>
        /// Path to the app log file
        /// </summary>
        string _log_file = Path.Combine(_app_data_dir, "runlog.txt");

        /// <summary>
        /// Path to where the Ember programing batch file is created
        /// </summary>
        string _ember_batchfile_path = Path.Combine(_app_data_dir, "patchit.bat");

        delegate void clickCalibrateCallback();
        delegate void activateCallback();
        delegate void setTextCallback(string txt);
        delegate void setTextColorCallback(string txt, Color forecolor, Color backcolor);
        delegate void setEnablementCallback(bool enable, bool isCoding);
        delegate void updateCallback();

        Form_PowerMeter _power_meter_dlg = null;

        /// <summary>
        /// Coding
        /// </summary>
        Point _coding_state_color_point;
        string _coding_error_msg;
        CancellationTokenSource _coding_token_src_cancel = new CancellationTokenSource();

        bool _calibrate_after_code = false;


        /// <summary>
        /// The main form constructor
        /// </summary>
        public Form_Main()
        {
            Stream outResultsFile = File.Create("output.txt");
            var textListener = new TextWriterTraceListener(outResultsFile);
            Trace.Listeners.Add(textListener);

            InitializeComponent();

            _stopwatch_running.Reset();
            _stopwatch_stopped.Start();

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
            RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(
                typeof(RelayControler.Device_Types), Properties.Settings.Default.Relay_Controller_Type);
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
            Dictionary<string, uint> dic = _relay_ctrl.DicLines_ReadSettings();
            if (dic.Count == 0)
            {
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Power, 0);
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Load, 1);
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Ember, 2);
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Voltmeter, 3);
                _relay_ctrl.DicLines_SaveSettings();
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

            _ember = new Ember();
            _ember.Process_ISAChan_Error_Event += p_ember_isachan_ErrorDataReceived;
            _ember.Process_ISAChan_Output_Event += p_ember_isachan_OutputDataReceived;

            _calibrate.Status_Event += calibrate_Status_event;
            _calibrate.Run_Status_Event += calibrate_Run_Status_Event;
            _calibrate.Relay_Event += calibrate_Relay_Event;

            if (Properties.Settings.Default.Coding_StatusX == 0 && Properties.Settings.Default.Coding_StatusY == 0)
            {
                buttonCode.Enabled = false;
            }
            else
            {
                buttonCode.Enabled = true;
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

                if (!enable && isCoding)
                {
                    buttonCode.Text = "&Cancel";
                    buttonCode.Enabled = true;
                }
                else
                {
                    buttonCode.Text = "&Code";
                    buttonCode.Enabled = enable;
                }

                //if (buttonCalibrate.Enabled)
                //{
                //    UseWaitCursor = false;
                //    this.Cursor = this.DefaultCursor;
                //}
                //else
                //{
                //    this.UseWaitCursor = true;
                //}

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
                setTextCallback d = new setTextCallback(setRunStatusText);
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

        void toolStripMenuItem_Click_Clear(object sender, EventArgs e)
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
        void toolStripMenuItem_Click_Settings(object sender, EventArgs e)
        {
            Form_Settings dlg = new Form_Settings();

            // DIO line assigment
            Dictionary<string, uint> relay_lines = _relay_ctrl.DicLines_ReadSettings();
            dlg.NumericUpDown_ACPower.Value = relay_lines[powercal.Relay_Lines.Power];
            dlg.NumericUpDown_Load.Value = relay_lines[powercal.Relay_Lines.Load];
            dlg.NumericUpDown_Ember.Value = relay_lines[powercal.Relay_Lines.Ember];
            dlg.numericUpDown_Voltmeter.Value = relay_lines[powercal.Relay_Lines.Voltmeter];

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
                relay_lines[powercal.Relay_Lines.Power] = (uint)dlg.NumericUpDown_ACPower.Value;
                relay_lines[powercal.Relay_Lines.Load] = (uint)dlg.NumericUpDown_Load.Value;
                relay_lines[powercal.Relay_Lines.Ember] = (uint)dlg.NumericUpDown_Ember.Value;
                relay_lines[powercal.Relay_Lines.Voltmeter] = (uint)dlg.numericUpDown_Voltmeter.Value;
                _relay_ctrl.DicLines_SaveSettings();

                // Ember
                Properties.Settings.Default.Ember_Interface = dlg.comboBoxEmberInterface.Text;
                Properties.Settings.Default.Ember_BinPath = dlg.TextBoxEmberBinPath.Text;
                if (dlg.comboBoxEmberInterface.Text == "IP")
                    Properties.Settings.Default.Ember_Interface_IP_Address = dlg.textBoxEmberInterfaceAddress.Text;
                else
                    Properties.Settings.Default.Ember_Interface_USB_Address = dlg.textBoxEmberInterfaceAddress.Text;

                Properties.Settings.Default.Save();

                if (Properties.Settings.Default.Coding_StatusX == 0 && Properties.Settings.Default.Coding_StatusY == 0)
                {
                    buttonCode.Enabled = false;
                }
                else
                {
                    buttonCode.Enabled = true;
                }

            }
            else
            {
                Properties.Settings.Default.Reload();
            }
        }

        void toolStripMenuItem_Click_Calculator(object sender, EventArgs e)
        {
            Form_Calculator dlg = new Form_Calculator();
            dlg.ShowDialog();
        }

        void toolStripMenuItem_Click_PowerMeter(object sender, EventArgs e)
        {
            if (_power_meter_dlg == null)
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

                _power_meter_dlg = new Form_PowerMeter(
                    ember_interface, ember_address,
                    calibrate.Voltage_Referencer, calibrate.Current_Referencer);

                _power_meter_dlg.FormClosed += power_meter_dlg_FormClosed;

                //_power_meter_dlg.Show();
                _power_meter_dlg.ShowDialog();
            }
            else
            {
                _power_meter_dlg.BringToFront();
            }
        }

        void power_meter_dlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            _power_meter_dlg = null;

            _relay_ctrl.Open();
            _relay_ctrl.WriteLine(Relay_Lines.Power, false);
            _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
            _relay_ctrl.WriteLine(Relay_Lines.Load, false);
            _relay_ctrl.Close();

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
        void toolStripMenuItem_Click_NI(object sender, EventArgs e)
        {
            Form_NIDigitalPortTest dlg = new Form_NIDigitalPortTest();
            dlg.Show();
        }

        /// <summary>
        /// Invokes the FTDI test dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_Click_FT232H(object sender, EventArgs e)
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
        void comboBoxBoardTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Last_Used_Board = this.comboBoxBoardTypes.Text;
            Properties.Settings.Default.Save();

            updateRunStatus("Ready for " + comboBoxBoardTypes.Text);
        }

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

            _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, false);
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

            _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, true);
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

            msg = string.Format("Meter VAC = {0:F8}.  VDC  = {0:F8}.", vac, vdc);
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

            if (!_calibrate_after_code)
            {
                if (!this.buttonCalibrate.Enabled)
                    return;

                // Just in case we look for an invalid calibrate re-run too fast
                if (_stopwatch_stopped.Elapsed.TotalMilliseconds < 500)
                {
                    _stopwatch_stopped.Restart();
                    return;
                }
            }

            // Disable the app
            setEnablement(false, false);

            _stopwatch_running.Restart();

            runStatus_Init();
            setRunStatusText("Start Calibration");

            if (!_calibrate_after_code)
                textBoxOutputStatus.Clear();

            toolStripStatusLabel.Text = "";
            statusStrip1.Update();

            updateOutputStatus("===============================Start Calibration==============================");

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

                Ember.Interfaces ember_interface = (Ember.Interfaces)Enum.Parse(typeof(Ember.Interfaces), Properties.Settings.Default.Ember_Interface);
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

                string telnet_address = "localhost";
                if (_ember.Interface == Ember.Interfaces.IP)
                    telnet_address = _ember.Interface_Address;
                _telnet_connection = new TelnetConnection(telnet_address, 4900);

                _calibrate.BoardType = getSelectedBoardType();

                _relay_ctrl.Open();
                if (_relay_ctrl.Device_Type == RelayControler.Device_Types.Manual)
                {
                    // Trun AC ON
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                    _relay_ctrl.WriteLine(Relay_Lines.Load, false);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    _relay_ctrl.WriteLine(Relay_Lines.Load, true);
                    Thread.Sleep(2000);
                }
                else
                {
                    _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, false);
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
                    _relay_ctrl.WriteLine(Relay_Lines.Load, false);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, true);

                    Thread.Sleep(1000);

                    verify_voltage_dc(_calibrate.Voltage_DC_Low_Limit, _calibrate.Voltage_DC_High_Limit);

                    _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, false);

                    // Should be safe to connect Ember
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                    Thread.Sleep(1000);
                }
                relaysSet();

                _calibrate.RelayController = _relay_ctrl;
                _calibrate.TelnetConnection = _telnet_connection;
                _calibrate.MultiMeter = _meter;

                Task task_calibrate = new Task(_calibrate.Run);
                task_calibrate.ContinueWith(calibrate_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
                task_calibrate.ContinueWith(calibration_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);
                task_calibrate.Start();

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
                wait_for_power_off();
            }

            if (_calibration_error_msg == null)
            {
                updateRunStatus("PASS", Color.White, Color.Green);

            }
            else
            {
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

            _stopwatch_running.Stop();
            TimeSpan ts = _stopwatch_running.Elapsed;
            string elapsedTime = String.Format("Elaspsed time {0:00} seconds", ts.TotalSeconds);
            updateOutputStatus(elapsedTime);

            _stopwatch_running.Reset();
            _stopwatch_stopped.Restart();

            _calibrate_after_code = false;

            setEnablement(true, false);

            updateOutputStatus(
                "================================End Calibration===============================");

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

        void timer_Update_Idle_Tick(object sender, EventArgs e)
        {
            if (_stopwatch_running.Elapsed.Ticks > 0)
            {
                this.toolStripStatusLabel.Text = string.Format("Running {0:dd\\.hh\\:mm\\:ss}", _stopwatch_running.Elapsed);
            }
            else if (_stopwatch_stopped.Elapsed.Ticks > 0)
            {
                this.toolStripStatusLabel.Text = string.Format("Idel {0:dd\\.hh\\:mm\\:ss}", _stopwatch_stopped.Elapsed);
            }
        }

        void Form_Main_Shown(object sender, EventArgs e)
        {
            buttonCalibrate.Focus();
        }

        public void Calibrate(string boardtype)
        {
            MessageBox.Show(boardtype);
        }

        void buttonClick_Code(object sender, EventArgs e)
        {
            if (buttonCode.Text == "&Cancel")
            {
                _coding_token_src_cancel.Cancel();
                return;
            }

            // Disable the app
            setEnablement(false, true);

            _stopwatch_running.Restart();

            runStatus_Init();
            setRunStatusText("Start Coding");

            this.textBoxOutputStatus.Clear();

            toolStripStatusLabel.Text = "";
            statusStrip1.Update();


            updateOutputStatus("===============================Start Coding==============================");

            _coding_error_msg = null;

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


                _coding_state_color_point.X = Properties.Settings.Default.Coding_StatusX;
                _coding_state_color_point.Y = Properties.Settings.Default.Coding_StatusY;

                Coder coder = new Coder(_coding_state_color_point, new TimeSpan(0, 2, 0));

                _calibrate.BoardType = getSelectedBoardType();
                _relay_ctrl.Open();
                if (_relay_ctrl.Device_Type == RelayControler.Device_Types.Manual)
                {
                    // Trun AC ON
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    Thread.Sleep(1000);
                }
                else
                {
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, true);

                    Thread.Sleep(1000);

                    verify_voltage_dc(_calibrate.Voltage_DC_Low_Limit, _calibrate.Voltage_DC_High_Limit);


                    // Should be safe to connect Ember
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                    Thread.Sleep(1000);
                }
                _relay_ctrl.Close();
                relaysSet();


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

        void coding_done()
        {
            if (_relay_ctrl != null)
            {
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Ember, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, false);
                _relay_ctrl.Close();
            }
            relaysSet();

            if (_meter != null)
            {
                _meter.CloseSerialPort();
                wait_for_power_off();
            }


            if (_coding_token_src_cancel.IsCancellationRequested)
            {
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

            _stopwatch_running.Stop();
            TimeSpan ts = _stopwatch_running.Elapsed;
            string elapsedTime = String.Format("Elaspsed time {0:00} seconds", ts.TotalSeconds);
            updateOutputStatus(elapsedTime);

            _stopwatch_running.Reset();
            _stopwatch_stopped.Restart();

            setEnablement(true, true);

            updateOutputStatus(
                "===============================End Coding==============================");

            if (_calibrate_after_code && _coding_error_msg == null &&
                !_coding_token_src_cancel.IsCancellationRequested)
            {
                clickCalibrate();
            }

            _coding_token_src_cancel = new CancellationTokenSource();
            activate();
        }

        void activate()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
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

        void clickCalibrate()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
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

        void coding_done_handler(Task task)
        {
            bool canceled = task.IsCanceled;
            var exception = task.Exception;

            coding_done();
        }

        void coding_exception_handler(Task task)
        {
            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;
            _coding_error_msg = errmsg;

            coding_done();
        }

        void buttonClick_All(object sender, EventArgs e)
        {
            _calibrate_after_code = true;
            buttonClick_Code(sender, e);
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
