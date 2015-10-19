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

            CheckBoxManualMultiMeter.Checked = Properties.Settings.Default.Meter_Manual_Measurement;
            TextBoxMeterCOM.Text = Properties.Settings.Default.Meter_COM_Port_Name;

            // Pupulate DIO controller types
            comboBoxDIOCtrollerTypes.Items.Clear();
            Array relay_types = Enum.GetValues(typeof(RelayControler.Device_Types));
            foreach (RelayControler.Device_Types relay_type in relay_types)
                comboBoxDIOCtrollerTypes.Items.Add(relay_type.ToString());
            comboBoxDIOCtrollerTypes.Text = Properties.Settings.Default.Relay_Controller_Type;

            // Ember
            comboBoxEmberInterface.Text = Properties.Settings.Default.Ember_Interface;
            if (Properties.Settings.Default.Ember_Interface == "IP")
                textBoxEmberInterfaceAddress.Text = Properties.Settings.Default.Ember_Interface_IP_Address;
            else
                textBoxEmberInterfaceAddress.Text = Properties.Settings.Default.Ember_Interface_USB_Address;
            TextBoxEmberBinPath.Text = Properties.Settings.Default.Ember_BinPath;

            comboBoxShortcutActions.Text = Properties.Settings.Default.Shortcut_Spacebar_Action;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void setLineEnablement(bool enable)
        {
            foreach (Control ctrl in groupBoxDIO.Controls)
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
            }
            else
            {
                TextBoxMeterCOM.Enabled = true;
            }

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
                    RelayControler rc = new RelayControler(device);
                    setLineEnablement(true);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("{0}\r\nTry unpluging and re-plug-in the USB device ", ex.Message);
                    MessageBox.Show(msg);
                    comboBoxDIOCtrollerTypes.Text = "Manual";
                    setLineEnablement(false);
                }
            }
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

        private void buttonCodingSetXY_Click(object sender, EventArgs e)
        {
            MouseHook.Start();
            MouseHook.MouseAction += MouseHook_MouseAction;
        }

        void MouseHook_MouseAction(object sender, EventArgs e)
        {
            MouseHook.Stop();

            MouseHook.POINT p = new MouseHook.POINT();
            MouseHook.GetCursorPos(out p);
        }

        private void comboBoxShortcutActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Shortcut_Spacebar_Action = comboBoxShortcutActions.Text;
        }

    }
}
