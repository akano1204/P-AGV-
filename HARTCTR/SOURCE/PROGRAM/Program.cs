using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.ObjectSync;
using BelicsClass.Common;
using BelicsClass.UI;

namespace PROGRAM
{
    static class Program
    {
        /// <summary>
        /// 0.01    2020/06/01  P型AGV用経路管理ソフトとして新規作成
        /// 
        /// </summary>
        public static string VER = "VER.0.30";

        public static BelicsClass.File.BL_IniFile ini_hokusho = new BelicsClass.File.BL_IniFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\DATA\HOKUSHO.INI"));
        public static BelicsClass.File.BL_IniFile ini_agv = new BelicsClass.File.BL_IniFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\DATA\AGV.INI"));
        public static BelicsClass.File.BL_IniFile ini_rack = new BelicsClass.File.BL_IniFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\DATA\RACK.INI"));
        public static BelicsClass.File.BL_IniFile ini_autorator = new BelicsClass.File.BL_IniFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\DATA\AUTORATOR.INI"));

        private static BelicsClass.ProcessManage.BL_Only only = null;

        public static AgvControlManager controller = null;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            only = new BelicsClass.ProcessManage.BL_Only("A1000_PROGRAM_MAIN");
            if (only.IsOnly)
            {
                ini_hokusho.Set("CHECK", "STA", DateTime.Now);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                controller = new AgvControlManager();

                string data_dir = Path.GetDirectoryName(ini_hokusho.FullName);
                string[] files = Directory.GetFiles(data_dir, "*.lag");
                if (0 < files.Length)
                {
                    controller.Load(files[0]);
                    controller.Start(AgvControlManager.enApplicateMode.AUTO_COMMANDER, 0, 100);
                }

                string err = AgvDatabase.Initialize();
                if (err != "")
                {
                    BL_MessageBox.Show(err, "DB異常");
                }
                else
                {
                    Application.Run(new MainForm());
                }

                BelicsClass.ProcessManage.BL_ThreadCollector.StopControl_All();

                ini_hokusho.Write("MAP", "OFFSET_X", controller.draw_offset_pixel.X);
                ini_hokusho.Write("MAP", "OFFSET_Y", controller.draw_offset_pixel.Y);
                ini_hokusho.Write("MAP", "SCALE", controller.draw_scale);
                ini_hokusho.Write("CHECK", "END", DateTime.Now);

                only.Release();
            }
        }
    }

    [Serializable()]
    public class SynchronizedList<T> : IList<T>
    {
        private List<T> _list;
        private Object _root;

        internal SynchronizedList()
        {
            _list = new List<T>();
            _root = ((System.Collections.ICollection)_list).SyncRoot;
        }

        internal SynchronizedList(List<T> list)
        {
            _list = list;
            _root = ((System.Collections.ICollection)list).SyncRoot;
        }

        public int Count
        {
            get
            {
                lock (_root)
                {
                    return _list.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<T>)_list).IsReadOnly;
            }
        }

        public void Add(T item)
        {
            lock (_root)
            {
                _list.Add(item);
            }
        }

        public void AddRange(SynchronizedList<T> item)
        {
            lock (_root)
            {
                foreach(var v in item) _list.Add(v);
            }
        }

        public void Clear()
        {
            lock (_root)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_root)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_root)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (_root)
            {
                return _list.Remove(item);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (_root)
            {
                return _list.GetEnumerator();
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (_root)
            {
                return ((IEnumerable<T>)_list).GetEnumerator();
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_root)
                {
                    return _list[index];
                }
            }
            set
            {
                lock (_root)
                {
                    _list[index] = value;
                }
            }
        }

        public int IndexOf(T item)
        {
            lock (_root)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_root)
            {
                _list.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_root)
            {
                _list.RemoveAt(index);
            }
        }

        public List<T> GetRange(int from, int to)
        {
            List<T> ret = new List<T>();

            lock (_root)
            {
                for (int i = from; i < to && i < _list.Count; i++)
                {
                    ret.Add(_list[i]);
                }
            }

            return ret;
        }
    }
}
