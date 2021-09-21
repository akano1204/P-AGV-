#define VER12
#define VER14

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;

using BelicsClass.Common;
using BelicsClass.File;
using BelicsClass.ObjectSync;
using BelicsClass.Network;
using BelicsClass.ProcessManage;
using System.Threading;

namespace AgvController
{
    #region enum

    public enum enSpeed : int
    {
        L = 0x01,
        ML = 0x01 | 0x02,
        M = 0x02,
        HM = 0x02 | 0x04,
        H = 0x04,
    }

    #endregion

    public class AgvCommunicator : BL_ThreadController_Base
    {
        #region イベント

        public class ReceiveEventArgs : EventArgs
        {
            public IPEndPoint remoteEP = new IPEndPoint(0, 0);
            public State state = new State();

            public ReceiveEventArgs(State state, IPEndPoint ep)
            {
                remoteEP = ep;
                this.state.SetBytes(state.GetBytes());

                this.state.last_time = state.last_time;
            }
        }

        public delegate void ReceiveEventHandler(AgvCommunicator sender, ReceiveEventArgs e);

        public event ReceiveEventHandler ReceiveEvent;

        #endregion

        #region 内部クラス

        /// <summary>
        /// PLC→PC
        /// </summary>
        public class State : BL_ObjectSync
        {
            public enum CMD : UInt16
            {
                /// <summary>状態報告</summary>
                STATE = 0x01,
                /// <summary>行き先要求</summary>
                REQUEST = 0x02,
                /// <summary>経路指示応答</summary>
                RES_ORDER = 0x05,
                /// <summary>経路キャンセル応答</summary>
                RES_ROUTE_CANCEL = 0x06,
                ///// <summary>手動指示応答</summary>
                //RES_MANUAL = 0x07,
                /// <summary>キャリブレーション応答</summary>
                RES_CALIBRATION = 0x08,
                /// <summary>充電停止応答</summary>
                RES_CHARGE_STOP = 0x0B,
                /// <summary>異常復帰応答</summary>
                RES_ERROR_RESET = 0x0C,
                ///// <summary>スリープ応答</summary>
                //RES_SLEEP = 0x0D,
                ///// <summary>緊急停止応答</summary>
                //RES_EMG_STOP = 0x0E,
            }

            public enum STA : ushort
            {
                ///<summary>BIT0 一時停止中</summary>
                STOP = 0x0001,
                ///<summary>BIT1 棚無異常</summary>
                NORACK = 0x0002,
                ///<summary>BIT2 棚積載中</summary>
                RACK = 0x0004,
                ///<summary>BIT3 充電中</summary>
                CHARGE = 0x0008,
                ///<summary>BIT4 運転モード</summary>
                RUN_MODE = 0x0010,
                ///<summary>BIT5 BMP SW</summary>
                BMP_SW = 0x0020,
                ///<summary>BIT6 ターンテーブル上限</summary>
                TURNTABLE_UPPER = 0x0040,
                ///<summary>BIT7 ターンテーブル下限</summary>
                TURNTABLE_LOWER = 0x0080,

                //上位８ビットは異常コード
            }

            /// <summary>一時停止中</summary>
            public bool sta_stop { get { return (sta & (UInt16)STA.STOP) != 0; } set { sta = value ? (UInt16)(sta | (UInt16)STA.STOP) : (UInt16)(sta & ~(UInt16)STA.STOP); } }

            /// <summary>ラック無異常</summary>
            public bool sta_norack { get { return (sta & (UInt16)STA.NORACK) != 0; } set { sta = value ? (UInt16)(sta | (UInt16)STA.NORACK) : (UInt16)(sta & ~(UInt16)STA.NORACK); } }

            /// <summary>ラック積載中</summary>
            public bool sta_rack { get { return (sta & (UInt16)STA.RACK) != 0; } set { sta = value ? (UInt16)(sta | (UInt16)STA.RACK) : (UInt16)(sta & ~(UInt16)STA.RACK); } }

            ///<summary>充電中</summary>
            public bool sta_charge { get { return (sta & (UInt16)STA.CHARGE) != 0; } set { sta = value ? (UInt16)(sta | (UInt16)STA.CHARGE) : (UInt16)(sta & ~(UInt16)STA.CHARGE); } }

