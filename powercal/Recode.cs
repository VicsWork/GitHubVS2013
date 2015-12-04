using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using MinimalisticTelnet;

namespace PowerCalibration
{
    class Recode
    {
        public delegate void StatusHandler(object sender, string status_txt);
        public event StatusHandler Status_Event;

        public delegate void RunStatusHandler(object sender, string status_txt);
        public event RunStatusHandler Run_Status_Event;


        static string _app_data_dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calibration"); //The app folder where we save most logs, etc

        Ember _ember = null;
        TelnetConnection _telnet_connection; // Telnet connection to ISA3 Adapter

        public Recode(Ember ember)
        {
            _ember = ember;
        }

        public void Run(CancellationToken cancel)
        {
            string serial_number = "";
            DialogResult rc = ShowInputDialog(ref serial_number, inputbox_label: "Serial");
            if (rc == DialogResult.Cancel)
                throw new Exception("Serial number not entered");

            // Check to see if Ember is to be used as USB and open ISA channel if so
            // Also set the box address
            Ember.Interfaces ember_interface = (Ember.Interfaces)Enum.Parse(
                typeof(Ember.Interfaces), Properties.Settings.Default.Ember_Interface);
            _ember.Interface = ember_interface;
            if (_ember.Interface == Ember.Interfaces.USB)
            {
                _ember.Interface_Address = Properties.Settings.Default.Ember_Interface_USB_Address;
                fire_run_status("Open ISA Channels");
                _ember.OpenISAChannels();
            }
            else
            {
                _ember.Interface_Address = Properties.Settings.Default.Ember_Interface_IP_Address;
            }

            // Create a new telnet connection
            fire_run_status("Start telnet");
            // If interface is USB we use localhost
            string telnet_address = "localhost";
            if (_ember.Interface == Ember.Interfaces.IP)
                telnet_address = _ember.Interface_Address;
            _telnet_connection = new TelnetConnection(telnet_address, 4900);

            fire_run_status("Get UUT Tokens");
            TCLI.Tokens caltokens = new TCLI.Tokens(0, 0, 0, 0);
            string msg = "";
            bool got_tokens = false;
            string log_file = "";
            while (true)
            {
                if (cancel.IsCancellationRequested)
                    break;

                try
                {
                    string cmd_prefix = TCLI.Get_Custom_Command_Prefix(_telnet_connection);
                    caltokens = TCLI.Parse_Pinfo_Tokens(_telnet_connection, cmd_prefix);

                    msg = string.Format("Voltage Factor: {0}, ", caltokens.VoltageFactor);
                    msg += string.Format("Current Factor: {0}, ", caltokens.CurrentFactor);
                    msg += string.Format("VGain Token: 0x{0:X08}, ", caltokens.VoltageGainToken);
                    msg += string.Format("VGain Token: 0x{0:X08}\n", caltokens.CurrentGainToken);
                    fire_status(msg);

                    string filename = string.Format("tokens_{0}-{1:yyyy-MM-dd_hh-mm-ss-tt}.txt", serial_number, DateTime.Now);
                    log_file = Path.Combine(_app_data_dir, filename); // Path to the app log file
                    File.WriteAllText(log_file, msg);

                    got_tokens = true;

                    break;
                }
                catch (Exception ex)
                {
                    string retry_err_msg = ex.Message;
                    int max_len = 1000;
                    if (retry_err_msg.Length > max_len)
                        retry_err_msg = retry_err_msg.Substring(0, max_len) + "...";
                    DialogResult dlg_rc = MessageBox.Show(retry_err_msg, "Exception reading tokens", MessageBoxButtons.RetryCancel);
                    if (dlg_rc == System.Windows.Forms.DialogResult.Cancel)
                        throw;
                }
                finally
                {
                    _telnet_connection.Close();
                    _ember.CloseISAChannels();
                }
            }
            if (!got_tokens)
            {
                throw new Exception("Tokens not read");
            }

            try
            {
                // Code here
                fire_run_status("Code");
                Coder coder = new Coder(new TimeSpan(0, 2, 0));
                coder.Code(cancel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Repatch the tokens
                fire_run_status("Patch UUT Tokens");
                //caltokens.VoltageGainToken = 0x00405E00;
                //caltokens.CurrentGainToken = 0X002F7EB7;
                //caltokens.VoltageFactor = 240;
                //caltokens.CurrentFactor = 15;

                _ember.VoltageRefereceValue = caltokens.VoltageFactor;
                _ember.CurrentRefereceValue = caltokens.CurrentFactor;
                string cmd_str = _ember.CreateCalibrationPatchBath(vgain: caltokens.VoltageGainToken, igain: caltokens.CurrentGainToken);
                if (File.Exists(log_file))
                    File.AppendAllText(log_file, cmd_str);


                while (true)
                {
                    string[] ember_temp_files = Ember.CleanupTempPatchFile();
                    foreach (string file in ember_temp_files)
                        fire_status(string.Format("Ember temp file found and removed {0}", file));

                    try
                    {
                        string patch_output = _ember.RunCalibrationPatchBatch();
                        break;
                    }
                    catch (Exception ex)
                    {
                        string retry_err_msg = ex.Message;
                        int max_len = 1000;
                        if (retry_err_msg.Length > max_len)
                            retry_err_msg = retry_err_msg.Substring(0, max_len) + "...";
                        DialogResult dlg_rc = MessageBox.Show(retry_err_msg, "Exception patching tokens", MessageBoxButtons.RetryCancel);
                        if (dlg_rc == System.Windows.Forms.DialogResult.Cancel)
                            throw;
                    }
                }
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

        private static DialogResult ShowInputDialog(ref string input, string inputbox_label = "Name")
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = inputbox_label;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }
    }
}
