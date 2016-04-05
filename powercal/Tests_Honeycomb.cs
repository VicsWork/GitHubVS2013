using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

        public bool _init_testx4a_relay = true;  // Indicates to set relay that controls meter to VAC_VDC relay or to Test X4A port
        public bool _init_meter = true;  // Indicates whether the meter needs to be initialize

        public bool _enable_resistor_test = false;


        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public delegate void RunStatusHandler(object sender, string status_txt);
        public event RunStatusHandler Run_Status_Event;

        public RelayControler JigRelayController { get { return _jig_relay_ctrl; } set { _jig_relay_ctrl = value; } }
        public MultiMeter MultiMeter { get { return _meter; } set { _meter = value; } }

        public enum Relay { Continuity = 0, Capacitor, Resistor};


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

            if(_telnet_conn == null)
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

            try
            {
                if (cancel.IsCancellationRequested) return;
                string msg = string.Format("Verify LASER");
                fire_run_status(msg);
                VerifyLaser();

                if (cancel.IsCancellationRequested) return;
                msg = "Verify Door Relay Continuity";
                fire_run_status(msg);
                Verify_DoorRelay_Continuity();

                if (cancel.IsCancellationRequested) return;
                msg = "Verify Capacitive Relay";
                fire_run_status(msg);
                Verify_Capacitance();
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

            try
            {
                _meter.SetupForContinuity();

                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Continuity, true);
                Thread.Sleep(500);

                string cont_str = _meter.Measure();
                double r = Convert.ToDouble(cont_str);

                string msg = string.Format("Continuity measurement with relay closed was {0:F8} ohms", r);
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
                _meter.SetupForCapacitance(5*multiplier);

                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Capacitor, true);
                Thread.Sleep(500);

                string cont_str = _meter.Measure();
                double c = Convert.ToDouble(cont_str)/multiplier;
                string msg = string.Format("Capacitance measurement relay close was {0:F8} uF", c);
                fire_status(msg);


                double exp_val = 1.0;
                double exp_max = exp_val + exp_val * 0.20;
                double exp_min = exp_val - exp_val * 0.20;

                if (c > exp_max || c < exp_min)
                {
                    msg = string.Format("Capacitance measurement with relay closed was {0:F8} uF, limit high {0:F8} uF, limit low {0:F8} uF", c, exp_max, exp_min);
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
                }

            }
            finally
            {
                close_meter();
                Set_UUT_Relay_State(Tests_Honeycomb.Relay.Capacitor, false);
            }

        }

        public void Verify_Resitance() 
        {
            init_relay_testx4a();
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
