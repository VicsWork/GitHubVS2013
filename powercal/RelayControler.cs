using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

using NationalInstruments.DAQmx;
using DIO;

namespace PowerCalibration
{

    /// <summary>
    /// Class to control the DIO lines connected to the different relays
    /// </summary>
    public class RelayControler
    {
        /// <summary>
        /// Supported device types
        /// </summary>
        public enum Device_Types { Manual, NI_USB6008, FT232H };

        /// <summary>
        /// Dic to store line numbers
        /// </summary>
        Dictionary<string, uint> _dic_lines = new Dictionary<string, uint>();
        public Dictionary<string, uint> Dictionary_Lines { get { return _dic_lines; } set { _dic_lines = value; } }

        /// <summary>
        /// Dic to store line state (true = ON, false = OFF)
        /// </summary>
        private Dictionary<string, bool> _dic_values = new Dictionary<string, bool>();

        Device_Types _dev_type;
        public Device_Types Device_Type { get { return _dev_type; } }

        bool _isOpened = false;
        public bool IsOpened { get { return _isOpened; } }

        /// <summary>
        /// Inits internal dictionary that holds line number and state info
        /// </summary>
        private void _initDicLines()
        {
            DicLines_ReadSettings();

            // Dic to store line state (true = ON, false = OFF)
            _dic_values.Clear();
            foreach (string key in _dic_lines.Keys)
            {
                _dic_values.Add(key, false);
            }
        }

        /// <summary>
        /// File used to save line settings
        /// </summary>
        private string _diclines_settings_file = "powercal.relaycontroller.diclines.xml";

        /// <summary>
        /// Save the dictionary lines settings
        /// </summary>
        public void DicLines_SaveSettings()
        {
            FileStream writer = new FileStream(_diclines_settings_file, FileMode.Create);
            DataContractSerializer ser = new DataContractSerializer(_dic_lines.GetType());
            ser.WriteObject(writer, _dic_lines);
            writer.Close();
        }

        /// <summary>
        /// Clears all dictionaries
        /// </summary>
        public void ClearDictionaries()
        {
            _dic_lines.Clear();
            _dic_values.Clear();
        }

        /// <summary>
        /// Read line settings from file
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, uint> DicLines_ReadSettings()
        {
            if (!File.Exists(_diclines_settings_file))
            {
                DicLines_SaveSettings();
                //string msg = string.Format("Dictionary setting file not found at {0}", _diclines_settings_file);
                //throw new Exception(msg);
            }

            FileStream reader = new FileStream(_diclines_settings_file, FileMode.Open);
            DataContractSerializer ser = new DataContractSerializer(_dic_lines.GetType());
            try
            {
                _dic_lines = (Dictionary<string, uint>)ser.ReadObject(reader);
            }
            catch (Exception)
            {
                reader.Close();
                DicLines_SaveSettings();
                reader = new FileStream(_diclines_settings_file, FileMode.Open);
                _dic_lines = (Dictionary<string, uint>)ser.ReadObject(reader);
            }
            reader.Close();

            return _dic_lines;
        }

        public void RecreateSettingsFile()
        {
            if (File.Exists(_diclines_settings_file))
            {
                File.Delete(_diclines_settings_file);
            }
            DicLines_SaveSettings();
        }

        /// <summary>
        /// Adds a line and its initial value to the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="line_num"></param>
        /// <param name="init_value"></param>
        public void DicLines_AddLine(string key, uint line_num, bool init_value = false)
        {
            if (_dic_lines.ContainsKey(key))
                throw new Exception(string.Format("Line Key \"{0} \" already present", key));
            if (_dic_values.ContainsKey(key))
                throw new Exception(string.Format("Value Key \"{0} \" already present", key));

            _dic_lines.Add(key, line_num);
            _dic_values.Add(key, init_value);
        }


        /// <summary>
        /// The NI DIO port info
        /// </summary>
        private string _ni_port_desc;

        /// <summary>
        /// FTDI Stuff
        /// </summary>
        DIO.FT232HDIO _ft232hdio;
        DIO.FT232HDIO.DIO_BUS _ftdi_bus = FT232HDIO.DIO_BUS.AC_BUS;
        public string FTDI_BUS { get { return _ftdi_bus.ToString(); } }

        int _ftdi_dev_index = -1;
        public int FTDI_DEVICE_ID { get { return _ftdi_dev_index; } }

        public string FTDI_DEVICE_YPE { get { return _ft232hdio.DeviceType.ToString(); } }

        /// <summary>
        /// Constructor
        /// </summary>
        public RelayControler(Device_Types devtype)
        {
            _dev_type = devtype;

            if (_dev_type == Device_Types.NI_USB6008)
            {
                initNI_USB6008();
            }
            else if (_dev_type == Device_Types.FT232H)
            {
                initFT232H();
            }

            _initDicLines();
        }

        /// <summary>
        /// Initializes the FT232H controller
        /// </summary>
        private void initFT232H()
        {
            _ft232hdio = new FT232HDIO();
            _ftdi_dev_index = _ft232hdio.GetFirstDevIndex();
            if (_ftdi_dev_index < 0)
            {
                throw new Exception("Unable to find an F232H device");
            }
        }

        /// <summary>
        /// Resets the device
        /// </summary>
        public void ResetDevice()
        {
            if (_dev_type == Device_Types.FT232H)
            {
                _ft232hdio.ResetDevice();
            }
        }

