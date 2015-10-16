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

namespace PowerCalibration
{
    public partial class Form_PowerMeter : Form
    {
        string _isachan_error;
        TelnetConnection _telnet_connection = null;
        string _cmd_prefix;
        Ember _ember;

        public delegate void CLIValueHandler(object sender, double voltage, double current);
        public event CLIValueHandler CLIValue_Event;
        delegate void SetTextCallback(double voltage, double current);

        CancellationTokenSource _tokenSrc = new CancellationTokenSource();
        Task _task;

        double _uut_voltage_reference = 240;
        double _uut_current_reference = 15;

        public Form_PowerMeter(string interface_type, string interface_address, double uut_voltage_reference, double uut_current_reference)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.IconPowerCalibration;

            _ember = new Ember();
            _ember.Process_ISAChan_Error_Event += ember_Process_ISAChan_Error_Event;

            _ember.Interface = (Ember.Interfaces)Enum.Parse(typeof(Ember.Interfaces), interface_type);
            _ember.Interface_Address = interface_address;

            if (_ember.Interface == Ember.Interfaces.USB)
            {
                _ember.CloseISAChannels();
                _ember.OpenISAChannels();
            }

            string telnet_address = "localhost";
            if (_ember.Interface == Ember.Interfaces.IP)
                telnet_address = _ember.Interface_Address;
            _telnet_connection = new TelnetConnection(telnet_address, 4900);

            _cmd_prefix = TCLI.Get_Custom_Command_Prefix(_telnet_connection);


            _uut_voltage_reference = uut_voltage_reference;
            _uut_current_reference = uut_current_reference;

            this.CLIValue_Event += Form_PowerMeter_CLIValue_Event;
        }

        void Form_PowerMeter_CLIValue_Event(object sender, double voltage, double current)
        {
            setCLIText(voltage, current);
        }

        void setCLIText(double voltage, double current)
        {
            if (_tokenSrc.IsCancellationRequested)
                return;

            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setCLIText);
                this.Invoke(d, new object[] { voltage, current });
            }
            else
            {
                labelPowerUUT.Text = string.Format("{0:F2}", voltage * current);
                labelVoltageUUT.Text = string.Format("{0:F4}", voltage);
                labelCurrentUUT.Text = string.Format("{0:F6}", current);
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

                try
                {
                    TCLI.Current_Voltage cv = get_uut_data(_uut_voltage_reference, _uut_current_reference);
                    if (CLIValue_Event != null)
                    {
                        CLIValue_Event(this, cv.Voltage, cv.Current);
                    }
                }
                catch (Exception e)
                {
                    if (!ct.IsCancellationRequested)
                        throw e;
                }
            }
        }


        TCLI.Current_Voltage get_uut_data(double voltage_reference, double current_reference)
        {
            return TCLI.Parse_Pload_Registers(_telnet_connection, _cmd_prefix, voltage_reference, current_reference);
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
            _tokenSrc.Cancel();

            if (_telnet_connection != null)
                _telnet_connection.Close();

            if (_ember != null)
                _ember.CloseISAChannels();
        }

    }
}
