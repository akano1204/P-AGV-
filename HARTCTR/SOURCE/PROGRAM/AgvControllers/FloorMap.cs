using System.Drawing;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        #region フロア管理クラス

        public class FloorMap
        {
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
        }

        #endregion
    }
}
