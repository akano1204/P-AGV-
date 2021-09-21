using System;
using System.Data;
using System.Data.SqlClient; 

namespace HokushoClass.Database
{
	#region �f�[�^�x�[�X�N���X
	/// <summary>
	/// �f�[�^�x�[�X�N���X
	/// </summary>
	public class Database
	{
		#region �񋓑�
		/// <summary>
		/// �G���[��
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
		/// �g�����U�N�V�����������x��
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
		// �R���X�g���N�^
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			ip_address			IP���ڽ
		//	int				port				�߰ć�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �f�[�^�x�[�X�N���X
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
		/// �f�[�^�x�[�X�N���X
		/// </summary>
 		/// <param name="cmdTimeout">�R�}���h�^�C���A�E�g�l�̐ݒ�B</param>
		public Database(int cmdTimeout) : this()
		{
			Command.CommandTimeout = cmdTimeout;
			Connect_timeout = cmdTimeout;
		}

		//====================================================================================================
		// �f�X�g���N�^
		//====================================================================================================
		/// <summary>
		/// �f�[�^�x�[�X�N���X
		/// </summary>
		~Database()
		{
			this.Disconnect();
		}

		//====================================================================================================
		// �f�[�^�x�[�X�ڑ�
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			initialCatalog		�ް��ް���
		//	string			dataSource			SQL���޲ݽ�ݽ
		//	string			userID				հ��ID
		//	string			password			�߽ܰ��
		//	int				connectTimeout		�ȸ�����ѱ��
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �f�[�^�x�[�X�ɐڑ����܂��B
		/// </summary>
		/// <param name="initialCatalog">�f�[�^�x�[�X�����i�[����Ă��镶����B</param>
		/// <param name="dataSource">�r�p�k�T�[�o�[�C���X�^���X���i�[����Ă��镶����B</param>
		/// <param name="userID">���[�U�h�c���i�[����Ă��镶����</param>
		/// <param name="password">�p�X���[�h���i�[����Ă��镶����B</param>
		/// <param name="connectTimeout">�R�l�N�V�����^�C���A�E�g�l�B</param>
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
		/// �f�[�^�x�[�X�ɐڑ����܂��B
		/// </summary>
		/// <param name="initialCatalog">�f�[�^�x�[�X�����i�[����Ă��镶����B</param>
		/// <param name="dataSource">�r�p�k�T�[�o�[�C���X�^���X���i�[����Ă��镶����B</param>
		/// <param name="userID">���[�U�h�c���i�[����Ă��镶����</param>
		/// <param name="password">�p�X���[�h���i�[����Ă��镶����B</param>
		public bool Connect(string initialCatalog, string dataSource, string userID, string password)
		{
			return this.Connect(initialCatalog, dataSource, userID, password, Connect_timeout);
		}
		/// <summary>
		/// �f�[�^�x�[�X�ɐڑ����܂��B
		/// </summary>
		/// <param name="initialCatalog">�f�[�^�x�[�X�����i�[����Ă��镶����B</param>
		/// <param name="dataSource">�r�p�k�T�[�o�[�C���X�^���X���i�[����Ă��镶����B</param>
		public bool Connect(string initialCatalog, string dataSource)
		{
			return this.Connect(initialCatalog, dataSource, "sa", "", Connect_timeout);
		}

		//====================================================================================================
		// �f�[�^�x�[�X�ؒf
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �f�[�^�x�[�X�Ƃ̐ڑ����I�����܂��B
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
        /// �f�[�^�x�[�X�Ƃ̐ڑ����I�����܂��B�i���^�݊��p�@�ʏ�͎g�p���Ȃ����j
        /// </summary>
        public bool DesConnect()
        {
            return Disconnect();   
        }
		//====================================================================================================
		// �r�p�k���s
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			sql					���s����SQL��
		//  string			dataTableName		�ް�ð��ٖ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// SQL�����s���܂��B
		/// </summary>
		/// <param name="sql">���s����SQL�����i�[����Ă��镶����B</param>
		public bool Execute(string sql)
		{
			errors();

			if (!Connected) 
			{
				errors((int)ExNumber.NotConnectedException, "�f�[�^�x�[�X�ɐڑ�����Ă��܂���B");

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
				if (ex.Message.IndexOf("�d��") != -1) 
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
		/// SQL�����s���f�[�^�Z�b�g���ɐV�K�f�[�^�e�[�u�����쐬���܂��B
		/// </summary>
		/// <param name="sql">���s����SQL�����i�[����Ă��镶����B</param>
		/// <param name="dataTableName">�V�K�ɍ쐬����f�[�^�e�[�u�������i�[����Ă��镶����B</param>
		public bool Execute(string sql, string dataTableName)
		{
			errors();

			if (!Connected) 
			{
				errors((int)ExNumber.NotConnectedException, "�f�[�^�x�[�X�ɐڑ�����Ă��܂���B");

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
		// �X�g�A�h�v���V�[�W�����s
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			procName			�ı����ۼ��ެ��
		//  ref SqlParameter[]	param			�ı����ۼ��ެ�����Ұ�
		//	int				paramCount			���Ұ���
		//  string			dataTableName		�ް�ð��ٖ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	out int			procReturn			�ı����ۼ��ެ�̖߂�l
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �X�g�A�h�v���V�[�W�������s���܂��B
		/// </summary>
		/// <param name="procName">���s����X�g�A�h�v���V�[�W�������i�[����Ă��镶����B</param>
		/// <param name="param">�p�����[�^�l�B</param>
		/// <param name="paramCount">�p�����[�^���B</param>
		/// <param name="procReturn">�X�g�A�h�v���V�[�W���̖߂�l�B</param>
		/// <returns></returns>
		public bool ProcExecute(string procName, ref SqlParameter[] param, int paramCount, out int procReturn)
		{
			SqlParameter	return_param; 

			errors();

			procReturn = 0;

			if (!Connected) 
			{
				errors((int)ExNumber.NotConnectedException, "�f�[�^�x�[�X�ɐڑ�����Ă��܂���B");

				return false;
			}

			if (paramCount > 0 && paramCount > param.Length) 
			{	
				errors((int)ExNumber.ProcedureParameterException, "�p�����[�^���s���ł��B");

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
		/// �X�g�A�h�v���V�[�W�������s���܂��B
		/// </summary>
		/// <param name="procName">���s����X�g�A�h�v���V�[�W�������i�[����Ă��镶����B</param>
		/// <param name="procReturn">�X�g�A�h�v���V�[�W���̖߂�l�B</param>
		/// <returns></returns>
		public bool ProcExecute(string procName, out int procReturn)
		{
			SqlParameter[]	param = new SqlParameter[1];

			return this.ProcExecute(procName, ref param, 0, out procReturn);
		}
		/// <summary>
		/// �X�g�A�h�v���V�[�W�������s���f�[�^�Z�b�g���ɐV�K�f�[�^�e�[�u�����쐬���܂��B
		/// </summary>
		/// <param name="procName">���s����X�g�A�h�v���V�[�W�������i�[����Ă��镶����B</param>
		/// <param name="param">�p�����[�^�l�B</param>
		/// <param name="paramCount">�p�����[�^���B</param>
		/// <param name="procReturn">�X�g�A�h�v���V�[�W���̖߂�l�B</param>
		/// <param name="dataTableName">�V�K�ɍ쐬����f�[�^�e�[�u�������i�[����Ă��镶����B</param>
		/// <returns></returns>
		public bool ProcExecute(string procName, ref SqlParameter[] param, int paramCount, out int procReturn, string dataTableName)
		{
			SqlParameter	return_param; 

			errors();

			procReturn = 0;

			if (!Connected) 
			{
				errors((int)ExNumber.NotConnectedException, "�f�[�^�x�[�X�ɐڑ�����Ă��܂���B");

				return false;
			}

			if (paramCount > 0 && paramCount > param.Length) 
			{	
				errors((int)ExNumber.ProcedureParameterException, "�p�����[�^���s���ł��B");

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
		/// �X�g�A�h�v���V�[�W�������s���f�[�^�Z�b�g���ɐV�K�f�[�^�e�[�u�����쐬���܂��B
		/// </summary>
		/// <param name="procName">���s����X�g�A�h�v���V�[�W�������i�[����Ă��镶����B</param>
		/// <param name="procReturn">�X�g�A�h�v���V�[�W���̖߂�l�B</param>
		/// <param name="dataTableName">�V�K�ɍ쐬����f�[�^�e�[�u�������i�[����Ă��镶����B</param>
		/// <returns></returns>
		public bool ProcExecute(string procName, out int procReturn, string dataTableName)
		{
			SqlParameter[]	param = new SqlParameter[1];

			return this.ProcExecute(procName, ref param, 0, out procReturn, dataTableName);
		}

		//====================================================================================================
		// �g�����U�N�V�����������x���ݒ�
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	IsolationLevel	level				��������
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �g�����U�N�V�����������x����ݒ肵�܂��B
		/// </summary>
		/// <param name="level">�ݒ肷�镪�����x���B</param>
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
		// �g�����U�N�V�����J�n
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �g�����U�N�V�������J�n���܂��B
		/// </summary>
		public bool BeginTransaction()
		{
			return this.Execute("BEGIN TRANSACTION");
		}

		//====================================================================================================
		// �g�����U�N�V��������I��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �g�����U�N�V�������R�~�b�g���܂��B
		/// </summary>
		public bool CommitTransaction()
		{
			return this.Execute("COMMIT TRANSACTION");
		}

		//====================================================================================================
		// �g�����U�N�V�����ُ�I��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �g�����U�N�V���������[���o�b�N���܂��B
		/// </summary>
		public bool RollBackTransaction()
		{
			return this.Execute("ROLLBACK TRANSACTION");
		}

		//====================================================================================================
		// �f�[�^�e�[�u���폜
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//  string			dataTableName		�ް�ð��ٖ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �f�[�^�Z�b�g���̎w�肵���f�[�^�e�[�u�����폜���܂��B
		/// </summary>
		/// <param name="dataTableName">�폜����f�[�^�e�[�u�������i�[����Ă��镶����B</param>
		/// <returns></returns>
		public bool ClearDataTable(string dataTableName)
		{
			errors();

			if (!Connected) 
			{
				errors((int)ExNumber.NotConnectedException, "�f�[�^�x�[�X�ɐڑ�����Ă��܂���B");

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
		// �e�[�u��(�r���[)�̑��݃`�F�b�N
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//  string			tableName			ð��ٖ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int				0					���݂��Ȃ�
		//					1					���݂���
		//					-1					�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �e�[�u���܂��̓r���[�̑��݂��m�F���܂��B
		/// </summary>
		/// <param name="tableName">���݂��m�F����e�[�u�������i�[����Ă��镶����B</param>
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
				errors((int)ExNumber.NotConnectedException, "�f�[�^�x�[�X�ɐڑ�����Ă��܂���B");

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
		// �X�g�A�h�v���V�[�W���̑��݃`�F�b�N
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			procName			�ı����ۼ��ެ��
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int				0					���݂��Ȃ�
		//					1					���݂���
		//					-1					�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �X�g�A�h�v���V�[�W���̑��݂��m�F���܂��B
		/// </summary>
		/// <param name="procName">���݂��m�F����X�g�A�h�v���V�[�W�������i�[����Ă��镶����B</param>
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
				errors((int)ExNumber.NotConnectedException, "�f�[�^�x�[�X�ɐڑ�����Ă��܂���B");

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
		// �C���f�N�T
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//  string			dataTableName		�ް�ð��ٖ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	DataTable							�ް�ð���
		//
		//====================================================================================================
		/// <summary>
		/// �f�[�^�Z�b�g����w�肵���f�[�^�e�[�u���̃R�s�[��Ԃ��܂��B
		/// </summary>
		public DataTable this[string dataTableName]
		{
			get
			{
				errors();

				if (!Connected) 
				{
					errors((int)ExNumber.NotConnectedException, "�f�[�^�x�[�X�ɐڑ�����Ă��܂���B");

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
		// �ڑ��^�C���A�E�g���ԃv���p�e�B
		//====================================================================================================
		/// <summary>
		/// �f�[�^�x�[�X�̐ڑ��^�C���A�E�g���Ԃ��擾�A�ݒ肵�܂��B�P�ʂ͕b�B
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
		// �ڑ���ԃv���p�e�B
		//====================================================================================================
		/// <summary>
		/// �f�[�^�x�[�X�̐ڑ���Ԃ��擾���܂��B�ڑ����Ă���ꍇ�� true�B����ȊO�̏ꍇ�� false�B
		/// </summary>
		public bool IsConnected
		{
			get
			{
				return Connected;
			}
		}

		//====================================================================================================
		// �ُ�R�[�h�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ�R�[�h���擾���܂��B
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return Error_code;
			}		
		}

		//====================================================================================================
		// �ُ���e�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ���e���擾���܂��B
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return Error_message;
			}
		}

		//====================================================================================================
		// �ُ�̐ݒ�
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				code				�ُ���
		//	string			message				�ُ���e
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
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
