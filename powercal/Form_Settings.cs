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
        public Form_Settings()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.IconPowerCalibration;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            // Ember
            comboBoxEmberInterface.Text = Properties.Settings.Default.Ember_Interface;
            TextBoxEmberBinPath.Text = Properties.Settings.Default.Ember_BinPath;

            // Populate DIO controller types
            comboBoxDIOCtrollerTypes.Items.Clear();
            Array relay_types = Enum.GetValues(typeof(RelayControler.Device_Types));
            foreach (RelayControler.Device_Types relay_type in relay_types)
                comboBoxDIOCtrollerTypes.Items.Add(relay_type.ToString());
            comboBoxDIOCtrollerTypes.Text = Properties.Settings.Default.Relay_Controller_Type;

            // Multimeter (Measurement)
            CheckBoxManualMultiMeter.Checked = Properties.Settings.Default.Meter_Manual_Measurement;
            TextBoxMeterCOM.Text = Properties.Settings.Default.Meter_COM_Port_Name;
            checkBoxPreProTest.Checked = Properties.Settings.Default.PrePost_Test_Enabled;

            // Shortcuts
            comboBoxShortcutActions.Text = Properties.Settings.Default.Shortcut_Spacebar_Action;

            // DB
            this.checkBox_EnableDBReporting.Checked = Properties.Settings.Default.DB_Loging_Enabled;
        }

        private void comboBoxDIOCtrollerTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDIOCtrollerTypes.Text == "Manual")
            {
                setLineEnablement(false);
            }
            else
            {
                try
                {
                    RelayControler.Device_Types device = (RelayControler.Device_Types)Enum.Parse(
                        typeof(RelayControler.Device_Types), comboBoxDIOCtrollerTypes.Text);

                    RelayControler relay_ctrl = new RelayControler(device);

                    Dictionary<string, uint> relay_lines = relay_ctrl.DicLines_ReadSettings();
                    NumericUpDown_ACPower.Value = relay_lines[PowerCalibration.Relay_Lines.Power];
                    NumericUpDown_Load.Value = relay_lines[PowerCalibration.Relay_Lines.Load];
                    NumericUpDown_Ember.Value = relay_lines[PowerCalibration.Relay_Lines.Ember];
                    numericUpDown_Voltmeter.Value = relay_lines[PowerCalibration.Relay_Lines.Voltmeter];

                    setLineEnablement(true);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("{0}\r\nTry unplugging and re-plugging the USB device ", ex.Message);
                    MessageBox.Show(msg);
                    comboBoxDIOCtrollerTypes.Text = "Manual";
                    setLineEnablement(false);
                }
            }

            Properties.Settings.Default.Relay_Controller_Type = comboBoxDIOCtrollerTypes.Text;

        }


        private void NumericUpDown_DIOLine_ValueChanged(object sender, EventArgs e)
        {
            RelayControler.Device_Types device = (RelayControler.Device_Types)Enum.Parse(
                typeof(RelayControler.Device_Types), comboBoxDIOCtrollerTypes.Text);

            RelayControler relay_ctrl = new RelayControler(device);

            Dictionary<string, uint> relay_lines = relay_ctrl.DicLines_ReadSettings();
            relay_lines[PowerCalibration.Relay_Lines.Power] = (uint)NumericUpDown_ACPower.Value;
            relay_lines[PowerCalibration.Relay_Lines.Load] = (uint)NumericUpDown_Load.Value;
            relay_lines[PowerCalibration.Relay_Lines.Ember] = (uint)NumericUpDown_Ember.Value;
            relay_lines[PowerCalibration.Relay_Lines.Voltmeter] = (uint)numericUpDown_Voltmeter.Value;

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

        private void CheckBoxManualMultiMeter_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxManualMultiMeter.Checked)
            {
                TextBoxMeterCOM.Enabled = false;
                this.checkBoxPreProTest.Checked = false;
                this.checkBoxPreProTest.Enabled = false;
            }
            else
            {
                TextBoxMeterCOM.Enabled = true;
                this.checkBoxPreProTest.Enabled = true;
                this.checkBoxPreProTest.Checked = Properties.Settings.Default.PrePost_Test_Enabled;
            }

            Properties.Settings.Default.Meter_Manual_Measurement = CheckBoxManualMultiMeter.Checked;

        }

        private void checkBoxPreProTest_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PrePost_Test_Enabled = this.checkBoxPreProTest.Checked;
        }

        private void comboBoxShortcutActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Shortcut_Spacebar_Action = comboBoxShortcutActions.Text;
        }

        private void TextBoxMeterCOM_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Meter_COM_Port_Name = TextBoxMeterCOM.Text;
        }

        private void comboBoxEmberInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Ember_Interface = comboBoxEmberInterface.Text;

            if (comboBoxEmberInterface.Text == "IP")
                textBoxEmberInterfaceAddress.Text = Properties.Settings.Default.Ember_Interface_IP_Address;
            else
                textBoxEmberInterfaceAddress.Text = Properties.Settings.Default.Ember_Interface_USB_Address;
        }

        private void textBoxEmberInterfaceAddress_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxEmberInterface.Text == "IP")
                Properties.Settings.Default.Ember_Interface_IP_Address = textBoxEmberInterfaceAddress.Text;
            else
                Properties.Settings.Default.Ember_Interface_USB_Address = textBoxEmberInterfaceAddress.Text;
        }

        private void TextBoxEmberBinPath_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Ember_BinPath = TextBoxEmberBinPath.Text;
        }

        private void buttonEmberBinPathBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = TextBoxEmberBinPath.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxEmberBinPath.Text = dlg.SelectedPath;
            }
        }

    }
}
