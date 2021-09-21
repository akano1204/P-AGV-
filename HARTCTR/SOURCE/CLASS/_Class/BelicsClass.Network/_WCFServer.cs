using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace BelicsClass.Network
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BL_WCFServer<T> : BL_WCFContract<T> where T : new()
    {
        /// <summary>
        /// データインスタンス
        /// 別途定義された取り扱いデータ用クラスのインスタンスです。
        /// </summary>
        public T Data = new T();

        /// <summary>データ更新をロックしているフラグ</summary>
        public bool IsLocked = false;

        #region サービス開始関連

        /// <summary>
        /// サーバー機能を開始します。
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="contactname"></param>
        public void StartNamedPipe(string uri, string contactname)
        {
            ServiceHost host = new ServiceHost(this, new Uri(uri));
            NetNamedPipeBinding bind = new NetNamedPipeBinding();
            bind.TransferMode = TransferMode.Streamed;
            bind.MaxReceivedMessageSize = long.MaxValue;
            host.AddServiceEndpoint(typeof(BL_WCFContract<T>), bind, contactname);
            host.Open();
        }

        /// <summary>
        /// サーバー機能を開始します。
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="contactname"></param>
        public void StartTcp(string uri, string contactname)
        {
            ServiceHost host = new ServiceHost(this, new Uri(uri));
            NetTcpBinding bind = new NetTcpBinding(SecurityMode.None);
            bind.TransferMode = TransferMode.Streamed;
            bind.MaxReceivedMessageSize = long.MaxValue;
            host.AddServiceEndpoint(typeof(BL_WCFContract<T>), bind, contactname);
            host.Open();
        }

        ///// <summary>
        ///// サーバー機能を開始します。
        ///// </summary>
        ///// <param name="uri"></param>
        ///// <param name="contactname"></param>
        //public void StartBasicHttp(string uri, string contactname)
        //{
        //    ServiceHost host = new ServiceHost(this, new Uri(uri));
        //    BasicHttpBinding bind = new BasicHttpBinding(BasicHttpSecurityMode.None);
        //    bind.TransferMode = TransferMode.Streamed;
        //    bind.MaxReceivedMessageSize = long.MaxValue;
        //    host.AddServiceEndpoint(typeof(BL_ServiceModelContract<T>), bind, contactname);
        //    host.Open();
        //}

        #endregion

        /// <summary>
        /// 抽象クラス T のインスタンス型を取得します
        /// </summary>
        /// <returns></returns>
        public Type GetInstanceType()
        {
            return typeof(T);
        }

        /// <summary>
        /// データ取得メソッド
        /// クライアントがサーバーからデータを取得する際に呼び出されます。
        /// オーバーライドして、データの取得処理を行うサーバー機能を実装してください。
        /// </summary>
        /// <returns></returns>
        virtual public T Get()
        {
            throw new NotSupportedException("データを取得するためのメソッドをオーバーライドで実装してください");
        }

        /// <summary>
        /// データ送信メソッド
        /// クライアントがサーバーのデータを更新する際に呼び出されます。
        /// オーバーライドして、データの設定処理を行うサーバー機能を実装してください。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        virtual public bool Put(T data)
        {
            throw new NotSupportedException("データを設定するためのメソッドをオーバーライドで実装してください");
        }

        /// <summary>
        /// 更新をロックします
        /// </summary>
        /// <returns></returns>
        virtual public void Lock()
        {
            IsLocked = true;
        }

        /// <summary>
        /// 更新をロック解除します
        /// </summary>
        /// <returns></returns>
        virtual public void Unlock()
        {
            IsLocked = false;
        }
    }
}
