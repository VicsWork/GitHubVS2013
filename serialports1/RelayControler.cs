using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.DAQmx;

namespace powercal
{
    class RelayControler
    {
        private void _initDicLines()
        {
            _dic_lines.Clear();
            _dic_lines.Add("AC Power", 1);
            _dic_lines.Add("Load", 2);
            _dic_lines.Add("Reset", 3);
            _dic_lines.Add("Output ", 4);

            _dic_values.Clear();
            _dic_values.Add("AC Power", false);
            _dic_values.Add("Load", false);
            _dic_values.Add("Reset", false);
            _dic_values.Add("Output ", false);

        }

        public bool AC_Power
        {
            get { return ReadLine("AC Power"); }
            set { WriteLine("AC Power", value); }
        }

        public bool Reset
        {
            get { return ReadLine("Reset"); }
            set { WriteLine("Reset", value); }
        }

        public bool Load
        {
            get { return ReadLine("Load"); }
            set { WriteLine("Load", value); }
        }

        public bool Output
        {
            get { return ReadLine("Output"); }
            set { WriteLine("Output", value); }
        }

        private Dictionary<string, int> _dic_lines = new Dictionary<string, int>();
        private Dictionary<string, bool> _dic_values = new Dictionary<string, bool>();

        public string DevPort
        {
            get { return _dev_port; }
            set { _dev_port = value; }
        }
        private string _dev_port;

        public RelayControler()
        {
            initDevPort();
            _initDicLines();
        }

        private bool _disable = false;
        public bool Disable
        {
            get { return _disable; }
            set { _disable = value; }
        }

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
                string linestr = string.Format("{0}/line{1}", DevPort, linenum);
                string name = string.Format("line{0}", linenum);
                digitalWriteTask.DOChannels.CreateChannel(linestr, name, ChannelLineGrouping.OneChannelForEachLine);

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                writer.WriteSingleSampleSingleLine(true, value);
            }
        }

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
                string linestr = string.Format("{0}/line{1}", DevPort, linenum);
                string name = string.Format("line{0}", linenum);
                digitalReaderTask.DOChannels.CreateChannel(linestr, name, ChannelLineGrouping.OneChannelForEachLine);

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReaderTask.Stream);
                return reader.ReadSingleSampleSingleLine();
            }

        }

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
