using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

using BelicsClass.Common;

namespace BelicsClass.ObjectSync
{
    /// <summary>
    /// クラスインスタンスを共有メモリでアクセスを行うクラス
    /// 派生クラスではなく、ランタイムで任意のオブジェクトを対象にする場合、本クラスを利用します
    /// 抽象クラスTは公開デフォルトコンストラクタが定義されていなければなりません
    /// 
    /// 抽象クラスTのフィールドは本クラスのInitializeメソッドコールによって解析され、バイト配列とオブジェクト間の相互変換が行えるようになります
    /// 抽象クラスTでnull値はフィールドの初期値として使用できません
    /// 抽象クラスTでstringフィールドは、必要文字数分の空白で初期化してください
    /// 
    /// 抽象クラスTのフィールドには「下記の組み込み型、および下記で構成されたクラス、配列」を使用することができます
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
    public class BL_TypedFaceMemorySync<T> : BL_FaceMemorySync where T : new()
    {
        /// <summary>
        /// 対象オブジェクト
        /// </summary>
        [BL_ObjectSyncAttribute]
        public T Obj;

        /// <summary>
        /// コンストラクタ
        /// デフォルトコンストラクタでオブジェクトが生成されます
        /// 共有メモリは生成されません
        /// </summary>
        public BL_TypedFaceMemorySync()
        {
            Obj = new T();
        }

        /// <summary>
        /// コンストラクタ
        /// 指定オブジェクトで初期化します
        /// 共有メモリは生成されません
        /// </summary>
        /// <param name="obj">指定オブジェクト</param>
        public BL_TypedFaceMemorySync(T obj)
        {
            Obj = obj;
        }

        /// <summary>
        /// コンストラクタ
        /// デフォルトコンストラクタでオブジェクトが生成されます
        /// 指定名で共有メモリを生成します
        /// </summary>
        /// <param name="sharemem_name">共有メモリ名</param>
        public BL_TypedFaceMemorySync(string sharemem_name)
        {
            Obj = new T();
            this.Initialize(Obj, sharemem_name);
        }

        /// <summary>
        /// コンストラクタ
        /// 指定オブジェクトで初期化します
        /// 指定名で共有メモリを生成します
        /// </summary>
        /// <param name="obj">指定オブジェクト</param>
        /// <param name="sharemem_name">共有メモリ名</param>
        public BL_TypedFaceMemorySync(T obj, string sharemem_name)
            : this(obj, sharemem_name, false)
        {
            Obj = obj;
            this.Initialize(Obj, sharemem_name);
        }

        /// <summary>
        /// コンストラクタ
        /// 指定オブジェクトで初期化します
        /// 指定名で共有メモリを生成します
        /// インスタンスの破棄によって共有メモリを破棄するかしないかを指定できます
        /// </summary>
        /// <param name="obj">指定オブジェクト</param>
        /// <param name="sharemem_name">共有メモリ名</param>
        /// <param name="notrelease">共有メモリを破棄しない場合trueを指定してください</param>
        public BL_TypedFaceMemorySync(T obj, string sharemem_name, bool notrelease)
            : base(notrelease)
        {
            Obj = obj;
            this.Initialize(Obj, sharemem_name);
        }

        /// <summary>
        /// 初期化します
        /// 共有メモリは生成されません
        /// 自インスタンスが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// </summary>
        /// <returns>初期化成功でtrueを返します</returns>
        public override bool Initialize()
        {
            return this.Initialize(Obj, "");
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
        public override bool Initialize(string sharemem_name)
        {
            return this.Initialize(Obj, sharemem_name);
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
            Obj = (T)x;
            return this.Initialize(x, "");
        }

        /// <summary>
        /// 共有メモリから自インスタンスの全データを読み込みます
        /// 読み込み中はロックされます
        /// </summary>
        /// <returns>対象オブジェクト</returns>
        public new T ReadMemory()
        {
            if (base.ReadMemory() == null) return default(T);

            return Obj;
        }
    }
}
