using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;

using BelicsClass.Common;
using BelicsClass.ObjectSync;


namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class FloorSerializer : BL_XmlSync
        {
            public class RACK : BL_XmlSync
            {
                public class RACK_FACE : BL_XmlSync
                {
                    [BL_ObjectSync]
                    public int deg = 0;
                    [BL_ObjectSync]
                    public string id = "";
                }

                [BL_ObjectSync]
                public string rack_no = "";

                [BL_ObjectSync]
                public List<RACK_FACE> face_id = new List<RACK_FACE>();
            }

            public class NEXT_WAY : BL_XmlSync
            {
                [BL_ObjectSync]
                public string qr_string = "";
                [BL_ObjectSync]
                public float deg = 0;
                [BL_ObjectSync]
                public string route_type = "";
            }

            public class QR : BL_XmlSync
            {
                [BL_ObjectSync]
                public string floor_code = "";
                [BL_ObjectSync]
                public int x = 0;
                [BL_ObjectSync]
                public int y = 0;

                [BL_ObjectSync]
                public bool rack_rotatable = false;
                [BL_ObjectSync]
                public int direction_charge = 0;
                [BL_ObjectSync]
                public bool charge_back = true;

                [BL_ObjectSync]
                public string autorator_id = "";

                [BL_ObjectSync]
                public int direction_station = 0;
                [BL_ObjectSync]
                public string station_id = "";
                [BL_ObjectSync]
                public string station_type = "";

                [BL_ObjectSync]
                public bool rack_settable = false;
                [BL_ObjectSync]
                public string speed_limit = "";

                [BL_ObjectSync]
                public RACK rack = new RACK();

                [BL_ObjectSync]
                public List<NEXT_WAY> next_way = new List<NEXT_WAY>();
                [BL_ObjectSync]
                public List<string> floorwarp_qr = new List<string>();
            }

            public class FLOOR : BL_XmlSync
            {
                [BL_ObjectSync]
                public string code = "";
                [BL_ObjectSync]
                public string floor_type = "";

                [BL_ObjectSync]
                public string background_filepath = "";
                [BL_ObjectSync]
                public float background_rx = 0;
                [BL_ObjectSync]
                public float background_ry = 0;
                [BL_ObjectSync]
                public float background_scale_w = 0;
                [BL_ObjectSync]
                public float background_scale_h = 0;
                [BL_ObjectSync]
                public float background_angle = 0;
                [BL_ObjectSync]
                public int background_opacity = 0;

                [BL_ObjectSync]
                public List<QR> qrs = new List<QR>();
            }

            #region シリアライズ・デシリアライズ

            public byte[] Serialize(AgvControlManager.FloorMap floor)
            {
                byte[] output = null;

                FloorSerializer.FLOOR save = new FloorSerializer.FLOOR();

                save.code = floor.code;
                save.floor_type = floor.is_fa ? "FA" : "LG";

                var background = floor.mapeditor.background;
                var relative = "";

                if (!string.IsNullOrEmpty(background.fileName))
                {
                    var exe_dir = Path.GetDirectoryName(Application.ExecutablePath) + @"\";

                    var u_exe = new Uri(exe_dir);
                    var u_bak = new Uri(background.fileName);

                    relative = u_exe.MakeRelativeUri(u_bak).ToString();
                }
                save.background_filepath = relative;
                save.background_rx = background.BackGroundRX;
                save.background_ry = background.BackGroundRY;
                save.background_scale_w = background.BackGroundScaleW;
                save.background_scale_h = background.BackGroundScaleH;
                save.background_angle = background.BackGroundAngle;
                save.background_opacity = background.BackGroundOpacity;

                foreach (var kvx in floor.mapeditor.qrs)
                {
                    foreach (var kvy in kvx.Value)
                    {
                        QR qr = new QR();

                        qr.floor_code = floor.code;
                        qr.x = kvx.Key;
                        qr.y = kvy.Key;
                        qr.rack_rotatable = kvy.Value.rack_rotatable;
                        qr.direction_charge = (int)kvy.Value.direction_charge;
                        qr.charge_back = kvy.Value.charge_back;
                        qr.autorator_id = kvy.Value.autorator_id;
                        qr.direction_station = (int)kvy.Value.direction_station;
                        qr.station_type = kvy.Value.station_type.ToString();
                        qr.rack_settable = kvy.Value.rack_setable;
                        qr.speed_limit = kvy.Value.speed_limit.ToString();

                        if (kvy.Value.rack != null)
                        {
                            qr.rack = new RACK();
                            qr.rack.rack_no = kvy.Value.rack.rack_no;
                            foreach (var kv in qr.rack.face_id)
                            {
                                RACK.RACK_FACE rackface = new RACK.RACK_FACE();
                                rackface.deg = kv.deg;
                                rackface.id = kv.id;

                                qr.rack.face_id.Add(rackface);
                            }
                        }

                        foreach (var kv in kvy.Value.next_way)
                        {
                            NEXT_WAY nextway = new NEXT_WAY();
                            nextway.qr_string = kv.Key.QrString;
                            nextway.deg = (float)kv.Value;

                            if (kvy.Value.next_way_type.ContainsKey(kv.Key))
                            {
                                nextway.route_type = kvy.Value.next_way_type[kv.Key].ToString();
                            }
                            else
                            {
                                nextway.route_type = "";
                            }

                            qr.next_way.Add(nextway);
                        }

                        foreach (var kv in kvy.Value.floorwarp_qr)
                        {
                            qr.floorwarp_qr.Add(kv.QrString);
                        }

                        save.qrs.Add(qr);
                    }
                }

                output = save.XmlSerialize();
                return output;
            }

            public void Deserialize(byte[] data, FloorMap floor)
            {
                FLOOR load = new FLOOR();
                load = (FLOOR)load.XmlDeserialize(data);

                if (floor == null)
                {
                    floor = new FloorMap(Program.controller, load.code);
                }
                else
                {
                    floor.mapeditor.qrs.Clear();
                    floor.mapeditor.selected_qr = null;
                }

                floor.code = load.code;
                floor.is_fa = load.floor_type == "FA";

                #region 背景画像展開

                if (floor.mapeditor.background == null)
                {
                    floor.mapeditor.background = new BackGround(floor.mapeditor);
                }

                string background_path = load.background_filepath;

                var absolute = "";

                if (!string.IsNullOrEmpty(background_path))
                {
                    var exe_dir = Path.GetDirectoryName(Application.ExecutablePath) + @"\";

                    var u_exe = new Uri(exe_dir);
                    var u_bak = new Uri(u_exe, background_path);

                    absolute = u_bak.LocalPath;
                }

                if (File.Exists(absolute))
                {
                    try
                    {
                        floor.mapeditor.background.fileName = absolute;
                        floor.mapeditor.background.image = new Bitmap(absolute);

                        floor.mapeditor.background.BackGroundRX = load.background_rx;
                        floor.mapeditor.background.BackGroundRY = load.background_ry;
                        floor.mapeditor.background.BackGroundScaleW = load.background_scale_w;
                        floor.mapeditor.background.BackGroundScaleH = load.background_scale_h;
                        floor.mapeditor.background.BackGroundAngle = load.background_angle;
                        floor.mapeditor.background.BackGroundOpacity = load.background_opacity;
                    }
                    catch { }
                }

                #endregion

                foreach (var v in load.qrs)
                {
                    FloorQR qr = new FloorQR(floor.mapeditor, v.x, v.y);

                    qr.autorator_id = v.autorator_id;
                    qr.direction_charge = (FloorQR.enDirection)v.direction_charge;
                    qr.charge_back = v.charge_back;
                    qr.direction_station = (FloorQR.enDirection)v.direction_station;
                    qr.station_id = v.station_id;

                    qr.station_type = FloorQR.enStatiionType.STATION;
                    foreach (FloorQR.enStatiionType type in Enum.GetValues(typeof(FloorQR.enStatiionType)))
                    {
                        if (type.ToString() == v.station_type)
                        {
                            qr.station_type = type;
                            break;
                        }
                    }

                    foreach (AgvController.enSpeed type in Enum.GetValues(typeof(AgvController.enSpeed)))
                    {
                        if (type.ToString() == v.speed_limit)
                        {
                            qr.speed_limit = type;
                            break;
                        }
                    }

                    if (v.rack != null && v.rack.rack_no != "")
                    {
                        qr.rack = RackMaster.Instance.LoadRack(v.rack.rack_no);
                    }

                    qr.rack_setable = v.rack_settable;
                    qr.rack_rotatable = v.rack_rotatable;

                    if (qr.autorator_id != "")
                    {
                        qr.rect.X = qr.x - qr.autorator_radius / 2;
                        qr.rect.Y = qr.y - qr.autorator_radius / 2;
                        qr.rect.Width = qr.autorator_radius;
                        qr.rect.Height = qr.autorator_radius;
                    }
                    else
                    {
                        qr.rect.X = qr.x - qr.radius / 2;
                        qr.rect.Y = qr.y - qr.radius / 2;
                        qr.rect.Width = qr.radius;
                        qr.rect.Height = qr.radius;
                    }

                    floor.mapeditor.AddQR(qr);
                }

                foreach (var v in load.qrs)
                {
                    if (floor.code != v.floor_code) continue;
                    FloorQR qr = floor.mapeditor.Exist(new PointF(v.x, v.y));
                    if (qr == null) continue;

                    foreach (var vv in v.next_way)
                    {
                        bool breaked = false;
                        foreach (var p in floor.mapeditor.qrs)
                        {
                            foreach (var q in p.Value)
                            {
                                if (q.Value.QrString == vv.qr_string)
                                {
                                    Enum.TryParse(vv.route_type, out FloorQR.RouteType route_type);

                                    qr.AddNextWay(q.Value, route_type);
                                    breaked = true;
                                    break;
                                }
                            }

                            if (breaked) break;
                        }
                    }
                }
            }

            #endregion
        }

        #region フロア管理クラス

        public class FloorMap
        {
            private List<byte[]> undo = new List<byte[]>();
            private List<byte[]> redo = new List<byte[]>();

            #region フィールド

            public AgvControlManager controller = null;

            public Font qrfont = SystemFonts.DefaultFont;
            public Brush brush_qr = SystemBrushes.ControlText;
            public Pen pen_grid = new Pen(Brushes.LightGray, 1);
            public Pen pen_cur = new Pen(Brushes.DodgerBlue, 2);

            public bool is_fa = false;
            public string code = " ";

            public AgvConditioner conditioner = null;
            public AgvMapEditor mapeditor = null;

            //public int rack_size_max = 170;

            #endregion

            #region コンストラクタ・プロパティ

            public override string ToString()
            {
                return code + " [" + mapeditor.Count.ToString() + "][" + controller.agvs.Count.ToString() + "]";
            }

            public FloorMap(AgvControlManager controller, string floor_code)
            {
                this.controller = controller;
                this.code = floor_code;

                conditioner = new AgvConditioner(this);
                mapeditor = new AgvMapEditor(this);
            }

            public int FloorNo
            {
                get
                {
                    int no = 0;
                    foreach (var kv in controller.map)
                    {
                        no++;
                        if (kv.Value == this) break;
                    }

                    return no;
                }
            }

            #endregion

            #region 座標取得

            public float pX(float x)
            {
                return x * controller.draw_scale_g + controller.Offset.X;
            }

            public float pY(float y)
            {
                return controller.draw_size_pixel.Height - (y * controller.draw_scale_g + controller.Offset.Y);
            }

            public float pW(float w)
            {
                return w * controller.draw_scale_g;
            }

            public float pH(float h)
            {
                return h * controller.draw_scale_g;
            }

            public float rX(float x)
            {
                return (x - controller.Offset.X) / controller.draw_scale_g;
            }

            public float rY(float y)
            {
                return (controller.draw_size_pixel.Height - (y + controller.Offset.Y)) / controller.draw_scale_g;
            }

            public float rW(float w)
            {
                return w / controller.draw_scale_g;
            }

            public float rH(float h)
            {
                return h / controller.draw_scale_g;
            }

            #endregion

            #region 描画

            public void Draw(Graphics g, bool transparent = false)
            {
                mapeditor.Draw(g, transparent);

                if (controller.applicate_mode != enApplicateMode.MAPEDITOR)
                {
                    conditioner.Draw(g);
                }
            }

            public void DrawIndicators(Graphics gh, Graphics gv)
            {
                Font font = new Font(SystemFonts.DefaultFont.FontFamily, 14f);

                {
                    for (float x = rW(-controller.draw_offset_pixel.X) + rW(controller.draw_offset_pixel.X) % 10; x < rW(controller.draw_size_pixel.Width - controller.draw_offset_pixel.X); x += 10f)
                    {
                        int h = x % 50 == 0 ? (x % 100 == 0 ? 10 : 7) : 5;
                        float px = pX(x);

                        if (0 <= x)
                        {
                            gh.DrawLine(Pens.Black, px, 0, px, h);
                        }
                        else
                        {
                            gh.DrawLine(Pens.Red, px, 0, px, h);
                        }

                        int pitch = 1000;
                        if (0.25 <= controller.draw_scale) pitch = 500;
                        if (0.5 <= controller.draw_scale) pitch = 200;
                        if (1.0 <= controller.draw_scale) pitch = 100;
                        if (2.0 <= controller.draw_scale) pitch = 50;

                        if (x % pitch == 0)
                        {
                            string s = x.ToString();
                            SizeF sz = gh.MeasureString(s, font);
                            StringFormat sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Near;

                            DrawString(gh, s, font, Brushes.Black, px, 10, -90, sf);
                        }
                    }
                }

                {
                    for (float y = rH(-controller.draw_offset_pixel.Y) + rH(controller.draw_offset_pixel.Y) % 10; y < rH(controller.draw_size_pixel.Height - controller.draw_offset_pixel.Y); y += 10f)
                    {
                        int w = y % 50 == 0 ? (y % 100 == 0 ? 10 : 7) : 5;
                        float py = pY(y);

                        if (0 <= y)
                        {
                            gv.DrawLine(Pens.Black, controller.indicator_width, py, controller.indicator_width - w, py);
                        }
                        else
                        {
                            gv.DrawLine(Pens.Red, controller.indicator_width, py, controller.indicator_width - w, py);
                        }

                        int pitch = 1000;
                        if (0.25 <= controller.draw_scale) pitch = 500;
                        if (0.5 <= controller.draw_scale) pitch = 200;
                        if (1.0 <= controller.draw_scale) pitch = 100;
                        if (2.0 <= controller.draw_scale) pitch = 50;

                        if (y % pitch == 0)
                        {
                            string s = y.ToString();
                            SizeF sz = gv.MeasureString(s, font);
                            StringFormat sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Far;

                            DrawString(gv, s, font, Brushes.Black, controller.indicator_width - 10, py, 0, sf);
                        }
                    }
                }

                //if (0 < map.mousePoint.X && 0 < map.mousePoint.Y)
                {
                    gh.DrawLine(pen_cur, pX(controller.mousePoint.X), 0, pX(controller.mousePoint.X), 20);
                    gv.DrawLine(pen_cur, 0, pY(controller.mousePoint.Y), 20, pY(controller.mousePoint.Y));
                }
            }

            public void DrawString(Graphics g, string s, Font f, Brush brush, float x, float y, float deg, StringFormat format)
            {
                deg = (-deg + 270 + 360) % 360;

                using (var pathText = new System.Drawing.Drawing2D.GraphicsPath())
                using (var mat = new System.Drawing.Drawing2D.Matrix())
                {
                    var formatTemp = (StringFormat)format.Clone();
                    formatTemp.Alignment = StringAlignment.Near;
                    formatTemp.LineAlignment = StringAlignment.Near;

                    pathText.AddString(s, f.FontFamily, (int)f.Style, f.SizeInPoints, new PointF(0, 0), format);
                    formatTemp.Dispose();

                    var rect = pathText.GetBounds();

                    float px;
                    switch (format.Alignment)
                    {
                        case StringAlignment.Near:
                            px = rect.Left;
                            break;
                        case StringAlignment.Center:
                            px = rect.Left + rect.Width / 2f;
                            break;
                        case StringAlignment.Far:
                            px = rect.Right;
                            break;
                        default:
                            px = 0;
                            break;
                    }

                    float py;
                    switch (format.LineAlignment)
                    {
                        case StringAlignment.Near:
                            py = rect.Top;
                            break;
                        case StringAlignment.Center:
                            py = rect.Top + rect.Height / 2f;
                            break;
                        case StringAlignment.Far:
                            py = rect.Bottom;
                            break;
                        default:
                            py = 0;
                            break;
                    }

                    mat.Translate(-px, -py, System.Drawing.Drawing2D.MatrixOrder.Append);
                    mat.Rotate(deg, System.Drawing.Drawing2D.MatrixOrder.Append);
                    mat.Translate(x, y, System.Drawing.Drawing2D.MatrixOrder.Append);
                    pathText.Transform(mat);

                    g.FillPath(brush, pathText);
                }
            }

            #endregion

            #region Undo／Redo機能

            public byte[] AddUndo()
            {
                System.Diagnostics.Debug.WriteLine("AddUndo[" + undo.Count + "]");

                FloorSerializer fs = new FloorSerializer();
                byte[] data = fs.Serialize(this);
                undo.Add(data);
                return data;
            }

            public void Undo()
            {
                System.Diagnostics.Debug.WriteLine("Undo[" + undo.Count + "]");

                if (0 < undo.Count)
                {
                    FloorSerializer fs = new FloorSerializer();
                    fs.Deserialize(undo[undo.Count - 1], this);

                    redo.Add(undo[undo.Count - 1]);
                    undo.RemoveAt(undo.Count - 1);
                }
            }

            public void Redo()
            {
                System.Diagnostics.Debug.WriteLine("Redo[" + redo.Count + "]");

                if (0 < redo.Count)
                {
                    FloorSerializer fs = new FloorSerializer();
                    fs.Deserialize(redo[redo.Count - 1], this);

                    undo.Add(redo[redo.Count - 1]);
                    redo.RemoveAt(redo.Count - 1);
                }
            }

            public void ClearUndoRedo()
            {
                undo.Clear();
                redo.Clear();
            }

            public bool CanUndo { get { return 0 < undo.Count; } }

            public bool CanRedo { get { return 0 < redo.Count; } }


            #endregion
        }

        #endregion
    }
}
