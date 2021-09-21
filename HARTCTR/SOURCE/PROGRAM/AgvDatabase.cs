using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BelicsClass.Common;
using BelicsClass.Database;
using BelicsClass.ProcessManage;

namespace PROGRAM
{
	using AGV = AgvControlManager.FloorAGV;
	using State = AgvController.AgvCommunicator.State;
	using RequestBase = AgvController.AgvOrderCommunicator.RequestBase;
	using RequestMove = AgvController.AgvOrderCommunicator.RequestMove;

	using AGV_STATE = AgvDatabase.AGV_STATE;
	using AGV_ORDER = AgvDatabase.AGV_ORDER;
	using AGV_SYS_ORDER = AgvDatabase.AGV_SYS_ORDER;

	using ActionType = AgvDatabase.ActionType;
	using BatteryStateType = AgvDatabase.BatteryStateType;
	using ActionModeType = AgvDatabase.ActionModeType;
	using FinishType = AgvDatabase.FinishType;
	using StActionType = AgvDatabase.StActionType;

	public static class AgvDatabase
	{
		#region 初期処理等

		public static BL_SQLServer DB = new BL_SQLServer();

		private static string InitialCatalog = "";
		private static string DataSource = "";

		private static string SQL_PATH = @"SQL";
		private static string TABLE_CREATION_DIR = "TableCreation";
		private static string VIEW_CREATION_DIR = "ViewCreation";
		private static string PROC_CREATION_DIR = "ProcCreation";

		private static bool enable = false;
		public static bool Enable { get { return enable; } }
		
		public static string Initialize()
		{
			var error = "";

			enable = Program.ini_hokusho.Get("DB", "ENABLE", enable);

			if (enable)
			{
				InitialCatalog = Program.ini_hokusho.Get("DB", "INITIAL_CATALOG", "HART");
				DataSource = Program.ini_hokusho.Get("DB", "DATA_SOURCE", "HART");

				error = CreateDatabase(); if (error != "") return error;

				DB.Connect(InitialCatalog, DataSource, "sa", "");

				error = DB.ErrorMessage; if (error != "") return error;

				error = CreateTables(); if (error != "") return error;

				error = CreateProcedures(); if (error != "") return error;

				error = AGV_ERR_NAME.Import();
			}

			return error;
		}

		private static string CreateDatabase()
		{
			var error = "";

			var connection = new SqlConnection()
			{
				ConnectionString = 
				$"Server={DataSource};" +
				$"Integrated Security=SSPI;" +
				$"Database=master"
			};

			try
			{
				// masterに接続
				connection.Open();

				// データベース存在確認
				var command = new SqlCommand()
				{
					Connection = connection,
					CommandText =
					$"SELECT * FROM sysdatabases " +
					$"WHERE Name = '{InitialCatalog}'"
				};

				var adapter = new SqlDataAdapter()
				{
					SelectCommand = command
				};

				var ds = new DataSet();
				var name = "dt_name";

				adapter.Fill(ds, name);

				var table = ds.Tables[name].Copy();

				ds.Tables[name].Clear();

				if (0 < table.Rows.Count) goto END;

				// データベースが無ければ生成
				command.CommandText =
					$"CREATE DATABASE {InitialCatalog}";

				command.ExecuteNonQuery();

				END:;
			}
			catch (Exception ex)
			{
				error = ex.Message;
			}
			finally
			{
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
				}
			}

			return error;
		}

		private static string CreateTables()
		{
			if (!DB.IsConnected) return "データベースに接続されていません。";

			var tables = new List<string>
			{
				nameof(AGV_STATE),
				nameof(AGV_ORDER),
				nameof(AGV_SYS_ORDER),
				nameof(AGV_ERR_HISTORY),
				nameof(AGV_ERR_NAME),
				nameof(AGV_WORK_HISTORY),
				nameof(AGV_WORK_DAY),
				nameof(AGV_WORK_TOTAL),
				nameof(STATION_INFO),
				nameof(AUTORATOR_STATE)
			};

			var views = new List<string>
			{
				nameof(AGV_ERR_VIEW),
				nameof(AGV_WORK_VIEW),
			};

			var combs = new Dictionary<string, DbObjectType>();

			foreach (var table in tables)
			{
				combs[table] = DbObjectType.TABLE;
			}

			foreach (var view in views)
			{
				combs[view] = DbObjectType.VIEW;
			}

			foreach (var comb in combs)
			{
				var name = comb.Key;
				var type = comb.Value;

				if (DB.CheckTable(name) == 0)
				{
					var file_name = $"{name}_CREATE.sql";

					var stream = GetResourceStream(file_name, type);

					var sql = BL_SQLServer.get_sql_text(stream).ToString();

					var status = DB.Execute(sql);

					if (status == false)
					{
						return DB.ErrorMessage;
					}
				}
			}

			return "";
		}

		private static string CreateProcedures()
		{
			if (!DB.IsConnected) return "データベースに接続されていません。";

			var procedures = new List<string>
			{
				nameof(AGV_STATE_UPDATE),
				nameof(AGV_ORDER_INSERT),
				nameof(AGV_ORDER_UPDATE),
				nameof(AGV_ORDER_UPDATE_FINISH),
				nameof(AGV_SYS_ORDER_INSERT),
				nameof(AGV_SYS_ORDER_UPDATE),
				nameof(AGV_ERR_HISTORY_INSERT),
				nameof(AGV_ERR_HISTORY_UPDATE),
				nameof(AGV_ERR_NAME_TRUNCATE),
				nameof(AGV_ERR_NAME_INSERT),
				nameof(AGV_WORK_HISTORY_INSERT),
				nameof(AGV_WORK_HISTORY_UPDATE),
				nameof(AGV_WORK_DAY_UPDATE),
				nameof(AGV_WORK_TOTAL_UPDATE),
				nameof(STATION_INFO_UPDATE),
				nameof(AUTORATOR_STATE_UPDATE),
			};

			foreach (var procedure in procedures)
			{
				//var a = $"DROP PROCEDURE {procedure}";

				//var b = DB.Execute(a);

				if (DB.CheckProcedure(procedure) == 0)
				{
					var file_name = $"{procedure}.sql";

					var stream = GetResourceStream(file_name, DbObjectType.PROCEDURE);

					var sql = BL_SQLServer.get_sql_text(stream).ToString();

					var status = DB.Execute(sql);

					if (status == false)
					{
						return DB.ErrorMessage;
					}
				}
			}

			return "";
		}

		private static Stream GetResourceStream(string file_name, DbObjectType type = DbObjectType.TABLE)
		{
			Stream stream = null;

			try
			{
				var dir = "";

				switch (type)
				{
					case DbObjectType.TABLE: dir = TABLE_CREATION_DIR; break;
					case DbObjectType.VIEW: dir = VIEW_CREATION_DIR; break;
					case DbObjectType.PROCEDURE: dir = PROC_CREATION_DIR; break;
				}

				var path = Path.Combine(SQL_PATH, dir);
				path = Path.Combine(path, file_name);

				stream = new FileStream(path, FileMode.Open);
			}
			catch
			{
			}

			return stream;
		}

		#endregion

		#region プロパティ

		internal static AgvControlManager controller => Program.controller;

		#endregion

		#region メソッド

		/// <summary>
		/// 動作指示テーブルの完了フラグを1に更新します
		/// </summary>
		public static string OrderComplete(AGV agv, OrderCompleteReason reason)
		{
			var req = agv.req;

			if (req.order_no == 0 || req.order_sub_no == 0) return "";

			return AGV_ORDER.Update(req.order_no, req.order_sub_no, reason);
		}

		/// <summary>
		/// 搬送履歴テーブルにレコードを追加します
		/// </summary>
		public static string BeginWork(AGV agv)
		{
			var req = agv.req;

			var qr = controller.GetStationQR(req.station);

			return AGV_WORK_HISTORY.Insert(out int id, req.order_no, req.order_sub_no,
					agv.on_qr.QrString, qr == null ? req.qr : qr.QrString, req.rack_no);
		}

		/// <summary>
		/// 搬送履歴テーブルの終了時刻に現在時刻をセットします
		/// </summary>
		public static string EndWork(AGV agv)
		{
			var req = agv.req;

			var qr = controller.GetStationQR(req.station);

			return AGV_WORK_HISTORY.Update(req.order_no, req.order_sub_no);
		}

		/// <summary>
		/// 異常履歴テーブルにレコードを追加します
		/// </summary>
		public static string RaiseError(AGV agv, State state)
		{
			return AGV_ERR_HISTORY.Insert(agv.id, state.error_code, agv.on_qr.QrString);
		}

		/// <summary>
		/// 異常履歴テーブルの完了フラグを1に更新します
		/// </summary>
		public static string ResetError(AGV agv, State state)
		{
			return AGV_ERR_HISTORY.Update(agv.id);
		}

		#endregion

		#region オブジェクトモデル

		/// <summary>
		/// オブジェクト共通
		/// </summary>
		public class AGV_DB_OBJECT
		{
			protected static BL_SQLServer DB => AgvDatabase.DB;
		}

		/// <summary>
		/// テーブル、ビュー共通
		/// </summary>
		public class AGV_DB_TABLE : AGV_DB_OBJECT
		{
			private static string DT_NAME = "DT_NAME";

			protected static string ExecuteSelectQuery(string sql, out DataTable dt)
			{
				dt = null;

				var error = "";

				var status = DB.Execute(sql, DT_NAME);

				if (status)
				{
					dt = DB[DT_NAME];

					DB.ClearDataTable(DT_NAME);
				}
				else
				{
					error = DB.ErrorMessage;
				}

				return error;
			}
		}

		#region テーブル

		/// <summary>
		/// AGV状態
		/// </summary>
		public class AGV_STATE : AGV_DB_TABLE
		{
			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>AGV識別ID</summary>
			public DateTime COMMAND_TIME = DateTime.MinValue;

			/// <summary>運航状態</summary>
			public ActionType ACTION = ActionType.U;

			/// <summary>動作指示親番</summary>
			public int ORDER_NO = 0;

			/// <summary>動作指示子番</summary>
			public int ORDER_SUB_NO = 0;

			/// <summary>充電残量</summary>
			public int BATTERY = 0;

			/// <summary>バッテリー状態</summary>
			public BatteryStateType BATTERY_STATE = BatteryStateType.NORMAL;

			/// <summary>動作モード</summary>
			public ActionModeType ACTION_MODE = ActionModeType.A;

			/// <summary>現在マップ</summary>
			public string MAP = "";

			/// <summary>現在地X座標</summary>
			public int LOCATION_X = 0;

			/// <summary>現在地Y座標</summary>
			public int LOCATION_Y = 0;

			/// <summary>ラックID</summary>
			public string RACK_ID = "";

