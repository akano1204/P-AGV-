using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace BelicsClass.Database
{
    /// <summary>
    /// データベースクラス(OleDB)
    /// SQL Server (SQL Server認証/Windows認証)に対応
    /// MS Access (mdb)/MS Access (accdb)に対応
    /// </summary>
    public class BL_OleDb : BL_Database
    {
        #region コンストラクタ

        /// <summary>
        /// データベースクラス
        /// </summary>
        public BL_OleDb()
            : base()
        {
            Command = new OleDbCommand();
            Connection = new OleDbConnection();
            Adapter = new OleDbDataAdapter();
        }
        /// <summary>
        /// データベースクラス
        /// </summary>
        /// <param name="cmdTimeout">コマンドタイムアウト値の設定。</param>
        public BL_OleDb(int cmdTimeout)
            : this()
        {
            Command_timeout = cmdTimeout;
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
            if (initialCatalog.Trim() != "")
            {
                dateQuoteFrom = "#";
                dateQuoteTo = "'";
                FileName = "";

                if (userID.Trim() != "")
                {
                    //SQL Server認証でSQLSERVERへ接続
                    return "Provider=SQLOLEDB;" +
                            "Data Source=" + dataSource + ";" +
                            "Persist Security Info=True;" +
                            "User Id=" + userID + ";" +
                            "Password=" + password + ";" +
                            "Initial Catalog=" + initialCatalog + ";" +
                            "Integrated Security=;" +
                            "Connect Timeout=" + Connect_timeout.ToString();
                }
                else
                {
                    //Windows認証でSQLSERVERへ接続
                    //return "Provider=SQLNCLI11;" +
                    //        "Data Source=" + dataSource + ";" +
                    //        "Integrated Security=SSPI;" +
                    //        "Initial Catalog=" + initialCatalog;

                    return "Provider=SQLOLEDB;" +
                             "Initial Catalog=" + initialCatalog + ";" +
                             "Data Source=" + dataSource + ";" +
                             "Integrated Security=SSPI;" +
                             "Connect Timeout=" + Connect_timeout.ToString();
                }

            }
            else
            {
                dateQuoteFrom = "";
                dateQuoteTo = "";
                FileName = dataSource;

                if (0 <= dataSource.ToLower().IndexOf(".accdb"))
                {
                    //MSAccess(ACCDB)データファイルへ接続
                    return "Provider=Microsoft.ACE.OLEDB.12.0;" +
                            "Data Source=" + dataSource + ";" +
                            "Persist Security Info=False";
                }

                //MSAccess(MDB)データファイルへ接続
                return "Provider=Microsoft.Jet.OLEDB.4.0;" +
                        "Data Source=" + dataSource + ";" +
                        "Persist Security Info=False";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override DbCommand GetDbCommand(string cmdText, DbConnection connection)
        {
            return new OleDbCommand(cmdText, (OleDbConnection)connection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectCommand"></param>
        /// <returns></returns>
        public override DbDataAdapter GetDbDataAdapter(DbCommand selectCommand)
        {
            return new OleDbDataAdapter((OleDbCommand)selectCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public override DbCommandBuilder GetDbCommandBuilder(DbDataAdapter adapter)
        {
            return new OleDbCommandBuilder((OleDbDataAdapter)adapter);
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
                db = ((OleDbConnection)Connection).GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });
            }
            catch (OleDbException ex)
            {
                errors(ex.ErrorCode, ex.Message);
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

        #endregion
    }
}