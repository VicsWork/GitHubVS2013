using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Forms;

using MinimalisticTelnet;

namespace powercal
{

    class Calibrate
    {
        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public delegate void RelayStatusHandler(object sender, RelayControler relay_controller);
        public event RelayStatusHandler Relay_Event;

        public delegate void RunStatusHandler(object sender, string status_txt);
        public event RunStatusHandler Run_Status_Event;

        public delegate void CompletedStatusHandler(object sender);
        public event CompletedStatusHandler Completed_Status_Event;

        BoardTypes _board_type;
        RelayControler _relay_ctrl;
        TelnetConnection _telnet_connection;
        MultiMeter _meter;

        /// <summary>
        /// Voltage and current limits
        /// </summary>
        double _voltage_ac_low_limit = 0.0;
        double _voltage_ac_high_limit = 0.0;

        double _voltage_dc_low_limit = 0.0;
        double _voltage_dc_high_limit = 0.0;

        /// <summary>
        /// Voltage and current gain and reference address and values
        /// </summary>
        int _voltage_gain_adress = 0;
        int _current_gain_adress = 0;
        int _referece_adress = 0;
        int _ac_offset_adress = 0;

        int _voltage_ac_reference = 0;
        int _current_ac_reference = 0;


        /// <summary>
        /// The app folder where we save most logs, etc
        /// </summary>
        static string _app_data_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration");

        /// <summary>
        /// Path to where the Ember programing batch file is created
        /// </summary>
        string _ember_batchfile_path = Path.Combine(_app_data_dir, "patchit.bat");

        public Calibrate(BoardTypes boardtype, RelayControler relay_controller, TelnetConnection telnet_connection, MultiMeter meter)
        {
            _board_type = boardtype;
            set_board_calibration_values();

            _relay_ctrl = relay_controller;
            _telnet_connection = telnet_connection;
            _meter = meter;

        }

        /// <summary>
        /// Sets values used for calibration for selected board
        /// </summary>
        private void set_board_calibration_values()
        {
            double voltage_dc = 3.3;
            double voltage_dc_delta = 0.1;

            // Default values (USA)
            double voltage_ac_load = 120;
            double voltage_ac_delta = voltage_ac_load * 0.2;
            double current_ac_load = voltage_ac_load / 500; // 500 Ohms
            double current_ac_delta = current_ac_load * 0.3;

            _voltage_gain_adress = 0x08040980;
            _current_gain_adress = 0x08040984;
            _referece_adress = 0x08040988;
            _ac_offset_adress = 0x080409CC;

            _voltage_ac_reference = 240;
            _current_ac_reference = 15;
            //_cmd_prefix = "cs5490"; // UART interface

            switch (_board_type)
            {
                case BoardTypes.Humpback:
                    voltage_ac_load = 240;
                    current_ac_load = voltage_ac_load / 2000; // 2K Ohms
                    voltage_ac_delta = voltage_ac_load * 0.3;
                    current_ac_delta = current_ac_load * 0.4;

                    _voltage_gain_adress = 0x08080980;
                    _current_gain_adress = 0x08080984;
                    _referece_adress = 0x08080988;
                    _ac_offset_adress = 0x080809CC;

                    break;
                case BoardTypes.Zebrashark:
                    //_cmd_prefix = "cs5480";  // SPI interface
                    break;
                case BoardTypes.Milkshark:
                case BoardTypes.Mudshark:
                    _current_ac_reference = 10;
                    break;
            }

            _voltage_ac_high_limit = voltage_ac_load + voltage_ac_delta;
            _voltage_ac_low_limit = voltage_ac_load - voltage_ac_delta;

            _voltage_dc_high_limit = voltage_dc + voltage_dc_delta;
            _voltage_dc_low_limit = voltage_dc - voltage_dc_delta;

        }

        /// <summary>
        /// Calibrates using just the Ember
        /// Voltage and Current register values are gathered using custom commands
        /// </summary>
        public void Run()
        {
            string datain, msg;

            fire_status("===============================Start Calibration==============================");
            _relay_ctrl.Open();
            if (_relay_ctrl.Device_Type == RelayControler.Device_Types.Manual)
            {
                // Trun AC ON
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Ember, true);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Load, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, true);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Load, true);
                fire_relay_status();
                Thread.Sleep(2000);
            }
            else
            {
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Voltmeter, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Ember, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Load, false);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Power, true);
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Voltmeter, true);
                fire_relay_status();

                Thread.Sleep(2000);

                verify_voltage_dc();

                _relay_ctrl.WriteLine(powercal.Relay_Lines.Voltmeter, false);

                // Should be safe to connect Ember
                _relay_ctrl.WriteLine(powercal.Relay_Lines.Ember, true);
            }

