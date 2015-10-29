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

        public static int getMachineID()
        {
            int machine_id = -1;
            SqlConnection con = new SqlConnection(ConnectionSB.ConnectionString);
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                string machine_name = Environment.MachineName;

                cmd.CommandText = string.Format("select id from machines where name='{0}'", machine_name);
                object o = cmd.ExecuteScalar();
                if (o == null)
                {

                    cmd.CommandText = string.Format("insert into machines (name) values ('{0}')", machine_name);
                    int n = cmd.ExecuteNonQuery();

                    cmd.CommandText = string.Format("select id from machines where name='{0}'", machine_name);
                    o = cmd.ExecuteScalar();
                }

                machine_id = (int)o;

            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                con.Close();
            }

            return machine_id;

        }
    }
}