			/// <summary>異常コード</summary>
			public int ERR_CODE = 0;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_STATE Make(DataRow row)
			{
				return new AGV_STATE
				{
					AGV_ID = row[nameof(AGV_ID)].ToString(),
					COMMAND_TIME = (DateTime)row[nameof(COMMAND_TIME)],
					ACTION = (ActionType)Enum.Parse(typeof(ActionType), row[nameof(ACTION)].ToString()),
					ORDER_NO = (int)row[nameof(ORDER_NO)],
					ORDER_SUB_NO = (int)row[nameof(ORDER_SUB_NO)],
					BATTERY = (int)row[nameof(BATTERY)],
					BATTERY_STATE = (BatteryStateType)row[nameof(BATTERY_STATE)],
					ACTION_MODE = (ActionModeType)Enum.Parse(typeof(ActionModeType), row[nameof(ACTION_MODE)].ToString()),
					MAP = row[nameof(MAP)].ToString(),
					LOCATION_X = (int)row[nameof(LOCATION_X)],
					LOCATION_Y = (int)row[nameof(LOCATION_Y)],
					RACK_ID = row[nameof(RACK_ID)].ToString(),
					ERR_CODE = (int)row[nameof(ERR_CODE)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(string agv_id, out AGV_STATE agv_state)
			{
				agv_state = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_STATE) } ";
				sql += $"WHERE AGV_ID = '{ agv_id }' ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_state = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_STATE> agv_states)
			{
				agv_states = new List<AGV_STATE>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_STATE) } ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_states.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを追加、更新
			/// </summary>
			public string InsertUpdate()
			{
				var error = AGV_STATE_UPDATE.Execute
					(AGV_ID, COMMAND_TIME, ACTION.ToString(),
					ORDER_NO, ORDER_SUB_NO, 
					BATTERY, (int)BATTERY_STATE, ACTION_MODE.ToString(), 
					MAP, LOCATION_X, LOCATION_Y, RACK_ID, ERR_CODE);

				return error;
			}

			/// <summary>
			/// レコードを追加、更新
			/// </summary>
			public static string InsertUpdate(AGV agv)
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// AGV動作指示
		/// </summary>
		public class AGV_ORDER : AGV_DB_TABLE
		{
			/// <summary>ID</summary>
			public int ID = 0;

			/// <summary>完了フラグ</summary>
			public FinishType FINISH = FinishType.INCOMPLETED;

			/// <summary>応答フラグ</summary>
			public OrderCompleteReason RESPONSE = OrderCompleteReason.SUCCESS;

			/// <summary>動作指示親番</summary>
			public int ORDER_NO = 0;

			/// <summary>動作指示子番</summary>
			public int ORDER_SUB_NO = 0;

			/// <summary>動作指示最終マーク</summary>
			public OrderMarkType ORDER_MARK = OrderMarkType.CONTINUE;

			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>目的ST</summary>
			public string ST_TO = "";

			/// <summary>目的QR</summary>
			public string TO_QR = "";

			/// <summary>目的地動作</summary>
			public StActionType ST_ACTION = StActionType.NONE;

			/// <summary>ラック到着面指定</summary>
			public RackAngleType RACK_ANGLE = RackAngleType.NONE;

			/// <summary>センサーパターン</summary>
			public int SENSOR = 0;

			/// <summary>ミュージックパターン</summary>
			public MusicType MUSIC = MusicType.NONE;

			/// <summary>動作OP1</summary>
			public int ORDER_OP1 = 0;

			/// <summary>動作OP2</summary>
			public int ORDER_OP2 = 0;

			/// <summary>動作OP3</summary>
			public int ORDER_OP3 = 0;

			/// <summary>動作OP4</summary>
			public int ORDER_OP4 = 0;

			/// <summary>動作OP5</summary>
			public int ORDER_OP5 = 0;

			/// <summary>動作情報01</summary>
			public string O_INFO1 = "";

			/// <summary>動作情報02</summary>
			public string O_INFO2 = "";

			/// <summary>動作情報03</summary>
			public string O_INFO3 = "";

			/// <summary>動作情報04</summary>
			public string O_INFO4 = "";

			/// <summary>動作情報05</summary>
			public string O_INFO5 = "";

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_ORDER Make(DataRow row)
			{
				return new AGV_ORDER
				{
					ID = (int)row[nameof(ID)],
					FINISH = (FinishType)row[nameof(FINISH)],
					RESPONSE = (OrderCompleteReason)row[nameof(RESPONSE)],
					ORDER_NO = (int)row[nameof(ORDER_NO)],
					ORDER_SUB_NO = (int)row[nameof(ORDER_SUB_NO)],
					ORDER_MARK = (OrderMarkType)row[nameof(ORDER_MARK)],
					AGV_ID = row[nameof(AGV_ID)].ToString().Trim(),
					ST_TO = row[nameof(ST_TO)].ToString().Trim(),
					TO_QR = row[nameof(TO_QR)].ToString().Trim(),
					ST_ACTION = (StActionType)row[nameof(ST_ACTION)],
					RACK_ANGLE = (RackAngleType)row[nameof(RACK_ANGLE)],
					SENSOR = (int)row[nameof(SENSOR)],
					MUSIC = (MusicType)row[nameof(MUSIC)],
					ORDER_OP1 = (int)row[nameof(ORDER_OP1)],
					ORDER_OP2 = (int)row[nameof(ORDER_OP2)],
					ORDER_OP3 = (int)row[nameof(ORDER_OP3)],
					ORDER_OP4 = (int)row[nameof(ORDER_OP4)],
					ORDER_OP5 = (int)row[nameof(ORDER_OP5)],
					O_INFO1 = row[nameof(O_INFO1)].ToString().Trim(),
					O_INFO2 = row[nameof(O_INFO2)].ToString().Trim(),
					O_INFO3 = row[nameof(O_INFO3)].ToString().Trim(),
					O_INFO4 = row[nameof(O_INFO4)].ToString().Trim(),
					O_INFO5 = row[nameof(O_INFO5)].ToString().Trim(),
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(FinishType finish, string agv_id, out AGV_ORDER agv_order)
			{
				agv_order = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_ORDER) } ";
				sql += $"WHERE FINISH = { (int)finish } AND ";
				sql += $"      AGV_ID = '{ agv_id }' ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_order = Make(row);
				}

				return error;
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(int order_no, int order_sub_no, out AGV_ORDER agv_order)
			{
				agv_order = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_ORDER) } ";
				sql += $"WHERE ORDER_NO = { order_no } AND ";
				sql += $"      ORDER_SUB_NO = { order_sub_no } ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_order = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_ORDER> agv_orders)
			{
				agv_orders = new List<AGV_ORDER>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_ORDER) } ";
				sql += $"ORDER BY ID";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_orders.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select(string agv_id, out List<AGV_ORDER> agv_orders)
			{
				agv_orders = new List<AGV_ORDER>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_ORDER) } ";
				sql += $"WHERE AGV_ID = '{ agv_id }' ";
				sql += $"ORDER BY ID";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_orders.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select(FinishType finish, string agv_id, out List<AGV_ORDER> agv_orders)
			{
				agv_orders = new List<AGV_ORDER>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_ORDER) } ";
				sql += $"WHERE FINISH = { (int)finish } AND ";
				sql += $"	   AGV_ID = '{ agv_id }' ";
				sql += $"ORDER BY ID";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_orders.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを追加
			/// </summary>
			public string Insert()
			{
				var error = AGV_ORDER_INSERT.Execute
					(out ID, ref ORDER_NO, ref ORDER_SUB_NO, (int)ORDER_MARK,
					AGV_ID, ST_TO, TO_QR, (int)ST_ACTION, (int)RACK_ANGLE,
					(int)SENSOR, (int)MUSIC,
					ORDER_OP1, ORDER_OP2, ORDER_OP3, ORDER_OP4, ORDER_OP5,
					O_INFO1, O_INFO2, O_INFO3, O_INFO4, O_INFO5);

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// </summary>
			public string Update()
			{
				var error = AGV_ORDER_UPDATE.Execute
					((int)FINISH, (int)RESPONSE, ORDER_NO, ORDER_SUB_NO,
					ORDER_OP1, ORDER_OP2, ORDER_OP3, ORDER_OP4, ORDER_OP5,
					O_INFO1, O_INFO2, O_INFO3, O_INFO4, O_INFO5);

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// </summary>
			public static string Update
				(int order_no, int order_sub_no, OrderCompleteReason response)
			{
				var error = AGV_ORDER_UPDATE_FINISH.Execute(order_no, order_sub_no, (int)response);

				return error;
			}
		}

		/// <summary>
		/// AGVシステム動作指示
		/// </summary>
		public class AGV_SYS_ORDER : AGV_DB_TABLE
		{
			/// <summary>ID</summary>
			public int ID = 0;

			/// <summary>完了フラグ</summary>
			public FinishType FINISH = FinishType.INCOMPLETED;

			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>システム動作</summary>
			public OrderType ORDER_TYPE = OrderType.RESET;

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_SYS_ORDER Make(DataRow row)
			{
				return new AGV_SYS_ORDER
				{
					ID = (int)row[nameof(ID)],
					FINISH = (FinishType)row[nameof(FINISH)],
					AGV_ID = row[nameof(AGV_ID)].ToString().Trim(),
					ORDER_TYPE = (OrderType)row[nameof(ORDER_TYPE)],
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(FinishType finish, string agv_id, out AGV_SYS_ORDER agv_sys_order)
			{
				agv_sys_order = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_SYS_ORDER) } ";
				sql += $"WHERE FINISH = { (int)finish } AND ";
				sql += $"      AGV_ID = '{ agv_id }' ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_sys_order = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_SYS_ORDER> agv_sys_orders)
			{
				agv_sys_orders = new List<AGV_SYS_ORDER>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_SYS_ORDER) } ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_sys_orders.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select(FinishType finish, out List<AGV_SYS_ORDER> agv_sys_orders)
			{
				agv_sys_orders = new List<AGV_SYS_ORDER>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_SYS_ORDER) } ";
				sql += $"WHERE FINISH = { (int)finish } ";
				sql += $" ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_sys_orders.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select(FinishType finish, string agv_id, out List<AGV_SYS_ORDER> agv_sys_orders)
			{
				agv_sys_orders = new List<AGV_SYS_ORDER>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_SYS_ORDER) } ";
				sql += $"WHERE FINISH = { (int)finish } AND ";
				sql += $"		AGV_ID = '{ agv_id }' ";
				sql += $" ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_sys_orders.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを追加
			/// FINISHには0が設定されます
			/// </summary>
			public string Insert()
			{
				var error = AGV_SYS_ORDER_INSERT.Execute(out ID, AGV_ID, (int)ORDER_TYPE);

				return error;
			}

			/// <summary>
			/// レコードを追加
			/// FINISHには0が設定されます
			/// </summary>
			public static string Insert(out int id, string agv_id, OrderType order_type)
			{
				var error = AGV_SYS_ORDER_INSERT.Execute(out id, agv_id, (int)order_type);

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// FINISHには1が設定されます
			/// </summary>
			public string Update()
			{
				var error = AGV_SYS_ORDER_UPDATE.Execute(ID);

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// FINISHには1が設定されます
			/// </summary>
			public static string Update(int id)
			{
				var error = AGV_SYS_ORDER_UPDATE.Execute(id);

				return error;
			}
		}

		/// <summary>
		/// AGV異常履歴
		/// </summary>
		public class AGV_ERR_HISTORY : AGV_DB_TABLE
		{
			/// <summary>ID</summary>
			public int ID = 0;

			/// <summary>完了フラグ</summary>
			public FinishType FINISH = FinishType.INCOMPLETED;

			/// <summary>日付</summary>
			public string MAKE_DATE = "";

			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>異常コード</summary>
			public int ERR_CODE = 0;

			/// <summary>最終床QR</summary>
			public string FLOOR_QR = "";

			/// <summary>発生時刻</summary>
			public DateTime START_TIME = DateTime.MinValue;

			/// <summary>復旧時刻</summary>
			public DateTime? END_TIME = null;

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_ERR_HISTORY Make(DataRow row)
			{
				var end_time = row[nameof(END_TIME)];

				return new AGV_ERR_HISTORY
				{
					ID = (int)row[nameof(ID)],
					FINISH = (FinishType)row[nameof(FINISH)],
					MAKE_DATE = row[nameof(MAKE_DATE)].ToString().Trim(),
					AGV_ID = row[nameof(AGV_ID)].ToString().Trim(),
					ERR_CODE = (int)row[nameof(ERR_CODE)],
					FLOOR_QR = row[nameof(FLOOR_QR)].ToString().Trim(),
					START_TIME = (DateTime)row[nameof(START_TIME)],
					END_TIME = end_time is DBNull ? (DateTime?)null : (DateTime)end_time,
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_ERR_HISTORY> agv_err_histories)
			{
				agv_err_histories = new List<AGV_ERR_HISTORY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_ERR_HISTORY) } ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_err_histories.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select
				(FinishType finish, out List<AGV_ERR_HISTORY> agv_err_histories)
			{
				agv_err_histories = new List<AGV_ERR_HISTORY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_ERR_HISTORY) } ";
				sql += $"WHERE FINISH = { (int)finish } ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_err_histories.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select
				(FinishType finish, string make_date, string agv_id,
				out List<AGV_ERR_HISTORY> agv_err_histories)
			{
				agv_err_histories = new List<AGV_ERR_HISTORY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_ERR_HISTORY) } ";
				sql += $"WHERE FINISH = { (int)finish } AND ";
				sql += $"		MAKE_DATE = '{ make_date }' AND ";
				sql += $"		AGV_ID = '{ agv_id }' ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_err_histories.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを追加
			/// FINISHには0
			/// MAKE_DATEには実行時の日付
			/// START_TIMEには実行時の時刻が設定されます
			/// </summary>
			public string Insert()
			{
				var error = AGV_ERR_HISTORY_INSERT.Execute(AGV_ID, ERR_CODE, FLOOR_QR);

				return error;
			}

			/// <summary>
			/// レコードを追加
			/// FINISHには0
			/// MAKE_DATEには実行時の日付
			/// START_TIMEには実行時の時刻が設定されます
			/// </summary>
			public static string Insert(string agv_id, int err_code, string floor_qr)
			{
				var error = AGV_ERR_HISTORY_INSERT.Execute(agv_id, err_code, floor_qr);

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// FINISHには1
			/// END_TIMEには実行時の時刻が設定されます
			/// </summary>
			public string Update()
			{
				var error = AGV_ERR_HISTORY_UPDATE.Execute(AGV_ID);

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// FINISHには1
			/// END_TIMEには実行時の時刻が設定されます
			/// </summary>
			public static string Update(string agv_id)
			{
				var error = AGV_ERR_HISTORY_UPDATE.Execute(agv_id);

				return error;
			}
		}

		/// <summary>
		/// AGV異常名称
		/// </summary>
		public class AGV_ERR_NAME : AGV_DB_TABLE
		{
			private static string ERR_NAME_PATH = @"..\DATA\errname.dat";
			private static Encoding ENCODING = Encoding.GetEncoding("SHIFT_JIS"); 

			/// <summary>異常コード</summary>
			public int ERR_CODE = 0;

			/// <summary>異常名称</summary>
			public string ERR_NAME = "";

			/// <summary>異常詳細</summary>
			public string ERR_DETAIL = "";

			/// <summary>復旧方法</summary>
			public string ERR_RECOVERY = "";

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_ERR_NAME Make(DataRow row)
			{
				return new AGV_ERR_NAME
				{
					ERR_CODE = (int)row[nameof(ERR_CODE)],
					ERR_NAME = row[nameof(ERR_NAME)].ToString().Trim(),
					ERR_DETAIL = row[nameof(ERR_DETAIL)].ToString().Trim(),
					ERR_RECOVERY = row[nameof(ERR_RECOVERY)].ToString().Trim(),
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(int err_code, out AGV_ERR_NAME agv_err_name)
			{
				agv_err_name = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_ERR_NAME) } ";
				sql += $"WHERE ERR_CODE = { err_code }";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_err_name = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_ERR_NAME> agv_err_name)
			{
				agv_err_name = new List<AGV_ERR_NAME>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_ERR_NAME) } ";
				sql += $"ORDER BY ERR_CODE ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_err_name.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// ファイルから異常名称一覧をインポート
			/// </summary>
			public static string Import()
			{
				var error = "";

				if (!File.Exists(ERR_NAME_PATH))
				{
					goto END;
				}

				var last = File.GetLastWriteTime(ERR_NAME_PATH);
				last = new DateTime(last.Year, last.Month, last.Day, last.Hour, last.Minute, last.Second);

				var last_ini = Program.ini_hokusho.Get("DB", "LAST_WRITE_TIME", DateTime.MinValue);

				if (last == last_ini)
				{
					goto END;
				}

				error = AGV_ERR_NAME_TRUNCATE.Execute();
				if (error != "") goto END;

				using (var sr = new FileStream(ERR_NAME_PATH, FileMode.Open, FileAccess.Read))
				{
					var length = sr.Length;

					var offset = 0;

					bool read(ref byte[] array, int count)
					{
						var end = false;
						var rest = length - (offset + count);

						var size = count;

						if (rest <= 0)
						{
							size = (int)(length - offset);
							end = true;
						}

						array = new byte[size];

						sr.Read(array, 0, size);

						offset += size;

						return end;
					}

					while (true)
					{
						byte[] array = null;

						var err_code = 0;
						var err_name = "";
						var err_detail = "";
						var err_recovery = "";

						var end = false;
						var str = "";

						// 異常コード
						end = read(ref array, 5);
						str = ENCODING.GetString(array);

						if (!int.TryParse(str, out err_code))
						{
							error = "異常コードの数値変換に失敗しました";
							goto END;
						}

						if (end) goto END_WHILE;

						// 異常名称
						end = read(ref array, 45);
						err_name = ENCODING.GetString(array);

						if (end) goto END_WHILE;

						// 異常詳細
						end = read(ref array, 200);
						err_detail = ENCODING.GetString(array);

						if (end) goto END_WHILE;

						// 復旧方法
						end = read(ref array, 250);
						err_recovery = ENCODING.GetString(array);

						if (end) goto END_WHILE;

						END_WHILE:
						{
							error = AGV_ERR_NAME_INSERT.Execute
								(err_code, err_name, err_detail, err_recovery);
							if (error != "") goto END;

							if (end)
							{
								break;
							}
						}
					}
				}

				if (error == "")
				{
					Program.ini_hokusho.Set("DB", "LAST_WRITE_TIME", last);
				}

				END:

				return error;
			}
		}

		/// <summary>
		/// AGV搬送履歴
		/// </summary>
		public class AGV_WORK_HISTORY : AGV_DB_TABLE
		{
			/// <summary>ID</summary>
			public int ID = 0;

			/// <summary>日付</summary>
			public string MAKE_DATE = "";

			/// <summary>動作指示親番</summary>
			public int ORDER_NO = 0;

			/// <summary>動作指示子番</summary>
			public int ORDER_SUB_NO = 0;

			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>発生時刻</summary>
			public DateTime START_TIME = DateTime.MinValue;

			/// <summary>復旧時刻</summary>
			public DateTime? END_TIME = null;

			/// <summary>現在地QR</summary>
			public string FROM_QR = "";

			/// <summary>目的地QR</summary>
			public string TO_QR = "";

			/// <summary>ラックID</summary>
			public string RACK_ID = "";

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_WORK_HISTORY Make(DataRow row)
			{
				var end_time = row[nameof(END_TIME)];

				return new AGV_WORK_HISTORY
				{
					ID = (int)row[nameof(ID)],
					MAKE_DATE = row[nameof(MAKE_DATE)].ToString().Trim(),
					ORDER_NO = (int)row[nameof(ORDER_NO)],
					ORDER_SUB_NO = (int)row[nameof(ORDER_SUB_NO)],
					AGV_ID = row[nameof(AGV_ID)].ToString().Trim(),
					START_TIME = (DateTime)row[nameof(START_TIME)],
					END_TIME = end_time is DBNull ? (DateTime?) null : (DateTime)end_time,
					FROM_QR = row[nameof(FROM_QR)].ToString().Trim(),
					TO_QR = row[nameof(TO_QR)].ToString().Trim(),
					RACK_ID = row[nameof(RACK_ID)].ToString().Trim(),
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(int order_no, int order_sub_no, out AGV_WORK_HISTORY agv_work_history)
			{
				agv_work_history = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_WORK_HISTORY) } ";
				sql += $"WHERE ORDER_NO = { order_no } AND ";
				sql += $"      ORDER_SUB_NO = { order_sub_no } ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_work_history = Make(row);
				}

				return error;
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(int id, out AGV_WORK_HISTORY agv_work_history)
			{
				agv_work_history = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_WORK_HISTORY) } ";
				sql += $"WHERE ID = { id } ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_work_history = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_WORK_HISTORY> agv_work_histories)
			{
				agv_work_histories = new List<AGV_WORK_HISTORY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_WORK_HISTORY) } ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_work_histories.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select
				(string make_date, string agv_id, 
				out List<AGV_WORK_HISTORY> agv_work_histories)
			{
				agv_work_histories = new List<AGV_WORK_HISTORY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_WORK_HISTORY) } ";
				sql += $"WHERE MAKE_DATE = '{ make_date }' AND ";
				sql += $"	   AGV_ID = '{ agv_id }' ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_work_histories.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select(int order_no, out List<AGV_WORK_HISTORY> agv_work_histories)
			{
				agv_work_histories = new List<AGV_WORK_HISTORY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_WORK_HISTORY) } ";
				sql += $"WHERE ORDER_NO = { order_no }";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_work_histories.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを追加
			/// MAKE_DATEには実行時の日付
			/// AGV_IDにはAGV_ORDERテーブルの動作指示親番、子番の一致するAGV_ID
			/// START_TIMEには実行時の時刻が設定されます
			/// </summary>
			public string Insert()
			{
				var error = AGV_WORK_HISTORY_INSERT.Execute
					(out ID, ORDER_NO, ORDER_SUB_NO, FROM_QR, TO_QR, RACK_ID);

				return error;
			}

			/// <summary>
			/// レコードを追加
			/// MAKE_DATEには実行時の日付
			/// AGV_IDにはAGV_ORDERテーブルの動作指示親番、子番の一致するAGV_ID
			/// START_TIMEには実行時の時刻が設定されます
			/// </summary>
			public static string Insert
				(out int id, int order_no, int order_sub_no,
				string from_qr, string to_qr, string rack_id)
			{
				var error = AGV_WORK_HISTORY_INSERT.Execute
					(out id, order_no, order_sub_no, from_qr, to_qr, rack_id);

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// END_TIMEには実行時の時刻が設定されます
			/// </summary>
			public string Update()
			{
				var error = AGV_WORK_HISTORY_UPDATE.Execute(ORDER_NO, ORDER_SUB_NO);

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// END_TIMEには実行時の時刻が設定されます
			/// </summary>
			public static string Update(int order_no, int order_sub_no)
			{
				var error = AGV_WORK_HISTORY_UPDATE.Execute(order_no, order_sub_no);

				return error;
			}
		}

		/// <summary>
		/// AGV稼働能力
		/// </summary>
		public class AGV_WORK_DAY : AGV_DB_TABLE
		{
			/// <summary>日付</summary>
			public string MAKE_DATE = "";

			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>走行時間(秒)</summary>
			public int RUN_SECONDS = 0;

			/// <summary>充電時間(秒)</summary>
			public int CHARGE_SECONDS = 0;

			/// <summary>搬送時間(秒)</summary>
			public int WORK_SECONDS = 0;

			/// <summary>搬送回数</summary>
			public int WORK_COUNT = 0;

			/// <summary>昇降回数</summary>
			public int LIFT_COUNT = 0;

			/// <summary>走行距離(cm)</summary>
			public int RUN_DISTANCE = 0;

			/// <summary>待ち時間</summary>
			public int WAIT_SECONDS = 0;

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_WORK_DAY Make(DataRow row)
			{
				return new AGV_WORK_DAY
				{
					MAKE_DATE = row[nameof(MAKE_DATE)].ToString().Trim(),
					AGV_ID = row[nameof(AGV_ID)].ToString().Trim(),
					RUN_SECONDS = (int)row[nameof(RUN_SECONDS)],
					CHARGE_SECONDS = (int)row[nameof(CHARGE_SECONDS)],
					WORK_SECONDS = (int)row[nameof(WORK_SECONDS)],
					WORK_COUNT = (int)row[nameof(WORK_COUNT)],
					LIFT_COUNT = (int)row[nameof(LIFT_COUNT)],
					RUN_DISTANCE = (int)row[nameof(RUN_DISTANCE)],
					WAIT_SECONDS = (int)row[nameof(WAIT_SECONDS)],
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(string make_date, string agv_id, out AGV_WORK_DAY agv_work_day)
			{
				agv_work_day = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_WORK_DAY) } ";
				sql += $"WHERE MAKE_DATE = '{ make_date }' AND ";
				sql += $"      AGV_ID = '{ agv_id }' ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_work_day = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_WORK_DAY> agv_work_days)
			{
				agv_work_days = new List<AGV_WORK_DAY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_WORK_DAY) } ";
				sql += $"ORDER BY MAKE_DATE, AGV_ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_work_days.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string Select(string make_date, out List<AGV_WORK_DAY> agv_work_days)
			{
				agv_work_days = new List<AGV_WORK_DAY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_WORK_DAY) } ";
				sql += $"WHERE MAKE_DATE = '{ make_date }' ";
				sql += $"ORDER BY AGV_ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_work_days.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// 複数レコードを取得
			/// </summary>
			public static string SelectByAGVID(string agv_id, out List<AGV_WORK_DAY> agv_work_days)
			{
				agv_work_days = new List<AGV_WORK_DAY>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_WORK_DAY) } ";
				sql += $"WHERE AGV_ID = '{ agv_id }' ";
				sql += $"ORDER BY MAKE_DATE ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_work_days.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを追加、更新
			/// 各値差分がAGV_WORK_TOTALのレコードに加算されます
			/// </summary>
			public string InsertUpdate()
			{
				var error = AGV_WORK_DAY_UPDATE.Execute
					(MAKE_DATE, AGV_ID,
					RUN_SECONDS, CHARGE_SECONDS, WORK_SECONDS,
					WORK_COUNT, LIFT_COUNT, RUN_DISTANCE, WAIT_SECONDS);

				return error;
			}
		}

		/// <summary>
		/// AGV稼働累積
		/// </summary>
		public class AGV_WORK_TOTAL : AGV_DB_TABLE
		{
			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>走行時間(秒)</summary>
			public int RUN_SECONDS = 0;

			/// <summary>充電時間(秒)</summary>
			public int CHARGE_SECONDS = 0;

			/// <summary>搬送時間(秒)</summary>
			public int WORK_SECONDS = 0;

			/// <summary>搬送回数</summary>
			public int WORK_COUNT = 0;

			/// <summary>昇降回数</summary>
			public int LIFT_COUNT = 0;

			/// <summary>走行距離(cm)</summary>
			public int RUN_DISTANCE = 0;

			/// <summary>警報昇降回数</summary>
			public int ALERT_LIFT_COUNT = 0;

			/// <summary>警報走行距離(cm)</summary>
			public int ALERT_RUN_DISTANCE = 0;

			/// <summary>メンテ時間1</summary>
			public DateTime? REPAIR_TIME1 = null;

			/// <summary>メンテ時間2</summary>
			public DateTime? REPAIR_TIME2 = null;

			/// <summary>メンテ時間3</summary>
			public DateTime? REPAIR_TIME3 = null;

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_WORK_TOTAL Make(DataRow row)
			{
				var repair_time1 = row[nameof(REPAIR_TIME1)];
				var repair_time2 = row[nameof(REPAIR_TIME2)];
				var repair_time3 = row[nameof(REPAIR_TIME3)];

				return new AGV_WORK_TOTAL
				{
					AGV_ID = row[nameof(AGV_ID)].ToString().Trim(),
					RUN_SECONDS = (int)row[nameof(RUN_SECONDS)],
					CHARGE_SECONDS = (int)row[nameof(CHARGE_SECONDS)],
					WORK_SECONDS = (int)row[nameof(WORK_SECONDS)],
					WORK_COUNT = (int)row[nameof(WORK_COUNT)],
					LIFT_COUNT = (int)row[nameof(LIFT_COUNT)],
					RUN_DISTANCE = (int)row[nameof(RUN_DISTANCE)],
					ALERT_LIFT_COUNT = (int)row[nameof(ALERT_LIFT_COUNT)],
					ALERT_RUN_DISTANCE = (int)row[nameof(ALERT_RUN_DISTANCE)],
					REPAIR_TIME1 = repair_time1 is DBNull ? (DateTime?)null : (DateTime)repair_time1,
					REPAIR_TIME2 = repair_time2 is DBNull ? (DateTime?)null : (DateTime)repair_time2,
					REPAIR_TIME3 = repair_time3 is DBNull ? (DateTime?)null : (DateTime)repair_time3,
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(string agv_id, out AGV_WORK_TOTAL agv_work_total)
			{
				agv_work_total = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AGV_WORK_TOTAL) } ";
				sql += $"WHERE AGV_ID = '{ agv_id }' ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					agv_work_total = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_WORK_TOTAL> agv_work_totals)
			{
				agv_work_totals = new List<AGV_WORK_TOTAL>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_WORK_TOTAL) } ";
				sql += $"ORDER BY AGV_ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_work_totals.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを更新
			/// </summary>
			public string Update()
			{
				var error = AGV_WORK_TOTAL_UPDATE.Execute
					(AGV_ID,
					RUN_SECONDS, CHARGE_SECONDS, WORK_SECONDS,
					WORK_COUNT, LIFT_COUNT, RUN_DISTANCE,
					ALERT_LIFT_COUNT, ALERT_RUN_DISTANCE,
					REPAIR_TIME1, REPAIR_TIME2, REPAIR_TIME3);

				return error;
			}
		}

		/// <summary>
		/// AGVステーション情報
		/// </summary>
		public class STATION_INFO : AGV_DB_TABLE
		{
			/// <summary>ステーションID</summary>
			public string ST_ID = "";

			/// <summary>ステーション名</summary>
			public string ST_NAME = "";

			/// <summary>ステーションタイプ</summary>
			public int ST_TYPE = 0;

			/// <summary>ステーション属性</summary>
			public int ST_ATTRIBUTE = 0;

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static STATION_INFO Make(DataRow row)
			{
				return new STATION_INFO
				{
					ST_ID = row[nameof(ST_ID)].ToString().Trim(),
					ST_NAME = row[nameof(ST_NAME)].ToString().Trim(),
					ST_TYPE = (int)row[nameof(ST_TYPE)],
					ST_ATTRIBUTE = (int)row[nameof(ST_ATTRIBUTE)],
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(string st_id, out STATION_INFO station_info)
			{
				station_info = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(STATION_INFO) } ";
				sql += $"WHERE ST_ID = '{ st_id }' ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					station_info = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<STATION_INFO> station_infos)
			{
				station_infos = new List<STATION_INFO>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(STATION_INFO) } ";
				sql += $"ORDER BY ST_ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						station_infos.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを追加、更新
			/// </summary>
			public string InsertUpdate()
			{
				var error = STATION_INFO_UPDATE.Execute(ST_ID, ST_NAME, ST_TYPE, ST_ATTRIBUTE);

				return error;
			}
		}

		/// <summary>
		/// 垂直機情報
		/// </summary>
		public class AUTORATOR_STATE : AGV_DB_TABLE
		{
			/// <summary>オートレーター識別ID</summary>
			public int AUTORATOR_ID = 0;

			/// <summary>最終コマンド受信時間</summary>
			public DateTime COMMAND_TIME = DateTime.MinValue;

			/// <summary>状態</summary>
			public AutoratorStateType TOTAL_STATE = AutoratorStateType.OFFLINE;

			/// <summary>異常コード</summary>
			public int ERR_CODE = 0;

			/// <summary>キャリッジ滞在階</summary>
			public int FLOOR = 0;

			/// <summary>搬入階</summary>
			public int IN_FLOOR = 0;

			/// <summary>搬入状態</summary>
			public CarryInStateType IN_STATE = CarryInStateType.NONE;

			/// <summary>搬出階</summary>
			public int OUT_FLOOR = 0;

			/// <summary>搬出状態</summary>
			public int OUT_STATE = 0;

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AUTORATOR_STATE Make(DataRow row)
			{
				return new AUTORATOR_STATE
				{
					AUTORATOR_ID = (int)row[nameof(AUTORATOR_ID)],
					COMMAND_TIME = (DateTime)row[nameof(COMMAND_TIME)],
					TOTAL_STATE = (AutoratorStateType)Enum.Parse
						(typeof(AutoratorStateType), row[nameof(TOTAL_STATE)].ToString()),
					ERR_CODE = (int)row[nameof(ERR_CODE)],
					FLOOR = (int)row[nameof(FLOOR)],
					IN_FLOOR = (int)row[nameof(IN_FLOOR)],
					IN_STATE = (CarryInStateType)Enum.Parse
						(typeof(CarryInStateType), row[nameof(IN_STATE)].ToString()),
					OUT_FLOOR = (int)row[nameof(OUT_FLOOR)],
					OUT_STATE = (int)row[nameof(OUT_STATE)],
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// レコードを取得
			/// </summary>
			public static string Select(int autorator_id, out AUTORATOR_STATE autorator_state)
			{
				autorator_state = null;

				var sql = "";

				sql = $"SELECT TOP(1) * FROM { nameof(AUTORATOR_STATE) } ";
				sql += $"WHERE AUTORATOR_ID = '{ autorator_id }' ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					var row = dt.Rows[0];

					autorator_state = Make(row);
				}

				return error;
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AUTORATOR_STATE> autorator_states)
			{
				autorator_states = new List<AUTORATOR_STATE>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AUTORATOR_STATE) } ";
				sql += $"ORDER BY AUTORATOR_ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						autorator_states.Add(Make(row));
					}
				}

				return error;
			}

			/// <summary>
			/// レコードを追加、更新
			/// </summary>
			public string InsertUpdate()
			{
				var error = AUTORATOR_STATE_UPDATE.Execute
					(AUTORATOR_ID, COMMAND_TIME, (int)TOTAL_STATE, ERR_CODE,
					FLOOR, IN_FLOOR, (int)IN_STATE, OUT_FLOOR, OUT_STATE);

				return error;
			}
		}

		#endregion

		#region ビュー

		/// <summary>
		/// AGV異常履歴ビュー
		/// </summary>
		public class AGV_ERR_VIEW : AGV_DB_TABLE
		{
			/// <summary>ID</summary>
			public int ID = 0;

			/// <summary>完了フラグ</summary>
			public FinishType FINISH = FinishType.INCOMPLETED;

			/// <summary>日付</summary>
			public string MAKE_DATE = "";

			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>異常コード</summary>
			public int ERR_CODE = 0;

			/// <summary>異常名称</summary>
			public string ERR_NAME = "";

			/// <summary>異常詳細</summary>
			public string ERR_DETAIL = "";

			/// <summary>復旧方法</summary>
			public string ERR_RECOVERY = "";

			/// <summary>最終床QR</summary>
			public string FLOOR_QR = "";

			/// <summary>発生時刻</summary>
			public DateTime START_TIME = DateTime.MinValue;

			/// <summary>復旧時刻</summary>
			public DateTime? END_TIME = null;

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_ERR_VIEW Make(DataRow row)
			{
				var end_time = row[nameof(END_TIME)];

				return new AGV_ERR_VIEW
				{
					ID = (int)row[nameof(ID)],
					FINISH = (FinishType)row[nameof(FINISH)],
					MAKE_DATE = row[nameof(MAKE_DATE)].ToString(),
					AGV_ID = row[nameof(AGV_ID)].ToString(),
					ERR_CODE = (int)row[nameof(ERR_CODE)],
					ERR_NAME = row[nameof(ERR_NAME)].ToString(),
					ERR_DETAIL = row[nameof(ERR_DETAIL)].ToString(),
					ERR_RECOVERY = row[nameof(ERR_RECOVERY)].ToString(),
					FLOOR_QR = row[nameof(FLOOR_QR)].ToString(),
					START_TIME = (DateTime)row[nameof(START_TIME)],
					END_TIME = end_time is DBNull ? (DateTime?)null : (DateTime)end_time,
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_ERR_VIEW> agv_err_views)
			{
				agv_err_views = new List<AGV_ERR_VIEW>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_ERR_VIEW) } ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_err_views.Add(Make(row));
					}
				}

				return error;
			}
		}

		/// <summary>
		/// AGV搬送履歴ビュー
		/// </summary>
		public class AGV_WORK_VIEW : AGV_DB_TABLE
		{
			/// <summary>ID</summary>
			public int ID = 0;

			/// <summary>日付</summary>
			public string MAKE_DATE = "";

			/// <summary>動作指示親番</summary>
			public int ORDER_NO = 0;

			/// <summary>動作指示子番</summary>
			public int ORDER_SUB_NO = 0;

			/// <summary>動作指示最終マーク</summary>
			public OrderMarkType ORDER_MARK = OrderMarkType.CONTINUE;

			/// <summary>AGV識別ID</summary>
			public string AGV_ID = "";

			/// <summary>発生時刻</summary>
			public DateTime START_TIME = DateTime.MinValue;

			/// <summary>復旧時刻</summary>
			public DateTime? END_TIME = null;

			/// <summary>現在地QR</summary>
			public string FROM_QR = "";

			/// <summary>目的地QR</summary>
			public string TO_QR = "";

			/// <summary>目的ST</summary>
			public string ST_TO = "";

			/// <summary>目的地動作</summary>
			public StActionType ST_ACTION = StActionType.NONE;

			/// <summary>ラックID</summary>
			public string RACK_ID = "";

			/// <summary>ラック到着面指定</summary>
			public RackAngleType RACK_ANGLE = RackAngleType.NONE;

			/// <summary>センサーパターン</summary>
			public int SENSOR = 0;

			/// <summary>ミュージックパターン</summary>
			public MusicType MUSIC = MusicType.NONE;

			/// <summary>動作OP1</summary>
			public int ORDER_OP1 = 0;

			/// <summary>動作OP2</summary>
			public int ORDER_OP2 = 0;

			/// <summary>動作OP3</summary>
			public int ORDER_OP3 = 0;

			/// <summary>動作OP4</summary>
			public int ORDER_OP4 = 0;

			/// <summary>動作OP5</summary>
			public int ORDER_OP5 = 0;

			/// <summary>動作情報01</summary>
			public string O_INFO1 = "";

			/// <summary>動作情報02</summary>
			public string O_INFO2 = "";

			/// <summary>動作情報03</summary>
			public string O_INFO3 = "";

			/// <summary>動作情報04</summary>
			public string O_INFO4 = "";

			/// <summary>動作情報05</summary>
			public string O_INFO5 = "";

			/// <summary>作成時刻</summary>
			public DateTime MAKE_TIME = DateTime.MinValue;

			/// <summary>最終更新時刻</summary>
			public DateTime LAST_TIME = DateTime.MinValue;

			/// <summary>
			/// データ行からインスタンスを生成
			/// </summary>
			private static AGV_WORK_VIEW Make(DataRow row)
			{
				var end_time = row[nameof(END_TIME)];

				return new AGV_WORK_VIEW
				{
					ID = (int)row[nameof(ID)],
					MAKE_DATE = row[nameof(MAKE_DATE)].ToString(),
					ORDER_NO = (int)row[nameof(ORDER_NO)],
					ORDER_SUB_NO = (int)row[nameof(ORDER_SUB_NO)],
					ORDER_MARK = (OrderMarkType)row[nameof(ORDER_MARK)],
					AGV_ID = row[nameof(AGV_ID)].ToString(),
					START_TIME = (DateTime)row[nameof(START_TIME)],
					END_TIME = end_time is DBNull ? (DateTime?)null : (DateTime)end_time,
					FROM_QR = row[nameof(FROM_QR)].ToString(),
					TO_QR = row[nameof(TO_QR)].ToString(),
					ST_TO = row[nameof(ST_TO)].ToString(),
					ST_ACTION = (StActionType)row[nameof(ST_ACTION)],
					RACK_ID = row[nameof(RACK_ID)].ToString(),
					RACK_ANGLE = (RackAngleType)row[nameof(RACK_ANGLE)],
					SENSOR = (int)row[nameof(SENSOR)],
					MUSIC = (MusicType)row[nameof(MUSIC)],
					ORDER_OP1 = (int)row[nameof(ORDER_OP1)],
					ORDER_OP2 = (int)row[nameof(ORDER_OP2)],
					ORDER_OP3 = (int)row[nameof(ORDER_OP3)],
					ORDER_OP4 = (int)row[nameof(ORDER_OP4)],
					ORDER_OP5 = (int)row[nameof(ORDER_OP5)],
					O_INFO1 = row[nameof(O_INFO1)].ToString(),
					O_INFO2 = row[nameof(O_INFO2)].ToString(),
					O_INFO3 = row[nameof(O_INFO3)].ToString(),
					O_INFO4 = row[nameof(O_INFO4)].ToString(),
					O_INFO5 = row[nameof(O_INFO5)].ToString(),
					MAKE_TIME = (DateTime)row[nameof(MAKE_TIME)],
					LAST_TIME = (DateTime)row[nameof(LAST_TIME)],
				};
			}

			/// <summary>
			/// 全レコードを取得
			/// </summary>
			public static string Select(out List<AGV_WORK_VIEW> agv_work_views)
			{
				agv_work_views = new List<AGV_WORK_VIEW>();

				var sql = "";

				sql = $"SELECT * FROM { nameof(AGV_WORK_VIEW) } ";
				sql += $"ORDER BY ID ";

				var error = ExecuteSelectQuery(sql, out DataTable dt);

				if (dt != null && 0 < dt.Rows.Count)
				{
					foreach (DataRow row in dt.Rows)
					{
						agv_work_views.Add(Make(row));
					}
				}

				return error;
			}
		}

		#endregion

		#region ストアドプロシージャ

		/// <summary>
		/// ストアドプロシージャ共通
		/// </summary>
		public class AGV_DB_PROCEDURE<T> : AGV_DB_OBJECT
		{
			private static object ExecutionLock = new object();

			protected static string ProcExecute(List<SqlParameter> parameters)
			{
				lock (ExecutionLock)
				{
					var proc_name = typeof(T).Name;

					for (int i = 0; i < parameters.Count; i++)
					{
						var parameter = parameters[i];

						parameter.ParameterName = $"@{parameter.ParameterName}";
					}

					var param = parameters.ToArray();

					var status = DB.ProcExecute(proc_name, ref param, param.Length, out int rtn);

					parameters = param.ToList();

					var error = "";

					if (status == false)
					{
						error = $"{ DB.ErrorMessage }\nストアドプロシージャエラーコード[{rtn}]";
					}
					else if (rtn == -1)
					{
						error = $"存在しないレコードを参照しようとしています。\n" +
							$"ストアドプロシージャエラーコード[{rtn}]";
					}

					return error;
				}
			}

			protected static string ProcExecute()
			{
				return ProcExecute(new List<SqlParameter>());
			}
		}

		/// <summary>
		/// AGV状態テーブル追加、更新
		/// </summary>
		public class AGV_STATE_UPDATE : AGV_DB_PROCEDURE<AGV_STATE_UPDATE>
		{
			public static string Execute
				(string Agv_id, DateTime Command_time, string Action,
				int Order_no, int Order_sub_no, int Battery, int Battery_state,
				string Action_mode, string Map, int Location_x, int Location_y,
				string Rack_id, int Err_code)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Agv_id), SqlDbType.NVarChar) { Size = 10, Value = Agv_id },
					new SqlParameter(nameof(Command_time), SqlDbType.DateTime) { Value = Command_time },
					new SqlParameter(nameof(Action), SqlDbType.Char) { Size = 1, Value = Action },
					new SqlParameter(nameof(Order_no), SqlDbType.Int) { Value = Order_no },
					new SqlParameter(nameof(Order_sub_no), SqlDbType.Int) { Value = Order_sub_no },
					new SqlParameter(nameof(Battery), SqlDbType.Int) { Value = Battery },
					new SqlParameter(nameof(Battery_state), SqlDbType.Int) { Value = Battery_state },
					new SqlParameter(nameof(Action_mode), SqlDbType.Char) { Size = 1, Value = Action_mode },
					new SqlParameter(nameof(Map), SqlDbType.Char) { Size = 1, Value = Map },
					new SqlParameter(nameof(Location_x), SqlDbType.Int) { Value = Location_x },
					new SqlParameter(nameof(Location_y), SqlDbType.Int) { Value = Location_y },
					new SqlParameter(nameof(Rack_id), SqlDbType.NVarChar) { Size = 20, Value = Rack_id },
					new SqlParameter(nameof(Err_code), SqlDbType.Int) { Value = Err_code },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGV動作指示テーブル追加
		/// </summary>
		public class AGV_ORDER_INSERT : AGV_DB_PROCEDURE<AGV_ORDER_INSERT>
		{
			/// <summary>
			/// AGV_ORDERテーブルにレコードを追加します
			/// Order_noが
			/// 0の場合:採番した親番をOrder_noにセットします
			/// 1以上の場合:採番した子番をOrder_sub_noにセットします
			/// 1以上の場合にはAgv_idはストアドプロシージャ内で同一の親番のものを探し、それを用います
			/// </summary>
			public static string Execute
				(out int Id, ref int Order_no, ref int Order_sub_no, int Order_mark,
				string Agv_id, string St_to, string To_qr, int St_action, int Rack_angle,
				int Sensor, int Music,
				int Order_op1, int Order_op2, int Order_op3, int Order_op4, int Order_op5,
				string O_info1, string O_info2, string O_info3, string O_info4, string O_info5)
			{
				Id = 0;

				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Id), SqlDbType.Int) { Direction = ParameterDirection.Output },
					new SqlParameter(nameof(Order_no), SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = Order_no },
					new SqlParameter(nameof(Order_sub_no), SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = Order_sub_no },
					new SqlParameter(nameof(Order_mark), SqlDbType.Int) { Value = Order_mark },
					new SqlParameter(nameof(Agv_id), SqlDbType.NVarChar) { Size = 10, Value = Agv_id },
					new SqlParameter(nameof(St_to), SqlDbType.NVarChar) { Size = 20, Value = St_to },
					new SqlParameter(nameof(To_qr), SqlDbType.NVarChar) { Size = 20, Value = To_qr },
					new SqlParameter(nameof(St_action), SqlDbType.Int) { Value = St_action },
					new SqlParameter(nameof(Rack_angle), SqlDbType.Int) { Value = Rack_angle },
					new SqlParameter(nameof(Sensor), SqlDbType.Int) { Value = Sensor },
					new SqlParameter(nameof(Music), SqlDbType.Int) { Value = Music },
					new SqlParameter(nameof(Order_op1), SqlDbType.Int) { Value = Order_op1 },
					new SqlParameter(nameof(Order_op2), SqlDbType.Int) { Value = Order_op2 },
					new SqlParameter(nameof(Order_op3), SqlDbType.Int) { Value = Order_op3 },
					new SqlParameter(nameof(Order_op4), SqlDbType.Int) { Value = Order_op4 },
					new SqlParameter(nameof(Order_op5), SqlDbType.Int) { Value = Order_op5 },
					new SqlParameter(nameof(O_info1), SqlDbType.NVarChar) { Size = 50, Value = O_info1 },
					new SqlParameter(nameof(O_info2), SqlDbType.NVarChar) { Size = 50, Value = O_info2 },
					new SqlParameter(nameof(O_info3), SqlDbType.NVarChar) { Size = 50, Value = O_info3 },
					new SqlParameter(nameof(O_info4), SqlDbType.NVarChar) { Size = 50, Value = O_info4 },
					new SqlParameter(nameof(O_info5), SqlDbType.NVarChar) { Size = 50, Value = O_info5 },
				};

				var error = ProcExecute(parameters);

				if (error == "")
				{
					var p = parameters.FirstOrDefault(e => e.ParameterName == $"@{nameof(Id)}");
					Id = (int)p.Value;
					p = parameters.FirstOrDefault(e => e.ParameterName == $"@{nameof(Order_no)}");
					Order_no = (int)p.Value;
					p = parameters.FirstOrDefault(e => e.ParameterName == $"@{nameof(Order_sub_no)}");
					Order_sub_no = (int)p.Value;
				}

				return error;
			}
		}

		/// <summary>
		/// AGV動作指示テーブル更新
		/// </summary>
		public class AGV_ORDER_UPDATE : AGV_DB_PROCEDURE<AGV_ORDER_UPDATE>
		{
			/// <summary>
			/// AGV_ORDERテーブルのレコードを更新します
			/// </summary>
			public static string Execute
				(int Finish, int Response, int Order_no, int Order_sub_no,
				int Order_op1, int Order_op2, int Order_op3, int Order_op4, int Order_op5,
				string O_info1, string O_info2, string O_info3, string O_info4, string O_info5)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Order_no), SqlDbType.Int) { Value = Order_no },
					new SqlParameter(nameof(Order_sub_no), SqlDbType.Int) { Value = Order_sub_no },
					new SqlParameter(nameof(Finish), SqlDbType.Int) { Value = Finish },
					new SqlParameter(nameof(Response), SqlDbType.Int) { Value = Response },
					new SqlParameter(nameof(Order_op1), SqlDbType.Int) { Value = Order_op1 },
					new SqlParameter(nameof(Order_op2), SqlDbType.Int) { Value = Order_op2 },
					new SqlParameter(nameof(Order_op3), SqlDbType.Int) { Value = Order_op3 },
					new SqlParameter(nameof(Order_op4), SqlDbType.Int) { Value = Order_op4 },
					new SqlParameter(nameof(Order_op5), SqlDbType.Int) { Value = Order_op5 },
					new SqlParameter(nameof(O_info1), SqlDbType.NVarChar) { Size = 50, Value = O_info1 },
					new SqlParameter(nameof(O_info2), SqlDbType.NVarChar) { Size = 50, Value = O_info2 },
					new SqlParameter(nameof(O_info3), SqlDbType.NVarChar) { Size = 50, Value = O_info3 },
					new SqlParameter(nameof(O_info4), SqlDbType.NVarChar) { Size = 50, Value = O_info4 },
					new SqlParameter(nameof(O_info5), SqlDbType.NVarChar) { Size = 50, Value = O_info5 },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGV動作指示テーブル更新
		/// </summary>
		public class AGV_ORDER_UPDATE_FINISH : AGV_DB_PROCEDURE<AGV_ORDER_UPDATE_FINISH>
		{
			/// <summary>
			/// AGV_ORDERテーブルのレコードを更新します
			/// </summary>
			public static string Execute
				(int Order_no, int Order_sub_no, int Response)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Order_no), SqlDbType.Int) { Value = Order_no },
					new SqlParameter(nameof(Order_sub_no), SqlDbType.Int) { Value = Order_sub_no },
					new SqlParameter(nameof(Response), SqlDbType.Int) { Value = Response },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGVシステム動作指示テーブル追加
		/// </summary>
		public class AGV_SYS_ORDER_INSERT : AGV_DB_PROCEDURE<AGV_SYS_ORDER_INSERT>
		{
			/// <summary>
			/// AGV_SYS_ORDERテーブルにレコードを追加します
			/// FINISHには0が設定されます
			/// </summary>
			public static string Execute(out int Id, string Agv_id, int Order_type)
			{
				Id = 0;

				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Id), SqlDbType.Int) { Direction = ParameterDirection.Output },
					new SqlParameter(nameof(Agv_id), SqlDbType.NVarChar) { Size = 10, Value = Agv_id },
					new SqlParameter(nameof(Order_type), SqlDbType.Int) { Value = Order_type },
				};

				var error = ProcExecute(parameters);

				if (error == "")
				{
					var p = parameters.FirstOrDefault(e => e.ParameterName == $"@{nameof(Id)}");
					Id = (int)p.Value;
				}

				return error;
			}
		}

		/// <summary>
		/// AGVシステム動作指示テーブル更新
		/// </summary>
		public class AGV_SYS_ORDER_UPDATE : AGV_DB_PROCEDURE<AGV_SYS_ORDER_UPDATE>
		{
			/// <summary>
			/// AGV_SYS_ORDERテーブルのレコードを更新します
			/// FINISHには1が設定されます
			/// </summary>
			public static string Execute(int Id)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Id), SqlDbType.Int) { Value = Id },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGV異常履歴テーブル追加
		/// </summary>
		public class AGV_ERR_HISTORY_INSERT : AGV_DB_PROCEDURE<AGV_ERR_HISTORY_INSERT>
		{
			/// <summary>
			/// AGV_ERR_HISTORYテーブルにレコードを追加します
			/// FINISHには0
			/// MAKE_DATEには実行時の日付
			/// START_TIMEには実行時の時刻が設定されます
			/// </summary>
			public static string Execute(string Agv_id, int Err_code, string Floor_qr)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Agv_id), SqlDbType.NVarChar) { Size = 10, Value = Agv_id },
					new SqlParameter(nameof(Err_code), SqlDbType.Int) { Value = Err_code },
					new SqlParameter(nameof(Floor_qr), SqlDbType.NVarChar) { Size = 20, Value = Floor_qr },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGV異常履歴テーブル更新
		/// </summary>
		public class AGV_ERR_HISTORY_UPDATE : AGV_DB_PROCEDURE<AGV_ERR_HISTORY_UPDATE>
		{
			/// <summary>
			/// AGV_ERR_HISTORYテーブルのレコードを更新します
			/// FINISHには1
			/// END_TIMEには実行時の時刻が設定されます
			/// </summary>
			public static string Execute(string Agv_id)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Agv_id), SqlDbType.NVarChar) { Size = 10, Value = Agv_id },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGV異常名称テーブルレコード除去
		/// </summary>
		public class AGV_ERR_NAME_TRUNCATE : AGV_DB_PROCEDURE<AGV_ERR_NAME_TRUNCATE>
		{
			/// <summary>
			/// AGV_ERR_NAMEテーブルから全てのレコードを除去します
			/// </summary>
			public static string Execute()
			{
				var error = ProcExecute();

				return error;
			}
		}

		/// <summary>
		/// AGV異常名称テーブル追加
		/// </summary>
		public class AGV_ERR_NAME_INSERT : AGV_DB_PROCEDURE<AGV_ERR_NAME_INSERT>
		{
			/// <summary>
			/// AGV_ERR_NAMEテーブルにレコードを追加します
			/// </summary>
			public static string Execute(int Err_code, string Err_name, string Err_detail, string Err_recovery)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Err_code), SqlDbType.Int) { Value = Err_code },
					new SqlParameter(nameof(Err_name), SqlDbType.NVarChar) { Size = 45, Value = Err_name },
					new SqlParameter(nameof(Err_detail), SqlDbType.NVarChar) { Size = 200, Value = Err_detail },
					new SqlParameter(nameof(Err_recovery), SqlDbType.NVarChar) { Size = 250, Value = Err_recovery },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGV搬送履歴テーブル追加
		/// </summary>
		public class AGV_WORK_HISTORY_INSERT : AGV_DB_PROCEDURE<AGV_WORK_HISTORY_INSERT>
		{
			/// <summary>
			/// AGV_WORK_HISTORYテーブルにレコードを追加します
			/// MAKE_DATEには実行時の日付
			/// AGV_IDにはAGV_ORDERテーブルの動作指示親番、子番の一致するAGV_ID
			/// START_TIMEには実行時の時刻が設定されます
			/// </summary>
			public static string Execute
				(out int Id, int Order_no, int Order_sub_no,
				string From_qr, string To_qr, string Rack_id)
			{
				Id = 0;

				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Id), SqlDbType.Int) { Direction = ParameterDirection.Output },
					new SqlParameter(nameof(Order_no), SqlDbType.Int) { Value = Order_no },
					new SqlParameter(nameof(Order_sub_no), SqlDbType.Int) { Value = Order_sub_no },
					new SqlParameter(nameof(From_qr), SqlDbType.NVarChar) { Size = 20, Value = From_qr },
					new SqlParameter(nameof(To_qr), SqlDbType.NVarChar) { Size = 20, Value = To_qr },
					new SqlParameter(nameof(Rack_id), SqlDbType.NVarChar) { Size = 20, Value = Rack_id },
				};

				var error = ProcExecute(parameters);

				if (error == "")
				{
					var p = parameters.FirstOrDefault(e => e.ParameterName == $"@{nameof(Id)}");
					Id = (int)p.Value;
				}

				return error;
			}
		}

		/// <summary>
		/// AGV搬送履歴テーブル更新
		/// </summary>
		public class AGV_WORK_HISTORY_UPDATE : AGV_DB_PROCEDURE<AGV_WORK_HISTORY_UPDATE>
		{
			/// <summary>
			/// AGV_WORK_HISTORYテーブルのレコードを更新します
			/// END_TIMEには実行時の時刻が設定されます
			/// </summary>
			public static string Execute(int Order_no, int Order_sub_no)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Order_no), SqlDbType.Int) { Value = Order_no },
					new SqlParameter(nameof(Order_sub_no), SqlDbType.Int) { Value = Order_sub_no },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGV稼働能力テーブル、AGV稼働累積テーブル追加、更新
		/// </summary>
		public class AGV_WORK_DAY_UPDATE : AGV_DB_PROCEDURE<AGV_WORK_DAY_UPDATE>
		{
			/// <summary>
			/// AGV_WORK_DAYテーブルのレコードを追加、更新します
			/// </summary>
			public static string Execute
				(string Make_date, string Agv_id,
				int Run_seconds, int Charge_seconds, int Work_seconds,
				int Work_count, int Lift_count, int Run_distance, int Wait_seconds)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Make_date), SqlDbType.Char) { Size = 8, Value = Make_date },
					new SqlParameter(nameof(Agv_id), SqlDbType.NVarChar) { Size = 10, Value = Agv_id },
					new SqlParameter(nameof(Run_seconds), SqlDbType.Int) { Value = Run_seconds },
					new SqlParameter(nameof(Charge_seconds), SqlDbType.Int) { Value = Charge_seconds },
					new SqlParameter(nameof(Work_seconds), SqlDbType.Int) { Value = Work_seconds },
					new SqlParameter(nameof(Work_count), SqlDbType.Int) { Value = Work_count },
					new SqlParameter(nameof(Lift_count), SqlDbType.Int) { Value = Lift_count },
					new SqlParameter(nameof(Run_distance), SqlDbType.Int) { Value = Run_distance },
					new SqlParameter(nameof(Wait_seconds), SqlDbType.Int) { Value = Wait_seconds },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGV稼働累積テーブル追加、更新
		/// </summary>
		public class AGV_WORK_TOTAL_UPDATE : AGV_DB_PROCEDURE<AGV_WORK_TOTAL_UPDATE>
		{
			public static string Execute
				(string Agv_id,
				int Run_seconds, int Charge_seconds, int Work_seconds,
				int Work_count, int Lift_count, int Run_distance,
				int Alert_lift_count, int Alert_run_distance,
				DateTime? Repair_time1, DateTime? Repair_time2, DateTime? Repair_time3)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Agv_id), SqlDbType.NVarChar) { Size = 10, Value = Agv_id },
					new SqlParameter(nameof(Run_seconds), SqlDbType.Int) { Value = Run_seconds },
					new SqlParameter(nameof(Charge_seconds), SqlDbType.Int) { Value = Charge_seconds },
					new SqlParameter(nameof(Work_seconds), SqlDbType.Int) { Value = Work_seconds },
					new SqlParameter(nameof(Work_count), SqlDbType.Int) { Value = Work_count },
					new SqlParameter(nameof(Lift_count), SqlDbType.Int) { Value = Lift_count },
					new SqlParameter(nameof(Run_distance), SqlDbType.Int) { Value = Run_distance },
					new SqlParameter(nameof(Alert_lift_count), SqlDbType.Int) { Value = Alert_lift_count },
					new SqlParameter(nameof(Alert_run_distance), SqlDbType.Int) { Value = Alert_run_distance },
					new SqlParameter(nameof(Repair_time1), SqlDbType.DateTime) { Value = Repair_time1 },
					new SqlParameter(nameof(Repair_time2), SqlDbType.DateTime) { Value = Repair_time2 },
					new SqlParameter(nameof(Repair_time3), SqlDbType.DateTime) { Value = Repair_time3 },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// AGVステーション情報テーブル更新
		/// </summary>
		public class STATION_INFO_UPDATE : AGV_DB_PROCEDURE<STATION_INFO_UPDATE>
		{
			public static string Execute(string St_id, string St_name, int St_type, int St_attribute)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(St_id), SqlDbType.NVarChar) { Size = 20, Value = St_id },
					new SqlParameter(nameof(St_name), SqlDbType.NVarChar) { Size = 100, Value = St_name },
					new SqlParameter(nameof(St_type), SqlDbType.Int) { Value = St_type },
					new SqlParameter(nameof(St_attribute), SqlDbType.Int) { Value = St_attribute },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		/// <summary>
		/// 垂直機情報テーブル更新
		/// </summary>
		public class AUTORATOR_STATE_UPDATE : AGV_DB_PROCEDURE<AUTORATOR_STATE_UPDATE>
		{
			public static string Execute
				(int Autorator_id, DateTime Command_time, int Total_state, int Err_code,
				int Floor, int In_floor, int In_state, int Out_floor, int Out_state)
			{
				var parameters = new List<SqlParameter>
				{
					new SqlParameter(nameof(Autorator_id), SqlDbType.Int) { Value = Autorator_id },
					new SqlParameter(nameof(Command_time), SqlDbType.DateTime) { Value = Command_time },
					new SqlParameter(nameof(Total_state), SqlDbType.Int) { Value = Total_state },
					new SqlParameter(nameof(Err_code), SqlDbType.Int) { Value = Err_code },
					new SqlParameter(nameof(Floor), SqlDbType.Int) { Value = Floor },
					new SqlParameter(nameof(In_floor), SqlDbType.Int) { Value = In_floor },
					new SqlParameter(nameof(In_state), SqlDbType.Int) { Value = In_state },
					new SqlParameter(nameof(Out_floor), SqlDbType.Int) { Value = Out_floor },
					new SqlParameter(nameof(Out_state), SqlDbType.Int) { Value = Out_state },
				};

				var error = ProcExecute(parameters);

				return error;
			}
		}

		#endregion

		#endregion

		#region 列挙体

		#region データベースオブジェクト

		/// <summary>
		/// データベースオブジェクト種
		/// </summary>
		public enum DbObjectType
		{
			/// <summary>テーブル</summary>
			TABLE,
			/// <summary>ビュー</summary>
			VIEW,
			/// <summary>プロシージャ</summary>
			PROCEDURE
		}

		#endregion

		#region フィールド

		/// <summary>
		/// 運航状態
		/// </summary>
		public enum ActionType
		{
			/// <summary>不明</summary>
			[BL_EnumLabel("不明")]
			U,
			/// <summary>停止中(動作指示なし)</summary>
			[BL_EnumLabel("停止中")]
			S,
			/// <summary>動作中</summary>
			[BL_EnumLabel("動作中")]
			M,
			/// <summary>充電中</summary>
			[BL_EnumLabel("充電中")]
			C,
			/// <summary>待機中</summary>
			[BL_EnumLabel("待機中")]
			W,
			/// <summary>異常中</summary>
			[BL_EnumLabel("異常中")]
			E,
			/// <summary>ダウン中</summary>
			[BL_EnumLabel("ダウン中")]
			D
		}

		/// <summary>
		/// バッテリー状態
		/// </summary>
		public enum BatteryStateType : int
		{
			/// <summary>通常</summary>
			[BL_EnumLabel("通常")]
			NORMAL = 0,
			/// <summary>異常</summary>
			[BL_EnumLabel("異常")]
			ERROR = 1
		}

		/// <summary>
		/// 動作モード
		/// </summary>
		public enum ActionModeType
		{
			/// <summary>AUTO</summary>
			[BL_EnumLabel("AUTO")]
			A,
			/// <summary>MANUAL</summary>
			[BL_EnumLabel("MANUAL")]
			M
		}

		/// <summary>
		/// 完了フラグ
		/// </summary>
		public enum FinishType : int
		{
			/// <summary>未完了</summary>
			[BL_EnumLabel("未完了")]
			INCOMPLETED = 0,
			/// <summary>完了</summary>
			[BL_EnumLabel("完了")]
			COMPLETED = 1
		}

		/// <summary>
		/// 応答フラグ
		/// </summary>
		public enum OrderCompleteReason : int
		{
			/// <summary>成功</summary>
			[BL_EnumLabel("成功")]
			SUCCESS = 0,
			/// <summary>ルート算出不可</summary>
			[BL_EnumLabel("ルート算出不可")]
			ROUTING_ERROR,
			/// <summary>リセット</summary>
			[BL_EnumLabel("リセット")]
			RESET,
			/// <summary>オートレーター使用不可</summary>
			[BL_EnumLabel("オートレーター使用不可")]
			AUTORATOR_OFFLINE,
			/// <summary>充電開始済み</summary>
			[BL_EnumLabel("充電開始済み")]
			ALREADY_CHARGE_STARTED,
			/// <summary>充電停止済み</summary>
			[BL_EnumLabel("充電停止済み")]
			ALREADY_CHARGE_STOPPED,
		}

		/// <summary>
		/// 動作指示最終マーク
		/// </summary>
		public enum OrderMarkType: int
		{
			/// <summary>最終</summary>
			[BL_EnumLabel("最終")]
			LAST = 0,
			/// <summary>続行</summary>
			[BL_EnumLabel("続行")]
			CONTINUE = 1
		}

		/// <summary>
		/// 目的地動作
		/// </summary>
		public enum StActionType : int
		{
			/// <summary>なし</summary>
			[BL_EnumLabel("なし")]
			NONE = 0,
			/// <summary>棚上昇</summary>
			[BL_EnumLabel("棚上昇")]
			LIFT_RACK = 1,
			/// <summary>棚下降</summary>
			[BL_EnumLabel("棚下降")]
			LOWER_RACK = 2,
			/// <summary>充電開始</summary>
			[BL_EnumLabel("充電開始")]
			START_CHARGING = 3,
			/// <summary>充電終了</summary>
			[BL_EnumLabel("充電終了")]
			STOP_CHARGING = 4,
		}

		/// <summary>
		/// ラック到着面指定
		/// </summary>
		public enum RackAngleType : int
		{
			/// <summary>指定なし</summary>
			[BL_EnumLabel("指定なし")]
			NONE = 0,
			/// <summary>0度</summary>
			[BL_EnumLabel("0度")]
			_0 = 1,
			/// <summary>90度</summary>
			[BL_EnumLabel("90度")]
			_90 = 2,
			/// <summary>180度</summary>
			[BL_EnumLabel("180度")]
			_180 = 3,
			/// <summary>270度</summary>
			[BL_EnumLabel("270度")]
			_270 = 4,
		}

		/// <summary>
		/// ミュージックパターン
		/// </summary>
		public enum MusicType : int
		{
			/// <summary>なし</summary>
			[BL_EnumLabel("なし")]
			NONE = 0,
			/// <summary>森のくまさん</summary>
			[BL_EnumLabel("森のくまさん")]
			_1 = 1
		}

		/// <summary>
		/// システム動作
		/// </summary>
		public enum OrderType : int
		{
			/// <summary>ダウン登録</summary>
			[BL_EnumLabel("ダウン登録")]
			REGISTER_DOWN = 1,
			/// <summary>ダウン解除</summary>
			[BL_EnumLabel("ダウン解除")]
			CANCEL_DOWN = 2,
			/// <summary>リセット</summary>
			[BL_EnumLabel("リセット")]
			RESET = 3
		}

		/// <summary>
		/// 垂直機状態
		/// </summary>
		public enum AutoratorStateType : int
		{
			/// <summary>オフライン</summary>
			[BL_EnumLabel("オフライン")]
			OFFLINE = 0,
			/// <summary>自動運転OFF</summary>
			[BL_EnumLabel("自動運転OFF")]
			OFF_AUTO_RUNNING = 1,
			/// <summary>手動中</summary>
			[BL_EnumLabel("手動中")]
			MANUAL = 2,
			/// <summary>異常中</summary>
			[BL_EnumLabel("異常中")]
			ERROR = 3,
			/// <summary>オンライン待機中</summary>
			[BL_EnumLabel("オンライン待機中")]
			WAITING_ONLINE = 11,
			/// <summary>オンライン昇降中</summary>
			[BL_EnumLabel("オンライン昇降中")]
			LIFTING_ONLINE = 12,
			/// <summary>搬入可</summary>
			[BL_EnumLabel("搬入可")]
			CAN_CARRY_IN = 20,
			/// <summary>搬出可</summary>
			[BL_EnumLabel("搬出可")]
			CAN_CARRY_OUT = 21,
		}

		/// <summary>
		/// 搬入状態
		/// </summary>
		public enum CarryInStateType : int
		{
			/// <summary>動作なし</summary>
			[BL_EnumLabel("動作なし")]
			NONE = 0,
			/// <summary>搬入/搬出準備中</summary>
			[BL_EnumLabel("搬入/搬出準備中")]
			PREPARING = 1,
			/// <summary>搬入/搬出動作中</summary>
			[BL_EnumLabel("搬入/搬出動作中")]
			RUNNING = 2,
		}

		#endregion

		#endregion
	}

	public class AgvDatabaseManager : BL_ThreadController_Base
	{
		#region イベント

		public delegate void AgvDatabaseError_EventHandler(AgvDatabaseManager sender, string message);
		public event AgvDatabaseError_EventHandler Event_AgvDatabaseError;

		public delegate void AgvOrderDown_EventHandler(AgvDatabaseManager sender,AGV_SYS_ORDER sys_order);
		public event AgvOrderDown_EventHandler Event_AgvOrderDown;

		public delegate void AgvOrder_EventHandler(AgvDatabaseManager sender, RequestBase req);
		public event AgvOrder_EventHandler Event_AgvOrder;

		#endregion

		#region フィールド

		private AGV agv = null;
		private State state_pre = new State();
		private int order_no { get { return agv.req == null ? 0 : agv.req.order_no; } }
		private int order_no_pre = 0;
		private int order_sub_no { get { return agv.req == null ? 0 : agv.req.order_sub_no; } }
		private int order_sub_no_pre = 0;
		private Queue<State> que = new Queue<State>();

		private int last_order_id = 0;

		#endregion

		#region コンストラクタ

		public AgvDatabaseManager(AGV agv)
		{
			this.agv = agv;
		}

		#endregion

		#region メソッド

		public void SetState(State state)
		{
			if (!AgvDatabase.Enable) return;

			lock (que) que.Enqueue(state);
		}

		private bool IsStateChanged(State state)
		{
			if (state_pre.last_time != state.last_time) return true;
			if (GetAction(state_pre) != GetAction(state)) return true;

			if (IsOrderNoChanged()) return true;

			if (state_pre.bat != state.bat) return true;
			if ((state_pre.bat == 255) != (state.bat == 255)) return true;
			if (state_pre.sta_runmode != state.sta_runmode) return true;
			if (state_pre.map != state.map) return true;
			if (state_pre.x != state.x) return true;
			if (state_pre.y != state.y) return true;
			if (GetRackID(state_pre) != GetRackID(state)) return true;
			if (state_pre.error_code != state.error_code) return true;

			return false;
		}

		private bool IsOrderNoChanged()
		{
			if (order_no_pre != order_no) return true;
			if (order_sub_no_pre != order_sub_no) return true;

			return false;
		}

		private ActionType GetAction(State state)
		{
			var ret = ActionType.U;

            if (agv != null && agv.agvRunner != null && agv.agvRunner.communicator != null && agv.agvRunner.communicator.Alive)
            {
                var cmd = state.cmd;

                if (state.error_code != 0)
                {
                    // 異常中
                    ret = ActionType.E;
                }
                else if (state.sta_charge == true)
                {
                    // 充電中
                    ret = ActionType.C;
                }
                else
                {
                    if (agv.req != null)
                    {
                        switch ((State.CMD)cmd)
                        {
                            // 動作中
                            case State.CMD.STATE: ret = ActionType.M; break;
                            // 待機中
                            case State.CMD.REQUEST: ret = ActionType.W; break;
                            default: ret = ActionType.S; break;
                        }
                    }
                    else
                    {
                        // 停止中(動作指示なし)
                        ret = ActionType.S;
                    }
                }
            }

			return ret;
		}

		private string GetRackID(State state)
		{
			var ret = "";

			ret = (state.rack_no == 0 || !state.sta_rack) ? 
				"" : state.racktype + state.rack_no.ToString();

			return ret;
		}

        State state_last = new State();
        BL_Stopwatch swupdate = new BL_Stopwatch();

        protected override bool DoControl(object message)
		{
			var states = new List<State>();
			var err = "";

			lock (que) while (0 < que.Count) states.Add(que.Dequeue());

            if (0 < states.Count)
            {
                state_last = states.Last();

                if (!swupdate.IsRunning || 5000 <= swupdate.ElapsedMilliseconds)
                {
                    swupdate.Restart();

                    //データベース登録
                    var state = new AGV_STATE
                    {
                        AGV_ID = agv.id,
                        COMMAND_TIME = state_last.last_time,
                        ACTION = GetAction(state_last),
                        ORDER_NO = agv.req == null ? 0 : agv.req.order_no,
                        ORDER_SUB_NO = agv.req == null ? 0 : agv.req.order_sub_no,
                        BATTERY = state_last.bat,
                        BATTERY_STATE =
                            state_last.bat == 255 ? BatteryStateType.ERROR : BatteryStateType.NORMAL,
                        ACTION_MODE =
                            state_last.sta_runmode ? ActionModeType.A : ActionModeType.M,
                        MAP = state_last.map,
                        LOCATION_X = state_last.x,
                        LOCATION_Y = state_last.y,
                        RACK_ID = GetRackID(state_last),
                        ERR_CODE = state_last.error_code,
                    };

                    err = state.InsertUpdate();

                    if (err == "")
                    {
                        if (state_pre.error_code != state_last.error_code)
                        {
                            //異常状態の更新
                            if (state_last.error_code != 0) err = AgvDatabase.RaiseError(agv, state_last);
                            else err = AgvDatabase.ResetError(agv, state_last);
                        }
                    }

                    if (err == "")
                    {
                        //state_pre更新
                        state_pre.SetBytes(state_last.GetBytes());
                        state_pre.last_time = state_last.last_time;
                        order_no_pre = order_no;
                        order_sub_no_pre = order_sub_no;
                    }
                    else
                    {
                        if (Event_AgvDatabaseError != null) Event_AgvDatabaseError(this, err);
                    }
                }
            }
            else
            {
                if (IsOrderNoChanged())
                {
                    err = AGV_STATE.Select(agv.id, out AGV_STATE s);

                    if (err != "") goto ORDER_NO_END;

                    s.ORDER_NO = order_no;
                    s.ORDER_SUB_NO = order_sub_no;

                    err = s.InsertUpdate();

                    if (err != "") goto ORDER_NO_END;

                    order_no_pre = order_no;
                    order_sub_no_pre = order_sub_no;

                ORDER_NO_END:

                    if (err != "")
                    {
                        if (Event_AgvDatabaseError != null) Event_AgvDatabaseError(this, err);
                    }
                }
            }

            //if (0 < states.Count)
            //{
            //    foreach (var v in states)
			//    {
			//		err = "";

			//		//変化検出処理～データベース登録
			//		//...
			//		if (IsStateChanged(v))
			//		{
			//			var state = new AGV_STATE
			//			{
			//				AGV_ID = agv.id,
			//				COMMAND_TIME = v.last_time,
			//				ACTION = GetAction(v),
			//				ORDER_NO = agv.req is null ? 0 : agv.req.order_no,
			//				ORDER_SUB_NO = agv.req is null ? 0 : agv.req.order_sub_no,
			//				BATTERY = v.bat,
			//				BATTERY_STATE =
			//					v.bat == 255 ? BatteryStateType.ERROR : BatteryStateType.NORMAL,
			//				ACTION_MODE =
			//					v.sta_runmode ? ActionModeType.A : ActionModeType.M,
			//				MAP = v.map,
			//				LOCATION_X = v.x,
			//				LOCATION_Y = v.y,
			//				RACK_ID = GetRackID(v),
			//				ERR_CODE = v.error_code,
			//			};

			//			err = state.InsertUpdate();

			//			if (err != "") goto STATE_END;
			//		}

			//		if (state_pre.error_code != v.error_code)
			//		{
			//			//異常状態の更新
			//			if (v.error_code != 0) err = AgvDatabase.RaiseError(agv, v);
			//			else err = AgvDatabase.ResetError(agv, v);
			//		}

			//		STATE_END:

			//		if (err == "")
			//		{
			//			//state_pre更新
			//			state_pre.SetBytes(v.GetBytes());
			//			state_pre.last_time = v.last_time;
			//			order_no_pre = order_no;
			//			order_sub_no_pre = order_sub_no;
			//		}
			//		else
			//		{
			//			if (Event_AgvDatabaseError != null) Event_AgvDatabaseError(this, err);
			//		}
			//	}
			//}
			//else
			//{
			//	if (IsOrderNoChanged())
			//	{
			//		err = AGV_STATE.Select(agv.id, out AGV_STATE s);

			//		if (err != "") goto ORDER_NO_END;

			//		s.ORDER_NO = order_no;
			//		s.ORDER_SUB_NO = order_sub_no;

			//		err = s.InsertUpdate();

			//		if (err != "") goto ORDER_NO_END;

			//		order_no_pre = order_no;
			//		order_sub_no_pre = order_sub_no;

			//		ORDER_NO_END:

			//		if (err != "")
			//		{
			//			if (Event_AgvDatabaseError != null) Event_AgvDatabaseError(this, err);
			//		}
			//	}
			//}

			if (AgvDatabase.Enable)
			{
				if (!m_swTemp.IsRunning) m_swTemp.Restart();
				if (2000 < m_swTemp.ElapsedMilliseconds)
				{
					err = "";

					//...データベース監視・・・ダウン登録や復帰登録
					err = AGV_SYS_ORDER.Select(FinishType.INCOMPLETED, agv.id, out List<AGV_SYS_ORDER> sys_orders);

					if (err == "")
					{
						foreach (var v in sys_orders)
						{
							if (Event_AgvOrderDown != null) Event_AgvOrderDown(this, v);
						}
					}
					else
					{
						// TODO
					}

					//...データベース監視・・・動作指示の取得
					err = AGV_ORDER.Select(FinishType.INCOMPLETED, agv.id, out AGV_ORDER order);

					if (err == "")
					{
						if (order != null && last_order_id < order.ID)
						{
							last_order_id = order.ID;

							var action = "";

							switch (order.ST_ACTION)
							{
								case StActionType.NONE: action = "0"; break;
								case StActionType.LIFT_RACK: action = "1"; break;
								case StActionType.LOWER_RACK: action = "2"; break;
								case StActionType.START_CHARGING: action = "C"; break;
								case StActionType.STOP_CHARGING: action = "c"; break;
							}

							var req = new RequestMove
							{
								agv = order.AGV_ID,
								station = order.ST_TO,
								qr = order.TO_QR,
								rack_action = action,
								rack_no = "",
								rack_face = ((int)order.RACK_ANGLE).ToString(),
								run_mode = order.SENSOR.ToString(),
								run_music = ((int)order.MUSIC).ToString(),
								working = ((int)order.ORDER_MARK).ToString(),
								order_no = order.ORDER_NO,
								order_sub_no = order.ORDER_SUB_NO,
								order_op1 = order.ORDER_OP1,
								order_op2 = order.ORDER_OP2,
								order_op3 = order.ORDER_OP3,
								order_op4 = order.ORDER_OP4,
								order_op5 = order.ORDER_OP5,
								o_info1 = order.O_INFO1,
								o_info2 = order.O_INFO2,
								o_info3 = order.O_INFO3,
								o_info4 = order.O_INFO4,
								o_info5 = order.O_INFO5,

								result = "RQ",
							};

							if (Event_AgvOrder != null) Event_AgvOrder(this, req);
						}
					}
					else
					{
						// TODO
					}

					m_swTemp.Reset();
				}
			}

			return base.DoControl(message);
		}

		#endregion
	}
}
