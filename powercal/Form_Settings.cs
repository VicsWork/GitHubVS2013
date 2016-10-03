using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerCalibration
{
    public partial class Form_Settings : Form
    {
        TabPage _superTab;
        public Form_Settings()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon_PowerCalibration;

            // Remove the supre tab
            _superTab = tabPageSuper;
            TabControl.TabPages.Remove(tabPageSuper);

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D))
            {
                labelDBConnectStr.Visible = !labelDBConnectStr.Visible;

                if (_superTab != null)
                {
                    TabControl.TabPages.Add(_superTab);
                    _superTab = null;
                }
                else
                {
                    _superTab = tabPageSuper;
                    TabControl.TabPages.Remove(tabPageSuper);
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.labelDBConnectStr.Visible = false;

            // Ember
            comboBox_EmberInterface.Text = Properties.Settings.Default.Ember_Interface;
            textBox_EmberBinPath.Text = Properties.Settings.Default.Ember_BinPath;

            // Populate DIO controller types
            comboBox_DIOCtrollerTypes.Items.Clear();
            Array relay_types = Enum.GetValues(typeof(RelayControler.Device_Types));
            foreach (RelayControler.Device_Types relay_type in relay_types)
                comboBox_DIOCtrollerTypes.Items.Add(relay_type.ToString());
            comboBox_DIOCtrollerTypes.Text = Properties.Settings.Default.Relay_Controller_Type;

            // Calibration
            double v = Properties.Settings.Default.CalibrationLoadVoltageValue;
            double r = Properties.Settings.Default.CalibrationLoadResistorValue;

            textBox_LoadVoltageValue.Text = string.Format("{0:F2}", v);
            textBox_LoadResitorValue.Text = string.Format("{0:F2}", r);

            // Multimeter (Measurement)
            checkBox_ManualMultiMeter.Checked = Properties.Settings.Default.Meter_Manual_Measurement;
            TextBox_MeterCOM.Text = Properties.Settings.Default.Meter_COM_Port_Name;
            checkBox_PreProTest.Checked = Properties.Settings.Default.PrePost_Test_Enabled;

            // DB
            checkBox_EnableDBReporting.Checked = Properties.Settings.Default.DB_Loging_Enabled;

            // Coding
            checkBoxCode_MinOnPass.Checked = Properties.Settings.Default.CodeMinimizedOnPASS;

            // Super
            checkBox_enableRdProt.Checked = Properties.Settings.Default.Ember_ReadProtect_Enabled;

            // Play Sounds
            checkBox_PlaySounds.Checked = Properties.Settings.Default.Play_Sounds;

            // Super Disable Rd Protect before coding
            checkBox_disableRdProtectionBeforeCode.Checked = Properties.Settings.Default.Disable_ReadProtection_BeforeCoding;

        }

        private void comboBox_DIOCtrollerTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_DIOCtrollerTypes.Text == "Manual")
            {
                setLineEnablement(false);
            }
            else
            {
                try
                {
                    RelayControler.Device_Types device = (RelayControler.Device_Types)Enum.Parse(
                        typeof(RelayControler.Device_Types), comboBox_DIOCtrollerTypes.Text);

                    RelayControler relay_ctrl = new RelayControler(device);

                    Dictionary<string, uint> relay_lines = relay_ctrl.DicLines_ReadSettings();

                    NumericUpDown_ACPower.Value = relay_lines[PowerCalibration.Relay_Lines.Power];
                    NumericUpDown_Load.Value = relay_lines[PowerCalibration.Relay_Lines.Load];
                    NumericUpDown_Ember.Value = relay_lines[PowerCalibration.Relay_Lines.Ember];
                    numericUpDown_VacVdc.Value = relay_lines[PowerCalibration.Relay_Lines.Vac_Vdc];

                    setLineEnablement(true);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("{0}\r\nTry unplugging and re-plugging the USB device ", ex.Message);
                    MessageBox.Show(msg);
                    comboBox_DIOCtrollerTypes.Text = "Manual";
                    setLineEnablement(false);
                }
            }

            Properties.Settings.Default.Relay_Controller_Type = comboBox_DIOCtrollerTypes.Text;

        }

        private void NumericUpDown_DIOLine_ValueChanged(object sender, EventArgs e)
        {
            RelayControler.Device_Types device = (RelayControler.Device_Types)Enum.Parse(
                typeof(RelayControler.Device_Types), comboBox_DIOCtrollerTypes.Text);

            RelayControler relay_ctrl = new RelayControler(device);

            Dictionary<string, uint> relay_lines = relay_ctrl.DicLines_ReadSettings();
            relay_lines[PowerCalibration.Relay_Lines.Power] = (uint)NumericUpDown_ACPower.Value;
            relay_lines[PowerCalibration.Relay_Lines.Load] = (uint)NumericUpDown_Load.Value;
            relay_lines[PowerCalibration.Relay_Lines.Ember] = (uint)NumericUpDown_Ember.Value;
            relay_lines[PowerCalibration.Relay_Lines.Vac_Vdc] = (uint)numericUpDown_VacVdc.Value;

            relay_ctrl.Dictionary_Lines = relay_lines;
            relay_ctrl.DicLines_SaveSettings();

        }

        private void setLineEnablement(bool enable)
        {
            foreach (Control ctrl in this.tabPageDIO.Controls)
            {
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    ctrl.Enabled = enable;
                }
            }
        }

        private void checkBox_manualMultiMeter_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ManualMultiMeter.Checked)
            {
                TextBox_MeterCOM.Enabled = false;
                this.checkBox_PreProTest.Checked = false;
                this.checkBox_PreProTest.Enabled = false;
            }
            else
            {
                TextBox_MeterCOM.Enabled = true;
                this.checkBox_PreProTest.Enabled = true;
                this.checkBox_PreProTest.Checked = Properties.Settings.Default.PrePost_Test_Enabled;
            }

            Properties.Settings.Default.Meter_Manual_Measurement = checkBox_ManualMultiMeter.Checked;

        }

        private void checkBoxPreProTest_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PrePost_Test_Enabled = this.checkBox_PreProTest.Checked;
        }

        private void TextBoxMeterCOM_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Meter_COM_Port_Name = TextBox_MeterCOM.Text;
        }

        private void comboBox_EmberInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Ember_Interface = comboBox_EmberInterface.Text;

            if (comboBox_EmberInterface.Text == "IP")
                textBox_EmberInterfaceAddress.Text = Properties.Settings.Default.Ember_Interface_IP_Address;
            else
                textBox_EmberInterfaceAddress.Text = Properties.Settings.Default.Ember_Interface_USB_Address;
        }

        private void textBox_EmberInterfaceAddress_TextChanged(object sender, EventArgs e)
        {
            if (comboBox_EmberInterface.Text == "IP")
                Properties.Settings.Default.Ember_Interface_IP_Address = textBox_EmberInterfaceAddress.Text;
            else
                Properties.Settings.Default.Ember_Interface_USB_Address = textBox_EmberInterfaceAddress.Text;
        }

        private void TextBox_EmberBinPath_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Ember_BinPath = textBox_EmberBinPath.Text;
        }

        private void button_EmberBinPathBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = textBox_EmberBinPath.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_EmberBinPath.Text = dlg.SelectedPath;
            }
        }

        private void textBox_loadValues_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double v = Properties.Settings.Default.CalibrationLoadVoltageValue;
                double r = Properties.Settings.Default.CalibrationLoadResistorValue;

                double i = v / r;

                TextBox textbox = (TextBox)sender;
                string tag = (string)textbox.Tag;
                if (tag == "voltage")
                {
                    v = Convert.ToDouble(textBox_LoadVoltageValue.Text);
                    i = v / r;
                }
                else if (tag == "resistance")
                {
                    r = Convert.ToDouble(textBox_LoadResitorValue.Text);
                    i = v / r;
                }
                else if (tag == "power")
                {
                    double p = Convert.ToDouble(textBox_LoadPower.Text);
                    i = p / v;
                    r = v / i;
                }

                Properties.Settings.Default.CalibrationLoadVoltageValue = v;
                Properties.Settings.Default.CalibrationLoadResistorValue = r;


                if (tag == "voltage" || tag == "resistance")
                {
                    textBox_LoadPower.TextChanged -= textBox_loadValues_TextChanged;

                    textBox_LoadPower.Text = string.Format("{0:F2}", v * i);

                    textBox_LoadPower.TextChanged += textBox_loadValues_TextChanged;

                }
                else if (tag == "power")
                {
                    textBox_LoadResitorValue.TextChanged -= textBox_loadValues_TextChanged;

                    textBox_LoadResitorValue.Text = string.Format("{0:F2}", r);

                    textBox_LoadResitorValue.TextChanged -= textBox_loadValues_TextChanged;
                }

                textBox_LoadCurrent.Text = string.Format("{0:F2}", i);
            }
            catch (FormatException ex)
            {
                string msg = ex.Message;
            }

        }

        private void checkBox_enableDBReporting_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DB_Loging_Enabled = this.checkBox_EnableDBReporting.Checked;
            if (this.checkBox_EnableDBReporting.Checked)
                this.labelDBConnectStr.Text = Properties.Settings.Default.DBConnectionString;
            else
                this.labelDBConnectStr.Text = "";
        }

        private void checkBox_codeMinOnPass_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CodeMinimizedOnPASS = checkBoxCode_MinOnPass.Checked;
        }

        private void checkBox_enableRdProt_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Ember_ReadProtect_Enabled = checkBox_enableRdProt.Checked;
        }

        private void checkBox_playSounds_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Play_Sounds = checkBox_PlaySounds.Checked;
        }

        private void checkBox_disableRdProtectionBeforeCode_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Disable_ReadProtection_BeforeCoding = 
                checkBox_disableRdProtectionBeforeCode.Checked;
        }


    }
}
