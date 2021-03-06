﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Forms;

using MinimalisticTelnet;

namespace PowerCalibration
{
    class CalibrationResultsEventArgs : EventArgs
    {
        public int Voltage_gain;
        public int Current_gain;

        public string EUI;

        public DateTime Timestamp;
    }

    class Calibrate
    {
        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public delegate void RelayStatusHandler(object sender, RelayControler relay_controller);
        public event RelayStatusHandler Relay_Event;

        public delegate void RunStatusHandler(object sender, string status_txt);
        public event RunStatusHandler Run_Status_Event;

        public delegate void CalibrationResultsHandler(object sender, CalibrationResultsEventArgs e);
        public event CalibrationResultsHandler CalibrationResults_Event;

        BoardTypes _board_type;
        RelayControler _relay_ctrl;
        TelnetConnection _telnet_connection;
        MultiMeter _meter;
        Ember _ember;

        /// <summary>
        /// Voltage and current limits
        /// </summary>
        double _load_voltage_ac_low_limit = 0.0;
        double _load_voltage_ac_high_limit = 0.0;

        double _load_current_ac_low_limit = 0.0;
        double _load_current_ac_high_limit = 0.0;

        double _uut_voltage_ac_low_limit = 0.0;
        double _uut_voltage_ac_high_limit = 0.0;

        double _uut_current_ac_low_limit = 0.0;
        double _uut_current_ac_high_limit = 0.0;

        double _voltage_dc_low_limit = 0.0;
        double _voltage_dc_high_limit = 0.0;

        public double Voltage_DC_Low_Limit { get { return _voltage_dc_low_limit; } }
        public double Voltage_DC_High_Limit { get { return _voltage_dc_high_limit; } }

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
        /// Path to where the Ember programming batch file is created
        /// </summary>
        string _ember_batchfile_patch_path = Path.Combine(_app_data_dir, "patchit.bat");

        // Where we save the calibration tokens
        string _tokens_backup_folder = _app_data_dir;
        public string Tokens_Backup_Folder { get { return _tokens_backup_folder; } set { _tokens_backup_folder = value; } }

        public BoardTypes BoardType
        {
            get { return _board_type; }
            set
            {
                _board_type = value;
                set_board_calibration_values();
            }
        }

        public double Voltage_Referencer { get { return _voltage_ac_reference; } }
        public double Current_Referencer { get { return _current_ac_reference; } }

        public RelayControler RelayController { get { return _relay_ctrl; } set { _relay_ctrl = value; } }
        public TelnetConnection TelnetConnection { get { return _telnet_connection; } set { _telnet_connection = value; } }
        public MultiMeter MultiMeter { get { return _meter; } set { _meter = value; } }
        public Ember Ember { get { return _ember; } set { _ember = value; } }

        public Calibrate() { }

        /// <summary>
        /// Sets values used for calibration for selected board
        /// </summary>
        private void set_board_calibration_values()
        {
            double voltage_dc = 3.3;
            double voltage_dc_delta = 0.1;

            // Default values (USA)
            double voltage_ac_load = Properties.Settings.Default.CalibrationLoadVoltageValue;
            double voltage_ac_delta = voltage_ac_load * (Properties.Settings.Default.CalibrationLoadVoltageTolarance / 100);

            double current_ac_load = voltage_ac_load / Properties.Settings.Default.CalibrationLoadResistorValue;
            double current_ac_delta = current_ac_load * (Properties.Settings.Default.CalibrationLoadResistorTolarance / 100);

            _voltage_gain_adress = 0x08040980;
            _current_gain_adress = 0x08040984;
            _referece_adress = 0x08040988;
            _ac_offset_adress = 0x080409CC;

            _voltage_ac_reference = 240;
            _current_ac_reference = 15;  // R = 0.002
            // The current reference is calculated by dividing 30x10-3/R.  
            // Where R is the current sensor value
            // For R=0.002 => 30*10e-3/2*10e-3 => 30/2 = 15

            //_cmd_prefix = "cs5490"; // UART interface

            switch (_board_type)
            {
                case BoardTypes.Humpback:
                case BoardTypes.Mahi:
                case BoardTypes.Halibut:
                    _voltage_gain_adress = 0x08080980;
                    _current_gain_adress = 0x08080984;
                    _referece_adress = 0x08080988;
                    _ac_offset_adress = 0x080809CC;
                    break;

                case BoardTypes.Honeycomb:
                    _voltage_gain_adress = 0x080409CC;
                    _current_gain_adress = 0x080409D0;
                    _referece_adress = 0x080409D4;
                    _ac_offset_adress = 0x080409D8;

                    // R = 0.003 => 30/3
                    _current_ac_reference = 10;
                    break;

                case BoardTypes.Zebrashark:
                    break;

                case BoardTypes.Milkshark:
                    // R8 = 0.01 => 30/10
                    _current_ac_reference = 3;
                    break;
                case BoardTypes.Mudshark:
                    // R = 0.003 => 30/3
                    _current_ac_reference = 10;
                    break;

            }

            _load_voltage_ac_high_limit = voltage_ac_load + voltage_ac_delta;
            _load_voltage_ac_low_limit = voltage_ac_load - voltage_ac_delta;

            _load_current_ac_high_limit = current_ac_load + current_ac_delta;
            _load_current_ac_low_limit = current_ac_load - current_ac_delta;

            _uut_voltage_ac_high_limit = voltage_ac_load * 3;
            _uut_voltage_ac_low_limit = voltage_ac_load / 3;

            _uut_current_ac_high_limit = current_ac_load * 4;
            _uut_current_ac_low_limit = current_ac_load / 3;

            _voltage_dc_high_limit = voltage_dc + voltage_dc_delta;
            _voltage_dc_low_limit = voltage_dc - voltage_dc_delta;

        }