            ///<summary>自動運転中</summary>
            public bool sta_runmode { get { return (sta & (UInt16)STA.RUN_MODE) != 0; } set { sta = value ? (UInt16)(sta | (UInt16)STA.RUN_MODE) : (UInt16)(sta & ~(UInt16)STA.RUN_MODE); } }

            ///<summary>BMP SW</summary>
            public bool sta_bmpsw { get { return (sta & (UInt16)STA.BMP_SW) != 0; } set { sta = value ? (UInt16)(sta | (UInt16)STA.BMP_SW) : (UInt16)(sta & ~(UInt16)STA.BMP_SW); } }

            ///<summary>ターンテーブル上限</summary>
            public bool sta_turntable_upper { get { return (sta & (UInt16)STA.TURNTABLE_UPPER) != 0; } set { sta = value ? (UInt16)(sta | (UInt16)STA.TURNTABLE_UPPER) : (UInt16)(sta & ~(UInt16)STA.TURNTABLE_UPPER); } }

            ///<summary>ターンテーブル下限</summary>
            public bool sta_turntable_lower { get { return (sta & (UInt16)STA.TURNTABLE_LOWER) != 0; } set { sta = value ? (UInt16)(sta | (UInt16)STA.TURNTABLE_LOWER) : (UInt16)(sta & ~(UInt16)STA.TURNTABLE_LOWER); } }

            /// <summary>異常コード</summary>
            public byte error_code { get { return (byte)((sta & 0xff00) >> 8); } set { sta = (UInt16)((sta & 0x00ff) | ((UInt16)value << 8)); } }

            /// <summary>CMD</summary>
            [BL_ObjectSync]
            public UInt16 cmd = 0;
            /// <summary>STA</summary>
            [BL_ObjectSync]
            public UInt16 sta = 0;
            /// <summary>BAT</summary>
            [BL_ObjectSync]
            public byte bat = 255;
            /// <summary>MAP</summary>
            [BL_ObjectSync]
            public string map = " ";
            /// <summary>DEG</summary>
            [BL_ObjectSync]
            public Int16 deg = 0;
            /// <summary>XPOS</summary>
            [BL_ObjectSync]
            public Int32 x = 0;
            /// <summary>YPOS</summary>
            [BL_ObjectSync]
            public Int32 y = 0;
            /// <summary>RACK DEG</summary>
            [BL_ObjectSync]
            public Int16 rack_deg = 0;
            /// <summary>RACK No</summary>
            [BL_ObjectSync]
            public UInt16 rack_no = 0;

            [BL_ObjectSync]
            public Int16 opt_x = 0;
            [BL_ObjectSync]
            public Int16 opt_y = 0;
            [BL_ObjectSync]
            public Int16 opt_deg = 0;
            [BL_ObjectSync]
            public Int16 opt_rack_x = 0;
            [BL_ObjectSync]
            public Int16 opt_rack_y = 0;
            [BL_ObjectSync]
            public Int16 opt_rack_deg = 0;
            [BL_ObjectSync]
            public byte opt_speed = 0;
            [BL_ObjectSync]
            public byte opt_lmod = 0;
            [BL_ObjectSync]
            public string racktype = " ";
            [BL_ObjectSync]
            public byte[] reserve1 = new byte[1];
            [BL_ObjectSync]
            public UInt16 seq_no = 0;
            [BL_ObjectSync]
            public byte[] reserve2 = new byte[2];


            public string _map = " ";
            public Int32 _x = 0;
            public Int32 _y = 0;
            public DateTime last_time = new DateTime(0);

            public State()
            {
                Initialize();
            }

