using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.IO.Ports;

namespace powercal
{
    public partial class FormSerialTest : Form
    {
        SerialPort _serialPort = new SerialPort();
        delegate void SetTextCallback();

        public FormSerialTest()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                this.comboBoxPorts.Items.Add(port);
            }
            if (ports.Length > 0)
            {
                initDlgPortSettings(ports[0]);
            }
        }

        private void openComPort()
        {
            try
            {
                updatePortSettings();
                _serialPort.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //SerialPort port = (SerialPort)sender;
            SetText();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if(!this.textBoxSend.AutoCompleteCustomSource.Contains(this.textBoxSend.Text))
                this.textBoxSend.AutoCompleteCustomSource.Add(this.textBoxSend.Text);

            if (_serialPort != null && !_serialPort.IsOpen)
            {
                openComPort();
            }

            if (_serialPort != null && _serialPort.IsOpen)
            {
                string txt = textBoxSend.Text;
                try
                {
                    if (this.checkBoxHex.Checked)
                    {
                        string[] hexValuesSplit = null;
                        if (txt.Contains(" "))
                        {
                            hexValuesSplit = txt.Split(' ');
                        }
                        else
                        {
                            hexValuesSplit = new string[txt.Length / 2];
                            int b = 0;
                            for (int c = 0; c < txt.Length; c += 2)
                            {
                                string bstr = string.Format("{0}{1}", txt[c], txt[c + 1]);
                                hexValuesSplit[b++] = bstr;
                            }
                        }

                        int count = hexValuesSplit.Length;
                        byte[] buffer = new byte[count];
                        int i = 0;
                        foreach (String hex in hexValuesSplit)
                        {
                            buffer[i++] = Convert.ToByte(hex, 16);
                        }
                        _serialPort.Write(buffer, 0, count);
                    }
                    else
                    {
                        int n = 0;
                        while (!_serialPort.DsrHolding)
                        {
                            Thread.Sleep(250);
                            n++;
                            if (n > 100)
                                break;
                        }

                        _serialPort.WriteLine(txt);
                        Thread.Sleep(250);

                        n = 0;
                        while (_serialPort.BytesToWrite > 0)
                        {
                            Thread.Sleep(100);
                            n++;
                            if (n > 100)
                                break;
                        }

                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void SetText()
        {
            if (this.textBoxReceive.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                try
                {
                    this.Invoke(d);
                }
                catch { }
            }
            else
            {
                if (this.checkBoxHex.Checked)
                {
                    int count = _serialPort.BytesToRead;
                    byte[] buffer = new byte[count];
                    int n = _serialPort.Read(buffer, 0, count);
                    foreach (byte b in buffer)
                    {
                        this.textBoxReceive.AppendText(string.Format("0x{0} ", b.ToString("X2")));
                    }

                }
                else
                {
                    string txt = _serialPort.ReadExisting();
                    this.textBoxReceive.AppendText(txt);
                }

            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBoxReceive.Clear();
        }

        private void refreshComboBoxPorts()
        {
            this.comboBoxPorts.Items.Clear();
            this.comboBoxPorts.Text = "";
            // Get a list of serial port names. 
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                this.comboBoxPorts.Items.Add(port);
            }
            if (ports.Length > 0)
            {
                this.comboBoxPorts.Text = ports[0];
            }
        }

        private void initDlgPortSettings(string portName)
        {
            this.textBoxBaud.Text = _serialPort.BaudRate.ToString();
            this.comboBoxParity.Items.AddRange(Enum.GetNames(typeof(Parity)));
            this.comboBoxParity.Text = Enum.GetName(typeof(Parity), _serialPort.Parity);
            this.textBoxdataBits.Text = _serialPort.DataBits.ToString();
            this.comboBoxStopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));
            this.comboBoxStopBits.Text = Enum.GetName(typeof(StopBits), _serialPort.StopBits);

            this.comboBoxPorts.Text = portName; // Do this last so the update port settings function has all the data it needs
        }

        private void updatePortSettings()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _serialPort.PortName = this.comboBoxPorts.Text;
            _serialPort.BaudRate = Convert.ToInt32(this.textBoxBaud.Text);
            _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), this.comboBoxParity.Text);
            _serialPort.DataBits = Convert.ToInt32(this.textBoxdataBits.Text);
            _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), this.comboBoxStopBits.Text);
            _serialPort.DtrEnable = true;  // This needs to be set for the 34401A metter to get data from it.  We may be able to use it for doing sycronous comunications.
            _serialPort.DataReceived += _serialPort_DataReceived;

        }

        private void buttonRefreshPorts_Click(object sender, EventArgs e)
        {
            refreshComboBoxPorts();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatePortSettings();
        }

        private void FormSerialTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            
        }


    }
}
