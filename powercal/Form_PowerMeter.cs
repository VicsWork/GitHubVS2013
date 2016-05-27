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
        TelnetConnection _telnet_connection;
        string _cmd_prefix;
        bool _static_values_read = false;

        public class CLIPowerArgsEventArgrs : EventArgs
        {
            public double Voltage = 0.0, Current = 0.0;

            public CLIPowerArgsEventArgrs(double voltage, double current)
            {
                Voltage = voltage;
                Current = current;
            }
        }

        public delegate void CLIValueHandler(object sender, CLIPowerArgsEventArgrs e);
        public event CLIValueHandler CLIValue_Event;

        public delegate void CLIErrorHandler(object sender, string text);
        public event CLIErrorHandler CLIError_Event;

        delegate void setCLIPowerTextCallback(CLIPowerArgsEventArgrs args);
        delegate void setCLIErrorTextCallback(string text);

        delegate void setErrorVisibilityCallback(bool visible);

        delegate void setCalibrationTokensTextCallback(TCLI.Tokens tokens);

        CancellationTokenSource _tokenSrc = new CancellationTokenSource();
        Task _task;

        double _uut_voltage_reference = 240;
        double _uut_current_reference = 15;

        public Form_PowerMeter(TelnetConnection telnet_connection)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.IconPowerCalibration;

            _telnet_connection = telnet_connection;

            this.CLIValue_Event += Form_PowerMeter_CLIValue_Event;
            this.CLIError_Event += Form_PowerMeter_CLIError_Event;

            setErrorVisibility(false);
        }

        void Form_PowerMeter_CLIError_Event(object sender, string text)
        {
            setCLIErrorText(text);
        }

        void setCLIErrorText(string text)
        {
            if (_tokenSrc.IsCancellationRequested)
                return;

            if (this.InvokeRequired)
            {
                setCLIErrorTextCallback d = new setCLIErrorTextCallback(setCLIErrorText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                setErrorVisibility(true);
                textBoxUUTError.Text = text;
            }

            setCLIPowerText( new CLIPowerArgsEventArgrs(0.0, 0.0));
        }

        void Form_PowerMeter_CLIValue_Event(object sender, CLIPowerArgsEventArgrs args)
        {
            setCLIPowerText(args);
        }

        void setCLIPowerText(CLIPowerArgsEventArgrs args)
        {
            if (_tokenSrc.IsCancellationRequested)
                return;

            if (this.InvokeRequired)
            {
                setCLIPowerTextCallback d = new setCLIPowerTextCallback(setCLIPowerText);
                this.Invoke(d, new object[] { args });
            }
            else
            {
                labelPowerUUT.Text = string.Format("{0:F2}", args.Voltage * args.Current);
                labelVoltageUUT.Text = string.Format("{0:F4}", args.Voltage);
                labelCurrentUUT.Text = string.Format("{0:F6}", args.Current);
            }
        }

        void setErrorVisibility(bool visible)
        {
            if (_tokenSrc.IsCancellationRequested)
                return;

            if (this.InvokeRequired)
            {
                setErrorVisibilityCallback d = new setErrorVisibilityCallback(setErrorVisibility);
                this.Invoke(d, new object[] { visible });
            }
            else
            {
                textBoxUUTError.Visible = visible;
            }
        }

        bool isUUTSaticValuesRead()
        {
            return _static_values_read;
        }

        private void setCalibrationTokensText(TCLI.Tokens calibration_tokens)
        {

            if (this.InvokeRequired)
            {
                setCalibrationTokensTextCallback d = new setCalibrationTokensTextCallback(setCalibrationTokensText);
                this.Invoke(d, new object[] { calibration_tokens });
            }
            else
            {
                this.labelVFactor.Text = string.Format("Voltage Factor: {0}", calibration_tokens.VoltageFactor);
                this.labelIFactor.Text = string.Format("Current Factor: {0}", calibration_tokens.CurrentFactor);
                this.labelVGain.Text = string.Format("VGain Token: 0x{0:X08}", calibration_tokens.VoltageGainToken);
                this.labelIGain.Text = string.Format("IGain Token: 0x{0:X08}", calibration_tokens.CurrentGainToken);

                string eui = TCLI.Get_EUI(_telnet_connection);
                this.labelEUI.Text = string.Format("EUI: {0}", eui);
            }
        }

        void readUUTStaticValues()
        {
            if (_cmd_prefix == null)
            {
                _cmd_prefix = TCLI.Get_Custom_Command_Prefix(_telnet_connection);
            }

            TCLI.Tokens calibration_tokens = TCLI.Parse_Pinfo_Tokens(_telnet_connection, _cmd_prefix);
            setCalibrationTokensText(calibration_tokens);
            _uut_voltage_reference = Convert.ToDouble(calibration_tokens.VoltageFactor);
            _uut_current_reference = Convert.ToDouble(calibration_tokens.CurrentFactor);

            _static_values_read = true;
        }

        void readCLIValues(CancellationToken ct)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;

                try
                {
                    if ( !isUUTSaticValuesRead() )
                    {
                        readUUTStaticValues();
                    }

                    TCLI.Current_Voltage cv = get_uut_data(_uut_voltage_reference, _uut_current_reference);
                    if (CLIValue_Event != null)
                    {
                        setErrorVisibility(false);
                        CLIValue_Event(this, 
                            new CLIPowerArgsEventArgrs(cv.Voltage, cv.Current));
                    }
                }
                catch (Exception ex)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        if (CLIError_Event != null)
                        {
                            CLIError_Event(this, ex.Message);
                        }
                        else
                        {
                            throw;
                        }
                    }
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

        }

    }
}
