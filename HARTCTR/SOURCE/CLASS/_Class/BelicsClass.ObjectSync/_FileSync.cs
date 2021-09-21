using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace BelicsClass.ObjectSync
{
    /// <summary>
    /// クラスインスタンスを共有メモリでアクセスを行うクラス
    /// 
    /// 本クラスから派生して、共有アクセスを行いたいフィールドが定義されたクラスを設計してください
    /// フィールドは本クラスのInitializeメソッドコールによって解析され、バイト配列とオブジェクト間の相互変換が行われます
    /// null値はフィールドの初期値として使用できません
    /// stringフィールドは、必要文字数分の空白で初期化してください
    /// 
    /// フィールドには「下記の組み込み型、および下記で構成されたクラス、配列」を使用することができます
    /// Boolean     4byte
    /// Int16       2byte
    /// Int32       4byte
    /// Int64       8byte
    /// UInt16      2byte
    /// UInt32      4byte
    /// UInt64      8byte
    /// Double      8byte
    /// Single      4byte
    /// Byte        1byte
    /// Char        1byte
    /// String      ??? byte
    /// 
    /// </summary>
    [Serializable]
    public class BL_FileSync : BL_FaceMemorySync
    {
        private string file_name = "";

        /// <summary>
        /// 共有メモリ名称を取得します
        /// </summary>
        public string FileName { get { return file_name; } }

        [NonSerialized]
        private FileStream File_stream = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_FileSync() { }

        /// <summary>
        /// 初期化します
        /// 名前を指定することによってファイルが生成されます
        /// 同一名の共有メモリが既存の場合は、共有メモリからデータが読み出されて初期化されます
        /// 自インスタンスが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// </summary>
        /// <param name="file_name">ファイル名</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public override bool Initialize(string file_name)
        {
            return Initialize(this, file_name);
        }

        /// <summary>
        /// 初期化します
        /// 名前を指定することによってファイルが生成されます
        /// 同一名のファイルが既存の場合は、ファイルからデータが読み出されて初期化されます
        /// 自インスタンスが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// </summary>
        /// <param name="x">自インスタンスの初期化時に反映させる初期化オブジェクト</param>
        /// <param name="file_name">共有メモリの名称</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public override bool Initialize(object x, string file_name)
        {
            this.file_name = file_name;
            bool status = false;

            base.Initialize(x, "");

#if WindowsCE
            status = true;
#else
            if (file_name != "")
            {
                bool isexist = false;

                isexist = System.IO.File.Exists(file_name);

                try
                {
                    File_stream = new FileStream(file_name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                    if (isexist)
                    {
                        ReadMemory();
                    }
                    else
                    {
                        WriteMemory();
                    }

                    status = true;
                }
                catch (Exception e)
                {
                    throw new NotSupportedException("ファイル生成エラー[" + file_name + "][" + e.Message + "]");
                }
            }
#endif
            return status;
        }

        /// <summary>
        /// 自インスタンスが保持している情報を破棄します
        /// </summary>
        public override void Dispose()
        {
            if (File_stream != null)
            {
                File_stream.Close();
                File_stream.Dispose();
                File_stream = null;
            }

            base.Dispose();
        }

        /// <summary>
        /// 指定された名称のファイルが存在するかをチェックします
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new static bool IsExist(string name)
        {
            return System.IO.File.Exists(name);
        }

        /// <summary>
        /// 指定されたフィールドのデータをファイルから読み込んでバイト配列を取得します
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public override byte[] GetMemory(string field_name, bool lockFlag)
        {
            if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");
            if (File_stream == null) throw new NotSupportedException("ファイルが初期化されていません[" + field_name + "]");

            byte[] data = null;
            if (field_name == "")
            {
                data = new byte[Length];
                File_stream.Position = 0;
                File_stream.Read(data, 0, Length);
            }
            else if (dictField.ContainsKey(field_name))
            {
                BL_ObjectSync.BL_ObjectInformation pl = dictField[field_name];
                data = new byte[pl.Length];
                File_stream.Position = pl.Offset;
                File_stream.Read(data, 0, pl.Length);
            }
            else if (dictClasses.ContainsKey(field_name))
            {
                int bytes = 0;
                foreach (BL_ObjectSync.BL_ObjectInformation oi in dictClasses[field_name])
                {
                    bytes += oi.Length;
                }

                data = new byte[bytes];

                int pos = 0;
                foreach (BL_ObjectSync.BL_ObjectInformation oi in dictClasses[field_name])
                {
                    byte[] data2 = new byte[oi.Length];
                    File_stream.Position = oi.Offset;
                    File_stream.Read(data2, 0, oi.Length);
                    Array.Copy(data2, 0, data, pos, oi.Length);
                    pos += oi.Length;
                }
            }
            else
            {
                throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
            }

            return data;
        }

        /// <summary>
        /// ファイルから指定フィールドへデータを読み込みます
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public override object ReadMemory(string field_name, bool lockFlag)
        {
            if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");

            if (field_name == "")
            {
                if (File_stream != null)
                {
                    if (lockFlag) Lock();
                    byte[] data = new byte[Length];
                    File_stream.Position = 0;
                    File_stream.Read(data, 0, Length);
                    if (lockFlag) Unlock();

                    if (!SetBytes(data)) return null;
                    return this;
                }
                else
                {
                    throw new NotSupportedException("ファイルが初期化されていません[" + field_name + "]");
                }
            }
            else if (dictField.ContainsKey(field_name))
            {
                BL_ObjectSync.BL_ObjectInformation pl = dictField[field_name];
                if (File_stream != null)
                {
                    if (lockFlag) Lock();

                    byte[] buffer = new byte[pl.Length];
                    File_stream.Position = pl.Offset;
                    File_stream.Read(buffer, 0, pl.Length);

                    if (lockFlag) Unlock();

                    return pl.SetBuffer(buffer);
                }
                else
                {
                    throw new NotSupportedException("ファイルが初期化されていません[" + field_name + "]");
                }
            }
            else if (dictClasses.ContainsKey(field_name))
            {
                if (File_stream != null)
                {
                    if (lockFlag) Lock();

                    foreach (BL_ObjectSync.BL_ObjectInformation oi in dictClasses[field_name])
                    {
                        byte[] buffer = new byte[oi.Length];
                        File_stream.Position = oi.Offset;
                        File_stream.Read(buffer, 0, oi.Length);
                        oi.SetBuffer(buffer);
                    }

                    if (lockFlag) Unlock();
                }
                else
                {
                    throw new NotSupportedException("ファイルが初期化されていません[" + field_name + "]");
                }
            }
            else
            {
                throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
            }

            string[] nest = field_name.Split('.');
            object obj = this;
            Type t = this.GetType();
            foreach (string field in nest)
            {
                string[] array = field.Split('[', ']');
                if (1 < array.Length)
                {
                    int index = int.Parse(array[1]);
                    FieldInfo f = t.GetField(array[0]);
                    obj = f.GetValue(obj);
                    obj = ((Array)obj).GetValue(index);
                }
                else
                {
                    FieldInfo f = t.GetField(field);
                    if (f != null) obj = f.GetValue(obj);
                    else
                    {
                        obj = null;
                        break;
                    }
                }

                if (obj != null) t = obj.GetType();
            }

            return obj;
        }

        /// <summary>
        /// 自インスタンスの指定フィールドデータをファイルへ書き込みます
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns>正常に書き込みできたらtrueを返します</returns>
        public override bool WriteMemory(string field_name, bool lockFlag)
        {
            bool ret = true;

            if (dictField == null) return false;

            if (File_stream != null)
            {
                if (field_name == "")
                {
                    if (lockFlag) Lock();
                    {
                        byte[] data = GetBytes();
                        File_stream.Position = 0;
                        File_stream.Write(data, 0, Length);
                        File_stream.Flush();
                    }
                    if (lockFlag) Unlock();

                    Ticks = DateTime.Now.Ticks;
                }
                else if (dictField.ContainsKey(field_name))
                {
                    BL_ObjectSync.BL_ObjectInformation oi = dictField[field_name];
                    byte[] buffer = oi.GetBuffer();

                    if (lockFlag) Lock();
                    {
                        try
                        {
                            File_stream.Position = oi.Offset;
                            File_stream.Write(buffer, 0, oi.Length);
                            File_stream.Flush();

                            Ticks = DateTime.Now.Ticks;
                        }
                        catch
                        {
                            ret = false;
                        }
                    }
                    if (lockFlag) Unlock();
                }
                else if (dictClasses.ContainsKey(field_name))
                {
                    if (lockFlag) Lock();
                    {
                        foreach (BL_ObjectSync.BL_ObjectInformation oi in dictClasses[field_name])
                        {
                            byte[] buffer = oi.GetBuffer();

                            try
                            {
                                File_stream.Position = oi.Offset;
                                File_stream.Write(buffer, 0, oi.Length);
                            }
                            catch
                            {
                                ret = false;
                                break;
                            }
                        }

                        File_stream.Flush();

                        if (ret)
                        {
                            Ticks = DateTime.Now.Ticks;
                        }
                    }
                    if (lockFlag) Unlock();
                }
                else
                {
                    throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
                }
            }
            else
            {
                throw new NotSupportedException("ファイルが初期化されていません[" + field_name + "]");
            }

            return ret;
        }

        /// <summary>
        /// ファイルを意図的にロックします
        /// </summary>
        public override void Lock()
        {
            File_stream.Lock(0, Length);
        }

        /// <summary>
        /// ファイルを意図的にアンロックします
        /// </summary>
        public override void Unlock()
        {
            File_stream.Unlock(0, Length);
        }

        /// <summary>
        /// ファイル内のデータと自インスタンスデータが不一致かどうかをチェックします
        /// 不一致を検出した最初のフィールド名を取得できます
        /// </summary>
        /// <param name="field_name">指定した場合、指定フィールドのみ不一致チェックします。不一致が検出されたフィールド名で更新されます</param>
        /// <returns>不一致の場合trueを返します</returns>
        public override BL_ObjectInformation[] IsModified(string field_name)
        {
            if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");
            if (File_stream == null) throw new NotSupportedException("ファイルが初期化されていません[" + field_name + "]");

            //List<string> modifiedfields = new List<string>();
            List<BL_ObjectInformation> modifiedobject = new List<BL_ObjectInformation>();

            if (field_name == "")
            {
                byte[] data_s = new byte[Length];
                File_stream.Position = 0;
                File_stream.Read(data_s, 0, Length);

                byte[] data_c = GetBytes();

                if (data_c.Length != data_s.Length) throw new NotSupportedException("ファイルサイズが異なります[" + data_c.Length.ToString() + ":" + data_s.Length.ToString() + "]");

                for (int i = 0; i < data_s.Length; i++)
                {
                    if (data_s[i] != data_c[i])
                    {
                        foreach (var v in dictField)
                        {
                            if (v.Value.Offset <= i && i < v.Value.Offset + v.Value.Length)
                            {
                                //modifiedfields.Add(v.Key);
                                if (modifiedobject.IndexOf(v.Value) < 0) modifiedobject.Add(v.Value);
                                //field_name = v.Key;
                                //return true;
                            }
                        }
                    }
                }

                return modifiedobject.ToArray();
            }
            else if (dictField.ContainsKey(field_name))
            {
                BL_ObjectSync.BL_ObjectInformation pl = dictField[field_name];
                byte[] data_s = new byte[pl.Length];
                File_stream.Position = pl.Offset;
                File_stream.Read(data_s, 0, pl.Length);

                byte[] data_c = pl.GetBuffer();

                bool same = true;
                for (int i = 0; i < data_s.Length; i++) if (data_s[i] != data_c[i]) { same = false; break; }
                if (!same)

                //if (!data_s.SequenceEqual(data_c))
                {
                    //modifiedfields.Add(field_name);
                    if (modifiedobject.IndexOf(pl) < 0) modifiedobject.Add(pl);
                }

                return modifiedobject.ToArray();
            }
            else if (dictClasses.ContainsKey(field_name))
            {
                foreach (BL_ObjectSync.BL_ObjectInformation oi in dictClasses[field_name])
                {
                    byte[] data_s = new byte[oi.Length];
                    File_stream.Position = oi.Offset;
                    File_stream.Read(data_s, 0, oi.Length);

                    byte[] data_c = oi.GetBuffer();

                    bool same = true;
                    for (int i = 0; i < data_s.Length; i++) if (data_s[i] != data_c[i]) { same = false; break; }
                    if (!same)
                    //if (!data_s.SequenceEqual(data_c))
                    {
                        //modifiedfields.Add(field_name);
                        if (modifiedobject.IndexOf(oi) < 0) modifiedobject.Add(oi);
                        break;
                    }
                }
                return modifiedobject.ToArray();
            }
            else
            {
                throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
            }
        }
    }
}
