using System;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Collections.Generic;

namespace BelicsClass.Database
{
    /// <summary>
    /// ExcelOleクラス
    /// </summary>
    public class BL_ExcelDb : BL_Database
    {
        /// <summary>
        /// EXCELのエラー
        /// </summary>
        private int ERR_EXCEL = -100;
        /// <summary>
        /// その他のエラー
        /// </summary>
        private int ERR_OTHERS = -101;


        /// <summary>
        /// 
        /// </summary>
        public BL_ExcelDb()
            : base()
        {
            Command = new OleDbCommand();
            Connection = new OleDbConnection();
            Adapter = new OleDbDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdTimeout">コマンドタイムアウト値の設定。</param>
        public BL_ExcelDb(int cmdTimeout)
            : this()
        {
            Command_timeout = cmdTimeout;
        }

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
            string type = isHeader ? "Yes" : "No";

            string ver = "8.0";
            string provider = "Microsoft.Jet.OLEDB.4.0";

            FileName = dataSource;
            if (Path.GetExtension(FileName).ToUpper() == ".XLSX")
            {
                ver = "12.0";
                provider = "Microsoft.ACE.OLEDB.12.0";
            }

            return "Provider=" + provider + ";" +
                    "Data Source=" + dataSource + ";" +
                    "Extended Properties=" + "\"Excel " + ver + ";HDR=" + type + ";IMEX=1\"";
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

        #region EXCEL固有メソッド

        #region 存在確認

        /// <summary>
        /// 存在確認
        /// </summary>
        public bool Exists()
        {
            bool status = false;

            errors();

            try
            {
                status = System.IO.File.Exists(FileName);
            }
            catch (Exception ex)
            {
                errors(ERR_OTHERS, ex.Message);
            }

            return status;
        }

        #endregion

        #region シート名取得

        /// <summary>
        /// シート名取得
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
            catch (Exception ex)
            {
                errors(ERR_EXCEL, ex.Message);
                return null;
            }

            foreach (DataRow dr in db.Rows)
            {
                string[] sheet;

                string name = dr[2].ToString();
                if (name.Substring(0, 1) == "'") name = name.Substring(1);
                if (name.Substring(name.Length - 1, 1) == "'") name = name.Substring(0, name.Length - 1);

                sheet = name.ToString().Split('$');

                if (1 < sheet.Length)
                {
                    if (!list.Contains(sheet[0]))
                    {
                        list.Add(sheet[0]);
                    }
                }
            }

            return list.ToArray();
        }

        #endregion

        #region シート名をテーブル名に変換

        /// <summary>
        /// シート名をテーブル名に変換
        /// </summary>
        /// <param name="sheet_name"></param>
        /// <returns></returns>
        public override string GetTableName(string sheet_name)
        {
            return "[" + sheet_name + "$]";
        }

        #endregion

        #region シートの全データを取得

        /// <summary>
        /// シートの全データを取得
        /// </summary>
        /// <param name="sheet_name"></param>
        /// <returns></returns>
        public bool GetSheetData(string sheet_name)
        {
            return Execute("select * from " + GetTableName(sheet_name), sheet_name);
        }

        #endregion

        #endregion
    }
}
