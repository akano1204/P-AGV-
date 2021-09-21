using System;
using System.Data;
using System.Data.SqlClient; 

namespace HokushoClass.Database
{
	#region データベースクラス
	/// <summary>
	/// データベースクラス
	/// </summary>
	public class Database
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

		private SqlCommand		Command;
		private SqlConnection	Connection;
		private SqlDataAdapter	Adapter;
		private DataSet			Ds;
		private int		Error_code;
		private string  Error_message;
		private bool	Connected = false;
		private int		Connect_timeout = 10;

		//====================================================================================================
		// コンストラクタ
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			ip_address			IPｱﾄﾞﾚｽ
		//	int				port				ﾎﾟｰﾄ№
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	なし
		//
		//====================================================================================================
		/// <summary>
		/// データベースクラス
		/// </summary>
		public Database()
		{
			errors();
			
			this.Disconnect();

			Command = new SqlCommand();
			Connection = new SqlConnection();
			Adapter = new SqlDataAdapter();

			Connected = false;
		}
		/// <summary>
		/// データベースクラス
		/// </summary>
 		/// <param name="cmdTimeout">コマンドタイムアウト値の設定。</param>
		public Database(int cmdTimeout) : this()
		{
			Command.CommandTimeout = cmdTimeout;
			Connect_timeout = cmdTimeout;
		}

		//====================================================================================================
		// デストラクタ
		//====================================================================================================
		/// <summary>
		/// データベースクラス
		/// </summary>
		~Database()
		{
			this.Disconnect();
		}

		//====================================================================================================
		// データベース接続
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			initialCatalog		ﾃﾞｰﾀﾍﾞｰｽ名
		//	string			dataSource			SQLｻｰﾊﾞｲﾝｽﾀﾝｽ
		//	string			userID				ﾕｰｻﾞID
		//	string			password			ﾊﾟｽﾜｰﾄﾞ
		//	int				connectTimeout		ｺﾈｸｼｮﾝﾀｲﾑｱｳﾄ
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
		/// <summary>
		/// データベースに接続します。
		/// </summary>
		/// <param name="initialCatalog">データベース名が格納されている文字列。</param>
		/// <param name="dataSource">ＳＱＬサーバーインスタンスが格納されている文字列。</param>
		/// <param name="userID">ユーザＩＤが格納されている文字列</param>
		/// <param name="password">パスワードが格納されている文字列。</param>
		/// <param name="connectTimeout">コネクションタイムアウト値。</param>
		public bool Connect(string initialCatalog, string dataSource, string userID, string password, int connectTimeout)
		{
			errors();

			if (Connected)
			{
				return true;
			}

			try 
			{
				Connection.ConnectionString = "User ID=" + userID + ";" +
					"Pwd=" + password + ";" + 
					"Initial Catalog=" + initialCatalog + ";" +
					"Data Source=" + dataSource + ";" + 
					"Connect Timeout=" + connectTimeout.ToString();
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

					Ds = new DataSet();

					Connected = true;
				}
				catch (InvalidOperationException ex)
				{
					errors((int)ExNumber.InvalidOperationException, ex.Message);
				}
				catch (SqlException ex)
				{			 
					errors(ex.Number, ex.Message);
				}
				catch (Exception ex)
				{
					errors((int)ExNumber.UnexpectedException, ex.Message);
				}
			}

