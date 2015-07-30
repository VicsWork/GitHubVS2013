using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

namespace powercal
{
    class CSCommander
    {
        private string _portName;
        private SerialPort _serialPort = new SerialPort();
        private static TraceSource _traceSource = new TraceSource("PowerCalTraceSource");

        Queue<byte> _rx_byte_queue = new Queue<byte>();
        private int _wait_ms = 250;  //   time to wait before we read

        public CSCommander(string portName)
        {
            this._portName = portName;
        }

        public SerialPort openComPort()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            //_serialPort = new SerialPort(_portName, 600, Parity.None, 8, StopBits.One);
            _serialPort.PortName = _portName;
            _serialPort.BaudRate = 600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;

            _serialPort.Open();

            return _serialPort;
        }

        public SerialPort openComPortIfNotOpened()
        {
            if (this._serialPort == null || !this._serialPort.IsOpen)
            {
                return openComPort();
            }
            else
            {
                return _serialPort;
            }
        }

        public void ClearSerialBuffer()
        {
            this._rx_byte_queue.Clear();
            this._serialPort.DiscardInBuffer();
            this._serialPort.DiscardOutBuffer();
        }

        public byte[] Send_Receive_Bytes(byte[] bytesToSend)
        {
            _traceSource.TraceEvent(TraceEventType.Information, -1, "Send_Recive_Bytes");

            // Clean up any data from serial
            //ClearSerialBuffer();
            WaitForWriteDone();

            byte[] presend_data = new byte[0];
            int len = _serialPort.BytesToRead;
            if (len > 0)
            {
                presend_data = new byte[_serialPort.BytesToRead];
                _serialPort.Read(presend_data, 0, len);
                Debug.WriteLine("Send_Receive_Bytes: BytesToRead > 0 before send!!!");

            }

            _serialPort.Write(bytesToSend, 0, bytesToSend.Length);

            Thread.Sleep(_wait_ms);

            int n = 0;
            while (_serialPort.BytesToRead < 3)
            {
                Thread.Sleep(_wait_ms);
                n++;
                if (n > 5)
                {
                    throw new Exception("Could not comunicate with CS5490.  Please check Ember in reset");
                    //Debug.WriteLine("Send_Receive_Bytes did not get 3 bytes!!!");
                    //break;
                }
            }

            len = _serialPort.BytesToRead;
            byte[] rx_bytes = new byte[len];
            _serialPort.Read(rx_bytes, 0, len);
            return rx_bytes;

        }

        public void WaitForWriteDone()
        {
            int n = 0;
            while (_serialPort.BytesToWrite > 0)
            {
                Thread.Sleep(_wait_ms);
                n++;
                if (n > 5)
                {
                    Debug.WriteLine("WaitForWriteDone: BytesToWrite > 0 for a long time!!!");
                    break;
                }
            }
        }

        public void Send(byte[] bytesToSend)
        {
            _traceSource.TraceEvent(TraceEventType.Information, -1, "Send");
            _serialPort.Write(bytesToSend, 0, bytesToSend.Length);
        }

        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            byte[] rx = new byte[port.BytesToRead];
            int count = port.Read(rx, 0, port.BytesToRead);
            lock (_rx_byte_queue)
            {
                foreach (byte b in rx)
                {
                    _rx_byte_queue.Enqueue(b);
                }
            }
        }

        public void CloseSerialPort()
        {
            this._serialPort.Close();
        }
    }
}
