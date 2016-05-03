using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

using MinimalisticTelnet;

namespace PowerCalibration
{
    class Tests_Honeycomb
    {

        RelayControler _jig_relay_ctrl;
        MultiMeter _meter;
        TelnetConnection _telnet_conn;

        const uint _test_x4a_vacdc_relay_linenum = 4;
        const uint _test_laser_relay_linenum = 5;
        const uint _test_joinhoney_button_linenum = 6;

        public bool _init_testx4a_relay = true;  // Indicates to set relay that controls meter to VAC_VDC relay or to Test X4A port
        public bool _init_meter = true;  // Indicates whether the meter needs to be initialize

        public bool _enable_resistor_test = false;


        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public delegate void RunStatusHandler(object sender, string status_txt);
        public event RunStatusHandler Run_Status_Event;

        public RelayControler JigRelayController { get { return _jig_relay_ctrl; } set { _jig_relay_ctrl = value; } }
        public MultiMeter MultiMeter { get { return _meter; } set { _meter = value; } }

        public enum Relay { Continuity = 0, Capacitor, Resistor };

        int _sensor_id = Properties.Settings.Default.HoneycombSensorID;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jig_relay_controller"></param>
        /// <param name="multi_meter"></param>
        /// <param name="telnet_connection"></param>
        public Tests_Honeycomb(RelayControler jig_relay_controller, MultiMeter multi_meter, TelnetConnection telnet_connection)
        {
            _jig_relay_ctrl = jig_relay_controller;
            _meter = multi_meter;
            _telnet_conn = telnet_connection;

            if (_meter == null)
            {
                _meter = new MultiMeter(Properties.Settings.Default.Meter_COM_Port_Name);
            }

            if (_jig_relay_ctrl == null)
            {
                _jig_relay_ctrl = new RelayControler(RelayControler.Device_Types.FT232H);
            }

            if (_telnet_conn == null || !_telnet_conn.IsConnected)
            {
                _telnet_conn = new TelnetConnection(Properties.Settings.Default.Ember_Interface_IP_Address, 4900);
            }
        }

        /// <summary>
        /// Runs all tests
        /// </summary>
        public void Run_Tests(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested) return;

            _init_meter = true;
            init_meter();
            _init_meter = false;

            _init_testx4a_relay = true;
            init_relay_testx4a();
            _init_testx4a_relay = false;
            string msg = "";

            try
            {
                /******************************************************/
                if (cancel.IsCancellationRequested) return;
                msg = "Pairing with Sensor Test";
                fire_run_status(msg);
                clearSensorId();
                int id = pairWithSensor();
                if (id != 0 && id != _sensor_id)
                {
                    if (cancel.IsCancellationRequested) return;
                    msg = string.Format("Pair with incorrect sensor 0x{0:X}.  Expected sensor id = 0x{1:X}. Retrying...",
                            id, _sensor_id);
                    fire_run_status(msg);

                    clearSensorId();
                    _jig_relay_ctrl.WriteLine(Relay_Lines.Power, false);
                    Thread.Sleep(1000);
                    _jig_relay_ctrl.WriteLine(Relay_Lines.Power, true);
                    Thread.Sleep(1000);

                    id = pairWithSensor();
                    if (id != _sensor_id)
                    {
                        if(id != 0)
                            clearSensorId();
                        msg = string.Format("Pair with incorrect sensor 0x{0:X}. Expected sensor id = 0x{1:X}",
                            id, _sensor_id);
                        throw new Exception(msg);
                    }
                }
                clearSensorId();
                /******************************************************/

                /******************************************************/
                if (cancel.IsCancellationRequested) return;
                msg = string.Format("LASER Test");
                fire_run_status(msg);
                VerifyLaser();

                if (cancel.IsCancellationRequested) return;
                msg = "Door Relay Continuity Test";
                fire_run_status(msg);
                Verify_DoorRelay_Continuity();

                if (cancel.IsCancellationRequested) return;
                msg = "Capacitive Relay Test";
                fire_run_status(msg);
                Verify_Capacitance();

                if (cancel.IsCancellationRequested) return;
                msg = "Resistance Relay Test";
                fire_run_status(msg);
                Verify_Resitance();
                /******************************************************/

            }
            catch (Exception ex)
            {
                msg = string.Format("{0}", ex.Message);
                fire_status(msg);
                throw;
            }
            finally
            {
                _init_meter = true;
                close_meter();

                _init_testx4a_relay = true;
                _jig_relay_ctrl.WriteLine(_test_x4a_vacdc_relay_linenum, false);
            }

        }

        /// <summary>
        /// Use to setup the meter if _init_meter is set
        /// </summary>
        void init_meter()
        {
            if (_init_meter)
            {
                _meter.Init();
                _meter.SetToRemote();
                _meter.ClearError();
            }
        }

