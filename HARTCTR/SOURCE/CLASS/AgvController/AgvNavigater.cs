using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BelicsClass.Network;
using BelicsClass.File;
using BelicsClass.ProcessManage;

/* memo

    AgvCommunicatorクラスで、送信元によって受信データ振り分ける機能を実装しました。
​
    public AgvCommunicator(string name, string ip, int recv_remote, int send_remote, BL_RawSocketUDP recv, BL_RawSocketUDP send)

    recv_remoteに送信元ポートNo、
​
    send_remoteに送信先ポートNo、

    recvとsendには各ローカルポートにバインド済みのBL_RawSocketUDPクラスのインスタンスを与えてください。
    
（例）
            recv = new BL_RawSocketUDP(9300);
            send = new BL_RawSocketUDP(9200);
            recv.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);
            send.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);
            AgvCommunicator agvcom = new AgvCommunicator("AGV1”, "192.168.1.1", 9100, 9000, recv, send);

    ※複数AGVでnameやIPアドレスは重複できません。
 */

namespace AgvController
{
    public class AgvSetting
    {
        public AgvSetting(string name, string ip, int remoteClient, int remoteHost, int recvPort, int sendPort)
        {
            Name = name;
            IP = ip;
            RemoteClientPort = remoteClient;
            RemoteHostPort = remoteHost;
            LocalRecvPort = recvPort;
            LocalSendPort = sendPort;
        }

        public string Name { get; set; }
        public string IP { get; set; }
        public int RemoteClientPort { get; set; }
        public int RemoteHostPort { get; set; }
        public int LocalRecvPort { get; set; }
        public int LocalSendPort { get; set; }
    }

    public class AgvNavigater
    {
        private class AGVSpeedFlags
        {
            public bool onSpeed0;
            public bool onSpeed1;
            public bool onSpeed2;
        }

        private class AgvCom
        {
            private BL_RawSocketUDP send;
            private BL_RawSocketUDP recv;
            public AgvCommunicator Communicator { get; } = null;

            public AgvCom(AgvSetting setting)
            {
                send = new BL_RawSocketUDP(setting.LocalSendPort);
                recv = new BL_RawSocketUDP(setting.LocalRecvPort);
                recv.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);
                send.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);

