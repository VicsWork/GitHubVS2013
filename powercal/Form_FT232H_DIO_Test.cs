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

namespace PowerCalibration
{
    public partial class Form_FT232H_DIO_Test : Form
    {
        RelayControler _relayCtrl;

        public Form_FT232H_DIO_Test(RelayControler relayCtrl)
        {
            InitializeComponent();
            Icon = Properties.Resources.IconPowerCalibration;

            if (relayCtrl.Device_Type != RelayControler.Device_Types.FT232H)
                throw new Exception("Incorrect relay controller");
            else
                _relayCtrl = relayCtrl;

            int dev_id = _relayCtrl.FTDI_DEVICE_ID;
            Label_physicalChannel.Text = string.Format("{0} {1} {2}", _relayCtrl.FTDI_DEVICE_YPE, _relayCtrl.FTDI_DEVICE_ID, _relayCtrl.FTDI_BUS); 

            // Add line numbers to labels and tag the control with the line number
            Dictionary<string, uint> lines = _relayCtrl.DicLines_ReadSettings();

            uint linenum = lines[PowerCalibration.Relay_Lines.Power];
            labelACPower.Text += string.Format("({0})", linenum);
            NumericUpDown_ACPower.Tag = linenum;
            NumericUpDown_ACPower.Value = Convert.ToDecimal(_relayCtrl.ReadLine(linenum));

            linenum = lines[PowerCalibration.Relay_Lines.Load];
            labelLoad.Text += string.Format("({0})", linenum);
            NumericUpDown_Load.Tag = linenum;
            NumericUpDown_Load.Value = Convert.ToDecimal(_relayCtrl.ReadLine(linenum));

            linenum = lines[PowerCalibration.Relay_Lines.Ember];
            labelEmber.Text += string.Format("({0})", linenum);
            NumericUpDown_Ember.Tag = linenum;
            NumericUpDown_Ember.Value = Convert.ToDecimal(_relayCtrl.ReadLine(linenum));

            linenum = lines[PowerCalibration.Relay_Lines.Voltmeter];
            labelVoltmeter.Text += string.Format("({0})", linenum);
            numericUpDown_Voltmeter.Tag = linenum;

            updateLineValues();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown ctrl = (NumericUpDown)sender;
            uint line_num = (uint)ctrl.Tag;
            bool value = Convert.ToBoolean(ctrl.Value);


            _relayCtrl.WriteLine(line_num, value);
        }

        private void buttonAllOff_Click(object sender, EventArgs e)
        {
            _relayCtrl.WriteAll(false);

            updateLineValues();
        }

        void updateLineValues()
        {
            var ctrls = groupBoxDIOLines.Controls.OfType<NumericUpDown>();
            foreach (NumericUpDown ctrl in ctrls)
                ctrl.Value = Convert.ToDecimal(_relayCtrl.ReadLine((uint)ctrl.Tag));
        }
    }
}
