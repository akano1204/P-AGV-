using System;
using System.Reflection;
using System.Collections.Generic;

using BelicsClass.Common;
using BelicsClass.ProcessManage;

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
    public class BL_FaceMemorySync : BL_XmlSync
    {
        /// <summary>
        /// 共有メモリへの書き込みが行われた時間を表すTick値を保持します
        /// </summary>
        [BL_ObjectSyncAttribute]
        public long Ticks = 0;

        private string sharemem_name = "";

        /// <summary>
        /// 共有メモリ名称を取得します
        /// </summary>
        public string SharememName { get { return sharemem_name; } }

        /// <summary>
        /// 共有メモリ操作インスタンス
        /// </summary>
        [NonSerialized]
        private BL_Memory mem = null;

        /// <summary>
        /// インスタンスが破棄される際に共有メモリを破棄するかどうかを表します
        /// </summary>
        private bool notrelease = false;

        /// <summary>
        /// 共有メモリを明示的に使用しない場合trueになります
        /// </summary>
        private bool nosharemem = false;

        /// <summary>
        /// 変化比較用ローカルバッファ
        /// </summary>
        private byte[] changedetector = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_FaceMemorySync() { }

        /// <summary>
        /// コンストラクタ
        /// インスタンスの破棄によって共有メモリを破棄するかしないかを指定できます
        /// </summary>
        /// <param name="notrelease">共有メモリを破棄しない場合trueを指定してください</param>
        public BL_FaceMemorySync(bool notrelease) { this.notrelease = notrelease; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nosharemem">共有メモリを明示的に利用しない場合trueを指定してください</param>
        /// <param name="notrelease">共有メモリを破棄しない場合trueを指定してください</param>
        public BL_FaceMemorySync(bool nosharemem, bool notrelease) { this.nosharemem = nosharemem; this.notrelease = notrelease; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SharememName + "@BL_FaceMemorySync";
        }

        /// <summary>
        /// 初期化します
        /// 共有メモリは生成されません
        /// 自インスタンスが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// </summary>
        /// <param name="x">自インスタンスの初期化時に反映させる初期化オブジェクト</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public override bool Initialize(object x)
        {
            return Initialize(x, "");
        }

        /// <summary>
        /// 初期化します
        /// 名前を指定することによって共有メモリが生成されます
        /// 同一名の共有メモリが既存の場合は、共有メモリからデータが読み出されて初期化されます
        /// 自インスタンスが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// </summary>
        /// <param name="sharemem_name">共有メモリの名称</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public virtual bool Initialize(string sharemem_name)
        {
            return Initialize(this, sharemem_name);
        }

        /// <summary>
        /// 初期化します
        /// 名前を指定することによって共有メモリが生成されます
        /// 同一名の共有メモリが既存の場合は、共有メモリからデータが読み出されて初期化されます
        /// 自インスタンスが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// </summary>
        /// <param name="x">自インスタンスの初期化時に反映させる初期化オブジェクト</param>
        /// <param name="sharemem_name">共有メモリの名称</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public virtual bool Initialize(object x, string sharemem_name)
        {
            this.sharemem_name = sharemem_name;
            bool status = false;

            base.Initialize(x);

#if WindowsCE
            status = true;
#else
            if (sharemem_name != "")
            {
                bool isexist = IsExist(sharemem_name);

                mem = new BL_Memory();
                if (mem.CreateMemory(sharemem_name, (uint)Length))
                {
                    if (isexist)
                    {
                        //ReadMemory(!mem.IsLock);
                        ReadMemory();
                    }

                    status = true;
                }
                else
                {
                    throw new NotSupportedException("共有メモリ生成エラー[" + sharemem_name + "][" + mem.ErrorMessage + "]");
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
            if (!notrelease)
            {
                if (mem != null)
                {
                    mem.ReleaseMemory();
                    mem = null;
                    nosharemem = true;
                }
            }

            base.Dispose();
        }

        /// <summary>
        /// 共有メモリを破棄します
        /// </summary>
        public void ReleaseMemory()
        {
            if (mem != null)
            {
                mem.ReleaseMemory();
                mem = null;
                nosharemem = true;
            }
        }

        /// <summary>
        /// 指定された名称の共有メモリが存在するかをチェックします
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsExist(string name)
        {
            BL_Memory m = new BL_Memory();
            return m.IsExist(name);
        }

        /// <summary>
        /// 自インスタンスの全データを共有メモリへ書き込みます
        /// 書き込み中はロックされます
        /// </summary>
        /// <returns>正常に書き込みできたらtrueを返します</returns>
        public bool WriteMemory()
        {
            return WriteMemory("", true);
        }

        /// <summary>
        /// 自インスタンスの全データを共有メモリへ書き込みます
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns>正常に書き込みできたらtrueを返します</returns>
        public bool WriteMemory(bool lockFlag)
        {
            return WriteMemory("", lockFlag);
        }

        /// <summary>
        /// 自インスタンスの指定フィールドデータを共有メモリへ書き込みます
        /// 書き込み中はロックされます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <returns>正常に書き込みできたらtrueを返します</returns>
        public bool WriteMemory(string field_name)
        {
            return WriteMemory(field_name, true);
        }

        /// <summary>
        /// 自インスタンスの指定フィールドデータを共有メモリへ書き込みます
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns>正常に書き込みできたらtrueを返します</returns>
        public virtual bool WriteMemory(string field_name, bool lockFlag)
        {
            bool ret = false;

            //lock (this)
            {
                if (dictField == null) return false;

                if (mem != null)
                {
                    if (field_name == "")
                    {
                        byte[] data = GetBytes();
                        mem.WriteMemory(0, data, (uint)data.Length, lockFlag);
                        if (mem.ErrorCode != 0)
                        {
                            return false;
                        }

                        Ticks = DateTime.Now.Ticks;
                        return true;
                    }
                    else if (dictField.ContainsKey(field_name))
                    {
                        BL_ObjectSync.BL_ObjectInformation oi = dictField[field_name];
                        byte[] buffer = oi.GetBuffer();

                        try
                        {
                            mem.WriteMemory(oi.Offset, buffer, (uint)oi.Length, lockFlag);
                            if (mem.ErrorCode != 0)
                            {
                                return false;
                            }

                            Ticks = DateTime.Now.Ticks;
                            ret = true;
                        }
                        catch
                        {
                            ret = false;
                        }
                    }
                    else if (dictClasses.ContainsKey(field_name))
                    {
                        if (lockFlag) Lock();
                        {
                            foreach (var kv in dictClasses)
                            {
                                if (0 == kv.Key.IndexOf(field_name))
                                {
                                    foreach (BL_ObjectSync.BL_ObjectInformation oi in kv.Value)
                                    {
                                        byte[] buffer = oi.GetBuffer();

                                        try
                                        {
                                            mem.WriteMemory(oi.Offset, buffer, (uint)oi.Length, false);
                                            if (mem.ErrorCode != 0)
                                            {
                                                if (lockFlag) Unlock();
                                                return false;
                                            }
                                        }
                                        catch
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }

                            Ticks = DateTime.Now.Ticks;
                            ret = true;
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
                    if (!nosharemem) throw new NotSupportedException("共有メモリが初期化されていません[" + field_name + "]");

                    ret = true;
                    Ticks = DateTime.Now.Ticks;
                }
            }
            return ret;
        }

        /// <summary>
        /// 指定フィールドデータを共有メモリへ書き込みます
        /// 書き込みデータを指定できます
        /// 書き込み中はロックされます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="val">書き込みデータ</param>
        /// <returns></returns>
        public bool WriteMemory(string field_name, object val)
        {
            return WriteMemory(field_name, val, true);
        }

        /// <summary>
        /// 指定フィールドデータを共有メモリへ書き込みます
        /// 書き込みデータを指定できます
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="val">書き込みデータ</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public bool WriteMemory(string field_name, object val, bool lockFlag)
        {
            bool ret = false;

            //lock (this)
            {
                if (dictField == null) return false;

                if (field_name == "")
                {
                    foreach (BL_ObjectSync.BL_ObjectInformation oi in dictField.Values)
                    {
                        byte[] buffer = oi.GetBuffer(val);
                        oi.SetBuffer(buffer);
                        ret = true;
                    }
                }
                else if (dictField.ContainsKey(field_name))
                {
                    BL_ObjectSync.BL_ObjectInformation pl = dictField[field_name];
                    byte[] buffer = pl.GetBuffer(val);
                    pl.SetBuffer(buffer);

                    ret = true;
                }
                else if (dictClasses.ContainsKey(field_name))
                {
                    foreach (BL_ObjectSync.BL_ObjectInformation oi in dictClasses[field_name])
                    {
                        byte[] buffer = oi.GetBuffer(val);
                        oi.SetBuffer(buffer);
                        ret = true;
                    }
                }
                else
                {
                    throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
                }

                if (ret)
                {
                    ret = WriteMemory(field_name, lockFlag);
                }
            }

            return ret;
        }

        /// <summary>
        /// 共有メモリから自インスタンスの全データを読み込みます
        /// 読み込み中はロックされます
        /// </summary>
        /// <returns></returns>
        public object ReadMemory()
        {
            return ReadMemory("", true);
        }

        /// <summary>
        /// 共有メモリから自インスタンスの全データを読み込みます
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public object ReadMemory(bool lockFlag)
        {
            return ReadMemory("", lockFlag);
        }

        /// <summary>
        /// 共有メモリから指定フィールドへデータを読み込みます
        /// 読み込み中はロックされます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <returns></returns>
        public object ReadMemory(string field_name)
        {
            return ReadMemory(field_name, true);
        }

        /// <summary>
        /// 共有メモリから指定フィールドへデータを読み込みます
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public virtual object ReadMemory(string field_name, bool lockFlag)
        {
            //lock (this)
            {
                //if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");
                if (dictField == null) return null;

                if (field_name == "")
                {
                    if (mem != null)
                    {
                        if (0 < Length)
                        {
                            byte[] data = mem.ReadMemory(0, (uint)Length, lockFlag);
                            if (mem.ErrorCode != 0)
                            {
                                throw new NotSupportedException("共有メモリアクセスエラー[" + field_name + "][" + mem.ErrorMessage + "]");
                            }

                            if (!SetBytes(data)) return null;
                        }
                        return this;
                    }
                    else
                    {
                        if (!nosharemem) throw new NotSupportedException("共有メモリが初期化されていません[" + field_name + "]");
                    }
                }
                else if (dictField.ContainsKey(field_name))
                {
                    BL_ObjectSync.BL_ObjectInformation pl = dictField[field_name];
                    if (mem != null)
                    {
                        if (0 < pl.Length)
                        {
                            byte[] buffer = mem.ReadMemory(pl.Offset, (uint)pl.Length, lockFlag);
                            if (mem.ErrorCode != 0)
                            {
                                throw new NotSupportedException("共有メモリアクセスエラー[" + field_name + "][" + mem.ErrorMessage + "]");
                            }

                            return pl.SetBuffer(buffer);
                        }

                        return pl.GetData();
                    }
                    else
                    {
                        if (!nosharemem) throw new NotSupportedException("共有メモリが初期化されていません[" + field_name + "]");
                    }
                }
                else if (dictClasses.ContainsKey(field_name))
                {
                    if (mem != null)
                    {
                        if (lockFlag) Lock();

                        foreach (var kv in dictClasses)
                        {
                            if (0 == kv.Key.IndexOf(field_name))
                            {
                                foreach (BL_ObjectSync.BL_ObjectInformation oi in kv.Value)
                                {
                                    if (0 < oi.Length)
                                    {
                                        byte[] buffer = mem.ReadMemory(oi.Offset, (uint)oi.Length, false);
                                        if (mem.ErrorCode != 0)
                                        {
                                            if (lockFlag) Unlock();
                                            throw new NotSupportedException("共有メモリアクセスエラー[" + field_name + "][" + mem.ErrorMessage + "]");
                                        }

                                        oi.SetBuffer(buffer);
                                    }
                                }
                            }
                        }

                        if (lockFlag) Unlock();
                    }
                    else
                    {
                        if (!nosharemem) throw new NotSupportedException("共有メモリが初期化されていません[" + field_name + "]");
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
        }

        /// <summary>
        /// 共有メモリから指定フィールドへデータを読み込みます
        /// 読み込んだ値は指定型の戻り値となります
        /// 読み込み中はロックされます
        /// </summary>
        /// <typeparam name="U">指定型</typeparam>
        /// <param name="field_name">指定フィールド</param>
        /// <returns></returns>
        public U ReadMemory<U>(string field_name)
        {
            return ReadMemory<U>(field_name, true);
        }

        /// <summary>
        /// 共有メモリから指定フィールドへデータを読み込みます
        /// 読み込んだ値は指定型の戻り値となります
        /// ロック有無を指定できます
        /// </summary>
        /// <typeparam name="U">指定型</typeparam>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public U ReadMemory<U>(string field_name, bool lockFlag)
        {
            object obj = ReadMemory(field_name, lockFlag);
            if (typeof(U).IsInstanceOfType(obj))
            {
                return (U)obj;
            }

            return default(U);
        }

        /// <summary>
        /// 共有メモリを意図的にロックします
        /// </summary>
        public virtual void Lock()
        {
            if (mem != null)
            {
                //if (!mem.IsLock) 
                    mem.Lock();
            }
        }

        /// <summary>
        /// 共有メモリを意図的にアンロックします
        /// </summary>
        public virtual void Unlock()
        {
            if (mem != null)
            {
                //if (mem.IsLock) 
                    mem.Unlock();
            }
        }

        ///// <summary>
        ///// 共有メモリのロック状態を取得します
        ///// </summary>
        //public bool IsLock
        //{
        //    get
        //    {
        //        if (mem != null) return mem.IsLock;
        //        return false;
        //    }
        //}

        /// <summary>
        /// 共有メモリ上のデータと自インスタンスデータが不一致かどうかをチェックします
        /// </summary>
        /// <returns>不一致の場合trueを返します</returns>
        public BL_ObjectInformation[] IsModified()
        {
            return IsModified("");
        }

        /// <summary>
        /// 共有メモリ上のデータと自インスタンスデータが不一致かどうかをチェックします
        /// 不一致を検出した最初のフィールド名を取得できます
        /// </summary>
        /// <param name="field_name">指定した場合、指定フィールドのみ不一致チェックします。不一致が検出されたフィールド名で更新されます</param>
        /// <returns>不一致の場合trueを返します</returns>
        public virtual BL_ObjectInformation[] IsModified(string field_name)
        {
            if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");
            if (mem == null) if (!nosharemem) throw new NotSupportedException("共有メモリが初期化されていません[" + field_name + "]");

            //List<string> modifiedfields = new List<string>();
            List<BL_ObjectInformation> modifiedobject = new List<BL_ObjectInformation>();
            if (changedetector == null) changedetector = new byte[Length];

            if (field_name == "")
            {
                if (0 < Length)
                {
                    byte[] data_s = mem.ReadMemory(0, (uint)Length, false);
                    if (mem.ErrorCode != 0)
                    {
                        throw new NotSupportedException("共有メモリアクセスエラー[" + field_name + "][" + mem.ErrorMessage + "]");
                    }

                    foreach (var v in dictField)
                    {
                        //if (0 == v.Value.Field.GetCustomAttributes(typeof(BL_ObjectSyncAttributeNoEvent), true).Length)
                        {
                            //if (v.Key != "Ticks")
                            {
                                for (int i = v.Value.Offset; i < v.Value.Offset + v.Value.Length; i++)
                                {
                                    if (data_s[i] != changedetector[i])
                                    {
                                        //modifiedfields.Add(v.Key);
                                        //v.Value.Key = v.Key;
                                        if (modifiedobject.IndexOf(v.Value) < 0) modifiedobject.Add(v.Value);

                                        Array.Copy(data_s, v.Value.Offset, changedetector, v.Value.Offset, v.Value.Length);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                return modifiedobject.ToArray();
            }
            else if (dictField.ContainsKey(field_name))
            {
                BL_ObjectSync.BL_ObjectInformation pl = dictField[field_name];
                //if (0 == pl.Field.GetCustomAttributes(typeof(BL_ObjectSyncAttributeNoEvent), true).Length)
                {
                    if (0 < pl.Length)
                    {
                        byte[] data_s = mem.ReadMemory(pl.Offset, (uint)pl.Length);
                        if (mem.ErrorCode != 0)
                        {
                            throw new NotSupportedException("共有メモリアクセスエラー[" + field_name + "][" + mem.ErrorMessage + "]");
                        }

                        for (int i = pl.Offset; i < pl.Offset + pl.Length; i++)
                        {
                            if (data_s[i] != changedetector[i])
                            {
                                //modifiedfields.Add(field_name);
                                if (modifiedobject.IndexOf(pl) < 0) modifiedobject.Add(pl);

                                Array.Copy(data_s, pl.Offset, changedetector, pl.Offset, pl.Length);
                                break;
                            }
                        }
                    }
                }
                return modifiedobject.ToArray();
            }
            else if (dictClasses.ContainsKey(field_name))
            {
                foreach (BL_ObjectSync.BL_ObjectInformation oi in dictClasses[field_name])
                {
                    //if (0 == oi.Field.GetCustomAttributes(typeof(BL_ObjectSyncAttributeNoEvent), true).Length)
                    {
                        if (0 < oi.Length)
                        {
                            byte[] data_s = mem.ReadMemory(oi.Offset, (uint)oi.Length, false);
                            if (mem.ErrorCode != 0)
                            {
                                throw new NotSupportedException("共有メモリアクセスエラー[" + field_name + "][" + mem.ErrorMessage + "]");
                            }

                            for (int i = oi.Offset; i < oi.Offset + oi.Length; i++)
                            {
                                if (data_s[i] != changedetector[i])
                                {
                                    //modifiedfields.Add(field_name);
                                    Array.Copy(data_s, oi.Offset, changedetector, oi.Offset, oi.Length);
                                    break;
                                }
                            }
                        }
                    }
                }
                return modifiedobject.ToArray();
            }
            else
            {
                throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
            }
        }

        /// <summary>
        /// 共有メモリ全体のバイト配列を取得します
        /// 読み込み中はロックされます
        /// </summary>
        /// <returns></returns>
        public byte[] GetMemory() { return GetMemory("", true); }

        /// <summary>
        /// 共有メモリ全体のバイト配列を取得します
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public byte[] GetMemory(bool lockFlag) { return GetMemory("", lockFlag); }

        /// <summary>
        /// 指定されたフィールドのデータを共有メモリから読み込んでバイト配列を取得します
        /// ロック有無を指定できます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public virtual byte[] GetMemory(string field_name, bool lockFlag)
        {
            if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");
            if (mem == null) throw new NotSupportedException("共有メモリが初期化されていません[" + field_name + "]");

            byte[] data = null;
            if (field_name == "")
            {
                data = mem.ReadMemory(0, (uint)Length, lockFlag);
                if (mem.ErrorCode != 0)
                {
                    throw new NotSupportedException("共有メモリアクセスエラー[" + field_name + "][" + mem.ErrorMessage + "]");
                }
            }
            else if (dictField.ContainsKey(field_name))
            {
                BL_ObjectSync.BL_ObjectInformation pl = dictField[field_name];
                if (mem != null)
                {
                    data = mem.ReadMemory(pl.Offset, (uint)pl.Length, lockFlag);
                    if (mem.ErrorCode != 0)
                    {
                        throw new NotSupportedException("共有メモリアクセスエラー[" + field_name + "][" + mem.ErrorMessage + "]");
                    }
                }
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
                    byte[] data2 = mem.ReadMemory(oi.Offset, (uint)oi.Length, lockFlag);
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
    }
}
