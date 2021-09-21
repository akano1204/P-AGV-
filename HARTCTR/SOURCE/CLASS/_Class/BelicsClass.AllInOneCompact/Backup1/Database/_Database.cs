using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace BelicsClass.Database
{
    /// <summary>
    /// 各種データベースクラスの基本クラスです
    /// </summary>
    public class BL_Database
    {
        #region 列挙体
        /// <summary>
        /// エラー№
        /// </summary>
        public enum ExNumber
        {
            /// <summary>
            /// 
            /// </summary>
            NoError = 0,
            /// <summary>
            /// 
            /// </summary>
            UnexpectedException = -1,
            /// <summary>
            /// 
            /// </summary>
            TableNotExistException = -2,
            /// <summary>
            /// 
            /// </summary>
            RecordNotExistException = -3,
            /// <summary>
            /// 
            /// </summary>
            RecordDupException = -4,
            /// <summary>
            /// 
            /// </summary>
            NotConnectedException = -5,
            /// <summary>
            /// 
            /// </summary>
            ProcedureParameterException = -6,
            /// <summary>
            /// 
            /// </summary>
            ArgumentException = -100,
            /// <summary>
            /// 
            /// </summary>
            InvalidOperationException = -101,
            /// <summary>
            /// 
            /// </summary>
            SqlException = -102,
            /// <summary>
            /// 
            /// </summary>
            SystemException = -103,
        }

        /// <summary>
        /// トランザクション分離レベル
        /// </summary>
        public enum IsolationLevel
        {
            /// <summary>
            /// 
            /// </summary>
            Serializable = 1,
            /// <summary>
            /// 
            /// </summary>
            Repeatable_Read = 2,
            /// <summary>
            /// 
            /// </summary>
            Read_Committed = 3,
            /// <summary>
            /// 
            /// </summary>
            Read_Uncommitted = 4,
        }
        #endregion

        #region フィールド

        /// <summary></summary>
        protected DbCommand Command;
        /// <summary></summary>
        protected DbConnection Connection;
        /// <summary></summary>
        protected DbDataAdapter Adapter;
        /// <summary></summary>
        protected DataSet Ds;
        /// <summary></summary>
        protected int Error_code;
        /// <summary></summary>
        protected string Error_message;
        /// <summary></summary>
        protected bool Connected = false;
        /// <summary></summary>
        protected int Connect_timeout = 10;
        /// <summary></summary>
        protected int Command_timeout = 200;

        /// <summary></summary>
        protected string dateQuoteFrom = "";
        /// <summary></summary>
        protected string dateQuoteTo = "";
        /// <summary></summary>
        protected bool isHeader = false;
        
        /// <summary>Excelファイル名</summary>
        protected string FileName = "";

        #endregion

		#region プロパティ

		/// <summary></summary>
		public string DateQuoteFrom { get { return dateQuoteFrom; } set { dateQuoteFrom = value; } }

		/// <summary></summary>
		public string DateQuoteTo { get { return dateQuoteTo; } set { dateQuoteTo = value; } }		

		#endregion

		#region コンストラクタ

		/// <summary>
        /// データベースクラス
        /// </summary>
        public BL_Database()
        {
            errors();

            this.Disconnect();

            Connected = false;
        }
        /// <summary>
        /// データベースクラス
        /// </summary>
        /// <param name="cmdTimeout">コマンドタイムアウト値の設定。</param>
        public BL_Database(int cmdTimeout)
            : this()
        {
            Command_timeout = cmdTimeout;
        }

        #endregion

        #region デストラクタ

        /// <summary>
        /// データベースクラス
        /// </summary>
        ~BL_Database()
        {
            this.Disconnect();
        }

        #endregion

        #region 接続

        /// <summary>
        /// 各種DB接続用の接続文字列を取得します。
        /// 派生クラスで処理を実装する必要があります。
        /// </summary>
        /// <param name="initialCatalog"></param>
        /// <param name="dataSource"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual string GetConnectionString(string initialCatalog, string dataSource, string userID, string password)
        {
            throw new NotSupportedException("派生クラスで処理を実装してください。");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCatalog"></param>
        /// <param name="dataSource"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="connectTimeout"></param>
        /// <returns></returns>
        public virtual bool Connect(string initialCatalog, string dataSource, string userID, string password, int connectTimeout)
        {
            errors();

            if (Connected)
            {
                return true;
            }

            try
            {
                Command.CommandTimeout = Command_timeout;

                Connection.ConnectionString = GetConnectionString(initialCatalog, dataSource, userID, password);
            }
            catch (ArgumentException ex)
            {
                errors((int)ExNumber.ArgumentException, ex.Message);
            }
            catch (Exception ex)
            {
                errors((int)ExNumber.UnexpectedException, ex.Message);
            }

            if (Error_code == 0)
            {
                try
                {
                    Connection.Open();

                    Command.Connection = Connection;
                    Command.CommandTimeout = Command_timeout;

                    Ds = new DataSet();

                    Connected = true;
                }
                catch (InvalidOperationException ex)
                {
                    errors((int)ExNumber.InvalidOperationException, ex.Message);
                }
                catch (DbException ex)
                {
                    errors(ex.ErrorCode, ex.Message);
                }
                catch (Exception ex)
                {
                    errors((int)ExNumber.UnexpectedException, ex.Message);
                }
            }

            return Error_code == 0 ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCatalog"></param>
        /// <param name="dataSource"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Connect(string initialCatalog, string dataSource, string userID, string password)
        {
            return this.Connect(initialCatalog, dataSource, userID, password, Connect_timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialCatalog"></param>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public bool Connect(string initialCatalog, string dataSource)
        {
            return this.Connect(initialCatalog, dataSource, "", "", Connect_timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public bool Connect(string dataSource)
        {
            return this.Connect("", dataSource, "", "", Connect_timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="is_header"></param>
        /// <returns></returns>
        public bool Connect(string dataSource, bool is_header)
        {
            isHeader = is_header;
            return this.Connect("", dataSource, "", "", Connect_timeout);
        }

        #endregion

        #region 切断

        /// <summary>
        /// データベースとの接続を終了します。
        /// </summary>
        /// <returns></returns>
        public virtual bool Disconnect()
        {
            errors();

            if (!Connected)
            {
                return true;
            }

            try
            {
                Connection.Close();

                Ds.Clear();
                Ds = null;

                Connected = false;
            }
            catch (DbException ex)
            {
                errors(ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                errors((int)ExNumber.UnexpectedException, ex.Message);
            }

            return Error_code == 0 ? true : false;
        }

        #endregion

        #region SQL実行

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public virtual string GetTableName(string table_name)
        {
            return table_name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetTables()
        {
            throw new NotSupportedException("派生クラスで処理を実装してください。");
        }

        /// <summary>
        /// SQLを実行します。
        /// </summary>
        /// <param name="sql">実行するSQL文が格納されている文字列。</param>
        public bool Execute(string sql)
        {
            lock (this)
            {
                errors();

                if (!Connected)
                {
                    errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

                    return false;
                }

                Command.Parameters.Clear();

                Command.CommandType = CommandType.Text;

                if (dateQuoteFrom != "" && dateQuoteTo != "")
                {
                    string s = "";
                    bool bstr = false;
                    bool bdate = false;
                    for (int pos = 0; pos < sql.Length; pos++)
                    {
                        string c0 = "";
                        if (0 < pos) c0 = sql[pos - 1].ToString();
                        string c1 = sql[pos].ToString();
                        string c2 = "";
                        if (pos < sql.Length - 1) c2 = sql[pos + 1].ToString();

                        //if (c1 == "'" && c2 == "'")
                        //{
                        //}

                        if (!bstr)
                        {
                            //if (c1 == "'" && c0 != "'")
                            if (c1 == "'")
                            {
                                bstr = true;
                            }
                            else if (!bdate)
                            {
                                if (c1 == dateQuoteFrom && c2 != dateQuoteFrom)
                                {
                                    c1 = dateQuoteTo;
                                    bdate = true;
                                }
                            }
                            else
                            {
                                if (c1 == dateQuoteFrom && c2 != dateQuoteFrom)
                                {
                                    c1 = dateQuoteTo;
                                    bdate = false;
                                }
                            }
                        }
                        else
                        {
                            //if (c0 != "'" && c1 == "'" && c2 != "'")
                            if (c1 == "'" && c2 != "'")
                            {
                                bstr = false;
                            }
                        }

                        s += c1;
                    }

                    sql = s;
                    //sql = sql.Replace(DateQuoteFrom, DateQuoteTo);
                }

                Command.CommandText = sql;

                try
                {
                    Command.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    if (ex.Message.IndexOf("重複") != -1)
                    {
                        errors((int)ExNumber.RecordDupException, ex.Message);
                    }
                    else
                    {
                        errors(ex.ErrorCode, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    errors((int)ExNumber.UnexpectedException, ex.Message);
                }
            }

            return Error_code == 0 ? true : false;
        }
        /// <summary>
        /// SQLを実行しデータセット内に新規データテーブルを作成します。
        /// </summary>
        /// <param name="sql">実行するSQL文が格納されている文字列。</param>
        /// <param name="dataTableName">新規に作成するデータテーブル名が格納されている文字列。</param>
        public bool Execute(string sql, string dataTableName)
        {
            lock (this)
            {
                errors();

                if (!Connected)
                {
                    errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

                    return false;
                }

                this.ClearDataTable(dataTableName);

                errors();

                Command.Parameters.Clear();

                Command.CommandType = CommandType.Text;

				if (dateQuoteFrom != "" && dateQuoteTo != "")
				{
					StringBuilder sb = new StringBuilder(sql);

					bool enable = true;
					for (int pos = 0; pos < sb.Length; pos++)
					{
						if (sb[pos] == '\'' && enable) enable = false;
						else if (sb[pos] == '\'' && !enable)
						{
							if (pos < sb.Length - 1 && sb[pos + 1] != '\'')
							{
								enable = true;
							}
						}

						if (enable)
						{
							if (sb[pos] == dateQuoteFrom[0]) sb[pos] = dateQuoteTo[0];
						}
					}

					sql = sb.ToString();
					//sql = sql.Replace(dateQuoteFrom, dateQuoteTo);
				}
                sql = sql.Replace(dataTableName, GetTableName(dataTableName));

                Command.CommandText = sql;

                Adapter.SelectCommand = Command;

                try
                {
                    Adapter.Fill(Ds, dataTableName);
                }
                catch (DbException ex)
                {
                    errors(ex.ErrorCode, ex.Message);
                }
                catch (SystemException ex)
                {
                    errors((int)ExNumber.SystemException, ex.Message);
                }
                catch (Exception ex)
                {
                    errors((int)ExNumber.UnexpectedException, ex.Message);
                }
            }

            return Error_code == 0 ? true : false;
        }

        #region トランザクション分離レベル設定

        /// <summary>
        /// トランザクション分離レベルを設定します。
        /// </summary>
        /// <param name="level">設定する分離レベル。</param>
        /// <returns></returns>
        public virtual bool SetIsolationLevel(IsolationLevel level)
        {
            throw new NotSupportedException("派生クラスで処理を実装してください。");
        }

        #endregion

        #region トランザクション開始

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        public virtual bool BeginTransaction()
        {
            throw new NotSupportedException("派生クラスで処理を実装してください。");
        }

        #endregion

        #region トランザクションコミット

        /// <summary>
        /// トランザクションをコミットします。
        /// </summary>
        public virtual bool CommitTransaction()
        {
            throw new NotSupportedException("派生クラスで処理を実装してください。");
        }

        #endregion

        #region トランザクションロールバック

        /// <summary>
        /// トランザクションをロールバックします。
        /// </summary>
        public virtual bool RollBackTransaction()
        {
            throw new NotSupportedException("派生クラスで処理を実装してください。");
        }

        #endregion

        #endregion

        #region データテーブル削除

        /// <summary>
        /// データセット内の指定したデータテーブルを削除します。
        /// </summary>
        /// <param name="dataTableName">削除するデータテーブル名が格納されている文字列。</param>
        /// <returns></returns>
        public bool ClearDataTable(string dataTableName)
        {
            lock (this)
            {
                errors();

                if (!Connected)
                {
                    errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

                    return false;
                }

                try
                {
                    Ds.Tables[dataTableName].Clear();
                }
                catch (Exception ex)
                {
                    errors((int)ExNumber.UnexpectedException, ex.Message);
                }
            }

            return Error_code == 0 ? true : false;
        }

        #endregion

        #region インデクサ

        /// <summary>
        /// データセットから指定したデータテーブルのコピーを返します。
        /// </summary>
        public DataTable this[string dataTableName]
        {
            get
            {
                lock (this)
                {
                    errors();

                    if (!Connected)
                    {
                        errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

                        return null;
                    }

                    try
                    {
                        DataTable dt = Ds.Tables[dataTableName].Copy();
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        errors((int)ExNumber.UnexpectedException, ex.Message);

                        return null;
                    }
                }
            }
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// SQL実行タイムアウト時間を取得、設定します。単位は秒。
        /// </summary>
        public int CommandTimeOut
        {
            get
            {
                return Command.CommandTimeout;
            }
            set
            {
                Command.CommandTimeout = value;
            }
        }

        /// <summary>
        /// データベースの接続状態を取得します。接続している場合は true。それ以外の場合は false。
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return Connected;
            }
        }

        /// <summary>
        /// 異常コードを取得します。
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return Error_code;
            }
        }

        /// <summary>
        /// 異常内容を取得します。
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return Error_message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            return Connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public virtual DbCommand GetDbCommand(string cmdText, DbConnection connection)
        {
            throw new NotSupportedException("派生クラスでオーバーライドしてください");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectCommand"></param>
        /// <returns></returns>
        public virtual DbDataAdapter GetDbDataAdapter(DbCommand selectCommand)
        {
            throw new NotSupportedException("派生クラスでオーバーライドしてください");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public virtual DbCommandBuilder GetDbCommandBuilder(DbDataAdapter adapter)
        {
            throw new NotSupportedException("派生クラスでオーバーライドしてください");
        }

        #endregion

        #region プライベートメソッド

        /// <summary>
        /// 異常を設定
        /// </summary>
        protected void errors()
        {
            Error_code = 0;
            Error_message = "";
        }
        /// <summary>
        /// 異常を設定
        /// </summary>
        /// <param name="error_code">異常コード</param>
        /// <param name="comment">異常内容</param>
        protected void errors(int error_code, string comment)
        {
            Error_code = error_code;
            Error_message = comment;
        }

        #endregion
    }
}
