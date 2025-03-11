using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngularSPAWebAPI.Models;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace AngularSPAWebAPI.Services
{
    public class Dal
    {
        // Local server
        private static string _cnnString = Environment.GetEnvironmentVariable("DEF_CONN");
        private static string _cnnString_PubScreen = Environment.GetEnvironmentVariable("PUB_CONN");
        private static string _cnnString_Cogbytes = Environment.GetEnvironmentVariable("COG_CONN");

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
                try
                {
                    cn.Open();
                    return ExecuteNonQuery(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
            }
        }


        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
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
                try
                {
                    cn.Open();
                    using (var bulk = new SqlBulkCopy(cn))
                    {
                        bulk.DestinationTableName = destinationTableName;
                        bulk.WriteToServer(dtToInsert);
                    }
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }

            }

        }

        public static async Task ExecuteNonQueryAsync(string query, SqlParameter[] parameters)
        {
            await ExecuteNonQueryAsync(query, _cnnString, parameters);
        }
        public static async Task ExecuteNonQueryPubAsync(string query, SqlParameter[] parameters)
        {
            await ExecuteNonQueryAsync(query, _cnnString_PubScreen, parameters);
        }
        public static async Task ExecuteNonQueryCogAsync(string query, SqlParameter[] parameters)
        {
            await ExecuteNonQueryAsync(query, _cnnString_Cogbytes, parameters);
        }
        public static async Task ExecuteNonQueryAsync(string query, string connectionString, SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 300;
                        if (parameters != null)
                        {
                            foreach (SqlParameter param in parameters)
                            {
                                if (param.Value == null)
                                {
                                    param.Value = DBNull.Value;
                                }

                                command.Parameters.Add(param);
                            }

                        }
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in ExecuteNonQueryAsync");
                throw;
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
                try
                {
                    cn.Open();
                    return ExecuteNonQueryPub(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
            }
        }

        public static int ExecuteNonQueryPub(SqlConnection connection, CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
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
                try
                {
                    cn.Open();
                    return ExecuteNonQueryCog(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
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
            return ExecDS(cmdType, cmdTxt, (SqlParameter[])null);
        }

        //PubScrren
        public static DataSet ExecDSPub(CommandType cmdType, string cmdTxt, bool logEnabled = true)
        {
            return ExecDSPub(cmdType, cmdTxt, (SqlParameter[])null);
        }

        // pubscreen
        public static DataSet ExecDSPub(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_PubScreen))
            {
                try
                {
                    cn.Open();
                    return ExecDS(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
            }
        }

        public static DataSet ExecDSCog(CommandType cmdType, string cmdTxt, bool logEnabled = true)
        {
            return ExecDSCog(cmdType, cmdTxt, (SqlParameter[])null);
        }

        public static DataSet ExecDSCog(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_Cogbytes))
            {
                try
                {
                    cn.Open();
                    return ExecDS(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
            }
        }

        public static DataSet ExecDS(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString))
            {
                try
                {
                    cn.Open();
                    return ExecDS(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
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


        private static async Task<List<T>> GetReaderAsync<T>(string connectionString, string cmdTxt, Func<SqlDataReader, T> map , List<SqlParameter> cmdParams = null)
        {
            var connection = new SqlConnection(connectionString);
            List<T> result = new List<T>();
            try
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(cmdTxt, connection);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 300;

                if (cmdParams != null)
                {
                    foreach (var param in cmdParams)
                    {
                        if (param.Value != null)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                }
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(map(reader));
                    }
                    return result;
                }
                return default(List<T>);
            }
            catch (Exception ex)
            {
                string message = "Error in GetReaderAsync, the following error occured: " + ex.Message;
                if (ex.InnerException != null)
                {
                    message += ". The following inner exception: " + ex.InnerException;
                }
                Log.Error("Error in GetReaderAsync, the following error occured: " + message);
                throw;
            }
            finally
            {
                if(connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public static async Task<List<T>> ExecuteQueryAsync<T>(string query, Func<SqlDataReader, T> map, List<SqlParameter> cmdParams = null)
        {
            List<T> result = new List<T>();

            using (SqlConnection conn = new SqlConnection(_cnnString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;
                        if (cmdParams != null)
                        {
                            foreach (SqlParameter param in cmdParams)
                            {
                                if (param.Value == null)
                                {
                                    param.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(param);
                            }
                        }

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(map(reader));
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Log.Error(ex, "SQL Exception occurred while executing query: {Query}", query);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception occurred while executing query: {Query}", query);
                    throw;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        await conn.CloseAsync();
                    }
                }
            }

            return result;
        }

        public static async Task<T> ExecuteQuerySingleAsync<T>(string sql, Func<SqlDataReader, T> map, List<SqlParameter> cmdParams)
        {
            using (var connection = new SqlConnection(_cnnString))
            {
                try
                {
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;
                        if (cmdParams != null)
                        {
                            foreach (SqlParameter param in cmdParams)
                            {
                                if (param.Value == null)
                                {
                                    param.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(param);
                            }
                        }

                        await connection.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return map(reader);
                            }
                            else
                            {
                                return default;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Log.Error(ex, "SQL Exception occurred while executing query: {Query}", sql);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception occurred while executing query: {Query}", sql);
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        await connection.CloseAsync();
                    }
                }
            }
        }

        public static async Task<object> ExecScalarAsync(string query, List<SqlParameter> cmdParams = null)
        {
            return await ExecScalarAsync(_cnnString, query, cmdParams);
        }
        public static async Task<object> ExecScalarAsync(string connectionString, string query, List<SqlParameter> cmdParams = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;
                        if (cmdParams != null)
                        {
                            foreach (SqlParameter param in cmdParams)
                            {
                                if (param.Value == null)
                                {
                                    param.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(param);
                            }
                        }

                        return await cmd.ExecuteScalarAsync();
                    }
                }
                catch (SqlException ex)
                {
                    Log.Error(ex, "SQL Exception occurred while executing scalar query: {Query}", query);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception occurred while executing scalar query: {Query}", query);
                    throw;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        await conn.CloseAsync();
                    }
                }
            }
        }

        public static async Task<int> ExecuteNonQueryAsync(string query, List<SqlParameter> cmdParams = null)
        {
            using (SqlConnection conn = new SqlConnection(_cnnString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;
                        if (cmdParams != null)
                        {
                            foreach (SqlParameter param in cmdParams)
                            {
                                if (param.Value == null)
                                {
                                    param.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(param);
                            }
                        }

                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (SqlException ex)
                {
                    Log.Error(ex, "SQL Exception occurred while executing non-query: {Query}", query);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception occurred while executing non-query: {Query}", query);
                    throw;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        await conn.CloseAsync();
                    }
                }
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
        public static async Task<object> ExecScalarPubAsync(string query, List<SqlParameter> cmdParams = null)
        {
            return await ExecScalarAsync(_cnnString_PubScreen, query, cmdParams);
        }

        public static object ExecScalarPub(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString_PubScreen))
            {
                try
                {
                    cn.Open();
                    return ExecScalar(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
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
                try
                {
                    cn.Open();
                    return ExecScalar(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
            }
        }

        public static object ExecScalar(CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            using (SqlConnection cn = new SqlConnection(_cnnString))
            {
                try
                {
                    cn.Open();
                    return ExecScalar(cn, cmdType, cmdTxt, cmdParams);
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
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

        public static async Task<T> ExecuteQueryAsync<T>(string sql, SqlParameter[] parameters, Func<SqlDataReader, Task<T>> readFunc)
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(_cnnString);
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddRange(parameters);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return await readFunc(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Fatal($"SQL Exception: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Exception: {ex.Message}");
                throw;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
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

        public static async Task<DataTable> GetDataTableAsync(string cmdTxt, List<SqlParameter> cmdParams = null)
        {
            DataSet ds = await ExecDSAsync(CommandType.Text, _cnnString, cmdTxt, cmdParams);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }

            return null;
        }

        public static async Task<DataTable> GetDataTablePubAsync(string cmdTxt, List<SqlParameter> cmdParams = null)
        {
            DataSet ds = await ExecDSAsync(CommandType.Text, _cnnString_PubScreen, cmdTxt, cmdParams);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }

            return null;
        }

        public static async Task<DataSet> ExecDSAsync(CommandType cmdType, string connectionString, string cmdTxt, List<SqlParameter> cmdParams = null)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                try
                {
                    await cn.OpenAsync();
                    return await ExecDSAsync(cn, cmdType, cmdTxt, cmdParams.ToArray());
                }
                catch (SqlException ex)
                {
                    Log.Fatal($"SQL Exception: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Exception: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        await cn.CloseAsync();
                    }
                }
            }
        }

        public static async Task<DataSet> ExecDSAsync(SqlConnection connection, CommandType cmdType, string cmdTxt, params SqlParameter[] cmdParams)
        {
            DataSet ds = new DataSet();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = 300;
                await ProcCmdAsync(cmd, connection, cmdType, cmdTxt, cmdParams);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    await Task.Run(() => da.Fill(ds));
                }

                cmd.Parameters.Clear();
            }

            return ds;
        }

        public static async Task<DataTable> GetDataTablePubAsync(string query)
        {
            var dataTable = new DataTable();
            try
            {
                using (var connection = new SqlConnection(_cnnString_PubScreen))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            await Task.Run(() => adapter.Fill(dataTable));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "SQL Exception occurred while executing query: {Query}", query);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while executing query: {Query}", query);
                throw;
            }

            return dataTable;
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

        private static async Task ProcCmdAsync(SqlCommand cmd, SqlConnection connection, CommandType cmdType, string cmdTxt, SqlParameter[] cmdParams)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                cmd.Connection = connection;
                cmd.CommandText = cmdTxt;
                cmd.CommandType = cmdType;

                if (cmdParams != null)
                {
                    foreach (SqlParameter param in cmdParams)
                    {
                        if (param.Value == null)
                        {
                            param.Value = DBNull.Value;
                        }

                        cmd.Parameters.Add(param);
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Fatal($"SQL Exception: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Exception: {ex.Message}");
                throw;
            }
        }

        public static async Task<PubScreen> GetPaperInfoByIDAsync(int id)
        {
            var pubScreen = new PubScreen();
            string sql;
            try
            {
                using (SqlConnection cn = new SqlConnection(_cnnString_PubScreen))
                {
                    await cn.OpenAsync();
                    using (SqlTransaction transaction = cn.BeginTransaction())
                    {
                        try
                        {
                            // Define all SQL queries
                            var queries = new Dictionary<string, string>
                            {
                                { "AuthourID", "Select AuthorID From Publication_Author Where PublicationID = @PublicationID" },
                                { "CellTypeID", "Select CelltypeID From Publication_CellType Where PublicationID = @PublicationID" },
                                { "DiseaseID", "Select DiseaseID From Publication_Disease Where PublicationID = @PublicationID" },
                                { "SubModelID", "Select SubModelID From Publication_SubModel Where PublicationID = @PublicationID" },
                                { "MethodID", "Select MethodID From Publication_Method Where PublicationID = @PublicationID" },
                                { "SubMethodID", "Select SubMethodID From Publication_SubMethod Where PublicationID = @PublicationID" },
                                { "TransmitterID", "Select TransmitterID From Publication_NeuroTransmitter Where PublicationID = @PublicationID" },
                                { "RegionID", "Select RegionID From Publication_Region Where PublicationID = @PublicationID" },
                                { "sexID", "Select SexID From Publication_Sex Where PublicationID = @PublicationID" },
                                { "SpecieID", "Select SpecieID From Publication_Specie Where PublicationID = @PublicationID" },
                                { "StrainID", "Select StrainID From Publication_Strain Where PublicationID = @PublicationID" },
                                { "SubRegionID", "Select SubRegionID From Publication_SubRegion Where PublicationID = @PublicationID" },
                                { "TaskID", "Select TaskID From Publication_Task Where PublicationID = @PublicationID" },
                                { "SubTaskID", "Select SubTaskID From Publication_SubTask Where PublicationID = @PublicationID" },
                                { "PaperTypeID", "Select PaperTypeID From Publication_PaperType Where PublicationID = @PublicationID" },
                                { "Publication", "Select * From Publication Where ID = @PublicationID" }
                            };

                            // Execute all queries
                            foreach (var query in queries)
                            {
                                using (SqlCommand cmd = new SqlCommand(query.Value, cn, transaction))
                                {
                                    cmd.CommandTimeout = 300;
                                    cmd.Parameters.AddWithValue("@PublicationID", id);
                                    if (query.Key == "Publication")
                                    {
                                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                                        {
                                            if (await reader.ReadAsync())
                                            {
                                                pubScreen.PaperLinkGuid = reader.GetGuid(reader.GetOrdinal("PaperLinkGuid"));
                                                pubScreen.DOI = reader.GetString(reader.GetOrdinal("DOI"));
                                                pubScreen.Keywords = reader.GetString(reader.GetOrdinal("Keywords"));
                                                pubScreen.Title = reader.GetString(reader.GetOrdinal("Title"));
                                                pubScreen.Abstract = reader.GetString(reader.GetOrdinal("Abstract"));
                                                pubScreen.Year = reader.GetString(reader.GetOrdinal("Year"));
                                                pubScreen.Reference = reader.GetString(reader.GetOrdinal("Reference"));
                                                pubScreen.Source = reader.GetString(reader.GetOrdinal("Source"));
                                            }
                                        }
                                    }
                                    else if (query.Key == "PaperTypeID")
                                    {
                                        var result = await cmd.ExecuteScalarAsync();
                                        if (result == null)
                                        {
                                            pubScreen.PaperTypeID = null;
                                        }
                                        else
                                        {
                                            pubScreen.PaperTypeID = Int32.Parse(result.ToString());
                                        }
                                    }
                                    else
                                    {
                                        var result = await cmd.ExecuteScalarAsync();
                                        if (result != null)
                                        {
                                            var property = pubScreen.GetType().GetProperty(query.Key);
                                            property.SetValue(pubScreen, new int?[] { Convert.ToInt32(result) });
                                        }
                                    }
                                }
                            }

                            // Commit transaction
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction if any error occurs
                            transaction.Rollback();
                            Log.Error(ex, "Error in GetPaperInfoByIDAsync");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetPaperInfoByIDAsync");
                throw;
            }

            return pubScreen;
        }

        // pubscreen
        public static async Task<List<T>> GetReaderPubAsync<T>(string cmdTxt, Func<SqlDataReader, T> map, List<SqlParameter> cmdParams = null)
        {
            return await GetReaderAsync(_cnnString_PubScreen, cmdTxt, map, cmdParams);
        }

        public static async Task<List<T>> GetReaderCogAsync<T>(string cmdTxt, Func<SqlDataReader, T> map, List<SqlParameter> cmdParams = null)
        {
            return await GetReaderAsync(_cnnString_Cogbytes, cmdTxt, map, cmdParams);
        }

        public static async Task<List<T>> GetReader<T>(string cmdTxt, Func<SqlDataReader, T> map, List<SqlParameter> cmdParams = null)
        {
            return await GetReaderAsync(_cnnString, cmdTxt, map, cmdParams);
        }
    }
}