        /// <summary>
        /// Opens the device
        /// </summary>
        public void Open()
        {
            if (_dev_type == Device_Types.FT232H)
            {
                _ft232hdio.Open((uint)_ftdi_dev_index);
                // Open functions resets device
                // Re-set line state
                foreach (string key in _dic_lines.Keys)
                {
                    uint pin = _dic_lines[key];
                    bool value = _dic_values[key];
                    _ft232hdio.SetPin(_ftdi_bus, pin, value);
                }
            }
            _isOpened = true;
        }

        /// <summary>
        /// Closes the device
        /// </summary>
        public void Close()
        {
            if (_dev_type == Device_Types.FT232H)
            {
                _ft232hdio.Close();
            }
            _isOpened = false;
        }

        /// <summary>
        /// Only opens the device if it is closed
        /// </summary>
        public void OpenIfClosed()
        {
            if (!_isOpened)
                Open();
        }

        /// <summary>
        /// Inits the DIO to be the first port found
        /// </summary>
        private void initNI_USB6008()
        {
            if (_ni_port_desc == null)
            {
                string[] data = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External);
                if (data.Length > 0)
                {
                    _ni_port_desc = data[0];
                }
            }
        }

        /// <summary>
        /// Writes to all lines the specified value
        /// </summary>
        /// <param name="value"></param>
        public void WriteAll(bool value)
        {

            foreach (string key in _dic_lines.Keys)
            {
                WriteLine(key, value);
            }
        }

        /// <summary>
        /// Writes to the specified line name
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteLine(string key, bool value)
        {
            if (_dev_type == Device_Types.Manual)
            {
                _dic_values[key] = value;
            }
            else
            {
                uint linenum = _dic_lines[key];
                WriteLine(linenum, value);
            }
        }

        /// <summary>
        /// Writes to the specified line number
        /// </summary>
        /// <param name="linenum"></param>
        /// <param name="value"></param>
        public void WriteLine(uint linenum, bool value)
        {
            string linename = GetName(linenum);
            _dic_values[linename] = value;

            if (_dev_type == Device_Types.NI_USB6008)
            {
                using (Task digitalWriteTask = new Task())
                {
                    //  Create an Digital Output channel and name it.
                    string linestr = string.Format("{0}/line{1}", _ni_port_desc, linenum);
                    string name = string.Format("line{0}", linenum);
                    digitalWriteTask.DOChannels.CreateChannel(linestr, name, ChannelLineGrouping.OneChannelForEachLine);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSampleSingleLine(true, value);
                }
                return;
            }
            else if (_dev_type == Device_Types.FT232H)
            {
                _ft232hdio.SetPin(_ftdi_bus, linenum, value);
                return;
            }
        }

        /// <summary>
        /// Reads the specified line name
        /// </summary>
        /// <param name="linename"></param>
        /// <returns></returns>
        public bool ReadLine(string linename)
        {
            if (_dev_type == Device_Types.Manual)
            {
                return _dic_values[linename];
            }
            else
            {
                uint linenum = _dic_lines[linename];
                return ReadLine(linenum);
            }
        }


        /// <summary>
        /// Gets the specified line number's name
        /// </summary>
        /// <param name="linenum"></param>
        /// <returns></returns>
        public string GetName(uint linenum)
        {
            string name = null;
            foreach (string key in _dic_lines.Keys)
            {
                uint value = _dic_lines[key];
                if (value == linenum)
                {
                    name = key;
                    break;
                }
            }

            if (name == null)
            {
                name = string.Format("C{0}", linenum);
                _dic_lines.Add(name, linenum);
            }

            return name;

        }

        /// <summary>
        /// Reads the specified line number
        /// </summary>
        /// <param name="linenum"></param>
        /// <returns></returns>
        public bool ReadLine(uint linenum)
        {
            if (_dev_type == Device_Types.Manual)
            {
                string linename = GetName(linenum);
                return _dic_values[linename];
            }
            else if (_dev_type == Device_Types.NI_USB6008)
            {
                using (Task digitalReaderTask = new Task())
                {
                    //  Create an Digital Output channel and name it.
                    string linestr = string.Format("{0}/line{1}", _ni_port_desc, linenum);
                    string name = string.Format("line{0}", linenum);
                    digitalReaderTask.DOChannels.CreateChannel(linestr, name, ChannelLineGrouping.OneChannelForEachLine);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReaderTask.Stream);
                    return reader.ReadSingleSampleSingleLine();
                }
            }
            else if (_dev_type == Device_Types.FT232H)
            {
                return _ft232hdio.ReadPin(_ftdi_bus, linenum);
            }

            Trace.TraceWarning("Unknown device");
            return false;

        }

        /// <summary>
        /// Returns the status of all lines
        /// </summary>
        /// <returns></returns>
        public string[] ToStrArray()
        {

            string[] msg = new string[_dic_lines.Count];
            int i = 0;
            foreach (string key in _dic_lines.Keys)
            {
                //bool value = ReadLine(key);
                bool value = _dic_values[key];
                string on_off_str = "OFF";
                if (value)
                    on_off_str = "ON";
                msg[i++] = string.Format("{0} = {1}", key, on_off_str);
            }

            return msg;

        }

        /// <summary>
        /// Returns the status of all lines formatted to display
        /// </summary>
        /// <returns></returns>
        public string ToDlgText()
        {
            string[] msg_array = this.ToStrArray();
            string msg_dlg = "";
            foreach (string txt in msg_array)
            {
                msg_dlg += txt + "\r\n";
            }

            return msg_dlg;
        }

        /// <summary>
        /// Returns the status of all lines as text
        /// </summary>
        /// <returns></returns>
        public string ToStatusText()
        {
            string[] msg_array = this.ToStrArray();
            string status = "";
            foreach (string txt in msg_array)
            {
                status += txt + ",";
            }
            status = status.TrimEnd(',');

            return status;
        }
    }
}
