using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MinimalisticTelnet;

namespace powercal
{
    public partial class Form_PowerMeter : Form
    {

        public Form_PowerMeter()
        {
            InitializeComponent();
        }

        private void buttonStartCS_Click(object sender, EventArgs e)
        {

        }

        TCLI.Current_Voltage get_uut_data()
        {
            return new TCLI.Current_Voltage();
        }
    }
}
