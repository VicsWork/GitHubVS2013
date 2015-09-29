using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace powercal
{
    public partial class Form_Settings : Form
    {
        public Form_Settings()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void FormSettings_Load(object sender, EventArgs e)
        {
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
                setLineEnablement(false);
            else
                setLineEnablement(true);
        }

    }
}