        void reset_workaround()
        {
            // Note there is a problem with Hornshark/Mudshark not resetting after a patch
            string data = "";
            int try_count = 0;
            while (true)
            {
                _telnet_connection.WriteLine("cu");
                Thread.Sleep(500);
                data += _telnet_connection.Read();
                if (data.Contains("pload"))
                    break;
                else
                {
                    TraceLogger.Log("Hard Reset UUT");
                    _relay_ctrl.WriteLine(Relay_Lines.Ember, true);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, false);
                    Thread.Sleep(1000);
                    _relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    Thread.Sleep(3000);
                    _telnet_connection.Read();
                }
                try_count++;
                if (try_count > 3)
                    break;
            }

        }

        /// <summary>
        /// Closes the board relay using custom command
        /// </summary>
        public void Set_board_relay(bool value)
        {
            TCLI.Wait_For_Prompt(_telnet_connection);

            switch (_board_type)
            {

                // These boards have relays
                case BoardTypes.Hornshark:
                case BoardTypes.Humpback:
                case BoardTypes.Zebrashark:
                case BoardTypes.Hooktooth:  // only 1.6.3 and greater code can use load_on/off
                    if (value)
                    {
                        TraceLogger.Log("Set relay On");
                        _telnet_connection.WriteLine("write 1 6 0 1 0x10 {01}");
                    }
                    else
                    {
                        TraceLogger.Log("Set relay Off");
                        _telnet_connection.WriteLine("write 1 6 0 1 0x10 {00}");
                    }
                break;

                case BoardTypes.Milkshark:  // only 1.6.3 code
                case BoardTypes.Halibut:
                case BoardTypes.Mahi:
                    if (value)
                    {
                        TraceLogger.Log("Set load on");
                        _telnet_connection.WriteLine("cu load_on");
                    }
                    else
                    {
                        TraceLogger.Log("Set load off");
                        _telnet_connection.WriteLine("cu load_off");
                    }
                break;
            }

            TCLI.Wait_For_Prompt(_telnet_connection);

        }

