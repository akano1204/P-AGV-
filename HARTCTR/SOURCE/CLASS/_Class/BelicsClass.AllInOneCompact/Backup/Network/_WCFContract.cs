using System;
using System.ServiceModel;

namespace BelicsClass.Network
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ServiceContract]
    public interface BL_WCFContract<T>
    {
        //------------------------------------------------------
        //必要に応じて本インターフェースに機能を追加してください
        //------------------------------------------------------

        /// <summary>
        /// 抽象クラス T のインスタンス型を取得します
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Type GetInstanceType();

        /// <summary>
        /// データを取得するためのメソッドを実装してください
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        T Get();

        /// <summary>
        /// データを設定するためのメソッドを実装してください
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OperationContract]
        bool Put(T data);

        /// <summary>
        /// 更新をロックするためのメソッドを実装してください
        /// </summary>
        [OperationContract]
        void Lock();

        /// <summary>
        /// 更新をロック解除するためのメソッドを実装してください
        /// </summary>
        [OperationContract]
        void Unlock();
    }
}
