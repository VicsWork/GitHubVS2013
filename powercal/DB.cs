using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Data.SqlTypes;

using System.Net.NetworkInformation;

using Microsoft.Win32;

namespace PowerCalibration
{
    class DB
    {
        static SqlConnectionStringBuilder _constr;

        public static SqlConnectionStringBuilder ConnectionSB { get { return _constr; } set { _constr = value; } }

        /// <summary>
        /// Gets the machine id from database
        /// It creates an entry if not found
        /// </summary>
        /// <returns></returns>
        public static int getMachineID()
        {
            int machine_id = -1;
            using (SqlConnection con = new SqlConnection(ConnectionSB.ConnectionString))
            {
                con.Open();

                object ret_obj = null;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;

                    string table_name = "TestStationMachines";
                    string machine_name = Environment.MachineName;

                    cmd.CommandText = string.Format("select id from {0} where name='{1}'", table_name, machine_name);
                    ret_obj = cmd.ExecuteScalar();
                    if (ret_obj == null)
                    {
                        // Machine name not found in database
                        // Create entry
                        string description = null;
                        try { description = GetComputerDescription(); }
                        catch (Exception) { };

                        // Get the first network interface
                        NetworkInterface nic = null;
                        try { nic = GetFirstNic(); }
                        catch (Exception) { };

                        // Get mac address and ip address
                        string macaddr_str = "000000000000";
                        string ip_str = "0.0.0.0";
                        if (nic != null)
                        {
                            try
                            {
                                macaddr_str = nic.GetPhysicalAddress().ToString();
                                foreach (var ua in nic.GetIPProperties().UnicastAddresses)
                                {
                                    if (ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    {
                                        ip_str = ua.Address.ToString();
                                        break;
                                    }
                                }
                            }
                            catch (Exception) { };
                        }

                        // Set a computer description based on domain and nic if one was not found
                        if (description == null || description == "")
                        { description = string.Format("{0}, {1}", Environment.UserDomainName, nic.Description); }

                        // Insert into database
                        cmd.CommandText = string.Format(
                            "insert into {0} (Name, Description, MacAddress, IpAddress) values ('{1}', '{2}', '{3}', '{4}')",
                            table_name, machine_name, description, macaddr_str, ip_str);

                        int n = cmd.ExecuteNonQuery();

                        // Get the id
                        cmd.CommandText = string.Format("select id from machines where name='{0}'", machine_name);
                        ret_obj = cmd.ExecuteScalar();
                    }
                }

                if (ret_obj != null)
                    machine_id = (int)ret_obj;
            }

            return machine_id;
        }

        /// <summary>
        /// Gets the first Network Interface of the system
        /// </summary>
        /// <returns>First Network Interface of the system</returns>
        public static NetworkInterface GetFirstNic()
        {
            //var myInterfaceAddress = NetworkInterface.GetAllNetworkInterfaces()
            //    .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            //    .OrderByDescending(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            //    .Select(n => n.GetPhysicalAddress())
            //    .FirstOrDefault();

            NetworkInterface myInterfaceAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .OrderByDescending(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .FirstOrDefault();

            return myInterfaceAddress;
        }

        /// <summary>
        /// Returns the computer description
        /// </summary>
        /// <returns>the computer description</returns>
        public static string GetComputerDescription()
        {
            string key = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\lanmanserver\parameters";
            string computerDescription = (string)Registry.GetValue(key, "srvcomment", null);

            return computerDescription;
        }
    }
}
