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


using NationalInstruments;
using NationalInstruments.DAQmx;

namespace powercal
{
    public partial class FormMain : Form
    {
        CSSequencer _sq = null;
        MultiMeter _meter = null;
        RelayControler _relay_ctrl = new RelayControler();

        static string _app_data_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration");

        string _log_file = Path.Combine(_app_data_dir, "runlog.txt");
        string _ember_batchfile_path = Path.Combine(_app_data_dir, "patchit.bat");

        delegate void SetTextCallback();


        public FormMain()
        {

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
            this.comboBoxBoardTypes.Items.AddRange(Enum.GetNames(typeof(CSSequencer.BoardTypes)));
            if (comboBoxBoardTypes.Items.Count > 0)
            {
                comboBoxBoardTypes.SelectedIndex = 0;
            }

            // Report COM ports found in system
            string[] ports = SerialPort.GetPortNames();
            string msg = "";
            bool detected_cs = false;
            foreach (string portname in ports)
            {
                msg += string.Format("{0}, ", portname);

                if (portname == Properties.Settings.Default.CS_COM_Port_Name)
                {
                    detected_cs = true;
                }
            }
            if (msg != "")
            {
                msg = msg.TrimEnd(new char[] { ' ', ',' });
                msg = string.Format("System serial ports: {0}", msg);
                updateOutputStatus(msg);
            }

            // Detect whether meter is connected to on eof the ports
            bool detected_meter = autoDetectMeterCOMPort();
            if (!detected_cs && !detected_meter && ports.Length > 0)
            {
                Properties.Settings.Default.CS_COM_Port_Name = ports[0];
                Properties.Settings.Default.Save();
            }

            // Ember path
            msg = string.Format("Cirrus Logic comunications port = {0}", Properties.Settings.Default.CS_COM_Port_Name);
            updateOutputStatus(msg);

            if (!Directory.Exists(Properties.Settings.Default.Ember_BinPath))
            {
                msg = string.Format("Unable to find Ember bin path \"{0}\"", Properties.Settings.Default.Ember_BinPath);
            }
            else
            {
                msg = string.Format("Ember bin path set at\"{0}\"", Properties.Settings.Default.Ember_BinPath);
            }
            updateOutputStatus(msg);

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

        private void setText()
        {
            if (this.textBoxOutputStatus.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setText);
                try
                {
                    this.Invoke(d);
                }
                catch { }
            }
            else
            {
            }
        }

        /// <summary>
        /// Updates the output status text box and log file
        /// </summary>
        /// <param name="txt"></param>
        private void updateOutputStatus(string txt)
        {
            string line = string.Format("{0:G}: {1}\r\n", DateTime.Now, txt);
            this.textBoxOutputStatus.AppendText(line);
            this.textBoxOutputStatus.Update();

            using (StreamWriter sw = File.AppendText(_log_file))
            {
                sw.WriteLine("{0:G}: {1}", DateTime.Now, txt);
            }
        }

        /// <summary>
        /// Updates the run status text box
        /// </summary>
        /// <param name="txt"></param>
        private void updateRunStatus(string txt)
        {
            this.textBoxRunStatus.Text = txt;
            this.textBoxRunStatus.Update();
        }

        /// <summary>
        /// Invokes Serial test gld
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialToolStripMenuItem_Click(object sender, EventArgs e)
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
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 dlg = new AboutBox1();
            DialogResult result = dlg.ShowDialog();
        }

