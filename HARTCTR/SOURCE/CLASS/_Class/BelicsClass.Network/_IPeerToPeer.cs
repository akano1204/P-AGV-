using System;
using System.Net;

namespace BelicsClass.Network
{
    /// <summary>
    /// ソケットを使用したピア・ツー・ピア通信のinterfaceです。
    /// </summary>
    public interface BL_IPeerToPeer : IDisposable
    {
        /// <summary>
        /// 指定したデータを送信します。<b/>
        /// 本メソッドは直ちに制御を返します。
        /// </summary>
        /// <param name="data">送信するデータを指定します。</param>
        /// <returns>通信相手とリンクできていない場合falseを返し、
        /// リンクできている場合trueを返します。<b/>
        /// 送信ができたことを意味していません。</returns>
        bool Send(byte[] data);

        /// <summary>
        /// 指定したデータを送信します。<b/>
        /// 本メソッドは直ちに制御を返します。
        /// </summary>
        /// <param name="data">送信するデータを指定します。</param>
        /// <param name="offset">dataの送信開始オフセットを指定します。</param>
        /// <param name="length">送信するデータ長(byte)を指定します。</param>
        /// <returns>
        /// 通信相手に送信する準備が整っていない場合falseを返します。<b/>
        /// 通信相手に送信する準備が整った場合trueを返します。<b/>
        /// 戻り値は送信ができたことを意味しません。</returns>
        bool Send(byte[] data, int offset, int length);

        /// <summary>
        /// 受信データを取得します。<b/>
        /// 本メソッドは直ちに制御を返します。
        /// </summary>
        /// <param name="data">受信データを返します。<b/>
        /// 受信データが無い場合、nullを返します。</param>
        /// <returns>受信データがある場合trueを返し、無い場合falseを返します。</returns>
        bool Receive(out byte[] data);

        /// <summary>
        /// 受信データを取得します。<b/>
        /// 本メソッドは直ちに制御を返します。
        /// </summary>
        /// <returns>
        /// 受信データを返します。<b/>
        /// 受信データが無い場合、nullを返します。
        /// </returns>
        byte[] Receive();

        /// <summary>
        /// 受信イベントハンドルを取得します。
        /// </summary>
        System.Threading.WaitHandle WaitHandle { get; }

        /// <summary>
        /// 通信相手とのリンク状態を取得します。
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// 通信処理を開始しているか否かを取得します。<b/>
        /// 本プロパティがtrueであっても接続が確立しているとは限りません。
        /// </summary>
        bool Started { get; }

        /// <summary>
        /// 通信処理を開始します。<b/>
        /// 相手装置と接続していない場合は、接続できるまで接続を試みます。
        /// 相手装置と接続済みの場合は、接続を継続し何もしません。
        /// </summary>
        void StartLink();

        /// <summary>
        /// 通信処理を終了します。<b/>
        /// 相手装置と接続済みの場合は、接続を切断します。
        /// 相手装置と接続していない場合は、何もしません。
        /// 切断後、StartLinkをコールされるまで接続を行いません。
        /// </summary>
        void EndLink();

        /// <summary>
        /// ローカルIPアドレスを取得します。
        /// </summary>
        IPAddress LocalIPAddress { get; }

        /// <summary>
        /// ローカルポート番号を取得します。
        /// </summary>
        int LocalPort { get; }

        /// <summary>
        /// リモートIPアドレスを取得します。
        /// </summary>
        IPAddress RemoteIPAddress { get; }

        /// <summary>
        /// リモートポート番号を取得します。
        /// </summary>
        int RemotePort { get; }
    }
}
