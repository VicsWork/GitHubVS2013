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
    public partial class FormFT232H_DIO_Test : Form
    {
        RelayControler _relayCtrl = new RelayControler(RelayControler.Device_Types.FT232H);

        public FormFT232H_DIO_Test()
        {
            InitializeComponent();

            // Add line numbers to labels
            uint linenum = Properties.Settings.Default.DIO_ACPower_LineNum;
            _relayCtrl.AC_Power_LineNum = linenum;
            labelACPower.Text += string.Format("({0})", linenum);

            linenum = Properties.Settings.Default.DIO_Load_LinNum;
            _relayCtrl.Load_LineNum = linenum;
            labelLoad.Text += string.Format("({0})", linenum);

            linenum = Properties.Settings.Default.DIO_Ember_LineNum;
            _relayCtrl.Ember_LineNum = linenum;
            labelOutput.Text += string.Format("({0})", linenum);

            int dev_id = _relayCtrl.FTDI_DEVICE_ID;
            this.physicalChannelLabel.Text = string.Format("{0} {1} {2}", _relayCtrl.FTDI_DEVICE_YPE, _relayCtrl.FTDI_DEVICE_ID, _relayCtrl.FTDI_BUS); 

        }

        private void NumericUpDownACPower_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;
            _relayCtrl.AC_Power = Convert.ToBoolean(num.Value);
        }

        private void NumericUpDownLoad_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;
            _relayCtrl.Load = Convert.ToBoolean(num.Value);
        }

        private void NumericUpDownOutput_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;
            _relayCtrl.Ember = Convert.ToBoolean(num.Value);
        }

        private void FormFT232H_DIO_Test_FormClosed(object sender, FormClosedEventArgs e)
        {
            _relayCtrl.Close();
        }
    }
}
