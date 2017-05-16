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
using System.Reflection;

using System.Net;

using System.Runtime.Serialization;
using System.Xml;

using System.ServiceModel;
using System.ServiceModel.Description;

using MinimalisticTelnet;

using System.Data.SqlClient;
using System.Data.SqlTypes;

using System.Media;

using CentraliteDataUtils;

namespace PowerCalibration
{
    enum BoardTypes { Mahi, Halibut, Humpback, Honeycomb, Hornshark, Mudshark, Hooktooth, Milkshark, Zebrashark };

    public partial class Form_Main : Form, ICalibrationService
    {
        enum TaskTypes { Pretest, Code, Test, Calibrate, Recode, None };
        enum Coding_Method { EBL, ISA_UTIL };

        MultiMeter _meter = null; // The multimeter controller
        RelayControler _relay_ctrl; // The relay controller
        TelnetConnection _telnet_connection; // Telnet connection to ISA3 Adapter
        Ember _ember; // The Ember box
        Stopwatch _stopwatch_running = new Stopwatch();  // Used to measure running tasks
        Stopwatch _stopwatch_idel = new Stopwatch();  // Used to measure idle
        string _calibration_error_msg = null;  //  If set this will indicate the calibration error
        string _coding_error_msg;  //  If set this will indicate the coding error
        string _pretest_error_msg;  //  If set this will indicate the pretest error
        string _test_error_msg;  //  If set this will indicate error for tests usually run after calibration
        string _mfg_str;  // Holds the manufacturing string
        Coding_Method _coding_method = Coding_Method.EBL;


        Task _task_uut;
        CancellationTokenSource _cancel_token_uut = new CancellationTokenSource();  // Used to cancel coding

        enum Sounds { PASS, FAIL };

        static string _app_data_dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration"); //The app folder where we save most logs, etc
        string _log_file = Path.Combine(_app_data_dir, "runlog.txt"); // Path to the app log file
        string _ember_batchfile_path = Path.Combine(_app_data_dir, "patchit.bat"); // Path to where the Ember programming batch file is created

        delegate void clickCalibrateCallback();  // Clicks Calibrate
        delegate void activateCallback();  // Activates the form
        delegate void setTextCallback(string txt);  // Set object text
        delegate void setControlPropertyValueCallback(Control control, object value, string property_name);  // Set object text
        delegate void setTextColorCallback(string txt, Color forecolor, Color backcolor); // Set objects color
        delegate void setEnablementCallback(bool enable, bool isCoding);  // Set enablement
        delegate BoardTypes getSelectedBoardTypeCallback();

        SqlConnectionStringBuilder _db_connect_str;
        bool _db_Loging = true; // use to enable/disable db logging
        Task _task_updatedb;
        DataTable _datatable_calibrate;
        uint _db_total_written = 0;

        bool _supervisor_mode = false;  // Use to enable/hide features

        bool _running_all = true;  // Indicates the run button was pressed;

        /// <summary>
        /// The main form constructor
        /// </summary>
        public Form_Main()
        {
            // Init the trace listener
            try
            {
                Stream outResultsFile = File.Create("output.txt");
                var textListener = new TextWriterTraceListener(outResultsFile);
                Trace.Listeners.Add(textListener);
            }
            catch (System.IO.IOException)
            {
                // Most likely we are already running
                MessageBox.Show("Unable to open trace listener.\r\n\"" + this.assemblyTitle + "\" may already be running?\r\nExiting...", this.assemblyTitle);
                this.Close();
            }

            InitializeComponent();

            CentraliteDataUtils.DataUtils.DBConnStr = Properties.Settings.Default.DBConnectionString;

            // Create the app data folder
            if (!Directory.Exists(_app_data_dir))
            {
                Directory.CreateDirectory(_app_data_dir);
            }
            // Init the log file
            initLogFile();

        }


        private void Form_Main_Load(object sender, EventArgs e)
        {

            // Load the app icon
            Icon = Properties.Resources.Icon_PowerCalibration;

            // Set the title to match assembly info from About dlg
            this.Text = this.assemblyTitle;

            // Init the stop watches
            _stopwatch_running.Reset();
            _stopwatch_idel.Start();


            // Always reset this settings as they are important to forget leaving them in a different state
            Properties.Settings.Default.Ember_ReadProtect_Enabled = true;
            Properties.Settings.Default.PrePost_Test_Enabled = true;
            Properties.Settings.Default.Save();

            // Init the status text box
            runStatus_Init();

            // Make sure we have a selection for board types
            this.comboBoxBoardTypes.Items.AddRange(Enum.GetNames(typeof(PowerCalibration.BoardTypes)));
            int index = comboBoxBoardTypes.Items.IndexOf(Properties.Settings.Default.Last_Used_Board);
            if (index < 0)
                index = 0;
            if (comboBoxBoardTypes.Items.Count > 0)
                comboBoxBoardTypes.SelectedIndex = index;

            string msg = "";
            // Report COM ports found in system
            string[] ports = SerialPort.GetPortNames();
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
            _ember.BatchFilePatchPath = Path.Combine(_app_data_dir, "patchit.bat");
            _ember.Process_ISAChan_Error_Event += p_ember_isachan_ErrorDataReceived;
            _ember.Process_ISAChan_Output_Event += p_ember_isachan_OutputDataReceived;


            // Init the db connection string
            _db_connect_str = new SqlConnectionStringBuilder(Properties.Settings.Default.DBConnectionString);
            _db_Loging = Properties.Settings.Default.DB_Loging_Enabled;
            if (isDBLogingEnabled())
            {
                // Create the internal result data table
                createResultTable();
                updateOutputStatus("DB logging enabled");
            }
            else
            {
                updateOutputStatus("DB logging disabled");
            }

            //CalibrationResultsEventArgs e = new CalibrationResultsEventArgs();
            //e.Current_gain = 1;
            //e.Voltage_gain = 2;
            //e.timestamp = DateTime.Now;
            //calibrationResults_Event(this, e);

            // Set the coding method to use;
            _coding_method = (Coding_Method)Enum.Parse(typeof(Coding_Method), 
                Properties.Settings.Default.Coding_Method);
            updateOutputStatus("Coding method set to " + _coding_method.ToString());
            if(_coding_method == Coding_Method.ISA_UTIL)
                updateOutputStatus("Coding file set to " + Properties.Settings.Default.Coding_File);


            // Enable the app
            setEnablement(true, false);

        }

        /// <summary>
        /// Shows the buttons depending on the space bar shortcut setting
        /// </summary>
        void setButtonsVisible()
        {
            this.buttonRun.Visible = true;

            if (_supervisor_mode)
            {
                this.buttonPreTest.Visible = true;
                this.buttonCode.Visible = true;
                this.buttonCalibrate.Visible = true;
                this.buttonTest.Visible = true;
                this.buttonRecode.Visible = true;
                this.buttonPreTest.Visible = true;

            }
            else
            {
                this.buttonCode.Visible = false;
                this.buttonCalibrate.Visible = false;
                this.buttonTest.Visible = false;
                this.buttonRecode.Visible = false;
                this.buttonPreTest.Visible = false;
            }
        }

