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
        private CSCommander _cscommander;
        private double _full_scale_current, _full_scale_voltage;
        public double FullScaleCurrent { get { return _full_scale_current; } }
        public double FullScaleVoltage { get { return _full_scale_voltage; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName">Physical serial (UART) port name connected to CS54xx chip</param>
        public CSSequencer(string portName)
        {
            _cscommander = new CSCommander(portName);
            _full_scale_current = 15;
            _full_scale_voltage = 240;
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
        /// Init the Cs54xx for calibration
        /// </summary>
        public void Init()
        {
            // Read Page 0 Addr 0 (80 00) deault value = C0 20 00
            byte[] tx_data = StrToBytes("80 40 20 20 C0");   // I gain 50X
            _cscommander.Send(tx_data);

            // 90 Select Page 16
            // 40 Write to register 0 (Config2)
            // 0A = Sets IFLT[1:0] and VFLT[1:0] to 01 = High-pass filter (HPF) on current and voltage channels
            tx_data = StrToBytes("90 40 0A 02 10");  // AFC mode, IFLT 
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
            value = (value * FullScaleCurrent) / 0.6;

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
            value = (value * FullScaleVoltage) / 0.6;

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

            SetACOffsetToZero();

            tx_data = StrToBytes("90 25");
            rx_data = _cscommander.Send_Receive_Bytes(tx_data); //Page 16 select, read IACOffset register

            // 0 <= value < 1.0
            double value = RegHex_ToDouble(rx_data);
            value = (value * FullScaleCurrent) / 0.6;

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

            SetACOffsetToZero();

            tx_data = StrToBytes("90 06"); //Page 16 select, read IRMS register
            rx_data = _cscommander.Send_Receive_Bytes(tx_data);

            double value = RegHex_ToDouble(rx_data);
            value = (value * FullScaleCurrent) / 0.6;

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
            Trace.WriteLine(msg);

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
            Trace.WriteLine(msg);

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