/*
            // Open Ember isa channels
            //updateRunStatus("Start Ember isachan");
            traceLog("Start Ember isachan");
            openEmberISAChannels();

            // Create a new telnet connection
            //updateRunStatus("Start telnet");
            traceLog("Start telnet");
            _telnet_connection = new TelnetConnection("localhost", 4900);
*/
            datain = _telnet_connection.Read();
            TraceLogger.Log(datain);

            // Patch gain to 1
            fire_run_status("Patch Gain to 1");
            msg = patch(0x400000, 0x400000);
            TraceLogger.Log(msg);

            Thread.Sleep(1000);
            datain = _telnet_connection.Read();
            TraceLogger.Log(datain);

            // Force reset by cycle power
            //reset_handler(board_type);
            // Close UUT relay
            //set_board_relay(board_type, telnet_connection, true);
            //Thread.Sleep(1000);
            //datain = telnet_connection.Read();
            //traceLog(datain);

            _relay_ctrl.WriteLine(powercal.Relay_Lines.Load, true);
            Thread.Sleep(1000);
            //_relay_ctrl.WriteLine(powercal.Relay_Lines.Voltmeter, false);
            verify_voltage_ac();

            string cmd_prefix = TCLI.Get_Custom_Command_Prefix(_telnet_connection);
            TraceLogger.Log("cmd_prefix = " + cmd_prefix);

            // Get UUT currect/voltage values
            fire_run_status("Get UUT values");
            TCLI.Current_Voltage cv = TCLI.Parse_Pload_Registers(_telnet_connection, cmd_prefix, _voltage_ac_reference, _current_ac_reference);
            msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}", cv.Current, cv.Voltage, cv.Current * cv.Voltage);
            TraceLogger.Log(msg);
            cv = TCLI.Parse_Pload_Registers(_telnet_connection, cmd_prefix, _voltage_ac_reference, _current_ac_reference);

            msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}", cv.Current, cv.Voltage, cv.Current * cv.Voltage);
            fire_status(msg);

            if (cv.Voltage < _voltage_ac_low_limit || cv.Voltage > _voltage_ac_high_limit)
            {
                msg = string.Format("Cirrus voltage before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", _voltage_ac_low_limit, cv.Voltage, _voltage_ac_high_limit);
                throw new Exception(msg);
            }
            //if (cv.Current < _current_low_limit || cv.Current > _current_high_limit)
            if (cv.Current < 0.01 || cv.Current > 1)
            {
                //msg = string.Format("Cirrus current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", _current_low_limit, cv.Current, _current_high_limit);
                msg = string.Format("Cirrus current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", 0.01, cv.Current, 1);
                throw new Exception(msg);
            }

            /// The meter measurements
            fire_run_status("Meter measurements");
            double current_meter;
            if (_meter == null)
            {
                Form_EnterMeasurement dlg = new Form_EnterMeasurement();
                current_meter = dlg.GetMeasurement("Enter AC Current");
            }
            else
            {
                _meter.OpenComPort();
                _meter.SetupForIAC();
                string current_meter_str = _meter.Measure();
                current_meter_str = _meter.Measure();
                current_meter = Double.Parse(current_meter_str);
            }
            msg = string.Format("Meter I = {0:F8}", current_meter);
            TraceLogger.Log(msg);

            double voltage_meter;
            if (_meter == null)
            {
                Form_EnterMeasurement dlg = new Form_EnterMeasurement();
                voltage_meter = dlg.GetMeasurement("Enter AC Voltage");
            }
            else
            {
                _meter.SetupForVAC();
                string voltage_meter_str = _meter.Measure();
                voltage_meter_str = _meter.Measure();
                _meter.CloseSerialPort();
                voltage_meter = Double.Parse(voltage_meter_str);
            }
            msg = string.Format("Meter V = {0:F8}", voltage_meter);
            TraceLogger.Log(msg);

            msg = string.Format("Meter I = {0:F8}, V = {1:F8}, P = {2:F8}", current_meter, voltage_meter, current_meter * voltage_meter);
            fire_status(msg);

            if (voltage_meter < _voltage_ac_low_limit || voltage_meter > _voltage_ac_high_limit)
            {
                msg = string.Format("Meter voltage before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", _voltage_ac_low_limit, voltage_meter, _voltage_ac_high_limit);
                throw new Exception(msg);
            }
            //if (current_meter < _current_low_limit || current_meter > _current_high_limit)
            if (current_meter < 0.01 || current_meter > 1)
            {
                //msg = string.Format("Meter current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", _current_low_limit, current_meter, _current_high_limit);
                msg = string.Format("Meter current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", 0.01, current_meter, 1);
                throw new Exception(msg);
            }

            // Gain calculations
            fire_run_status("Gain calculations");
            double current_gain = current_meter / cv.Current;
            //double current_gain = current_meter / current_cs;
            int current_gain_int = (int)(current_gain * 0x400000);
            msg = string.Format("Current Gain = {0:F8} (0x{1:X})", current_gain, current_gain_int);
            fire_status(msg);

            double voltage_gain = voltage_meter / cv.Voltage;
            int voltage_gain_int = (int)(voltage_gain * 0x400000);
            msg = string.Format("Voltage Gain = {0:F8} (0x{1:X})", voltage_gain, voltage_gain_int);
            fire_status(msg);

            // Patch new gain
            fire_run_status("Patch Gain");
            msg = patch(voltage_gain_int, current_gain_int);
            TraceLogger.Log(msg);

            Thread.Sleep(3000);
            datain = _telnet_connection.Read();
            TraceLogger.Log(datain);

            _telnet_connection.WriteLine(string.Format("cu {0}_pinfo", cmd_prefix));
            Thread.Sleep(500);
            datain = _telnet_connection.Read();
            TraceLogger.Log(datain);

            // Get UUT currect/voltage values
            fire_run_status("Get UUT calibrated values");
            cv = TCLI.Parse_Pload_Registers(_telnet_connection, cmd_prefix, _voltage_ac_reference, _current_ac_reference);
            msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}", cv.Current, cv.Voltage, cv.Current * cv.Voltage);
            TraceLogger.Log(msg);
            cv = TCLI.Parse_Pload_Registers(_telnet_connection, cmd_prefix, _voltage_ac_reference, _current_ac_reference);
            msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}", cv.Current, cv.Voltage, cv.Current * cv.Voltage);
            fire_status(msg);

            // Disconnect Power
            _relay_ctrl.WriteLine(Relay_Lines.Power, false);
            _relay_ctrl.WriteLine(Relay_Lines.Ember, false);
            fire_relay_status();

            // Check calibration
            double delta = voltage_meter * 0.3;
            double high_limit = voltage_meter + delta;
            double low_limit = voltage_meter - delta;
            if (cv.Voltage < low_limit || cv.Voltage > high_limit)
            {
                msg = string.Format("Voltage after calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", low_limit, cv.Voltage, high_limit);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }
            delta = current_meter * 0.3;
            high_limit = current_meter + delta;
            low_limit = current_meter - delta;
            if (cv.Current < low_limit || cv.Current > high_limit)
            {
                msg = string.Format("Current after calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", low_limit, cv.Current, high_limit);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }

            fire_status("================================End Calibration===============================");

            fire_completed_status();
        }

        void verify_voltage_dc()
        {
            if (_meter == null)
                return;

            fire_run_status("Verify Voltage DC");
            _meter.OpenComPort();
            _meter.SetToRemote();
            _meter.ClearError();
            _meter.SetupForVDC();

            string meter_voltage_str = _meter.Measure();
            double meter_voltage_dc = Double.Parse(meter_voltage_str);
            _meter.SetupForVAC();
            meter_voltage_str = _meter.Measure();
            double meter_voltage_ac = Double.Parse(meter_voltage_str);

            string msg = string.Format("Meter DC Voltage at {0:F8} V.  AC {1:F8}", meter_voltage_dc, meter_voltage_ac);
            fire_status(msg);

            _meter.CloseSerialPort();

            if (meter_voltage_ac >= 1.0)
            {
                msg = string.Format("AC volatge detected at {0:F8}, DC Voltage {1:F8}", meter_voltage_ac, meter_voltage_dc);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }

            if (meter_voltage_dc < _voltage_dc_low_limit || meter_voltage_dc > _voltage_dc_high_limit)
            {
                msg = string.Format("Volatge DC is not within limits values: {0:F8} < {1:F8} < {2:F8}", _voltage_dc_low_limit, meter_voltage_dc, _voltage_dc_high_limit);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }
        }

        private void verify_voltage_ac()
        {
            if (_meter == null)
                return;

            fire_run_status("Verify Voltage AC");
            _meter.OpenComPort();
            _meter.SetToRemote();
            _meter.ClearError();
            _meter.SetupForVAC();

            string meter_voltage_str = _meter.Measure();
            meter_voltage_str = _meter.Measure();
            double meter_voltage = Double.Parse(meter_voltage_str);
            string msg = string.Format("Meter AC Voltage at {0:F8} V", meter_voltage);
            fire_status(msg);

            _meter.CloseSerialPort();

            if (meter_voltage < _voltage_ac_low_limit || meter_voltage > _voltage_ac_high_limit)
            {
                msg = string.Format("Volatge AC is not within limits values: {0:F8} < {1:F8} < {2:F8}", _voltage_ac_low_limit, meter_voltage, _voltage_ac_high_limit);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }

        }

        /// <summary>
        /// Patches the flash with calibration tokens
        /// </summary>
        /// <param name="board_type"></param>
        /// <param name="voltage_gain"></param>
        /// <param name="current_gain"></param>
        /// <returns></returns>
        string patch(int voltage_gain, int current_gain)
        {
            Ember ember = new Ember();
            ember.EmberBinPath = Properties.Settings.Default.Ember_BinPath;
            ember.BatchFilePath = _ember_batchfile_path;
            ember.VoltageRefereceValue = _voltage_ac_reference;
            ember.CurrentRefereceValue = _current_ac_reference;
            ember.VoltageAdress = _voltage_gain_adress;
            ember.CurrentAdress = _current_gain_adress;
            ember.RefereceAdress = _referece_adress;
            ember.ACOffsetAdress = _ac_offset_adress;
            ember.CreateCalibrationPatchBath(voltage_gain, current_gain);

            bool patchit_fail = false;
            string exception_msg = "";
            string coding_output = "";
            // Retry patch loop if fail
            while (true)
            {
                cleanupEmberTempPatchFile();

                patchit_fail = false;
                exception_msg = "";
                coding_output = "";
                try
                {
                    string output = ember.RunCalibrationPatchBatch();
                    if (output.Contains("ERROR:"))
                    {
                        patchit_fail = true;
                        exception_msg = "Patching error detected:\r\n";
                        exception_msg += output;
                    }
                    coding_output = output;
                }
                catch (Exception e)
                {
                    patchit_fail = true;
                    exception_msg = "Patching exception detected:\r\n";
                    exception_msg += e.Message;
                }

                if (patchit_fail)
                {
                    // Trun power off for safety
                    _relay_ctrl.WriteLine(Relay_Lines.Power, false);

                    string retry_err_msg = exception_msg;
                    int max_len = 1000;
                    if (retry_err_msg.Length > max_len)
                        retry_err_msg = retry_err_msg.Substring(0, max_len) + "...";
                    DialogResult dlg_rc = MessageBox.Show(retry_err_msg, "Patching fail", MessageBoxButtons.RetryCancel);
                    if (dlg_rc == System.Windows.Forms.DialogResult.Cancel)
                        break;

                    // Turn power back on
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    Thread.Sleep(1000);

                }
                else
                {
                    break;
                }
            }

            if (patchit_fail)
            {
                throw new Exception(exception_msg);
            }

            return coding_output;
        }

        /// <summary>
        /// Removes any Ember temp files and reports it in output status
        /// </summary>
        void cleanupEmberTempPatchFile()
        {
            string[] ember_temp_files = Ember.CleanupTempPatchFile();
            foreach (string file in ember_temp_files)
            {
                fire_status(string.Format("Ember temp file found and removed {0}", file));
            }
        }

        void fire_status(string msg)
        {
            if (Status_Event != null)
            {
                Status_Event(this, msg);
            }
        }

        void fire_run_status(string msg)
        {
            if (Run_Status_Event != null)
            {
                Run_Status_Event(this, msg);
            }
        }


        void fire_relay_status()
        {
            if (Relay_Event != null)
            {
                Relay_Event(this, _relay_ctrl);
            }
        }

        void fire_completed_status()
        {
            if (Completed_Status_Event != null)
            {
                Completed_Status_Event(this);
            }
        }

    }
}
