using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

using BelicsClass.Common;
using BelicsClass.UI.Controls;
using BelicsClass.File;
using BelicsClass.ObjectSync;

using AgvController;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        #region イベントハンドラー

        public delegate void FloorQR_EventHandler(FloorQR qr);
        public event FloorQR_EventHandler EventQrSelected;
        public event FloorQR_EventHandler EventQrMoved;

        public delegate void FloorAgv_EventHandler(FloorAGV agv);
        public event FloorAgv_EventHandler EventAgvSelected;
        //public event FloorAgv_EventHandler EventAgvMoved;

        public delegate void Condition_EventHandler(RouteCondition conditions);
        public event Condition_EventHandler EventEditCondition;

        public delegate void Trigger_EventHandler(FloorQR qr, string pos, string INorOUT);
        public event Trigger_EventHandler EventTrigger;

        public delegate void FloorQRCommand_EventHandler(FloorQR qr, Point mouse_location);
        public event FloorQRCommand_EventHandler EventQrCommand;

        public delegate void Paint_EventHandler();
        public event Paint_EventHandler EventPaint;

        public delegate void DatabaseError_EventHandler(AgvRunner sender, string message);
        public event DatabaseError_EventHandler EventAgvDatabaseError;

        #endregion

        #region 情報保持クラス
        
        public class AgvInfo
        {
            public int no { get; set; } = 0;
            public string id { get; set; } = "";
            public string ipaddress { get; set; } = "";
            public int hostport { get; set; } = 9000;
            public int clientport { get; set; } = 9100;

            public int stop_distance { get; set; } = 375;

            public int radius { get; set; } = 50;
        }

        public class RackInfo
        {
            public string no { get; set; } = "";
            public int sizeW { get; set; } = Rack.DEFAULT_SIZE;
            public int sizeL { get; set; } = Rack.DEFAULT_SIZE;

            public bool can_inout_0 { get; set; } = true;
            public bool can_inout_90 { get; set; } = true;
            public bool can_inout_180 { get; set; } = true;
            public bool can_inout_270 { get; set; } = true;

            public bool can_move_0 { get; set; } = true;
            public bool can_move_90 { get; set; } = true;
            public bool can_move_180 { get; set; } = true;
            public bool can_move_270 { get; set; } = true;

            public bool anyface { get; set; } = true;
            public bool overhang { get; set; } = false;
        }

        #endregion

        #region フィールド

        public class MoveMode
        {
            public int mode = 0;
            public string name = "なし";
            public string option = "";

            public override string ToString()
            {
                return mode.ToString() + "[" + name + "][" + option + "]";
            }
        }

        public enum enApplicateMode
        {
            /// <summary>不明</summary>
            UNKNOWN,
            /// <summary>マップ編集</summary>
            MAPEDITOR,
            /// <summary>モード動作編集</summary>
            CONDITIONER,
            
            /// <summary>モード動作シミュレーション</summary>
            MODE_SIMULATOR,
            /// <summary>運用シミュレーション</summary>
            OPERATION_SIMULATOR,
            /// <summary>手動コマンドテストシミュレーション</summary>
            MANUAL_SIMULATOR,

            /// <summary>実機モード動作</summary>
            MODE_COMMANDER,

            /// <summary>実機自動コマンド</summary>
            AUTO_COMMANDER,
        }

        public enum enPlaceOperate
        {
            /// <summary>目的地での動作なし</summary>
            NONE,
            /// <summary>目的地で棚上昇</summary>
            RACK_UP,
            /// <summary>目的地で棚下降</summary>
            RACK_DOWN,
            /// <summary>目的地で充電開始</summary>
            CHARGE,
            /// <summary>目的地でステーション待機</summary>
            STATION_WAIT,
            /// <summary>目的地でスピンターン</summary>
            TURN,
        }

        static public SortedDictionary<int, MoveMode> moveModes = new SortedDictionary<int, MoveMode>();

        public enApplicateMode applicate_mode = enApplicateMode.UNKNOWN;
        public int movemode_current = 0;
        public int interval_current = 5;

        public Bitmap bmp = null;
        public Bitmap bmpIndicatorH = null;
        public Bitmap bmpIndicatorV = null;

        public List<FloorAGV> agvs = new List<FloorAGV>();
        public SortedDictionary<string, FloorMap> map = new SortedDictionary<string, FloorMap>();
        public string selected_floor = " ";

        public Point mousePoint = new Point(0, 0);
        public Point mousePoint_down = new Point(0, 0);
        public Point mousePoint_last = new Point(0, 0);
        public BL_Stopwatch sw_dragstart = new BL_Stopwatch();
        public bool is_mousedrag = false;

        public Cursor panel_cursor = Cursors.Default;
        public bool is_editing_background = false;
        public bool is_erasing_ways = false;
        public bool is_retending_aspect = true;

        public FloorQR.RouteType route_type = FloorQR.RouteType.NONE;

        public bool is_stuck = false;
        public int stuck_distance = 50;
        public int stuck_base = 125;

        public float draw_scale = 1.0f;
        public float draw_scale_g { get { return draw_scale * 0.4f; } set { draw_scale = value / 0.4f; } }

        public PointF draw_offset_pixel = new PointF(0, 0);
        public Size draw_size_pixel = new Size();
        public PointF zoom_center = new PointF(0, 0);
        public Point indicator_offset_pixel;
        public bool draw_status_mode = false;

        public int indicator_width = 24;

        public string filepath = "";

        public FloorQR floorwarp_qr = null;
        public RouteCondition floorwarp_con = null;

        public AgvRunManager run_manager = null;
        public AgvRouteManager routeManager = null;

        public bool draw_allfloor = false;

        public Dictionary<string, AgvInfo> agvid_list = new Dictionary<string, AgvInfo>();

        public FloorAGV selected_agv = null;

        #endregion

        #region コンストラクタ・プロパティ

        public AgvControlManager()
        {
            indicator_offset_pixel = new Point(indicator_width, indicator_width);

            moveModes[0] = new MoveMode();

            routeManager = new AgvRouteManager(this);
        }

        public PointF Offset
        {
            get { return new PointF(draw_offset_pixel.X + indicator_offset_pixel.X, draw_offset_pixel.Y + indicator_offset_pixel.Y); }
        }

        List<FloorQR> all_qr = null;
        public List<FloorQR> AllQR
        {
            get
            {
                if (all_qr == null)
                {
                    all_qr = new List<FloorQR>();
                    foreach (var v in map)
                    {
                        all_qr.AddRange(v.Value.mapeditor.list);
                    }
                }

                return all_qr;
            }
        }

        //public List<FloorQR> all_rack_qr = null;

        public List<FloorQR> AllRackQR
        {
            get
            {
                return AllQR.Where(e => e.rack != null).ToList();
                //if (all_rack_qr == null) all_rack_qr = AllQR.Where(e => e.rack != null).ToList();
                //return all_rack_qr;
            }
        }

        //List<Rack> all_rack = null;
        //public List<Rack> AllRack
        //{
        //    get
        //    {
        //        if (all_rack == null) all_rack = AllRackQR.ConvertAll<Rack>(e => e.rack);
        //        return all_rack;
        //    }
        //}

        List<FloorQR> all_station = null;
        public List<FloorQR> AllStationQR
        {
            get
            {
                if (all_station == null) all_station = AllQR.Where(e => e.station_id != "").ToList();
                return all_station;
            }
        }

        List<FloorQR> all_autorator = null;
        public List<FloorQR> AllAutoratorQR
        {
            get
            {
                if (all_autorator == null) all_autorator = AllQR.Where(e => e.autorator_id != "").ToList();
                return all_autorator;
            }
        }

        public FloorQR GetRackQR(string rack_id)
        {
            var qr = AllRackQR.Where(e => e.rack != null && e.rack.rack_no == rack_id);
            if (0 < qr.Count())
            {
                return qr.First();
            }

            return null;
        }

        public FloorQR GetStationQR(string station_id)
        {
            var qr = AllStationQR.Where(e => e.station_id == station_id);
            if (0 < qr.Count())
            {
                return qr.First();
            }

            return null;
        }

        #endregion

        #region フロア操作

        public FloorMap AddFloor(string floor_code)
        {
            if (map.ContainsKey(floor_code)) map.Remove(floor_code);
            map[floor_code] = new FloorMap(this, floor_code);

            return map[floor_code];
        }

        public void RemoveFloor(string floor_code)
        {
            if (map.ContainsKey(floor_code))
            {
                List<FloorAGV> remove_agvs = new List<FloorAGV>();
                foreach (var v in agvs)
                {
                    if (v.on_qr.floor.code == floor_code)
                    {
                        remove_agvs.Add(v);
                    }
                }

                foreach (var v in remove_agvs)
                {
                    map[floor_code].conditioner.RemoveAGV(v);
                }

                map.Remove(floor_code);
            }
        }

        public FloorMap SelectFloor()
        {
            return SelectFloor(selected_floor);
        }

        public FloorMap SelectFloor(string floor_code)
        {
            if (map.ContainsKey(floor_code))
            {
                selected_floor = floor_code;
                return map[floor_code];
            }

            return null;
        }

        #endregion

        #region 描画

        public void Draw(Graphics g)
        {
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.CompositingQuality = CompositingQuality.HighQuality;
            //g.SmoothingMode = SmoothingMode.HighQuality;
            //g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            //g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.Low;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.TextRenderingHint = TextRenderingHint.SystemDefault;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            #region ビットマップ生成

            if (bmp == null)
            {
                int w = 1920;
                int h = 1920;

                foreach (var v in System.Windows.Forms.Screen.AllScreens)
                {
                    if (w < v.Bounds.Width) w = v.Bounds.Width;
                    if (h < v.Bounds.Height) h = v.Bounds.Height;
                }

                bmp = new Bitmap(w, h);
            }

            if (bmpIndicatorH == null)
            {
                bmpIndicatorH = new Bitmap(bmp.Width, indicator_width);
            }

            if (bmpIndicatorV == null)
            {
                bmpIndicatorV = new Bitmap(indicator_width, bmp.Height);
            }

            #endregion

            Graphics gg = Graphics.FromImage(bmp);
            Graphics gh = Graphics.FromImage(bmpIndicatorH);
            Graphics gv = Graphics.FromImage(bmpIndicatorV);

            {
                gg.InterpolationMode = InterpolationMode.Low;
                gg.CompositingQuality = CompositingQuality.HighSpeed;
                gg.SmoothingMode = SmoothingMode.HighSpeed;
                gg.TextRenderingHint = TextRenderingHint.SystemDefault;
                gg.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                gh.InterpolationMode = InterpolationMode.Low;
                gh.CompositingQuality = CompositingQuality.HighSpeed;
                gh.SmoothingMode = SmoothingMode.HighSpeed;
                gh.TextRenderingHint = TextRenderingHint.SystemDefault;
                gh.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                gv.InterpolationMode = InterpolationMode.Low;
                gv.CompositingQuality = CompositingQuality.HighSpeed;
                gv.SmoothingMode = SmoothingMode.HighSpeed;
                gv.TextRenderingHint = TextRenderingHint.SystemDefault;
                gv.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            }

            gh.FillRectangle(Brushes.LightGray, 0, 0, bmpIndicatorH.Width, bmpIndicatorH.Height);
            gv.FillRectangle(Brushes.LightGray, 0, 0, bmpIndicatorV.Width, bmpIndicatorV.Height);

            if (draw_allfloor)
            {
                int count = 0;
                foreach (var m in map.Values)
                {
                    m.Draw(gg, 0 < count);
                    if (count == 0) m.DrawIndicators(gh, gv);
                    count++;
                }
            }
            else
            {
                if (map.ContainsKey(selected_floor))
                {
                    map[selected_floor].Draw(gg);
                    map[selected_floor].DrawIndicators(gh, gv);
                }
            }

            gv.Dispose();
            gh.Dispose();
            gg.Dispose();
            
            g.DrawImage(bmp, 0, 0);
            g.DrawImage(bmpIndicatorV, 0, 0);
            g.DrawImage(bmpIndicatorH, 0, draw_size_pixel.Height - bmpIndicatorH.Height);

            g.FillRectangle(Brushes.LightGray, 0, draw_size_pixel.Height - indicator_width, indicator_width, indicator_width);
        }

        #endregion

        #region 　共通マウス操作

        public void Scale(float scale, PointF center_point)
        {
            var floor = SelectFloor();
            if (floor == null) return;

            float scale_pre = this.draw_scale_g;

            zoom_center.X = floor.rX(center_point.X);
            zoom_center.Y = floor.rY(center_point.Y);

            draw_scale = scale;

            if (scale_pre != draw_scale_g)
            {
                float scale_diff = (draw_scale_g / scale_pre) - 1.0f;
                float cx = zoom_center.X * scale_pre;
                float cy = zoom_center.Y * scale_pre;

                float x_diff = (float)(cx * scale_diff);
                float y_diff = (float)(cy * scale_diff);

                draw_offset_pixel.X -= x_diff;
                draw_offset_pixel.Y -= y_diff;
            }
        }

        public void MouseWheel(MouseEventArgs e)
        {
            if (map.ContainsKey(selected_floor))
            {
                float scale_pre = draw_scale_g;

                if (0 < e.Delta)
                {
                    draw_scale = ((int)(draw_scale * 100) * 1.1f + 0.5f) / 100;
                    if (3.0f < draw_scale) draw_scale = 3.0f;
                    if (0.95f < draw_scale && draw_scale < 1.05f) draw_scale = 1.0f;
                }
                else if (e.Delta < 0)
                {
                    draw_scale = ((int)(draw_scale * 100) * 0.9f + 0.5f) / 100;
                    if (draw_scale < 0.1f) draw_scale = 0.1f;
                    if (0.95f < draw_scale && draw_scale < 1.05f) draw_scale = 1.0f;
                }

                if (scale_pre != draw_scale_g)
                {
                    float scale_diff = (draw_scale_g / scale_pre) - 1.0f;
                    float cx = zoom_center.X * scale_pre;
                    float cy = zoom_center.Y * scale_pre;

                    float x_diff = (float)(cx * scale_diff);
                    float y_diff = (float)(cy * scale_diff);

                    draw_offset_pixel.X -= x_diff;
                    draw_offset_pixel.Y -= y_diff;
                }
            }
        }

        public void DoStack()
        {
            if (is_stuck)
            {
                var target_floor = SelectFloor();
                if (target_floor != null)
                {
                    int nearest_x_key = -1;
                    int nearest_y_key = -1;
                    int nearest_x_distance = int.MaxValue;
                    int nearest_y_distance = int.MaxValue;

                    foreach (var kvx in target_floor.mapeditor.qrs)
                    {
                        if (kvx.Key - stuck_distance <= mousePoint.X && mousePoint.X <= kvx.Key + stuck_distance)
                        {
                            if (Math.Abs(kvx.Key - mousePoint.X) < nearest_x_distance)
                            {
                                nearest_x_distance = (int)Math.Abs(kvx.Key - mousePoint.X);
                                nearest_x_key = kvx.Key;
                            }
                        }

                        foreach (var kvy in kvx.Value)
                        {
                            if (kvy.Key - stuck_distance <= mousePoint.Y && mousePoint.Y <= kvy.Key + stuck_distance)
                            {
                                if (Math.Abs(kvy.Key - mousePoint.Y) < nearest_y_distance)
                                {
                                    nearest_y_distance = (int)Math.Abs(kvy.Key - mousePoint.Y);
                                    nearest_y_key = kvy.Key;
                                }
                            }
                        }
                    }

                    if (0 <= nearest_x_key) mousePoint.X = nearest_x_key;
                    else mousePoint.X -= (mousePoint.X) % stuck_base;
                    if (0 <= nearest_y_key) mousePoint.Y = nearest_y_key;
                    else mousePoint.Y -= (mousePoint.Y) % stuck_base;
                }
            }
        }

        #endregion

        #region 吸着モード切替

        public void Stuck(bool enable)
        {
            is_stuck = enable;
        }

        #endregion

        #region 保存・読込

        public string Save(string filepath)
        {
            string ret = "";

            //try
            {
                if (File.Exists(filepath)) File.Delete(filepath);
                BL_IniFile ini = new BL_IniFile(filepath);

                Program.ini_hokusho.Write("MAP", "OFFSET_X", draw_offset_pixel.X);
                Program.ini_hokusho.Write("MAP", "OFFSET_Y", draw_offset_pixel.Y);
                Program.ini_hokusho.Write("MAP", "SCALE", draw_scale);

                this.filepath = filepath;

                foreach (var kv in moveModes)
                {
                    if (kv.Key == 0) continue;
                    ini.Write("MODE", kv.Key.ToString(), kv.Value.name + "," + kv.Value.option);
                }

                int fno = 1;
                foreach (var kvm in map)
                {
                    string floor_code = kvm.Key;
                    ini.Write("FLOOR", fno.ToString(), floor_code);
                    ini.Write("FLOOR", fno.ToString() + "_TYPE", kvm.Value.is_fa ? "FA" : "LG");

                    var background = kvm.Value.mapeditor.background;
                    var relative = "";

                    if (!string.IsNullOrEmpty(background.fileName))
                    {
                        var exe_dir = Path.GetDirectoryName(Application.ExecutablePath) + @"\";

                        var u_exe = new Uri(exe_dir);
                        var u_bak = new Uri(background.fileName);
                    
                        relative = u_exe.MakeRelativeUri(u_bak).ToString();
                    }

                    ini.Write("FLOOR", $"{floor_code}_BACKGROUND_PATH", relative);
                    ini.Write("FLOOR", $"{floor_code}_BACKGROUND_X", background.BackGroundRX);
                    ini.Write("FLOOR", $"{floor_code}_BACKGROUND_Y", background.BackGroundRY);
                    ini.Write("FLOOR", $"{floor_code}_BACKGROUND_W", background.BackGroundScaleW);
                    ini.Write("FLOOR", $"{floor_code}_BACKGROUND_H", background.BackGroundScaleH);
                    ini.Write("FLOOR", $"{floor_code}_BACKGROUND_ANGLE", background.BackGroundAngle);
                    ini.Write("FLOOR", $"{floor_code}_BACKGROUND_OPACITY", background.BackGroundOpacity);

                    int no = 1;
                    foreach (var kvx in kvm.Value.mapeditor.qrs)
                    {
                        int x = kvx.Key;

                        foreach (var kvy in kvx.Value)
                        {
                            int y = kvy.Key;
                            FloorQR qr = kvy.Value;

                            string key = floor_code + x.ToString("00000") + y.ToString("00000");
                            ini.Write(floor_code, no.ToString(), key);

                            ini.Write(key, "X", qr.x);
                            ini.Write(key, "Y", qr.y);
                            ini.Write(key, "RACKROTATABLE", qr.rack_rotatable);
                            ini.Write(key, "CHARGE", (int)qr.direction_charge);
                            ini.Write(key, "CHARGE_BACK", qr.charge_back);

                            ini.Write(key, "IS_AUTORATOR", qr.autorator_id);
                            
                            ini.Write(key, "STATION", (int)qr.direction_station);
                            ini.Write(key, "STATION_ID", qr.station_id);
                            //ini.Write(key, "STATION_IP", qr.station_ip);
                            //ini.Write(key, "STATION_PORT", qr.station_port);
                            ini.Write(key, "STATION_TYPE", qr.station_type.ToString());
                            
                            ini.Write(key, "RACK_SETABLE", qr.rack_setable);
                            ini.Write(key, "SPEED_LIMIT", qr.speed_limit.ToString());
                            ini.Write(key, "ESCAPE_CHECK", qr.escape_to != null ? qr.escape_to.QrString : "");

                            if (qr.rack != null)
                            {
                                ini.Write(key, "RACK_ID", qr.rack.rack_no);
                                int rno = 1;
                                foreach (var v in qr.rack.face_id)
                                {
                                    ini.Write(key, "RACK_" + rno.ToString() + "_DEG", v.Key);
                                    ini.Write(key, "RACK_" + rno.ToString() + "_ID", v.Value);

                                    //if (qr.rack.can_inout.ContainsKey(v.Key))
                                    //{
                                    //    ini.Write(key, "RACK_" + rno.ToString() + "_INOUT", qr.rack.can_inout[v.Key]);
                                    //}
                                    rno++;
                                }
                            }

                            int nno = 1;
                            foreach (var v in qr.next_way)
                            {
                                ini.Write(key, "NEXT_" + nno.ToString(), v.Key.QrString + "," + v.Value.ToString("#0.000"));
                                ini.Write(key, "ROUTE_TYPE_" + nno.ToString(), qr.next_way_type[v.Key].ToString());
                                nno++;
                            }

                            int pno = 1;
                            foreach (var v in qr.prev_way)
                            {
                                ini.Write(key, "PREV_" + pno.ToString(), v.QrString);
                                pno++;
                            }

                            int wno = 1;
                            foreach (var v in qr.floorwarp_qr)
                            {
                                ini.Write(key, "WARP_" + wno.ToString(), v.QrString);
                                wno++;
                            }

                            no++;
                        }
                    }

                    no = 1;
                    foreach (var agv in agvs)
                    {
                        if (agv.floor.code != floor_code) continue;

                        string key = floor_code + "_AGV" + no.ToString();
                        ini.Write(floor_code + "_AGV", no.ToString(), key);

                        ini.Write(key, "X", agv.x);
                        ini.Write(key, "Y", agv.y);
                        ini.Write(key, "ID", agv.id);
                        ini.Write(key, "DEGREE", agv.degree);


                        foreach (var v in agv.mode_conditions)
                        {
                            string k = key + "_" + v.Key.ToString();
                            int cno = 1;

                            foreach (var vv in v.Value)
                            {
                                ini.Write(k, "F" + cno.ToString(), vv.cur_qr.floor.code);
                                ini.Write(k, "X" + cno.ToString(), vv.cur_qr.x);
                                ini.Write(k, "Y" + cno.ToString(), vv.cur_qr.y);
                                //ini.Write(k, "SPEED" + cno.ToString(), (int)vv.speed);

                                ini.Write(k, "AGVTURN_ARRIVE" + cno.ToString(), vv.agv_turn_arrive);
                                ini.Write(k, "AGVTURN_DEGREE_ARRIVE" + cno.ToString(), vv.agv_turn_arrive_degree);
                                ini.Write(k, "RACK_UP_ARRIVE" + cno.ToString(), vv.rack_up_arrive);
                                ini.Write(k, "RACK_DOWN_ARRIVE" + cno.ToString(), vv.rack_down_arrive);
                                ini.Write(k, "RACK_TURN_ARRIVE" + cno.ToString(), vv.rack_turn_arrive);
                                ini.Write(k, "RACK_TURN_DEGREE_ARRIVE" + cno.ToString(), vv.rack_turn_arrive_degree);

                                ini.Write(k, "AGVTURN_DEPARTURE" + cno.ToString(), vv.agv_turn_departure);
                                ini.Write(k, "AGVTURN_DEGREE_DEPARTURE" + cno.ToString(), vv.agv_turn_departure_degree);
                                ini.Write(k, "RACK_UP_DEPARTURE" + cno.ToString(), vv.rack_up_departure);
                                ini.Write(k, "RACK_DOWN_DEPARTURE" + cno.ToString(), vv.rack_down_departure);
                                ini.Write(k, "RACK_TURN_DEPARTURE" + cno.ToString(), vv.rack_turn_departure);
                                ini.Write(k, "RACK_TURN_DEGREE_DEPARTURE" + cno.ToString(), vv.rack_turn_departure_degree);
                                ini.Write(k, "RACK_DOWN_DEPARTURE_LAST" + cno.ToString(), vv.rack_down_departure_last);

                                ini.Write(k, "RACK_REGULATION" + cno.ToString(), vv.rack_regulation);

                                ini.Write(k, "STATION_TRG" + cno.ToString(), vv.wait_station_trg);
                                ini.Write(k, "AUTORATOR_IN_TRG" + cno.ToString(), vv.wait_autorator_in_trg);
                                ini.Write(k, "AUTORATOR_OUT_TRG" + cno.ToString(), vv.wait_autorator_out_trg);
                                ini.Write(k, "CHARGE_TRG" + cno.ToString(), vv.wait_charge_trg);
                                ini.Write(k, "WAIT_TIMER" + cno.ToString(), vv.wait_timer);

                                int lno = 1;
                                foreach (var vvv in vv.wait_other_agv_locations)
                                {
                                    ini.Write(k, "AGV_LOCATIONS" + cno.ToString() + "_" + lno.ToString(), vvv.Key.ToString() + "," + vvv.Value.X.ToString() + "," + vvv.Value.Y.ToString());
                                    lno++;
                                }

                                cno++;
                            }
                        }
                        no++;
                    }

                    fno++;
                }

            }
            //catch (Exception ex)
            //{
            //    ret = ex.Message;
            //}

            SaveRack();

            return ret;
        }

        public string Load(string filepath)
        {
            string ret = "";
            enApplicateMode appmode_pre = applicate_mode;

            //try
            {
                if (!File.Exists(filepath)) throw new Exception("ファイルが存在しません。");
                BL_IniFile ini = new BL_IniFile(filepath);
                this.filepath = filepath;

                //all_rack_qr = null;
                //all_rack = null;
                all_qr = null;
                all_station = null;
                all_autorator = null;

                Stop();

                foreach (var kv in map)
                {
                    foreach (var kvv in kv.Value.mapeditor.qrs)
                    {
                        kvv.Value.Clear();
                    }
                }

                map.Clear();
                agvs.Clear();

                moveModes.Clear();
                moveModes[0] = new MoveMode();
                for (int mno = 1; ; mno++)
                {
                    string m = ini.Read("MODE", mno.ToString(), "");
                    if (m == "") break;

                    moveModes[mno] = new MoveMode();
                    string[] mm = m.Split(',');

                    moveModes[mno].mode = mno;
                    moveModes[mno].name = mm[0];
                    if (1 < mm.Length) moveModes[mno].option = mm[1];
                }

                string first_floor = "";
                PointF min = new PointF(float.MaxValue, float.MaxValue);

                for (int fno = 1; ; fno++)
                {
                    string floor_code = ini.Read("FLOOR", fno.ToString(), "");
                    if (floor_code == "") break;

                    AddFloor(floor_code);
                    FloorMap floor = SelectFloor(floor_code);
                    if (first_floor == "") first_floor = floor_code;

                    floor.is_fa = ini.Read("FLOOR", fno.ToString() + "TYPE", "LG") != "LG";

                    var background = floor.mapeditor.background;

                    string background_path = ini.Read("FLOOR", $"{floor_code}_BACKGROUND_PATH", "");

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
                            background.fileName = absolute;
                            background.image = new Bitmap(absolute);

                            background.BackGroundRX = ini.Read("FLOOR", $"{floor_code}_BACKGROUND_X", 0.0f);
                            background.BackGroundRY = ini.Read("FLOOR", $"{floor_code}_BACKGROUND_Y", 0.0f);
                            background.BackGroundScaleW = ini.Read("FLOOR", $"{floor_code}_BACKGROUND_W", 1.0f);
                            background.BackGroundScaleH = ini.Read("FLOOR", $"{floor_code}_BACKGROUND_H", 1.0f);
                            background.BackGroundAngle = ini.Read("FLOOR", $"{floor_code}_BACKGROUND_ANGLE", 1.0f);
                            background.BackGroundOpacity = ini.Read("FLOOR", $"{floor_code}_BACKGROUND_OPACITY", 20);
                        }
                        catch { }
                    }

                    Dictionary<FloorQR, string> escape_points = new Dictionary<FloorQR, string>();

                    //int rack_size_max = 0;
                    for (int no = 1; ; no++)
                    {
                        string key = ini.Read(floor_code, no.ToString(), "");
                        if (key == "") break;

                        floor.is_fa = ini.Read(floor_code, "TYPE" + no.ToString(), "LG") != "LG";

                        int x = ini.Read(key, "X", 0);
                        int y = ini.Read(key, "Y", 0);

                        if (x < min.X) min.X = x;
                        if (y < min.Y) min.Y = y;

                        FloorQR qr = new FloorQR(floor.mapeditor, x, y);
                        qr.autorator_id = ini.Read(key, "IS_AUTORATOR", "");

                        //qr.rack_rotatable = ini.Read(key, "RACKROTATABLE", false);
                        qr.direction_charge = (FloorQR.enDirection)ini.Read(key, "CHARGE", -999);
                        qr.charge_back = ini.Read(key, "CHARGE_BACK", false);

                        qr.direction_station = (FloorQR.enDirection)ini.Read(key, "STATION", -999);
                        qr.station_id = ini.Read(key, "STATION_ID", "");
                        //qr.station_ip = ini.Read(key, "STATION_IP", "");
                        //qr.station_port = ini.Read(key, "STATION_PORT", 0);
                        string st = ini.Read(key, "STATION_TYPE", "");
                        if (st != "")
                        {
                            foreach (FloorQR.enStatiionType type in Enum.GetValues(typeof(FloorQR.enStatiionType)))
                            {
                                if (type.ToString() == st)
                                {
                                    qr.station_type = type;
                                    break;
                                }
                            }
                        }

                        qr.rack_setable = ini.Read(key, "RACK_SETABLE", false);

                        string sl = ini.Read(key, "SPEED_LIMIT", "");
                        if (sl != "")
                        {
                            foreach (enSpeed type in Enum.GetValues(typeof(enSpeed)))
                            {
                                if (type.ToString() == sl)
                                {
                                    qr.speed_limit = type;
                                    break;
                                }
                            }
                        }

                        string escape_point = ini.Read(key, "ESCAPE_CHECK", "");
                        if (escape_point.Trim() != "") escape_points[qr] = escape_point;

                        string rackid = ini.Read(key, "RACK_ID", "");
                        if (rackid != "")
                        {
                            qr.rack = RackMaster.Instance.LoadRack(rackid);

                            //qr.rack = new Rack();
                            //qr.rack.rack_no = rackid;
                            //qr.rack.sizeW = ini.Read(key, "RACK_SIZE", qr.rack.sizeW);

                            //if (rack_size_max < qr.rack.sizeW) rack_size_max = qr.rack.sizeW;

                            //for (int rno = 1; ; rno++)
                            //{
                            //    int deg = ini.Read(key, "RACK_" + rno.ToString() + "_DEG", -999);
                            //    if (deg == -999) break;

                            //    string id = ini.Read(key, "RACK_" + rno.ToString() + "_ID", "");

                            //    qr.rack.face_id[deg] = id;
                            //    //qr.rack.can_inout[deg] = ini.Read(key, "RACK_" + rno.ToString() + "_INOUT", true);
                            //}
                        }

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

                    foreach(var kv in escape_points)
                    {
                        FloorQR check_qr = kv.Key;
                        string escape_point = kv.Value;

                        FloorQR escape_qr = floor.mapeditor.list.Where(e => e.QrString == escape_point).FirstOrDefault();
                        //FloorQR escape_qr = floor.controller.AllQR.Where(e => e.QrString == escape_point).FirstOrDefault();
                        if (escape_qr != null)
                        {
                            check_qr.escape_to = escape_qr;
                            escape_qr.escape_from = check_qr;
                        }
                    }

                    //if (rack_size_max != 0) floor.rack_size_max = rack_size_max;

                    for (int no = 1; ; no++)
                    {
                        string key = ini.Read(floor_code, no.ToString(), "");
                        if (key == "") break;

                        float x = ini.Read(key, "X", 0f);
                        float y = ini.Read(key, "Y", 0f);

                        if (x < min.X) min.X = x;
                        if (y < min.Y) min.Y = y;

                        FloorQR qr = floor.mapeditor.Exist(new PointF(x, y));
                        if (qr == null) continue;

                        qr.rack_rotatable = ini.Read(key, "RACKROTATABLE", false);

                        if (qr.autorator_id != "")
                        {
                            if (qr.rack_rotatable && qr.rack != null)
                            {
                                qr.rect.X = x - qr.rack.sizeW / 2;
                                qr.rect.Y = y - qr.rack.sizeW / 2;
                                qr.rect.Width = qr.rack.sizeW;
                                qr.rect.Height = qr.rack.sizeW;
                            }
                            else
                            {
                                qr.rect.X = x - qr.radius;
                                qr.rect.Y = y - qr.radius;
                                qr.rect.Width = qr.radius * 2;
                                qr.rect.Height = qr.radius * 2;
                            }
                        }
                    }

                    for (int no = 1; ; no++)
                    {
                        string key = ini.Read(floor_code, no.ToString(), "");
                        if (key == "") break;

                        // 通路幅
                        for (int wno = 1; ; wno++)
                        {
                            string w = ini.Read(key, "NEXT_WAY_TYPE_" + wno.ToString(), "");
                            if (w == "") break;

                            string[] nw = w.Split(',');
                            if (nw.Length < 2) continue;

                            FloorQR cur_qr = null;
                            FloorQR next_qr = null;

                            foreach (var p in floor.mapeditor.qrs)
                            {
                                foreach (var q in p.Value)
                                {
                                    var qr = q.Value;
                                    if (key == qr.QrString) cur_qr = qr;
                                    if (nw[0] == qr.ToString()) next_qr = qr;
                                }
                            }

                            if (cur_qr != null && next_qr != null && nw[1] != "")
                            {
                                foreach (FloorQR.RouteType way_type in Enum.GetValues(typeof(FloorQR.RouteType)))
                                {
                                    if (way_type.ToString() == nw[1])
                                    {
                                        cur_qr.next_way_type.Add(next_qr, way_type);
                                    }
                                }
                            }
                        }
                    }

                    for (int no = 1; ; no++)
                    {
                        string key = ini.Read(floor_code, no.ToString(), "");
                        if (key == "") break;

                        for (int nno = 1; ; nno++)
                        {
                            string d = ini.Read(key, "NEXT_" + nno.ToString(), "");
                            if (d == "") break;

                            string[] next_key = d.Split(',');
                            if (next_key.Length < 2) continue;

                            FloorQR cur_qr = null;
                            FloorQR next_qr = null;

                            foreach (var kvx in floor.mapeditor.qrs)
                            {
                                int x = kvx.Key;

                                foreach (var kvy in kvx.Value)
                                {
                                    int y = kvy.Key;
                                    FloorQR qr = kvy.Value;

                                    if (key == qr.QrString) cur_qr = qr;
                                    if (next_key[0] == qr.QrString) next_qr = qr;
                                }
                            }

                            if (cur_qr != null && next_qr != null)
                            {
                                var route_type_str = ini.Read(key, "ROUTE_TYPE_" + nno.ToString(), "");

                                Enum.TryParse(route_type_str, out FloorQR.RouteType route_type);
                                
                                cur_qr.AddNextWay(next_qr, route_type);
                            }
                        }
                    }

                    for (int no = 1; ; no++)
                    {
                        string key = ini.Read(floor_code + "_AGV", no.ToString(), "");
                        if (key == "") break;

                        float x = ini.Read(key, "X", 0f);
                        float y = ini.Read(key, "Y", 0f);

                        FloorAGV agv = new FloorAGV(floor, x, y);
                        agv.id = ini.Read(key, "ID", "");
                        //agv.no = no;
                        agv.degree = ini.Read(key, "DEGREE", 0);

                        var qr = floor.mapeditor.HitTest(agv.Location);
                        if (qr != null)
                        {
                            foreach (var kv in moveModes)
                            {
                                if (!agv.mode_conditions.ContainsKey(kv.Key)) agv.mode_conditions[kv.Key] = new RouteConditionList();

                                var v = agv.mode_conditions[kv.Key];

                                RouteCondition con_pre = null;
                                for (int cno = 1; ; cno++)
                                {
                                    string k = key + "_" + kv.Key.ToString();

                                    x = ini.Read(k, "X" + cno.ToString(), float.MinValue);
                                    y = ini.Read(k, "Y" + cno.ToString(), float.MinValue);
                                    if (x < -99999 || y < -99999) break;

                                    string f = ini.Read(k, "F" + cno.ToString(), "");
                                    if (f == "")
                                    {
                                        f = floor_code;
                                        //continue;
                                    }

                                    var con = new RouteCondition();

                                    var fl = SelectFloor(f);
                                    if (fl == null) continue;

                                    con.cur_qr = fl.mapeditor.Exist(new PointF(x, y));
                                    con.owner_agv = agv;

                                    if (con_pre != null)
                                    {
                                        con_pre.next_condition = con;
                                        con.prev_condition = con_pre;
                                    }

                                    //con.speed = (enSpeed)ini.Read(k, "SPEED" + cno.ToString(), (int)con.speed);

                                    con.agv_turn_arrive = ini.Read(k, "AGVTURN_ARRIVE" + cno.ToString(), con.agv_turn_arrive);
                                    con.agv_turn_arrive_degree = ini.Read(k, "AGVTURN_DEGREE_ARRIVE" + cno.ToString(), con.agv_turn_arrive_degree);
                                    con.rack_up_arrive = ini.Read(k, "RACK_UP_ARRIVE" + cno.ToString(), con.rack_up_arrive);
                                    con.rack_down_arrive = ini.Read(k, "RACK_DOWN_ARRIVE" + cno.ToString(), con.rack_down_arrive);
                                    con.rack_turn_arrive = ini.Read(k, "RACK_TURN_ARRIVE" + cno.ToString(), con.rack_turn_arrive);
                                    con.rack_turn_arrive_degree = ini.Read(k, "RACK_TURN_DEGREE_ARRIVE" + cno.ToString(), con.rack_turn_arrive_degree);

                                    con.agv_turn_departure = ini.Read(k, "AGVTURN_DEPARTURE" + cno.ToString(), con.agv_turn_departure);
                                    con.agv_turn_departure_degree = ini.Read(k, "AGVTURN_DEGREE_DEPARTURE" + cno.ToString(), con.agv_turn_departure_degree);
                                    con.rack_up_departure = ini.Read(k, "RACK_UP_DEPARTURE" + cno.ToString(), con.rack_up_departure);
                                    con.rack_down_departure = ini.Read(k, "RACK_DOWN_DEPARTURE" + cno.ToString(), con.rack_down_departure);
                                    con.rack_turn_departure = ini.Read(k, "RACK_TURN_DEPARTURE" + cno.ToString(), con.rack_turn_departure);
                                    con.rack_turn_departure_degree = ini.Read(k, "RACK_TURN_DEGREE_DEPARTURE" + cno.ToString(), con.rack_turn_departure_degree);
                                    con.rack_down_departure_last = ini.Read(k, "RACK_DOWN_DEPARTURE_LAST" + cno.ToString(), con.rack_down_departure_last);
                                    
                                    con.rack_regulation = ini.Read(k, "RACK_REGULATION" + cno.ToString(), con.rack_regulation);

                                    con.wait_station_trg = ini.Read(k, "STATION_TRG" + cno.ToString(), con.wait_station_trg);
                                    con.wait_autorator_in_trg = ini.Read(k, "AUTORATOR_IN_TRG" + cno.ToString(), con.wait_autorator_in_trg);
                                    con.wait_autorator_out_trg = ini.Read(k, "AUTORATOR_OUT_TRG" + cno.ToString(), con.wait_autorator_out_trg);
                                    con.wait_charge_trg = ini.Read(k, "CHARGE_TRG" + cno.ToString(), con.wait_charge_trg);
                                    con.wait_timer = ini.Read(k, "WAIT_TIMER" + cno.ToString(), con.wait_timer);

                                    if (con.cur_qr != null) v.Add(con);

                                    con_pre = con;
                                }
                            }

                            foreach (var v in agv.mode_conditions)
                            {
                                if (v.Value.Count == 0)
                                {
                                    var con = new RouteCondition();

                                    var fl = SelectFloor(agv.floor.code);
                                    if (fl == null) continue;

                                    con.cur_qr = fl.mapeditor.Exist(agv.Location);
                                    con.owner_agv = agv;

                                    v.Value.Add(con);
                                }
                            }

                            agv.on_qr = qr;
                            //qr.on_agv = agv;
                            qr.PlaceAgv(agv);
                            agvs.Add(agv);
                        }
                    }

                    for (int no = 1; ; no++)
                    {
                        string key = ini.Read(floor_code + "_AGV", no.ToString(), "");
                        if (key == "") break;

                        string id = ini.Read(key, "ID", "");

                        FloorAGV agv = null;
                        foreach (var v in agvs)
                        {
                            if (v.floor.code != floor_code) continue;

                            if (v.id == id)
                            {
                                agv = v;
                                break;
                            }
                        }

                        if (agv == null) continue;

                        foreach (var v in agv.mode_conditions)
                        {
                            for (int cno = 1; ; cno++)
                            {
                                string k = key + "_" + v.Key.ToString();

                                if (v.Value.Count <= cno - 1) break;

                                RouteCondition con = v.Value[cno - 1];

                                for (int lno = 1; ; lno++)
                                {
                                    string l = ini.Read(k, "AGV_LOCATIONS" + cno.ToString() + "_" + lno.ToString(), "");
                                    if (l == "") break;

                                    string[] loc = l.Split(',');
                                    if (loc.Length > 3) continue;

                                    int x; if (!int.TryParse(loc[1], out x)) continue;
                                    int y; if (!int.TryParse(loc[2], out y)) continue;

                                    foreach (var a in agvs)
                                    {
                                        if (a.ToString() == loc[0])
                                        {
                                            con.wait_other_agv_locations[a] = new PointF(x, y);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                {
                    var floor = SelectFloor(first_floor);
                    if (floor != null && min.X != float.MaxValue && min.Y != float.MaxValue)
                    {
                        draw_offset_pixel.X = 0;
                        draw_offset_pixel.Y = 0;
                        draw_offset_pixel.X = floor.pX(-min.X + 120);
                        draw_offset_pixel.Y = floor.pY(min.Y - 120) - draw_size_pixel.Height + indicator_offset_pixel.Y;
                    }

                    draw_offset_pixel.X = Program.ini_hokusho.Read("MAP", "OFFSET_X", draw_offset_pixel.X);
                    draw_offset_pixel.Y = Program.ini_hokusho.Read("MAP", "OFFSET_Y", draw_offset_pixel.Y);
                    draw_scale = Program.ini_hokusho.Read("MAP", "SCALE", draw_scale);

                    routeManager.RefreshMap(this);
                }
            }
            //catch (Exception ex)
            //{
            //    ret = ex.Message;
            //}

            if (ret == "")
            {
                LoadAutorator();
                ListupConflictQR();
            }

            if (ret == "")
            {
                LoadRack();

                if (appmode_pre != enApplicateMode.UNKNOWN)
                {
                    Start(appmode_pre, movemode_current, interval_current);
                }
            }

            return ret;
        }

        public void ListupConflictQR()
        {
            agvid_list.Clear();

            for (int i = 1; ; i++)
            {
                AgvControlManager.AgvInfo d = new AgvControlManager.AgvInfo();
                d.id = Program.ini_agv.Read(i.ToString(), "ID", "");
                if (d.id == "") break;

                d.ipaddress = Program.ini_agv.Read(i.ToString(), "IP", "");
                d.hostport = Program.ini_agv.Read(i.ToString(), "REMOTE_HOST", 9000);
                d.clientport = Program.ini_agv.Read(i.ToString(), "REMOTE_CLIENT", 9100);
                d.stop_distance = Program.ini_agv.Read(i.ToString(), "STOP_DISTANCE", 500);
                d.radius = Program.ini_agv.Read(i.ToString(), "RADIUS", 102);

                agvid_list[d.id] = d;
            }

            foreach (var v in agvs)
            {
                var agv = agvid_list.Values.Where(e => e.id == v.id).FirstOrDefault();
                if (agv != null)
                {
                    v.radius = agv.radius / 2;
                }
            }

            //if (0 < agvs.Count)
            //{
            //    var max_size_agv = agvs.OrderByDescending(e => e.radius).FirstOrDefault();
            //    //int agv_rotate_size = (int)(max_size_agv.radius * 2 * Math.Sqrt(2));
            //    int agv_rotate_size = (int)max_size_agv.radius * 2;
            //    //int rack_rotate_size = RackMaster.RackRotateSize;
            //    int rack_rotate_size = RackMaster.RackSizeMax;

            //    foreach (var v in AllQR)
            //    {
            //        v.agv_conflict_qr.Clear();
            //        v.agv_conflict_qr.AddRange(v.floor.mapeditor.list.Where(e => e != v && v.Distance(e) <= agv_rotate_size));

            //        v.rack_conflict_qr.Clear();
            //        v.rack_conflict_qr.AddRange(v.floor.mapeditor.list.Where(e => e != v && v.Distance(e) <= rack_rotate_size));
            //    }
            //}
        }

        public void LoadAutorator()
        {
            if (Program.controller == null) return;

            var autorators = Program.controller.AllAutoratorQR;
            SortedDictionary<string, AgvControlManager.AutoratorController> l = new SortedDictionary<string, AgvControlManager.AutoratorController>();

            SortedDictionary<string, List<int>> degrees = new SortedDictionary<string, List<int>>();
            SortedDictionary<string, List<string>> floors = new SortedDictionary<string, List<string>>();

            foreach (var v in autorators)
            {
                if (!degrees.ContainsKey(v.autorator_id)) degrees[v.autorator_id] = new List<int>();
                if (!floors.ContainsKey(v.autorator_id)) floors[v.autorator_id] = new List<string>();

                if (!floors[v.autorator_id].Contains(v.floor.code))
                {
                    if (0 < v.next_way.Count || 0 < v.prev_way.Count)
                    {
                        floors[v.autorator_id].Add(v.floor.code);
                    }
                }

                foreach (var vv in v.next_way)
                {
                    if (!degrees[v.autorator_id].Contains((int)vv.Value)) degrees[v.autorator_id].Add((int)vv.Value);
                }

                foreach (var vv in v.prev_way)
                {
                    int deg = ((int)vv.next_way[v] + 180) % 360;

                    if (!degrees[v.autorator_id].Contains(deg)) degrees[v.autorator_id].Add(deg);
                }
            }

            foreach (var v in autorators)
            {
                if (!l.ContainsKey(v.autorator_id))
                {
                    AgvControlManager.AutoratorController d = null;
                    bool is_assister = Program.ini_autorator.Read(v.autorator_id, "ASSISTER", false);
                    if (is_assister)
                    {
                        d = new AgvControlManager.AutoratorController_withAssister();
                    }
                    else
                    {
                        d = new AgvControlManager.AutoratorController();
                    }
                    d.id = v.autorator_id;
                    d.ipaddress = Program.ini_autorator.Read(d.id, "IP", "");
                    d.hostport = Program.ini_autorator.Read(d.id, "PORT", 0);
                    
                    d.is_assister = is_assister;
                    d.hostport_assister = Program.ini_autorator.Read(d.id, "PORT_ASSISTER", 0);

                    foreach (var vv in degrees[v.autorator_id])
                    {
                        string code = Program.ini_autorator.Read(d.id, "SIDE_CODE_" + vv.ToString(), "");

                        AgvControlManager.AutoratorController.SideInfo sideinfo = new AgvControlManager.AutoratorController.SideInfo();
                        sideinfo.degree = vv;
                        sideinfo.code = code;
                        d.sideinfo.Add(sideinfo);
                    }

                    l[v.autorator_id] = d;
                }

                l[v.autorator_id].autorator_qrs.Add(v);
                v.autorator_info = l[v.autorator_id];
            }

            foreach (var kv in l)
            {
                kv.Value.floorinfo.Clear();
                foreach (var v in kv.Value.autorator_qrs)
                {
                    if (0 < v.next_way.Count && 0 < v.prev_way.Count)
                    {
                        AgvControlManager.AutoratorController.FloorInfo floorinfo = new AgvControlManager.AutoratorController.FloorInfo();
                        floorinfo.code = v.floor.code;

                        floorinfo.no = Program.ini_autorator.Read(kv.Key, "FLOOR_CODE_" + v.floor.code, 0);
                        floorinfo.is_qrcode = Program.ini_autorator.Read(kv.Key, "QR_CODE" + v.floor.code, false);

                        kv.Value.floorinfo.Add(floorinfo);
                    }
                }
            }

            foreach (var kv in l)
            {
                foreach (var vv in degrees[kv.Key])
                {
                    if (0 == l[kv.Key].sideinfo.Where(e => e.degree == vv).Count())
                    {
                        AutoratorController.SideInfo sideinfo = new AutoratorController.SideInfo();
                        sideinfo.degree = vv;
                        sideinfo.code = "";
                        l[kv.Key].sideinfo.Add(sideinfo);
                    }
                }


                List<int> remove_deg = new List<int>();
                foreach (var vv in l[kv.Key].sideinfo)
                {
                    if (!degrees[kv.Key].Contains(vv.degree)) remove_deg.Add(vv.degree);
                }

                foreach (var v in remove_deg)
                {
                    l[kv.Key].sideinfo.Remove(l[kv.Key].sideinfo.Where(e => e.degree == v).FirstOrDefault());
                }

                foreach (var vv in floors[kv.Key])
                {
                    if (0 == l[kv.Key].floorinfo.Where(e => e.code == vv).Count())
                    {
                        AutoratorController.FloorInfo floorinfo = new AutoratorController.FloorInfo();
                        floorinfo.code = vv;
                        floorinfo.no = 0;
                        l[kv.Key].floorinfo.Add(floorinfo);
                    }
                }

                List<string> remove_floor = new List<string>();
                foreach (var vv in l[kv.Key].floorinfo)
                {
                    if (!floors[kv.Key].Contains(vv.code)) remove_floor.Add(vv.code);
                }

                foreach (var v in remove_floor)
                {
                    l[kv.Key].floorinfo.Remove(l[kv.Key].floorinfo.Where(e => e.code == v).FirstOrDefault());
                }
            }
        }

        public void SaveRack()
        {
            if (filepath == "") return;

            string inipath = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath) + ".rack");

            if (File.Exists(inipath))
            {
                try
                {
                    File.Delete(inipath);
                }
                catch { }
            }

            BL_IniFile ini = new BL_IniFile(inipath);

            try
            {
                int no = 0;
                foreach (var rackqr in AllRackQR)
                {
                    no++;
                    ini.Write("RACK_LAYOUT", no.ToString(), rackqr.rack.rack_no + "," + rackqr.QrString + "," + rackqr.rack.degree.ToString());
                }
            }
            catch { }
        }

        public void LoadRack()
        {
            string inipath = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath) + ".rack");

            if (!File.Exists(inipath)) return;
            BL_IniFile ini = new BL_IniFile(inipath);

            Dictionary<string, Rack> placed_rack = new Dictionary<string, Rack>();

            var allrackqr = AllRackQR.ToList();
            foreach (var v in allrackqr)
            {
                placed_rack[v.rack.rack_no] = v.rack;
                v.rack = null;
            }

            int no = 0;
            while (true)
            {
                no++;
                string rackidqr = ini.Read("RACK_LAYOUT", no.ToString(), "");
                if (rackidqr == "") break;

                string[] idqr = rackidqr.Split(',');
                if (3 <= idqr.Length)
                {
                    string rackid = idqr[0];
                    string qrstring = idqr[1];
                    int deg = int.Parse(idqr[2]);

                    var qr = AllQR.Where(e => e.QrString == qrstring).FirstOrDefault();
                    if (qr != null)
                    {
                        //if (placed_rack.ContainsKey(rackid))
                        //{
                        //    qr.rack = placed_rack[rackid];
                        //}
                        //else
                        {
                            qr.rack = RackMaster.Instance.LoadRack(rackid);
                        }

                        qr.rack.degree = deg;
                    }
                }
            }
        }


        public string Import(string filepath)
        {
            string ret = "";
            enApplicateMode appmode_pre = applicate_mode;

            //try
            {
                if (!File.Exists(filepath)) throw new Exception("ファイルが存在しません。");

                BL_CsvFile file = new BL_CsvFile();
                file.Load(filepath);

                all_qr = null;
                all_station = null;
                all_autorator = null;

                Stop();

                FloorMap floor_now = SelectFloor();

                for (int row = 0; row < file.Count; row++)
                {
                    BL_CommaText line = file[row];
                    if (line.Count < 2) continue;

                    string qrcode = line[1];
                    string floorcode = qrcode.PadRight(12).Substring(0, 1);
                    string xpos = qrcode.PadRight(12).Substring(1, 5);
                    string ypos = qrcode.PadRight(12).Substring(6, 5);
                    string floorcode_sub = qrcode.PadRight(12).Substring(11, 1);

                    FloorMap floor = SelectFloor();
                    if (floorcode.Trim() != "")
                    {
                        floor = SelectFloor(floorcode);
                        if (floor != null && floor_now != null && floor != floor_now) continue;
                        if (floor == null)
                        {
                            floor = AddFloor(floorcode);
                            floor_now = floor;
                        }
                    }

                    int x = 0; int.TryParse(xpos, out x);
                    int y = 0; int.TryParse(ypos, out y);

                    //string stationid = ""; if (3 < line.Count) stationid = line[3].Trim();
                    //int stationdir = 0;
                    //if (4 < line.Count)
                    //{
                    //    stationdir = 0; int.TryParse(line[4].Trim(), out stationdir);
                    //}
                    //string rackrotate = ""; if (5 < line.Count) rackrotate = line[5].Trim();
                    //string rackset = ""; if (6 < line.Count) rackset = line[6].Trim();


                    FloorQR qr = new FloorQR(floor.mapeditor, x, y);
                    //qr.station_id = stationid;
                    //qr.direction_station = (FloorQR.enDirection)stationdir;
                    //qr.rack_rotatable = rackrotate != "";
                    //qr.rack_setable = rackset != "";

                    if (floor.mapeditor.Exist(new PointF(x, y)) == null)
                    {
                        floor.mapeditor.AddQR(qr);
                    }
                }
            }

            if (ret == "")
            {
                if (appmode_pre != enApplicateMode.UNKNOWN)
                {
                    Start(appmode_pre, movemode_current, interval_current);
                }
            }

            return ret;
        }


        #endregion

        #region 処理開始・停止

        public void Start(enApplicateMode appmode, int mode, int interval)
        {
            if (applicate_mode != appmode)
            {
                applicate_mode = appmode;
                movemode_current = mode;
                interval_current = interval;

                if (run_manager != null) run_manager.StopControl();
                run_manager = new AgvRunManager(this);
                run_manager.Start(mode, interval);
            }
        }

        public void Stop()
        {
            applicate_mode = AgvControlManager.enApplicateMode.UNKNOWN;
            if (run_manager == null) return;
            run_manager.Stop();
            run_manager = null;
        }

        #endregion

        #region 動作モード

        public void SetMoveMode(int mode)
        {
            if (run_manager != null)
            {
                movemode_current = mode;
                run_manager.MoveMode(mode);
            }
            else
            {
                foreach (var v in agvs)
                {
                    if (0 < v.mode_conditions[mode].Count)
                    {
                        v.SetLocation("", v.mode_conditions[mode][0].Location);
                    }
                }
            }
        }

        #endregion
    }
}
