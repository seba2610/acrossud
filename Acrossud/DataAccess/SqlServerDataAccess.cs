using Microsoft.Win32;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acrossud
{
    /// <summary>
    /// Clase que implementas la clase abstracta DataAccess para Sql Server
    /// </summary>
    public class SqlServerDataAccess : DataAccess
    {
        public SqlServerDataAccess(string connStr)
        {
            this.SetConnStringEncripted(connStr);
        }

        public override DataSet ExecuteStoreProcedure(string procedureName, Dictionary<string, object> params_values = null)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                using (SqlCommand sqlCmd = new SqlCommand(procedureName, conn))
                {
                    using (SqlDataAdapter sqlDA = new SqlDataAdapter())
                    {
                        sqlCmd.Connection = (SqlConnection)conn;
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        if (params_values != null)
                        {
                            // Se recorre la lista de <parametro,valor> agregando cada uno a la sentencia
                            foreach (KeyValuePair<string, object> pair in params_values)
                            {
                                if (pair.Value.GetType() == typeof(int))
                                    sqlCmd.Parameters.Add(pair.Key, SqlDbType.Int, 4).Value = pair.Value;
                                else if (pair.Value.GetType() == typeof(byte[]))
                                    sqlCmd.Parameters.Add(pair.Key, SqlDbType.Binary).Value = pair.Value;
                                else if (pair.Value.GetType() == typeof(double))
                                    sqlCmd.Parameters.Add(pair.Key, SqlDbType.Float).Value = pair.Value;
                                else if (pair.Value.GetType() == typeof(DateTime))
                                    sqlCmd.Parameters.Add(pair.Key, SqlDbType.DateTime).Value = pair.Value;
                                else if (pair.Value.GetType() == typeof(DataTable))
                                    sqlCmd.Parameters.Add(pair.Key, SqlDbType.Structured).Value = pair.Value;
                                else
                                    sqlCmd.Parameters.Add(pair.Key, SqlDbType.VarChar, 6000).Value = pair.Value;
                            }
                        }

                        sqlDA.SelectCommand = sqlCmd;
                        sqlDA.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public override object ExecuteStoreProcedureWithReturnValue(string procedureName, Dictionary<string, object> params_values = null)
        {
            SqlParameter returnValue = null;

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlCommand sqlCmd = new SqlCommand(procedureName, conn))
                {

                    sqlCmd.Connection = (SqlConnection)conn;
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    returnValue = new SqlParameter();
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    sqlCmd.Parameters.Add(returnValue);

                    if (params_values != null)
                    {
                        // Se recorre la lista de <parametro,valor> agregando cada uno a la sentencia
                        foreach (KeyValuePair<string, object> pair in params_values)
                        {
                            if (pair.Value.GetType() == typeof(int))
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.Int, 4).Value = pair.Value;
                            else if (pair.Value.GetType() == typeof(byte[]))
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.Binary).Value = pair.Value;
                            else if (pair.Value.GetType() == typeof(double))
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.Float).Value = pair.Value;
                            else if (pair.Value.GetType() == typeof(DateTime))
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.DateTime).Value = pair.Value;
                            else if (pair.Value.GetType() == typeof(DataTable))
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.Structured).Value = pair.Value;
                            else
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.VarChar, 6000).Value = pair.Value;
                        }
                    }

                    sqlCmd.ExecuteNonQuery();
                }
            }

            return returnValue.Value;
        }
    }
}