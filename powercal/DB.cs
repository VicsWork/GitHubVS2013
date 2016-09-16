using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Data.SqlTypes;

using System.Net;
using System.Net.NetworkInformation;

using Microsoft.Win32;

namespace PowerCalibration
{
    class DB
    {
        // The connection string
        static SqlConnectionStringBuilder _constr;
        public static SqlConnectionStringBuilder ConnectionSB { get { return _constr; } set { _constr = value; } }

        // The site id is cached
        static int _site_id = -1;
        static public int Site_ID
        {
            get
            {
                if (_site_id < 0)
                {
                    try
                    {
                        string macaddr_str = GetMacAndIpAddress().Item1;
                        ManufacturingStore_DataDataContext dc = new ManufacturingStore_DataDataContext(ConnectionSB.ConnectionString);
                        _site_id = dc.StationSites.Where(d => d.StationMac == macaddr_str).Select(s => s.ProductionSiteId).Single<int>();
                    }
                    catch { };
                }
                return _site_id;
            }
        }

        // The machine id is cached
        static int _machine_id = -1;
        static public int Machine_ID
        {
            get
            {
                if (_machine_id < 0)
                {
                    try
                    {
                        ManufacturingStore_DataDataContext dc = new ManufacturingStore_DataDataContext(ConnectionSB.ConnectionString);
                        try
                        {

                            // Set machine id
                            // This is commented out because we are updating data below
                            //_machine_id = dc.TestStationMachines.Where(m => m.Name == Environment.MachineName).Select(s => s.Id).Single<int>();

                            // Machine guid column was added after data had already been inserted
                            // Update data
                            TestStationMachine machine = dc.TestStationMachines.Single<TestStationMachine>(m => m.Name == Environment.MachineName);
                            _machine_id = machine.Id;
                            try
                            {
                                // Update machine GUI if null
                                if (machine.MachineGuid == null)
                                {
                                    machine.MachineGuid = GetMachineGuid();
                                    dc.SubmitChanges();
                                }
                            }
                            catch (Exception ex) { string m = ex.Message; };
                        }
                        catch { };

                        if (_machine_id < 0)
                        {
                            _machine_id = insertMachine();
                        }
                    }
                    catch (Exception ex) { string msg = ex.Message; };
                }

                return _machine_id;
            }
        }



        /// <summary>
        /// Gets the EUI Id
        /// </summary>
        /// <param name="eui"></param>
        /// <returns>EUI ID</returns>
        public static int GetEUIID(string eui)
        {
            int id = -1;
            using (SqlConnection con = new SqlConnection(ConnectionSB.ConnectionString))
            {
                con.Open();

                object ret_obj = null;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    string table_name = "[EuiList]";
                    cmd.CommandText = string.Format("select id from {0} where EUI='{1}'", table_name, eui);
                    ret_obj = cmd.ExecuteScalar();

                    if (ret_obj == null)
                    {
                        // Insert into database
                        cmd.CommandText = string.Format(
                            "insert into {0} (EUI, ProductionSiteId) values ('{1}', '{2}')",
                            table_name, eui, Site_ID);

                        int n = cmd.ExecuteNonQuery();

                        TraceLogger.Log(cmd.CommandText);

                        // Get the id
                        cmd.CommandText = string.Format("select id from {0} where EUI='{1}'", table_name, eui);
                        ret_obj = cmd.ExecuteScalar();
                    }
                }
                if (ret_obj != null)
                    id = (int)ret_obj;
            }
            return id;
        }


        /// <summary>
        /// Gets the machine id from database
        /// It creates an entry if not found
        /// </summary>
        /// <returns></returns>
        public static int insertMachine()
        {
            var mac_ip = GetMacAndIpAddress();
            string macaddr_str = mac_ip.Item1;
            string ip_str = mac_ip.Item2;

            ManufacturingStore_DataDataContext dc = new ManufacturingStore_DataDataContext(ConnectionSB.ConnectionString);

            string description = null;
            try { description = GetComputerDescription(); }
            catch (Exception) { };
            // Set a computer description based on domain and nic if one was not found
            if (description == null || description == "")
            {
                string production_site_name = null;
                try
                {
                    var ss = dc.StationSites.Where(d => d.StationMac == macaddr_str).Single();
                    production_site_name = ss.ProductionSite.Name;
                }
                catch (Exception) { };

                NetworkInterface nic = GetFirstNic();
                if (production_site_name != null)
                    description = string.Format("{0}, {1}", production_site_name, nic.Description);
                else
                    description = string.Format("{0}, {1}", Environment.UserDomainName, nic.Description);
            }


            TestStationMachine machine = new TestStationMachine();
            machine.Name = Environment.MachineName;
            machine.IpAddress = ip_str;
            machine.MacAddress = macaddr_str;
            machine.Description = description;
            try
            {
                string machineguid = GetMachineGuid();
                // Database should default to "00000000-0000-0000-0000-000000000000"
                // Just check to make sure we got the same length
                if (machineguid.Length <= 36)
                    machine.MachineGuid = machineguid;
            }
            catch { };

            dc.TestStationMachines.InsertOnSubmit(machine);
            dc.SubmitChanges();

            return machine.Id;

        }

        /// <summary>
        /// Gets the specified Nics mac and ip address
        /// </summary>
        /// <param name="nic"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetMacAndIpAddress()
        {
            string macaddr_str = "000000000000";
            string ip_str = "0.0.0.0";

            // Get the first network interface
            NetworkInterface nic = null;
            try { nic = GetFirstNic(); }
            catch (Exception) { };
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

            return Tuple.Create(macaddr_str, ip_str);
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
        /// Gets the machine gui id stored in the registry
        /// </summary>
        /// <returns></returns>
        public static string GetMachineGuid()
        {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";

            using (RegistryKey localMachineX64View =
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(
                            string.Format("Key Not Found: {0}", location));

                    object machineGuid = rk.GetValue(name);
                    if (machineGuid == null)
                        throw new IndexOutOfRangeException(
                            string.Format("Index Not Found: {0}", name));

                    return machineGuid.ToString();
                }
            }
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

        public static string[] GetISAAdapterIPsFromLikeLocation(string location)
        {
            using (SqlConnection con = new SqlConnection(ConnectionSB.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    string table_name = "[InsightAdapter]";
                    cmd.CommandText = string.Format("select IpAddress from {0} where Location like '{1}'", table_name, location);
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string> datalist = new List<string>();
                    while (reader.Read())
                    {
                        datalist.Add( reader.GetValue(0).ToString() );
                    }
                    return datalist.ToArray();
                }
            }
        }

        public static IPAddress GetFiratGatewayAddress()
        {
            NetworkInterface nic = GetFirstNic();

            var gate = nic.GetIPProperties().GatewayAddresses
                .Where(n => n.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .FirstOrDefault();

            return gate.Address;

        }

    }
}