        /// <summary>
        /// Shows dialog with relays sates defined in the RelayControler when in manual mode
        /// </summary>
        /// <param name="relay_ctrl"></param>
        private void relaysSet(RelayControler relay_ctrl)
        {

            bool manual_relay = Properties.Settings.Default.Manual_Relay_Control;
            if (manual_relay)
            {
                //string msg_dlg = relay_ctrl.ToDlgText();
                string key = "AC Power";
                bool value = _relay_ctrl.ReadLine(key);
                string on_off_str = "OFF";
                if (value)
                    on_off_str = "ON";
                string msg_dlg = string.Format("{0} = {1}\r\n", key, on_off_str);

                key = "Reset";
                value = _relay_ctrl.ReadLine(key);
                on_off_str = "OFF";
                if (value)
                    on_off_str = "ON";
                msg_dlg += string.Format("{0} = {1}\r\n", key, on_off_str);

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
            else
            {
                string status = _relay_ctrl.ToStatusText();
                updateOutputStatus(status);
            }

        }

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

        private void buttonRun_Click(object sender, EventArgs e)
        {
            //this.buttonRun.Enabled = false;
            this.textBoxOutputStatus.Clear();
            initTextBoxRunStatus();
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                calibrate();
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                // Format and display the TimeSpan value. 
                string elapsedTime = String.Format("Elaspsed time {0:00} seconds", ts.Seconds);
                updateOutputStatus(elapsedTime);

            }
            catch (Exception ex)
            {
                this.textBoxRunStatus.BackColor = Color.Red;
                this.textBoxRunStatus.ForeColor = Color.White;
                updateRunStatus("FAIL");
                updateOutputStatus(ex.Message);

                bool manual_relay = Properties.Settings.Default.Manual_Relay_Control;
                if (manual_relay)
                    _relay_ctrl.Disable = true;
                _relay_ctrl.Ember = false;
                _relay_ctrl.AC_Power = false;
                _relay_ctrl.Load = false;
                _relay_ctrl.Reset = false;
                relaysSet(_relay_ctrl);

                if (_sq != null)
                    _sq.CloseSerialPort();

                if (_meter != null)
                    _meter.CloseSerialPort();
            }
            this.buttonRun.Enabled = true;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxOutputStatus.Clear();
        }