            public override string ToString()
            {
                string ret = ",R[";

                ret += ((CMD)cmd).ToString().PadRight(20);

                ret += ",SEQ_NO=" + seq_no.ToString().PadRight(6);
                ret += ",MAP=" + map.PadRight(1);
                ret += ",X=" + x.ToString().PadRight(6);
                ret += ",Y=" + y.ToString().PadRight(6);
                ret += ",DEG=" + deg.ToString().PadRight(4);
                ret += ",RDEG=" + rack_deg.ToString().PadRight(4);
                ret += ",RNO=" + rack_no.ToString().PadRight(5);
                ret += ",RTP=" + racktype.ToString().PadRight(1);

                ret += error_code != 0 ? "," + error_code.ToString("0000") : ",    ";
                ret += sta_stop ? ",STOP" : ",    ";
                ret += sta_norack ? ",NORACK" : ",      ";
                ret += sta_rack ? ",RACK" : ",    ";
                ret += sta_charge ? ",CHARGE" : ",      ";
                ret += sta_runmode ? ",AUTO" : ",MANU";
                ret += sta_bmpsw ? ",BMPSW" : ",     ";
                ret += sta_turntable_upper ? ",TU" : ",  ";
                ret += sta_turntable_lower ? ",TL" : ",  ";

                ret += ",OPT_X=" + opt_x.ToString().PadRight(6);
                ret += ",OPT_Y=" + opt_y.ToString().PadRight(6);
                ret += ",OPT_D=" + opt_deg.ToString().PadRight(6);
                ret += ",OPT_RX=" + opt_rack_x.ToString().PadRight(6);
                ret += ",OPT_RY=" + opt_rack_y.ToString().PadRight(6);
                ret += ",OPT_RD=" + opt_rack_deg.ToString().PadRight(6);
                ret += ",OPT_SPEED=" + opt_speed.ToString().PadRight(6);
                ret += ",OPT_LMOD=" + opt_lmod.ToString().PadRight(6);

                ret += ",BAT=" + bat.ToString().PadRight(4);

                ret += "]";

                return ret;
            }

            public Point Location { get { return new Point(x, y); } }

            public bool IsChanged(State pre)
            {
                if (pre.cmd != cmd) return true;
                if (pre.sta != sta) return true;
                if (pre.deg != deg) return true;
                if (pre.error_code != error_code) return true;
                if (pre.Location != Location) return true;
                if (pre.map != map) return true;
                if (pre.rack_deg != rack_deg) return true;
                if (pre.rack_no != rack_no) return true;
                if (pre.bat != bat) return true;

                if (pre.opt_x != opt_x) return true;
                if (pre.opt_y != opt_y) return true;
                if (pre.opt_deg != opt_deg) return true;
                if (pre.opt_rack_x != opt_rack_x) return true;
                if (pre.opt_rack_y != opt_rack_y) return true;
                if (pre.opt_rack_deg != opt_rack_deg) return true;
                if (pre.opt_speed != opt_speed) return true;
                if (pre.opt_lmod != opt_lmod) return true;
                if (pre.racktype != racktype) return true;
                //if (pre.seq_no != seq_no) return true;

                return false;
            }
        }

        /// <summary>
        /// PC→PLC
        /// </summary>
        public class Order : BL_ObjectSync
        {
            public enum CMD : UInt16
            {
                /// <summary>動作指示</summary>
                OPERATE = 0x01,

                /// <summary>状態報告＆行先要求 応答</summary>
                RESPONSE = 0x05,
                /// <summary>経路キャンセル</summary>
                ROUTE_CANCEL = 0x06,
                /// <summary>キャリブレーション指示</summary>
                CALIBRATION = 0x08,

                /// <summary>充電終了指示</summary>
                CHARGE_STOP = 0x0B,
                /// <summary>異常復帰指示</summary>
                ERROR_RESET = 0x0C,

                /// <summary>異常発報</summary>
                RAISE_ERROR = 0x0E,
            }

            public enum MOD : UInt16
            {
                ///<summary>BIT0 徐行</summary>
                SPEED_0 = 0x0001,
                ///<summary>BIT1 中速</summary>
                SPEED_1 = 0x0002,
                ///<summary>BIT2 自動</summary>
                SPEED_2 = 0x0004,

                ///<summary>BIT3 ラック回転</summary>
                RACK_ROTATE = 0x0008,
                ///<summary>BIT4 AGV回転</summary>
                AGV_ROTATE = 0x0010,
                ///<summary>BIT5 後退走行指示</summary>
                AGV_BACK = 0x0020,
                ///<summary>BIT6 充電</summary>
                CHARGE = 0x0040,
                ///<summary>BIT7 キャリブレーション不可</summary>
                UNCALIBRATABLE = 0x0080,
                ///<summary>BIT8 ラック上昇</summary>
                RACK_UP = 0x0100,
                ///<summary>BIT9 ラック下降</summary>
                RACK_DOWN = 0x0200,
                ///<summary>BIT10 棚位置決め指示</summary>
                RACK_UP_REGULATION = 0x0400,

#if VER12
                ///<summary>BIT11 通信断無効</summary>
                DISABLE_WIFIERR = 0x0800,
                ///<summary>BIT12 複数QRポイント</summary>
                MULTI_QR = 0x1000,
                ///<summary>BIT13 次ポイントRACK確認</summary>
                STOP_EMPTYPOINT = 0x2000,
#endif
            }

