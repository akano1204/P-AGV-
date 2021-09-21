using System;
using System.Collections.Generic;
using System.IO;

namespace BelicsClass.Common
{
    /// <summary>
    /// シリアライズ機能が付加されたListクラスです
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BL_BinarySerializableList<T> : List<T>
    {
        /// <summary>
        /// 自オブジェクトをファイルへシリアライズします
        /// 例外は発生しません(内部で例外処理が行われます)
        /// </summary>
        /// <param name="path">保存先パス</param>
        /// <returns>シリアライズが成功するとtrueを返します</returns>
        public bool Save(string path)
        {
            //try
            {
                MemoryStream ms = BL_BinarySerializer.SerializeToStream(this);

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    catch { }
                }

                lock (this)
                {
                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                    ms.WriteTo(fs);
                    fs.Close();
                    ms.Close();
                }
                return true;
            }
            //catch { }

            //return false;
        }

        /// <summary>
        /// ファイルからデシリアライズして自オブジェクトへ読み込みます
        /// 既存のデータはクリアされます
        /// </summary>
        /// <param name="path"></param>
        /// <returns>デシリアライズが成功するとtrueを返します</returns>
        public bool Load(string path)
        {
            try
            {
                lock (this)
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    BL_BinarySerializableList<T> newlist = BL_BinarySerializer.DeserializeFromStream<BL_BinarySerializableList<T>>(fs);
                    fs.Close();

                    this.Clear();
                    foreach (var v in newlist)
                    {
                        this.Add(v);
                    }
                }
                return true;
            }
            catch { }

            return false;
        }
    }
}
