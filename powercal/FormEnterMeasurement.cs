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
    public partial class FormEnterMeasurement : Form
    {
        public FormEnterMeasurement()
        {
            InitializeComponent();
        }

        public double GetMeasurement(string label_txt)
        {
            this.label.Text = label_txt;
            ShowDialog();
            return Convert.ToDouble(this.textBox1.Text);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
