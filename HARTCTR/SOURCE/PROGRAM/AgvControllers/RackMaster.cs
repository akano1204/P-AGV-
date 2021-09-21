using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BelicsClass.File;

namespace PROGRAM
{
    using Rack = AgvControlManager.Rack;

    public class RackMaster
    {
        #region シングルトン

        private static RackMaster instance = new RackMaster();
        public static RackMaster Instance => instance;

        #endregion

        #region 定数

        public string INI_PATH = Path.Combine(Path.GetDirectoryName(Program.ini_hokusho.FullName), "RACK.INI");

        #endregion

        #region フィールド

        private BL_IniFile ini = null;

		private static int racksizemax = -1;
		private static int racksizemax_rotate = -1;

		#endregion

		#region プロパティ

		/// <summary>
		/// 棚の最大サイズを取得する
		/// </summary>
		public static int RackSizeMax
        {
			get
			{
				if (racksizemax < 0)
				{
					var maxrack = RackMaster.Instance.GetRackList().OrderByDescending(e => e.SizeMax).FirstOrDefault();
					if (maxrack != null)
					{
						racksizemax = maxrack.SizeMax;
					}
					else
					{
						racksizemax = Rack.DEFAULT_SIZE;
					}
				}

				return racksizemax;
			}
		}

		public static int RackRotateSize
		{
			get
			{
				if (racksizemax_rotate < 0 || racksizemax < 0)
				{
					racksizemax_rotate = (int)(Math.Sqrt(2.0) * (double)RackSizeMax);
				}

				return racksizemax_rotate;
			}
		}

        #endregion

        #region メソッド

        public Rack LoadRack(string no)
		{
			return GetRackFromFile(no) ?? Rack.GetDefault(no);
		}

		public Rack GetRackFromFile(string no, bool check_sections = true)
		{
			if (string.IsNullOrEmpty(no)) return null;

			if (check_sections)
			{
				var sections = ini.GetSectionNames().ToList();

				if (!sections.Contains(no))
				{
					return null;
				}
			}

			var face_id = new Dictionary<int, string>();
			var can_inout = new Dictionary<int, bool>();
			var can_move = new Dictionary<int, bool>();

			int id = 1;
			foreach (var deg in Rack.FACE_DEGREES)
			{
				face_id[deg] = id++.ToString();
				can_inout[deg] = ini.Get(no, $"CAN_INOUT_{deg}", true);
				can_move[deg] = ini.Get(no, $"CAN_MOVE_{deg}", true);
			}

			var sizeW = ini.Get(no, "SIZE_W", Rack.DEFAULT_SIZE);
			var sizeL = ini.Get(no, "SIZE_L", Rack.DEFAULT_SIZE);
			var anyface = ini.Get(no, "ANYFACE", false);
			var overhang = ini.Get(no, "OVERHANG", false);

			Rack ret = new Rack
			{
				rack_no = no,
				face_id = face_id,
				can_inout = can_inout,
				can_move = can_move,
				sizeW = sizeW,
				sizeL = sizeL,
				anyface = anyface,
				overhang = overhang,
			};

			return ret;
		}

		public List<Rack> GetRackList()
		{
			var ret = new List<Rack>();

			var sections = ini.GetSectionNames();

			foreach (var section in sections)
			{
				var rack = GetRackFromFile(section, false);

				if (rack != null)
				{
					ret.Add(rack);
				}
			}

			return ret;
		}

		public string Save(List<Rack> rack_collection)
		{
			var ret = "";

			if (File.Exists(INI_PATH))
			{
				try
				{
					File.Delete(INI_PATH);
				}
				catch (Exception e)
				{
					ret = $"RACK.INIの削除時に例外が発生しました。\n{e.Message}\n{e.InnerException}";
				}
			}

			foreach (var rack in rack_collection)
			{
				var no = rack.rack_no;

				foreach (var kv in rack.can_inout)
				{
					var deg = kv.Key;
					var value = kv.Value;

					ini.Write(no, $"CAN_INOUT_{deg}", value);
				}

				foreach (var kv in rack.can_move)
				{
					var deg = kv.Key;
					var value = kv.Value;

					ini.Write(no, $"CAN_MOVE_{deg}", value);
				}

				ini.Write(no, "SIZE_W", rack.sizeW);
				ini.Write(no, "SIZE_L", rack.sizeL);
				ini.Write(no, "ANYFACE", rack.anyface);
				ini.Write(no, "OVERHANG", rack.overhang);
			}

			racksizemax = -1;

            Program.controller.ListupConflictQR();

			return ret;
		}

		#endregion

		#region コンストラクタ

		private RackMaster()
		{
			if (!File.Exists(INI_PATH))
			{
				File.Create(INI_PATH);
			}

			ini = new BL_IniFile(INI_PATH);
		}

		#endregion


	}
}