            ///<summary>CMD</summary>
            [BL_ObjectSync]
            public UInt16 cmd = 0;
            ///<summary>MOD</summary>
            [BL_ObjectSync]
            public UInt16 mod = 0;
            ///<summary>DEG</summary>
            [BL_ObjectSync]
            public Int16 deg = 0;

#if VER14
            ///<summary>FromXPOS</summary>
            [BL_ObjectSync]
            public Int32 fx = 0;
            ///<summary>FromYPOS</summary>
            [BL_ObjectSync]
            public Int32 fy = 0;
#endif

            ///<summary>XPOS</summary>
            [BL_ObjectSync]
            public Int32 x = 0;
            ///<summary>YPOS</summary>
            [BL_ObjectSync]
            public Int32 y = 0;
            ///<summary>RACK DEG</summary>
            [BL_ObjectSync]
            public Int16 rack_deg = 0;
            /// <summary>伝文No</summary>
            [BL_ObjectSync]
            public UInt16 seq_no = 0;
            ///<summary>旋回R</summary>
            [BL_ObjectSync]
            public UInt16 turn_r = 0;

#if VER12
            ///<summary>MOD OPT</summary>
            [BL_ObjectSync]
            public UInt16 mod_opt = 0;
            ///<summary>LPAT(ﾚｰｻﾞｽｷｬﾅｾﾝｻの検出ｴﾘｱﾊﾟﾀｰﾝ)</summary>
            [BL_ObjectSync]
            public byte l_pat = 0;
            ///<summary>MPAT(走行ﾒﾛﾃﾞｨのﾊﾟﾀｰﾝ)</summary>
            [BL_ObjectSync]
            public byte m_pat = 0;
            ///<summary>DIST走行指示時のTOﾎﾟｲﾝﾄまでの距離(単位=cm)</summary>
            [BL_ObjectSync]
            public UInt32 dist = 0;

            ///<summary>予備</summary>
            [BL_ObjectSync]
            public byte[] reserve = new byte[4];
#endif

            ///<summary>徐行</summary>
            public bool mod_speed_0 { get { return (mod & (UInt16)MOD.SPEED_0) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.SPEED_0) : (UInt16)(mod & ~(UInt16)MOD.SPEED_0); } }

            ///<summary>中速</summary>
            public bool mod_speed_1 { get { return (mod & (UInt16)MOD.SPEED_1) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.SPEED_1) : (UInt16)(mod & ~(UInt16)MOD.SPEED_1); } }

            ///<summary>高速</summary>
            public bool mod_speed_2 { get { return (mod & (UInt16)MOD.SPEED_2) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.SPEED_2) : (UInt16)(mod & ~(UInt16)MOD.SPEED_2); } }



            ///<summary>ラック回転</summary>
            public bool mod_rack_rorate { get { return (mod & (UInt16)MOD.RACK_ROTATE) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.RACK_ROTATE) : (UInt16)(mod & ~(UInt16)MOD.RACK_ROTATE); } }

            ///<summary>AGV回転</summary>
            public bool mod_agv_rorate { get { return (mod & (UInt16)MOD.AGV_ROTATE) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.AGV_ROTATE) : (UInt16)(mod & ~(UInt16)MOD.AGV_ROTATE); } }

            ///<summary>後退走行指示</summary>
            public bool mod_agv_back { get { return (mod & (UInt16)MOD.AGV_BACK) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.AGV_BACK) : (UInt16)(mod & ~(UInt16)MOD.AGV_BACK); } }

            ///<summary>充電</summary>
            public bool mod_charge { get { return (mod & (UInt16)MOD.CHARGE) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.CHARGE) : (UInt16)(mod & ~(UInt16)MOD.CHARGE); } }

            ///<summary>キャリブレーション不可</summary>
            public bool mod_uncalibratable { get { return (mod & (UInt16)MOD.UNCALIBRATABLE) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.UNCALIBRATABLE) : (UInt16)(mod & ~(UInt16)MOD.UNCALIBRATABLE); } }

            ///<summary>ラック上昇</summary>
            public bool mod_rack_up { get { return (mod & (UInt16)MOD.RACK_UP) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.RACK_UP) : (UInt16)(mod & ~(UInt16)MOD.RACK_UP); } }

            ///<summary>ラック下降</summary>
            public bool mod_rack_down { get { return (mod & (UInt16)MOD.RACK_DOWN) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.RACK_DOWN) : (UInt16)(mod & ~(UInt16)MOD.RACK_DOWN); } }

            ///<summary>棚位置決め指示</summary>
            public bool mod_rack_up_regulation { get { return (mod & (UInt16)MOD.RACK_UP_REGULATION) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.RACK_UP_REGULATION) : (UInt16)(mod & ~(UInt16)MOD.RACK_UP_REGULATION); } }

