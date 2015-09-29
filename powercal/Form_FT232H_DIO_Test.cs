using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace powercal
{
    public partial class Form_FT232H_DIO_Test : Form
    {
        RelayControler _relayCtrl;

        public Form_FT232H_DIO_Test(RelayControler relayCtrl)
        {
            InitializeComponent();

            if (relayCtrl.Device_Type != RelayControler.Device_Types.FT232H)
                throw new Exception("Incorrect relay controller");
            else
                this._relayCtrl = relayCtrl;

            int dev_id = _relayCtrl.FTDI_DEVICE_ID;
            this.physicalChannelLabel.Text = string.Format("{0} {1} {2}", _relayCtrl.FTDI_DEVICE_YPE, _relayCtrl.FTDI_DEVICE_ID, _relayCtrl.FTDI_BUS); 

            // Add line numbers to labels and tag the control with the line number
            Dictionary<string, uint> lines = _relayCtrl.DicLines_ReadSettings();

            uint linenum = lines[powercal.Relay_Lines.Power];
            labelACPower.Text += string.Format("({0})", linenum);
            NumericUpDown_ACPower.Tag = linenum;

            linenum = lines[powercal.Relay_Lines.Load];
            labelLoad.Text += string.Format("({0})", linenum);
            NumericUpDown_Load.Tag = linenum;

            linenum = lines[powercal.Relay_Lines.Ember];
            labelEmber.Text += string.Format("({0})", linenum);
            NumericUpDown_Ember.Tag = linenum;

            linenum = lines[powercal.Relay_Lines.Voltmeter];
            labelVoltmeter.Text += string.Format("({0})", linenum);
            numericUpDown_Voltmeter.Tag = linenum;

        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown ctrl = (NumericUpDown)sender;
            uint line_num = (uint)ctrl.Tag;
            bool value = Convert.ToBoolean(ctrl.Value);


            _relayCtrl.WriteLine(line_num, value);
        }

        private void FormFT232H_DIO_Test_FormClosed(object sender, FormClosedEventArgs e)
        {
            _relayCtrl.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _relayCtrl.WriteAll(false);
            Thread.Sleep(500);
            _relayCtrl.WriteAll(true);
        }

    }
}
