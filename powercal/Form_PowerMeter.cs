using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using MinimalisticTelnet;

namespace powercal
{
    public partial class Form_PowerMeter : Form
    {
        Process _p_ember_isachan;
        string _isachan_error;
        TelnetConnection _telnet_connection = null;
        string _cmd_prefix;

        public Form_PowerMeter()
        {
            InitializeComponent();

            openEmberISAChannels();

            _telnet_connection = new TelnetConnection("localhost", 4900);
            _cmd_prefix = TCLI.Get_Custom_Command_Prefix(_telnet_connection);

        }

        private void buttonStartCS_Click(object sender, EventArgs e)
        {
            while (true)
            {
                TCLI.Current_Voltage cv = get_uut_data();
                this.labelPowerCS.Text = string.Format("{0:F8}", cv.Voltage);
                this.labelPowerCS.Update();
            }

        }

        TCLI.Current_Voltage get_uut_data()
        {
            return TCLI.Parse_Pload_Registers(_telnet_connection, _cmd_prefix, 240, 10);
        }

        private void Form_PowerMeter_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Starts the process responsible to open the Ember box isa channels
        /// </summary>
        void openEmberISAChannels()
        {
            Ember ember = new Ember();
            ember.Process_ISAChan_Error_Event += ember_Process_ISAChan_Error_Event;
            //ember.Process_ISAChan_Output_Event += p_ember_isachan_OutputDataReceived;
            _p_ember_isachan = ember.OpenEmberISAChannels();
        }

        void ember_Process_ISAChan_Error_Event(object sender, DataReceivedEventArgs e)
        {
            _isachan_error += e.Data + "\n";
        }

        /// <summary>
        /// Closes the Ember process that open the isa channels
        /// <seealso cref="openEmberISAChannels"/>
        /// </summary>
        void closeEmberISAChannels()
        {
            if (_p_ember_isachan != null)
            {
                _p_ember_isachan.CancelErrorRead();
                _p_ember_isachan.CancelOutputRead();
                if (!_p_ember_isachan.HasExited)
                    _p_ember_isachan.Kill();
                _p_ember_isachan.Close();
            }
        }

        private void timerISAChan_Tick(object sender, EventArgs e)
        {
            if (_isachan_error != null || _isachan_error == "")
            {
                _isachan_error = null;
                closeEmberISAChannels();
                openEmberISAChannels();
            }

            if (_cmd_prefix == null)
            {
                if (_telnet_connection != null)
                {
                    _telnet_connection.Close();
                }

                try
                {
                    _telnet_connection = new TelnetConnection("localhost", 4900);
                    _cmd_prefix = TCLI.Get_Custom_Command_Prefix(_telnet_connection);
                }
                catch { };
            }
        }

        private void Form_PowerMeter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_telnet_connection != null)
            {
                _telnet_connection.Close();
            }

            closeEmberISAChannels();
        }

    }
}
