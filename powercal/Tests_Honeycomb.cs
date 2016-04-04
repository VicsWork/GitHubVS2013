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

        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public delegate void RunStatusHandler(object sender, string status_txt);
        public event RunStatusHandler Run_Status_Event;

        public RelayControler JigRelayController { get { return _jig_relay_ctrl; } set { _jig_relay_ctrl = value; } }
        public MultiMeter MultiMeter { get { return _meter; } set { _meter = value; } }

        public enum Relay { Continuity = 0, Capacitor, Resistor};

        /// <summary>
        /// Runs all tests
        /// </summary>
        public void Run_Tests(CancellationToken cancel)
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

            _jig_relay_ctrl.WriteLine(Relay_Lines.TestRelays_VacVdc, true);
            _meter.Init();
            try
            {
                _meter.SetToRemote();
                _meter.ClearError();
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
                _meter.CloseSerialPort();
            }

        }

        /// <summary>
        /// Verifies the Capacitor relay
        /// </summary>
        public void Verify_Capacitance()
        {

            _jig_relay_ctrl.WriteLine(Relay_Lines.TestRelays_VacVdc, true);

            _meter.Init();
            try
            {
                _meter.SetToRemote();
                _meter.ClearError();

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
                _meter.CloseSerialPort();
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
            _jig_relay_ctrl.WriteLine(5, true);
            Thread.Sleep(500);

            TCLI.Wait_For_Prompt(_telnet_conn);
            msg = string.Format("Detect Laser is available");
            fire_status(msg);
            TCLI.Wait_For_String(_telnet_conn, "cu isLaserConnected", "Laser is available");

            msg = string.Format("Remove LASER signal");
            fire_status(msg);
            _jig_relay_ctrl.WriteLine(5, false);
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
