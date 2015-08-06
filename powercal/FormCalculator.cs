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
    public partial class FormCalculator : Form
    {
        public FormCalculator()
        {
            InitializeComponent();
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.textBoxHex24.Text.Length > 0)
                {
                    int hexval = Convert.ToInt32(textBoxHex24.Text, 16);
                    this.textBox2.Text = string.Format("{0}", bit24_ToDouble(hexval));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static double bit24_ToDouble(int data)
        {
            // Maximum 1 =~ 0xFFFFFF
            // Max rms 0.6 =~ 0x999999
            // Half rms 0.36 =~ 0x5C28F6
            double value = (double)(data)/0x1000000; // 2^24
            return value;
        }

        private void buttonCalculateGain_Click(object sender, EventArgs e)
        {
            if (textBoxGainDec.Text.Length > 0)
            {
                double dvalue = Convert.ToDouble(textBoxGainDec.Text);
                int ival = (int)(dvalue * 0x400000);
                this.textBoxGainHex.Text = string.Format("{0:X}", ival);

            }
        }


    }
}