        /// <summary>
        /// Creates the internal data table to store results
        /// </summary>
        void createResultTable()
        {
            _datatable_calibrate = new DataTable("CalibrationResults");
            _datatable_calibrate.Columns.Add("VoltageGain", typeof(SqlInt32));
            _datatable_calibrate.Columns.Add("CurrentGain", typeof(SqlInt32));
            _datatable_calibrate.Columns.Add("Eui", typeof(string));
            _datatable_calibrate.Columns.Add("DateCalibrated", typeof(SqlDateTime));
        }

        /// <summary>
        /// Returns whether DB logging is disabled
        /// </summary>
        /// <returns></returns>
        bool isDBLogingEnabled()
        {
            return _db_Loging;
        }

        /// <summary>
        /// Save results to database
        /// </summary>
        void updateDB()
        {
            string msg = "";

            try
            {
                // Update result table with 
                //Utils.ConnectionSB = _db_connect_str;
                DataUtils.DBConnStr = Properties.Settings.Default.DBConnectionString;
                int machine_id = DataUtils.Machine_ID;
                if (machine_id >= 0)
                {
                    foreach (DataRow r in _datatable_calibrate.Rows)
                    {
                        string eui = (string)r["Eui"];
                        int eui_id = DataUtils.GetEUIID(eui);

                        using (SqlConnection con = new SqlConnection(_db_connect_str.ConnectionString))
                        {
                            con.Open();

                            using (SqlCommand cmd = new SqlCommand())
                            {
                                cmd.Connection = con;
                                string table_name = "[CalibrationResults]";

                                cmd.CommandText = string.Format(
                                    "insert into {0} (EuiId, VoltageGain, CurrentGain, DateCalibrated, MachineId) values ('{1}', '{2}', '{3}', '{4}', '{5}')",
                                    table_name, eui_id, r["VoltageGain"], r["CurrentGain"], r["DateCalibrated"], machine_id);

                                int n = cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    _db_total_written += (uint)_datatable_calibrate.Rows.Count;

                    msg = string.Format("{0}W/{1}T {2:H:mm:ss}",
                        _datatable_calibrate.Rows.Count, _db_total_written, DateTime.Now);

                    _datatable_calibrate.Rows.Clear();
                }

            }
            catch (Exception ex)
            {
                TraceLogger.Log("Database write error");
                TraceLogger.Log(ex.Message);
                msg = string.Format("{0}F/{1}T {2:H:mm:ss}",
                    _datatable_calibrate.Rows.Count, _db_total_written, DateTime.Now);
                TraceLogger.Log(msg);
            }

            try { toolStripGeneralStatusLabel.Text = msg; }
            catch { };
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


                // In this version we are adding a new lines for Honeycomb
                // So lets refresh the file as to not to have conflicts with older versions
                // Should not cause any problems unless someone rewired the relay controller
                _relay_ctrl.ClearDictionaries();
                _relay_ctrl.RecreateSettingsFile();

                _relay_ctrl.Open();
                initRelayController_Lines();
                msg = string.Format("Relay controller \"{0}:{1}\" ready.", rdevtype, _relay_ctrl.SerialNumber);
                updateOutputStatus(msg);
            }
            catch (Exception ex)
            {
                msg = string.Format("{0}\r\nTry unplugging and re-plugging the USB device.\r\nThen try to change relay controller in settings dialog", ex.Message);
                showDialogMsg(msg);

                msg = string.Format("Unable to init relay controller \"{0}\".  Switching to Manual relay mode", rdevtype);
                updateOutputStatus(msg);

                _relay_ctrl = new RelayControler(RelayControler.Device_Types.Manual);
                initRelayController_Lines();
                Properties.Settings.Default.Relay_Controller_Type = _relay_ctrl.Device_Type.ToString();
                Properties.Settings.Default.Save();

                Form_Settings dlg = new Form_Settings();
                dlg.TabControl.SelectedTab = dlg.TabControl.TabPages["tabPageDIO"];
                DialogResult rc = dlg.ShowDialog();
                if (rc == DialogResult.OK)
                {
                    // DIO controller type
                    Properties.Settings.Default.Relay_Controller_Type = dlg.comboBox_DIOCtrollerTypes.Text;
                    rdevtype = (RelayControler.Device_Types)Enum.Parse(typeof(RelayControler.Device_Types),
                        Properties.Settings.Default.Relay_Controller_Type);
                    _relay_ctrl = new RelayControler(rdevtype);
                }
                else
                {
                    Properties.Settings.Default.Reload();
                }

            }
            //_relay_ctrl.WriteAll(false);
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
                _relay_ctrl.DicLines_AddLine(Relay_Lines.Vac_Vdc, 3);

                _relay_ctrl.DicLines_SaveSettings();
            }
        }

        /// <summary>
        /// Detects whether the meter is ON and connected to one of the COM ports
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

                    Task<string> idntask = new Task<string>(()=>{ return meter.IDN(); });
                    idntask.Start();
                    string idn = "";
                    if (idntask.Wait(1000))
                    {
                        idn = idntask.Result;
                        meter.CloseSerialPort();
                    }

                    if (
                        idn.StartsWith("HEWLETT-PACKARD,34401A") ||
                        idn.StartsWith("GWInstek,GDM8341")
                        )
                    {
                        detected = true;
                        Properties.Settings.Default.Meter_COM_Port_Name = portname;

                        Properties.Settings.Default.Meter_Manual_Measurement = false;
                        Properties.Settings.Default.PrePost_Test_Enabled = true;

                        Properties.Settings.Default.Save();
                        string msg = string.Format("Multimeter '{0}' communications port auto detected at {1}", idn.TrimEnd('\n'),
                            Properties.Settings.Default.Meter_COM_Port_Name);
                        updateOutputStatus(msg);
                        break;
                    }

                }
                catch (Exception ex)
                {
                    string msgx = ex.Message;
                }

                Task closeport = new Task(()=> meter.CloseSerialPort());
                closeport.Start();
            }
            if (!detected)
            {
                string msg = string.Format("Unable to detect Multimeter communications port. Using {0}.  Measurements set to manual mode",
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
                buttonRun.Enabled = enable;
                buttonCalibrate.Enabled = enable;
                buttonRecode.Enabled = enable;
                buttonPreTest.Enabled = enable;

                comboBoxBoardTypes.Enabled = enable;
                menuStripMain.Enabled = enable;

                setTestButtonEnablement(enable);

                setCodeEnablement(enable, isCoding);

                setButtonsVisible();
            }
        }

        void setTestButtonEnablement(bool enable)
        {

            if (getSelectedBoardType() == BoardTypes.Honeycomb)
            {
                this.buttonTest.Enabled = enable;
            }
            else
            {
                this.buttonTest.Enabled = false;
            }
        }

