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
    public partial class FormSettings : Form
    {
        public FormSettings()
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

        private void checkBoxDisableDIO_CheckedChanged(object sender, EventArgs e)
        {
            setLineEnablement(!checkBoxDisableDIO.Checked);
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
    }
}
