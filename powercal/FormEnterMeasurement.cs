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
    /// <summary>
    /// Simple form to enter a single value
    /// </summary>
    public partial class FormEnterMeasurement : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FormEnterMeasurement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the dialog
        /// </summary>
        /// <param name="label_txt">Text to appear on label next to the measurement text box</param>
        /// <returns></returns>
        public double GetMeasurement(string label_txt)
        {
            this.label.Text = label_txt;
            ShowDialog();
            return Convert.ToDouble(this.textBox1.Text);
        }

        /// <summary>
        /// Handle OK click. Close dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
