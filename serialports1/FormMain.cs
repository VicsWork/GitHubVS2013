using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        string _log_file = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        delegate void SetTextCallback();

        public FormMain()
        {
            InitializeComponent();

            initLogFile();

            initTextBoxRunStatus();

            // Set the title to match assembly
            AboutBox1 aboutdlg = new AboutBox1();
            this.Text = aboutdlg.AssemblyTitle;
            aboutdlg.Dispose();

            this.comboBoxBoardTypes.Items.AddRange(Enum.GetNames(typeof(CSSequencer.BoardTypes)));
            if (comboBoxBoardTypes.Items.Count > 0)
            {
                comboBoxBoardTypes.SelectedIndex = 0;
            }

            autoDetectMeterCOMPort();
            string msg = string.Format("Cirrus Logic comunications port = {0}", Properties.Settings.Default.CS_COM_Port_Name);
            updateOutputStatus(msg);


        }

        void autoDetectMeterCOMPort()
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
                string msg = string.Format("Unable to detect Multimetter comunications port. Using {0}", Properties.Settings.Default.Meter_COM_Port_Name);
                updateOutputStatus(msg);
            }

        }
        void initLogFile()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            _log_file = Path.Combine(dir, "runlog.txt");
            if (!File.Exists(_log_file))
            {
                using (StreamWriter sw = File.CreateText(_log_file))
                {
                    sw.WriteLine("{0:G}: Log created", DateTime.Now);
                    sw.Close();
                }
            }
        }

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

        private void updateRunStatus(string txt)
        {
            this.textBoxRunStatus.Text = txt;
            this.textBoxRunStatus.Update();
        }

        private void serialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSerialTest dlg = new FormSerialTest();
            DialogResult result = dlg.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 dlg = new AboutBox1();
            DialogResult result = dlg.ShowDialog();
        }

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

                key = "Output";
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

        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            this.textBoxOutputStatus.Clear();
            initTextBoxRunStatus();
            try
            {
                calibrate();
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
                _relay_ctrl.AC_Power = false;
                _relay_ctrl.Output = false;
                _relay_ctrl.Load = false;
                _relay_ctrl.Reset = false;
                relaysSet(_relay_ctrl);

                if (_sq != null)
                    _sq.CloseSerialPort();

                if (_meter != null)
                    _meter.CloseSerialPort();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxOutputStatus.Clear();
        }

        private void calibrate()
        {
            string msg;
            updateOutputStatus("===============================Start Calibration==============================");
            string csPortName = Properties.Settings.Default.CS_COM_Port_Name;
            CSSequencer.BoardTypes board = (CSSequencer.BoardTypes)Enum.Parse(typeof(CSSequencer.BoardTypes), comboBoxBoardTypes.Text);
            _sq = new CSSequencer(csPortName, board);

            // Setup multi-meter
            updateRunStatus("Setup multi-meter");
            string meterPortName = Properties.Settings.Default.Meter_COM_Port_Name;
            _meter = new MultiMeter(meterPortName);
            _meter.OpenComPort();
            _meter.SetToRemote();
            _meter.ClearError();
            string idn = _meter.IDN();

            // IOffsetPre_Cal
            updateRunStatus("IOffsetPre_Cal");
            bool manual_relay = Properties.Settings.Default.Manual_Relay_Control;
            if (manual_relay)
                _relay_ctrl.Disable = true;
            _relay_ctrl.AC_Power = true;
            _relay_ctrl.Reset = true;
            _relay_ctrl.Output = true;
            _relay_ctrl.Load = false;
            relaysSet(_relay_ctrl);
            Thread.Sleep(1000);

            _sq.openSerialPort();

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
            _relay_ctrl.Load = true;
            relaysSet(_relay_ctrl);
            Thread.Sleep(1000);

            double iRMSPreCal = _sq.GetIRMS();
            if (iRMSPreCal < 0.17 || iRMSPreCal > 0.3)
            {
                // With 500 Ohms load and 120VAC this value should be around 0.24
                msg = string.Format("Bad IrmsPreCal value:{0:F8}", iRMSPreCal);
                throw new Exception(msg);
            }
            updateOutputStatus(string.Format("IrmsPreCal = {0:F8}", iRMSPreCal));

            // VRMSPre_Cal
            updateRunStatus("VRMSPre_Cal");
            double vRMSPreCal = _sq.GetVRMS();
            updateOutputStatus(string.Format("VrmsPreCal = {0:F8}", vRMSPreCal));
            if (vRMSPreCal < 100 || vRMSPreCal > 150)
            {
                msg = string.Format("Bad VrmsPreCal value:{0:F}", vRMSPreCal);
                throw new Exception(msg);
            }

            // IRMSMeasure
            updateRunStatus("IRMSMeasure");
            _meter.SetupForIAC();
            string iac_measurement = _meter.Measure();
            double iRMSMeasure = Double.Parse(iac_measurement);
            updateOutputStatus(string.Format("IrmsMeasure = {0:F8}", iRMSMeasure));

            Thread.Sleep(1000);

            // VRMSMeasure
            updateRunStatus("VRMSMeasure");
            _meter.SetupForVAC();
            string vac_measurement = _meter.Measure();
            double vRMSMeasure = Double.Parse(vac_measurement);
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
            int vRMSGainInt = (int)(iRMSGain * 0x400000);
            updateOutputStatus(string.Format("VrmsGain = {0:F8} (0x{1:X})", vRMSGain, vRMSGainInt));
            _sq.VGainCal(vRMSGainInt);

            // IRMSNoLoad
            _relay_ctrl.Load = false;
            relaysSet(_relay_ctrl);
            Thread.Sleep(1000);

            updateRunStatus("IRMSNoLoad");

            iRMSNoLoad = _sq.IRMSNoLoad();
            updateOutputStatus(string.Format("IrmsNoLoad = {0:F8}", iRMSNoLoad));
            if (iRMSNoLoad > 0.05)
            {
                msg = string.Format("Bad IrmsNoLoad value:{0:F8}", iRMSNoLoad);
                throw new Exception(msg);
            }

            _relay_ctrl.Load = true;
            relaysSet(_relay_ctrl);
            Thread.Sleep(1000);

            // IRMSAfter_Cal
            updateRunStatus("IRMSAfter_Cal");

            double iRMSAfterCal = _sq.GetIRMS();
            updateOutputStatus(string.Format("IrmsAfterCal = {0:F8}", iRMSAfterCal));
            double delta = iRMSMeasure * 0.03;
            double lowlimit = iRMSMeasure - delta;
            double highlimit = iRMSMeasure + delta;
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


            // PatchingGain
            updateRunStatus("PatchingGain");
            _relay_ctrl.AC_Power = false;
            _relay_ctrl.Output = false;
            _relay_ctrl.Load = false;
            _relay_ctrl.Reset = false;
            relaysSet(_relay_ctrl);

            Ember ember = new Ember();
            ember.CreateCalibrationPachBath(vAdjustInt, iAdjustInt);
            ember.CreateCalibrationPachBath(0x7D3012, 0x35D6FF);
            string coding_output = "";
            try
            {
                string output = ember.RunCalibrationPatchBatch();
                if (output.Contains("ERROR:"))
                {
                    msg = "Patching error detected:\r\n";
                    msg += output;
                    throw new Exception(msg);
                }
                coding_output = output;
            }
            catch (Exception e)
            {
                msg = "Patching exception detected:\r\n";
                msg += e.Message;
                throw new Exception(msg);
            }
            this.updateOutputStatus(coding_output);

            updateOutputStatus("================================End Calibration===============================");

            _sq.CloseSerialPort();
            _meter.CloseSerialPort();


            if (true)
            {
                this.textBoxRunStatus.BackColor = Color.Green;
                this.textBoxRunStatus.ForeColor = Color.White;
                this.textBoxRunStatus.Text = "PASS";
            }
        }

        private void digitalOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDigitalPortTest dlg = new FormDigitalPortTest();
            dlg.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettings dlg = new FormSettings();
            dlg.textBoxCirrusCOM.Text = Properties.Settings.Default.CS_COM_Port_Name;
            dlg.textBoxMeterCOM.Text = Properties.Settings.Default.Meter_COM_Port_Name;
            DialogResult rc = dlg.ShowDialog();
            if (rc == DialogResult.OK)
            {

            }
        }


    }
}
