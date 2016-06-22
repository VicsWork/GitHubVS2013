using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.DAQmx;

namespace PowerCalibration
{
    public partial class Form_NIDigitalPortTest : Form
    {
        RelayControler _relayCtrl = new RelayControler(RelayControler.Device_Types.NI_USB6008);

        /// <summary>
        /// Class use to perform tests on DIO board
        /// </summary>
        public Form_NIDigitalPortTest()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon_PowerCalibration;

            initPhysicalChannelComboBox();

            // Add line numbers to labels and tag the control with the line number
            Dictionary<string, uint> lines = _relayCtrl.DicLines_ReadSettings();

            uint linenum = lines[PowerCalibration.Relay_Lines.Power];
            labelACPower.Text += string.Format("({0})", linenum);
            NumericUpDown_ACPower.Tag = linenum;

            linenum = lines[PowerCalibration.Relay_Lines.Load];
            labelLoad.Text += string.Format("({0})", linenum);
            NumericUpDown_Load.Tag = linenum;

            linenum = lines[PowerCalibration.Relay_Lines.Ember];
            labelEmber.Text += string.Format("({0})", linenum);
            NumericUpDown_Ember.Tag = linenum;

            linenum = lines[PowerCalibration.Relay_Lines.Vac_Vdc];
            labelVoltmeter.Text += string.Format("({0})", linenum);
            numericUpDown_Voltmeter.Tag = linenum;

            if (this.physicalChannelComboBox.Text != "")
            {
                NumericUpDowndataToWrite.Value = rearPort();

                refreshNumericUpDownValue();
            }
        }

        /// <summary>
        /// Gets the dev and ports available and updates combo box
        /// </summary>
        void initPhysicalChannelComboBox()
        {
            physicalChannelComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External));
            if (physicalChannelComboBox.Items.Count > 0)
                physicalChannelComboBox.SelectedIndex = 0;
            else
            {
                physicalChannelComboBox.Items.Clear();
                physicalChannelComboBox.Text = "";
                physicalChannelComboBox.Enabled = false;

                writeButton.Enabled = false;

                setLineEnablement(false);
            }
        }

        /// <summary>
        /// Reads the DIO lines and updates the corresponding numeric up-down controls
        /// </summary>
        private void refreshNumericUpDownValue()
        {
            try
            {
                // Reads all values from DIO and updates the numeric up and down control
                NumericUpDown_ACPower.Value = Convert.ToDecimal( _relayCtrl.ReadLine(PowerCalibration.Relay_Lines.Power) );
                NumericUpDown_Load.Value = Convert.ToDecimal(_relayCtrl.ReadLine(PowerCalibration.Relay_Lines.Load) );
                NumericUpDown_Ember.Value = Convert.ToDecimal(_relayCtrl.ReadLine(PowerCalibration.Relay_Lines.Ember));
                numericUpDown_Voltmeter.Value = Convert.ToDecimal(_relayCtrl.ReadLine(PowerCalibration.Relay_Lines.Vac_Vdc));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception");
                setLineEnablement(false);
            }

        }

        /// <summary>
        /// Controls the enablement of all numeric up-down controls within the DIO lines group
        /// </summary>
        /// <param name="enable"></param>
        private void setLineEnablement(bool enable)
        {
            foreach (Control ctrl in groupBoxDIOLines.Controls)
            {
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    ctrl.Enabled = enable;
                }
            }
        }

        /// <summary>
        /// Writes the specified byte to the dev/port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void writeButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                using (Task digitalWriteTask = new Task())
                {

                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(physicalChannelComboBox.Text, "port0", ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, (UInt32)NumericUpDowndataToWrite.Value);
                }

                refreshNumericUpDownValue();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Reads the port
        /// </summary>
        /// <returns></returns>
        private byte rearPort()
        {
            Cursor.Current = Cursors.WaitCursor;
            byte data = 0;
            try
            {
                using (Task digitalReadTask = new Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalReadTask.DOChannels.CreateChannel(physicalChannelComboBox.Text, "port0", ChannelLineGrouping.OneChannelForAllLines);

                    //  Read digital port data. 
                    DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReadTask.Stream);
                    data = reader.ReadSingleSamplePortByte();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return data;
        }

        private void NumericUpDownr_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown ctrl = (NumericUpDown)sender;
            uint line_num = (uint) ctrl.Tag;
            bool value = Convert.ToBoolean(ctrl.Value);

            _relayCtrl.WriteLine(line_num, value);

            NumericUpDowndataToWrite.Value = rearPort();
        }

    }
}
