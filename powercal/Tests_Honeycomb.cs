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

        RelayControler _relay_ctrl;
        MultiMeter _meter;

        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public RelayControler RelayController { get { return _relay_ctrl; } set { _relay_ctrl = value; } }
        public MultiMeter MultiMeter { get { return _meter; } set { _meter = value; } }

        public Tests_Honeycomb(RelayControler relay_controller, MultiMeter multi_meter)
        {
            _relay_ctrl = relay_controller;
            _meter = multi_meter;

            if (_meter == null)
            {
                throw new Exception("Meter not specified");
            }

            if (_relay_ctrl == null)
            {
                throw new Exception("Relay controller not specified");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="telnet_connection"></param>
        /// <param name="relay"></param>
        /// <param name="value"></param>
        public static void Set_Relay_State(TelnetConnection telnet_connection, uint relay, bool value)
        {
            if (value)
            {
                TCLI.Wait_For_Prompt(telnet_connection);

                if (value)
                {
                    telnet_connection.WriteLine("write 1 6 0 1 0x10 {01}");
                }

                TCLI.Wait_For_Prompt(telnet_connection);

            }
            else
            {
                telnet_connection.WriteLine("write 1 6 0 1 0x10 {00}");
            }
        }


        public void Verify_Continuity(bool isShort, CancellationToken cancel)
        {
            double max_short = 0.1;
            double max_open = 1.0;

            _relay_ctrl.WriteLine(Relay_Lines.TestRelays_VacVdc, true);

            fire_status("Verify Continuity");
            _meter.Init();

            try
            {
                _meter.SetToRemote();
                _meter.ClearError();
                _meter.SetupForContinuity();

                string cont_str = _meter.Measure();
                double r = Convert.ToDouble(cont_str);

                string msg = string.Format("Continuity measurement was {0:F8} ohms", r);
                fire_status(msg);

                if (isShort && r > max_short)
                {
                    msg += string.Format(" . Max set to {0:F8}", max_short);
                    TraceLogger.Log(msg);
                    throw new Exception(msg);

                }

                if(!isShort && r < max_open)
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

        void fire_status(string msg)
        {
            if (Status_Event != null)
            {
                Status_Event(this, msg);
            }
        }


    }
}