        private void calibrate()
        {

            bool manual_measure = Properties.Settings.Default.Meter_Manual_Measurement;

            string msg;
            updateOutputStatus("===============================Start Calibration==============================");
            string csPortName = Properties.Settings.Default.CS_COM_Port_Name;
            CSSequencer.BoardTypes board = (CSSequencer.BoardTypes)Enum.Parse(typeof(CSSequencer.BoardTypes), comboBoxBoardTypes.Text);
            _sq = new CSSequencer(csPortName, board);

            // Setup multi-meter
            string meterPortName = Properties.Settings.Default.Meter_COM_Port_Name;
            _meter = new MultiMeter(meterPortName);
            if (!manual_measure)
            {
                updateRunStatus("Setup multi-meter");
                _meter.OpenComPort();
                _meter.SetToRemote();
                _meter.ClearError();
                string idn = _meter.IDN();
            }

            // IOffsetPre_Cal
            updateRunStatus("IOffsetPre_Cal");
            bool manual_relay = Properties.Settings.Default.Manual_Relay_Control;
            if (manual_relay)
                _relay_ctrl.Disable = true;
            _relay_ctrl.Ember = false;
            _relay_ctrl.Load = false;
            _relay_ctrl.Reset = true;
            _relay_ctrl.AC_Power = true;
            relaysSet(_relay_ctrl);
            Thread.Sleep(1000);

            _sq.OpenSerialPort();

            _sq.SoftReset();
            Thread.Sleep(500);

            double acOffsetPre_Cal = _sq.GetIOffset();
            updateOutputStatus(string.Format("ACOffsetPre_Cal = {0:F8}", acOffsetPre_Cal));

            // IRMSNoLoad
            updateRunStatus("IRMSNoLoad");
            double iRMSNoLoad = _sq.IRMSNoLoad();
            if (iRMSNoLoad > 0.05)
            {
                msg = string.Format("Bad IrmsNoLoad value:{08:F}", iRMSNoLoad);
                throw new Exception(msg);
            }
            updateOutputStatus(string.Format("IrmsNoLoad = {0:F8}", iRMSNoLoad));

            // IRMSPre_Cal
            updateRunStatus("IRMSPre_Cal");

            // Connect the load
            _relay_ctrl.Load = true;
            relaysSet(_relay_ctrl);

            _sq.EnableHiPassFilter();
            _sq.StartContinuousConvertion();
            
            Thread.Sleep(1000);

            if (!manual_measure)
            {
                _meter.SetupForVAC();

                for (int i = 0; i < 3; i++)
                {
                    string vac_measurement = _meter.Measure();
                    double meter_vrms = Double.Parse(vac_measurement);
                    updateOutputStatus(string.Format("Meter Vrms{0} = {1:F8}", i, meter_vrms));
                }
            }

            for (int i = 0; i < 5; i++) {
                double iPreCal = _sq.GetIRMS();
                updateOutputStatus(string.Format("IrmsPreCal{0} = {1:F8}", i, iPreCal));
                Thread.Sleep(250);
            }


            double iRMSPreCal = _sq.GetIRMS();
            // for hooktooth iRMSPreCal < 0.17 || iRMSPreCal > 0.3
            double lowlimit = 0.17;
            double highlimit = 0.3;
            switch (board)
            {
                case(CSSequencer.BoardTypes.Humpback):
                    lowlimit = 0.08;
                    highlimit = 0.19;
                    break;
            }

            if (iRMSPreCal < lowlimit || iRMSPreCal > highlimit)
            {
                // With 500 Ohms load and 120VAC this value should be around 0.240 mA
                // With 2k Ohms load and 240VAC this value should be around 0.120 mA
                msg = string.Format("Bad IrmsPreCal value:{0:F8}", iRMSPreCal);
                throw new Exception(msg);
            }
            updateOutputStatus(string.Format("IrmsPreCal = {0:F8}", iRMSPreCal));

            // VRMSPre_Cal
            updateRunStatus("VRMSPre_Cal");

            for (int i = 0; i < 5; i++)
            {
                double vPreCal = _sq.GetVRMS();
                updateOutputStatus(string.Format("VrmsPreCal = {0:F8}", vPreCal));
                Thread.Sleep(250);
            }

            double vRMSPreCal = _sq.GetVRMS();
            updateOutputStatus(string.Format("VrmsPreCal = {0:F8}", vRMSPreCal));

            // Hooktooth (vRMSPreCal < 100 || vRMSPreCal > 150)
            lowlimit = 100;
            highlimit = 150;
            switch (board)
            {
                case (CSSequencer.BoardTypes.Humpback):
                    lowlimit = 200;
                    highlimit = 260;
                    break;
            }

            if (vRMSPreCal < lowlimit || vRMSPreCal > highlimit)
            {
                msg = string.Format("Bad VrmsPreCal value:{0:F}", vRMSPreCal);
                throw new Exception(msg);
            }

            // IRMSMeasure
            updateRunStatus("IRMSMeasure");
            double iRMSMeasure = 0;
            if (manual_measure)
            {
                // Enter measurement
                FormEnterMeasurement dlg = new FormEnterMeasurement();
                iRMSMeasure = dlg.GetMeasurement("Irms:");
            }
            else
            {
                _meter.SetupForIAC();
                for (int i = 0; i < 3; i++)
                {
                    string iac_str = _meter.Measure();
                    double meter_irms = Double.Parse(iac_str);
                    updateOutputStatus(string.Format("Meter Irms{0} = {1:F8}", i, meter_irms));
                }

                string iac_measurement = _meter.Measure();
                iRMSMeasure = Double.Parse(iac_measurement);
                Thread.Sleep(1000);
            }
            updateOutputStatus(string.Format("IrmsMeasure = {0:F8}", iRMSMeasure));

            // VRMSMeasure
            updateRunStatus("VRMSMeasure");
            double vRMSMeasure = 0;
            if (manual_measure)
            {
                // Enter measurement
                FormEnterMeasurement dlg = new FormEnterMeasurement();
                vRMSMeasure = dlg.GetMeasurement("Vrms:");
            }
            else
            {
                _meter.SetupForVAC();

                for (int i = 0; i < 3; i++)
                {
                    string vac_str = _meter.Measure();
                    double meter_vrms = Double.Parse(vac_str);
                    updateOutputStatus(string.Format("Meter Irms{0} = {1:F8}", i, meter_vrms));
                }

                string vac_measurement = _meter.Measure();
                vRMSMeasure = Double.Parse(vac_measurement);
            }
            updateOutputStatus(string.Format("VrmsMeasure = {0:F8}", vRMSMeasure));

            // IGainCal
            updateRunStatus("IGainCal");
            double iRMSGain = iRMSMeasure / iRMSPreCal;
            int iRMSGainInt = (int)(iRMSGain * 0x400000);
            updateOutputStatus(string.Format("IrmsGain = {0:F8} (0x{1:X})", iRMSGain, iRMSGainInt));
            _sq.IGainCal(iRMSGainInt);

            // VGainCal
            updateRunStatus("VGainCal");
            double vRMSGain = vRMSMeasure / vRMSPreCal;
            int vRMSGainInt = (int)(vRMSGain * 0x400000);
            updateOutputStatus(string.Format("VrmsGain = {0:F8} (0x{1:X})", vRMSGain, vRMSGainInt));
            _sq.VGainCal(vRMSGainInt);

            // IRMSNoLoad

            // Disconnect the load
            _relay_ctrl.Load = false;
            relaysSet(_relay_ctrl);
            Thread.Sleep(2000);

            updateRunStatus("IRMSNoLoad");

            iRMSNoLoad = _sq.IRMSNoLoad();
            updateOutputStatus(string.Format("IrmsNoLoad = {0:F8}", iRMSNoLoad));
            if (iRMSNoLoad > 0.05)
            {
                msg = string.Format("Bad IrmsNoLoad value:{0:F8}", iRMSNoLoad);
                throw new Exception(msg);
            }

            // Connect the load
            _relay_ctrl.Load = true;
            relaysSet(_relay_ctrl);
            Thread.Sleep(2000);

            // IRMSAfter_Cal
            updateRunStatus("IRMSAfter_Cal");
            double iRMSAfterCal = _sq.GetIRMS();
            updateOutputStatus(string.Format("IrmsAfterCal = {0:F8}", iRMSAfterCal));
            double delta = iRMSMeasure * 0.03;
            lowlimit = iRMSMeasure - delta;
            highlimit = iRMSMeasure + delta;
            if (iRMSAfterCal < lowlimit || iRMSAfterCal > highlimit)
            {
                msg = string.Format("IrmsAfterCal not within limits values: {0:F8} < {1:F8} < {2:F8}", lowlimit, iRMSAfterCal, highlimit);
                Debug.WriteLine(msg);
                throw new Exception(msg);
            }

            Thread.Sleep(1000);

            // VRMSAfter_Cal
            updateRunStatus("VRMSAfter_Cal");
            double vRMSAfterCal = _sq.GetVRMS();
            updateOutputStatus(string.Format("VrmsAfterCal = {0:F8}", vRMSAfterCal));
            delta = vRMSMeasure * 0.03;
            lowlimit = vRMSMeasure - delta;
            highlimit = vRMSMeasure + delta;
            if (vRMSAfterCal < lowlimit || vRMSAfterCal > highlimit)
            {
                msg = string.Format("VrmsAfterCal not within limits values: {0:F8} < {1:F8} < {2:F8}", lowlimit, vRMSAfterCal, highlimit);
                Debug.WriteLine(msg);
                throw new Exception(msg);
            }

            // IGainAdj
            updateRunStatus("IGainAdj");
            double iRMSAdjust = iRMSGain * _sq.IRef;
            int iAdjustInt = (int)(iRMSAdjust * 0x400000);
            updateOutputStatus(string.Format("IrmsAdjust = {0:F8} (0x{1:X})", iRMSAdjust, iAdjustInt));

            // VGainAdj
            updateRunStatus("VGainAdj");
            double vRMSAdjust = vRMSGain * _sq.VRef;
            int vAdjustInt = (int)(vRMSAdjust * 0x400000);
            updateOutputStatus(string.Format("VrmsAdjust = {0:F8} (0x{1:X})", vRMSAdjust, vAdjustInt));

            int n = 0;
            while (this.toolStripMenuItemPowerMeter.CheckState == CheckState.Checked)
            {
                double v_cs = _sq.GetVRMS();
                double i_cs = _sq.GetIRMS();
                double power = v_cs * i_cs;

                updateOutputStatus(string.Format("{0:F8} V * {1:F8} A = {2:F8} W", v_cs, i_cs, power));

                if (n > 100)
                    break;
            }

            // PatchingGain
            updateRunStatus("PatchingGain");
            _relay_ctrl.AC_Power = false;
            _relay_ctrl.Load = false;
            _relay_ctrl.Reset = false;
            _relay_ctrl.Ember = true;
            relaysSet(_relay_ctrl);
            Thread.Sleep(1000);

            Ember ember = new Ember();
            ember.EmberBinPath = Properties.Settings.Default.Ember_BinPath;
            ember.BatchFilePath = _ember_batchfile_path;
            switch (board)
            {
                case (CSSequencer.BoardTypes.Humpback):
                    ember.VAdress = 0x08080980;
                    ember.IAdress = 0x08080984;
                    ember.RefereceAdress = 0x08080988;
                    ember.ACOffsetAdress = 0x080809CC;

                    // TODO:  Need to do the same for 120 V (Hookt0oth)
                    ember.VRefereceValue = 0xF0; // 240 V
                    ember.IRefereceValue = 0x0F; // 15 A

                    break;
                case (CSSequencer.BoardTypes.Hooktooth):
                case (CSSequencer.BoardTypes.Milkshark):
                    ember.VAdress = 0x08040980;
                    ember.IAdress = 0x08040984;
                    ember.ACOffsetAdress = 0x080409CC;
                    break;
            }
            ember.CreateCalibrationPatchBath(vAdjustInt, iAdjustInt);

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

            }