                Communicator = new AgvCommunicator(setting.Name, setting.IP, setting.RemoteClientPort, 
                    setting.RemoteHostPort, recv, send);
            }
        }

        private List<AgvCom> agvComs = new List<AgvCom>();
        private Map map;
        private BL_Log log;

        public AgvNavigater(string mappath, List<AgvSetting> agvSettings)
        {
            map = new Map(mappath);
            log = new BL_Log(true, "", "AgvNavigationManager.log");
            
            foreach(AgvSetting setting in agvSettings)
            {
                agvComs.Add(new AgvCom(setting));
            }
        }


        public List<AgvCommunicator.Order> ToNodeOrder(AgvCommunicator.State state, string goalNodeName)
        {
            FloorMap floor = map.SelectFloor(state.map);
            MapNode startNode = floor.SearchNode(new Vector(state.x, state.y));

            List<AgvCommunicator.Order> orders = new List<AgvCommunicator.Order>();

            List<Vector> route = floor.GenerateRoute(startNode.Name, goalNodeName, state.sta_rack);

            short preDeg = state.deg;

            int count = route.Count - 1;
            while (count > 0)
            {
                AgvCommunicator.Order order = new AgvCommunicator.Order();

                Vector current = route[count];
                Vector next = route[count - 1];

                short deg = AGVDegree(current, next, preDeg);
                AGVSpeedFlags flags = AGVSpeed(count, route.Count);

                order.cmd = (ushort)AgvCommunicator.Order.CMD.THROUGH;
                if (count == route.Count - 1 && deg != preDeg)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.STAY;
                }
                else if (deg != preDeg)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.OPERATE;
                }
                else if (count == 1)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.GOAL;
                }

                order.mod_speed_0 = flags.onSpeed0;
                order.mod_speed_1 = flags.onSpeed1;
                order.mod_speed_2 = flags.onSpeed2;

                if (deg != preDeg) order.mod_agv_rorate = true;
                order.mod_rack_up = state.sta_rack;

                order.x = current.x;
                order.y = current.y;
                order.deg = deg;
                preDeg = deg;

                count--;
                orders.Add(order);
            }

            return orders;
        }

        public List<AgvCommunicator.Order> TakeRackToPickOrder(AgvCommunicator.State state, string rackNodeName, string pickNodeName)
        {
            List<AgvCommunicator.Order> orders = new List<AgvCommunicator.Order>();

            FloorMap floor = map.SelectFloor(state.map);
            MapNode node = floor.SearchNode(new Vector(state.x, state.y));

            List<Vector> route1 = floor.GenerateRoute(node.Name, rackNodeName, false);
            List<Vector> route2 = floor.GenerateRoute(rackNodeName, pickNodeName, true);

            short preDeg = state.deg;

            int count = route1.Count - 1;
            while (count > 0)
            {
                AgvCommunicator.Order order = new AgvCommunicator.Order();

                Vector current = route1[count];
                Vector next = route1[count - 1];

                short deg = AGVDegree(current, next, preDeg);
                AGVSpeedFlags flags = AGVSpeed(count, route1.Count);


                order.cmd = (ushort)AgvCommunicator.Order.CMD.THROUGH;
                if (count == route1.Count - 1 && deg != preDeg)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.STAY;
                }
                else if (count == 1 || deg != preDeg)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.OPERATE;
                }

                order.mod_speed_0 = flags.onSpeed0;
                order.mod_speed_1 = flags.onSpeed1;
                order.mod_speed_2 = flags.onSpeed2;

                if (deg != preDeg) order.mod_agv_rorate = true;
                if (count == 1) order.mod_rack_up = true;

                order.x = current.x;
                order.y = current.y;
                order.deg = deg;
                preDeg = deg;

                count--;
                orders.Add(order);
            }

            count = route2.Count - 2;
            while (count > 0)
            {
                AgvCommunicator.Order order = new AgvCommunicator.Order();

                Vector current = route2[count];
                Vector next = route2[count - 1];

                short deg = AGVDegree(current, next, preDeg);
                AGVSpeedFlags flags = AGVSpeed(count, route2.Count);

                order.cmd = (ushort)AgvCommunicator.Order.CMD.THROUGH;
                if (count == 2 || deg != preDeg)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.OPERATE;
                }
                else if (count == 1)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.GOAL;
                }

                order.mod_speed_0 = flags.onSpeed0;
                order.mod_speed_1 = flags.onSpeed1;
                order.mod_speed_2 = flags.onSpeed2;

                if (deg != preDeg) order.mod_agv_rorate = true;
                order.mod_rack_up = true;

                order.x = current.x;
                order.y = current.y;
                order.deg = deg;
                preDeg = deg;

                if (count == 2)
                {
                    MapNode n = floor.SearchNode(next);
                    if (n != null && n.RackRotable)
                    {
                        short angle = (short)(n.StationDirection -  state.deg);
                        order.rack_deg = angle;
                        order.mod_rack_rorate = true;
                    }
                }

                count--;
                orders.Add(order);
            }

            return orders;
        }

        public List<AgvCommunicator.Order> ReturnRackOrder(AgvCommunicator.State state, string rackNodeName)
        {
            FloorMap floor = map.SelectFloor(state.map);
            MapNode startNode = floor.SearchNode(new Vector(state.x, state.y));

            List<AgvCommunicator.Order> orders = new List<AgvCommunicator.Order>();

            List<Vector> route = floor.GenerateRoute(startNode.Name, rackNodeName, true);

            short preDeg = state.deg;

            int count = route.Count - 1;
            while (count > 0)
            {
                AgvCommunicator.Order order = new AgvCommunicator.Order();

                Vector current = route[count];
                Vector next = route[count - 1];

                short deg = AGVDegree(current, next, preDeg);
                AGVSpeedFlags flags = AGVSpeed(count, route.Count);

                order.cmd = (ushort)AgvCommunicator.Order.CMD.THROUGH;
                if (count == route.Count - 1 && deg != preDeg)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.STAY;
                }
                else if (deg != preDeg)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.OPERATE;
                }
                else if (count == 1)
                {
                    order.cmd = (ushort)AgvCommunicator.Order.CMD.GOAL;
                }

                order.mod_speed_0 = flags.onSpeed0;
                order.mod_speed_1 = flags.onSpeed1;
                order.mod_speed_2 = flags.onSpeed2;

                if (deg != preDeg) order.mod_agv_rorate = true;
                if (count > 1) order.mod_rack_up = true;

                order.x = current.x;
                order.y = current.y;
                order.deg = deg;
                preDeg = deg;

                count--;
                orders.Add(order);
            }

            return orders;
        }


        private short AGVDegree(Vector current, Vector next, short defaultDegree)
        {
            short degree = defaultDegree;
            Vector dv = next - current;
            if (next.x == current.x)
            {
                if (dv.y > 0) degree = 0;
                if (dv.y < 0) degree = 180;
            }
            if (next.y == current.y)
            {
                if (dv.x > 0) degree = -90;
                if (dv.x < 0) degree = 90;
            }

            return degree;
        }

        private AGVSpeedFlags AGVSpeed(int nowCount, int routeLength)
        {
            AGVSpeedFlags flags = new AGVSpeedFlags();

            if (routeLength == 1)
            {
                flags.onSpeed0 = true;
                flags.onSpeed1 = true;
            }
            else if (routeLength == 2 || routeLength == 3)
            {
                flags.onSpeed1 = true;
                if (nowCount == 1)
                {
                    flags.onSpeed0 = true;
                }
            }
            else
            {
                if (nowCount == 2)
                {
                    flags.onSpeed1 = true;
                    flags.onSpeed2 = true;
                }
                else if (nowCount == 1)
                {
                    flags.onSpeed0 = true;
                    flags.onSpeed1 = true;
                }
                else
                {
                    flags.onSpeed2 = true;
                }
            }

            return flags;
        }
    }
}
