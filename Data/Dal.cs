using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using Microsoft.Extensions.Configuration;

namespace AngularSPAWebAPI.Services
{
    public class Dal
    {
        // Local server

        private static string _cnnString = "Server=.\\sqlexpress;Database=MouseBytes;Trusted_Connection=True;MultipleActiveResultSets=true";
        private static string _cnnString_PubScreen = "Server=.\\sqlexpress;Database=PubScreen;Trusted_Connection=True;MultipleActiveResultSets=true";
        private static string _cnnString_Cogbytes = "Server=.\\sqlexpress;Database=CogBytes;Trusted_Connection=True;MultipleActiveResultSets=true";



        public Dal()
        {
        }

        public static int ExecuteNonQuery(string cmdTxt)
        {
            return ExecuteNonQuery(CommandType.Text, cmdTxt, (SqlParameter[])null);
        }

        public static int ExecuteNonQuery(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString))
            {
                cn.Open();

                return ExecuteNonQuery(cn, cmdType, cmdTxt, cmdParams);
            }
        }


        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            //LogHelper.Log(cmdTxt);

            int retval = -1;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = 300;
                ProcCmd(cmd, connection, cmdType, cmdTxt, cmdParams);

                retval = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
            }

            return retval;
        }

        public static void BulkInsert(DataTable dtToInsert, string destinationTableName)
        {

            using (SqlConnection cn = new SqlConnection(_cnnString))
            {
                cn.Open();

                using (var bulk = new SqlBulkCopy(cn))
                {
                    bulk.DestinationTableName = destinationTableName;
                    bulk.WriteToServer(dtToInsert);
                }

                cn.Close();

            }

        }

        public static int ExecuteNonQueryPub(string cmdTxt)
        {
            return ExecuteNonQueryPub(CommandType.Text, cmdTxt, (SqlParameter[])null);
        }

        public static int ExecuteNonQueryPub(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_PubScreen))
            {
                cn.Open();

                return ExecuteNonQueryPub(cn, cmdType, cmdTxt, cmdParams);
            }
        }

        public static int ExecuteNonQueryPub(SqlConnection connection, CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            //LogHelper.Log(cmdTxt);

            int retval = -1;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = 300;
                ProcCmd(cmd, connection, cmdType, cmdTxt, cmdParams);

                retval = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
            }

            return retval;
        }

        public static int ExecuteNonQueryCog(string cmdTxt)
        {
            return ExecuteNonQueryCog(CommandType.Text, cmdTxt, (SqlParameter[])null);
        }

        public static int ExecuteNonQueryCog(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_Cogbytes))
            {
                cn.Open();

                return ExecuteNonQueryCog(cn, cmdType, cmdTxt, cmdParams);
            }
        }

        public static int ExecuteNonQueryCog(SqlConnection connection, CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            //LogHelper.Log(cmdTxt);

            int retval = -1;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = 300;
                ProcCmd(cmd, connection, cmdType, cmdTxt, cmdParams);

                retval = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
            }

            return retval;
        }

        public static DataSet ExecDS(CommandType cmdType, string cmdTxt, bool logEnabled = true)
        {
            logEnabled = true;
            if (logEnabled)
            {
                //LogHelper.Log(cmdTxt);
            }

            return ExecDS(cmdType, cmdTxt, (SqlParameter[])null);
        }

        //PubScrren
        public static DataSet ExecDSPub(CommandType cmdType, string cmdTxt, bool logEnabled = true)
        {
            logEnabled = true;
            if (logEnabled)
            {
                //LogHelper.Log(cmdTxt);
            }

            return ExecDSPub(cmdType, cmdTxt, (SqlParameter[])null);
        }

        // pubscreen
        public static DataSet ExecDSPub(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_PubScreen))
            {
                cn.Open();

                return ExecDS(cn, cmdType, cmdTxt, cmdParams);
            }
        }

        public static DataSet ExecDSCog(CommandType cmdType, string cmdTxt, bool logEnabled = true)
        {
            logEnabled = true;
            if (logEnabled)
            {
                //LogHelper.Log(cmdTxt);
            }

            return ExecDSCog(cmdType, cmdTxt, (SqlParameter[])null);
        }

        public static DataSet ExecDSCog(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_Cogbytes))
            {
                cn.Open();

                return ExecDS(cn, cmdType, cmdTxt, cmdParams);
            }
        }

        public static DataSet ExecDS(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString))
            {
                cn.Open();

                return ExecDS(cn, cmdType, cmdTxt, cmdParams);
            }
        }

        public static DataSet ExecDS(SqlConnection connection, CommandType cmdType, string cmdTxt)
        {
            return ExecDS(connection, cmdType, cmdTxt, (SqlParameter[])null);
        }

        public static DataSet ExecDS(SqlConnection connection, CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            //LogHelper.Log(cmdTxt);
            DataSet ds = null;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = 300;

                ProcCmd(cmd, connection, cmdType, cmdTxt, cmdParams);

                ds = new DataSet();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }

                cmd.Parameters.Clear();
            }

            return ds;
        }



        private static SqlDataReader GetReader(SqlConnection connection, CommandType cmdType, string cmdTxt, SqlParameter[] cmdParams)
        {
            SqlDataReader dr;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = 300;
                ProcCmd(cmd, connection, cmdType, cmdTxt, cmdParams);

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                cmd.Parameters.Clear();
            }

            return dr;
        }

        public static SqlDataReader GetReader(string cmdTxt)
        {
            return GetReader(CommandType.Text, cmdTxt);
        }

        public static SqlDataReader GetReader(CommandType cmdType, string cmdTxt)
        {
            return GetReader(cmdType, cmdTxt, (SqlParameter[])null);
        }

        // pubscreen
        public static SqlDataReader GetReaderPub(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            SqlConnection cn = new SqlConnection(_cnnString_PubScreen);
            cn.Open();

            try
            {
                return GetReader(cn, cmdType, cmdTxt, cmdParams);
            }
            catch
            {
                cn.Close();
                throw;
            }
        }

        public static SqlDataReader GetReaderCog(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            SqlConnection cn = new SqlConnection(_cnnString_Cogbytes);
            cn.Open();

            try
            {
                return GetReader(cn, cmdType, cmdTxt, cmdParams);
            }
            catch
            {
                cn.Close();
                throw;
            }
        }

        public static SqlDataReader GetReader(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            SqlConnection cn = new SqlConnection(_cnnString);
            cn.Open();

            try
            {
                return GetReader(cn, cmdType, cmdTxt, cmdParams);
            }
            catch
            {
                cn.Close();
                throw;
            }
        }


        public static object ExecScalar(string cmdTxt)
        {
            return ExecScalar(CommandType.Text, cmdTxt);
        }


        // pubscreen

        public static object ExecScalarPub(string cmdTxt)
        {
            return ExecScalarPub(CommandType.Text, cmdTxt);
        }

        public static object ExecScalarPub(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_PubScreen))
            {
                cn.Open();
                return ExecScalar(cn, cmdType, cmdTxt, cmdParams);
            }
        }

        public static object ExecScalarCog(string cmdTxt)
        {
            return ExecScalarCog(CommandType.Text, cmdTxt);
        }

        public static object ExecScalarCog(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_Cogbytes))
            {
                cn.Open();
                return ExecScalar(cn, cmdType, cmdTxt, cmdParams);
            }
        }

        public static object ExecScalar(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString))
            {
                cn.Open();
                return ExecScalar(cn, cmdType, cmdTxt, cmdParams);
            }
        }

        public static object ExecScalar(SqlConnection connection, CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            //LogHelper.Log(cmdTxt);

            object retval = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = 300;
                ProcCmd(cmd, connection, cmdType, cmdTxt, cmdParams);
                retval = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
            }

            return retval;

        }


        public static DataTable GetDataTable(string cmdTxt, bool logEnabled = true)
        {
            return GetDataTable(CommandType.Text, cmdTxt, logEnabled);
        }
        public static DataTable GetDataTable(CommandType cmdType, string cmdTxt, bool logEnabled = true)
        {
            DataSet ds = null;
            ds = ExecDS(cmdType, cmdTxt, logEnabled);
            if ((ds != null))
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

            return null;
        }

        //Pubscreen
        public static DataTable GetDataTablePub(string cmdTxt, bool logEnabled = true)
        {
            return GetDataTablePub(CommandType.Text, cmdTxt, logEnabled);
        }
        public static DataTable GetDataTablePub(CommandType cmdType, string cmdTxt, bool logEnabled = true)
        {
            DataSet ds = null;
            ds = ExecDSPub(cmdType, cmdTxt, logEnabled);
            if ((ds != null))
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

            return null;
        }

        public static DataTable GetDataTableCog(string cmdTxt, bool logEnabled = true)
        {
            return GetDataTableCog(CommandType.Text, cmdTxt, logEnabled);
        }
        public static DataTable GetDataTableCog(CommandType cmdType, string cmdTxt, bool logEnabled = true)
        {
            DataSet ds = null;
            ds = ExecDSCog(cmdType, cmdTxt, logEnabled);
            if ((ds != null))
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

            return null;
        }
        private static void ProcCmd(SqlCommand cmd, SqlConnection connection, CommandType cmdType, string cmdTxt, SqlParameter[] cmdParams)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            cmd.Connection = connection;
            cmd.CommandText = cmdTxt;
            cmd.CommandType = cmdType;

            if (cmdParams != null)
            {

                foreach (SqlParameter param in cmdParams)
                {
                    if ((param.Direction == ParameterDirection.InputOutput) && (param.Value == null))
                    {
                        param.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(param);
                }

            }

            return;
        }

    }
}
