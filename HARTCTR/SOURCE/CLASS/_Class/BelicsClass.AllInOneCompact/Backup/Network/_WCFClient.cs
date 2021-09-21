using System;
using System.ServiceModel;

namespace BelicsClass.Network
{
    /// <summary>
    /// 
    /// </summary>
    public class BL_WCFClient<T>
    {
        /// <summary>
        /// イベント定義
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        public delegate void EventHandler(object sender, CommunicationState state);
        /// <summary></summary>
        public virtual event EventHandler OnChangeState;
        
        private ChannelFactory<T> channel;
        private T server;

        /// <summary>
        /// 
        /// </summary>
        public T Connecter { get { return server; } }

        /// <summary>
        /// チャンネルを作成します
        /// </summary>
        /// <param name="address"></param>
        public void StartNamedPipe(string address)
        {
            CloseChannel();

            NetNamedPipeBinding bind = new NetNamedPipeBinding();
            bind.TransferMode = TransferMode.Streamed;
            bind.MaxReceivedMessageSize = long.MaxValue;
            channel = new ChannelFactory<T>(bind, new EndpointAddress(address));
            channel.Opening += channel_Opening;
            channel.Opened += channel_Opened;
            channel.Closing += channel_Closing;
            channel.Closed += channel_Closed;
            channel.Faulted += channel_Faulted;
            server = channel.CreateChannel();
        }

        /// <summary>
        /// チャンネルを作成します
        /// </summary>
        /// <param name="address"></param>
        public void StartTcp(string address)
        {
            CloseChannel();

            NetTcpBinding bind = new NetTcpBinding(SecurityMode.None);
            bind.TransferMode = TransferMode.Streamed;
            bind.MaxReceivedMessageSize = long.MaxValue;
            channel = new ChannelFactory<T>(bind, new EndpointAddress(address));
            channel.Opening += channel_Opening;
            channel.Opened += channel_Opened;
            channel.Closing += channel_Closing;
            channel.Closed += channel_Closed;
            channel.Faulted += channel_Faulted;
            server = channel.CreateChannel();
        }

        ///// <summary>
        ///// チャンネルを作成します
        ///// </summary>
        ///// <param name="address"></param>
        //public void StartBasicHttp(string address)
        //{
        //    CloseChannel();

        //    BasicHttpBinding bind = new BasicHttpBinding(BasicHttpSecurityMode.None);
        //    bind.TransferMode = TransferMode.Streamed;
        //    bind.MaxReceivedMessageSize = long.MaxValue;
        //    channel = new ChannelFactory<T>(bind, new EndpointAddress(address));
        //    channel.Opening += channel_Opening;
        //    channel.Opened += channel_Opened;
        //    channel.Closing += channel_Closing;
        //    channel.Closed += channel_Closed;
        //    channel.Faulted += channel_Faulted;
        //    server = channel.CreateChannel();
        //}

        /// <summary>
        /// チャンネルを閉じます
        /// </summary>
        public void CloseChannel()
        {
            if (channel != null)
            {
                //if (channel.State == CommunicationState.Opened) channel.Close();

                channel.Opening -= channel_Opening;
                channel.Opened -= channel_Opened;
                channel.Closing -= channel_Closing;
                channel.Closed -= channel_Closed;
                channel.Faulted += channel_Faulted;
                channel = null;
                
                if (OnChangeState != null) OnChangeState(this, CommunicationState.Closed);
            }
        }

        /// <summary>
        /// 接続状態を取得します
        /// </summary>
        public CommunicationState State { get { return (channel == null) ? CommunicationState.Faulted : channel.State; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void channel_Closed(object sender, EventArgs e)
        {
            if (OnChangeState != null) OnChangeState(this, CommunicationState.Closed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void channel_Closing(object sender, EventArgs e)
        {
            if (OnChangeState != null) OnChangeState(this, CommunicationState.Closing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void channel_Opened(object sender, EventArgs e)
        {
            if (OnChangeState != null) OnChangeState(this, CommunicationState.Opened);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void channel_Opening(object sender, EventArgs e)
        {
            if (OnChangeState != null) OnChangeState(this, CommunicationState.Opening);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void channel_Faulted(object sender, EventArgs e)
        {
            if (OnChangeState != null) OnChangeState(this, CommunicationState.Faulted);
        }
    }
}
