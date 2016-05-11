using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PowerCalibration
{
    class Tests
    {
        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public RelayControler RelayController { get { return _relay_ctrl; } set { _relay_ctrl = value; } }
        public MultiMeter MultiMeter { get { return _meter; } set { _meter = value; } }

        RelayControler _relay_ctrl;
        MultiMeter _meter;

        public void Verify_Voltage(double voltage_dc_low_limit=0.0, double voltage_dc_high_limit=0.0)
        {
            if (_meter == null)
            {
                fire_status("Unable to verify voltage.  Meter not specified");
                return;
            }

            if (_relay_ctrl != null && _relay_ctrl.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Vac_Vdc, true);  // DC

            fire_status("Verify Voltage DC");
            _meter.Init();
            _meter.SetToRemote();
            _meter.ClearError();
            _meter.SetupForVDC();

            string meter_voltage_str = _meter.Measure();
            double meter_voltage_dc = Double.Parse(meter_voltage_str);
            _meter.SetupForVAC();
            meter_voltage_str = _meter.Measure();
            double meter_voltage_ac = Double.Parse(meter_voltage_str);

            string msg = string.Format("Meter DC Voltage at {0:F8} V.  AC {1:F8}", 
                meter_voltage_dc, meter_voltage_ac);
            fire_status(msg);

            _meter.CloseSerialPort();

            if (meter_voltage_ac >= 1.0)
            {
                msg = string.Format("AC voltage detected at {0:F8}, DC Voltage {1:F8}", 
                    meter_voltage_ac, meter_voltage_dc);
                TraceLogger.Log(msg);
                throw new Exception(msg);
            }

            if (meter_voltage_dc < voltage_dc_low_limit || meter_voltage_dc > voltage_dc_high_limit)
            {
                msg = string.Format("Voltage DC is not within limits values: {0:F8} < {1:F8} < {2:F8}", 
                    voltage_dc_low_limit, meter_voltage_dc, voltage_dc_high_limit);
                TraceLogger.Log(msg);
                throw new Exception(msg);
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