#if VER12
            ///<summary>通信断無効</summary>
            public bool mod_disable_wifierr { get { return (mod & (UInt16)MOD.DISABLE_WIFIERR) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.DISABLE_WIFIERR) : (UInt16)(mod & ~(UInt16)MOD.DISABLE_WIFIERR); } }

            ///<summary>複数QRポイント</summary>
            public bool mod_multi_qr { get { return (mod & (UInt16)MOD.MULTI_QR) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.MULTI_QR) : (UInt16)(mod & ~(UInt16)MOD.MULTI_QR); } }

            ///<summary>空きポイント停止</summary>
            public bool mod_stop_emptypoint { get { return (mod & (UInt16)MOD.STOP_EMPTYPOINT) != 0; } set { mod = value ? (UInt16)(mod | (UInt16)MOD.STOP_EMPTYPOINT) : (UInt16)(mod & ~(UInt16)MOD.STOP_EMPTYPOINT); } }
#endif

            /// <summary>停止</summary>
            public bool stop = false;

            public Order()
            {
                Initialize();
            }

            public Order(AgvController.AgvCommunicator.Order.CMD cmd, PointF location, bool agv_turn, short deg, bool rackup, bool rackdown, bool rack_turn, short rackdeg, bool back, bool charge, bool rack_regulation)
                : this()
            {
                this.cmd = (ushort)cmd;
                this.Location = location;
                if (deg != 999) this.mod_agv_rorate = agv_turn;
                this.deg = deg;
                this.mod_rack_up = rackup;
                this.mod_rack_down = rackdown;
                this.mod_rack_rorate = rack_turn;
                this.rack_deg = (short)((rackdeg + 360) % 360);
                this.mod_agv_back = back;
                this.mod_charge = charge;
                this.mod_rack_up_regulation = rack_regulation;
            }

#if VER12
            public Order(AgvController.AgvCommunicator.Order.CMD cmd, PointF location, bool agv_turn, short deg, bool rackup, bool rackdown, bool rack_turn, short rackdeg, bool back, bool charge, bool rack_regulation,
                         bool disable_wifierr, bool multi_qr, bool stop_emptypoint, byte l_pat, byte m_pat, UInt32 dist)
                : this(cmd, location, agv_turn, deg, rackup, rackdown, rack_turn, rackdeg, back, charge, rack_regulation)
            {
                this.mod_disable_wifierr = disable_wifierr;
                this.mod_multi_qr = multi_qr;
                this.mod_stop_emptypoint = stop_emptypoint;
                this.l_pat = l_pat;
                this.m_pat = m_pat;
                this.dist = dist;
            }
#endif
            public override string ToString()
            {
                string ret = ",S[";

                ret += ((CMD)cmd).ToString().PadRight(20);

                ret += ",SEQ_NO=" + seq_no.ToString().PadRight(6);

#if VER14
                ret += ",FX=" + fx.ToString().PadRight(6);
                ret += ",FY=" + fy.ToString().PadRight(6);
#endif
                ret += ",X=" + x.ToString().PadRight(6);
                ret += ",Y=" + y.ToString().PadRight(6);
                ret += ",AGVDEG=" + deg.ToString().PadRight(4);
                ret += ",RACKDEG=" + rack_deg.ToString().PadRight(4);
                ret += ",LPAT=" + l_pat.ToString().PadRight(1);
                ret += ",MPAT=" + m_pat.ToString().PadRight(1);
                ret += ",DIST=" + dist.ToString().PadLeft(5);

                ret += mod_speed_2 ? ",H" : ", ";
                ret += mod_speed_1 ? ",M" : ", ";
                ret += mod_speed_0 ? ",L" : ", ";
                ret += mod_rack_rorate ? ",RACKROT" : ",       ";
                ret += mod_agv_rorate ? ",AGVROT" : ",      ";
                ret += mod_agv_back ? ",AGVBACK" : ",       ";
                ret += mod_charge ? ",CHARGE" : ",      ";
                ret += mod_rack_up ? ",RACKUP" : ",      ";
                ret += mod_rack_down ? ",RACKDOWN" : ",        ";
                ret += mod_uncalibratable ? ",NOCALIB" : ",       ";
                ret += mod_rack_up_regulation ? ",RACKREG" : ",       ";

                ret += "]";

                return ret;
            }

            public UInt16 speed
            {
                get
                {
                    return (UInt16)((mod & (UInt16)MOD.SPEED_0) | (mod & (UInt16)MOD.SPEED_1) | (mod & (UInt16)MOD.SPEED_2));
                }

                set
                {
                    mod_speed_0 = (value & (UInt16)MOD.SPEED_0) != 0;
                    mod_speed_1 = (value & (UInt16)MOD.SPEED_1) != 0;
                    mod_speed_2 = (value & (UInt16)MOD.SPEED_2) != 0;
                }
            }

