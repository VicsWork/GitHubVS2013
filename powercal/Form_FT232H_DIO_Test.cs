﻿using System;
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
            Icon = Properties.Resources.Icon_PowerCalibration;

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

            linenum = lines[PowerCalibration.Relay_Lines.Vac_Vdc];
            labelVacVdc.Text += string.Format("({0})", linenum);
            numericUpDown_VacVdc.Tag = linenum;
            numericUpDown_VacVdc.Value = Convert.ToDecimal(_relayCtrl.ReadLine(linenum));

            Dictionary<string, uint> relay_lines = _relayCtrl.Dictionary_Lines;
            linenum = 4;
            //if(_relayCtrl.Dictionary_Lines.Count > linenum){

            //    labelTest_VacVdc.Text = relay_lines.ElementAt((int)linenum).Key;
            //}
            labelTest_VacVdc.Text += string.Format("({0})", linenum);
            numericUpDown_Test_VacVdc.Tag = linenum;
            numericUpDown_Test_VacVdc.Value = Convert.ToDecimal(_relayCtrl.ReadLine(linenum));

            linenum = 5;
            labelTestC5.Text += string.Format("({0})", linenum);
            numericUpDown_TestC5.Tag = linenum;
            numericUpDown_TestC5.Value = Convert.ToDecimal(_relayCtrl.ReadLine(linenum));

            linenum = 6;
            labelTestC6.Text += string.Format("({0})", linenum);
            numericUpDown_TestC6.Tag = linenum;
            numericUpDown_TestC6.Value = Convert.ToDecimal(_relayCtrl.ReadLine(linenum));

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
            {
                try
                {
                    ctrl.Value = Convert.ToDecimal(_relayCtrl.ReadLine((uint)ctrl.Tag));
                }
                catch { }
            }
        }
    }
}
