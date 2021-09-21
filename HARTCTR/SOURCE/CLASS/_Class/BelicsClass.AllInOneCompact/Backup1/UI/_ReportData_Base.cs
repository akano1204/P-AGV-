using System;
using System.Runtime.InteropServices;

using BelicsClass.Common;
using BelicsClass.ObjectSync;

namespace BelicsClass.UI.Report
{
    /// <summary>
    /// レポート用のデータ定義のための基本クラス
    /// </summary>
    public class BL_ReportData_Base : BL_FaceMemorySync
    {
        /// <summary>
        /// レポート１ページ分のデータを抽出するためのパラメータを管理する基本クラスです
        /// </summary>
        public class BL_ReportPageParam : BL_ObjectSync
        {
            /// <summary>
            /// レポート名称
            /// </summary>
            [BL_ObjectSyncAttribute] public string Report_Name = "                                ";
        }

        /// <summary>レポート１ページ分のデータを抽出するためのパラメータを保持します。</summary>
        [BL_ObjectSyncAttribute] protected BL_ReportPageParam parameters = null;

        /// <summary>
        /// レポート１ページ分のデータを抽出するためのパラメータを取得します。
        /// </summary>
        public BL_ReportPageParam Parameters { get { return parameters; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="page_parameters">レポート１ページ分のデータを抽出するためのパラメータ</param>
        public BL_ReportData_Base(BL_ReportData_Base.BL_ReportPageParam page_parameters)
            : base()
        {
            parameters = page_parameters;

            if (page_parameters != null)
            {
                Initialize(page_parameters.Report_Name.Trim());
            }
            else
            {
                Initialize("");
            }
        }

        /// <summary>
        /// データインスタンスへのデータ読み込み
        /// オーバーライドして、１ページ分のデータを読み込む処理を実装してください。
        /// </summary>
        /// <returns></returns>
        public virtual bool Report_DataRead()
        {
            return true;
        }

        /// <summary>
        /// データインスタンスから書き込み
        /// オーバーライドして、１ページ分のデータを書き込む処理を実装してください。
        /// </summary>
        /// <returns></returns>
        public virtual bool Report_DataWrite()
        {
            return true;
        }
    }
}