        /// <summary>
        /// Calibrates using just the Ember
        /// Voltage and Current register values are gathered using custom commands
        /// </summary>
        public void Run(CancellationToken cancel)
        {
            string datain, msg;

            datain = _telnet_connection.Read();
            TraceLogger.Log(datain);

            // Patch gain to 1
            fire_run_status("Patch Gain to 1");
            msg = patch(0x400000, 0x400000);
            TraceLogger.Log(msg);

            // Note there is a problem with Hornshark/Mudshark not resetting after a patch
            reset_workaround();

            Thread.Sleep(1000);
            datain = _telnet_connection.Read();
            TraceLogger.Log(datain);

            fire_run_status("Verify Voltage AC");
            load_on();

            string cmd_prefix = TCLI.Get_Custom_Command_Prefix(_telnet_connection);
            TraceLogger.Log("cmd_prefix = " + cmd_prefix);

            string eui = TCLI.Get_EUI(_telnet_connection);
            TraceLogger.Log("eui = " + eui);

            // Get UUT current/voltage values
            fire_run_status("Get UUT values");

            TCLI.Current_Voltage cv = new TCLI.Current_Voltage();
            double power1 = 0.0, power2 = 0.0;
            int max_try_count = 5;
            for (int i = 0; i < max_try_count; i++)
            {
                bool error_reading = false;
                try
                {
                    cv = TCLI.Parse_Pload_Registers(
                        _telnet_connection, cmd_prefix, _voltage_ac_reference, _current_ac_reference);
                }
                catch (Exception ex)
                {
                    if (i + 1 >= max_try_count)
                    {
                        throw;
                    }
                    error_reading = true;
                    fire_status(ex.Message + ".  Retrying...");
                }

                if (!error_reading)
                {
                    if(cv.Voltage == 0.0 && cv.Current == 0.0)
                    {
                        RelayController.WriteLine(Relay_Lines.Power, false);
                        Thread.Sleep(1000);
                        RelayController.WriteLine(Relay_Lines.Power, true);
                    }

                    power1 = cv.Voltage * cv.Current;
                    double delta1 = 100;
                    if (power1 != 0)
                        delta1 = Math.Abs((power2 - power1) * 100 / power1);

                    msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}, D = {3:F8}",
                        cv.Current, cv.Voltage, cv.Current * cv.Voltage, delta1);
                    fire_status(msg);
                    //TraceLogger.Log(msg);

                    if (delta1 != 100.0 && delta1 != 0.0 && delta1 < 1.0)
                        break;
                }
                power2 = power1;
                Thread.Sleep(250);
            }

            if (cv.Voltage < _uut_voltage_ac_low_limit || cv.Voltage > _uut_voltage_ac_high_limit)
            {
                msg = string.Format(
                    "Cirrus voltage before calibration not within allowed limit values: {0:F8} < {1:F8} < {2:F8}",
                    _uut_voltage_ac_low_limit, cv.Voltage, _uut_voltage_ac_high_limit);
                throw new Exception(msg);
            }
            if (cv.Current < _uut_current_ac_low_limit || cv.Current > _uut_current_ac_high_limit)
            {
                msg = string.Format(
                    "Cirrus current before calibration not within allowed limit values: {0:F8} < {1:F8} < {2:F8}",
                    _uut_current_ac_low_limit, cv.Current, _uut_current_ac_high_limit);
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
                _meter.Init();
                _meter.SetupForIAC();
                string current_meter_str = _meter.Measure();
                current_meter_str = _meter.Measure();

                try
                {
                    current_meter = Double.Parse(current_meter_str);
                }
                catch (Exception)
                {
                    throw;
                }

                // Note that input should be on white 0.5 A terminal
                // Make sure System->Language = COMP
                if (_meter.Model == MultiMeter.Models.GDM8341)
                    current_meter /= 1000;
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
            msg = string.Format("Meter I = {0:F8}, V = {1:F8}, P = {2:F8}",
                current_meter, voltage_meter, current_meter * voltage_meter);
            fire_status(msg);

            //if (voltage_meter < 100 || voltage_meter > 260)
            if (voltage_meter < _load_voltage_ac_low_limit || voltage_meter > _load_voltage_ac_high_limit)
            {
                msg = string.Format(
                    "Meter voltage before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}",
                    _load_voltage_ac_low_limit, voltage_meter, _load_voltage_ac_high_limit);
                throw new Exception(msg);
            }

            if (current_meter < _load_current_ac_low_limit || current_meter > _load_current_ac_high_limit)
            //if (current_meter < 0.01 || current_meter > 1)
            {
                msg = string.Format(
                    "Meter current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}",
                    _load_current_ac_low_limit, current_meter, _load_current_ac_high_limit);
                //msg = string.Format("Meter current before calibration not within limit values: {0:F8} < {1:F8} < {2:F8}", 0.01, current_meter, 1);
                throw new Exception(msg);
            }

            // Gain calculations
            fire_run_status("Gain calculations");
            double current_gain = current_meter / cv.Current;
            //double current_gain = current_meter / current_cs;
            int current_gain_int = (int)(current_gain * 0x400000);
            msg = string.Format("Current Gain = {0:F8} (0x{1:X})", current_gain, current_gain_int);
            fire_status(msg);
            if (current_gain < Properties.Settings.Default.Gain_Current_Min ||
                current_gain > Properties.Settings.Default.Gain_Current_Max)
            {
                msg = string.Format(
                    "Current gain outside valid range: {0:F8} < {1:F8} < {2:F8}",
                    Properties.Settings.Default.Gain_Current_Min, current_gain, Properties.Settings.Default.Gain_Current_Max);
                throw new Exception(msg);
            }

            double voltage_gain = voltage_meter / cv.Voltage;
            int voltage_gain_int = (int)(voltage_gain * 0x400000);
            msg = string.Format("Voltage Gain = {0:F8} (0x{1:X})", voltage_gain, voltage_gain_int);
            fire_status(msg);
            if (voltage_gain < Properties.Settings.Default.Gain_Voltage_Min ||
                voltage_gain > Properties.Settings.Default.Gain_Voltage_Max)
            {
                msg = string.Format(
                    "Voltage gain outside valid range: {0:F8} < {1:F8} < {2:F8}",
                    Properties.Settings.Default.Gain_Voltage_Min, voltage_gain, Properties.Settings.Default.Gain_Voltage_Max);
                throw new Exception(msg);
            }

            CalibrationResultsEventArgs args_results = new CalibrationResultsEventArgs();
            args_results.Timestamp = DateTime.Now;
            args_results.EUI = eui;
            args_results.Voltage_gain = voltage_gain_int;
            args_results.Current_gain = current_gain_int;
            fire_results_status(args_results);


            // Patch new gain
            fire_run_status("Patch Gain");
            msg = patch(voltage_gain_int, current_gain_int);
            TraceLogger.Log(msg);

            // Note there is a problem with Hornshark/Mudshark not resetting after a patch
            reset_workaround();

            // villa dimmer not coming on after patch
            fire_run_status("Verify Voltage AC");
            load_on();

            Thread.Sleep(3000);
            datain = _telnet_connection.Read();
            TraceLogger.Log(datain);

            _telnet_connection.WriteLine(string.Format("cu {0}_pinfo", cmd_prefix));
            Thread.Sleep(500);
            datain = _telnet_connection.Read();
            TraceLogger.Log(datain);

            // Get UUT current/voltage values
            fire_run_status("Get UUT calibrated values");
            power1 = 0.0; power2 = 0.0;
            cv = new TCLI.Current_Voltage();
            max_try_count = 5;
            for (int i = 0; i < max_try_count; i++)
            {
                bool error_reading = false;
                try
                {
                    cv = TCLI.Parse_Pload_Registers(
                        _telnet_connection, cmd_prefix, _voltage_ac_reference, _current_ac_reference);
                }
                catch (Exception ex)
                {
                    if (i + 1 >= max_try_count)
                    {
                        throw;
                    }
                    error_reading = true;
                    fire_status(ex.Message + ".  Retrying...");
                }

                if (!error_reading)
                {
                    power1 = cv.Voltage * cv.Current;
                    double delta1 = 100;
                    if (power1 != 0)
                        delta1 = Math.Abs((power2 - power1) * 100 / power1);

                    msg = string.Format("Cirrus I = {0:F8}, V = {1:F8}, P = {2:F8}, D = {3:F8}",
                        cv.Current, cv.Voltage, cv.Current * cv.Voltage, delta1);
                    fire_status(msg);

                    if (delta1 != 100.0 && delta1 != 0.0 && delta1 < 1.0)
                        break;
                }
                power2 = power1;
                Thread.Sleep(250);
            }

            // Check calibration
            double delta = voltage_meter * 0.3;
            double high_limit = voltage_meter + delta;
            double low_limit = voltage_meter - delta;
            if (cv.Voltage < low_limit || cv.Voltage > high_limit)
            {
                msg = string.Format(
                    "Voltage after calibration not within limit values: {0:F8} < {1:F8} < {2:F8}",
                    low_limit, cv.Voltage, high_limit);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }
            delta = current_meter * 0.3;
            high_limit = current_meter + delta;
            low_limit = current_meter - delta;
            if (cv.Current < low_limit || cv.Current > high_limit)
            {
                msg = string.Format(
                    "Current after calibration not within limit values: {0:F8} < {1:F8} < {2:F8}",
                    low_limit, cv.Current, high_limit);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }

            if(_ember != null)
            {
                try
                {
                    string filename = eui + ".hex";
                    string fileurl = Path.Combine(Tokens_Backup_Folder, filename);
                    fire_status("Saved tokens " + filename);

                    //_ember.SaveCalibrationTokens(_voltage_gain_adress, fileurl);

                    //Task sttask = new Task(() => _ember.SaveCalibrationTokens(_voltage_gain_adress, fileurl));
                    //sttask.ContinueWith(saveTokensError, TaskContinuationOptions.OnlyOnFaulted);
                    //sttask.Start();

                    Task t = null;
                    t = Task.Factory.StartNew(
                        () =>
                        {
                            _ember.SaveCalibrationTokens(_voltage_gain_adress, fileurl);
                            t.ContinueWith(saveTokensError, TaskContinuationOptions.OnlyOnFaulted);
                        });
                }
                catch (Exception ex)
                {
                    TraceLogger.Log(ex.Message + "\n\n" + ex.StackTrace);
                }
            }

            // Check releay can be turned off
            if (Properties.Settings.Default.Calibration_Check_Relay_Can_Turn_Off)
            {
                fire_status("Load Off Command Check");
                int try_count = 0;
                while (true)
                {
                    Set_board_relay(false);
                    cv = new TCLI.Current_Voltage();
                    if (cv.Current == 0)
                    {
                        fire_status("Current off detected after Load Off command");
                        break;
                    }
                    Thread.Sleep(250);
                    if(try_count++ > 5)
                    {
                        msg = string.Format(
                            "Current after Load Off command not zero: Ci={0:F2}, Cv={1:F2}", cv.Current, cv.Voltage);
                        TraceLogger.Log(msg);
                        throw new Exception(msg);
                    }
                }
            }
        }

        private void saveTokensError(Task obj)
        {
            try
            {
                if (obj.Exception.InnerException != null)
                    fire_status("Error saving tokens\r\n" + obj.Exception.InnerException.Message);
                else
                    fire_status("Error saving tokens");
            }
            catch { };
        }

        /// <summary>
        /// Truns on Load and measure Vac on the load
        /// </summary>
        void load_on()
        {
            int trycount = 0;
            while (true)
            {
                // Connect the load and verify ac
                _relay_ctrl.WriteLine(Relay_Lines.Load, true);

                // Close the UUT relay
                // Some jigs short-out the relay....
                Set_board_relay(true);

                Thread.Sleep(1000);

                try
                {
                    verify_voltage_ac();
                    break;
                }
                catch
                {
                    if(_ember != null)
                    {
                        _ember.PinReset();
                        Thread.Sleep(2000);
                    }
                    trycount++;
                    if (trycount > 3)
                        throw;
                }
            }
        }

        private void verify_voltage_ac()
        {
            if (_meter == null)
                return;

            if (_relay_ctrl?.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Vac_Vdc, false);  // AC

            _meter.Init();
            _meter.SetToRemote();
            _meter.ClearError();
            _meter.SetupForVAC();

            string meter_voltage_str = _meter.Measure();
            meter_voltage_str = _meter.Measure();
            double meter_voltage = Double.Parse(meter_voltage_str);
            string msg = string.Format("Meter AC Voltage at {0:F8} V", meter_voltage);
            fire_status(msg);

            _meter.CloseSerialPort();

            //if (meter_voltage < 100 || meter_voltage > 260)
            if (meter_voltage < _load_voltage_ac_low_limit || meter_voltage > _load_voltage_ac_high_limit)
            {
                msg = string.Format("Voltage AC is not within limits values: {0:F8} < {1:F8} < {2:F8}", _load_voltage_ac_low_limit, meter_voltage, _load_voltage_ac_high_limit);
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
            _ember.EmberBinPath = Properties.Settings.Default.Ember_BinPath;
            _ember.BatchFilePatchPath = _ember_batchfile_patch_path;
            _ember.VoltageRefereceValue = _voltage_ac_reference;
            _ember.CurrentRefereceValue = _current_ac_reference;
            _ember.VoltageAdress = _voltage_gain_adress;
            _ember.CurrentAdress = _current_gain_adress;
            _ember.RefereceAdress = _referece_adress;
            _ember.ACOffsetAdress = _ac_offset_adress;

            _ember.CreateCalibrationPatchBath(voltage_gain, current_gain);

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
                    string output = _ember.RunCalibrationPatchBatch();
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
                    Thread.Sleep(3000);

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

        void fire_results_status(CalibrationResultsEventArgs e)
        {
            if (CalibrationResults_Event != null)
            {
                CalibrationResults_Event(this, e);
            }
        }

    }
}
