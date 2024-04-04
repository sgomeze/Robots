using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lbDataBase
{
    public static class DBUtil
    {
        public static DataTable GetData(string CadConn, string sqlCommand)
        {
            using(SqlConnection conn = new SqlConnection(CadConn))
            {
                using(SqlCommand cmd = new SqlCommand(sqlCommand, conn))
                {
                    try
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        dt.Load(cmd.ExecuteReader());
                        conn.Close();
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error GetData() error:{ex.Message}");
                    }
                }

            }
        }
    }
}
