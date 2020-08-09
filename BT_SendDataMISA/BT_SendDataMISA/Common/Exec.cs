using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BT_SendDataMISA.Common
{
    public class Exec : IDisposable
    {
        const string PREFIX_VARIABLE_SQL = "@";

        public const int QueryTypeNoOutput = 1;
        public const int QueryTypeWithOutputs = 2;
        public const int QueryTypeGetDataTable = 3;
        static public string ConnectionString { get; set; }

        public DbTransaction transac { get; set; }
        private string storeName = "";
        private string _QueryString = "";
        private Hashtable htParams = new Hashtable();
        public DbConnection conn { get; set; }

        static public DbConnection GetDbConnection(string ConnStr)
        {
            try
            {
                ConnectionString = ConnStr;
                DbConnection conn = new SqlConnection(ConnStr);
                return conn;
            }
            catch { }

            return null;
        }

        #region
        /// Hàm tĩnh, cho phép execute store storeName trong DB mà không cần khởi tạo object Exec, dùng default ConnectionString (được set trong key AppSettingsDBKey = "MSSQL" ở file app.config hoặc web.config)
        /// </summary>
        /// <param name="storeName"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName)
        {
            return ExecStore(ConnectionString, storeName, new { }, QueryTypeNoOutput, null, out List<object> outValue);
        }
        /// <summary>
        /// Hàm tĩnh, cho phép execute store storeName trong DB mà không cần khởi tạo object Exec, với connection string strConnect được truyền vào
        /// </summary>
        /// <param name="strConnect"></param>
        /// <param name="storeName"></param>
        static public string ExecStore(string strConnect, string storeName)
        {
            return ExecStore(strConnect, storeName, new { }, QueryTypeNoOutput, null, out List<object> outValue);
        }
        /// <summary>
        /// Hàm tĩnh, cho phép execute store storeName trong DB, với tham số parameters mà không cần khởi tạo object Exec, dùng default ConnectionString (được set trong key AppSettingsDBKey = "MSSQL" ở file app.config hoặc web.config)
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="parameters"></param>
        static public string ExecStore<T>(string storeName, T parameters) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, QueryTypeNoOutput, null, out List<object> outValues);
        }
        /// <summary>
        /// Hàm tĩnh, cho phép execute store storeName trong DB, với tham số parameters mà không cần khởi tạo object Exec, với connection string strConnect được truyền vào
        /// </summary>
        /// <param name="strConnect"></param>
        /// <param name="storeName"></param>
        /// <param name="parameters"></param>
        static public string ExecStore<T>(string strConnect, string storeName, T parameters) where T : class
        {
            return ExecStore(strConnect, storeName, parameters, QueryTypeNoOutput, null, out List<object> outValue);
        }
        #endregion


        #region One output: Các hàm tĩnh, cho phép execute store và trả về 1 giá trị mà không cần chỉ rõ tên tham số trả về (Trả về 1 giá trị có kiểu là 1 trong các kiểu: string, int, long, object)
        // GetField by Scalar

        #region One output: type string
        /// <summary>
        /// ExecStore and return one output with type of string
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, out string v)
        {
            return ExecStore(ConnectionString, storeName, new { }, null, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, out string v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, null, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, out string v) where T : class
        {
            return ExecStore(strConnect, storeName, parameters, null, out v);
        }
        #endregion

        #region One output: type int
        /// <summary>
        /// ExecStore and return one output with type of int
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, out int v)
        {
            return ExecStore(ConnectionString, storeName, new { }, null, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, out int v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, null, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, out int v) where T : class
        {
            return ExecStore(strConnect, storeName, parameters, null, out v);
        }
        #endregion

        #region One output: type long
        /// <summary>
        /// ExecStore and return one output with type of long
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, out long v)
        {
            return ExecStore(ConnectionString, storeName, new { }, null, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, out long v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, null, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, out long v) where T : class
        {
            return ExecStore(strConnect, storeName, parameters, null, out v);
        }
        #endregion

        #region One output: type bool
        /// <summary>
        /// ExecStore and return one output with type of bool
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, out bool v)
        {
            return ExecStore(ConnectionString, storeName, new { }, null, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, out bool v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, null, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, out bool v) where T : class
        {
            return ExecStore(strConnect, storeName, parameters, null, out v);
        }
        #endregion

        #region One output: type Datatable
        /// <summary>
        /// ExecStore and return one output with type of DataTable
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, out DataTable dt)
        {
            return ExecStore(ConnectionString, storeName, null, out dt);
        }
        static public string ExecStore(string strConnect, string storeName, out DataTable dt)
        {
            return ExecStore(strConnect, storeName, null, out dt);
        }
        static public string ExecStore<T>(string storeName, T parameters, out DataTable dt) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, out dt);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, out DataTable dt) where T : class
        {
            Hashtable htParams = new Hashtable();
            AddParams(strConnect, ref htParams, parameters);

            return ExecStore(strConnect, storeName, htParams, out dt);
        }
        static public string ExecStore(string strConnect, string storeName, Hashtable htParams, out DataTable dt)
        {
            dt = null;

            string msg = ExecStore(strConnect, storeName, htParams, QueryTypeGetDataTable, null, out List<object> outValue);
            if (msg.Length > 0) return msg;

            if (outValue == null) return "Error: outValue is null @ExecStore";
            if (outValue.Count == 0) return "Error: outValue has no value @ExecStore";

            dt = outValue[0] as DataTable; // cast về DataTable. Trả về null nếu không cast được

            return msg;
        }
        #endregion

        #region One output: type object
        /// <summary>
        /// ExecStore and return one output with type of object
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, out object v)
        {
            return ExecStore(ConnectionString, storeName, new { }, null, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, out object v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, null, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, out object v) where T : class
        {
            return ExecStore(strConnect, storeName, parameters, null, out v);
        }
        #endregion
        #endregion


        #region One output: Các hàm tĩnh, cho phép execute store và trả về 1 giá trị của tham số có tên được chỉ định (Trả về 1 giá trị có kiểu là 1 trong các kiểu: string, int, long, object)
        // GetField by Output Var      

        #region One output: type Guid
        /// <summary>
        /// ExecStore with storeName, params, out param outVarName and return one output with type of Guid
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="outVarName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, string outVarName, out Guid v)
        {
            return ExecStore(ConnectionString, storeName, new { }, outVarName, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, string outVarName, out Guid v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, outVarName, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, string outVarName, out Guid v) where T : class
        {
            v = Guid.Empty;

            string msg = ExecStore(strConnect, storeName, parameters, outVarName, out object outValue);
            if (msg.Length > 0) return msg;

            if (outValue == null) return "Error: Out value is null @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);

            msg = Convertor.StringToGuid(outValue.ToString(), out v);

            return msg;
        }
        #endregion

        #region One output: type string
        /// <summary>
        /// ExecStore with storeName, params, out param outVarName and return one output with type of string
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="outVarName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, string outVarName, out string v)
        {
            return ExecStore(ConnectionString, storeName, new { }, outVarName, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, string outVarName, out string v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, outVarName, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, string outVarName, out string v) where T : class
        {
            v = null;

            object outValue = null;
            string msg = ExecStore(strConnect, storeName, parameters, outVarName, out outValue);
            if (msg.Length > 0) return msg;

            //if (outValue == null) return "Error: Out value is null @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);
            if (outValue == null) return ""; // accept null value for out string

            v = Convert.ToString(outValue);

            return msg;
        }
        #endregion

        #region One output: type int
        /// <summary>
        /// ExecStore with storeName, params, out param outVarName and return one output with type of int
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="outVarName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, string outVarName, out int v)
        {
            return ExecStore(ConnectionString, storeName, new { }, outVarName, out v);
        }
        static public string ExecStoreOutRowsAffected<T>(string storeName, T parameters, out int v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, "RowsAffected", out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, string outVarName, out int v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, outVarName, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, string outVarName, out int v) where T : class
        {
            v = 0;

            object outValue = null;
            string msg = ExecStore(strConnect, storeName, parameters, outVarName, out outValue);
            if (msg.Length > 0) return msg;

            if (outValue == null) return "Error: Out value is null @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);

            if (!int.TryParse(outValue.ToString(), out v))
                msg = "Error: Cannot convert out value to int @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);

            return msg;
        }
        #endregion

        #region One output: type long
        /// <summary>
        /// ExecStore with storeName, params, out param outVarName and return one output with type of long
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="outVarName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, string outVarName, out long v)
        {
            return ExecStore(ConnectionString, storeName, new { }, outVarName, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, string outVarName, out long v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, outVarName, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, string outVarName, out long v) where T : class
        {
            v = 0;

            string msg = ExecStore(strConnect, storeName, parameters, outVarName, out object outValue);
            if (msg.Length > 0) return msg;

            if (outValue == null) return "Error: Out value is null @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);

            if (!long.TryParse(outValue.ToString(), out v))
                msg = "Error: Cannot convert out value to long @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);

            return msg;
        }
        #endregion

        #region One output: type bool
        /// <summary>
        /// ExecStore with storeName, params, out param outVarName and return one output with type of bool
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="outVarName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, string outVarName, out bool v)
        {
            return ExecStore(ConnectionString, storeName, new { }, outVarName, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, string outVarName, out bool v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, outVarName, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, string outVarName, out bool v) where T : class
        {
            v = false;

            object outValue = null;
            string msg = ExecStore(strConnect, storeName, parameters, outVarName, out outValue);
            if (msg.Length > 0) return msg;

            if (outValue == null) return "Error: Out value is null @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);

            if (!bool.TryParse(outValue.ToString(), out v))
                msg = "Error: Cannot convert out value to bool @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);

            return msg;
        }
        #endregion

        #region One output: type object
        /// <summary>
        /// ExecStore with storeName, params, out param outVarName and return one output with type of object
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="outVarName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, string outVarName, out object v)
        {
            return ExecStore(ConnectionString, storeName, new { }, outVarName, out v);
        }
        static public string ExecStore<T>(string storeName, T parameters, string outVarName, out object v) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, outVarName, out v);
        }
        static public string ExecStore(string strConnect, string storeName, string outVarName, out object v)
        {
            return ExecStore(strConnect, storeName, new { }, outVarName, out v);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, string outVarName, out object v) where T : class
        {
            v = null;
            List<string> outVarNames = null;
            List<object> vs = null;

            if (!string.IsNullOrWhiteSpace(outVarName)) outVarNames = new List<string> { outVarName };
            string msg = ExecStore(strConnect, storeName, parameters, QueryTypeWithOutputs, outVarNames, out vs);
            if (msg.Length > 0) return msg;

            if (vs == null) return "Error: outValue is null @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);
            if (vs.Count == 0) return "Error: outValue has no value @ExecStore with spName " + storeName + " and param " + GetListParam(parameters);

            v = vs[0];
            return "";
        }
        #endregion

        #region Many outputs: type List<object>
        /// <summary>
        /// ExecStore with storeName, params, out param outVarName and return one output with type of object
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="outVarNames"></param>
        /// <param name="vs"></param>
        /// <returns></returns>
        static public string ExecStore(string storeName, List<string> outVarNames, out List<object> vs)
        {
            return ExecStore(ConnectionString, storeName, new { }, QueryTypeWithOutputs, outVarNames, out vs);
        }
        static public string ExecStore<T>(string storeName, T parameters, List<string> outVarNames, out List<object> vs) where T : class
        {
            return ExecStore(ConnectionString, storeName, parameters, QueryTypeWithOutputs, outVarNames, out vs);
        }
        static public string ExecStore<T>(string strConnect, string storeName, T parameters, List<string> outVarNames, out List<object> vs) where T : class
        {
            return ExecStore(strConnect, storeName, parameters, QueryTypeWithOutputs, outVarNames, out vs);
        }
        #endregion
        #endregion


        #region Hàm private, phục vụ các hàm tĩnh execute store ở trên
        // Hàm tĩnh, cho phép execute store trong DB với SqlConn, SqlTransac, storeName, và DS tham số htParam được truyền vào
        // Đây là hàm "nguyên tố", được nạp chồng bởi các hàm khác, nên có đầy đủ các tham số để có thể "tự chạy"
        // Hàm này tự tạo SqlConn, dùng để chạy các câu query đơn, không có transaction
        // Example: BSS.Exec.ExecStore("sp_test", new { var1, var2, var3 }, "Name", out sName);


        static private string ExecStore<T>(string strConnect, string storeName, T parameters, int queryType, List<string> outVarNames, out List<object> outValues) where T : class
        {
            Hashtable htParams = new Hashtable();
            AddParams(strConnect, ref htParams, parameters);

            return ExecStore(strConnect, storeName, htParams, queryType, outVarNames, out outValues);

        }
        static private string ExecStore(string strConnect, string storeName, Hashtable htParams, int queryType, List<string> outVarNames, out List<object> outValues)
        {
            outValues = null;
            if (string.IsNullOrWhiteSpace(strConnect)) return "Error: Connection string is null or empty @ExecStore";

            DbConnection conn = GetDbConnection(strConnect);
            var msg = ExecStore(conn, null, storeName, htParams, queryType, false, null, outVarNames, out outValues);
            conn?.Dispose();

            return msg;
        }
        #endregion


        #region Hàm nguyên tố tĩnh: cho phép execute store trong DB với SqlConn, SqlTransac, storeName, và DS tham số htParam được truyền vào
        // Đây là hàm "nguyên tố", được nạp chồng bởi các hàm khác, nên có đầy đủ các tham số để có thể "tự chạy"

        public delegate void HandleCallbackFunc(IAsyncResult result);

        static private string ExecStore(DbConnection conn, DbTransaction transac, string storeName, Hashtable htParams, int queryType, bool isAsync, HandleCallbackFunc HandleCallback, List<string> outVarNames, out List<object> outValues)
        {
            outValues = null;

            if (conn == null) return "Error: conn is null @ExecStore";

            string msg = "";

            DbCommand cmdToExecute = new SqlCommand(storeName, conn as SqlConnection);

            if (transac != null) cmdToExecute.Transaction = transac;

            cmdToExecute.CommandType = CommandType.StoredProcedure;

            string sParam = "";
            if (htParams != null)
            {
                foreach (DictionaryEntry p in htParams)
                {
                    string value = "";
                    DbParameter sp = null;

                    if (p.Value == null)
                    {
                        sp = new SqlParameter(p.Key.ToString(), DBNull.Value);
                        value = "null";
                    }
                    else if (p.Value.GetType().IsArray)
                    {
                        sp = new SqlParameter(p.Key.ToString(), SqlDbType.Binary);
                        sp.Value = p.Value;
                        value = "Binary Type";
                    }
                    else
                    {
                        sp = new SqlParameter(p.Key.ToString(), p.Value);
                        if (p.Value.GetType() == typeof(DateTime))
                        {
                            value = "'" + DateTime.Parse(p.Value.ToString()).ToString("yyyy-MM-dd hh:mm:ss") + "'";
                        }
                        else if (p.Value.ToString() == "") value = "''";
                        else value = p.Value.ToString();
                    }

                    cmdToExecute.Parameters.Add(sp);

                    sParam += p.Key.ToString() + " = " + value + ",";
                }
            }

            List<DbParameter> spOuts = null;
            bool isOutRowsAffected = false;
            if (outVarNames != null)
            {
                if (outVarNames.Count == 1 && outVarNames[0].ToLower() == "rowsaffected") isOutRowsAffected = true;
                else
                {
                    spOuts = new List<DbParameter>();
                    foreach (string outVarName in outVarNames)
                    {
                        DbParameter spOut = new SqlParameter(outVarName, DBNull.Value);

                        spOut.Direction = ParameterDirection.Output;
                        spOut.Size = 1000;
                        cmdToExecute.Parameters.Add(spOut);

                        spOuts.Add(spOut);
                    }
                }
            }

            int rowsAffected = -1;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                switch (queryType)
                {
                    case QueryTypeNoOutput:
                        if (isAsync)
                        {
                            if (HandleCallback == null) HandleCallback = HandleCallback_Default;
                            AsyncCallback callback = new AsyncCallback(HandleCallback);

                            ((SqlCommand)cmdToExecute).BeginExecuteNonQuery(callback, cmdToExecute);
                        }
                        else rowsAffected = cmdToExecute.ExecuteNonQuery();
                        break;
                    case QueryTypeWithOutputs:
                        if (isOutRowsAffected) rowsAffected = cmdToExecute.ExecuteNonQuery();
                        else
                        {
                            outValues = new List<object>();

                            if (outVarNames != null)
                            {
                                rowsAffected = cmdToExecute.ExecuteNonQuery();

                                foreach (var spOut in spOuts) outValues.Add(spOut.Value);
                            }
                            else outValues.Add(cmdToExecute.ExecuteScalar());
                        }
                        break;
                    case QueryTypeGetDataTable:
                        using (var ds = new DataSet())
                        {
                            DbDataAdapter dad = new SqlDataAdapter((SqlCommand)cmdToExecute);

                            rowsAffected = dad.Fill(ds);
                            outValues = new List<object>();
                            outValues.Add(ds.Tables[0]);
                            dad.Dispose();
                        }
                        break;
                    default: msg = "Error: QueryType is unknown @ExecStore"; break;
                }
            }
            catch (Exception ex)
            {
                msg = "StoreName EXEC " + storeName + " " + sParam + " error: " + ex.ToString();
                if (transac == null && conn != null && conn.State != ConnectionState.Closed) conn.Close();
            }
            finally
            {
                if (!isAsync) // Nếu không phải async thì close SQL connection. Nếu là async thì connection sẽ được close ở hàm callback async.      
                    if (transac == null && conn != null && conn.State != ConnectionState.Closed) conn.Close();
            }
            cmdToExecute?.Dispose();

            if (isOutRowsAffected)
            {
                outValues = new List<object>();
                outValues.Add(rowsAffected);
            }

            return msg;
        }

        static public string ExecStoreAsync<T>(string storeName, T parameters, HandleCallbackFunc HandleCallback = null) where T : class
        {
            Hashtable htParams = new Hashtable();
            string msg = AddParams(ConnectionString, ref htParams, parameters);
            if (msg.Length > 0) return msg;

            DbConnection conn = GetDbConnection(ConnectionString);
            msg = ExecStore(conn, null, storeName, htParams, QueryTypeNoOutput, true, HandleCallback, null, out List<object> outValues);
            conn?.Dispose();

            return msg;
        }
        static private void HandleCallback_Default(IAsyncResult result)
        {
            DbConnection conn = null;
            string sParam = "";
            try
            {

                DbCommand cmd = result.AsyncState as SqlCommand;
                conn = cmd.Connection;

                sParam = cmd.CommandText + " => ";
                foreach (SqlParameter pa in cmd.Parameters) sParam += "[" + pa.ParameterName + " = " + pa.Value + "]";

                int rowCount = ((SqlCommand)result.AsyncState).EndExecuteNonQuery(result);
            }
            catch /*(Exception ex)*/
            {
            }
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        #endregion

        #region Các hàm động, cho phép execute store với SqlConn, SqlTransac, storeName, htParam đã được set với object Exec. Dùng khi cần có transaction với các câu query khác
        public string ExecStore()
        {
            List<object> outValue = null;
            return ExecStore(conn, transac, storeName, htParams, QueryTypeNoOutput, false, null, null, out outValue);
        }
        /// <summary>
        /// Hàm thực thi các store sau khi chạy lệnh SetStoreNameAndParams để điền các tham số đầu vào và tên store cần thực thi và có giá trị trả ra 
        /// </summary>
        /// <param name="outVarNames">Tên biến quy định là biến output trong store</param>
        /// <param name="outValues">Biến sẽ được gán giá trị sau khi chạy</param>
        /// <returns>Giá trị trả về rỗng là thành công, còn khác rỗng là lỗi</returns>
        public string ExecStore(List<string> outVarNames, out List<object> outValues)
        {
            outValues = null;
            return ExecStore(conn, transac, storeName, htParams, QueryTypeWithOutputs, false, null, outVarNames, out outValues);
        }
        /// <summary>
        /// Hàm thực thi các store sau khi chạy lệnh SetStoreNameAndParams để điền các tham số đầu vào và tên store cần thực thi và có giá trị trả ra
        /// BSS.Exec dbm = new BSS.Exec();
        /// DataTable dt = null;
        /// string err = obj.SetStoreNameAndParams("dbo.sp_LogActivity_SelectOne", new { ID = 1 });
        /// err = ExecStore(out dt);
        /// </summary>
        /// <param name="dt">Table chứa giá trị sau khi chạy</param>
        /// <returns>Giá trị trả về rỗng là thành công, còn khác rỗng là lỗi</returns>
        public string ExecStore(out DataTable dt)
        {
            dt = null;

            List<object> outValues = null;
            string msg = "";
            msg = ExecStore(conn, transac, storeName, htParams, QueryTypeGetDataTable, false, null, null, out outValues);
            if (msg.Length > 0) return msg;

            if (outValues.Count == 0) return "Error: No out value @ExecStore with spName " + storeName;

            dt = outValues[0] as DataTable; // cast về DataTable. Trả về null nếu không cast được

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storeName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string SetStoreNameAndParams<T>(string storeName, T parameters) where T : class
        {
            this.storeName = storeName;
            htParams.Clear();

            return AddParams(parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Exec ClearParams()
        {
            htParams.Clear();
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string AddParams<T>(T parameters) where T : class
        {
            return AddParams(ConnectionString, ref htParams, parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strConnect"></param>
        /// <param name="htParams"></param>
        /// <param name="parameters"></param>
        static public string AddParams<T>(string strConnect, ref Hashtable htParams, T parameters) where T : class
        {
            if (parameters == null) return "Error: parameters is null @Exec.AddParams";

            string msg = "";
            try
            {
                var ps = typeof(T).GetProperties();
                foreach (var p in ps)
                    htParams.Add(GetPrefixVariable(strConnect) + p.Name, p.GetValue(parameters, null));
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        private static string GetPrefixVariable(string strConnect)
        {
            return PREFIX_VARIABLE_SQL;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenConnection()
        {
            if (conn.State != ConnectionState.Open) conn.Open();
        }
        /// <summary>
        /// 
        /// </summary>
        public void CloseConnection()
        {
            if (transac == null)
            {
                if (conn.State != ConnectionState.Closed) conn.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void BeginTransac()
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            transac = conn.BeginTransaction();
        }
        /// <summary>
        /// 
        /// </summary>
        public void CommitTransac()
        {
            if (transac != null)
            {
                transac.Commit();
                transac = null;
            }
            if (conn.State != ConnectionState.Closed) conn.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        public void RollBackTransac()
        {
            if (transac != null)
            {
                transac.Rollback();
                transac = null;
            }
            if (conn.State != ConnectionState.Closed) conn.Close();
        }
        /// <summary>
        /// Dispose object Exec
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CloseConnection();
            conn?.Dispose();
        }
        #endregion

        #region
        static string GetListParam<T>(T parameters) where T : class
        {
            Hashtable htParams = new Hashtable();
            AddParams(ConnectionString, ref htParams, parameters);

            string sParam = "";
            if (htParams != null)
            {
                foreach (DictionaryEntry p in htParams)
                {
                    string value = "";

                    if (p.Value == null) value = "null";
                    else if (p.Value.GetType().IsArray) value = "Binary Type";
                    else value = p.Value.ToString();

                    sParam += p.Key.ToString() + " = " + value + ";";
                }
            }

            return sParam;
        }
        #endregion


        #region Các hàm convert từ 1 Row trong DataTable về 1 Object; từ 1 DataTable về 1 list các Object
        /// <summary>
        /// Hàm để lấy một đối tượng sau khi chạy lệnh SetStoreNameAndParams để điền các tham số đầu vào và tên store cần thực thi và có giá trị trả ra
        /// BSS.Exec dbm = new BSS.Exec();
        /// T obj = null;
        /// string err = obj.SetStoreNameAndParams("dbo.sp_LogActivity_SelectOne", new { ID = 1 }).ExecStore(out obj);
        /// </summary>
        /// <param name="oneItem">Đối tượng chứa giá trị sau khi chạy</param>
        /// <returns>Giá trị trả về rỗng là thành công, còn khác rỗng là lỗi</returns>
        public string GetOne<T>(out T oneItem) where T : class
        {
            return GetOne(this, ConnectionString, storeName, htParams, out oneItem);
        }
        /// <summary>
        /// Hàm để lấy danh sách đối tượng sau khi chạy lệnh SetStoreNameAndParams để điền các tham số đầu vào và tên store cần thực thi và có giá trị trả ra
        /// BSS.Exec dbm = new BSS.Exec();
        /// List lt = null;
        /// string err = obj.SetStoreNameAndParams("dbo.sp_LogActivity_SelectOne", new { ID = 1 }).ExecStore(out lt);
        /// </summary>
        /// <param name="list">List chứa giá trị sau khi chạy</param>
        /// <returns>Giá trị trả về rỗng là thành công, còn khác rỗng là lỗi</returns>
        public string GetList<T>(out List<T> list) where T : class
        {
            return GetList(this, storeName, htParams, out list);
        }

        static public string GetOne<T>(string storeName, out T oneItem) where T : class
        {
            return GetOne(ConnectionString, storeName, new { }, out oneItem);
        }
        static public string GetOne<T>(string strConnect, string storeName, out T oneItem) where T : class
        {
            return GetOne(strConnect, storeName, new { }, out oneItem);
        }
        static public string GetOne<T, TL>(string storeName, T parameters, out TL oneItem) where T : class where TL : class
        {
            return GetOne(ConnectionString, storeName, parameters, out oneItem);
        }
        /// <summary>
        /// Hàm tĩnh để lấy 1 đối tượng
        /// LogActivity objLogActivity = null;
        /// DataTable dt = null;
        /// string err = Exec.GetOne("dbo.sp_LogActivity_SelectOne", new { ID = 5 }, out objLogActivity);
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng chứa biến</typeparam>
        /// <typeparam name="TL">Kiểu đối tượng cần lấy</typeparam>
        /// <param name="strConnect"></param>
        /// <param name="storeName">Tên store</param>
        /// <param name="parameters">Đối tượng chứa biến chuyền vào</param>
        /// <param name="oneItem">Đối tượng trả ra</param>
        /// <returns>Giá trị trả về rỗng là thành công, còn khác rỗng là lỗi</returns>
        static public string GetOne<T, TL>(string strConnect, string storeName, T parameters, out TL oneItem) where T : class where TL : class
        {
            Hashtable htParams = new Hashtable();
            AddParams(strConnect, ref htParams, parameters);

            return GetOne(null, strConnect, storeName, htParams, out oneItem);
        }

        static public string GetList<T>(string storeName, out List<T> list) where T : class
        {
            return GetList(ConnectionString, storeName, new { }, out list);
        }
        static public string GetList<T>(string strConnect, string storeName, out List<T> list) where T : class
        {
            return GetList(strConnect, storeName, new { }, out list);
        }
        static public string GetList<T, TL>(string storeName, T parameters, out List<TL> list) where T : class where TL : class
        {
            return GetList(ConnectionString, storeName, parameters, out list);
        }
        /// <summary> 
        /// Hàm tĩnh để lấy danh sách đối tượng
        /// List clsLogActivity objListLogActivity = null;
        /// DataTable dt = null;
        /// string err = Exec.GetList("dbo.sp_LogActivity_SelectOne", new { ID = 5 }, out objListLogActivity); 
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng chứa biến</typeparam>
        /// <typeparam name="TL">Kiểu đối tượng cần lấy</typeparam>
        /// <param name="strConnect"></param>
        /// <param name="storeName">Tên store</param>
        /// <param name="parameters">Đối tượng chứa biến chuyền vào</param>
        /// <param name="list">Đối tượng trả ra</param>
        /// <returns>Giá trị trả về rỗng là thành công, còn khác rỗng là lỗi</returns>
        static public string GetList<T, TL>(string strConnect, string storeName, T parameters, out List<TL> list) where T : class where TL : class
        {
            Hashtable htParams = new Hashtable();
            AddParams(strConnect, ref htParams, parameters);

            return GetList(null, strConnect, storeName, htParams, out list);
        }

        static private string GetOne<T>(Exec dbm, string strConnect, string storeName, Hashtable htParams, out T oneItem)
        {
            oneItem = default(T);

            List<T> list = null;
            string msg = GetList(dbm, strConnect, storeName, htParams, out list);
            if (msg.Length > 0) return msg;

            if (list == null) return "Error: list is null @Exec.GetOne";
            if (list.Count == 0) return "";

            oneItem = list[0];

            return msg;
        }
        static private string GetList<T>(Exec dbm, string storeName, Hashtable htParams, out List<T> list)
        {
            return GetList(dbm, ConnectionString, storeName, htParams, out list);
        }
        static private string GetList<T>(string strConnect, string storeName, Hashtable htParams, out List<T> list)
        {
            return GetList(null, strConnect, storeName, htParams, out list);
        }
        static private string GetList<T>(Exec dbm, string strConnect, string storeName, Hashtable htParams, out List<T> list)
        {
            string msg = "";
            list = null;

            DataTable dt = null;
            if (dbm == null) msg = ExecStore(strConnect, storeName, htParams, out dt);
            else msg = dbm.ExecStore(out dt);

            if (msg.Length > 0) return msg;

            msg = Convertor.DataTableToList(dt, out list);

            return msg;
        }

        #endregion

        private string queryString = "";

        public string ExecQueryString()
        {
            return ExecQueryString(conn as SqlConnection, transac as SqlTransaction, queryString);
        }

        public Exec SetQueryString(string queryString)
        {
            this.queryString = queryString;
            return this;
        }

        /// <summary>
        /// Thực thi bằng câu lênh trực tiếp
        /// </summary>       
        /// <param name="_QueryString"></param>
        /// <returns></returns>
        static public string ExecQueryString(string _QueryString)
        {
            return ExecQueryString(ConnectionString, _QueryString);
        }

        static public string ExecQueryString(string strConnect, string _QueryString)
        {
            var conn = GetDbConnection(strConnect);

            return ExecQueryString(conn, null, _QueryString);
        }
        /// <summary>
        /// Thực thi bằng câu lênh trực tiếp
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transac"></param>
        /// <param name="_QueryString"></param>
        /// <returns></returns>
        static private string ExecQueryString(DbConnection conn, DbTransaction transac, string _QueryString)
        {
            if (conn == null) return "Error: SqlConn is null @DBM.ExecQueryString";
            if (string.IsNullOrEmpty(_QueryString)) return "Error: QueryString is null @DBM.ExecQueryString";
            string msg = "";

            DbCommand cmdToExecute = new SqlCommand(_QueryString, conn as SqlConnection);

            if (transac != null) cmdToExecute.Transaction = transac;
            cmdToExecute.CommandType = CommandType.Text;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                cmdToExecute.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                msg = "SQL query error : " + ex.ToString();
            }
            finally
            {
                cmdToExecute?.Dispose();
                if (transac == null) if (conn.State != ConnectionState.Closed) conn.Close();
            }

            return msg;
        }


        static public string ExecQueryStringOne<T>(string _QueryString, out T oneItem) where T : class
        {
            return ExecQueryStringOne(ConnectionString, _QueryString, new { }, out oneItem);
        }
        static public string ExecQueryStringOne<T>(string strConnect, string _QueryString, out T oneItem) where T : class
        {
            return ExecQueryStringOne(strConnect, _QueryString, new { }, out oneItem);
        }
        static public string ExecQueryStringOne<T, TL>(string _QueryString, T parameters, out TL oneItem) where T : class where TL : class
        {
            return ExecQueryStringOne(ConnectionString, _QueryString, parameters, out oneItem);
        }
        /// <summary>
        /// Hàm tĩnh để lấy 1 đối tượng
        /// LogActivity objLogActivity = null;
        /// DataTable dt = null;
        /// string err = Exec.ExecQueryStringOne("dbo.sp_LogActivity_SelectOne", new { ID = 5 }, out objLogActivity);
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng chứa biến</typeparam>
        /// <typeparam name="TL">Kiểu đối tượng cần lấy</typeparam>
        /// <param name="strConnect"></param>
        /// <param name="storeName">Tên store</param>
        /// <param name="parameters">Đối tượng chứa biến chuyền vào</param>
        /// <param name="oneItem">Đối tượng trả ra</param>
        /// <returns>Giá trị trả về rỗng là thành công, còn khác rỗng là lỗi</returns>
        static public string ExecQueryStringOne<T, TL>(string strConnect, string _QueryString, T parameters, out TL oneItem) where T : class where TL : class
        {
            Hashtable htParams = new Hashtable();
            AddParams(strConnect, ref htParams, parameters);

            return ExecQueryStringOne(null, strConnect, _QueryString, htParams, out oneItem);
        }

        static private string ExecQueryStringOne<T>(Exec dbm, string strConnect, string _QueryString, Hashtable htParams, out T oneItem)
        {
            oneItem = default(T);

            List<T> list = null;
            string msg = GetExecQuery(dbm, strConnect, _QueryString, htParams, out list);
            if (msg.Length > 0) return msg;

            if (list == null) return "Error: list is null @Exec.GetOne";
            if (list.Count == 0) return "";

            oneItem = list[0];

            return msg;
        }

        static private string GetExecQuery<T>(Exec dbm, string strConnect, string _QueryString, Hashtable htParams, out List<T> list)
        {
            string msg = "";
            list = null;

            DataTable dt = null;
            if (dbm == null) msg = GetExecQuery(strConnect, _QueryString, htParams, out dt);
            else msg = dbm.GetExecQuery(out dt);

            if (msg.Length > 0) return msg;

            msg = Convertor.DataTableToList(dt, out list);

            return msg;
        }

        static public string GetExecQuery(string strConnect, string _QueryString, Hashtable htParams, out DataTable dt)
        {
            dt = null;

            List<object> outValue = null;
            string msg = GetExecQuery(strConnect, _QueryString, htParams, QueryTypeGetDataTable, null, out outValue);
            if (msg.Length > 0) return msg;

            if (outValue == null) return "Error: outValue is null @ExecQueryString";
            if (outValue.Count == 0) return "Error: outValue has no value @ExecQueryString";

            dt = outValue[0] as DataTable; // cast về DataTable. Trả về null nếu không cast được

            return msg;
        }

        public string GetExecQuery(out DataTable dt)
        {
            dt = null;

            List<object> outValues = null;
            string msg = "";
            msg = ExecStore(conn, transac, _QueryString, htParams, QueryTypeGetDataTable, false, null, null, out outValues);
            if (msg.Length > 0) return msg;

            if (outValues.Count == 0) return "Error: No out value @ExecQueryString with spName " + storeName;

            dt = outValues[0] as DataTable; // cast về DataTable. Trả về null nếu không cast được

            return "";
        }

        static private string GetExecQuery(string strConnect, string _QueryString, Hashtable htParams, int queryType, List<string> outVarNames, out List<object> outValues)
        {
            outValues = null;
            if (string.IsNullOrWhiteSpace(strConnect)) return "Error: Connection string is null or empty @ExecQueryString";

            DbConnection conn = GetDbConnection(strConnect);
            var msg = GetExecQuery(conn, null, _QueryString, htParams, queryType, false, null, outVarNames, out outValues);
            conn?.Dispose();

            return msg;
        }

        static private string GetExecQuery(DbConnection conn, DbTransaction transac, string _QueryString, Hashtable htParams, int queryType, bool isAsync, HandleCallbackFunc HandleCallback, List<string> outVarNames, out List<object> outValues)
        {
            outValues = null;

            if (conn == null) return "Error: conn is null @ExecQueryString";

            string msg = "";

            DbCommand cmdToExecute = new SqlCommand(_QueryString, conn as SqlConnection);

            if (transac != null) cmdToExecute.Transaction = transac;

            cmdToExecute.CommandType = CommandType.Text;

            string sParam = "";
            if (htParams != null)
            {
                foreach (DictionaryEntry p in htParams)
                {
                    string value = "";
                    DbParameter sp = null;

                    if (p.Value == null)
                    {
                        sp = new SqlParameter(p.Key.ToString(), DBNull.Value);
                        value = "null";
                    }
                    else if (p.Value.GetType().IsArray)
                    {
                        sp = new SqlParameter(p.Key.ToString(), SqlDbType.Binary);
                        sp.Value = p.Value;
                        value = "Binary Type";
                    }
                    else
                    {
                        sp = new SqlParameter(p.Key.ToString(), p.Value);
                        if (p.Value.GetType() == typeof(DateTime))
                        {
                            value = "'" + DateTime.Parse(p.Value.ToString()).ToString("yyyy-MM-dd hh:mm:ss") + "'";
                        }
                        else if (p.Value.ToString() == "") value = "''";
                        else value = p.Value.ToString();
                    }

                    cmdToExecute.Parameters.Add(sp);

                    sParam += p.Key.ToString() + " = " + value + ",";
                }
            }

            List<DbParameter> spOuts = null;
            bool isOutRowsAffected = false;
            if (outVarNames != null)
            {
                if (outVarNames.Count == 1 && outVarNames[0].ToLower() == "rowsaffected") isOutRowsAffected = true;
                else
                {
                    spOuts = new List<DbParameter>();
                    foreach (string outVarName in outVarNames)
                    {
                        DbParameter spOut = new SqlParameter(outVarName, DBNull.Value);

                        spOut.Direction = ParameterDirection.Output;
                        spOut.Size = 1000;
                        cmdToExecute.Parameters.Add(spOut);

                        spOuts.Add(spOut);
                    }
                }
            }

            int rowsAffected = -1;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                switch (queryType)
                {
                    case QueryTypeNoOutput:
                        if (isAsync)
                        {
                            if (HandleCallback == null) HandleCallback = HandleCallback_Default;
                            AsyncCallback callback = new AsyncCallback(HandleCallback);

                            ((SqlCommand)cmdToExecute).BeginExecuteNonQuery(callback, cmdToExecute);
                        }
                        else rowsAffected = cmdToExecute.ExecuteNonQuery();
                        break;
                    case QueryTypeWithOutputs:
                        if (isOutRowsAffected) rowsAffected = cmdToExecute.ExecuteNonQuery();
                        else
                        {
                            outValues = new List<object>();

                            if (outVarNames != null)
                            {
                                rowsAffected = cmdToExecute.ExecuteNonQuery();

                                foreach (var spOut in spOuts) outValues.Add(spOut.Value);
                            }
                            else outValues.Add(cmdToExecute.ExecuteScalar());
                        }
                        break;
                    case QueryTypeGetDataTable:
                        using (var ds = new DataSet())
                        {
                            DbDataAdapter dad = new SqlDataAdapter((SqlCommand)cmdToExecute);

                            rowsAffected = dad.Fill(ds);
                            outValues = new List<object>();
                            outValues.Add(ds.Tables[0]);
                            dad.Dispose();
                        }
                        break;
                    default: msg = "Error: QueryType is unknown @ExecQueryString"; break;
                }
            }
            catch (Exception ex)
            {
                msg = "StoreName EXEC " + _QueryString + " " + sParam + " error: " + ex.ToString();
                if (transac == null && conn != null && conn.State != ConnectionState.Closed) conn.Close();
            }
            finally
            {
                if (!isAsync) // Nếu không phải async thì close SQL connection. Nếu là async thì connection sẽ được close ở hàm callback async.      
                    if (transac == null && conn != null && conn.State != ConnectionState.Closed) conn.Close();
            }
            cmdToExecute?.Dispose();

            if (isOutRowsAffected)
            {
                outValues = new List<object>();
                outValues.Add(rowsAffected);
            }

            return msg;
        }

        static public string GetExecQuery(string storeName)
        {
            List<object> outValue = null;
            return GetExecQuery(ConnectionString, storeName, new { }, QueryTypeNoOutput, null, out outValue);
        }

        static private string GetExecQuery<T>(string strConnect, string storeName, T parameters, int queryType, List<string> outVarNames, out List<object> outValues) where T : class
        {
            Hashtable htParams = new Hashtable();
            AddParams(strConnect, ref htParams, parameters);

            return GetExecQuery(strConnect, storeName, htParams, queryType, outVarNames, out outValues);

        }

        /* MultipleResult */
        static public string MultipleResult<T, TL, TLL>(string storeName, T parameters, out TL oneItem, out List<TLL> twiceItem) where T : class where TL : class where TLL : class
        {
            return MultipleResult(ConnectionString, storeName, parameters, out oneItem, out twiceItem);
        }

        static public string MultipleResult<T, TL, TLL>(string strConnect, string storeName, T parameters, out TL oneItem, out List<TLL> twiceItem) where T : class where TL : class where TLL : class
        {
            Hashtable htParams = new Hashtable();
            AddParams(strConnect, ref htParams, parameters);

            return MultipleResult(null, strConnect, storeName, htParams, out oneItem, out twiceItem);
        }

        static private string MultipleResult<T, TLL>(Exec dbm, string strConnect, string storeName, Hashtable htParams, out T oneItem, out List<TLL> twiceItem)
        {
            oneItem = default(T);
            twiceItem = default(List<TLL>);

            List<T> list = null;
            List<TLL> listTLL = null;

            string msg = GetMultipleResult(dbm, strConnect, storeName, htParams, out list, out listTLL);
            if (msg.Length > 0) return msg;

            if (list == null) return "Error: list is null @Exec.MultipleResult";
            if (list.Count == 0) return "";

            if (listTLL == null) return "Error: listTLL is null @Exec.MultipleResult";
            if (listTLL.Count == 0) return "";

            oneItem = list[0];
            twiceItem = listTLL;

            return msg;
        }

        static private string GetMultipleResult<T, TLL>(Exec dbm, string strConnect, string storeName, Hashtable htParams, out List<T> list, out List<TLL> listTLL)
        {
            string msg = "";

            list = null;
            listTLL = null;

            DataTable dt = null;
            DataTable twicedt = null;

            if (dbm == null) msg = ExecStoreMultipleResult(strConnect, storeName, htParams, out dt, out twicedt);
            else msg = dbm.ExecStoreMultipleResult(out dt, out twicedt);

            if (msg.Length > 0) return msg;

            msg = Convertor.DataTableToList(dt, out list);
            msg = Convertor.DataTableToList(twicedt, out listTLL);

            return msg;
        }

        public string ExecStoreMultipleResult(out DataTable dt, out DataTable dtTwice)
        {
            dt = null;
            dtTwice = null;

            List<object> outValues = null;
            List<object> outValueTwice = null;

            string msg = "";
            msg = ExecStoreMultipleResult(conn, transac, storeName, htParams, QueryTypeGetDataTable, false, null, null, out outValues, out outValueTwice);
            if (msg.Length > 0) return msg;

            if (outValues.Count == 0) return "Error: No out value @ExecStore with spName " + storeName;
            if (outValueTwice.Count == 0) return "Error: No out value @ExecStore with spName " + storeName;

            dt = outValues[0] as DataTable; // cast về DataTable. Trả về null nếu không cast được
            dtTwice = outValueTwice[0] as DataTable; // cast về DataTable. Trả về null nếu không cast được

            return "";
        }

        static public string ExecStoreMultipleResult(string strConnect, string storeName, Hashtable htParams, out DataTable dt, out DataTable twicedt)
        {
            dt = null;
            twicedt = null;

            string msg = ExecStoreMultipleResult(strConnect, storeName, htParams, QueryTypeGetDataTable, null, out List<object> outValue, out List<object> outValueTwice);
            if (msg.Length > 0) return msg;

            if (outValue == null) return "Error: outValue is null @ExecStoreMultipleResult";
            if (outValue.Count == 0) return "Error: outValue has no value @ExecStoreMultipleResult";

            if (outValueTwice == null) return "Error: outValue is null @ExecStoreMultipleResult";
            if (outValueTwice.Count == 0) return "Error: outValue has no value @ExecStoreMultipleResult";

            dt = outValue[0] as DataTable; // cast về DataTable. Trả về null nếu không cast được
            twicedt = outValueTwice[0] as DataTable; // cast về DataTable. Trả về null nếu không cast được

            return msg;
        }

        static private string ExecStoreMultipleResult(string strConnect, string storeName, Hashtable htParams, int queryType, List<string> outVarNames, out List<object> outValues, out List<object> outValueTwice)
        {
            outValues = null;
            outValueTwice = null;

            if (string.IsNullOrWhiteSpace(strConnect)) return "Error: Connection string is null or empty @ExecStore";

            DbConnection conn = GetDbConnection(strConnect);
            var msg = ExecStoreMultipleResult(conn, null, storeName, htParams, queryType, false, null, outVarNames, out outValues, out outValueTwice);
            conn?.Dispose();

            return msg;
        }

        static private string ExecStoreMultipleResult(DbConnection conn, DbTransaction transac, string storeName, Hashtable htParams, int queryType, bool isAsync, HandleCallbackFunc HandleCallback, List<string> outVarNames, out List<object> outValues, out List<object> outValueTwice)
        {
            outValues = null;
            outValueTwice = null;

            if (conn == null) return "Error: conn is null @ExecStoreMultipleResult";

            string msg = "";

            DbCommand cmdToExecute = new SqlCommand(storeName, conn as SqlConnection);

            if (transac != null) cmdToExecute.Transaction = transac;

            cmdToExecute.CommandType = CommandType.StoredProcedure;

            string sParam = "";
            if (htParams != null)
            {
                foreach (DictionaryEntry p in htParams)
                {
                    string value = "";
                    DbParameter sp = null;

                    if (p.Value == null)
                    {
                        sp = new SqlParameter(p.Key.ToString(), DBNull.Value);
                        value = "null";
                    }
                    else if (p.Value.GetType().IsArray)
                    {
                        sp = new SqlParameter(p.Key.ToString(), SqlDbType.Binary);
                        sp.Value = p.Value;
                        value = "Binary Type";
                    }
                    else
                    {
                        sp = new SqlParameter(p.Key.ToString(), p.Value);
                        if (p.Value.GetType() == typeof(DateTime))
                        {
                            value = "'" + DateTime.Parse(p.Value.ToString()).ToString("yyyy-MM-dd hh:mm:ss") + "'";
                        }
                        else if (p.Value.ToString() == "") value = "''";
                        else value = p.Value.ToString();
                    }

                    cmdToExecute.Parameters.Add(sp);

                    sParam += p.Key.ToString() + " = " + value + ",";
                }
            }

            List<DbParameter> spOuts = null;
            bool isOutRowsAffected = false;
            if (outVarNames != null)
            {
                if (outVarNames.Count == 1 && outVarNames[0].ToLower() == "rowsaffected") isOutRowsAffected = true;
                else
                {
                    spOuts = new List<DbParameter>();
                    foreach (string outVarName in outVarNames)
                    {
                        DbParameter spOut = new SqlParameter(outVarName, DBNull.Value);

                        spOut.Direction = ParameterDirection.Output;
                        spOut.Size = 1000;
                        cmdToExecute.Parameters.Add(spOut);

                        spOuts.Add(spOut);
                    }
                }
            }

            int rowsAffected = -1;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                switch (queryType)
                {
                    case QueryTypeNoOutput:
                        if (isAsync)
                        {
                            if (HandleCallback == null) HandleCallback = HandleCallback_Default;
                            AsyncCallback callback = new AsyncCallback(HandleCallback);

                            ((SqlCommand)cmdToExecute).BeginExecuteNonQuery(callback, cmdToExecute);
                        }
                        else rowsAffected = cmdToExecute.ExecuteNonQuery();
                        break;
                    case QueryTypeWithOutputs:
                        if (isOutRowsAffected) rowsAffected = cmdToExecute.ExecuteNonQuery();
                        else
                        {
                            outValues = new List<object>();
                            outValueTwice = new List<object>();

                            if (outVarNames != null)
                            {
                                rowsAffected = cmdToExecute.ExecuteNonQuery();

                                foreach (var spOut in spOuts) outValues.Add(spOut.Value);
                            }
                            else outValues.Add(cmdToExecute.ExecuteScalar());
                        }
                        break;
                    case QueryTypeGetDataTable:
                        using (var ds = new DataSet())
                        {
                            DbDataAdapter dad = new SqlDataAdapter((SqlCommand)cmdToExecute);

                            rowsAffected = dad.Fill(ds);
                            outValues = new List<object>();
                            outValueTwice = new List<object>();

                            outValues.Add(ds.Tables[0]);
                            outValueTwice.Add(ds.Tables[1]);
                            dad.Dispose();
                        }
                        break;
                    default: msg = "Error: QueryType is unknown @ExecStoreMultipleResult"; break;
                }
            }
            catch (Exception ex)
            {
                msg = "StoreName EXEC " + storeName + " " + sParam + " error: " + ex.ToString();
                if (transac == null && conn != null && conn.State != ConnectionState.Closed) conn.Close();
            }
            finally
            {
                if (!isAsync) // Nếu không phải async thì close SQL connection. Nếu là async thì connection sẽ được close ở hàm callback async.      
                    if (transac == null && conn != null && conn.State != ConnectionState.Closed) conn.Close();
            }
            cmdToExecute?.Dispose();

            if (isOutRowsAffected)
            {
                outValues = new List<object>();
                outValues.Add(rowsAffected);
            }

            return msg;
        }
    }
}
