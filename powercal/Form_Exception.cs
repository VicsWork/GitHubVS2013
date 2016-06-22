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
        int _height_small = -1;
        int _height_large = -1;
        public Form_Exception(string message = "", string detail = "", string title = "Error")
        {
            InitializeComponent();

            Icon = Properties.Resources.Icon_PowerCalibration;
            Text = title;
            textBoxMessage.Text = message;
            textBoxDetail.Text = detail;
            textBoxDetail.Visible = false;

            _height_large = this.Height;
            this.Height -= textBoxDetail.Height;
            _height_small = this.Height;
        }

        private void buttonMore_Click(object sender, EventArgs e)
        {
            if (!textBoxDetail.Visible)
            {
                //textBoxDetail.Anchor = System.Windows.Forms.AnchorStyles.None;
                //textBoxDetail.Update();
                //Update();

                buttonMore.Text = "&Less";
                //this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
                textBoxDetail.Visible = true;
                this.Height = _height_large;

            }
            else
            {
                buttonMore.Text = "&More";
                textBoxDetail.Visible = false;
                this.Height = _height_small;
            }
        }
    }
}
