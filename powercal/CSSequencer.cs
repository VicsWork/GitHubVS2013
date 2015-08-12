using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;

namespace powercal
{
    /// <summary>
    /// Class used for calibration.  
    /// Sends specific commands (page selects, read/write register, etc) sequences to a CS54xx chip 
    /// and calculates measurement values for different board types (i.e. Humpback, Hooktooth, etc)
    /// 
    /// Note this class is just a proff of concept ported from work done by Edgard Lerma at Jabil
    /// 
    /// </summary>
    class CSSequencer
    {
        public enum BoardTypes { Humpback, Hooktooth, Milkshark };
        private CSCommander _cscommander;
        private int _currentFactor;
        private double _iref, _vref;
        public double IRef { get { return _iref; } }
        public double VRef { get { return _vref; } }

        private static TraceSource _source = new TraceSource("PowerCalTraceSource");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName">Physical serial (UART) port name connected to CS54xx chip</param>
        /// <param name="boardType">The board type (i.e. Humpback).  Different boards use different current factors, etc.</param>
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

        /// <summary>
        /// Opens the serial comunications
        /// </summary>
        /// <returns></returns>
        public SerialPort OpenSerialPort()
        {
            return _cscommander.openComPort();
        }

        /// <summary>
        /// Closes the serial port
        /// </summary>
        public void CloseSerialPort()
        {
            _cscommander.CloseSerialPort();
        }

        /// <summary>
        /// Sends a software reset instruction
        /// </summary>
        public void SoftReset()
        {
            byte[] tx_data = StrToBytes("C1"); //Soft reset
            _cscommander.Send(tx_data);
        }

        /// <summary>
        /// Enables the HPF
        /// </summary>
        public void EnableHiPassFilter()
        {
            // 90 Select Page 16
            // 40 Write to register 0 (Config2)
            // 08 = Sets IFLT[1:0] = 01 = High-pass filter (HPF) on current channel
            byte[] tx_data = StrToBytes("90 40 08 20 10"); //Enable High pass filter
            _cscommander.Send(tx_data);
        }

        /// <summary>
        /// Starts continues conversion
        /// </summary>
        public void StartContinuousConvertion()
        {
            byte[] tx_data = StrToBytes("D5"); //Start Continuous Convertion
            _cscommander.Send(tx_data);
        }

        /// <summary>
        /// Sets AC Offset to 0
        /// </summary>
        public void SetACOffsetToZero()
        {
            byte[] tx_data = StrToBytes("90 65 00 00 00"); //Page 16 select, set ACOffset to 0
            _cscommander.Send(tx_data);
        }

        /// <summary>
        /// Reads the Irms register
        /// </summary>
        /// <returns>Irms value</returns>
        public double GetIRMS()
        {
            //EnableHiPassFilter();
            //StartContinuousConvertion();

            byte[] tx_data = StrToBytes("90 06"); //Page 16 select, reads IRMS register
            byte[] rx_data = _cscommander.Send_Receive_Bytes(tx_data);

            double value = RegHex_ToDouble(rx_data);
            value *= _currentFactor;

            return value;
        }

        /// <summary>
        /// Reads the Vrms register
        /// </summary>
        /// <returns>Vrms value</returns>
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

        /// <summary>
        /// Sets AC offset to 0 and then reads I AC offset
        /// </summary>
        /// <returns>I AC offset</returns>
        public double GetIOffset()
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 79 D0 07 00"); //Set Tsettle to 2000ms
            _cscommander.Send(tx_data);

            //EnableHiPassFilter();
            SetACOffsetToZero();
            //StartContinuousConvertion();

            tx_data = StrToBytes("90 25");
            rx_data = _cscommander.Send_Receive_Bytes(tx_data); //Page 16 select, read IACOffset register

            // 0 <= value < 1.0
            double value = RegHex_ToDouble(rx_data);
            value *= _currentFactor;

            return value;
        }

        /// <summary>
        /// Sets AC offset to 0 and then reads Irms
        /// </summary>
        /// <returns>I AC offset</returns>
        public double IRMSNoLoad()
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 79 D0 07 00"); //Set Tsettle to 2000ms
            _cscommander.Send(tx_data);

            //EnableHiPassFilter();
            SetACOffsetToZero();
            //StartContinuousConvertion();

            tx_data = StrToBytes("90 06"); //Page 16 select, read IRMS register
            rx_data = _cscommander.Send_Receive_Bytes(tx_data);

            double value = RegHex_ToDouble(rx_data);
            value *= _currentFactor;

            return value;

        }

        /// <summary>
        /// Modifies I gain
        /// </summary>
        /// <param name="iRMSGain"></param>
        public void IGainCal(int iRMSGain)
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 79 D0 07 00"); //Set Tsettle to 2000ms
            _cscommander.Send(tx_data);

            tx_data = new byte[5];
            int i = 0;
            tx_data[i++] = 0x90;
            tx_data[i++] = 0x61;
            tx_data[i++] = (byte)(iRMSGain);
            tx_data[i++] = (byte)(iRMSGain >> 8);
            tx_data[i++] = (byte)(iRMSGain >> 16);

            string msg = "IGainCal command: ";
            foreach (byte b in tx_data)
                msg += string.Format("0x{0:X} ", b);
            Debug.WriteLine(msg);

            _cscommander.Send(tx_data); //Modify IGain
        }

        /// <summary>
        /// Modifies V gain
        /// </summary>
        /// <param name="vRMSGain"></param>
        public void VGainCal(int vRMSGain)
        {
            byte[] rx_data = new byte[0];

            byte[] tx_data = StrToBytes("90 79 D0 07 00"); //Set Tsettle to 2000ms
            _cscommander.Send(tx_data);

            tx_data = new byte[5];
            int i = 0;
            tx_data[i++] = 0x90;
            tx_data[i++] = 0x63;
            tx_data[i++] = (byte)(vRMSGain);
            tx_data[i++] = (byte)(vRMSGain >> 8);
            tx_data[i++] = (byte)(vRMSGain >> 16);

            string msg = "VGainCal command: ";
            foreach (byte b in tx_data)
                msg += string.Format("0x{0:X} ", b);
            Debug.WriteLine(msg);

            _cscommander.Send(tx_data); //Modify VGain
        }

        /// <summary>
        /// Converts a 24bit hex (3 bytes) CS register value to a double
        /// </summary>
        /// <example>
        /// byte[] rx_data = new byte[3];
        /// rx_data[2] = 0x5c;
        /// rx_data[1] = 0x28;
        /// rx_data[0] = 0xf6;
        /// Should return midrange =~ 0.36
        /// </example>
        /// <param name="rx_data">data byte array byte[2] <=> MSB ... byte[0] <=> LSB</param>
        /// <returns>range 0 <= value < 1.0</returns>
        private double RegHex_ToDouble(byte[] rx_data)
        {
            // Maximum 1 =~ 0xFFFFFF
            // Max rms 0.6 =~ 0x999999
            // Half rms 0.36 =~ 0x5C28F6
            double reg_value = (double)(rx_data[2] << 16 | rx_data[1] << 8 | rx_data[0]);
            double value = reg_value/0x1000000; // 2^24
            return value;
        }

        /// <summary>
        /// Converts a string to byte array
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
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