        /// <summary>
        /// Use to close the meter when no longer needed if _init_meter is set
        /// </summary>
        void close_meter()
        {
            if (_init_meter)
            {
                _meter.CloseSerialPort();
            }

        }

        void init_relay_testx4a()
        {
            if (_init_testx4a_relay)
            {
                _jig_relay_ctrl.WriteLine(_test_x4a_vacdc_relay_linenum, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="telnet_connection"></param>
        /// <param name="relay_num">The relay</param>
        /// <param name="value">0 to opens the relay, 1 closes the relay</param>
        public void Set_UUT_Relay_State(Relay relay_num, bool close_open)
        {
            string cmd = "cu finaltest ";
            int num = (int)relay_num;

            if (close_open)
            {
                cmd += string.Format(" closerelay {0}", num);
            }
            else
            {
                cmd += string.Format(" openrelay {0}", num);
            }

            TCLI.Wait_For_Prompt(_telnet_conn);
            _telnet_conn.WriteLine(cmd);
            fire_status(cmd);
        }


        public void Verify_DoorRelay_Continuity()
        {
            double max_short = 0.01;  // We usually get 0.00030000
            double max_open = 9.9; // usually 9.9999

            init_relay_testx4a();
            init_meter();
            string msg = "";

            try
            {
                _meter.SetupForContinuity();

                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Continuity, true);
                Thread.Sleep(500);

                string cont_str = _meter.Measure();

                double r = 0.0;
                try
                {
                    r = Convert.ToDouble(cont_str);
                }
                catch
                {
                    msg = string.Format("Error reading continuity from meter.  Meter returned \"{0}\"", cont_str);
                    fire_status(msg);
                    throw;
                }

                msg = string.Format("Continuity measurement with relay closed was {0:F8} ohms", r);
                fire_status(msg);

                if (r > max_short)
                {
                    msg += string.Format(" . Max set to {0:F8}", max_short);
                    TraceLogger.Log(msg);
                    throw new Exception(msg);

                }

                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Continuity, false);
                Thread.Sleep(500);

                cont_str = _meter.Measure();
                r = Convert.ToDouble(cont_str);

                msg = string.Format("Continuity measurement with relay opened was {0:F8} ohms", r);
                fire_status(msg);

                if (r < max_open)
                {
                    msg += string.Format(" . Max set to {0:F8}", max_open);
                    TraceLogger.Log(msg);
                    throw new Exception(msg);
                }
            }
            finally
            {
                close_meter();
            }

        }

        /// <summary>
        /// Verifies the Capacitor relay
        /// </summary>
        public void Verify_Capacitance()
        {
            init_relay_testx4a();
            init_meter();

            try
            {
                int multiplier = 1000;
                _meter.SetupForCapacitance(5 * multiplier);

                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Capacitor, true);
                Thread.Sleep(500);

                string cont_str = _meter.Measure();
                double c = Convert.ToDouble(cont_str) / multiplier;
                string msg = string.Format("Capacitance measurement relay close was {0:F8} uF", c);
                fire_status(msg);


                double exp_val = 1.0;
                double exp_max = exp_val + exp_val * 0.20;
                double exp_min = exp_val - exp_val * 0.20;

                if (c > exp_max || c < exp_min)
                {
                    msg = string.Format("Capacitance measurement with relay closed was {0:F8} uF, limit high {0:F8} uF, limit low {0:F8} uF", c, exp_max, exp_min);
                    throw new Exception(msg);
                }

                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Capacitor, false);
                Thread.Sleep(500);

                cont_str = _meter.Measure();
                c = Convert.ToDouble(cont_str) / multiplier;
                msg = string.Format("Capacitance measurement relay open was {0:F8} uF", c);
                fire_status(msg);

                exp_max = 0.1;
                if (c > 0.1)
                {
                    msg = string.Format("Capacitance measurement with relay opened was {0:F8} uF, limit high {0:F8} uF", c, exp_max);
                    throw new Exception(msg);
                }

            }
            finally
            {
                close_meter();
                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Capacitor, false);
            }

        }

        /// <summary>
        /// Verifies the resistor
        /// Note that we need a release version of the firmware for this to work due to the fact
        /// that the line that controls the relay it's also used for debug
        /// </summary>
        public void Verify_Resitance()
        {
            init_relay_testx4a();
            init_meter();

            try
            {
                _meter.SetupForResistance("0.5");

                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Resistor, true);
                Thread.Sleep(500);

                string r_str = _meter.Measure();
                double r = Convert.ToDouble(r_str) * 1000;
                string msg = string.Format("Resistance measurement relay closed was {0:F8} Ohms", r);
                fire_status(msg);
                double exp_val = 200.0;
                double exp_max = exp_val + exp_val * 0.20;
                double exp_min = exp_val - exp_val * 0.20;

                if (r > exp_max || r < exp_min)
                {
                    msg = string.Format("Resistance measurement with relay closed was {0:F8} Ohms, limit high {0:F8} Ohms, limit low {0:F8} Ohms", r, exp_max, exp_min);
                    throw new Exception(msg);
                }


                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Resistor, false);
                Thread.Sleep(500);

                r_str = _meter.Measure();
                r = Convert.ToDouble(r_str) * 1000;
                msg = string.Format("Resistance measurement relay open was {0:F8} Ohms", r);
                fire_status(msg);

                exp_min = 999;
                if (r < exp_min)
                {
                    msg = string.Format("Resistance measurement with relay opened was {0:F8} Ohms, limit high {0:F8} Ohms", r, exp_max);
                    throw new Exception(msg);
                }

            }
            finally
            {
                close_meter();
                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Capacitor, false);
            }
        }

        /// <summary>
        /// Verifies the LASER circuit
        /// </summary>
        public void VerifyLaser()
        {
            string msg = string.Format("Apply LASER signal");
            fire_status(msg);
            _jig_relay_ctrl.WriteLine(_test_laser_relay_linenum, true);
            Thread.Sleep(500);

            TCLI.Wait_For_Prompt(_telnet_conn);
            msg = string.Format("Detect Laser is available");
            fire_status(msg);
            TCLI.Wait_For_String(_telnet_conn, "cu isLaserConnected", "Laser is available");

            msg = string.Format("Remove LASER signal");
            fire_status(msg);
            _jig_relay_ctrl.WriteLine(_test_laser_relay_linenum, false);
            Thread.Sleep(500);

            msg = string.Format("Detect Laser is NOT available");
            fire_status(msg);
            TCLI.Wait_For_String(_telnet_conn, "cu isLaserConnected", "Laser is NOT available");
        }


        int getSensorId()
        {
            Match m = TCLI.Wait_For_Match(_telnet_conn, "cu si4355 readSensorId", "Current si4355 Sensor ID: ([0-9,A-F]+)");
            string idstr = m.Groups[1].Value;
            int id = Convert.ToInt32(idstr, 16);
            return id;
        }

        int clearSensorId()
        {
            // Clear the sensor
            string msg = string.Format("Clear Sensor id");
            fire_status(msg);

            string data = "";
            //data = TCLI.Wait_For_String(_telnet_conn, "cu si4355 clearSensorId", "si4355 Sensor ID cleared");
            TCLI.WriteLine(_telnet_conn, "cu si4355 clearSensorId");

            // Get the id after we cleaned it.  It now should be zero
            int id = getSensorId();
            if (id == 0)
            {
                return id;
            }

            msg = string.Format(
                "Unable to clear sensor id after command \"cu si4355 clearSensorId\".  Data was: {0}",
                data);
            throw new Exception(msg);

        }


        /// <summary>
        /// Clears the any current id
        /// Turns jig relay on to press pairing switch on sensor
        /// Waits for pairing
        /// </summary>
        /// <returns>Sensor id</returns>
        int pairWithSensor()
        {
            int id = 0;
            string msg = "";
            TCLI.Wait_For_Prompt(_telnet_conn);
            try
            {
                // Note that the id is part of this
                string data = "";

                for (int i = 0; i < 5; i++)
                {
                    msg = string.Format("Toggle sensor join switch");
                    fire_status(msg);

                    _jig_relay_ctrl.WriteLine(_test_joinhoney_button_linenum, true);
                    Thread.Sleep(2000);
                    _jig_relay_ctrl.WriteLine(_test_joinhoney_button_linenum, false);

                    data += TCLI.Read(_telnet_conn);

                    // Try to find a match
                    Match m = Regex.Match(data, "New sensor paired, ([0-9,A-F]+)");
                    if (m.Success)
                    {
                        id = Convert.ToInt32(m.Groups[1].Value, 16);
                        msg = string.Format("Paired with Sensor id {0} (0x{0:X})", id);
                        fire_status(msg);
                        break;
                    }
                    else
                    {
                        // Just in case, query for the id
                        id = getSensorId();
                        if (id != 0)
                        {
                            msg = string.Format("Paired with Sensor id {0} (0x{0:X})", id);
                            fire_status(msg);
                            break;
                        }
                    }
                }

                TraceLogger.Log(data);
            }
            finally
            {
                _jig_relay_ctrl.WriteLine(_test_joinhoney_button_linenum, false);
            }

            return id;
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

    }
}
