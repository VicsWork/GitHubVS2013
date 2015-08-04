using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;

namespace powercal
{
    class CSSequencer
    {
        public enum BoardTypes { Humpback, Hooktooth, Milkshark };
        private CSCommander _cscommander;
        private int _currentFactor;
        private double _iref, _vref;
        public double IRef { get { return _iref; } }
        public double VRef { get { return _vref; } } 

        private static TraceSource _source = new TraceSource("PowerCalTraceSource");

        public CSSequencer(string portName, BoardTypes boardType)
        {
            _cscommander = new CSCommander(portName);
            switch (boardType)
            {
                case BoardTypes.Humpback:
                case BoardTypes.Hooktooth:
                    _currentFactor = 100;
                    _iref = 0.8542;
                    _vref = 1.9840;
                    break;
                case BoardTypes.Milkshark:
                    _currentFactor = 25;
                    _iref = 0.9490;
                    _vref = 2.0219;
                    break;
            }
        }

        public SerialPort openSerialPort()
        {
            return _cscommander.openComPort();
        }

        public void CloseSerialPort()
        {
            _cscommander.CloseSerialPort();
        }

        public void SoftReset()
        {
            byte[] tx_data = StrToBytes("C1"); //Soft reset
            _cscommander.Send(tx_data);
        }

        public void EnableHiPassFilter()
        {
            // 90 Select Page 16
            // 40 Write to register 0 (Config2)
            // 08 = Sets IFLT[1:0] = 01 = High-pass filter (HPF) on current channel
            byte[] tx_data = StrToBytes("90 40 08 20 10"); //Enable High pass filter
            _cscommander.Send(tx_data);
        }

        public void StartContinuousConvertion()
        {
            byte[] tx_data = StrToBytes("D5"); //Start Continuous Convertion
            _cscommander.Send(tx_data);
        }

        public double GetIRMS()
        {
            EnableHiPassFilter();
            StartContinuousConvertion();

            byte[] tx_data = StrToBytes("90 06"); //Page 16 select, reads IRMS register
            byte[] rx_data = _cscommander.Send_Receive_Bytes(tx_data);

            double value = RegHex_ToDouble(rx_data);
            value *= _currentFactor;

            return value;
        }

        public double GetVRMS()
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 07"); //Page 16 select, reads VRMS register
            rx_data = _cscommander.Send_Receive_Bytes(tx_data);

            double value = RegHex_ToDouble(rx_data);
            value *= 1691 * 2.4; //1691 = sum of all input resistors, 2.4 reference voltage
            value /= 10; //10 = CS5490 input gain

            return value;
        }

        public double GetIOffset()
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 79 D0 07 00"); //Set Tsettle to 2000ms
            _cscommander.Send(tx_data);

            EnableHiPassFilter();

            tx_data = StrToBytes("90 65 00 00 00"); //Page 16 select, set ACOffset to 0
            _cscommander.Send(tx_data);

            StartContinuousConvertion();

            tx_data = StrToBytes("90 25");
            rx_data = _cscommander.Send_Receive_Bytes(tx_data); //Page 16 select, read IACOffset register

            // 0 <= value < 1.0
            double value = RegHex_ToDouble(rx_data);
            value *= _currentFactor;

            return value;
        }

        public double IRMSNoLoad()
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 79 D0 07 00"); //Set Tsettle to 2000ms
            _cscommander.Send(tx_data);

            EnableHiPassFilter();

            tx_data = StrToBytes("90 65 00 00 00"); //Page 16 select, set ACOffset to 0
            _cscommander.Send(tx_data);

            StartContinuousConvertion();

            tx_data = StrToBytes("90 06"); //Page 16 select, read IRMS register
            rx_data = _cscommander.Send_Receive_Bytes(tx_data);

            double value = RegHex_ToDouble(rx_data);
            value *= _currentFactor;

            return value;

        }

        public void IGainCal(int iRMSGain)
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 79 D0 07 00"); //Set Tsettle to 2000ms
            _cscommander.Send(tx_data);

            tx_data = new byte[5];
            int i = 0;
            tx_data[i++]= 0x90;
            tx_data[i++]= 0x61;
            tx_data[i++] = (byte)(iRMSGain & 0xFF);
            tx_data[i++] = (byte)(iRMSGain >> 8);
            tx_data[i++] = (byte)(iRMSGain >> 16);

            string msg = "IGainCal command: ";
            foreach (byte b in tx_data)
                msg += string.Format("0x{0:X} ", b);
            Debug.WriteLine(msg);

            _cscommander.Send(tx_data); //Modify IGain
        }

        public void VGainCal(int vRMSGain)
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 79 D0 07 00"); //Set Tsettle to 2000ms
            _cscommander.Send(tx_data);

            tx_data = new byte[5];
            int i = 0;
            tx_data[i++] = 0x90;
            tx_data[i++] = 0x63;
            tx_data[i++] = (byte)(vRMSGain & 0xFF);
            tx_data[i++] = (byte)(vRMSGain >> 8);
            tx_data[i++] = (byte)(vRMSGain >> 16);

            string msg = "VGainCal command: ";
            foreach (byte b in tx_data)
                msg += string.Format("0x{0:X} ", b);
            Debug.WriteLine(msg);

            _cscommander.Send(tx_data); //Modify VGain
        }

        private double RegHex_ToDouble(byte[] rx_data)
        {
            double reg_value = (double)(rx_data[2] << 16 | rx_data[1] << 8 | rx_data[0]);
            double value = (double)(reg_value / 0x1000000); // 2^24
            return value;
        }

        public byte[] StrToBytes(string txt)
        {
            string[] hexValuesSplit;
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

            return buffer;
        }
    }
}
