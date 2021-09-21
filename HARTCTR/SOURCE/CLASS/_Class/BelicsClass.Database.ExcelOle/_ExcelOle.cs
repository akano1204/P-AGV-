using System;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;

namespace BelicsClass.Database
{
    /// <summary>
    /// ExcelOleクラス
    /// </summary>
    public sealed class BL_ExcelOle
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="DataSoure">Excelファイル名</param>
        public BL_ExcelOle(string DataSoure)
        {
            errors();

            FileName = DataSoure;

            string ver = "8.0";
            string provider = "Microsoft.Jet.OLEDB.4.0";
            if (Path.GetExtension(DataSoure).ToUpper() == ".XLSX")
            {
                provider = "Microsoft.ACE.OLEDB.12.0";
                ver = "12.0";
            }

            Connection = new OleDbConnection();
            Connection.ConnectionString = "Provider=" + provider + ";" +
                "Data Source=" + DataSoure + ";" +
                "Extended Properties=" + "\"Excel " + ver + ";HDR=Yes;IMEX=1\"";
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="DataSoure">Excelファイル名</param>
        /// <param name="Hdr">一行目を項目名:true,データ:false</param>
        public BL_ExcelOle(string DataSoure, bool Hdr)
        {
            string type;
            errors();

            FileName = DataSoure;

            if (Hdr == true) { type = "Yes"; }
            else { type = "No"; }

            string ver = "8.0";
            string provider = "Microsoft.Jet.OLEDB.4.0";
            if (Path.GetExtension(DataSoure).ToUpper() == ".XLSX")
            {
                provider = "Microsoft.ACE.OLEDB.12.0";
                ver = "12.0";
            }

            Connection = new OleDbConnection();
            Connection.ConnectionString = "Provider=" + provider + ";" +
                "Data Source=" + DataSoure + ";" +
                "Extended Properties=" + "\"Excel " + ver + ";HDR=" + type + ";IMEX=1\"";
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="Provider">プロバイダー名</param>
        /// <param name="DataSoure">Excelファイル名</param>
        /// <param name="ExtendedProperties">ユーザー情報</param>
        public BL_ExcelOle(string Provider, string DataSoure, string ExtendedProperties)
        {
            string ex;

            errors();

            FileName = DataSoure;

            ex = "\"" + ExtendedProperties + "\"";

            Connection = new OleDbConnection();

            Connection.ConnectionString = "Provider=" + Provider + ";" +
                "Data Source=" + DataSoure + ";" +
                "Extended Properties=" + ex;
        }

        #endregion

        #region フィールド・プロパティ

        private OleDbConnection Connection;

        /// <summary>
        /// Excelファイル名
        /// </summary>
        private string FileName;

        /// <summary>
        /// EXCELのエラー
        /// </summary>
        private int ERR_EXCEL = -100;
        /// <summary>
        /// その他のエラー
        /// </summary>
        private int ERR_OTHERS = -101;

        /// <summary>
        /// エラーコード
        /// </summary>
        private int Error_code;
        /// <summary>
        /// 異常コードを取得します
        /// </summary>
        public int ErrorCode
        {
            get { return Error_code; }
        }

        /// <summary>
        /// エラー内容
        /// </summary>
        private string Error_message;
        /// <summary>
        /// 異常メッセージを取得します
        /// </summary>
        public string ErrorMessage
        {
            get { return Error_message; }
        }

        #endregion

        #region メソッド

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
        /// <param name="List">シート名</param>
        public bool GetSheetNames(out List<string> List)
        {
            bool status = true;
            System.Data.DataTable db = new System.Data.DataTable();

            List = new List<string>();

            try
            {
                Connection.Open();
                db = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });
                Connection.Close();
            }
            catch (Exception ex)
            {
                Connection.Close();
                errors(ERR_EXCEL, ex.Message);

                status = false;
            }

            if (status == true)
            {
                foreach (System.Data.DataRow dr in db.Rows)
                {
                    string[] sheet;

                    sheet = dr[2].ToString().Split('$');

                    if (sheet.Length > 1)
                    {
                        if (List.Contains(sheet[0]) == false)
                        {
                            List.Add(sheet[0]);
                        }
                    }
                }
            }

            return status;
        }

        #endregion

        #region シート内容の読込

        /// <summary>
        /// シート内容の読込
        /// </summary>
        /// <param name="SheetName">シート名</param>
        /// <param name="Dt">データテーブル</param>
        public bool Select(string SheetName, out System.Data.DataTable Dt)
        {
            bool status = true;
            OleDbDataAdapter adapter;
            DataSet dataset = new System.Data.DataSet(SheetName);
            string sheetname_;

            sheetname_ = "[" + SheetName + "$]";

            Dt = new System.Data.DataTable();

            try
            {
                adapter = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM " + sheetname_, Connection);
                adapter.Fill(dataset, SheetName);

                Dt = dataset.Tables[SheetName];
            }
            catch (Exception ex)
            {
                errors(ERR_EXCEL, ex.Message);

                status = false;
            }

            return status;
        }

        #endregion

        #region エラーセット

        /// <summary>
        /// エラー内容のセット
        /// </summary>
        /// <param name="Number">エラー№</param>
        /// <param name="Message">エラーメッセージ</param>
        private void errors(int Number, string Message)
        {
            Error_code = Number;
            Error_message = Message;
        }

        /// <summary>
        /// エラー内容のリセット
        /// </summary>
        private void errors()
        {
            Error_code = 0;
            Error_message = "";
        }

        #endregion

        #endregion
    }
}