            if (patchit_fail)
            {
                throw new Exception(exception_msg);
            }
            updateOutputStatus(coding_output);

            // Disconnect Ember
            _relay_ctrl.Ember = false;
            relaysSet(_relay_ctrl);

            updateOutputStatus("================================End Calibration===============================");

            if (_sq != null)
                _sq.CloseSerialPort();
            if (_meter != null)
                _meter.CloseSerialPort();


            if (true)
            {
                this.textBoxRunStatus.BackColor = Color.Green;
                this.textBoxRunStatus.ForeColor = Color.White;
                this.textBoxRunStatus.Text = "PASS";
            }
        }

        /// <summary>
        /// Invokes the DIO test dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void digitalOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDigitalPortTest dlg = new FormDigitalPortTest();
            //dlg.ShowDialog();
            dlg.Show();
        }

        /// <summary>
        /// Invikes the settings dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettings dlg = new FormSettings();

            // COM ports
            dlg.TextBoxCirrusCOM.Text = Properties.Settings.Default.CS_COM_Port_Name;

            dlg.TextBoxMeterCOM.Text = Properties.Settings.Default.Meter_COM_Port_Name;
            dlg.CheckBoxManualMultiMeter.Checked = Properties.Settings.Default.Meter_Manual_Measurement;


            // DIO Disable
            dlg.checkBoxDisableDIO.Checked = Properties.Settings.Default.Manual_Relay_Control;

            // DIO line assigment
            dlg.NumericUpDownACPower.Value = Properties.Settings.Default.DIO_ACPower_LineNum;
            dlg.NumericUpDownLoad.Value = Properties.Settings.Default.DIO_Load_LinNum;
            dlg.NumericUpDownReset.Value = Properties.Settings.Default.DIO_Reset_LineNum;
            dlg.NumericUpDownEmber.Value = Properties.Settings.Default.DIO_Ember_LineNum;

            // Ember
            dlg.TextBoxEmberBinPath.Text = Properties.Settings.Default.Ember_BinPath;

            DialogResult rc = dlg.ShowDialog();
            if (rc == DialogResult.OK)
            {
                // COM ports
                Properties.Settings.Default.CS_COM_Port_Name = dlg.TextBoxCirrusCOM.Text;

                Properties.Settings.Default.Meter_COM_Port_Name = dlg.TextBoxMeterCOM.Text;
                Properties.Settings.Default.Meter_Manual_Measurement = dlg.CheckBoxManualMultiMeter.Checked;

                // DIO Disable
                Properties.Settings.Default.Manual_Relay_Control = dlg.checkBoxDisableDIO.Checked;

                // DIO line assigment
                Properties.Settings.Default.DIO_ACPower_LineNum = (int)dlg.NumericUpDownACPower.Value;
                Properties.Settings.Default.DIO_Load_LinNum = (int)dlg.NumericUpDownLoad.Value;
                Properties.Settings.Default.DIO_Reset_LineNum = (int)dlg.NumericUpDownReset.Value;
                Properties.Settings.Default.DIO_Ember_LineNum = (int)dlg.NumericUpDownEmber.Value;

                // Ember
                Properties.Settings.Default.Ember_BinPath = dlg.TextBoxEmberBinPath.Text;

                Properties.Settings.Default.Save();
            }
        }

        private void calculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCalculator dlg = new FormCalculator();
            dlg.ShowDialog();
        }


    }
}
