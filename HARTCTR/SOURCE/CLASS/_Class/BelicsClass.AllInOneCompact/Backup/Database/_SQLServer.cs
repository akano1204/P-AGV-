using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace BelicsClass.Database
{
    /// <summary>
    /// データベースクラス(SQLServer)
    /// </summary>
    public class BL_SQLServer : BL_Database
    {
        #region コンストラクタ

        /// <summary>
        /// データベースクラス
        /// </summary>
        public BL_SQLServer()
            : base()
        {
            Command = new SqlCommand();
            Connection = new SqlConnection();
            Adapter = new SqlDataAdapter();
        }
        /// <summary>
        /// データベースクラス
        /// </summary>
        /// <param name="cmdTimeout">コマンドタイムアウト値の設定。</param>
        public BL_SQLServer(int cmdTimeout)
            : this()
        {
            Command.CommandTimeout = cmdTimeout;
        }

        #endregion

        #region メソッド

        #region 接続

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCatalog"></param>
        /// <param name="dataSource"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override string GetConnectionString(string initialCatalog, string dataSource, string userID, string password)
        {
            dateQuoteFrom = "#";
            dateQuoteTo = "'";

            if (userID.Trim() == "")
            {
                //Windows認証でSQLSERVERへ接続
                return "Initial Catalog=" + initialCatalog + ";" +
                "Data Source=" + dataSource + ";" +
                "Integrated Security=SSPI;" +
                "Connect Timeout=" + Connect_timeout.ToString();
            }


            //SQL Server認証でSQLSERVERへ接続
            return "Data Source=" + dataSource + ";" +
                    "Persist Security Info=True;" +
                    "User Id=" + userID + ";" +
                    "Password=" + password + ";" +
                    "Initial Catalog=" + initialCatalog + ";" +
                    "Integrated Security=;" +
                    "Connect Timeout=" + Connect_timeout.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override DbCommand GetDbCommand(string cmdText, DbConnection connection)
        {
            return new SqlCommand(cmdText, (SqlConnection)connection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectCommand"></param>
        /// <returns></returns>
        public override DbDataAdapter GetDbDataAdapter(DbCommand selectCommand)
        {
            return new SqlDataAdapter((SqlCommand)selectCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public override DbCommandBuilder GetDbCommandBuilder(DbDataAdapter adapter)
        {
            return new SqlCommandBuilder((SqlDataAdapter)adapter);
        }

        #endregion

        #region テーブル名取得

        /// <summary>
        /// テーブル名取得
        /// </summary>
        public override string[] GetTables()
        {

            if (!Connected)
            {
                errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");
                return null;
            }

            List<string> list = new List<string>();
            System.Data.DataTable db = new System.Data.DataTable();

            try
            {
                db = ((SqlConnection)Connection).GetSchema("Tables");
            }
            catch (SqlException ex)
            {
                errors(ex.Number, ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                errors(-1, ex.Message);
                return null;
            }

            foreach (DataRow dr in db.Rows)
            {
                if (!list.Contains(dr[2].ToString()))
                {
                    list.Add(dr[2].ToString());
                }
            }

            return list.ToArray();
        }

        #endregion

        #region トランザクション分離レベル設定

        /// <summary>
        /// トランザクション分離レベルを設定します。
        /// </summary>
        /// <param name="level">設定する分離レベル。</param>
        /// <returns></returns>
        public override bool SetIsolationLevel(IsolationLevel level)
        {
            string sql;

            switch (level)
            {
                case IsolationLevel.Serializable:
                    sql = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE";
                    break;

                case IsolationLevel.Repeatable_Read:
                    sql = "SET TRANSACTION ISOLATION LEVEL REPEATABLE READ";
                    break;

                case IsolationLevel.Read_Committed:
                    sql = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
                    break;

                case IsolationLevel.Read_Uncommitted:
                    sql = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED";
                    break;

                default:
                    sql = "";
                    break;
            }

            return this.Execute(sql);
        }

        #endregion

        #region トランザクション開始

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        public override bool BeginTransaction()
        {
            return this.Execute("BEGIN TRANSACTION");
        }

        #endregion

        #region トランザクションコミット

        /// <summary>
        /// トランザクションをコミットします。
        /// </summary>
        public override bool CommitTransaction()
        {
            return this.Execute("COMMIT TRANSACTION");
        }

        #endregion

        #region トランザクションロールバック

        /// <summary>
        /// トランザクションをロールバックします。
        /// </summary>
        public override bool RollBackTransaction()
        {
            return this.Execute("ROLLBACK TRANSACTION");
        }

        #endregion



        #region ストアドプロシージャ実行

        /// <summary>
        /// ストアドプロシージャを実行します。
        /// </summary>
        /// <param name="procName">実行するストアドプロシージャ名が格納されている文字列。</param>
        /// <param name="param">パラメータ値。</param>
        /// <param name="paramCount">パラメータ数。</param>
        /// <param name="procReturn">ストアドプロシージャの戻り値。</param>
        /// <returns></returns>
        public bool ProcExecute(string procName, ref SqlParameter[] param, int paramCount, out int procReturn)
        {
            lock (this)
            {
                SqlParameter return_param;

                errors();

                procReturn = 0;

                if (!Connected)
                {
                    errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

                    return false;
                }

                if (paramCount > 0 && paramCount > param.Length)
                {
                    errors((int)ExNumber.ProcedureParameterException, "パラメータが不正です。");

                    return false;
                }

                Command.Parameters.Clear();

                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = procName;

                return_param = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
                return_param.Direction = ParameterDirection.ReturnValue;
                return_param.Value = 0;

                Command.Parameters.Add(return_param);

                for (int count = 0; count < paramCount; count++)
                {
                    Command.Parameters.Add(param[count]);
                }

                try
                {
                    Command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    errors(ex.Number, ex.Message);
                }
                catch (Exception ex)
                {
                    errors((int)ExNumber.UnexpectedException, ex.Message);
                }
                finally
                {
                    if (Command.Parameters.Count > 0)
                    {
                        try
                        {
                            procReturn = (int)Command.Parameters[0].Value;
                        }
                        catch
                        {
                        }

                        for (int count = 1; count < Command.Parameters.Count; count++)
                        {
                            param[count - 1].Value = Command.Parameters[count].Value;
                        }
                    }
                }
            }

            return Error_code == 0 ? true : false;
        }
        /// <summary>
        /// ストアドプロシージャを実行します。
        /// </summary>
        /// <param name="procName">実行するストアドプロシージャ名が格納されている文字列。</param>
        /// <param name="procReturn">ストアドプロシージャの戻り値。</param>
        /// <returns></returns>
        public bool ProcExecute(string procName, out int procReturn)
        {
            SqlParameter[] param = new SqlParameter[1];

            return this.ProcExecute(procName, ref param, 0, out procReturn);
        }
        /// <summary>
        /// ストアドプロシージャを実行しデータセット内に新規データテーブルを作成します。
        /// </summary>
        /// <param name="procName">実行するストアドプロシージャ名が格納されている文字列。</param>
        /// <param name="param">パラメータ値。</param>
        /// <param name="paramCount">パラメータ数。</param>
        /// <param name="procReturn">ストアドプロシージャの戻り値。</param>
        /// <param name="dataTableName">新規に作成するデータテーブル名が格納されている文字列。</param>
        /// <returns></returns>
        public bool ProcExecute(string procName, ref SqlParameter[] param, int paramCount, out int procReturn, string dataTableName)
        {
            lock (this)
            {
                SqlParameter return_param;

                errors();

                procReturn = 0;

                if (!Connected)
                {
                    errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

                    return false;
                }

                if (paramCount > 0 && paramCount > param.Length)
                {
                    errors((int)ExNumber.ProcedureParameterException, "パラメータが不正です。");

                    return false;
                }

                this.ClearDataTable(dataTableName);

                errors();

                Command.Parameters.Clear();

                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = procName;

                return_param = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
                return_param.Direction = ParameterDirection.ReturnValue;
                return_param.Value = 0;

                Command.Parameters.Add(return_param);

                for (int count = 0; count < paramCount; count++)
                {
                    Command.Parameters.Add(param[count]);
                }

                Adapter.SelectCommand = Command;

                try
                {
                    Adapter.Fill(Ds, dataTableName);
                }
                catch (SqlException ex)
                {
                    errors(ex.Number, ex.Message);
                }
                catch (SystemException ex)
                {
                    errors((int)ExNumber.SystemException, ex.Message);
                }
                catch (Exception ex)
                {
                    errors((int)ExNumber.UnexpectedException, ex.Message);
                }
                finally
                {
                    if (Command.Parameters.Count > 0)
                    {
                        try
                        {
                            procReturn = (int)Command.Parameters[0].Value;
                        }
                        catch
                        {
                        }

                        for (int count = 1; count < Command.Parameters.Count; count++)
                        {
                            param[count - 1].Value = Command.Parameters[count].Value;
                        }
                    }
                }
            }

            return Error_code == 0 ? true : false;
        }
        /// <summary>
        /// ストアドプロシージャを実行しデータセット内に新規データテーブルを作成します。
        /// </summary>
        /// <param name="procName">実行するストアドプロシージャ名が格納されている文字列。</param>
        /// <param name="procReturn">ストアドプロシージャの戻り値。</param>
        /// <param name="dataTableName">新規に作成するデータテーブル名が格納されている文字列。</param>
        /// <returns></returns>
        public bool ProcExecute(string procName, out int procReturn, string dataTableName)
        {
            SqlParameter[] param = new SqlParameter[1];

            return this.ProcExecute(procName, ref param, 0, out procReturn, dataTableName);
        }

        #endregion

        #region テーブルまたはビューの存在を確認

        /// <summary>
        /// テーブルまたはビューの存在を確認します。
        /// </summary>
        /// <param name="tableName">存在を確認するテーブル名が格納されている文字列。</param>
        /// <returns></returns>
        public int CheckTable(string tableName)
        {
            const string data_table_name = "BL_CheckTableSQLSERVER";

            SqlParameter[] param = new SqlParameter[1];
            DataTable dt;
            int proc_return;
            int status = 0;
            string sql;

            errors();

            if (!Connected)
            {
                errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

                return -1;
            }

            sql = "sp_tables";

            param[0] = new SqlParameter("@table_type", SqlDbType.NVarChar, 20);
            param[0].Value = "['TABLE', 'VIEW']";

            if (this.ProcExecute(sql, ref param, 1, out proc_return, data_table_name))
            {
                dt = this[data_table_name];

                for (int count = 0; count < dt.Rows.Count; count++)
                {
                    if (tableName == dt.Rows[count]["TABLE_NAME"].ToString())
                    {
                        status = 1;

                        break;
                    }
                }

                this.ClearDataTable(data_table_name);
            }
            else
            {
                status = -1;
            }

            return status;
        }

        #endregion

        #region ストアドプロシージャの存在を確認

        /// <summary>
        /// ストアドプロシージャの存在を確認します。
        /// </summary>
        /// <param name="procName">存在を確認するストアドプロシージャ名が格納されている文字列。</param>
        /// <returns></returns>
        public int CheckProcedure(string procName)
        {
            const string data_table_name = "BL_CheckProcedureSQLSERVER";

            SqlParameter[] param = new SqlParameter[1];
            DataTable dt;
            int proc_return;
            int status = 0;
            string sql;

            errors();

            if (!Connected)
            {
                errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

                return -1;
            }

            sql = "sp_stored_procedures";

            param[0] = new SqlParameter("@sp_name", SqlDbType.NVarChar, 60);
            param[0].Value = procName;

            if (this.ProcExecute(sql, ref param, 1, out proc_return, data_table_name))
            {
                dt = this[data_table_name];

                if (dt.Rows.Count > 0)
                {
                    status = 1;
                }

                this.ClearDataTable(data_table_name);
            }
            else
            {
                status = -1;
            }

            return status;
        }

        #endregion

        #region ストアドプロシージャ定義ファイルからソースコードを取得します

        /// <summary>
        /// ストアドプロシージャ定義ファイルからソースコードを取得します
        /// </summary>
        /// <param name="stream">ストアドプロシージャ定義ファイルのストリーム</param>
        /// <returns></returns>
        public static StringBuilder get_sql_text(Stream stream)
        {
            int pos;
            string read_data, data;
            StringBuilder sql = new StringBuilder();
            StreamReader reader;

            if (stream == null) return sql;

            reader = new StreamReader(stream, Encoding.Default);

            while (true)
            {
                read_data = reader.ReadLine();

                if (read_data == null) break;

                pos = read_data.IndexOf("--");

                if (pos >= 0)
                {
                    data = read_data.Substring(0, pos).Trim();
                }
                else
                {
                    data = read_data.Trim();
                }

                if (data.Length > 0)
                {
                    sql.Append(data + " ");
                }
            }

            return sql;
        }

        #endregion

        #endregion

    }
}
