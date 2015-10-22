using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerCalibration
{
    class PreTest
    {
        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public RelayControler RelayController { get { return _relay_ctrl; } set { _relay_ctrl = value; } }
        public MultiMeter MultiMeter { get { return _meter; } set { _meter = value; } }
        public double Voltage_DC_Low_Limit { get { return _voltage_dc_low_limit; } set { _voltage_dc_low_limit = value; } }
        public double Voltage_DC_High_Limit { get { return _voltage_dc_high_limit; } set { _voltage_dc_high_limit = value; } }

        RelayControler _relay_ctrl;
        MultiMeter _meter;
        double _voltage_dc_low_limit = 0.0, _voltage_dc_high_limit = 0.0;

        public void Verify_Voltage()
        {
            if (_meter == null)
                return;

            if (_relay_ctrl != null && _relay_ctrl.Device_Type != RelayControler.Device_Types.Manual)
                _relay_ctrl.WriteLine(Relay_Lines.Voltmeter, true);  // DC

            fire_status("Verify Voltage DC");
            _meter.OpenComPort();
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

            if (meter_voltage_dc < _voltage_dc_low_limit || meter_voltage_dc > _voltage_dc_high_limit)
            {
                msg = string.Format("Voltage DC is not within limits values: {0:F8} < {1:F8} < {2:F8}", 
                    _voltage_dc_low_limit, meter_voltage_dc, _voltage_dc_high_limit);
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
