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
        public FormDigitalPortTest()
        {
            InitializeComponent();

            initphysicalChannelComboBox();
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
    }
}
