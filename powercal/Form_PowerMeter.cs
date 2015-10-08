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
using System.Threading;

using MinimalisticTelnet;

namespace powercal
{
    public partial class Form_PowerMeter : Form
    {
        string _isachan_error;
        TelnetConnection _telnet_connection = null;
        string _cmd_prefix;
        Ember _ember;

        public delegate void CLIValueHandler(object sender, string text);
        public event CLIValueHandler CLIValue_Event;

        delegate void SetTextCallback(string txt);

        CancellationTokenSource _tokenSrc = new CancellationTokenSource();
        Task _task;


        public Form_PowerMeter()
        {
            InitializeComponent();

            _ember = new Ember();
            _ember.Process_ISAChan_Error_Event += ember_Process_ISAChan_Error_Event;
            _ember.CloseISAChannels();
            _ember.Probe_IP_Address = "172.19.14.121";
            _ember.OpenISAChannels();

            _telnet_connection = new TelnetConnection(_ember.Probe_IP_Address, 4900);
            _cmd_prefix = TCLI.Get_Custom_Command_Prefix(_telnet_connection);

            this.CLIValue_Event += Form_PowerMeter_CLIValue_Event;
        }

        void Form_PowerMeter_CLIValue_Event(object sender, string text)
        {
            setCLIText(text);
        }

        void setCLIText(string text)
        {
            if (_tokenSrc.IsCancellationRequested)
                return;

            if (this.labelPowerCS.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setCLIText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                //this.labelPowerCS.Text = string.Format("{0:F8}", cv.Voltage);
                this.labelPowerCS.Text = text;
                //this.labelPowerCS.Update();
            }
        }

        void buttonStartCS_Click(object sender, EventArgs e)
        {
        }

        void readCLIValues(CancellationToken ct)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;

                TCLI.Current_Voltage cv = get_uut_data();

                if (CLIValue_Event != null)
                {
                    string txt = string.Format("{0:F8}", cv.Voltage);
                    CLIValue_Event(this, txt);
                }
            }
        }


        TCLI.Current_Voltage get_uut_data()
        {
            return TCLI.Parse_Pload_Registers(_telnet_connection, _cmd_prefix, 240, 10);
        }

        private void Form_PowerMeter_Load(object sender, EventArgs e)
        {
            var token = _tokenSrc.Token;
            _task = new Task(() => readCLIValues(token), token);
            _task.Start();
        }

        void ember_Process_ISAChan_Error_Event(object sender, DataReceivedEventArgs e)
        {
            _isachan_error += e.Data + "\n";
        }

        private void timerISAChan_Tick(object sender, EventArgs e)
        {
        }

        private void Form_PowerMeter_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                _tokenSrc.Cancel();
                //_task.Wait(_tokenSrc.Token);
            }
            catch (Exception)
            {
            }

            while (!_task.IsCompleted)
                Thread.Sleep(1000);

            if (_telnet_connection != null)
                _telnet_connection.Close();

            if(_ember != null)
                _ember.CloseISAChannels();
        }

    }
}
