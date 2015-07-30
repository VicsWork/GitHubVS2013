using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace powercal
{
    class MultiMeter
    {
        public string meter_type = "34401A";

        public bool WaitForDsrHolding
        {
            get { return _waitForDsrHolding; }
            set { _waitForDsrHolding = value; }
        }

        private bool _waitForDsrHolding = true;
        private string _portName;
        private SerialPort _serialPort = new SerialPort();
        private string _value_txt = "";

        public MultiMeter(string portName)
        {
            this._portName = portName;

        }

        public SerialPort OpenComPort()
        {
            //if (_serialPort != null && _serialPort.IsOpen)
            //{
                _serialPort.Close();
            //}
            //_serialPort = new SerialPort(_portName, 600, Parity.None, 8, StopBits.One);
            _serialPort.PortName = _portName;
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.DtrEnable = true;

            _serialPort.DataReceived += _serialPort_DataReceived;
            _serialPort.Open();

            return _serialPort;
        }

        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_value_txt)
            {
                _value_txt += _serialPort.ReadExisting();
            }
        }

        string waitForData()
        {
            int n = 0;
            while (_value_txt == "")
            {
                Thread.Sleep(100);
                n++;
                if (n > 5)
                {
                    break;
                }
            }
            n = 0;
            while (_serialPort.BytesToRead > 0)
            {
                Thread.Sleep(250);
                n++;
                if (n > 10)
                {
                    break;
                }
            }

            return _value_txt;
        }

        void clearData()
        {
            lock (_value_txt)
                _value_txt = "";
        }

        public void writeLine(string cmd)
        {
            int n = 0;

            if (_waitForDsrHolding)
            {
                while (!_serialPort.DsrHolding)
                {
                    Thread.Sleep(250);
                    n++;
                    if (n > 20)
                        throw new Exception("Multimeter not responding to serial commands.  Make sure multi-meter is on and serial cable connected");
                }
            }

            _serialPort.WriteLine(cmd);
            Thread.Sleep(250);

            n = 0;
            while (_serialPort.BytesToWrite > 0)
            {
                Thread.Sleep(100);
                n++;
                if (n > 20)
                    throw new Exception("Multimeter write buffer not empty");
            }
        }

        public void ClearError()
        {
            writeLine("*CLS");
        }

        public void SetToRemote()
        {
            writeLine("SYST:REM");
        }

        public string IDN()
        {
            clearData();
            writeLine("*IDN?");
            string data = waitForData();
            return data;
        }

        public void SetupForVAC()
        {
            writeLine(":CONF:VOLT:AC 1000,0.01");
            writeLine(":TRIG:SOUR BUS");
        }

        public void SetupForIAC()
        {
            writeLine(":CONF:CURR:AC 1,0.000001");
            writeLine(":TRIG:SOUR BUS");
        }

        public string Measure()
        {
            clearData();
            writeLine(":INIT");
            writeLine("*TRG");
            writeLine(":FETC?");
            string data = waitForData();
            return data;
        }
        public void CloseSerialPort()
        {
            this._serialPort.Close();
        }
    }
}