        /// <summary>
        /// Sets the Code button control enablement and label
        /// This button is also to cancel the operation
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
                buttonRun.Enabled = enable;
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
        /// Shows a dialog with specified message
        /// </summary>
        /// <param name="msg"></param>
        void showDialogMsg(string msg)
        {
            if (this.InvokeRequired)
            {
                setTextCallback d = new setTextCallback(showDialogMsg);
                this.Invoke(d, new object[] { msg });
            }
            else
            {
                MessageBox.Show(msg);
            }
        }

        /// <summary>
        /// Sets the text property of any control as long as it has one
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        void controlSetText(Control control, object value, string property_name = "Text")
        {
            if (control.InvokeRequired)
            {
                setControlPropertyValueCallback d = new setControlPropertyValueCallback(controlSetText);
                this.Invoke(d, new object[] { control, value, property_name });
            }
            else
            {
                var property = control.GetType().GetProperty(property_name);
                if (property != null)
                {
                    property.SetValue(control, value);
                }
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
        void setRunStatus(string text)
        {
            controlSetText(textBoxRunStatus, text);
        }

        /// <summary>
        /// Sets the Run status text and background color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="forecolor"></param>
        /// <param name="backcolor"></param>
        void setRunStatus(string text, Color forecolor, Color backcolor)
        {
            if (this.textBoxRunStatus.InvokeRequired)
            {
                setTextColorCallback d = new setTextColorCallback(setRunStatus);
                this.Invoke(d, new object[] { text, forecolor, backcolor });
            }
            else
            {
                this.textBoxRunStatus.Text = text;
                this.textBoxRunStatus.ForeColor = forecolor;
                this.textBoxRunStatus.BackColor = backcolor;
                this.textBoxRunStatus.Update();

                if (text.StartsWith("FAIL"))
                    playSound(Sounds.FAIL);
            }
        }

        /// <summary>
        /// Plays a program sound
        /// </summary>
        /// <param name="sound"></param>
        void playSound(Sounds sound)
        {
            Task.Factory.StartNew(() => playsound(sound));
        }

        /// <summary>
        /// Updates the run status text box
        /// </summary>
        /// <param name="txt"></param>
        void updateRunStatus(string txt)
        {
            setRunStatus(txt);
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
            setRunStatus(txt, forecolor, backcolor);
            setOutputStatus(txt);
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
        /// Shows dialog with relays sates defined in the RelayControler when in manual mode
        /// </summary>
        /// <param name="relay_ctrl"></param>
        void relaysShowSetttings()
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

                showDialogMsg(msg_dlg);
            }
            string status = _relay_ctrl.ToStatusText();
            updateOutputStatus(status);
        }

        /// <summary>
        /// Logs the status of the relays
        /// </summary>
        void relay_log_status()
        {
            string status = _relay_ctrl.ToStatusText();
            updateOutputStatus(status);
        }

        /// <summary>
        /// Invokes Serial test dld
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
            AboutBox dlg = new AboutBox();
            DialogResult result = dlg.ShowDialog();
        }

        /// <summary>
        /// Clears the Output status text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_StatusClear(object sender, EventArgs e)
        {
            this.textBoxOutputStatus.Clear();
        }

        /// <summary>
        /// Copies the status selected text or all when nothing selected to
        /// the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_StatusCopy(object sender, EventArgs e)
        {
            try
            {
                string text = textBoxOutputStatus.SelectedText;
                if (text.Length == 0)
                    text = textBoxOutputStatus.Text;
                System.Windows.Forms.Clipboard.SetText(text);
            }
            catch (Exception ex)
            {
                // The clipboard settext  may fail when using vnc
                TraceLogger.Log("Exception in toolStripMenuItem_StatusCopy: " + ex.Message);
            }
        }

        /// <summary>
        /// Invokes the settings dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_Settings(object sender, EventArgs e)
        {
            Form_Settings dlg = new Form_Settings();
            DialogResult rc = dlg.ShowDialog();
            if (rc == DialogResult.OK)
            {
                // DIO controller type
                Properties.Settings.Default.Relay_Controller_Type = dlg.comboBox_DIOCtrollerTypes.Text;
                RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(typeof(RelayControler.Device_Types),
                    Properties.Settings.Default.Relay_Controller_Type);
                _relay_ctrl = new RelayControler(rdevtype);
            }
            else
            {
                Properties.Settings.Default.Reload();
            }

        }

        /// <summary>
        /// Opens up a basic calculator for 24bit register values conversions
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
                _relay_ctrl.OpenIfClosed();
                _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                _relay_ctrl.WriteLine(Relay_Lines.Load, true);
                Thread.Sleep(1000);

                Calibrate calibrate = new Calibrate();
                calibrate.BoardType = (BoardTypes)Enum.Parse(typeof(BoardTypes), comboBoxBoardTypes.Text);

                if (_telnet_connection == null || !_telnet_connection.IsConnected)
                {
                    createTelnet();
                }

                Form_PowerMeter power_meter_dlg = new Form_PowerMeter(_telnet_connection);
                power_meter_dlg.ShowDialog();
                closeTelnet();
                _relay_ctrl.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _relay_ctrl.OpenIfClosed();
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
        /// Invokes the FTDI test dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void toolStripMenuItem_FT232H(object sender, EventArgs e)
        {
            if (_relay_ctrl == null || _relay_ctrl.Device_Type != RelayControler.Device_Types.FT232H)
            {
                RelayControler.Device_Types rdevtype = (RelayControler.Device_Types)Enum.Parse(
                    typeof(RelayControler.Device_Types), Properties.Settings.Default.Relay_Controller_Type);
                _relay_ctrl = new RelayControler(rdevtype);
            }

            try
            {
                _relay_ctrl.OpenIfClosed();
                Form_FT232H_DIO_Test dlg = new Form_FT232H_DIO_Test(_relay_ctrl);
                dlg.ShowDialog();
            }
            catch
            {
                throw;
            }
            finally
            {
                _relay_ctrl.Close();
            }
        }

        /// <summary>
        /// Handles error data from the em3xx_load process
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
        /// Handles output data from the em3xx_load process
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

            setTestButtonEnablement(true);

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
                case (Keys.Control | Keys.D):
                    // Toggle supervisor mode
                    _supervisor_mode = !_supervisor_mode;
                    setButtonsVisible();
                    break;
                case Keys.Space:
                    if (buttonRun.Enabled)
                        buttonRun.PerformClick();
                    break;
                case Keys.L:
                    if (buttonCalibrate.Enabled)
                        buttonCalibrate.PerformClick();
                    break;
                case Keys.C:
                    if (buttonCode.Enabled)
                        buttonCode.PerformClick();
                    break;
                case Keys.R:
                    if (buttonRun.Enabled)
                        buttonRun.PerformClick();
                    break;

