using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

using MinimalisticTelnet;

namespace powercal
{
    /// <summary>
    /// Class to handle telnet command line interfacing
    /// </summary>
    class TCLI
    {
        /// <summary>
        /// Simple structure used to return voltage/current value pair
        /// </summary>
        public struct Current_Voltage
        {
            public double Current;
            public double Voltage;

            public Current_Voltage(double i = 0.0, double v = 0.0)
            {
                Current = i;
                Voltage = v;
            }
        }

        /// <summary>
        /// Sends a pload command and returns the current and voltage values
        /// </summary>
        /// <param name="telnet_connection">Already opened Telnet connection to the Ember</param>
        /// <param name="board_type">What board are we using</param>
        /// <returns>Current/Voltage structure values</returns>
        public static Current_Voltage Parse_Pload_Registers(TelnetConnection telnet_connection, string cmd_prefix, double voltage_ac_reference, double current_ac_reference)
        {
            string rawCurrentPattern = "Raw IRMS: ([0-9,A-F]{8})";
            string rawVoltagePattern = "Raw VRMS: ([0-9,A-F]{8})";
            double current_cs = 0.0;
            double voltage_cs = 0.0;

            telnet_connection.WriteLine(string.Format("cu {0}_pload", cmd_prefix));
            Thread.Sleep(500);
            string datain = telnet_connection.Read();
            Trace.WriteLine(datain);
            string msg;

            if (datain != null && datain.Length > 0)
            {
                Match on_off_match = Regex.Match(datain, "Changing OnOff .*");
                if (on_off_match.Success)
                {
                    msg = on_off_match.Value;
                }

                Match match = Regex.Match(datain, rawCurrentPattern);
                if (match.Groups.Count != 2)
                {
                    msg = string.Format("Unable to parse pinfo for current.  Output was:{0}", datain);
                    throw new Exception(msg);
                }

                string current_hexstr = match.Groups[1].Value;
                int current_int = Convert.ToInt32(current_hexstr, 16);
                current_cs = RegHex_ToDouble(current_int);
                current_cs = current_cs * current_ac_reference / 0.6;

                voltage_cs = 0.0;
                match = Regex.Match(datain, rawVoltagePattern);
                if (match.Groups.Count != 2)
                {
                    msg = string.Format("Unable to parse pinfo for voltage.  Output was:{0}", datain);
                    throw new Exception(msg);
                }

                string voltage_hexstr = match.Groups[1].Value;
                int volatge_int = Convert.ToInt32(voltage_hexstr, 16);
                voltage_cs = RegHex_ToDouble(volatge_int);
                voltage_cs = voltage_cs * voltage_ac_reference / 0.6;

            }

            Current_Voltage current_voltage = new Current_Voltage(i: current_cs, v: voltage_cs);
            return current_voltage;
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
        public static double RegHex_ToDouble(int data)
        {
            // Maximum 1 =~ 0xFFFFFF
            // Max rms 0.6 =~ 0x999999
            // Half rms 0.36 =~ 0x5C28F6
            double value = ((double)data) / 0x1000000; // 2^24
            return value;
        }

        /// <summary>
        /// Converts a hex string (3 bytes) CS register vaue to a double
        /// </summary>
        /// <param name="hexstr"></param>
        /// <returns>range 0 <= value < 1.0</returns>
        /// <seealso cref="double RegHex_ToDouble(int data)"/>
        public static double RegHex_ToDouble(string hexstr)
        {
            int val_int = Convert.ToInt32(hexstr, 16);
            return RegHex_ToDouble(val_int); ;
        }

        /// <summary>
        /// Telnets to the Ember and prints custom commands
        /// Parses command list and tries to find the pload or pinfo comand prefix
        /// It is usually "cs5480_" in the case of SPDI or "cs5490_" in the case of UART comunications
        /// Exception is thrown if not pload command is found after typing "cu"
        /// </summary>
        /// <returns></returns>
        public static string Get_Custom_Command_Prefix(TelnetConnection telnet_connection)
        {
            string cmd_pre = null;

            int try_count = 0;
            string data = "";

            while (true)
            {
                telnet_connection.WriteLine("cu");
                data += telnet_connection.Read();
                if (data.Contains("pload"))
                    break;
                try_count++;
                if (try_count > 3)
                    break;
            }

            string msg = "";
            if (!data.Contains("pload"))
            {
                msg = string.Format("Unable to get custum command output list from Ember.  Output was: {0}", data);
                throw new Exception(msg);
            }

            string pattern = @"(cs[0-9]{4})_pload\r\n";
            Match match = Regex.Match(data, pattern);
            if (match.Groups.Count != 2)
            {
                msg = string.Format("Unable to parse custom command list for pload.  Output was:{0}", data);
                throw new Exception(msg);
            }

            cmd_pre = match.Groups[1].Value;
            return cmd_pre;
        }


    }
}
