using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace BelicsClass.Database
{
    /// <summary>
    /// ORACLEへ接続します。
    /// Oracle Client Software 8.1.7以降が必要です。
    /// </summary>
    public class BL_Oracle : BelicsClass.Database.BL_Database
    {
		private Queue<DbTransaction> trans_que = new Queue<DbTransaction>();

        /// <summary>
        /// 
        /// </summary>
        public BL_Oracle()
        {
            Command = new System.Data.OracleClient.OracleCommand();
            Connection = new System.Data.OracleClient.OracleConnection();
            Adapter = new System.Data.OracleClient.OracleDataAdapter();
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
            DateQuoteFrom = "#";
            DateQuoteTo = "YYYY/MM/DD HH24:MI:SS";
            FileName = "";

            return "Data Source=" + dataSource +
                    ";Persist Security Info=True;" +
                    "User ID=" + userID + ";" +
                    "Password=" + password + ";" +
                    "Unicode=True";
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public override string ConvertDateQuate_NonQuery(string sql)
		{
			string s = sql;

			if (dateQuoteFrom != "" && dateQuoteTo != "")
			{
				s = "";
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
								c1 = "TO_DATE('";
								bdate = true;
							}
						}
						else
						{
							if (c1 == dateQuoteFrom && c2 != dateQuoteFrom)
							{
								c1 = "', '" + dateQuoteTo + "')";
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

			}

			return s;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public override string ConvertDateQuate(string sql)
		{
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
						if (sb[pos] == dateQuoteFrom[0]) sb[pos] = '\'';
					}
				}

				sql = sb.ToString();
				//sql = sql.Replace(dateQuoteFrom, dateQuoteTo);
			}

			return sql;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override DbCommand GetDbCommand(string cmdText, DbConnection connection)
        {
            return new System.Data.OracleClient.OracleCommand(cmdText, (System.Data.OracleClient.OracleConnection)connection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectCommand"></param>
        /// <returns></returns>
        public override DbDataAdapter GetDbDataAdapter(DbCommand selectCommand)
        {
            return new System.Data.OracleClient.OracleDataAdapter((System.Data.OracleClient.OracleCommand)selectCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public override DbCommandBuilder GetDbCommandBuilder(DbDataAdapter adapter)
        {
            return new System.Data.OracleClient.OracleCommandBuilder((System.Data.OracleClient.OracleDataAdapter)adapter);
        }

        /// <summary>
        /// トランザクションの開始
        /// </summary>
        /// <returns></returns>
		public override bool BeginTransaction()
		{
			try
			{
				DbTransaction trans = this.GetConnection().BeginTransaction();
				trans_que.Enqueue(trans);
				return true;
			}
			catch { }

			return false;
		}

        /// <summary>
        /// トランザクションのロールバック
        /// </summary>
        /// <returns></returns>
		public override bool RollBackTransaction()
		{
			if (0 < trans_que.Count)
			{
				DbTransaction trans = trans_que.Dequeue();

				try
				{
					trans.Rollback();
					return true;
				}
				catch { }
			}

			return false;
		}

        /// <summary>
        /// トランザクションのコミット
        /// </summary>
        /// <returns></returns>
		public override bool CommitTransaction()
		{
			if (0 < trans_que.Count)
			{
				DbTransaction trans = trans_que.Dequeue();

				try
				{
					trans.Commit();
					return true;
				}
				catch { }
			}

			return false;
		}

        /// <summary>
        /// 切断
        /// </summary>
        /// <returns></returns>
		public override bool Disconnect()
		{
			while (0 < trans_que.Count)
			{
				try
				{
					trans_que.Dequeue().Rollback();
				}
				catch { }
			}

			return base.Disconnect();
		}
    }
}
