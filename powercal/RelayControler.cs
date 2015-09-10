using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.DAQmx;

namespace powercal
{
    /// <summary>
    /// Class to control the DIO lines connecte dto the different relays
    /// </summary>
    class RelayControler
    {
        string _acPowerLbl = "AC Power";
        string _loadLbl = "Load";
        string _emberLbl = "Ember";

        /// <summary>
        /// Inits internal dictionary that holds line number and state info
        /// </summary>
        private void _initDicLines()
        {
            // Dic to store line numbers
            _dic_lines.Clear();
            _dic_lines.Add(_acPowerLbl, 1);
            _dic_lines.Add(_loadLbl, 2);
            _dic_lines.Add(_emberLbl, 4);

            // Dic to store line state (true = ON, false = OFF)
            _dic_values.Clear();
            _dic_values.Add(_acPowerLbl, false);
            _dic_values.Add(_loadLbl, false);
            _dic_values.Add(_emberLbl, false);

        }

        public bool AC_Power
        {
            get { return ReadLine(_acPowerLbl); }
            set { WriteLine(_acPowerLbl, value); }
        }
        public int AC_Power_LineNum
        {
            get { return _dic_lines[_acPowerLbl]; }
            set { _dic_lines[_acPowerLbl] = value; }
        }
        public string AC_Power_Label
        {
            get { return _acPowerLbl; }
            set { _acPowerLbl = value; }
        }

        public bool Load
        {
            get { return ReadLine(_loadLbl); }
            set { WriteLine(_loadLbl, value); }
        }
        public int Load_LineNum
        {
            get { return _dic_lines[_loadLbl]; }
            set { _dic_lines[_loadLbl] = value; }
        }

        public bool Ember
        {
            get { return ReadLine(_emberLbl); }
            set { WriteLine(_emberLbl, value); }
        }
        public int Ember_LineNum
        {
            get { return _dic_lines[_emberLbl]; }
            set { _dic_lines[_emberLbl] = value; }
        }


        /// <summary>
        /// Dic to store line numbers
        /// </summary>
        private Dictionary<string, int> _dic_lines = new Dictionary<string, int>();
        /// <summary>
        /// Dic to store line state (true = ON, false = OFF)
        /// </summary>
        private Dictionary<string, bool> _dic_values = new Dictionary<string, bool>();

        /// <summary>
        /// The DIO port info
        /// </summary>
        private string _dev_port;

        /// <summary>
        /// Constructor
        /// </summary>
        public RelayControler()
        {
            initDevPort();
            _initDicLines();
        }

        /// <summary>
        /// When disable we only store values.  This is usualfuul when running in manual mode
        /// </summary>
        public bool Disable
        {
            get { return _disable; }
            set { _disable = value; }
        }
        private bool _disable = false;

        /// <summary>
        /// Inits the DIO to be the first port found
        /// </summary>
        private void initDevPort()
        {
            if (_dev_port == null)
            {
                string[] data = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External);
                if (data.Length > 0)
                {
                    _dev_port = data[0];
                }
            }
        }

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
        /// <param name="linename"></param>
        /// <param name="value"></param>
        public void WriteLine(string linename, bool value)
        {
            if (Disable)
            {
                _dic_values[linename] = value;
            }
            else
            {
                int linenum = _dic_lines[linename];
                WriteLine(linenum, value);
            }
        }

        /// <summary>
        /// Writes to the specified line number
        /// </summary>
        /// <param name="linenum"></param>
        /// <param name="value"></param>
        public void WriteLine(int linenum, bool value)
        {
            if (Disable)
            {
                string linename = GetName(linenum);
                _dic_values[linename] = value;
                return;
            }

            using (Task digitalWriteTask = new Task())
            {
                //  Create an Digital Output channel and name it.
                string linestr = string.Format("{0}/line{1}", _dev_port, linenum);
                string name = string.Format("line{0}", linenum);
                digitalWriteTask.DOChannels.CreateChannel(linestr, name, ChannelLineGrouping.OneChannelForEachLine);

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                writer.WriteSingleSampleSingleLine(true, value);
            }
        }

        /// <summary>
        /// Reads the specified line name
        /// </summary>
        /// <param name="linename"></param>
        /// <returns></returns>
        public bool ReadLine(string linename)
        {
            if (Disable)
            {
                return _dic_values[linename];
            }
            else
            {
                int linenum = _dic_lines[linename];
                return ReadLine(linenum);
            }
        }

        /// <summary>
        /// Gets the specified line number's name
        /// </summary>
        /// <param name="linenum"></param>
        /// <returns></returns>
        public string GetName(int linenum)
        {
            string name = null;
            foreach (string key in _dic_lines.Keys)
            {
                int value = _dic_lines[key];
                if (value == linenum)
                {
                    name = key;
                    break;
                }
            }

            return name;

        }

        /// <summary>
        /// Reads the specified line number
        /// </summary>
        /// <param name="linenum"></param>
        /// <returns></returns>
        public bool ReadLine(int linenum)
        {
            if (Disable)
            {
                string linename = GetName(linenum);
                return _dic_values[linename];
            }

            using (Task digitalReaderTask = new Task())
            {
                //  Create an Digital Output channel and name it.
                string linestr = string.Format("{0}/line{1}", _dev_port, linenum);
                string name = string.Format("line{0}", linenum);
                digitalReaderTask.DOChannels.CreateChannel(linestr, name, ChannelLineGrouping.OneChannelForEachLine);

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReaderTask.Stream);
                return reader.ReadSingleSampleSingleLine();
            }

        }

        /// <summary>
        /// Returs the status of all lines
        /// </summary>
        /// <returns></returns>
        public string[] ToStrArray()
        {

            string[] msg = new string[_dic_lines.Count];
            int i = 0;
            foreach (string key in _dic_lines.Keys)
            {
                bool value = ReadLine(key);
                string on_off_str = "OFF";
                if (value)
                    on_off_str = "ON";
                msg[i++] = string.Format("{0} = {1}", key, on_off_str);
            }

            return msg;

        }

        /// <summary>
        /// Retuns the status of all lines formated to display
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
        /// Returs the status of all lines a text
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

            return status;
        }
    }
}
