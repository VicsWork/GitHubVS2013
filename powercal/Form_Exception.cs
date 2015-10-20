using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerCalibration
{
    public partial class Form_Exception : Form
    {
        public Form_Exception(string message="", string detail="", string title="Error")
        {
            InitializeComponent();

            Icon = Properties.Resources.IconPowerCalibration;
            Text = title;
            textBoxMessage.Text = message;
            textBoxDetail.Text = detail;
            textBoxDetail.Visible = false;
        }

        private void buttonMore_Click(object sender, EventArgs e)
        {
            if (!textBoxDetail.Visible)
            {
                textBoxDetail.Visible = true;
                buttonMore.Text = "&Less";
                this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;

            }
            else
            {
                buttonMore.Text = "&More";
                textBoxDetail.Visible = false;
                this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            }
        }
    }
}
