using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace PowerCalibration
{
    class DB
    {
        static SqlConnectionStringBuilder _constr;

        public static SqlConnectionStringBuilder ConnectionSB{get { return _constr; } set { _constr = value; } }

        /// <summary>
        /// Gets the machine id from database
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

                        string machine_name = Environment.MachineName;

                        cmd.CommandText = string.Format("select id from machines where name='{0}'", machine_name);
                        object o = cmd.ExecuteScalar();
                        if (o == null)
                        {

                            cmd.CommandText = string.Format("insert into machines (name) values ('{0}')", machine_name);
                            int n = cmd.ExecuteNonQuery();

                            cmd.CommandText = string.Format("select id from machines where name='{0}'", machine_name);
                            ret_obj = cmd.ExecuteScalar();
                        }
                    }

                    if(ret_obj != null)
                        machine_id = (int)ret_obj;
            }

            return machine_id;
        }
    }
}