			return Error_code == 0 ? true : false;
		}
		/// <summary>
		/// データベースに接続します。
		/// </summary>
		/// <param name="initialCatalog">データベース名が格納されている文字列。</param>
		/// <param name="dataSource">ＳＱＬサーバーインスタンスが格納されている文字列。</param>
		/// <param name="userID">ユーザＩＤが格納されている文字列</param>
		/// <param name="password">パスワードが格納されている文字列。</param>
		public bool Connect(string initialCatalog, string dataSource, string userID, string password)
		{
			return this.Connect(initialCatalog, dataSource, userID, password, Connect_timeout);
		}
		/// <summary>
		/// データベースに接続します。
		/// </summary>
		/// <param name="initialCatalog">データベース名が格納されている文字列。</param>
		/// <param name="dataSource">ＳＱＬサーバーインスタンスが格納されている文字列。</param>
		public bool Connect(string initialCatalog, string dataSource)
		{
			return this.Connect(initialCatalog, dataSource, "sa", "", Connect_timeout);
		}

		//====================================================================================================
		// データベース切断
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	なし
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
		/// <summary>
		/// データベースとの接続を終了します。
		/// </summary>
		/// <returns></returns>
		public bool Disconnect()
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
			catch (SqlException ex)
			{
				errors(ex.Number, ex.Message);
			}
			catch (Exception ex)
			{
				errors((int)ExNumber.UnexpectedException, ex.Message);
			}

			return Error_code == 0 ? true : false;
		}

        /// <summary>
        /// データベースとの接続を終了します。（旧型互換用　通常は使用しない事）
        /// </summary>
        public bool DesConnect()
        {
            return Disconnect();   
        }
		//====================================================================================================
		// ＳＱＬ実行
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			sql					実行するSQL文
		//  string			dataTableName		ﾃﾞｰﾀﾃｰﾌﾞﾙ名
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
		/// <summary>
		/// SQLを実行します。
		/// </summary>
		/// <param name="sql">実行するSQL文が格納されている文字列。</param>
		public bool Execute(string sql)
		{
			errors();

			if (!Connected) 
			{
				errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

				return false;
			}

			Command.Parameters.Clear();

			Command.CommandType = CommandType.Text;
			Command.CommandText = sql;

			try
			{
				Command.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				if (ex.Message.IndexOf("重複") != -1) 
				{
					errors((int)ExNumber.RecordDupException, ex.Message);
				}
				else
				{
					errors(ex.Number, ex.Message);
				}
			}
			catch (Exception ex)
			{
				errors((int)ExNumber.UnexpectedException, ex.Message);
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
			Command.CommandText = sql;

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

			return Error_code == 0 ? true : false;
		}

		//====================================================================================================
		// ストアドプロシージャ実行
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			procName			ｽﾄｱﾄﾞﾌﾟﾛｼｰｼﾞｬ名
		//  ref SqlParameter[]	param			ｽﾄｱﾄﾞﾌﾟﾛｼｰｼﾞｬのﾊﾟﾗﾒｰﾀ
		//	int				paramCount			ﾊﾟﾗﾒｰﾀ数
		//  string			dataTableName		ﾃﾞｰﾀﾃｰﾌﾞﾙ名
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	out int			procReturn			ｽﾄｱﾄﾞﾌﾟﾛｼｰｼﾞｬの戻り値
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
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
			SqlParameter	return_param; 

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
						param[count -1].Value = Command.Parameters[count].Value;
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
			SqlParameter[]	param = new SqlParameter[1];

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
			SqlParameter	return_param; 

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
						param[count -1].Value = Command.Parameters[count].Value;
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
			SqlParameter[]	param = new SqlParameter[1];

			return this.ProcExecute(procName, ref param, 0, out procReturn, dataTableName);
		}

		//====================================================================================================
		// トランザクション分離レベル設定
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	IsolationLevel	level				分離ﾚﾍﾞﾙ
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
		/// <summary>
		/// トランザクション分離レベルを設定します。
		/// </summary>
		/// <param name="level">設定する分離レベル。</param>
		/// <returns></returns>
		public bool SetIsolationLevel(IsolationLevel level)
		{
			string	sql;

			switch (level)
			{
			case	IsolationLevel.Serializable:
				sql = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE";
				break;

			case	IsolationLevel.Repeatable_Read:
				sql = "SET TRANSACTION ISOLATION LEVEL REPEATABLE READ";
				break;

			case	IsolationLevel.Read_Committed:
				sql = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
				break;

			case	IsolationLevel.Read_Uncommitted:
				sql = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED";
				break;

			default:
				sql = "";
				break;
			}

			return this.Execute(sql);
		}

		//====================================================================================================
		// トランザクション開始
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	なし
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
		/// <summary>
		/// トランザクションを開始します。
		/// </summary>
		public bool BeginTransaction()
		{
			return this.Execute("BEGIN TRANSACTION");
		}

		//====================================================================================================
		// トランザクション正常終了
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	なし
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
		/// <summary>
		/// トランザクションをコミットします。
		/// </summary>
		public bool CommitTransaction()
		{
			return this.Execute("COMMIT TRANSACTION");
		}

		//====================================================================================================
		// トランザクション異常終了
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	なし
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
		/// <summary>
		/// トランザクションをロールバックします。
		/// </summary>
		public bool RollBackTransaction()
		{
			return this.Execute("ROLLBACK TRANSACTION");
		}

		//====================================================================================================
		// データテーブル削除
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//  string			dataTableName		ﾃﾞｰﾀﾃｰﾌﾞﾙ名
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				正常
		//					false				異常
		//
		//====================================================================================================
		/// <summary>
		/// データセット内の指定したデータテーブルを削除します。
		/// </summary>
		/// <param name="dataTableName">削除するデータテーブル名が格納されている文字列。</param>
		/// <returns></returns>
		public bool ClearDataTable(string dataTableName)
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

			return Error_code == 0 ? true : false;
		}

		//====================================================================================================
		// テーブル(ビュー)の存在チェック
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//  string			tableName			ﾃｰﾌﾞﾙ名
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int				0					存在しない
		//					1					存在する
		//					-1					異常
		//
		//====================================================================================================
		/// <summary>
		/// テーブルまたはビューの存在を確認します。
		/// </summary>
		/// <param name="tableName">存在を確認するテーブル名が格納されている文字列。</param>
		/// <returns></returns>
		public int CheckTable(string tableName) 
		{
			const string	data_table_name = "HokushoClass_CheckTable";
			
			SqlParameter[]	param = new SqlParameter[1];
			DataTable	dt;
			int		proc_return;
			int		status = 0;
			string	sql;

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

		//====================================================================================================
		// ストアドプロシージャの存在チェック
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			procName			ｽﾄｱﾄﾞﾌﾟﾛｼｰｼﾞｬ名
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int				0					存在しない
		//					1					存在する
		//					-1					異常
		//
		//====================================================================================================
		/// <summary>
		/// ストアドプロシージャの存在を確認します。
		/// </summary>
		/// <param name="procName">存在を確認するストアドプロシージャ名が格納されている文字列。</param>
		/// <returns></returns>
		public int CheckProcedure(string procName) 
		{
			const string	data_table_name = "HokushoClass_CheckProcedure";
			
			SqlParameter[]	param = new SqlParameter[1];
			DataTable	dt;
			int		proc_return;
			int		status = 0;
			string	sql;

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


		//====================================================================================================
		// インデクサ
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//  string			dataTableName		ﾃﾞｰﾀﾃｰﾌﾞﾙ名
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	DataTable							ﾃﾞｰﾀﾃｰﾌﾞﾙ
		//
		//====================================================================================================
		/// <summary>
		/// データセットから指定したデータテーブルのコピーを返します。
		/// </summary>
		public DataTable this[string dataTableName]
		{
			get
			{
				errors();

				if (!Connected) 
				{
					errors((int)ExNumber.NotConnectedException, "データベースに接続されていません。");

					return null;
				}

				try
				{
					return Ds.Tables[dataTableName].Copy();
				}
				catch (Exception ex)
				{
					errors((int)ExNumber.UnexpectedException, ex.Message);
					
					return null;
				}
			}
		}

		//====================================================================================================
		// 接続タイムアウト時間プロパティ
		//====================================================================================================
		/// <summary>
		/// データベースの接続タイムアウト時間を取得、設定します。単位は秒。
		/// </summary>
		public int ConnectTimeOut
		{
			get
			{
				return Connect_timeout;
			}
			set
			{
				Connect_timeout = value;
			}
		}

		//====================================================================================================
		// 接続状態プロパティ
		//====================================================================================================
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

		//====================================================================================================
		// 異常コードプロパティ
		//====================================================================================================
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

		//====================================================================================================
		// 異常内容プロパティ
		//====================================================================================================
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

		//====================================================================================================
		// 異常の設定
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				code				異常ｺｰﾄﾞ
		//	string			message				異常内容
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	なし
		//
		//====================================================================================================
		private void errors()
		{
			Error_code = 0;
			Error_message = "";
		}
		private void errors(int error_code, string comment)
		{
			Error_code = error_code;
			Error_message = comment;
		}
	}
	#endregion
}
