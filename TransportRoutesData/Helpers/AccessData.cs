using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TransportRoutesData.Interfaces;
using TransportRoutesModel.ModelView;

namespace TransportRoutesData.Helpers
{
    public class AccessData : IAccessData
    {
        private readonly ILogger<AccessData> _logger;
        private readonly IConfiguration _config;

        public AccessData(IConfiguration config, ILogger<AccessData> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<ResponseList<T>> ExecuteReaderSpAsync<T>(string nameSP, List<SqlParameter> sqlParameters) where T : new()
        {
            ResponseList<T> data = new();
            data.response.status = true;

            using SqlConnection conn = new(_config["ConnectionStrings:SqlServer"]);
            SqlDataReader? reader = null;

            try
            {
                using SqlCommand StoreProc = new(nameSP, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                StoreProc.Parameters.AddRange(sqlParameters.ToArray());

                conn.Open();

                reader = await StoreProc.ExecuteReaderWithRetryAsync();

                if (reader != null)
                    while (reader.Read())
                    {
                        bool execute = false;
                        try
                        {
                            execute = Convert.ToInt32(reader["HasErrors"]?.ToString() ?? "0") == 0;
                        }
                        catch
                        {
                            execute = true;
                        }
                        if (execute)
                        {
                            T t = new();
                            Type type = t.GetType();

                            for (int inc = 0; inc < reader.FieldCount; inc++)
                            {
                                if (reader.GetName(inc).ToString() == "HasErrors")
                                    continue;

                                PropertyInfo? prop = type.GetProperty(reader.GetName(inc));

                                if (!string.IsNullOrEmpty(reader.GetValue(inc).ToString()))
                                {
                                    prop.SetValue(t, reader.GetValue(inc), null);
                                }
                            }

                            data.List.Add(t);
                        }
                        else
                        {
                            if (reader["ErrorMessage"] != DBNull.Value)
                                data.response.message = reader["ErrorMessage"].ToString() ?? "";
                            if (reader["ErrorSeverity"] != DBNull.Value)
                                data.response.code = reader["ErrorSeverity"].ToString() ?? "";
                            data.response.status = false;
                        }
                    }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return data;
        }

        public async Task<ResponseList<T>> ExecuteReaderSpEAsync<T>(string nameSP, List<SqlParameter> sqlParameters) where T : new()
        {
            ResponseList<T> data = new();
            data.response.status = true;

            using SqlConnection conn = new(_config["ConnectionStrings:SqlServer"]);
            SqlDataReader? reader = null;

            try
            {
                using SqlCommand StoreProc = new(nameSP, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                StoreProc.Parameters.AddRange(sqlParameters.ToArray());                

                conn.Open();

                reader = await StoreProc.ExecuteReaderWithRetryAsync();

                if (reader != null)
                    while (reader.Read())
                    {

                        if (Convert.ToInt32(reader["HasErrors"].ToString()) == 0)
                        {
                            T t = new();
                            Type type = t.GetType();

                            for (int inc = 0; inc < reader.FieldCount; inc++)
                            {
                                if (reader.GetName(inc).ToString() == "HasErrors")
                                    continue;

                                PropertyInfo? prop = type.GetProperty(reader.GetName(inc));

                                if (!string.IsNullOrEmpty(reader.GetValue(inc).ToString()))
                                {
                                    prop?.SetValue(t, reader.GetValue(inc), null);
                                }
                            }

                            data.List.Add(t);
                        }
                        else
                        {
                            Error Error = new();
                            Type type = Error.GetType();

                            for (int inc = 0; inc < reader.FieldCount; inc++)
                            {
                                if (reader.GetName(inc).ToString() == "HasErrors")
                                    continue;

                                PropertyInfo? prop = type.GetProperty(reader.GetName(inc));

                                if (!string.IsNullOrEmpty(reader.GetValue(inc).ToString()))
                                {
                                    prop?.SetValue(Error, reader.GetValue(inc), null);
                                }
                            }

                            data.Errors.Add(Error);
                            data.response.status = false;
                            data.response.message = Error.ErrorMessage;
                        }
                    }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return data;
        }

        public async Task<ResponseSelectListItem> ExecuteListSPAsync(string ValueList, string TextList, string NameSP)
        {
            ResponseSelectListItem responseSelectList = new();
            List<SelectListItem> listitems = new();
            Response response = new();

            using SqlConnection conn = new(_config["ConnectionStrings:SqlServer"]);
            try
            {
                SqlCommand StoreProc_enc = new(NameSP, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using (SqlDataReader? reader = await StoreProc_enc.ExecuteReaderWithRetryAsync())
                {
                    if (reader != null)
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader["HasErrors"].ToString()) == 0)
                            {
                                listitems.Add(new SelectListItem { Value = reader[ValueList].ToString(), Text = reader[TextList].ToString() });
                                response.status = true;
                            }
                            else
                            {
                                if (reader["ErrorMessage"] != DBNull.Value) response.message = reader["ErrorMessage"].ToString();
                                if (reader["ErrorSeverity"] != DBNull.Value) response.code = reader["ErrorSeverity"].ToString();
                                response.status = false;
                            }

                        }
                }
                responseSelectList.List = listitems;
                responseSelectList.Response = response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return responseSelectList;
        }

        public async Task<ResponseSelectListItem> ExecuteListSPAsync(string ValueList, string TextList, string NameSP, object value, string paramaterName)
        {

            ResponseSelectListItem responseSelectList = new();
            List<SelectListItem> listitems = new();
            Response response = new();

            using SqlConnection conn = new(_config["ConnectionStrings:SqlServer"]);
            try
            {
                SqlCommand StoreProc_enc = new(NameSP, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                StoreProc_enc.Parameters.AddWithValue(paramaterName, value);

                conn.Open();
                using (SqlDataReader? reader = await StoreProc_enc.ExecuteReaderWithRetryAsync())
                {
                    if (reader != null)
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader["HasErrors"].ToString()) == 0)
                            {
                                listitems.Add(new SelectListItem { Value = reader[ValueList].ToString(), Text = reader[TextList].ToString() });
                                response.status = true;
                            }
                            else
                            {
                                response.message = reader["ErrorMessage"] != DBNull.Value ? reader["ErrorMessage"]?.ToString() ?? string.Empty : string.Empty;
                                response.code = reader["ErrorSeverity"] != DBNull.Value ? reader["ErrorSeverity"]?.ToString() ?? string.Empty : string.Empty;
                                response.status = false;
                            }
                        }
                }
                responseSelectList.List = listitems;
                responseSelectList.Response = response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return responseSelectList;
        }

        public async Task<ResponseSelectListItem> ExecuteListSPAsync(string ValueList, string TextList, string NameSP, List<SqlParameter> sqlParameters)
        {

            ResponseSelectListItem responseSelectList = new();
            List<SelectListItem> listitems = new();
            Response response = new();

            using SqlConnection conn = new(_config["ConnectionStrings:SqlServer"]);
            try
            {
                SqlCommand StoreProc_enc = new(NameSP, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                StoreProc_enc.Parameters.AddRange(sqlParameters.ToArray());

                conn.Open();
                using (SqlDataReader? reader = await StoreProc_enc.ExecuteReaderWithRetryAsync())
                {
                    if (reader != null)
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader["HasErrors"].ToString()) == 0)
                            {
                                listitems.Add(new SelectListItem { Value = reader[ValueList].ToString(), Text = reader[TextList].ToString() });
                                response.status = true;
                            }
                            else
                            {
                                response.message = reader["ErrorMessage"] != DBNull.Value ? reader["ErrorMessage"]?.ToString() ?? string.Empty : string.Empty;
                                response.code = reader["ErrorSeverity"] != DBNull.Value ? reader["ErrorSeverity"]?.ToString() ?? string.Empty : string.Empty;
                                response.status = false;
                            }
                        }
                }
                responseSelectList.List = listitems;
                responseSelectList.Response = response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return responseSelectList;
        }        

        public async Task<DataSet?> GetDataset(string NameSP, object value, string paramaterName)
        {

            using SqlConnection conn = new(_config["ConnectionStrings:SqlServer"]);
            DataSet dataset = new DataSet();

            try
            {
                SqlCommand StoreProc = new(NameSP, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                StoreProc.Parameters.AddWithValue(paramaterName, value);

                conn.Open();

                DataTable dataTable = new DataTable();
                using (SqlDataReader? reader = await StoreProc.ExecuteReaderWithRetryAsync())
                {
                    if (reader != null)
                    {
                        do
                        {
                            dataTable = new();

                            DataColumn column;

                            //CABECERAS
                            foreach (DataRow rowSchema in reader.GetSchemaTable().Rows)
                            {
                                string? columnName = rowSchema["ColumnName"].ToString();
                                Type? dataType = (Type)rowSchema["DataType"];
                                column = new DataColumn(columnName, dataType);
                                dataTable.Columns.Add(column);
                            }

                            //DATOS
                            while (reader.Read())
                            {

                                DataRow row = dataTable.NewRow();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[i] = reader[i];
                                }
                                dataTable.Rows.Add(row);
                                dataTable.AcceptChanges();
                            }


                            dataset.Tables.Add(dataTable);
                            dataset.AcceptChanges();

                        } while (reader.NextResult());
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                if (dataset != null)
                    dataset.Dispose();

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dataset;
        }

        public async Task<DataSet?> GetDataset(string NameSP, List<SqlParameter> sqlParameters)
        {

            using SqlConnection conn = new(_config["ConnectionStrings:SqlServer"]);
            DataSet dataset = new DataSet();

            try
            {
                SqlCommand StoreProc = new(NameSP, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                StoreProc.Parameters.AddRange(sqlParameters.ToArray());

                conn.Open();

                DataTable dataTable = new DataTable();
                using (SqlDataReader? reader = await StoreProc.ExecuteReaderWithRetryAsync())
                {
                    if (reader != null)
                    {
                        do
                        {
                            dataTable = new();

                            DataColumn column;

                            //CABECERAS
                            foreach (DataRow rowSchema in reader.GetSchemaTable().Rows)
                            {
                                string? columnName = rowSchema["ColumnName"].ToString();
                                Type? dataType = (Type)rowSchema["DataType"];
                                column = new DataColumn(columnName, dataType);
                                dataTable.Columns.Add(column);
                            }

                            //DATOS
                            while (reader.Read())
                            {

                                DataRow row = dataTable.NewRow();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[i] = reader[i];
                                }
                                dataTable.Rows.Add(row);
                                dataTable.AcceptChanges();
                            }


                            dataset.Tables.Add(dataTable);
                            dataset.AcceptChanges();

                        } while (reader.NextResult());
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                if (dataset != null)
                    dataset.Dispose();

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dataset;
        }

    }
}
