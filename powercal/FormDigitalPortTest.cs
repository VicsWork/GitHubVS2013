using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.DAQmx;

namespace powercal
{
    public partial class FormDigitalPortTest : Form
    {
        RelayControler _relayCtrl = new RelayControler();

        public FormDigitalPortTest()
        {
            InitializeComponent();

            initphysicalChannelComboBox();

            refreshNumericUpDownValue();
        }

        void initphysicalChannelComboBox()
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
            }
        }

        private void refreshNumericUpDownValue()
        {
            try
            {

                // Reads all values from DIO and updates the numeric up and down control
                NumericUpDownACPower.Value = Convert.ToDecimal(_relayCtrl.AC_Power);
                NumericUpDownLoad.Value = Convert.ToDecimal(_relayCtrl.Load);
                NumericUpDownReset.Value = Convert.ToDecimal(_relayCtrl.Reset);
                NumericUpDownOutput.Value = Convert.ToDecimal(_relayCtrl.Output);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception");
            }

        }

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
                    writer.WriteSingleSamplePort(true, (UInt32)dataToWriteNumericUpDown.Value);
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

        private void NumericUpDownReset_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;
            _relayCtrl.Reset = Convert.ToBoolean(num.Value);
        }

        private void NumericUpDownOutput_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;
            _relayCtrl.Output = Convert.ToBoolean(num.Value);

        }
    }
}