                // Power ON and DC meter
                case (Keys.Control | Keys.P):

                    Task task = new Task(() =>
                   {

                       updateOutputStatus("Turn power and DC measurement ON");
                       _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                       _relay_ctrl.WriteLine(Relay_Lines.Vac_Vdc, true);

                       updateOutputStatus("Setting meter");
                       if (_meter == null)
                       {
                           _meter = new MultiMeter(Properties.Settings.Default.Meter_COM_Port_Name);
                       }
                       else
                       {
                           _meter.CloseSerialPort();
                       }
                       _meter.Init();
                       _meter.SetupForVDC();
                       _meter.writeLine("TRIG:SOUR INT");
                       updateOutputStatus("Meter set");
                   }
                    );

                    task.Start();
                    break;

                // All off
                case (Keys.Control | Keys.O):
                    _relay_ctrl.WriteAll(false);
                    break;

                case (Keys.Control | Keys.E):
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                    break;
                case (Keys.Control | Keys.W):
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
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
            if (_relay_ctrl != null)
            {
                try
                {
                    _relay_ctrl.Open();
                    _relay_ctrl.WriteAll(false);
                }
                catch { }
                finally
                {
                    _relay_ctrl.Close();
                }
            }

            Trace.Flush();
            Trace.Close();
        }

        void initEmberIF()
        {
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

        }

        /// <summary>
        /// Creates telnet session using app settings
        /// </summary>
        TelnetConnection createTelnet()
        {
            initEmberIF();

            // Create a new telnet connection
            TraceLogger.Log("Start telnet");
            // If interface is USB we use localhost
            string telnet_address = "localhost";
            if (_ember.Interface == Ember.Interfaces.IP)
                telnet_address = _ember.Interface_Address;
            _telnet_connection = new TelnetConnection(telnet_address, 4900);

            return _telnet_connection;
        }

        /// <summary>
        /// Returns opened telnet
        /// </summary>
        /// <returns></returns>
        TelnetConnection openTelnet()
        {
            if (_telnet_connection == null)
                createTelnet();
            else if (!_telnet_connection.IsConnected)
                createTelnet();

            return _telnet_connection;
        }

        /// <summary>
        /// Closes telnet and ISA channels
        /// </summary>
        void closeTelnet()
        {
            if (_telnet_connection != null)
            {
                TraceLogger.Log("Close telnet");
                _telnet_connection.Close();
                _telnet_connection = null;
            }

            if (_ember != null && _ember.Interface == Ember.Interfaces.USB)
            {
                _ember.CloseISAChannels();
            }

        }

        /// <summary>
        /// Returns the MFG string from UUT
        /// </summary>
        /// <returns></returns>
        string getMfgString()
        {
            string msg = "";
            string mfg_str = null;
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    mfg_str = TCLI.Get_MFGString(openTelnet());
                    break;
                }
                catch (Exception ex)
                {
                    msg = string.Format("Unable to read MFG string: {0}", ex.Message);
                }
            }

            if (mfg_str == null)
            {
                throw new Exception(msg);
            }

            return mfg_str;

        }

        /// <summary>
        /// Selects the Board type by MFG string
        /// </summary>
        /// <param name="mfg_str"></param>
        void selectBoardByMFGString(string mfg_str)
        {

            string msg = "";
            bool autodetected = false;
            // Strip postfix
            string pmfgstr = mfg_str;
            int pos = mfg_str.IndexOf('-');
            if (pos > 0)
                pmfgstr = mfg_str.Substring(0, pos);

            switch (pmfgstr)
            {
                case "3110":
                    controlSetText(comboBoxBoardTypes, BoardTypes.Mudshark.ToString());
                    autodetected = true;
                    break;
                case "3200":
                    controlSetText(comboBoxBoardTypes, BoardTypes.Humpback.ToString());
                    autodetected = true;
                    break;
                case "3210":
                    controlSetText(comboBoxBoardTypes, BoardTypes.Zebrashark.ToString());
                    autodetected = true;
                    break;
                case "3115":
                    controlSetText(comboBoxBoardTypes, BoardTypes.Hornshark.ToString());
                    autodetected = true;
                    break;
                case "3141":
                    controlSetText(comboBoxBoardTypes, BoardTypes.Mahi.ToString());
                    autodetected = true;
                    break;
                case "3146":
                    controlSetText(comboBoxBoardTypes, BoardTypes.Halibut.ToString());
                    autodetected = true;
                    break;
                case "3220":
                    controlSetText(comboBoxBoardTypes, BoardTypes.Honeycomb.ToString());
                    autodetected = true;
                    break;
            }

            if (autodetected)
            {
                msg = string.Format("Board type {0} auto selected from MFG string {1}",
                    getSelectedBoardType(), mfg_str);
                updateOutputStatus(msg);
            }
            else
            {
                msg = string.Format("Unable to auto selected Board from MFG string {0}",
                    mfg_str);
                updateOutputStatus(msg);
            }


        }

        /// <summary>
        /// Tries to select the Board type by using the UUT MFG string
        /// </summary>
        void autoSelectBoardByMfgString()
        {
            if (_mfg_str != null)  // This indicates already done
                return;

            // Get MFG String and set board type
            closeTelnet();
            openTelnet();
            _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    TCLI.Wait_For_Prompt(_telnet_connection);
                    break;
                }
                catch { }
                _relay_ctrl.WriteLine(Relay_Lines.Power, false);
                Thread.Sleep(1000);
                _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                Thread.Sleep(1000);
            }
            try
            {
                _mfg_str = getMfgString();
                selectBoardByMFGString(_mfg_str);
            }
            catch (Exception ex)
            {
                updateOutputStatus(ex.Message);
            };

        }

        /// <summary>
        /// Wait for VAC power to be off
        /// </summary>
        /// <returns></returns>
        double wait_for_power_off()
        {
            if (!Properties.Settings.Default.PrePost_Test_Enabled)
                return 0;

            if (_meter == null)
                return -1;

            _relay_ctrl.OpenIfClosed();
            if (_relay_ctrl != null && _relay_ctrl.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Vac_Vdc, false);  // AC measure

            // Measure Voltage after power off
            if (!_meter.IsSerialPortOpen)
                _meter.Init();
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
                if (n++ > 10)
                {
                    _meter.CloseSerialPort();
                    msg = string.Format("Warning.  Failed to power off. VAC = {0:F8}", vac);
                    throw new Exception(msg);
                }
            }

            if (_relay_ctrl != null && _relay_ctrl.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Vac_Vdc, true);  // DC Measure
            _meter.SetupForVDC();

            double vdc = -1.0;
            n = 0;
            while (true)
            {
                string voltage_meter_str = _meter.Measure();
                vdc = Double.Parse(voltage_meter_str);
                if (vdc < 1.0)
                    break;
                if (n++ > 10)
                {
                    _meter.CloseSerialPort();
                    msg = string.Format("Warning DC voltage detected after power off. VDC = {0:F8}", vdc);
                    throw new Exception(msg);
                }
            }

            _meter.CloseSerialPort();

            msg = string.Format("Meter VAC = {0:F8}.  VDC  = {1:F8}.", vac, vdc);
            updateOutputStatus(msg);

            if (_relay_ctrl != null && _relay_ctrl.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Vac_Vdc, false);

            return vac;
        }

        /// <summary>
        /// Powers off uut
        /// </summary>
        void power_off()
        {
            // Turn power off
            if (_relay_ctrl != null)
            {
                _relay_ctrl.OpenIfClosed();
                _relay_ctrl.WriteLine(PowerCalibration.Relay_Lines.Power, false);
                relaysShowSetttings();
            }

            // Wait for power off
            if (_meter != null)
            {
                try
                {
                    wait_for_power_off();
                }
                catch (Exception ex)
                {
                    updateOutputStatus(ex.Message);
                }

            }

            if (_relay_ctrl != null)
            {
                _relay_ctrl.WriteLine(PowerCalibration.Relay_Lines.Load, false);
                _relay_ctrl.WriteLine(PowerCalibration.Relay_Lines.Ember, false);
            }

            _relay_ctrl.Close();

        }

        /// <summary>
        /// Gets the selected board type
        /// </summary>
        /// <returns></returns>
        BoardTypes getSelectedBoardType()
        {
            if (comboBoxBoardTypes.InvokeRequired)
            {
                getSelectedBoardTypeCallback d = new getSelectedBoardTypeCallback(getSelectedBoardType);
                return (BoardTypes)comboBoxBoardTypes.Invoke(d, new object[] { });

            }
            else
            {
                return (BoardTypes)Enum.Parse(typeof(BoardTypes), comboBoxBoardTypes.Text);
            }
        }

        /// <summary>
        /// Verifies DC Voltage
        /// </summary>
        void preTest(TaskTypes next_task)
        {
            // Start the running watch
            _stopwatch_running.Restart();

            // Disable the app
            setEnablement(false, false);

            // Init the status text box
            runStatus_Init();

            // Clear output status
            textBoxOutputStatus.Clear();

            // Clear tool strip
            toolStripTimingStatusLabel.Text = "";
            statusStrip.Update();

            setRunStatus("Start Pre-test", Color.Black, Color.White);
            updateOutputStatus("Start Pre-test".PadBoth(80, '-'));

            // Clear the error
            _pretest_error_msg = null;

            // Init the meter object
            if (Properties.Settings.Default.Meter_Manual_Measurement)
                _meter = null;
            else
            {
                if (_meter != null && _meter.IsSerialPortOpen)
                    _meter.CloseSerialPort();
                _meter = new MultiMeter(Properties.Settings.Default.Meter_COM_Port_Name);
            }

            _relay_ctrl.OpenIfClosed();
            _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
            _relay_ctrl.WriteLine(Relay_Lines.Load, false);
            _relay_ctrl.WriteLine(Relay_Lines.Power, true);
            Thread.Sleep(1000);
            if (getSelectedBoardType() == BoardTypes.Honeycomb)
            {
                _relay_ctrl.WriteLine(4, false);
            }
            relaysShowSetttings();

            if (!Properties.Settings.Default.PrePost_Test_Enabled)
            {
                pretest_done(next_task);
                return;
            }

            Tests pretest = new Tests();
            pretest.Status_Event += _pretest_Status_Event;
            pretest.MultiMeter = _meter;
            pretest.RelayController = _relay_ctrl;

            _task_uut = new Task(() => pretest.Verify_Voltage(3.3 - 0.33, 3.3 + 0.33));
            _task_uut.ContinueWith((t) => pretest_done_handler(next_task), TaskContinuationOptions.OnlyOnRanToCompletion);
            _task_uut.ContinueWith((t) => pretest_exception_handler(t, next_task), TaskContinuationOptions.OnlyOnFaulted);
            _task_uut.Start();

        }

        /// <summary>
        /// Pretest done handler
        /// </summary>
        void pretest_done(TaskTypes next_task)
        {
            // Check PASS or FAIL
            if (_pretest_error_msg == null)
            {
                updateRunStatus("PASS", Color.White, Color.Green);

                // Should be safe to connect Ember
                _relay_ctrl.OpenIfClosed();
                _relay_ctrl.WriteLine(Relay_Lines.Power, false);
                Thread.Sleep(500);

                if (next_task == TaskTypes.Test || next_task == TaskTypes.Calibrate)
                {
                    openTelnet();
                }
                _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                Thread.Sleep(1000);
            }
            else
            {
                closeTelnet();
                try
                {
                    power_off();
                }
                catch (Exception ex)
                {
                    updateOutputStatus("Power off exception:" + ex.Message);
                }

                if (_meter != null)
                    _meter.CloseSerialPort();

                updateRunStatus("FAIL", Color.White, Color.Red);
                updateOutputStatus(_pretest_error_msg);

                setEnablement(true, false);
            }

            _stopwatch_running.Stop();
            string elapsedTime = string.Format("Elapsed time {0:00} seconds", _stopwatch_running.Elapsed.TotalSeconds);
            updateOutputStatus(elapsedTime);
            updateOutputStatus("End Pre-test".PadBoth(80, '-'));

            if (_pretest_error_msg != null)
            {
                setEnablement(true, false);
                return;
            }

            try
            {
                // Start the running stopwatch
                _stopwatch_running.Restart();
                // Run the next task
                switch (next_task)
                {
                    case TaskTypes.Code:
                        _coding_error_msg = null;
                        try
                        {
                            code();
                        }
                        catch (Exception ex)
                        {
                            _coding_error_msg = ex.Message + "\r\n" + ex.StackTrace;
                            coding_done();
                        }
                        break;

                    case TaskTypes.Calibrate:
                        _calibration_error_msg = null;
                        try
                        {
                            calibrate();
                        }
                        catch (Exception ex)
                        {
                            _calibration_error_msg = ex.Message;
                            calibration_done();
                        }
                        break;

                    case TaskTypes.Test:
                        if (getSelectedBoardType() == BoardTypes.Honeycomb)
                        {
                            _test_error_msg = null;
                            try
                            {
                                hct_Run_Tests();
                            }
                            catch (Exception ex)
                            {
                                _test_error_msg = ex.Message;
                                hct_done();
                            }
                        }
                        break;
                    case TaskTypes.Recode:
                        recode();
                        break;
                    case TaskTypes.None:
                        setEnablement(true, false);
                        _stopwatch_running.Stop();
                        break;
                }

            }
            catch (Exception ex)
            {
                switch (next_task)
                {
                    case TaskTypes.Code:
                        _coding_error_msg = ex.Message;
                        coding_done();
                        break;
                    case TaskTypes.Calibrate:
                        _calibration_error_msg = ex.Message;
                        calibration_done();
                        break;
                    case TaskTypes.Recode:
                        power_off();
                        break;
                }

            }
        }

        /// <summary>
        /// Handles when pretest is done
        /// </summary>
        /// <param name="task"></param>
        void pretest_done_handler(TaskTypes next_task)
        {
            pretest_done(next_task);
        }

        /// <summary>
        /// Handles when pretest throws an error
        /// </summary>
        /// <param name="task"></param>
        void pretest_exception_handler(Task task, TaskTypes next_task)
        {
            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;
            _pretest_error_msg = errmsg;
            pretest_done(next_task);
        }

        /// <summary>
        /// Pretest status event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status_txt"></param>
        void _pretest_Status_Event(object sender, string status_txt)
        {
            updateOutputStatus(status_txt);
        }

        /// <summary>
        /// Run Calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonClick_Calibrate(object sender, EventArgs e)
        {
            _running_all = false;
            _mfg_str = null;

            if (!this.buttonCalibrate.Enabled) return;

            preTest(TaskTypes.Calibrate);
        }

        /// <summary>
        /// Main Calibration function
        /// </summary>
        void calibrate()
        {
            try
            {
                _calibration_error_msg = null;

                setRunStatus("Start Calibration", Color.Black, Color.White);
                updateOutputStatus("Start Calibration".PadBoth(80, '-'));

                // Connect the load
                _relay_ctrl.OpenIfClosed();
                _relay_ctrl.WriteLine(Relay_Lines.Load, true);
                relaysShowSetttings();

                //for debug
                //calibration_done();
                //return;

                autoSelectBoardByMfgString();
                // Disable the app
                setEnablement(false, false);

                // Run the calibration
                Calibrate calibrate = new Calibrate(); // Calibration object
                calibrate.Status_Event += calibration_Status_event;
                calibrate.Run_Status_Event += calibration_Run_Status_Event;
                calibrate.Relay_Event += calibration_Relay_Event;
                calibrate.CalibrationResults_Event += calibration_Results_Event;
                calibrate.BoardType = getSelectedBoardType();
                calibrate.Ember = _ember;
                calibrate.MultiMeter = _meter;
                calibrate.RelayController = _relay_ctrl;
                calibrate.TelnetConnection = _telnet_connection;

                _cancel_token_uut = new CancellationTokenSource();
                _task_uut = new Task(() => calibrate.Run(_cancel_token_uut.Token), _cancel_token_uut.Token);
                _task_uut.ContinueWith(
                    calibration_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
                _task_uut.ContinueWith(
                    calibration_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);
                _task_uut.Start();
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
            // enable read protection only if enable and this is a complete run
            if (_running_all && getSelectedBoardType() == BoardTypes.Honeycomb)
            {
                updateRunStatus("Clear Sensor Id");
                Tests_Honeycomb.ClearSensorId(_telnet_connection);
                Thread.Sleep(500);
            }

            if (_running_all && Properties.Settings.Default.Ember_ReadProtect_Enabled)
            {
                try
                {
                    updateRunStatus("EnableRdProt");

                    string err_msg = "";
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            string output = _ember.EnableRdProt(true);
                            err_msg = "";
                            TraceLogger.Log(output);
                            break;
                        }
                        catch (Exception ex)
                        {
                            err_msg = ex.Message;
                        }

                    }
                    if (err_msg != "")
                    {
                        throw new Exception(err_msg);
                    }
                }
                catch (Exception ex)
                {
                    updateOutputStatus("EnableRdProt exception:" + ex.Message);
                }
            }

            // If we fail lets disable read protection to force flash erase
            if (_calibration_error_msg != null && 
                _running_all && 
                Properties.Settings.Default.Ember_ReadProtect_Enabled)
            {
                updateRunStatus("DisableRdProt");
                try
                {
                    string output = _ember.EnableRdProt(false);
                    TraceLogger.Log(output);
                    if (!output.Contains("Disable Read Protection"))
                        updateOutputStatus("Unable to disable read protection: " + output);

                }
                catch (Exception ex)
                {
                    updateOutputStatus(ex.Message);
                }
            }


            try
            {
                power_off();
            }
            catch (Exception ex)
            {
                updateOutputStatus("Power off exception:" + ex.Message);
            }

            // Check PASS or FAIL
            if (_calibration_error_msg == null)
            {
                updateRunStatus("PASS", Color.White, Color.Green);
                playSound(Sounds.PASS);
            }
            else
            {
                updateRunStatus("FAIL", Color.White, Color.Red);
                updateOutputStatus(_calibration_error_msg);
            }

            _running_all = false;  // Reset

            if (_meter != null)
                _meter.CloseSerialPort();
            closeTelnet();

            // Stop running watch and report time lapse
            _stopwatch_running.Stop();
            string elapsedTime = String.Format("Elapsed time {0:00} seconds", _stopwatch_running.Elapsed.TotalSeconds);
            updateOutputStatus(elapsedTime);
            //_stopwatch_running.Reset();

            setEnablement(true, false);

            updateOutputStatus("End Calibration".PadBoth(80, '-'));

        }

        /// <summary>
        /// Handles when calibration is done
        /// </summary>
        /// <param name="task"></param>
        void calibration_done_handler(Task task)
        {
            calibration_done();
        }

        /// <summary>
        /// Handles when calibration throws an error
        /// </summary>
        /// <param name="task"></param>
        void calibration_exception_handler(Task task)
        {
            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;

            _calibration_error_msg = errmsg;
            calibration_done();
        }

        /// <summary>
        /// Handle the calibration results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void calibration_Results_Event(object sender, CalibrationResultsEventArgs e)
        {
            // Check whether db logging is disabled
            if (!isDBLogingEnabled())
                return;

            DataRow r = _datatable_calibrate.NewRow();
            r["VoltageGain"] = e.Voltage_gain;
            r["CurrentGain"] = e.Current_gain;
            r["Eui"] = e.EUI;
            r["DateCalibrated"] = e.Timestamp;

            lock (_datatable_calibrate)
            {
                _datatable_calibrate.Rows.Add(r);
            }

            if (_task_updatedb == null || _task_updatedb.Status != TaskStatus.Created)
            {
                _task_updatedb = new Task(updateDB);
            }

            if (_task_updatedb.Status != TaskStatus.Running)
            {
                _task_updatedb.Start();
            }

            if (_datatable_calibrate.Rows.Count > 10000)
            {
                _datatable_calibrate.Rows.Clear();
            }

        }

        /// <summary>
        /// Handles relay controller event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="relay_controller"></param>
        void calibration_Relay_Event(object sender, RelayControler relay_controller)
        {
            relaysShowSetttings();
        }

        /// <summary>
        /// Handel calibration status events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status_txt"></param>
        void calibration_Status_event(object sender, string status_txt)
        {
            updateOutputStatus(status_txt);
        }

        /// <summary>
        /// Handles calibration run status events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status_txt"></param>
        void calibration_Run_Status_Event(object sender, string status_txt)
        {
            updateRunStatus(status_txt);
        }

        /// <summary>
        /// Honeycomb 
        /// </summary>
        void hct_Run_Tests()
        {
            try
            {
                // Clear error message
                _test_error_msg = null;

                // Disable the app
                setEnablement(false, false);

                setRunStatus("Start Honeycomb Tests", Color.Black, Color.White);
                updateOutputStatus("Start Honeycomb Tests".PadBoth(80, '-'));

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        TCLI.Wait_For_Prompt(openTelnet(), retry_count: 10);
                        break;
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        _relay_ctrl.WriteLine(Relay_Lines.Power, false);
                        Thread.Sleep(1000);
                        _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                        Thread.Sleep(2000);
                    }
                }
                relaysShowSetttings();

                Tests_Honeycomb hct = new Tests_Honeycomb(_relay_ctrl, _meter, _telnet_connection);
                hct.Status_Event += hct_Status_Event;
                hct.Run_Status_Event += hct_Run_Status_Event;

                _cancel_token_uut = new CancellationTokenSource();
                _task_uut = new Task(() => hct.Run_Tests(_cancel_token_uut.Token), _cancel_token_uut.Token);
                _task_uut.ContinueWith(
                    hct_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
                _task_uut.ContinueWith(
                    hct_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);
                _task_uut.Start();

            }
            catch (Exception ex)
            {
                _test_error_msg = ex.Message;
                hct_done();
            }
        }

        /// <summary>
        /// Honeycomb 
        /// </summary>
        void hct_Status_Event(object sender, string status_txt)
        {
            updateOutputStatus(status_txt);
        }

        /// <summary>
        /// Honeycomb 
        /// </summary>
        void hct_exception_handler(Task task)
        {
            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;

            _test_error_msg = errmsg;
            hct_done();
        }

        /// <summary>
        /// Honeycomb 
        /// </summary>
        void hct_done_handler(Task task)
        {
            hct_done();
        }

        /// <summary>
        /// Honeycomb 
        /// </summary>
        void hct_done()
        {
            if (!_running_all || _test_error_msg != null)
            {
                try
                {
                    power_off();
                }
                catch (Exception ex)
                {
                    updateOutputStatus("Power off exception:" + ex.Message);
                }
                closeTelnet();
            }

            // Check PASS or FAIL
            if (_test_error_msg == null)
            {
                updateRunStatus("PASS", Color.White, Color.Green);

            }
            else
            {
                if (_meter != null)
                    _meter.CloseSerialPort();
                updateRunStatus("FAIL", Color.White, Color.Red);
                updateOutputStatus(_test_error_msg);
            }

            // Stop running watch and report time lapse
            _stopwatch_running.Stop();
            string elapsedTime = String.Format("Elapsed time {0:00} seconds", _stopwatch_running.Elapsed.TotalSeconds);
            updateOutputStatus(elapsedTime);
            _stopwatch_running.Reset();

            updateOutputStatus("End Honeycomb tests".PadBoth(80, '-'));

            if (_running_all && _test_error_msg == null)
            {
                calibrate();
            }
            else
            {
                setEnablement(true, false);
            }

        }

        /// <summary>
        /// Honeycomb 
        /// </summary>
        void hct_Run_Status_Event(object sender, string status_txt)
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
            string msg = "";

            if (_task_uut != null && _task_uut.Status == TaskStatus.Running)
            {
                if (!_stopwatch_running.IsRunning)
                    _stopwatch_running.Start();

                if (_stopwatch_idel.IsRunning)
                    _stopwatch_idel.Reset();

                msg = string.Format("Running {0:dd\\.hh\\:mm\\:ss}", _stopwatch_running.Elapsed);
            }
            else
            {
                if (!_stopwatch_idel.IsRunning)
                    _stopwatch_idel.Start();

                if (_stopwatch_running.IsRunning)
                    _stopwatch_running.Stop();

                msg = string.Format("Idle {0:dd\\.hh\\:mm\\:ss}", _stopwatch_idel.Elapsed);

            }
            toolStripTimingStatusLabel.Text = msg;
        }

        /// <summary>
        /// Handles when the form is shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Form_Main_Shown(object sender, EventArgs e)
        {
            // Init relay controller
            initRelayController();

            string msg = "";

            // Try to find isa3 ip if not set
            string isa3ip = Properties.Settings.Default.Ember_Interface_IP_Address;
            if (isa3ip == "0.0.0.0")
            {
                try
                {
                    // First try using relay controller id
                    string[] ips = new string[] { };
                    if (_relay_ctrl.ID != 0)
                        ips = DataUtils.GetISAAdapterIPsFromLikeLocation(string.Format("FT232H:{0}", _relay_ctrl.SerialNumber));
                    if (ips.Length == 0)
                        ips = DataUtils.GetISAAdapterIPsFromLikeLocation(string.Format("{0}", getSelectedBoardType()));

                    if (ips.Length >= 1)
                    {

                        Properties.Settings.Default.Ember_Interface_IP_Address = ips[0];
                        Properties.Settings.Default.Save();

                        msg = string.Format("ISA3 adapter ip set to {0}", ips[0]);
                        updateOutputStatus(msg);
                    }
                    else
                    {
                        throw new Exception("Unable to get ISA3 ip");
                    }

                }
                catch (Exception ex)
                {
                    msg = "ISA3 adapter ip not set: " + ex.Message;
                    updateOutputStatus(msg);
                }

            }

            // Report EnableRdProt
            msg = "EnableRdProt = " + Properties.Settings.Default.Ember_ReadProtect_Enabled.ToString();
            updateOutputStatus(msg);


            buttonRun.Focus();
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
            _running_all = false;
            _mfg_str = null;

            // Just in case
            if (!this.buttonCode.Enabled) return;

            // If the button is labeled "Cancel" then just cancel the task
            if (buttonCode.Text == "&Cancel")
            {
                _cancel_token_uut.Cancel();
                return;
            }

            // Disable the app
            buttonCode.Text = "&Cancel";
            setEnablement(false, true);

            preTest(TaskTypes.Code);
        }

        /// <summary>
        /// Starts the coding task
        /// </summary>
        void code()
        {
            _coding_error_msg = null;

            // Run coding
            _cancel_token_uut = new CancellationTokenSource();
            _task_uut = null;
            switch (_coding_method)
            {
                case Coding_Method.EBL:

                    // Init coder
                    Coder coder = new Coder(new TimeSpan(0, 2, 0));
                    coder.Status_Event += coder_Status_Event;

                    // This may only be needed fwhen using USB?  Otherwise using initEmberIF(); may be all we need here
                    openTelnet();

                    coder.Ember = _ember;

                    _task_uut = new Task(() => coder.Code(_cancel_token_uut.Token), _cancel_token_uut.Token);
                    _task_uut.ContinueWith(coding_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
                    _task_uut.ContinueWith(coding_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);
                    break;
                case Coding_Method.ISA_UTIL:

                    initEmberIF();

                    _ember.Process_Output_Event += _ember_Process_Output_Event;
                    _task_uut = new Task<string>(() => _ember.Load(Properties.Settings.Default.Coding_File) );
                    _task_uut.ContinueWith(isa_load_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
                    _task_uut.ContinueWith(isa_load_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);


                    //string output = _ember.Load(@"C:\Users\victormartin\Downloads\mahi.hex");
                    break;
            }


            if (_task_uut != null)
            {
                setRunStatus("Start Coding", Color.Black, Color.White);
                updateOutputStatus("Start Coding".PadBoth(80, '-'));
                relaysShowSetttings();

                _task_uut.Start();
            }

        }

        private void _ember_Process_Output_Event(object sender, string line)
        {
            updateOutputStatus(line);
        }

        void coder_Status_Event(object sender, string status_txt)
        {
            updateOutputStatus(status_txt);
        }

        /// <summary>
        /// Coding done handler
        /// </summary>
        void coding_done()
        {
            // reactivate the window
            activate();

            // Check whether PASS, Cancelled or FAIL
            if (_cancel_token_uut != null && _cancel_token_uut.IsCancellationRequested)
            {
                _cancel_token_uut = new CancellationTokenSource();
                updateRunStatus("Cancelled", Color.Black, Color.Yellow);
            }
            else
            {

                if (_coding_error_msg == null)
                {
                    if (_running_all)
                        autoSelectBoardByMfgString();

                    updateRunStatus("PASS", Color.White, Color.Green);
                }
                else
                {
                    updateRunStatus("FAIL", Color.White, Color.Red);
                    updateOutputStatus(_coding_error_msg);

                    closeTelnet();

                    if (_meter != null)
                        _meter.CloseSerialPort();
                }
            }

            // Turn power off if we are not running all or
            // there was a coding error
            if (!_running_all || _coding_error_msg != null)
            {
                try
                {
                    power_off();
                }
                catch (Exception ex)
                {
                    updateOutputStatus("Power off exception:" + ex.Message);
                }
                setEnablement(true, false);
            }

            // Stop running watch and report time lapse
            _stopwatch_running.Stop();
            string elapsedTime = String.Format("Elapsed time {0:00} seconds", _stopwatch_running.Elapsed.TotalSeconds);
            updateOutputStatus(elapsedTime);
            _stopwatch_running.Reset();

            updateOutputStatus("End Coding".PadBoth(80, '-'));

            // Run next task if everything is OK
            if (_running_all && _coding_error_msg == null &&
                !_cancel_token_uut.IsCancellationRequested)
            {

                setRunStatus("Reset UUT");
                TraceLogger.Log("Reset UUT");

                _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        TCLI.Wait_For_Prompt(openTelnet());
                        break;
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                    }

                    if (_relay_ctrl.Device_Type == RelayControler.Device_Types.Manual)
                        showDialogMsg("Reset UUT");

                    _relay_ctrl.WriteLine(Relay_Lines.Power, false);
                    Thread.Sleep(1000);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    Thread.Sleep(2000);
                }

                if (getSelectedBoardType() == BoardTypes.Honeycomb)
                {
                    hct_Run_Tests();
                }
                else
                {
                    calibrate();
                }
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

        void isa_load_done_handler(Task task)
        {
            _ember.Process_Output_Event -= _ember_Process_Output_Event;

            coding_done();
        }

        void isa_load_exception_handler(Task task)
        {
            _ember.Process_Output_Event -= _ember_Process_Output_Event;

            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;
            _coding_error_msg = errmsg;

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
        /// Clicks the calibrate button
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
        /// Runs Code + calibration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonClick_Run(object sender, EventArgs e)
        {
            _running_all = true;
            _mfg_str = null;
            preTest(TaskTypes.Code);
        }

        /// <summary>
        /// Plays a sounds
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="delay_before_ms"></param>
        void playsound(Sounds sound)
        {
            if (!Properties.Settings.Default.Play_Sounds)
                return;

            Stream str = null;
            if (sound == Sounds.FAIL)
                str = Properties.Resources.Sound_Glass_Break1;
            else if (sound == Sounds.PASS)
                str = Properties.Resources.Sound_Yeah1;

            if (str == null)
                return;

            SoundPlayer snd = new SoundPlayer(str);
            snd.Play();

        }


        private void buttonRecode_Click(object sender, EventArgs e)
        {
            _running_all = false;
            _mfg_str = null;
            preTest(TaskTypes.Recode);
        }

        /// <summary>
        /// Runs the recode task
        /// </summary>
        void recode()
        {
            _cancel_token_uut = new CancellationTokenSource();

            Recode recode = new Recode(_ember);

            recode.Status_Event += recode_Status_Event;
            recode.Run_Status_Event += recode_Run_Status_Event;

            _task_uut = new Task(() => recode.Run(_cancel_token_uut.Token), _cancel_token_uut.Token);
            _task_uut.ContinueWith(
                recoder_exception_handler, TaskContinuationOptions.OnlyOnFaulted);
            _task_uut.ContinueWith(
                recoder_done_handler, TaskContinuationOptions.OnlyOnRanToCompletion);
            _task_uut.Start();
        }

        void recode_Run_Status_Event(object sender, string status_txt)
        {
            updateRunStatus(status_txt);

            if (status_txt == "Patch UUT Tokens")
            {
                activate();
            }
        }

        void recode_Status_Event(object sender, string status_txt)
        {
            updateOutputStatus(status_txt);
        }

        void recoder_done_handler(Task task)
        {
            updateRunStatus("PASS", Color.White, Color.Green);
        }

        void recoder_exception_handler(Task task)
        {
            var exception = task.Exception;
            string errmsg = exception.InnerException.Message;

            updateRunStatus("FAIL", Color.White, Color.Red);
            updateOutputStatus(errmsg);
        }

        string assemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            _running_all = false;
            _mfg_str = null;

            preTest(TaskTypes.Test);
        }

        private void buttonPreTest_Click(object sender, EventArgs e)
        {
            preTest(TaskTypes.None);
        }
    }


    /// <summary>
    /// Defines the relay lines. 
    /// </summary>
    public class Relay_Lines
    {
        static string _key_acPower = "AC Power";
        static string _key_load = "Load";
        static string _key_ember = "Ember";
        static string _key_vac_or_vdc = "VacVdc";

        public static string Power { get { return _key_acPower; } }
        public static string Load { get { return _key_load; } }
        public static string Ember { get { return _key_ember; } }
        public static string Vac_Vdc { get { return _key_vac_or_vdc; } }

    }

}

namespace System
{
    public static class StringExtensions
    {
        public static string PadBoth(this string str, int length, char paddingChar)
        {
            int spaces = length - str.Length;
            int padLeft = spaces / 2 + str.Length;
            return str.PadLeft(padLeft, paddingChar).PadRight(length, paddingChar);
        }
    }
}