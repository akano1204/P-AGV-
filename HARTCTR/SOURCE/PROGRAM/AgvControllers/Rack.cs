using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BelicsClass.ObjectSync;
using BelicsClass.Common;

using AgvController;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        #region ラック情報管理

        public class Rack : BL_ObjectSync
        {
			#region フィールド・プロパティ

			/// <summary>作業指示（要求～ステーション完了）</summary>
			public AgvOrderCommunicator.RequestDelivery req = null;

            /// <summary>作業完了指示（ステーション完了後～棚返却）</summary>
            public AgvOrderCommunicator.RequestDelivery req_return = null;

            /// <summary>ステーション作業中</summary>
            public bool station_working = false;

            /// <summary>自棚が邪魔をしている棚が通るルート（要求）</summary>
            public RouteConditionList obstruct_route = null;

            /// <summary>積載中AGV</summary>
            public FloorAGV loading_agv = null;

            /// <summary>積載予定AGV</summary>
            public FloorAGV loadreserve_agv = null;

            public bool loading { get { return loading_agv != null; } }

            public bool degree_received = false;
            
            [BL_ObjectSync]
            private float _degree = 0;

            public float degree
            {
                get
                {
                    return _degree;
                }
                set
                {
                    _degree = value;
                    degree_received = true;
                }
            }

            [BL_ObjectSync]
            public string rack_no = "";
            public Dictionary<int, string> face_id = new Dictionary<int, string>();

            /// <summary>出入り可能な方向</summary>
            public Dictionary<int, bool> can_inout = new Dictionary<int, bool>();

            /// <summary>移動可能な方向</summary>
            public Dictionary<int, bool> can_move = new Dictionary<int, bool>();

            /// <summary>X軸方向幅</summary>
            public int sizeW = DEFAULT_SIZE;
            /// <summary>Y軸方向幅</summary>
            public int sizeL = DEFAULT_SIZE;
            /// <summary>面考慮不要</summary>
            public bool anyface = false;
            /// <summary>オーバーハング</summary>
            public bool overhang = false;

            public PointF defaultLocation = new PointF();

            public int SizeMax { get { return System.Math.Max(sizeW, sizeL); } }

            public int SizeMin { get { return System.Math.Min(sizeW, sizeL); } }

            #endregion

            #region 定数

            public static readonly int DEFAULT_SIZE = 170;
            public static readonly int RADIUS = 60; // 描画されるQR円半径
            public static readonly List<int> FACE_DEGREES = new List<int> { 0, 90, 180, 270 };

            #endregion

            #region 列挙体

            public enum LiftStatusType
            {
                ON_FLOOR,
                ON_AGV
            }

            #endregion

            #region 汎用メソッド

            public static Rack GetDefault(string rack_no)
            {
                var _face_id = new Dictionary<int, string>();
                var _can_inout = new Dictionary<int, bool>();
                var _can_move = new Dictionary<int, bool>();

                var id = 1;

                foreach (var deg in FACE_DEGREES)
                {
                    _face_id[deg] = id++.ToString();
                    _can_inout[deg] = true;
                    _can_move[deg] = true;
                }

                return new Rack
                {
                    rack_no = rack_no,
                    face_id = _face_id,
                    can_inout = _can_inout,
                    can_move = _can_move,
                    sizeW = DEFAULT_SIZE,
                    sizeL = DEFAULT_SIZE,
                    anyface = true
                };
            }

            public override string ToString()
            {
                return rack_no +
                    ",deg:"+degree.ToString("000") +
                    ",loadreserve_agv:" + (loadreserve_agv != null ? loadreserve_agv.ToString() : "   ") +
                    ",loading_agv:" + (loading_agv != null ? loading_agv.ToString() : "   ") +
                    ",station_work:" + (station_working ? "true" : "false") +
                    ",req_station:" + (req != null ? req.station : "   ") +
                    ",req_return:" + (req_return != null ? req_return.station : "   ") +
                    ",obstruct_route:" + (obstruct_route != null ? (0 < obstruct_route.Count ? obstruct_route[0].cur_qr.ToString() : "?") : "   ");
                    //",obstructing_route:" + (obstructing_route != null ? (0 < obstructing_route.Count ? obstructing_route[0].cur_qr.ToString() : "?") : "   ");
            }

            #endregion

            #region 角度・座標取得

            public PointF AngleDistance(PointF p, double degree, double distance)
            {
                PointF ret = new PointF();

                degree = (degree + 90 + 360) % 360;
                double radian = degree * Math.PI / 180;

                ret.Y = p.Y + (float)(Math.Sin(radian) * distance);
                ret.X = p.X + (float)(Math.Cos(radian) * distance);

                return ret;
            }

            public PointF AngleDistancePoint(PointF point, double degree, double distance)
            {
                degree = (degree + 90 + 360) % 360;
                double radian = degree * Math.PI / 180;

                PointF vertex = new PointF();
                vertex.X = (float)(point.X + (distance * Math.Cos(radian)));
                vertex.Y = (float)(point.Y + (distance * Math.Sin(radian)));

                return vertex;
            }

            #endregion

            #region ロード・アンロード

            public void Load(FloorAGV agv)
            {
                lock (agv)
                {
                    defaultLocation = agv.on_qr.Location;
                    agv.on_qr.rack = null;
                    agv.rack = this;
                    loading_agv = agv;
                }
            }

            public void Unload(FloorAGV agv)
            {
                lock (agv)
                {
                    defaultLocation = agv.on_qr.Location;
                    agv.rack = null;
                    //@@@FA　棚をマップ上に配置しない
                    //agv.on_qr.rack = this;
                    loading_agv = null;
                }
            }

            public void Load(FloorAGV agv, FloorQR qr)
            {
                lock (agv)
                {
                    defaultLocation = qr.Location;
                    qr.rack = null;
                    agv.rack = this;
                    loading_agv = agv;
                    agv.floor.mapeditor.redraw_rack = true;
                }
            }

            public void Unload(FloorAGV agv, FloorQR qr)
            {
                lock (agv)
                {
                    defaultLocation = qr.Location;
                    agv.rack = null;
                    //@@@FA　棚をマップ上に配置しない
                    //qr.rack = this;
                    loading_agv = null;
                    agv.floor.mapeditor.redraw_rack = true;
                }
            }

            #endregion

            #region 描画

            public void Draw(Graphics g, FloorMap floor, PointF location, LiftStatusType lift_status)
            {
                var status_color = Color.Black;
                var line_thickness = 0;
                var face_id_distance = 0;
                var status_collection = can_inout;

                switch (lift_status)
                {
                    case LiftStatusType.ON_FLOOR:
                        {
                            if (req != null) status_color = Color.Lime;
                            if (req_return != null) status_color = Color.Cyan;
                            if (obstruct_route != null) status_color = Color.Magenta;

                            line_thickness = 5;
                            face_id_distance = -10;
                        }
                        break;

                    case LiftStatusType.ON_AGV:
                        {
                            status_color = Color.Blue;
                            line_thickness = 10;
                            face_id_distance = 20;
                            status_collection = can_move;
                        }
                        break;
                }

                var distance = Math.Sqrt(Math.Pow(sizeW, 2) + Math.Pow(sizeL, 2)) / 2.0 + 2.0;

                Font font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(24f));
                StringFormat f = new StringFormat();
                Brush br = null;

                f.Alignment = StringAlignment.Center;
                f.LineAlignment = StringAlignment.Near;

                foreach (var kv in face_id)
                {
                    if (br != null) br.Dispose();
                    br = new SolidBrush(Color.FromArgb(180, status_collection[kv.Key] ? status_color : Color.Red));

                    float degree = (kv.Key + this.degree) % 360f;
                    string id = kv.Value;

                    var diff_degree = 45.0;
                    var side = sizeL / 2.0;

                    switch (kv.Key)
                    {
                        case 0:
                        case 180:
                            {
                                diff_degree = Math.Atan2(sizeW, sizeL);
                                side = RADIUS * ((double)sizeL / DEFAULT_SIZE);
                            }
                            break;

                        case 90:
                        case 270:
                            {
                                diff_degree = Math.Atan2(sizeL, sizeW);
                                side = RADIUS * ((double)sizeW / DEFAULT_SIZE);
                            }
                            break;
                    }

                    diff_degree *= 180.0 / Math.PI;

                    PointF p1 = AngleDistancePoint(location, degree + diff_degree, distance);
                    PointF p2 = AngleDistancePoint(location, degree - diff_degree, distance);
                    PointF p3 = AngleDistancePoint(p1, degree, -line_thickness);
                    PointF p4 = AngleDistancePoint(p2, degree, -line_thickness);
                    p1.X = floor.pX(p1.X); p1.Y = floor.pY(p1.Y);
                    p2.X = floor.pX(p2.X); p2.Y = floor.pY(p2.Y);
                    p3.X = floor.pX(p3.X); p3.Y = floor.pY(p3.Y);
                    p4.X = floor.pX(p4.X); p4.Y = floor.pY(p4.Y);

                    g.FillPolygon(br, new PointF[] { p1, p3, p4, p2 });

                    PointF p = AngleDistancePoint(location, degree, side + face_id_distance);
                    floor.DrawString(g, id, font, br, floor.pX(p.X), floor.pY(p.Y), degree - 90, f);
                }

                {
                    Font fo = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(32f));
                    Pen pe = new Pen(status_color, 2);

                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;

                    var diff_degree = Math.Atan2(sizeL, sizeW) * 180.0 / Math.PI;

                    PointF p = AngleDistancePoint(location, degree + diff_degree, sizeL / 4);

                    g.DrawEllipse(pe, floor.pX(p.X - 25), floor.pY(p.Y + 25), floor.pW(50), floor.pH(50));
                    floor.DrawString(g, rack_no, fo, br, floor.pX(p.X), floor.pY(p.Y), degree - 90, f);

                    pe.Dispose();
                    fo.Dispose();
                }

                if (br != null) br.Dispose();
            }

            #endregion
        }

		#endregion

	}
}