#if VER14
            public PointF LocationFrom
            {
                get
                {
                    return new PointF(fx, fy);
                }

                set
                {
                    fx = (int)value.X;
                    fy = (int)value.Y;
                }
            }
#endif

            public PointF Location
            {
                get
                {
                    return new PointF(x, y);
                }

                set
                {
                    x = (int)value.X;
                    y = (int)value.Y;
                }
            }

            public bool IsChanged(Order pre)
            {
                if (pre.cmd != cmd) return true;
                if (pre.deg != deg) return true;
                if (pre.Location != Location) return true;
                if (pre.mod != mod) return true;
                if (pre.rack_deg != rack_deg) return true;
                //if (pre.reserve != reserve) return true;
                if (pre.speed != speed) return true;

#if VER14
                if (pre.LocationFrom != LocationFrom) return true;
#endif

                return false;
            }
        }

        #endregion

        #region フィールド


        private object sync_state = new object();
        private State state = new State();

        private object sync_orders = new object();
        private List<Order> orders = new List<Order>();

        private BL_RawSocketUDP recv = null;
        private IPEndPoint remote_client = null;
        private BL_RawSocketUDP send = null;
        private IPEndPoint remote_host = null;
        private bool owncreated = false;

        private BL_Stopwatch swTimeout = new BL_Stopwatch();

        private ushort seq_no = 10000 - 1;

        private Dictionary<int, List<Order>> ordered = new Dictionary<int, List<Order>>();

        #endregion

        public bool Alive { get { return swTimeout.ElapsedMilliseconds < 30000; } }

        /// <summary>
        /// AGVと通信するクラスのコンストラクタ
        /// 送受信するためのソケットを複数AGVで共有する場合は、こちら。
        /// </summary>
        /// <param name="ip">通信対象IPアドレス</param>
        /// <param name="remote_client">送信元ポートNo</param>
        /// <param name="remote_host">送信先ポートNo</param>
        /// <param name="recv">受信用ソケット</param>
        /// <param name="send">送信用ソケット</param>
        public AgvCommunicator(string name, string ip, int remote_client, int remote_host, BL_RawSocketUDP recv, BL_RawSocketUDP send)
            : base("AgvCommunicator_" + name)
        {
            this.Name = name;

            this.remote_client = new IPEndPoint(IPAddress.Parse(ip), remote_client);
            this.recv = recv;

            this.remote_host = new IPEndPoint(IPAddress.Parse(ip), remote_host);
            this.send = send;
        }

        /// <summary>
        /// AGVと通信するクラスのコンストラクタ
        /// 1対1で通信する場合は、こちら。（送受信用ソケットを生成します。）
        /// </summary>
        /// <param name="ip">通信対象IPアドレス</param>
        /// <param name="local_host">受信待機ポートNo</param>
        /// <param name="remote_host">送信先ポートNo</param>
        /// <param name="local_client">送信用ポートNo</param>
        public AgvCommunicator(string name, string ip, int local_host, int remote_host, int local_client)
            : base("AgvCommunicator_" + name)
        {
            this.Name = name;

            this.remote_client = new IPEndPoint(IPAddress.Any, 0);
            recv = new BL_RawSocketUDP(local_host);

            this.remote_host = new IPEndPoint(IPAddress.Parse(ip), remote_host);
            send = new BL_RawSocketUDP(local_client);

            recv.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);
            send.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);

            owncreated = true;
        }

        public override string StartControl(int sleep, ThreadPriority priority)
        {
            swTimeout.Restart();

            return base.StartControl(sleep, priority);
        }

        public override void StopControl()
        {
            base.StopControl();

            if (owncreated)
            {
                recv.Close();
                send.Close();

                recv = null;
                send = null;
            }

            ordered.Clear();
        }

        protected override bool DoControl(object message)
        {
            if (m_swTemp.IsRunning && 2000 <= m_swTemp.ElapsedMilliseconds)
            {
                Log("処理時間 DoControl:" + m_swTemp.ElapsedMilliseconds.ToString() + "ms");
            }
            m_swTemp.Restart();

            lock (sync_state)
            {
                if (recv != null)
                {
                    byte[] rdata = recv.ReceiveBytes(remote_client);

                    if (state.Length <= rdata.Length)
                    {
                        state.last_time = DateTime.Now;

                        swTimeout.Restart();

                        //string l = "";
                        //foreach (var v in rdata) l += (v.ToString("X2") + " ");
                        //Log("[" + remote_client + "]->R[" + l + "]");

                        state.SetBytes(rdata);

                        Log(state.ToString());

                        if (state.cmd == (ushort)State.CMD.RES_ORDER)
                        {
                            if (ordered.ContainsKey(state.seq_no))
                            {
                                ordered.Remove(state.seq_no);
                            }
                            else
                            {
                                Log("不明伝文No[" + state.seq_no + "]");
                            }
                        }

                        if (ReceiveEvent != null) ReceiveEvent(this, new ReceiveEventArgs(state, remote_client));
                    }
                    else if (0 < rdata.Length)
                    {
                        Log("データサイズ不足[" + rdata.Length.ToString() + "]");

                        string l = "";
                        foreach (var v in rdata) l += (v.ToString("X2") + " ");
                        Log("[" + remote_client + "]->R[" + l + "]");
                    }
                }
            }

            lock (sync_orders)
            {
                if (send != null)
                {
                    if (0 < orders.Count)
                    {
                        {
                            var operates = orders.Where(e => e.cmd == (ushort)Order.CMD.OPERATE).ToList();

                            List<byte> sdata = new List<byte>();
                            foreach (var v in operates)
                            {
                                seq_no = (ushort)(seq_no + 1);
                                if (19999 < seq_no) seq_no = 10000;

                                v.seq_no = seq_no;

                                byte[] d = v.GetBytes();
                                sdata.AddRange(d);

                                Log(v.ToString());

                                orders.Remove(v);

                                ordered[seq_no] = new List<Order>();
                                ordered[seq_no].Add(v);

                                //string l = "";
                                //foreach (var vv in d) l += (vv.ToString("X2") + " ");
                                //Log("S[" + l + "]");
                            }

                            if (!send.SendBytes(sdata.ToArray(), remote_host))
                            {
                                Log("送信エラー:" + send.ErrorMessage);
                            }
                            else
                            {
                                if (0 < operates.Count)
                                {
                                }
                            }
                        }

                        {
                            foreach (var v in orders)
                            {
                                //seq_no = (ushort)(seq_no + 1);
                                //if (9999 < seq_no) seq_no = 1;
                                //v.seq_no = seq_no;

                                byte[] d = v.GetBytes();

                                Log(v.ToString());

                                if (!send.SendBytes(d, remote_host))
                                {
                                    Log("送信エラー:" + send.ErrorMessage);
                                }

                                //string l = "";
                                //foreach (var vv in d) l += (vv.ToString("X2") + " ");
                                //Log("S[" + l + "]");
                            }
                        }

                        orders.Clear();
                    }
                }
            }

            return base.DoControl(message);
        }

        public string Name { get; } = "";

        public IPEndPoint RemoteHost { get { return remote_host; } }
        public IPEndPoint RemoteClient { get { return remote_client; } }

        public State GetState { get { return state; } }

        public void SetOrder(Order[] orders)
        {
            lock (sync_orders)
            {
                this.orders.AddRange(orders);
            }
        }

        public double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
        }

        public double Degree(PointF p1, PointF p2)
        {
            double r = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
            if (r < 0) r = r + 2 * Math.PI;
            double degree = Math.Floor(r * 360 / (2 * Math.PI));

            degree = (degree + 90) % 360;

            return degree;
        }
    }
}
