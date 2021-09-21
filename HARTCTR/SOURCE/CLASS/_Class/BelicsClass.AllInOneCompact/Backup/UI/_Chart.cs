using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BelicsClass.UI.Graph
{
    /// <summary>チャートを描画するコントロール</summary>
    public partial class BL_Chart : UserControl
    {
        #region ポイント定義クラス

        /// <summary>
        /// １ポイントデータを保持するクラス
        /// </summary>
        public class BL_ChartPoint
        {
            /// <summary>X軸値</summary>
            public double xvalue = 0;
            /// <summary>Y軸値のリスト</summary>
            public List<double> yvalues = new List<double>();

            /// <summary>Y軸値のリスト</summary>
            public List<double> actualyvalues = null;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public BL_ChartPoint() { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public BL_ChartPoint(double x, params double[] y)
            {
                xvalue = x;
                foreach (double Y in y) yvalues.Add(Y);
            }
        }

        #endregion

        #region 線定義クラス

        /// <summary>
        /// １線のデータを保持する基本クラス
        /// </summary>
        public class BL_ChartSeries
        {
            public class GDI
            {
                [System.Runtime.InteropServices.DllImport("gdi32.dll")]
                internal static extern bool SetPixel(IntPtr hdc, int X, int Y, uint crColor);
            }

            /// <summary>名称</summary>
            public string Name = "";
            /// <summary>境界線のペン</summary>
            public Pen BorderPen = Pens.Black;
            /// <summary>背景色のブラシ</summary>
            public Brush BackBrush = Brushes.RoyalBlue;
            /// <summary>チャート領域の軸描画ペン</summary>
            public Pen CrossPen = Pens.Black;
            /// <summary>凡例描画ブラシ</summary>
            public Brush LegendBrush = Brushes.Black;
            /// <summary>X軸</summary>
            public BL_ChartAxis AxisX = null;
            /// <summary>Y軸</summary>
            public BL_ChartAxis AxisY = null;

            /// <summary>ポイントデータのリスト</summary>
            public List<BL_ChartPoint> Points = new List<BL_ChartPoint>();

            /// <summary>Y軸値を軸ガイド上に表示する</summary>
            public bool IsFollowValueY = false;
            /// <summary>X軸値を軸ガイド上に表示する</summary>
            public bool IsFollowValueX = false;

            /// <summary></summary>
            public float FollowValueY_PositionX = float.NaN;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name"></param>
            /// <param name="axisX"></param>
            /// <param name="axisY"></param>
            /// <param name="border"></param>
            /// <param name="back"></param>
            /// <param name="cross"></param>
            /// <param name="legend"></param>
            public BL_ChartSeries(string name, BL_ChartAxis axisX, BL_ChartAxis axisY, Pen border, Brush back, Pen cross, Brush legend)
            {
                Name = name;
                AxisX = axisX;
                AxisY = axisY;
                BorderPen = border;
                BackBrush = back;
                CrossPen = cross;
                LegendBrush = legend;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public void DrawCross(Graphics g, int width, int height)
            {
                AxisX.DrawCursor(g, width, height, CrossPen);
                AxisX.DrawCross(g, width, height, CrossPen);
                AxisY.DrawCross(g, width, height, CrossPen);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public void DrawCursorValue(Graphics g, int width, int height)
            {
                AxisX.DrawCursorValue(g, width, height, this);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public virtual void Draw(Graphics g, int width, int height)
            {
                Dictionary<int, Dictionary<int, byte>> plot = new Dictionary<int, Dictionary<int, byte>>();

                IntPtr hdc = g.GetHdc();
                uint colorRef = (uint)((BorderPen.Color.B << 16) | (BorderPen.Color.G << 8) | (BorderPen.Color.R));

                float xaxis_min = 0;
                float xaxis_max = width;

                foreach (BL_ChartPoint point in Points)
                {
                    float x = AxisX.GetPixel(width, point.xvalue);
                    float y = AxisY.GetPixel(height, point.yvalues[0]);
                    if (point.actualyvalues != null) y = AxisY.GetPixel(height, point.actualyvalues[0]);

                    if (xaxis_min <= x && x <= xaxis_max)
                    {
                        if (!plot.ContainsKey((int)x)) plot[(int)x] = new Dictionary<int, byte>();

                        if (!plot[(int)x].ContainsKey((int)y))
                        {
                            //g.FillEllipse(BackBrush, x - 2, y - 2, x + 2, y + 2);
                            //g.DrawEllipse(BorderPen, x - 2, y - 2, x + 2, y + 2);

                            GDI.SetPixel(hdc, (int)x, (int)y, colorRef);

                            plot[(int)x][(int)y] = 1;
                        }
                    }
                }

                g.ReleaseHdc(hdc);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public virtual BL_ChartPoint[] Contains(double x, double y)
            {
                return new BL_ChartPoint[0];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="left"></param>
            /// <param name="top"></param>
            /// <param name="needunit"></param>
            /// <returns></returns>
            public SizeF DrawLegend(Graphics g, float left, float top, bool needunit)
            {
                string s = Name;
                if (needunit) s = s + "[" + AxisY.Name + "]";

                SizeF size = g.MeasureString(s, AxisY.Owner.Font);
                RectangleF rect = new RectangleF(left, top, size.Width, size.Height);

                try { g.FillRectangle(BackBrush, rect); }
                catch { }
                try { g.DrawRectangle(BorderPen, rect.X, rect.Y, rect.Width, rect.Height); }
                catch { }
                try { g.DrawString(s, AxisY.Owner.Font, LegendBrush, rect); }
                catch { }

                return size;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="target"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public virtual bool WithIn(double target, double from, double to)
            {
                if (from == target || from < target && target <= to) return true;
                return false;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="pixelX"></param>
            /// <returns></returns>
            public virtual BL_ChartPoint[] GetPoint(int width, float pixelX)
            {
                double xval = AxisX.GetValue(width, pixelX);

                for (int i = Points.Count - 1; 0 < i; i--)
                {
                    BL_ChartPoint point2 = Points[i];
                    BL_ChartPoint point1 = Points[i - 1];

                    if (point2.xvalue < point1.xvalue)
                    {
                        point1 = Points[i];
                        point2 = Points[i - 1];
                    }

                    if (point2.xvalue == xval)
                    {
                        return new BL_ChartPoint[] { point2, point2 };
                    }
                    else if (point1.xvalue == xval)
                    {
                        return new BL_ChartPoint[] { point1, point1 };
                    }
                    else if (point1.xvalue < xval && xval < point2.xvalue)
                    {
                        if (Math.Abs(point1.xvalue - xval) < Math.Abs(point2.xvalue - xval))
                        {
                            return new BL_ChartPoint[] { point1, point1 };
                        }
                        else
                        {
                            return new BL_ChartPoint[] { point2, point2 };
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="pixelX"></param>
            /// <returns></returns>
            public virtual int GetPointIndex(int width, float pixelX)
            {
                double xval = AxisX.GetValue(width, pixelX);

                for (int i = Points.Count - 1; 0 < i; i--)
                {
                    BL_ChartPoint point2 = Points[i];
                    BL_ChartPoint point1 = Points[i - 1];

                    if (point2.xvalue < point1.xvalue)
                    {
                        point1 = Points[i];
                        point2 = Points[i - 1];
                    }

                    if (point2.xvalue == xval)
                    {
                        return i - 1;
                    }
                    else if (point1.xvalue == xval)
                    {
                        return i;
                    }
                    else if (point1.xvalue < xval && xval < point2.xvalue)
                    {
                        if (Math.Abs(point1.xvalue - xval) < Math.Abs(point2.xvalue - xval))
                        {
                            return i;
                        }
                        else
                        {
                            return i - 1;
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="plotwidth"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="positionX"></param>
            public virtual void DrawValueY(Graphics g, int plotwidth, int width, int height, float positionX)
            {
                if (!IsFollowValueY) return;

                List<double> yvals = new List<double>();

                BL_ChartPoint[] points = null;
                if (float.IsNaN(positionX))
                {
                    if (AxisX.RightToLeft)
                    {
                        positionX = AxisX.GetPixel(plotwidth, AxisX.Minimum);
                    }
                    else
                    {
                        positionX = AxisX.GetPixel(plotwidth, AxisX.Maximum);
                    }
                }

                points = GetPoint(plotwidth, positionX);
                if (points == null) return;

                if (points[0].actualyvalues == null)
                {
                    foreach (double y in points[0].yvalues) yvals.Add(y);
                }
                else
                {
                    foreach (double y in points[0].actualyvalues) yvals.Add(y);
                }

                if (0 < yvals.Count)
                {
                    Font font = AxisY.Owner.Font;
                    StringFormat format = new StringFormat();

                    format.Alignment = StringAlignment.Center;

                    foreach (double y in yvals)
                    {
                        string s = AxisY.ToString(y);
                        SizeF size = g.MeasureString(s, font);
                        float yy = AxisY.GetPixel(height, y);

                        try
                        {
                            if (typeof(BL_ChartAxis_Left).IsInstanceOfType(AxisY))
                            {
                                RectangleF rect = new RectangleF(AxisY.DisplayWidth + AxisY.DisplayOffset - AxisY.DisplayWidth / 2 - size.Width / 2, yy - size.Height / 2, size.Width, size.Height);
                                g.DrawLine(BorderPen, width, yy, AxisY.DisplayOffset + AxisY.DisplayWidth / 2 + size.Width / 2, yy);
                                g.FillRectangle(BackBrush, rect);
                                g.DrawRectangle(BorderPen, rect.X, rect.Y, rect.Width, rect.Height);
                                g.DrawString(s, font, LegendBrush, rect, format);
                            }
                            else if (typeof(BL_ChartAxis_Right).IsInstanceOfType(AxisY))
                            {
                                RectangleF rect = new RectangleF(AxisY.DisplayOffset + AxisY.DisplayWidth / 2 - size.Width / 2, yy - size.Height / 2, size.Width, size.Height);
                                g.DrawLine(BorderPen, 0, yy, AxisY.DisplayOffset + AxisY.DisplayWidth / 2 - size.Width / 2, yy);
                                g.FillRectangle(BackBrush, rect);
                                g.DrawRectangle(BorderPen, rect.X, rect.Y, rect.Width, rect.Height);
                                g.DrawString(s, font, LegendBrush, rect, format);
                            }
                        }
                        catch { }
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="positionX"></param>
            public virtual void DrawValueX(Graphics g, int width, float positionX)
            {
                if (!IsFollowValueX) return;

                Font font = AxisX.Owner.Font;
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;

                double xval = 0;
                if (float.IsNaN(positionX))
                {
                    if (Points.Count < 2) return;

                    if (AxisX.RightToLeft)
                    {
                        positionX = AxisX.GetPixel(width, AxisX.Minimum);
                    }
                    else
                    {
                        positionX = AxisX.GetPixel(width, AxisX.Maximum);
                    }
                }

                BL_ChartPoint[] point = GetPoint(width, positionX);
                if (point != null)
                {
                    xval = point[0].xvalue;
                }
                else
                {
                    xval = AxisX.GetValue(width, positionX);
                }

                float xx = AxisX.GetPixel(width, xval);
                string s = AxisX.ToString(xval);
                SizeF size = g.MeasureString(s, font);

                if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(AxisX))
                {
                    try
                    {
                        RectangleF rect = new RectangleF(xx - size.Width / 2, AxisX.DisplayOffset + AxisX.DisplayWidth / 2 - size.Height / 2, size.Width, size.Height);
                        g.DrawLine(BorderPen, xx, 0, xx, AxisX.DisplayOffset + AxisX.DisplayWidth / 2 - size.Height / 2);
                        g.FillRectangle(BackBrush, rect);
                        g.DrawRectangle(BorderPen, rect.X, rect.Y, rect.Width, rect.Height);
                        g.DrawString(s, font, LegendBrush, rect, format);
                    }
                    catch { }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public virtual double[] MinMax()
            {
                double[] minmax = new double[] { double.MaxValue, double.MinValue };
                foreach (var p in Points)
                {
                    if (AxisX.Minimum <= p.xvalue && p.xvalue <= AxisX.Maximum)
                    {
                        if (p.actualyvalues == null)
                        {
                            foreach (var v in p.yvalues)
                            {
                                if (v < minmax[0]) minmax[0] = v;
                                if (minmax[1] < v) minmax[1] = v;
                            }
                        }
                        else
                        {
                            foreach (var v in p.actualyvalues)
                            {
                                if (v < minmax[0]) minmax[0] = v;
                                if (minmax[1] < v) minmax[1] = v;
                            }
                        }
                    }
                }
                return minmax;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="cursorno"></param>
            /// <returns></returns>
            public virtual BL_ChartPoint GetCursorPoints(int width, int cursorno)
            {
                if (AxisX.CursorPosition.Count <= cursorno) return null;

                double xval = AxisX.CursorPosition[cursorno];
                float xpos = AxisX.GetPixel(width, xval);
                BL_ChartPoint[] points = GetPoint(width, xpos);
                if (points != null)
                {
                    return points[0];
                }

                return null;
            }
        }

        /// <summary>
        /// 折れ線
        /// </summary>
		public class BL_ChartSeries_Line : BL_ChartSeries
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="name"></param>
			/// <param name="axisX"></param>
			/// <param name="axisY"></param>
			/// <param name="border"></param>
			/// <param name="back"></param>
			/// <param name="cross"></param>
			/// <param name="legend"></param>
			public BL_ChartSeries_Line(string name, BL_ChartAxis axisX, BL_ChartAxis axisY, Pen border, Brush back, Pen cross, Brush legend)
				: base(name, axisX, axisY, border, back, cross, legend)
			{
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="width"></param>
			/// <param name="height"></param>
			public override void Draw(Graphics g, int width, int height)
			{
				bool bfirst = true;
				float ypre = 0;
				float xpre = 0;
				double xvalpre = 0;

				float yfirst = float.NaN;
				float ymax = float.NaN;
				float ymin = float.NaN;
				float xfirst = float.NaN;
				float xint = float.NaN;
				int pointcount = 0;
				int drawcount = 0;

				float xaxis_min = 0;
				float xaxis_max = width;

				foreach (BL_ChartPoint point in Points)
				{
					try
					{
						float x = AxisX.GetPixel(width, point.xvalue);
						float y = 0;
						if (point.actualyvalues == null)
						{
							y = AxisY.GetPixel(height, point.yvalues[0]);
						}
						else
						{
							y = AxisY.GetPixel(height, point.actualyvalues[0]);
						}

						if (!bfirst)
						{
							if (0 != AxisX.ContinueThreshold && AxisX.ContinueThreshold < Math.Abs(point.xvalue - xvalpre)) bfirst = true;
							//else if (x < xpre) bfirst = true;
							else
							{
								if ((int)x != (int)xint)
								{
									if (1 < pointcount)
									{
										if ((xaxis_min <= x && x <= xaxis_max) || (xaxis_min <= xfirst && xfirst <= xaxis_max) || (xaxis_min <= xpre && xpre <= xaxis_max))
										{
											g.DrawLine(BorderPen, xfirst, yfirst, xpre, ypre);
											g.DrawLine(BorderPen, xpre, ymin, xpre, ymax);
											g.DrawLine(BorderPen, xpre, ypre, x, y);
											drawcount++;
										}

										ymax = float.NaN;
										ymin = float.NaN;
										xint = float.NaN;
									}
									else
									{
										if ((xaxis_min <= x && x <= xaxis_max) || (xaxis_min <= xpre && xpre <= xaxis_max))
										{
											g.DrawLine(BorderPen, xpre, ypre, x, y);
											drawcount++;
										}
									}
									pointcount = 0;
								}
								else
								{
									if (float.IsNaN(ymax) || ymax < y) ymax = y;
									if (float.IsNaN(ymin) || y < ymin) ymin = y;
									pointcount++;
								}
							}
						}

						if (float.IsNaN(xint))
						{
							xint = x;
							xfirst = x;
							yfirst = y;
						}

						xpre = x;
						ypre = y;
						xvalpre = point.xvalue;
						bfirst = false;
					}
					catch
					{
						break;
					}
				}
			}
		}

        /// <summary>
        /// 移動平均折れ線
        /// </summary>
        public class BL_ChartSeries_LineFiltered : BL_ChartSeries_Line
        {
            /// <summary></summary>
            public int FilterPoints = 1;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name"></param>
            /// <param name="axisX"></param>
            /// <param name="axisY"></param>
            /// <param name="border"></param>
            /// <param name="back"></param>
            /// <param name="cross"></param>
            /// <param name="legend"></param>
            /// <param name="filterPoints"></param>
            public BL_ChartSeries_LineFiltered(string name, BL_ChartAxis axisX, BL_ChartAxis axisY, Pen border, Brush back, Pen cross, Brush legend, int filterPoints)
                : base(name, axisX, axisY, border, back, cross, legend)
            {
                FilterPoints = filterPoints;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public override void Draw(Graphics g, int width, int height)
            {
                bool bfirst = true;
                float ypre = 0;
                float xpre = 0;
                double xvalpre = 0;
                double[] yvalues = new double[FilterPoints];
                int moving_pos = 0;
                int moving_count = 0;

                double ytotal = 0;

                foreach (BL_ChartPoint point in Points)
                {
                    ytotal -= yvalues[moving_pos];
                    yvalues[moving_pos] = point.yvalues[0];
                    ytotal += yvalues[moving_pos];

                    if (moving_count < FilterPoints) moving_count++;
                    moving_pos = (moving_pos + 1) % FilterPoints;

                    double yave = ytotal / moving_count;

                    point.actualyvalues = new List<double>();
                    point.actualyvalues.Add(yave);

                    float x = AxisX.GetPixel(width, point.xvalue);
                    float y = AxisY.GetPixel(height, yave);

                    if (!bfirst)
                    {
                        if (0 != AxisX.ContinueThreshold && AxisX.ContinueThreshold < Math.Abs(point.xvalue - xvalpre)) bfirst = true;
                        //else if (x < xpre) bfirst = true;
                        else g.DrawLine(BorderPen, xpre, ypre, x, y);
                    }

                    xpre = x;
                    ypre = y;
                    xvalpre = point.xvalue;
                    bfirst = false;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="plotwidth"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="positionX"></param>
            public override void DrawValueY(Graphics g, int plotwidth, int width, int height, float positionX)
            {
                if (!IsFollowValueY) return;

                List<double> yvals = new List<double>();

                if (float.IsNaN(positionX))
                {
                    if (AxisX.RightToLeft)
                    {
                        positionX = AxisX.GetPixel(plotwidth, AxisX.Minimum);
                    }
                    else
                    {
                        positionX = AxisX.GetPixel(plotwidth, AxisX.Maximum);
                    }
                }

                BL_ChartPoint[] points = GetPoint(plotwidth, positionX);
                if (points == null) return;

                yvals.Add(points[0].actualyvalues[0]);

                if (0 < yvals.Count)
                {
                    Font font = AxisY.Owner.Font;
                    StringFormat format = new StringFormat();

                    format.Alignment = StringAlignment.Center;

                    foreach (double y in yvals)
                    {
                        string s = AxisY.ToString(y);
                        SizeF size = g.MeasureString(s, font);
                        float yy = AxisY.GetPixel(height, y);

                        if (typeof(BL_ChartAxis_Left).IsInstanceOfType(AxisY))
                        {
                            RectangleF rect = new RectangleF(AxisY.DisplayWidth + AxisY.DisplayOffset - AxisY.DisplayWidth / 2 - size.Width / 2, yy - size.Height / 2, size.Width, size.Height);
                            g.DrawLine(BorderPen, width, yy, AxisY.DisplayOffset + AxisY.DisplayWidth / 2 + size.Width / 2, yy);
                            g.FillRectangle(BackBrush, rect);
                            g.DrawRectangle(BorderPen, rect.X, rect.Y, rect.Width, rect.Height);
                            g.DrawString(s, font, LegendBrush, rect, format);
                        }
                        else if (typeof(BL_ChartAxis_Right).IsInstanceOfType(AxisY))
                        {
                            RectangleF rect = new RectangleF(AxisY.DisplayOffset + AxisY.DisplayWidth / 2 - size.Width / 2, yy - size.Height / 2, size.Width, size.Height);
                            g.DrawLine(BorderPen, 0, yy, AxisY.DisplayOffset + AxisY.DisplayWidth / 2 - size.Width / 2, yy);
                            g.FillRectangle(BackBrush, rect);
                            g.DrawRectangle(BorderPen, rect.X, rect.Y, rect.Width, rect.Height);
                            g.DrawString(s, font, LegendBrush, rect, format);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 折れ線＋データポイント＋データ値
        /// </summary>
        public class BL_ChartSeries_LinePoint : BL_ChartSeries_Line
        {
            /// <summary>
            /// 
            /// </summary>
            public enum PointTypes
            {
                /// <summary></summary>
                Ellipse,
                /// <summary></summary>
                Rectangle,
                /// <summary></summary>
                FillEllipse,
                /// <summary></summary>
                FillRectangle,

                /// <summary></summary>
                Ellipse_Value,
                /// <summary></summary>
                Rectangle_Value,
                /// <summary></summary>
                FillEllipse_Value,
                /// <summary></summary>
                FillRectangle_Value,
            }

            /// <summary></summary>
            public int PointPixels = 4;
            /// <summary></summary>
            public Pen PointBorderPen = Pens.Black;
            /// <summary></summary>
            public Brush PointBackBrush = Brushes.RoyalBlue;
            /// <summary></summary>
            public PointTypes PointType = PointTypes.FillEllipse;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="axisX"></param>
            /// <param name="axisY"></param>
            /// <param name="border"></param>
            /// <param name="back"></param>
            /// <param name="cross"></param>
            /// <param name="pointtype"></param>
            /// <param name="pointpixels"></param>
            /// <param name="pointborder"></param>
            /// <param name="pointback"></param>
            /// <param name="legend"></param>
            public BL_ChartSeries_LinePoint(string name, BL_ChartAxis axisX, BL_ChartAxis axisY, Pen border, Brush back, Pen cross, PointTypes pointtype, int pointpixels, Pen pointborder, Brush pointback, Brush legend)
                : base(name, axisX, axisY, border, back, cross, legend)
            {
                PointType = pointtype;
                PointPixels = pointpixels;
                PointBorderPen = pointborder;
                PointBackBrush = pointback;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public override void Draw(Graphics g, int width, int height)
            {
                bool bfirst = true;
                float ypre = 0;
                float xpre = 0;
                double xvalpre = 0;

                foreach (BL_ChartPoint point in Points)
                {
                    float x = AxisX.GetPixel(width, point.xvalue);
                    float y = AxisY.GetPixel(height, point.yvalues[0]);
                    if (point.actualyvalues != null) y = AxisY.GetPixel(height, point.actualyvalues[0]);

                    if (!bfirst)
                    {
                        if (0 != AxisX.ContinueThreshold && AxisX.ContinueThreshold < Math.Abs(point.xvalue - xvalpre)) bfirst = true;
                        //else if (x < xpre) bfirst = true;
                        else
                        {
                            try
                            {
                                g.DrawLine(BorderPen, xpre, ypre, x, y);
                            }
                            catch { }
                        }
                    }

                    xpre = x;
                    ypre = y;
                    xvalpre = point.xvalue;
                    bfirst = false;
                }

				if (0 < PointPixels)
				{
					xpre = ypre = 0;
					int count = 0;
					foreach (BL_ChartPoint point in Points)
					{
						float x = AxisX.GetPixel(width, point.xvalue);
						float y = AxisY.GetPixel(height, point.yvalues[0]);
						if (point.actualyvalues != null) y = AxisY.GetPixel(height, point.actualyvalues[0]);

						try
						{
							switch (PointType)
							{
								case PointTypes.FillEllipse:
								case PointTypes.FillEllipse_Value:
									g.FillEllipse(PointBackBrush, x - PointPixels / 2, y - PointPixels / 2, PointPixels, PointPixels);
									g.DrawEllipse(PointBorderPen, x - PointPixels / 2, y - PointPixels / 2, PointPixels, PointPixels);
									break;
								case PointTypes.Ellipse:
								case PointTypes.Ellipse_Value:
									g.DrawEllipse(PointBorderPen, x - PointPixels / 2, y - PointPixels / 2, PointPixels, PointPixels);
									break;

								case PointTypes.FillRectangle:
								case PointTypes.FillRectangle_Value:
									g.FillRectangle(PointBackBrush, x - PointPixels / 2, y - PointPixels / 2, PointPixels, PointPixels);
									g.DrawRectangle(PointBorderPen, x - PointPixels / 2, y - PointPixels / 2, PointPixels, PointPixels);
									break;
								case PointTypes.Rectangle:
								case PointTypes.Rectangle_Value:
									g.DrawRectangle(PointBorderPen, x - PointPixels / 2, y - PointPixels / 2, PointPixels, PointPixels);
									break;
							}
						}
						catch { }

						try
						{
							switch (PointType)
							{
								case PointTypes.FillEllipse_Value:
								case PointTypes.Ellipse_Value:
								case PointTypes.FillRectangle_Value:
								case PointTypes.Rectangle_Value:
									{
										string s = AxisY.ToString(point.yvalues[0]);
										if (point.actualyvalues != null) s = AxisY.ToString(point.actualyvalues[0]);

										Font f = new Font(AxisY.Owner.Font.Name, AxisY.Owner.Font.Height / 3);
										SizeF size = g.MeasureString(s, f);

										if (count % 2 == 0)
										{
											g.DrawString(s, f, base.BackBrush, x - PointPixels / 2 - size.Width / 2, y - size.Height);
										}
										else
										{
											g.DrawString(s, f, base.BackBrush, x - PointPixels / 2 - size.Width / 2, y + PointPixels);
										}
										f.Dispose();

										count++;
									}
									break;
							}
						}
						catch { }

						xpre = x;
						ypre = y;
					}
				}
            }
        }

        /// <summary>
        /// 棒
        /// </summary>
        public class BL_ChartSeries_Bar : BL_ChartSeries
        {
            /// <summary></summary>
            public double BarWidth = 5;
            /// <summary></summary>
            public List<Brush> SecondaryBacks = new List<Brush>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="axisX"></param>
            /// <param name="axisY"></param>
            /// <param name="border"></param>
            /// <param name="back"></param>
            /// <param name="cross"></param>
            /// <param name="legend"></param>
            /// <param name="barwidth"></param>
            /// <param name="secondary_backs"></param>
            public BL_ChartSeries_Bar(string name, BL_ChartAxis axisX, BL_ChartAxis axisY, Pen border, Brush back, Pen cross, Brush legend, double barwidth, params Brush[] secondary_backs)
                : base(name, axisX, axisY, border, back, cross, legend)
            {
                BarWidth = barwidth;

                foreach (Brush b in secondary_backs) SecondaryBacks.Add(b);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public override void Draw(Graphics g, int width, int height)
            {
                float size = AxisX.GetPixelSize(width, BarWidth);

                try
                {
                    foreach (BL_ChartPoint point in Points)
                    {
                        double fromy = 0;
                        float x1 = AxisX.GetPixel(width, point.xvalue);
                        float x2 = AxisX.GetPixel(width, point.xvalue + BarWidth);

                        int i = -1;
                        foreach (double yval in point.yvalues)
                        {
                            float y1 = AxisY.GetPixel(height, fromy);
                            float y2 = AxisY.GetPixel(height, fromy + yval);

                            float x, y;
                            if (x2 < x1) { x = x1; x1 = x2; x2 = x; }
                            if (y2 < y1) { y = y1; y1 = y2; y2 = y; }

                            Brush br = BackBrush;

                            if (0 <= i && 0 < SecondaryBacks.Count) br = SecondaryBacks[i % SecondaryBacks.Count];

                            g.FillRectangle(br, x1 - size / 2, y1, x2 - x1, y2 - y1);
                            g.DrawRectangle(BorderPen, x1 - size / 2, y1, x2 - x1, y2 - y1);

                            fromy = yval;
                            i++;
                        }
                    }
                }
                catch { }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public override BL_ChartPoint[] Contains(double x, double y)
            {
                List<BL_ChartPoint> points = new List<BL_ChartPoint>();

                foreach (BL_ChartPoint point in Points)
                {
                    foreach (double yval in point.yvalues)
                    {
                        if (point.xvalue - BarWidth / 2 < x && x <= point.xvalue + BarWidth / 2 && y <= yval)
                        {
                            points.Add(point);
                        }
                    }
                }

                return points.ToArray();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override double[] MinMax()
            {
                double[] minmax = new double[] { 0, 0 };
                foreach (var p in Points)
                {
                    foreach (var v in p.yvalues)
                    {
                        if (v < minmax[0]) minmax[0] = v;
                        if (minmax[1] < v) minmax[1] = v;
                    }
                }
                return minmax;
            }
        }

        /// <summary>
        /// バンド枠
        /// </summary>
        public class BL_ChartSeries_Band : BL_ChartSeries
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="axisX"></param>
            /// <param name="axisY"></param>
            /// <param name="border"></param>
            /// <param name="back"></param>
            /// <param name="cross"></param>
            /// <param name="legend"></param>
            public BL_ChartSeries_Band(string name, BL_ChartAxis axisX, BL_ChartAxis axisY, Pen border, Brush back, Pen cross, Brush legend)
                : base(name, axisX, axisY, border, back, cross, legend)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public override void Draw(Graphics g, int width, int height)
            {
                bool bfirst = true;
                float ypre0 = 0;
                float ypre1 = 0;
                float xpre = 0;
                double xvalpre = 0;

                foreach (BL_ChartPoint point in Points)
                {
                    if (point.yvalues.Count < 2) continue;

                    float x = AxisX.GetPixel(width, point.xvalue);
                    float y0 = AxisY.GetPixel(height, point.yvalues[0]);
                    float y1 = AxisY.GetPixel(height, point.yvalues[1]);

                    if (!bfirst)
                    {
                        if (0 != AxisX.ContinueThreshold && AxisX.ContinueThreshold < Math.Abs(point.xvalue - xvalpre)) bfirst = true;
                        //if (x < xpre) bfirst = true;
                        else
                        {
                            PointF[] poi = {
                                new PointF(xpre, ypre0),
                                new PointF(x, y0),
                                new PointF(x, y1),
                                new PointF(xpre, ypre1),
                            };

                            try
                            {
                                g.FillPolygon(BackBrush, poi, System.Drawing.Drawing2D.FillMode.Winding);
                                g.DrawLine(BorderPen, xpre, ypre0, x, y0);
                                g.DrawLine(BorderPen, xpre, ypre1, x, y1);
                            }
                            catch { }
                        }
                    }

                    xpre = x;
                    ypre0 = y0;
                    ypre1 = y1;
                    xvalpre = point.xvalue;
                    bfirst = false;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public override BL_ChartPoint[] Contains(double x, double y)
            {
                List<BL_ChartPoint> points = new List<BL_ChartPoint>();
                double ypre0 = 0;
                double ypre1 = 0;
                double xpre = 0;

                foreach (BL_ChartPoint point in Points)
                {
                    if (xpre < x && x <= point.xvalue && ypre0 < y && y <= point.yvalues[0])
                    {
                        points.Add(point);
                    }

                    if (xpre < x && x <= point.xvalue && ypre1 < y && y <= point.yvalues[1])
                    {
                        points.Add(point);
                    }

                    xpre = point.xvalue;
                    ypre0 = point.yvalues[0];
                    ypre1 = point.yvalues[1];
                }

                return points.ToArray();
            }
        }

        #endregion

        #region 軸定義クラス

        /// <summary>
        /// 
        /// </summary>
        public class BL_ChartAxis
        {
            /// <summary></summary>
            public Control Owner = null;
            /// <summary></summary>
            public string Name = "";
            /// <summary></summary>
            public double Minimum = 0;
            /// <summary></summary>
            public double Maximum = 100;
            /// <summary></summary>
            public List<double> Guides = new List<double>();
            /// <summary></summary>
            public double GuideInner = 0;
            /// <summary></summary>
            public double GuideOuter = 10;
            /// <summary></summary>
            public double GuideOffset = 0;
            /// <summary></summary>
            public int DisplayWidth = 50;
            /// <summary></summary>
            public int DisplayOffset = 0;
            /// <summary></summary>
            public bool RightToLeft = false;
            /// <summary></summary>
            public string IndicateFormat = "";
            /// <summary></summary>
            public int MarginLeft = 0;
            /// <summary></summary>
            public int MarginRight = 0;
            /// <summary></summary>
            public Pen DrawPen = Pens.Black;
            /// <summary></summary>
            public int FontMargin = 0;
            /// <summary></summary>
            public double Scale = 1.0;
            /// <summary></summary>
            public double ContinueThreshold = 0;

            /// <summary>
            /// カーソル位置保持
            /// </summary>
            public List<double> CursorPosition = new List<double>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="guideinner"></param>
            /// <param name="guideouter"></param>
            /// <param name="guideoffset"></param>
            /// <param name="displaywidth"></param>
            /// <param name="displayoffset"></param>
            /// <param name="righttoleft"></param>
            /// <param name="format"></param>
            /// <param name="draw"></param>
            /// <param name="fontmargin"></param>
            /// <param name="leftmargin"></param>
            /// <param name="rightmargin"></param>
            /// <param name="rate"></param>
            public BL_ChartAxis(Control owner, string name, double min, double max, double guideinner, double guideouter, double guideoffset, int displaywidth, int displayoffset, bool righttoleft, string format, Pen draw, int fontmargin, int leftmargin, int rightmargin, double rate)
                : this(owner, name, min, max, guideinner, guideouter, guideoffset, displaywidth, displayoffset, righttoleft, format, draw, fontmargin, leftmargin, rightmargin, rate, 0)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="guideinner"></param>
            /// <param name="guideouter"></param>
            /// <param name="guideoffset"></param>
            /// <param name="displaywidth"></param>
            /// <param name="displayoffset"></param>
            /// <param name="righttoleft"></param>
            /// <param name="format"></param>
            /// <param name="draw"></param>
            /// <param name="fontmargin"></param>
            /// <param name="leftmargin"></param>
            /// <param name="rightmargin"></param>
            /// <param name="rate"></param>
            /// <param name="continue_threshold"></param>
            public BL_ChartAxis(Control owner, string name, double min, double max, double guideinner, double guideouter, double guideoffset, int displaywidth, int displayoffset, bool righttoleft, string format, Pen draw, int fontmargin, int leftmargin, int rightmargin, double rate, double continue_threshold)
            {
                Owner = owner;
                Name = name;
                Minimum = min;
                Maximum = max;
                GuideInner = guideinner;
                GuideOuter = guideouter;
                GuideOffset = guideoffset;
                DisplayWidth = displaywidth;
                DisplayOffset = displayoffset;
                RightToLeft = righttoleft;
                IndicateFormat = format;
                DrawPen = draw;
                FontMargin = fontmargin;
                MarginLeft = leftmargin;
                MarginRight = rightmargin;
                Scale = rate;
                ContinueThreshold = continue_threshold;
            }

			public BL_ChartAxis Clone()
			{
				return new BL_ChartAxis(Owner, Name, Minimum, Maximum, GuideInner, GuideOuter, GuideOffset, DisplayWidth, DisplayOffset, RightToLeft, IndicateFormat, DrawPen, FontMargin, MarginLeft, MarginRight, Scale);
			}

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public float GetPixel(int width, double value)
            {
                double range = Maximum - Minimum;
                if (range == 0) return 0;
                double offset = Minimum;
                int w = width - MarginRight - MarginLeft;

                if (RightToLeft) return w - (float)((double)w / range * (value - offset)) + MarginLeft;
                return (float)((double)w / range * (value - offset)) + MarginLeft;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public float GetPixelSize(int width, double value)
            {
                double range = Maximum - Minimum;
                if (range == 0) return 0;
                double offset = Minimum;

                if (RightToLeft) return width - (float)((double)width / range * value);
                return (float)((double)width / range * value);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="pixel"></param>
            /// <returns></returns>
            public double GetValue(int width, float pixel)
            {
                double range = Maximum - Minimum;
                double offset = Minimum;
                int w = width - MarginRight - MarginLeft;

                if (RightToLeft)
                {
                    double d = (float)(((double)pixel - (double)MarginLeft) / ((double)w / range));
                    return Maximum - d;
                }

                return ((double)pixel - (double)MarginLeft) / ((double)w / range) + offset;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            public virtual void Draw(Graphics g, int width)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public virtual string ToString(double value)
            {
                return (value / Scale).ToString(IndicateFormat);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="crossPen"></param>
            public virtual void DrawCross(Graphics g, int width, int height, Pen crossPen)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="crossPen"></param>
            public virtual void DrawCursor(Graphics g, int width, int height, Pen crossPen)
            {
                //カーソル描画
                foreach (double val in CursorPosition)
                {
                    if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(this))
                    {
                        float xpos = GetPixel(width, val);
                        Pen pen = new Pen(Color.RoyalBlue, 3);
                        g.DrawLine(pen, xpos, 0, xpos, (float)height);
                    }
                    else if (typeof(BL_ChartAxis_Left).IsInstanceOfType(this) || typeof(BL_ChartAxis_Right).IsInstanceOfType(this))
                    {
                        float ypos = GetPixel(height, val);
                        Pen pen = new Pen(Color.RoyalBlue, 3);
                        g.DrawLine(pen, 0, ypos, (float)width, ypos);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="series"></param>
            public virtual void DrawCursorValue(Graphics g, int width, int height, BL_ChartSeries series)
            {
                foreach (double val in CursorPosition)
                {
                    float xpos = series.AxisX.GetPixel(width, val);
                    BL_ChartPoint[] points = series.GetPoint(width, xpos);
                    if (points == null) continue;

                    double xval = points[0].xvalue;
                    float x = series.AxisX.GetPixel(width, xval);

                    double[] yvals = points[0].yvalues.ToArray();
                    if (points[0].actualyvalues != null) yvals = points[0].actualyvalues.ToArray();

                    foreach (double yval in yvals)
                    {
                        float y = series.AxisY.GetPixel(height, yval);
                        string s = series.AxisX.ToString(xval) + "," + series.AxisY.ToString(yval);
                        Font font = new Font(series.AxisY.Owner.Font.Name, series.AxisY.Owner.Font.Height / 2);
                        SizeF size = g.MeasureString(s, font);

                        float yp = y;
                        float xp = xpos;

                        xp += size.Width / 2 + 10;
                        yp += size.Height / 2 + 10;

                        if (width <= xp + size.Width) xp = xpos - size.Width / 2 - 10;
                        if (height <= yp + size.Height) yp = y - size.Height / 2 - 10;

                        RectangleF rect = new RectangleF(xp - size.Width / 2, yp - size.Height / 2, size.Width, size.Height);

                        Pen pen = new Pen(series.BorderPen.Color, 2f);
                        g.DrawLine(pen, xp, yp, x, y);

                        g.FillRectangle(Brushes.White, rect);
                        g.FillRectangle(series.BackBrush, rect);
                        g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
                        g.DrawString(s, font, series.LegendBrush, rect);
                        pen.Dispose();
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="pixel"></param>
            public virtual double AddCursorPosition(int width, float pixel)
            {
                double val = GetValue(width, pixel);
                CursorPosition.Add(val);
                return val;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="val"></param>
            public virtual void AddCursorValue(double val)
            {
                CursorPosition.Add(val);
            }

            /// <summary>
            /// 
            /// </summary>
            public virtual void RemoveLastCursorPosition()
            {
                if (0 < CursorPosition.Count) CursorPosition.RemoveAt(CursorPosition.Count - 1);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="val"></param>
            public virtual int RemoveCursorValue(double val)
            {
                int remove_count = 0;

                for (int i = 0; i < CursorPosition.Count; i++)
                {
                    if (CursorPosition[i] == val)
                    {
                        CursorPosition.RemoveAt(i);
                        i--;
                        remove_count++;
                    }
                }

                return remove_count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class BL_ChartAxis_Bottom : BL_ChartAxis
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="guideinner"></param>
            /// <param name="guideouter"></param>
            /// <param name="guideoffset"></param>
            /// <param name="displaywidth"></param>
            /// <param name="displayoffset"></param>
            /// <param name="righttoleft"></param>
            /// <param name="format"></param>
            /// <param name="draw"></param>
            /// <param name="fontmargin"></param>
            /// <param name="leftmargin"></param>
            /// <param name="rightmargin"></param>
            /// <param name="rate"></param>
            public BL_ChartAxis_Bottom(Control owner, string name, double min, double max, double guideinner, double guideouter, double guideoffset, int displaywidth, int displayoffset, bool righttoleft, string format, Pen draw, int fontmargin, int leftmargin, int rightmargin, double rate)
                : this(owner, name, min, max, guideinner, guideouter, guideoffset, displaywidth, displayoffset, righttoleft, format, draw, fontmargin, leftmargin, rightmargin, rate, 0)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="guideinner"></param>
            /// <param name="guideouter"></param>
            /// <param name="guideoffset"></param>
            /// <param name="displaywidth"></param>
            /// <param name="displayoffset"></param>
            /// <param name="righttoleft"></param>
            /// <param name="format"></param>
            /// <param name="draw"></param>
            /// <param name="fontmargin"></param>
            /// <param name="leftmargin"></param>
            /// <param name="rightmargin"></param>
            /// <param name="rate"></param>
            /// <param name="continue_threshold"></param>
            public BL_ChartAxis_Bottom(Control owner, string name, double min, double max, double guideinner, double guideouter, double guideoffset, int displaywidth, int displayoffset, bool righttoleft, string format, Pen draw, int fontmargin, int leftmargin, int rightmargin, double rate, double continue_threshold)
                : base(owner, name, min, max, guideinner, guideouter, guideoffset, displaywidth, displayoffset, righttoleft, format, draw, fontmargin, leftmargin, rightmargin, rate, continue_threshold)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            public override void Draw(Graphics g, int width)
            {
                //Font font = new Font("Meiryo UI", 8.0f, FontStyle.Regular);
                Font font = Owner.Font;
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;

                double min = Minimum;
                double max = Maximum;
                if (Maximum < Minimum) { min = Maximum; max = Minimum; }

                g.DrawLine(DrawPen, 0, DisplayOffset + 5, (int)width, DisplayOffset + 5);

                float xpre = 0;

                if (min < 0 && 0 < max && 0 != GuideOuter)
                {
                    for (double x = GuideOffset; min <= x; x -= GuideOuter)
                    {
                        float xpos = GetPixel((int)width, x);
                        if (xpos == width) xpos = (float)width - 1.0f;

                        string s = ToString(x);
                        SizeF size = g.MeasureString(s, font);
                        float xx = xpos - size.Width / 2;

                        if (xx < 0) xx = -FontMargin;
                        if (width < xx + size.Width) xx = (float)(width - size.Width + FontMargin);

                        g.DrawLine(DrawPen, xpos, DisplayOffset + 5f, xpos, DisplayOffset + 10f);

                        if (((xpre == 0 || xx + size.Width < xpre) && !RightToLeft) ||
                            ((xpre == 0 || xpre < xx) && RightToLeft))
                        {
                            if (!RightToLeft) xpre = xx;
                            else xpre = xx + size.Width;

                            g.DrawString(s, font, DrawPen.Brush, new RectangleF(xx, DisplayOffset + 12.0f, size.Width, size.Height), format);
                        }
                    }
                    min = 0;
                }

                float xpremax = 0;
                {
                    float xpos = GetPixel((int)width, max);
                    if (xpos == width) xpos = (float)width - 1.0f;
                    double x = max;
                    string s = ToString(x);
                    SizeF size = g.MeasureString(s, font);
                    float xx = xpos - size.Width / 2;

                    if (xx < 0) xx = -FontMargin;
                    if (width < xx + size.Width) xx = (float)(width - size.Width + FontMargin);

                    if (RightToLeft) xpremax = xx + size.Width;
                    else xpremax = xx;

                    g.DrawLine(DrawPen, xpos, DisplayOffset + 5f, xpos, DisplayOffset + 10f);
                    g.DrawString(s, font, DrawPen.Brush, new RectangleF(xx, DisplayOffset + 12.0f, size.Width, size.Height), format);
                }

                if (GuideOuter != 0)
                {
                    xpre = 0;
                    for (double x = min + GuideOffset; x <= max; x += GuideOuter)
                    {
                        float xpos = GetPixel((int)width, x);
                        if (xpos == width) xpos = (float)width - 1.0f;

                        string s = ToString(x);
                        SizeF size = g.MeasureString(s, font);
                        float xx = xpos - size.Width / 2;

                        if (xx < 0) xx = -FontMargin;
                        if (width < xx + size.Width) xx = (float)(width - size.Width + FontMargin);

                        g.DrawLine(DrawPen, xpos, DisplayOffset + 5f, xpos, DisplayOffset + 10f);

                        if (((xpre == 0 || xx + size.Width < xpre) && RightToLeft && (xpremax == 0 || xpremax < xx)) ||
                            ((xpre == 0 || xpre < xx) && !RightToLeft && (xpremax == 0 || xx + size.Width < xpremax)))
                        {
                            if (RightToLeft) xpre = xx;
                            else xpre = xx + size.Width;

                            g.DrawString(s, font, DrawPen.Brush, new RectangleF(xx, DisplayOffset + 12.0f, size.Width, size.Height), format);
                        }
                    }
                }

                foreach (double x in CursorPosition)
                {
                    float xpos = GetPixel((int)width, x);

                    if (xpos == width) xpos = (float)width - 1.0f;

                    string s = ToString(x);
                    SizeF size = g.MeasureString(s, font);
                    float xx = xpos - size.Width / 2;

                    if (0 < xx && xx + size.Width < width)
                    {
                        g.DrawLine(DrawPen, xpos, DisplayOffset + 5f, xpos, DisplayOffset + DisplayWidth / 2 - size.Height / 2);

                        RectangleF rect = new RectangleF(xx, DisplayOffset + DisplayWidth / 2 - size.Height / 2, size.Width, size.Height);
                        g.DrawString(s, font, DrawPen.Brush, rect, format);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="crossPen"></param>
            public override void DrawCross(Graphics g, int width, int height, Pen crossPen)
            {
                base.DrawCross(g, width, height, crossPen);

                double min = Minimum;
                double max = Maximum;
                if (max < min) { min = Maximum; max = Minimum; }

                //Pen pen = (Pen)crossPen.Clone();
                Pen pen = new Pen(Color.FromArgb(80, crossPen.Color), 0.1f);
                //pen.Color = Color.FromArgb(80, pen.Color);
                //pen.Width = 0.1f;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                if (0 < min && 0 < max)
                {
                    if (GuideInner != 0)
                    {
                        for (double x = min; x <= max; x += GuideInner)
                        {
                            float xpos = GetPixel(width, x);
                            g.DrawLine(pen, xpos, 0, xpos, (float)height);
                        }
                    }

                    {
                        float xpos = GetPixel(width, max);
                        g.DrawLine(pen, xpos, 0, xpos, (float)height);
                    }
                }
                else if (min < 0 && max < 0)
                {
                    if (GuideInner != 0)
                    {
                        for (double x = max; min <= x; x -= GuideInner)
                        {
                            float xpos = GetPixel(width, x);
                            g.DrawLine(pen, xpos, 0, xpos, (float)height);
                        }
                    }

                    {
                        float xpos = GetPixel(width, min);
                        g.DrawLine(pen, xpos, 0, xpos, (float)height);
                    }
                }
                else
                {
                    if (GuideInner != 0)
                    {
                        for (double x = 0; min <= x; x -= GuideInner)
                        {
                            float xpos = GetPixel(width, x);
                            g.DrawLine(pen, xpos, 0, xpos, (float)height);
                        }
                    }

                    {
                        float xpos = GetPixel(width, min);
                        g.DrawLine(pen, xpos, 0, xpos, (float)height);
                    }

                    if (GuideInner != 0)
                    {
                        for (double x = GuideInner; x <= max; x += GuideInner)
                        {
                            float xpos = GetPixel(width, x);
                            g.DrawLine(pen, xpos, 0, xpos, (float)height);
                        }
                    }

                    {
                        float xpos = GetPixel(width, max);
                        g.DrawLine(pen, xpos, 0, xpos, (float)height);
                    }
                }

                //pen.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class BL_ChartAxis_Bottom_DateTime : BL_ChartAxis_Bottom
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="guideinner"></param>
            /// <param name="guideouter"></param>
            /// <param name="guideoffset"></param>
            /// <param name="displaywidth"></param>
            /// <param name="displayoffset"></param>
            /// <param name="righttoleft"></param>
            /// <param name="format"></param>
            /// <param name="draw"></param>
            /// <param name="fontmargin"></param>
            /// <param name="leftmargin"></param>
            /// <param name="rightmargin"></param>
            public BL_ChartAxis_Bottom_DateTime(Control owner, string name, double min, double max, double guideinner, double guideouter, double guideoffset, int displaywidth, int displayoffset, bool righttoleft, string format, Pen draw, int fontmargin, int leftmargin, int rightmargin)
                : this(owner, name, min, max, guideinner, guideouter, guideoffset, displaywidth, displayoffset, righttoleft, format, draw, fontmargin, leftmargin, rightmargin, 0)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="guideinner"></param>
            /// <param name="guideouter"></param>
            /// <param name="guideoffset"></param>
            /// <param name="displaywidth"></param>
            /// <param name="displayoffset"></param>
            /// <param name="righttoleft"></param>
            /// <param name="format"></param>
            /// <param name="draw"></param>
            /// <param name="fontmargin"></param>
            /// <param name="leftmargin"></param>
            /// <param name="rightmargin"></param>
            /// <param name="continue_threshold"></param>
            public BL_ChartAxis_Bottom_DateTime(Control owner, string name, double min, double max, double guideinner, double guideouter, double guideoffset, int displaywidth, int displayoffset, bool righttoleft, string format, Pen draw, int fontmargin, int leftmargin, int rightmargin, double continue_threshold)
                : base(owner, name, min, max, guideinner, guideouter, guideoffset, displaywidth, displayoffset, righttoleft, format, draw, fontmargin, leftmargin, rightmargin, 1.0, continue_threshold)
            {
                IndicateFormat = "";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public override string ToString(double value)
            {
                try
                {
                    DateTime dt = new DateTime((long)value);
                    string s = dt.ToString(IndicateFormat);
                    if (IndicateFormat != "") return s;
                    string[] ss = s.Split(' ');
                    s = "";
                    foreach (string sss in ss)
                    {
                        if (s != "") s += "\n";
                        s += sss;
                    }
                    return s;
                }
                catch
                {
                }
                return "";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            public override void Draw(Graphics g, int width)
            {
                //Font font = new Font("Meiryo UI", 8.0f, FontStyle.Regular);
                //Font font = new Font(Owner.Font.FontFamily, Owner.Font.Size - 2f);
                Font font = Owner.Font;
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;

                double min = Minimum;
                double max = Maximum;
                if (Maximum < Minimum) { min = Maximum; max = Minimum; }

                g.DrawLine(DrawPen, 0, DisplayOffset + 5, (int)width, DisplayOffset + 5);

                float xpre = 0;

                if (min < 0 && 0 < max && GuideOuter != 0)
                {
                    for (double x = GuideOffset - min % GuideOuter; min <= x; x -= GuideOuter)
                    {
                        float xpos = GetPixel((int)width, x);
                        if (xpos == width) xpos = (float)width - 1.0f;

                        string s = ToString(x);
                        SizeF size = g.MeasureString(s, font);
                        float xx = xpos - size.Width / 2;

                        if (xx < 0) xx = -FontMargin;
                        if (width < xx + size.Width) xx = (float)(width - size.Width + FontMargin);

                        if (((xpre == 0 || xx + size.Width < xpre) && !RightToLeft) ||
                            ((xpre == 0 || xpre < xx) && RightToLeft))
                        {
                            if (!RightToLeft) xpre = xx;
                            else xpre = xx + size.Width;

                            g.DrawLine(DrawPen, xpos, DisplayOffset + 5f, xpos, DisplayOffset + 10f);
                            g.DrawString(s, font, DrawPen.Brush, new RectangleF(xx, DisplayOffset + 12.0f, size.Width, size.Height), format);
                        }
                    }
                    min = 0;
                }

                xpre = 0;
                {
                    float xpos = GetPixel((int)width, max);
                    if (xpos == width) xpos = (float)width - 1.0f;
                    double x = max;
                    string s = ToString(x);
                    SizeF size = g.MeasureString(s, font);
                    float xx = xpos - size.Width / 2;

                    if (xx < 0) xx = -FontMargin;
                    if (width < xx + size.Width) xx = (float)(width - size.Width + FontMargin);

                    if (!RightToLeft) xpre = xx;
                    else xpre = xx + size.Width;

                    g.DrawLine(DrawPen, xpos, DisplayOffset + 5f, xpos, DisplayOffset + 10f);
                    g.DrawString(s, font, DrawPen.Brush, new RectangleF(xx, DisplayOffset + 12.0f, size.Width, size.Height), format);
                }

                if (GuideOuter != 0)
                {
                    for (double x = max - GuideOffset + GuideOuter - max % GuideOuter; min + GuideOuter <= x; x -= GuideOuter)
                    {
                        float xpos = GetPixel((int)width, x);
                        if (xpos == width) xpos = (float)width - 1.0f;

                        string s = ToString(x);
                        SizeF size = g.MeasureString(s, font);
                        float xx = xpos - size.Width / 2;

                        if (xx < 0) xx = -FontMargin;
                        if (width < xx + size.Width) xx = (float)(width - size.Width + FontMargin);

                        if ((!RightToLeft && (xpre == 0 || xx + size.Width < xpre)) ||
                            (RightToLeft && (xpre == 0 || xpre < xx)))
                        {
                            if (!RightToLeft) xpre = xx;
                            else xpre = xx + size.Width;

                            g.DrawLine(DrawPen, xpos, DisplayOffset + 5f, xpos, DisplayOffset + 10f);
                            g.DrawString(s, font, DrawPen.Brush, new RectangleF(xx, DisplayOffset + 12.0f, size.Width, size.Height), format);
                        }
                    }
                }

                {
                    float xpos = GetPixel((int)width, min);
                    if (xpos == width) xpos = (float)width - 1.0f;

                    string s = ToString(min);
                    SizeF size = g.MeasureString(s, font);
                    float xx = xpos - size.Width / 2;

                    if (xx < 0) xx = -FontMargin;
                    if (width < xx + size.Width) xx = (float)(width - size.Width + FontMargin);

                    if ((!RightToLeft && (xpre == 0 || xx + size.Width < xpre)) ||
                        (RightToLeft && (xpre == 0 || xpre < xx)))
                    {
                        if (!RightToLeft) xpre = xx;
                        else xpre = xx + size.Width;

                        g.DrawLine(DrawPen, xpos, DisplayOffset + 5f, xpos, DisplayOffset + 10f);
                        g.DrawString(s, font, DrawPen.Brush, new RectangleF(xx, DisplayOffset + 12.0f, size.Width, size.Height), format);
                    }
                }
                //font.Dispose();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="crossPen"></param>
            public override void DrawCross(Graphics g, int width, int height, Pen crossPen)
            {
                double min = Minimum;
                double max = Maximum;
                if (max < min) { min = Maximum; max = Minimum; }

                //Pen pen = (Pen)crossPen.Clone();
                Pen pen = new Pen(Color.FromArgb(80, crossPen.Color), 0.1f);
                //pen.Color = Color.FromArgb(80, pen.Color);
                //pen.Width = 0.1f;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                if ((0 <= min && 0 <= max) || (min < 0 && max < 0))
                {
                    if (GuideInner != 0)
                    {
                        for (double x = min + GuideOffset - min % GuideInner; x <= max; x += GuideInner)
                        {
                            float xpos = GetPixel(width, x);
                            g.DrawLine(pen, xpos, 0, xpos, (float)height);
                        }
                    }

                    {
                        float xpos = GetPixel(width, max);
                        g.DrawLine(pen, xpos, 0, xpos, (float)height);
                    }
                }
                else
                {
                    if (GuideInner != 0)
                    {
                        for (double x = 0; min <= x; x -= GuideInner)
                        {
                            float xpos = GetPixel(width, x);
                            g.DrawLine(pen, xpos, 0, xpos, (float)height);
                        }
                    }

                    {
                        float xpos = GetPixel(width, min);
                        g.DrawLine(pen, xpos, 0, xpos, (float)height);
                    }

                    if (GuideInner != 0)
                    {
                        for (double x = GuideInner; x <= max; x += GuideInner)
                        {
                            float xpos = GetPixel(width, x);
                            g.DrawLine(pen, xpos, 0, xpos, (float)height);
                        }
                    }

                    {
                        float xpos = GetPixel(width, max);
                        g.DrawLine(pen, xpos, 0, xpos, (float)height);
                    }
                }

                //pen.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class BL_ChartAxis_Left : BL_ChartAxis
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="guideinner"></param>
            /// <param name="guideouter"></param>
            /// <param name="guideoffset"></param>
            /// <param name="displaywidth"></param>
            /// <param name="displayoffset"></param>
            /// <param name="bottomtoup"></param>
            /// <param name="format"></param>
            /// <param name="draw"></param>
            /// <param name="fontmargin"></param>
            /// <param name="topmargin"></param>
            /// <param name="bottommargin"></param>
            /// <param name="rate"></param>
            public BL_ChartAxis_Left(Control owner, string name, double min, double max, double guideinner, double guideouter, double guideoffset, int displaywidth, int displayoffset, bool bottomtoup, string format, Pen draw, int fontmargin, int topmargin, int bottommargin, double rate)
                : base(owner, name, min, max, guideinner, guideouter, guideoffset, displaywidth, displayoffset, bottomtoup, format, draw, fontmargin, topmargin, bottommargin, rate)
            {
                if (bottomtoup) MarginLeft += Owner.Font.Height;
                else MarginRight += Owner.Font.Height;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            public override void Draw(Graphics g, int width)
            {
                Font font = Owner.Font;
                StringFormat format = new StringFormat();

                format.Alignment = StringAlignment.Center;
                SizeF sz = g.MeasureString(Name, font);
                RectangleF rect = new RectangleF(7 + DisplayOffset, 0, DisplayWidth - 12, sz.Height);

                g.DrawRectangle(DrawPen, rect.X - 2, rect.Y, rect.Width + 2, rect.Height);
                g.DrawString(Name, font, DrawPen.Brush, rect, format);

                double min = Minimum;
                double max = Maximum;
                if (Maximum < Minimum) { min = Maximum; max = Minimum; }

                g.DrawLine(DrawPen, DisplayWidth - 5 + DisplayOffset, 0, DisplayWidth - 5 + DisplayOffset, (float)width);

                float ypre = 0;

                if (min < 0 && 0 < max)
                {
                    if (GuideOuter != 0)
                    {
                        for (double y = GuideOffset; min <= y; y -= GuideOuter)
                        {
                            float ypos = GetPixel((int)width, y);
                            if (ypos == width) ypos = (float)width - 1.0f;

                            string s = ToString(y);
                            SizeF size = g.MeasureString(s, font);
                            float yy = ypos - size.Height / 2;

                            if (yy < 0) yy = -FontMargin;
                            if (width < yy + size.Height) yy = (float)(width - size.Height + FontMargin);

                            if (((ypre == 0 || yy + size.Height < ypre) && !RightToLeft) ||
                                ((ypre == 0 || ypre < yy) && RightToLeft))
                            {
                                if (!RightToLeft) ypre = yy;
                                else ypre = yy + size.Height;

                                g.DrawLine(DrawPen, DisplayWidth - 5 + DisplayOffset, ypos, DisplayWidth - 10 + DisplayOffset, ypos);
                                g.DrawString(s, font, DrawPen.Brush, new RectangleF(DisplayWidth - size.Width - 12 + DisplayOffset, yy, size.Width, size.Height), format);
                            }
                        }
                    }
                    min = 0;
                }

                if (GuideOuter != 0)
                {
                    ypre = 0;
                    for (double y = min + GuideOffset - (min + GuideOffset) % GuideOuter; y < max; y += GuideOuter)
                    {
                        float ypos = GetPixel((int)width, y);
                        if (ypos == width) ypos = (float)width - 1.0f;

                        string s = ToString(y);
                        SizeF size = g.MeasureString(s, font);
                        float yy = ypos - size.Height / 2;

                        if (yy < 0) yy = -FontMargin;
                        if (width < yy + size.Height) yy = (float)(width - size.Height + FontMargin);

                        if (((ypre == 0 || yy + size.Height < ypre) && RightToLeft) ||
                            ((ypre == 0 || ypre < yy) && !RightToLeft))
                        {
                            if (RightToLeft) ypre = yy;
                            else ypre = yy + size.Height;

                            g.DrawLine(DrawPen, DisplayWidth - 5 + DisplayOffset, ypos, DisplayWidth - 10 + DisplayOffset, ypos);
                            g.DrawString(s, font, DrawPen.Brush, new RectangleF(DisplayWidth - size.Width - 12 + DisplayOffset, yy, size.Width, size.Height), format);
                        }
                    }
                }

                {
                    float ypos = GetPixel((int)width, max);
                    if (ypos == width) ypos = (float)width - 1.0f;

                    string s = ToString(max);
                    SizeF size = g.MeasureString(s, font);
                    float yy = ypos - size.Height / 2;

                    if (yy < 0) yy = -FontMargin;
                    if (width < yy + size.Height) yy = (float)(width - size.Height + FontMargin);

                    if (((ypre == 0 || yy + size.Height < ypre) && RightToLeft) ||
                        ((ypre == 0 || ypre < yy) && !RightToLeft))
                    {
                        if (RightToLeft) ypre = yy;
                        else ypre = yy + size.Height;

                        g.DrawLine(DrawPen, DisplayWidth - 5 + DisplayOffset, ypos, DisplayWidth - 10 + DisplayOffset, ypos);
                        g.DrawString(s, font, DrawPen.Brush, new RectangleF(DisplayWidth - size.Width - 12 + DisplayOffset, yy, size.Width, size.Height), format);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="crossPen"></param>
            public override void DrawCross(Graphics g, int width, int height, Pen crossPen)
            {
                base.DrawCross(g, width, height, crossPen);

                double ymin = Minimum;
                double ymax = Maximum;
                if (ymax < ymin) { ymin = Maximum; ymax = Minimum; }

                //Pen pen = (Pen)crossPen.Clone();
                Pen pen = new Pen(Color.FromArgb(80, crossPen.Color), 0.1f);
                //pen.Color = Color.FromArgb(80, pen.Color);
                //pen.Width = 0.1f;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                if (GuideInner != 0)
                {
                    for (double y = 0; ymin <= y; y -= GuideInner)
                    {
                        float ypos = GetPixel(height, y);
                        g.DrawLine(pen, 0, ypos, (float)width, ypos);
                    }
                }

                {
                    float ypos = GetPixel(height, ymin);
                    g.DrawLine(pen, 0, ypos, (float)width, ypos);
                }

                if (GuideInner != 0)
                {
                    for (double y = GuideInner; y <= ymax; y += GuideInner)
                    {
                        float ypos = GetPixel(height, y);
                        g.DrawLine(pen, 0, ypos, (float)width, ypos);
                    }
                }

                {
                    float ypos = GetPixel(height, ymax);
                    g.DrawLine(pen, 0, ypos, (float)width, ypos);
                }

                //pen.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class BL_ChartAxis_Right : BL_ChartAxis
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="guideinner"></param>
            /// <param name="guideouter"></param>
            /// <param name="guideoffset"></param>
            /// <param name="displaywidth"></param>
            /// <param name="displayoffset"></param>
            /// <param name="bottomtoup"></param>
            /// <param name="format"></param>
            /// <param name="draw"></param>
            /// <param name="fontmargin"></param>
            /// <param name="topmargin"></param>
            /// <param name="bottommargin"></param>
            /// <param name="rate"></param>
            public BL_ChartAxis_Right(Control owner, string name, double min, double max, double guideinner, double guideouter, double guideoffset, int displaywidth, int displayoffset, bool bottomtoup, string format, Pen draw, int fontmargin, int topmargin, int bottommargin, double rate)
                : base(owner, name, min, max, guideinner, guideouter, guideoffset, displaywidth, displayoffset, bottomtoup, format, draw, fontmargin, topmargin, bottommargin, rate)
            {
                if (bottomtoup) MarginLeft += Owner.Font.Height;
                else MarginRight += Owner.Font.Height;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            public override void Draw(Graphics g, int width)
            {
                Font font = Owner.Font;
                StringFormat format = new StringFormat();

                format.Alignment = StringAlignment.Center;
                SizeF sz = g.MeasureString(Name, font);
                RectangleF rect = new RectangleF(7 + DisplayOffset, 0, DisplayWidth - 12, sz.Height);
                g.DrawRectangle(DrawPen, rect.X - 2, rect.Y, rect.Width + 2, rect.Height);
                g.DrawString(Name, font, DrawPen.Brush, rect, format);

                double min = Minimum;
                double max = Maximum;
                if (Maximum < Minimum) { min = Maximum; max = Minimum; }

                g.DrawLine(DrawPen, 5 + DisplayOffset, 0, 5 + DisplayOffset, (float)width);

                float ypre = 0;

                if (min < 0 && 0 < max && GuideOuter != 0)
                {
                    for (double y = GuideOffset; min <= y; y -= GuideOuter)
                    {
                        float ypos = GetPixel((int)width, y);
                        if (ypos == width) ypos = (float)width - 1.0f;

                        string s = ToString(y);
                        SizeF size = g.MeasureString(s, font);
                        float yy = ypos - size.Height / 2;

                        if (yy < 0) yy = -FontMargin;
                        if (width < yy + size.Height) yy = (float)(width - size.Height + FontMargin);

                        if (((ypre == 0 || yy + size.Height < ypre) && !RightToLeft) ||
                            ((ypre == 0 || ypre < yy) && RightToLeft))
                        {
                            if (!RightToLeft) ypre = yy;
                            else ypre = yy + size.Height;

                            g.DrawLine(DrawPen, 5 + DisplayOffset, ypos, 10 + DisplayOffset, ypos);
                            g.DrawString(s, font, DrawPen.Brush, new RectangleF(12 + DisplayOffset, yy, size.Width, size.Height), format);
                        }
                    }

                    min = 0;
                }

                if (GuideOuter != 0)
                {
                    ypre = 0;
                    for (double y = min + GuideOffset - (min + GuideOffset) % GuideOuter; y <= max; y += GuideOuter)
                    {
                        float ypos = GetPixel((int)width, y);
                        if (ypos == width) ypos = (float)width - 1.0f;

                        string s = ToString(y);
                        SizeF size = g.MeasureString(s, font);
                        float yy = ypos - size.Height / 2;

                        if (yy < 0) yy = -FontMargin;
                        if (width < yy + size.Height) continue;//yy = (float)(width - size.Height + FontMargin);

                        if (((ypre == 0 || yy + size.Height < ypre) && RightToLeft) ||
                            ((ypre == 0 || ypre < yy) && !RightToLeft))
                        {
                            if (RightToLeft) ypre = yy;
                            else ypre = yy + size.Height;

                            g.DrawLine(DrawPen, 5 + DisplayOffset, ypos, 10 + DisplayOffset, ypos);
                            g.DrawString(s, font, DrawPen.Brush, new RectangleF(12 + DisplayOffset, yy, size.Width, size.Height), format);
                        }
                    }
                }

                {
                    float ypos = GetPixel((int)width, max);
                    if (ypos == width) ypos = (float)width - 1.0f;

                    string s = ToString(max);
                    SizeF size = g.MeasureString(s, font);
                    float yy = ypos - size.Height / 2;

                    if (yy < 0) yy = -FontMargin;
                    if (width < yy + size.Height) yy = (float)(width - size.Height + FontMargin);

                    if (((ypre == 0 || yy + size.Height < ypre) && RightToLeft) ||
                        ((ypre == 0 || ypre < yy) && !RightToLeft))
                    {
                        if (RightToLeft) ypre = yy;
                        else ypre = yy + size.Height;

                        g.DrawLine(DrawPen, 5 + DisplayOffset, ypos, 10 + DisplayOffset, ypos);
                        g.DrawString(s, font, DrawPen.Brush, new RectangleF(12 + DisplayOffset, yy, size.Width, size.Height), format);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="crossPen"></param>
            public override void DrawCross(Graphics g, int width, int height, Pen crossPen)
            {
                base.DrawCross(g, width, height, crossPen);

                double ymin = Minimum;
                double ymax = Maximum;
                if (ymax < ymin) { ymin = Maximum; ymax = Minimum; }

                //Pen pen = (Pen)crossPen.Clone();
                Pen pen = new Pen(Color.FromArgb(80, crossPen.Color), 0.1f);
                //pen.Color = Color.FromArgb(80, pen.Color);
                //pen.Width = 0.1f;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                if (GuideInner != 0)
                {
                    for (double y = 0; ymin <= y; y -= GuideInner)
                    {
                        float ypos = GetPixel(height, y);
                        g.DrawLine(pen, 0, ypos, (float)width, ypos);
                    }
                }

                {
                    float ypos = GetPixel(height, ymin);
                    g.DrawLine(pen, 0, ypos, (float)width, ypos);
                }

                if (GuideInner != 0)
                {
                    for (double y = GuideInner; y <= ymax; y += GuideInner)
                    {
                        float ypos = GetPixel(height, y);
                        g.DrawLine(pen, 0, ypos, (float)width, ypos);
                    }
                }

                {
                    float ypos = GetPixel(height, ymax);
                    g.DrawLine(pen, 0, ypos, (float)width, ypos);
                }

                //pen.Dispose();
            }
        }

        #endregion

        #region フィールド

        Bitmap bmpPlot = null;
        Bitmap bmpLeft = null;
        Bitmap bmpRight = null;
        Bitmap bmpBottom = null;

        /// <summary>
        /// 軸コレクションリスト（拡張版）
        /// </summary>
        public class BL_ChartAxisList : List<BL_ChartAxis>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public BL_ChartAxis this[string key]
            {
                get
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].Name == key) return this[i];
                    }
                    return null;
                }

                set
                {
                    bool found = false;
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].Name == key)
                        {
                            this[i] = value;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        this.Add(value);
                    }
                }
            }
        }

        /// <summary>
        /// シリーズコレクションリスト（拡張版）
        /// </summary>
        public class BL_ChartSeriesList : List<BL_ChartSeries>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public BL_ChartSeries this[string key]
            {
                get
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].Name == key) return this[i];
                    }
                    return null;
                }

                set
                {
                    bool found = false;
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this[i].Name == key)
                        {
                            this[i] = value;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        this.Add(value);
                    }
                }
            }
        }

        /// <summary>軸のコレクション</summary>
        public BL_ChartAxisList dictAxises = new BL_ChartAxisList();
        /// <summary>シリーズのコレクション</summary>
        public BL_ChartSeriesList dictSeries = new BL_ChartSeriesList();

        int axisbottomheight = 0;
        int axisleftwidth = 0;
        int axisrightwidth = 0;

        #endregion

        #region プロパティ

        /// <summary>描画領域内の背景色</summary>
        public Color BackColor_Plot { get { return backColor_Plot; } set { backColor_Plot = value; } }
        Color backColor_Plot = Color.White;

        /// <summary>
        /// 
        /// </summary>
        public bool DebugMode { get { return _debugmode; } set { _debugmode = value; } }
        private bool _debugmode = false;

        /// <summary>
        /// マウスポインターの位置でクロスラインの表示を行います
        /// </summary>
        public bool ShowCrossLine { get { return _showcrossline; } set { _showcrossline = value; } }
        private bool _showcrossline = false;
        private bool showcrossline = false;

        /// <summary>
        /// マウス操作による表示範囲指定の有効・無効を指定します
        /// </summary>
        public bool EnableClip { get { return _enableclip; } set { _enableclip = value; } }
        private bool _enableclip = false;

        /// <summary>
        /// マウス操作による表示範囲指定で、左クリック・右クリックの挙動を独立させます
        /// 左クリック：X軸／右クリック：Y軸
        /// </summary>
        public bool EnableClip_Separate { get { return _enableclip_separate; } set { _enableclip_separate = value; } }
        private bool _enableclip_separate = false;

        /// <summary>
        /// カーソルの有効・無効を設定します
        /// </summary>
        public int CursorCount { get { return _cursorcount; } set { _cursorcount = value; } }
        private int _cursorcount = 0;

		public int TitileHeight
		{
			get
			{
				if (splitContainerHeader.Panel1Collapsed) return 0;
				return splitContainerHeader.SplitterDistance;
			}

			set
			{
				if (value < 1) splitContainerHeader.Panel1Collapsed = true;
				else
				{
					splitContainerHeader.Panel1Collapsed = false;
					splitContainerHeader.SplitterDistance = value;
				}
			}
		}

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_Chart()
        {
            InitializeComponent();
            bmpPlot = new Bitmap(panelPlot.Width, panelPlot.Height);
            bmpLeft = new Bitmap(panelAxisLeft.Width, panelAxisLeft.Height);
            bmpRight = new Bitmap(panelAxisRight.Width, panelAxisRight.Height);
            bmpBottom = new Bitmap(panelAxisBottom.Width, panelAxisBottom.Height);

            //if (!this.IsHandleCreated)
            //{
            //    BL_Chart chart = this;

            //    Pen pen = new Pen(Color.Blue, 2f);
            //    Brush brush = new SolidBrush(Color.FromArgb(80, Color.Yellow));
            //    chart.AddAxis(new BL_Chart.BL_ChartAxis_Bottom(this, "時間", 1, 24, 1, 1, 0, 40, 0, false, "0時", Pens.Black, 0, 40, 40, 1));
            //    chart.AddAxis(new BL_Chart.BL_ChartAxis_Left(this, "kWh", 0, 240, 10, 10, 0, 80, 0, true, "0", Pens.Black, 0, 20, 0, 1));
            //    chart.AddSeries(new BL_Chart.BL_ChartSeries_Band("L3", chart.dictAxises["時間"], chart.dictAxises["kWh"], Pens.DarkOrange, brush, Pens.Lime));
            //    chart.AddSeries(new BL_Chart.BL_ChartSeries_Bar("L1", chart.dictAxises["時間"], chart.dictAxises["kWh"], Pens.Black, Brushes.Lime, Pens.Lime, 0.5, Brushes.LimeGreen));
            //    chart.AddSeries(new BL_Chart.BL_ChartSeries_LinePoint("L2", chart.dictAxises["時間"], chart.dictAxises["kWh"], pen, Brushes.Lime, Pens.Lime, BL_Chart.BL_ChartSeries_LinePoint.PointTypes.FillEllipse, 4, Pens.Black, Brushes.Blue));
            //    chart.Title = "電力量 1";

            //    chart.Title = "電力量 1";
            //    chart.dictSeries["L1"].Points.Clear();

            //    for (int i = 0; i <= 24; i++)
            //    {
            //        chart.dictSeries["L1"].Points.Add(new BL_Chart.BL_ChartPoint(i, i * 10));
            //        chart.dictSeries["L2"].Points.Add(new BL_Chart.BL_ChartPoint(i, i * 10 / 2));
            //        chart.dictSeries["L3"].Points.Add(new BL_Chart.BL_ChartPoint(i, i * 10 / 2 - i * 2, i * 10 / 2 + i * 2));
            //    }

            //    Redraw();
            //}
        }

        #endregion

        #region データクリア処理

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            foreach (var v in dictSeries)
            {
                try
                {
                    v.BackBrush.Dispose();
                    v.LegendBrush.Dispose();
                    v.BorderPen.Dispose();
                    v.CrossPen.Dispose();
                }
                catch { }

                foreach (var vv in v.Points)
                {
                    if (vv.yvalues != null) vv.yvalues.Clear();
                    if (vv.actualyvalues != null) vv.actualyvalues.Clear();
                }
                v.Points.Clear();
            }
            dictSeries.Clear();

            foreach (var v in dictAxises)
            {
                try
                {
                    v.DrawPen.Dispose();
                }
                catch { }

                v.Guides.Clear();
            }
            dictAxises.Clear();

            //splitContainerAxisBottom.SplitterDistance = splitContainerAxisBottom.Size.Height;
            //splitContainerAxisLeft.SplitterDistance = 50;
            //splitContainerAxisBottomLeft.SplitterDistance = 50;
            //splitContainerAxisRight.SplitterDistance = splitContainerAxisRight.Width - 25;
            //splitContainerAxisBottomRight.SplitterDistance = splitContainerAxisBottomRight.Width - 25;

            axisbottomheight = 0;
            axisleftwidth = 0;
            axisrightwidth = 0;
        }

        #endregion

        #region イベント処理

        private void panelAxisBottom_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bmpBottom, 0, 0);
        }

        private void panelAxisLeft_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bmpLeft, 0, 0);
        }

        private void panelAxisRight_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bmpRight, 0, 0);
        }

        private void panelPlot_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bmpPlot, 0, 0);
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, panelPlot.Width - 1, panelPlot.Height - 1);

            if (eX != int.MaxValue || eY != int.MaxValue)
            {
                foreach (BL_ChartSeries series in dictSeries)
                {
                    try
                    {
                    double xvalue = series.AxisX.GetValue(panelPlot.Width, eX);
                    double yvalue = series.AxisY.GetValue(panelPlot.Height, eY);
                    float xpixel = series.AxisX.GetPixel(panelPlot.Width, xvalue);
                    float ypixel = series.AxisY.GetPixel(panelPlot.Height, yvalue);

                    e.Graphics.DrawLine(Pens.DodgerBlue, 0, ypixel, panelPlot.Width, ypixel);
                    e.Graphics.DrawLine(Pens.DodgerBlue, xpixel, 0, xpixel, panelPlot.Height);
                    }
                    catch { }

                    break;
                }
            }
        }

        private void BL_Chart_Resize(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.LButton) return;
            Redraw();
        }

        #endregion

        #region 軸・シリーズ　追加処理

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        public void AddAxis(BL_ChartAxis axis)
        {
            //dictAxises[axis.Name] = axis;
            dictAxises.Add(axis);

            if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(axis))
            {
                if (axisbottomheight < axis.DisplayWidth + axis.DisplayOffset)
                {
                    axisbottomheight = axis.DisplayWidth + axis.DisplayOffset;
                    if (3 < splitContainerAxisBottom.Size.Height - axisbottomheight)
                        splitContainerAxisBottom.SplitterDistance = splitContainerAxisBottom.Size.Height - axisbottomheight;
                    else
                        splitContainerAxisBottom.SplitterDistance = 2;
                }
            }
            if (typeof(BL_ChartAxis_Left).IsInstanceOfType(axis))
            {
                if (axisleftwidth < axis.DisplayWidth + axis.DisplayOffset)
                {
                    axisleftwidth = axis.DisplayWidth + axis.DisplayOffset;
                    splitContainerAxisLeft.SplitterDistance = axisleftwidth;
                    splitContainerAxisBottomLeft.SplitterDistance = axisleftwidth;
                }
            }
            if (typeof(BL_ChartAxis_Right).IsInstanceOfType(axis))
            {
                if (axisrightwidth < axis.DisplayWidth + axis.DisplayOffset)
                {
                    axisrightwidth = axis.DisplayWidth + axis.DisplayOffset;
                    if (3 < splitContainerAxisRight.Width - axisrightwidth)
                        splitContainerAxisRight.SplitterDistance = splitContainerAxisRight.Width - axisrightwidth;

                    if (3 < splitContainerAxisBottomRight.Width - axisrightwidth)
                        splitContainerAxisBottomRight.SplitterDistance = splitContainerAxisBottomRight.Width - axisrightwidth;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        public void AddSeries(BL_ChartSeries series)
        {
            //dictSeries[series.Name] = series;
            dictSeries.Add(series);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowPlotOnly()
        {
            try
            {
                splitContainerHeader.Panel1Collapsed = true;
                splitContainerAxisLeft.Panel1Collapsed = true;
                splitContainerAxisRight.Panel2Collapsed = true;
                splitContainerAxisBottom.Panel2Collapsed = true;
                splitContainerFooter.Panel2Collapsed = true;

                splitContainerPlotBottom.Panel2Collapsed = true;
                splitContainerPlotLeft.Panel1Collapsed = true;
                splitContainerPlotRight.Panel2Collapsed = true;
                splitContainerPlotTop.Panel1Collapsed = true;
            }
            catch
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowAll()
        {
            try
            {
                splitContainerHeader.Panel1Collapsed = false;
                splitContainerAxisLeft.Panel1Collapsed = false;
                splitContainerAxisRight.Panel2Collapsed = false;
                splitContainerAxisBottom.Panel2Collapsed = false;
                //splitContainerFooter.Panel2Collapsed = false;

                //splitContainerPlotBottom.Panel2Collapsed = false;
                //splitContainerPlotLeft.Panel1Collapsed = false;
                //splitContainerPlotRight.Panel2Collapsed = false;
                //splitContainerPlotTop.Panel1Collapsed = false;
            }
            catch
            { }
        }

        #endregion

        #region 再描画処理

        /// <summary>
        /// 
        /// </summary>
        public void Redraw()
        {
            Redraw_Bottom();
            Redraw_Left();
            Redraw_Right();
            Redraw_Plot();

            Invalidate(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Redraw_Left()
        {
            if (bmpLeft != null)
            {
                bmpLeft.Dispose();
                if (0 < panelAxisLeft.Width && 0 < panelAxisLeft.Height)
                {
                    bmpLeft = new Bitmap(panelAxisLeft.Width, panelAxisLeft.Height);
                    Graphics g = Graphics.FromImage(bmpLeft);
                    g.FillRectangle(new SolidBrush(BackColor), 0, 0, panelAxisLeft.Width, panelAxisLeft.Height);

                    foreach (var kv in dictAxises)
                    {
                        if (typeof(BL_ChartAxis_Left).IsInstanceOfType(kv))
                        {
                            kv.Draw(g, panelPlot.Height);
                        }
                    }

                    if (ShowCrossLine)
                    {
                        foreach (var kv in dictSeries)
                        {
                            if (typeof(BL_ChartAxis_Left).IsInstanceOfType(kv.AxisY))
                            {
                                //kv.DrawValueY(g, panelPlot.Width, panelAxisLeft.Width, panelAxisLeft.Height, panelPlot.Width - kv.AxisX.MarginRight  /*kv.AxisX.GetPixel(panelPlot.Width, kv.AxisX.Maximum*/);
                                kv.DrawValueY(g, panelPlot.Width, panelAxisLeft.Width, panelAxisLeft.Height, kv.FollowValueY_PositionX);
                            }
                        }
                    }

                    g.Dispose();
                }
            }

            panelAxisLeft.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Redraw_Right()
        {
            if (bmpRight != null)
            {
                bmpRight.Dispose();

                if (0 < panelAxisRight.Width && 0 < panelAxisRight.Height)
                {
                    bmpRight = new Bitmap(panelAxisRight.Width, panelAxisRight.Height);

                    Graphics g = Graphics.FromImage(bmpRight);
                    g.FillRectangle(new SolidBrush(BackColor), 0, 0, panelAxisRight.Width, panelAxisRight.Height);

                    foreach (var kv in dictAxises)
                    {
                        if (typeof(BL_ChartAxis_Right).IsInstanceOfType(kv))
                        {
                            kv.Draw(g, panelPlot.Height);
                        }
                    }

                    if (ShowCrossLine)
                    {
                        foreach (var kv in dictSeries)
                        {
                            if (typeof(BL_ChartAxis_Right).IsInstanceOfType(kv.AxisY))
                            {
                                //kv.DrawValueY(g, panelPlot.Width, panelAxisRight.Width, panelAxisRight.Height, panelPlot.Width - kv.AxisX.MarginRight  /*kv.AxisX.GetPixel(panelPlot.Width, kv.AxisX.Maximum*/);
                                kv.DrawValueY(g, panelPlot.Width, panelAxisRight.Width, panelAxisRight.Height, kv.FollowValueY_PositionX);
                            }
                        }
                    }

                    g.Dispose();
                }
            }

            panelAxisRight.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Redraw_Bottom()
        {
            if (bmpBottom != null)
            {
                bmpBottom.Dispose();

                if (0 < panelAxisBottom.Width && 0 < panelAxisBottom.Height)
                {
                    bmpBottom = new Bitmap(panelAxisBottom.Width, panelAxisBottom.Height);

                    Graphics g = Graphics.FromImage(bmpBottom);
                    g.FillRectangle(new SolidBrush(BackColor), 0, 0, panelAxisBottom.Width, panelAxisBottom.Height);

                    foreach (var kv in dictAxises)
                    {
                        if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(kv))
                        {
                            kv.Draw(g, panelPlot.Width);
                        }
                    }

                    if (ShowCrossLine)
                    {
                        foreach (var kv in dictSeries)
                        {
                            if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(kv.AxisX))
                            {
                                kv.DrawValueX(g, panelPlot.Width, kv.FollowValueY_PositionX);
                            }
                        }
                    }

                    g.Dispose();
                }
            }

            panelAxisBottom.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Redraw_Plot()
        {
            if (bmpPlot != null)
            {
                bmpPlot.Dispose();
                if (0 < panelPlot.Width && 0 < panelPlot.Height)
                {
                    bmpPlot = new Bitmap(panelPlot.Width, panelPlot.Height);
                    if (bmpPlot == null) return;

                    Graphics g = Graphics.FromImage(bmpPlot);
                    g.FillRectangle(new SolidBrush(backColor_Plot), 0, 0, panelPlot.Width, panelPlot.Height);

                    List<BL_ChartAxis> axislistX = new List<BL_ChartAxis>();
                    List<BL_ChartAxis> axislistY = new List<BL_ChartAxis>();
                    foreach (var kv in dictSeries)
                    {
                        kv.DrawCross(g, panelPlot.Width, panelPlot.Height);
                    }

                    foreach (var kv in dictSeries)
                    {
                        kv.Draw(g, panelPlot.Width, panelPlot.Height);
                    }

                    string unit = "";
                    bool needunit = false;
                    foreach (BL_ChartAxis axis in dictAxises)
                    {
                        if (!typeof(BL_ChartAxis_Bottom).IsInstanceOfType(axis))
                        {
                            if (unit == "") unit = axis.Name;
                            else if (unit != axis.Name)
                            {
                                needunit = true;
                                break;
                            }
                        }
                    }

                    float top = 0f;
                    float left = 0f;
                    foreach (BL_ChartSeries series in dictSeries)
                    {
                        SizeF size = series.DrawLegend(g, left, top, needunit);
                        //top += (int)(size.Height + 2);
                        left += (int)(size.Width + 4);
                        if (panelPlot.Width <= left + size.Width)
                        {
                            left = 0;
                            top += (int)(size.Height + 2);
                        }
                    }

                    if (ShowCrossLine)
                    {
                        foreach (var kv in dictSeries)
                        {
                            if (eX == int.MaxValue)
                            {
                                if (0 < kv.Points.Count)
                                {
                                    double xval = kv.Points[kv.Points.Count - 1].xvalue;
                                    kv.FollowValueY_PositionX = kv.AxisX.GetPixel(panelPlot.Width, xval);
                                }
                            }
                            else
                            {
                                kv.FollowValueY_PositionX = eX;
                            }

                            kv.DrawCursorValue(g, panelPlot.Width, panelPlot.Height);
                        }
                    }

                    g.Dispose();
                }
            }

            panelPlot.Invalidate();
        }

        #endregion

        #region タイトル設定

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return labelTitle.Text; }
            set { labelTitle.Text = value; }
        }

        #endregion

        #region スケール調整処理

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
        public delegate void Event_Handler_RecalculateAxesScale(BL_Chart sender, double min, double max);

		/// <summary>
		/// 
		/// </summary>
        public virtual event Event_Handler_RecalculateAxesScale EventRecalculateAxesScale;

        /// <summary>
        /// 
        /// </summary>
        public void RecalculateAxesScale()
        {
            RecalculateAxesScale(double.MaxValue, double.MinValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RecalculateAxesScale(double min, double max)
        {
            if (EventRecalculateAxesScale != null)
            {
                EventRecalculateAxesScale(this, min, max);
                return;
            }

            Dictionary<string, double[]> minmax = new Dictionary<string, double[]>();

            foreach (var axis in dictAxises)
            {
                foreach (var v in dictSeries)
                {
                    if (v.AxisY == axis)
                    {
                        if (!minmax.ContainsKey(axis.Name))
                        {
                            minmax[axis.Name] = new double[] { min, max };
                        }

                        double[] mm = v.MinMax();
                        if (mm[0] < minmax[axis.Name][0]) minmax[axis.Name][0] = mm[0];
                        if (minmax[axis.Name][1] < mm[1]) minmax[axis.Name][1] = mm[1];
                    }
                }
            }

            foreach (var v in minmax)
            {
                double minvalue = 0;
                double maxvalue = 0;

                if (0 < v.Value[0])
                {
                    double mul = 1;
                    while (mul < v.Value[1])
                    {
                        if (v.Value[0] < mul && mul < v.Value[1])
                        {
                            v.Value[0] = 0;
                            break;
                        }

                        mul = mul * 10;
                    }
                }
                else if (v.Value[0] < 0)
                {
                    double mul = 1;
                    while (mul < v.Value[1])
                    {
                        if (v.Value[1] < mul && mul < v.Value[0])
                        {
                            v.Value[1] = 0;
                            break;
                        }

                        mul = mul * 10;
                    }
                }

                double diff = v.Value[1] - v.Value[0];

                if (v.Value[0] != 0.0 && v.Value[0] != double.MaxValue)
                {
                    if (0 != diff && Math.Abs(diff) < 1.0)
                    {
                        int multi = Math.Abs((int)(1.0 / diff));
                        int power = multi.ToString().Length;
                        int pvalue = (int)Math.Pow(10, power);

                        minvalue = (double)((int)(v.Value[0] * pvalue)) / (double)pvalue;
                    }
                    else
                    {
                        int multi = Math.Abs((int)v.Value[0]);
                        int power = multi.ToString().Length;
                        int pvalue = (int)Math.Pow(10, power - 1);
                        double minus = v.Value[0] % pvalue;
                        if (minus == 0.0) minus = pvalue;
                        minvalue = v.Value[0] - minus;
                    }
                }

                if (v.Value[1] != 0.0 && v.Value[1] != double.MinValue)
                {
                    if (0 != diff && Math.Abs(diff) < 1.0)
                    {
                        int multi = Math.Abs((int)(1.0 / diff));
                        int power = multi.ToString().Length;
                        int pvalue = (int)Math.Pow(10, power);

                        maxvalue = (double)((int)(v.Value[1] * pvalue + 1)) / (double)pvalue;
                    }
                    else
                    {
                        int multi = Math.Abs((int)v.Value[1]);
                        int power = multi.ToString().Length;
                        int pvalue = (int)Math.Pow(10, power - 1);
                        double plus = pvalue - v.Value[1] % pvalue;
                        maxvalue = v.Value[1] + plus;
                    }
                }


                dictAxises[v.Key].Minimum = minvalue;
                dictAxises[v.Key].Maximum = maxvalue;

                int width = (int)(maxvalue - minvalue + 0.99999);
                double div = 1;

                if (1.0 < width)
                {
                    while (10 < width / div)
                    {
                        div = div * 10;
                    }
                }
                else
                {
                    div = 0.1;
                    while (10 < width / div)
                    {
                        div = div / 10;
                    }
                }


                dictAxises[v.Key].GuideInner = (maxvalue - minvalue) / (width / div);
                dictAxises[v.Key].GuideOuter = (maxvalue - minvalue) / (width / div);
            }
        }

        #endregion

        #region クリッピング処理

        private PointF clipStaX = new PointF();
        private PointF clipEndX = new PointF();
        private bool clippingX = false;

        private PointF clipStaY = new PointF();
        private PointF clipEndY = new PointF();
        private bool clippingY = false;

        private int eX = 0;
        private int eY = 0;

        private void panelPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (clippingX || clippingY || _showcrossline || _debugmode)
            {
                Graphics g = panelPlot.CreateGraphics();
                g.DrawImage(bmpPlot, 0, 0);

                if (clippingX && clippingY)
                {
                    clipEndX = e.Location;
                    clipEndY = e.Location;

                    PointF sta = clipStaX;
                    PointF end = clipEndX;
                    sta.Y = clipStaY.Y;
                    end.Y = clipEndY.Y;

                    if (end.X < sta.X)
                    {
                        float temp = sta.X;
                        sta.X = end.X;
                        end.X = temp;
                    }

                    if (end.Y < sta.Y)
                    {
                        float temp = sta.Y;
                        sta.Y = end.Y;
                        end.Y = temp;
                    }

                    SolidBrush br = new SolidBrush(Color.FromArgb(30, Color.DodgerBlue));
                    g.FillRectangle(br, sta.X, sta.Y, end.X - sta.X, end.Y - sta.Y);
                    g.DrawRectangle(Pens.DodgerBlue, sta.X, sta.Y, end.X - sta.X, end.Y - sta.Y);
                }
                else if (clippingX)
                {
                    clipEndX = e.Location;

                    PointF sta = clipStaX;
                    PointF end = clipEndX;

                    if (end.X < sta.X)
                    {
                        float temp = sta.X;
                        sta.X = end.X;
                        end.X = temp;
                    }

                    SolidBrush br = new SolidBrush(Color.FromArgb(30, Color.DodgerBlue));
                    g.FillRectangle(br, sta.X, 0, end.X - sta.X, panelPlot.Height);
                    g.DrawRectangle(Pens.DodgerBlue, sta.X, 0, end.X - sta.X, panelPlot.Height);
                }
                else if (clippingY)
                {
                    clipEndY = e.Location;

                    PointF sta = clipStaY;
                    PointF end = clipEndY;

                    if (end.Y < sta.Y)
                    {
                        float temp = sta.Y;
                        sta.Y = end.Y;
                        end.Y = temp;
                    }

                    SolidBrush br = new SolidBrush(Color.FromArgb(30, Color.DodgerBlue));
                    g.FillRectangle(br, 0, sta.Y, panelPlot.Width, end.Y - sta.Y);
                    g.DrawRectangle(Pens.DodgerBlue, 0, sta.Y, panelPlot.Width, end.Y - sta.Y);
                }

                if (_showcrossline)
                {
                    eX = e.X;
                    eY = e.Y;

                    bool isproceed = false;
                    foreach (BL_ChartSeries series in dictSeries)
                    {
                        series.FollowValueY_PositionX = e.X;

                        if (!isproceed)
                        {
                            isproceed = true;

                            double xvalue = series.AxisX.GetValue(panelPlot.Width, eX);
                            double yvalue = series.AxisY.GetValue(panelPlot.Height, eY);
                            float xpixel = series.AxisX.GetPixel(panelPlot.Width, xvalue);
                            float ypixel = series.AxisY.GetPixel(panelPlot.Height, yvalue);

                            g.DrawLine(Pens.DodgerBlue, 0, ypixel, panelPlot.Width, ypixel);
                            g.DrawLine(Pens.DodgerBlue, xpixel, 0, xpixel, panelPlot.Height);
                        }
                    }

                    Redraw_Left();
                    Redraw_Right();
                    Redraw_Bottom();
                }

                if (_debugmode)
                {
                    float ypos = 60;
                    foreach (BL_ChartSeries series in dictSeries)
                    {
                        BL_ChartPoint[] point = series.GetPoint(panelPlot.Width, e.X);
                        string s = "                    ";
                        if (point != null)
                        {
                            s = "(" + point[0].xvalue.ToString("0.00") + "," + point[0].yvalues[0].ToString("0.00");
                            s += ")～(" + point[1].xvalue.ToString("0.00") + "," + point[1].yvalues[0].ToString("0.00") + ")";
                        }

                        RectangleF rect = new RectangleF(20f, ypos, 500f, 30f);
                        g.DrawString(s, this.Font, series.BackBrush, rect);
                        ypos += 30;
                    }
                }
            }
        }

        private void panelPlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (_enableclip)
            {
                if (_enableclip_separate)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left && !clippingX)
                    {
                        clippingX = true;
                        clipStaX = e.Location;
                    }
                    else if (e.Button == System.Windows.Forms.MouseButtons.Right && !clippingY)
                    {
                        clippingY = true;
                        clipStaY = e.Location;
                    }
                }
                else
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left && !clippingX)
                    {
                        clippingX = true;
                        clipStaX = e.Location;
                        clippingY = true;
                        clipStaY = e.Location;
                    }
                }
            }
        }

        private void panelPlot_MouseEnter(object sender, EventArgs e)
        {
            if (clippingX || clippingY) return;

            showcrossline = _showcrossline;
        }

        private void panelPlot_MouseLeave(object sender, EventArgs e)
        {
            eX = int.MaxValue;
            eY = int.MaxValue;

            if (clippingX || clippingY) return;

            showcrossline = false;

            Graphics g = panelPlot.CreateGraphics();
            g.DrawImage(bmpPlot, 0, 0);

            foreach (BL_ChartSeries series in dictSeries)
            {
                series.FollowValueY_PositionX = float.NaN;
            }

            Redraw_Left();
            Redraw_Right();
            Redraw_Bottom();
        }

        private void panelPlot_MouseUp(object sender, MouseEventArgs e)
        {
            if (clippingX)
            {
                clipEndX = e.Location;

                if (clipEndX.X < clipStaX.X)
                {
                    foreach (BL_ChartSeries series in dictSeries)
                    {
                        double xmin = double.MaxValue;
                        double xmax = double.MinValue;
                        double ymin = double.MaxValue;
                        double ymax = double.MinValue;

                        foreach (BL_ChartPoint point in series.Points)
                        {
                            if (point.xvalue < xmin) xmin = point.xvalue;
                            if (xmax < point.xvalue) xmax = point.xvalue;

                            double[] yvals = point.yvalues.ToArray();
                            if (point.actualyvalues != null) yvals = point.actualyvalues.ToArray();

                            foreach (double y in yvals)
                            {
                                if (y < ymin) ymin = y;
                                if (ymax < y) ymax = y;
                            }
                        }

                        if (xmin < series.AxisX.Minimum) series.AxisX.Minimum = xmin;
                        if (series.AxisX.Maximum < xmax) series.AxisX.Maximum = xmax;

                        if (ymin < series.AxisY.Minimum) series.AxisY.Minimum = ymin;
                        if (series.AxisY.Maximum < ymax) series.AxisY.Maximum = ymax;
                    }


                    clippingX = false;
                    clippingY = false;

                    RecalculateAxesScale();
                    Redraw();
                    return;
                }
            }

            if (clippingX && clippingY)
            {
                clipEndX = e.Location;
                clipEndY = e.Location;

                if (clipEndX.X == clipStaX.X || clipEndY.Y == clipStaY.Y)
                {
                    clippingX = false;
                    clippingY = false;
                    return;
                }

                Graphics g = panelPlot.CreateGraphics();
                g.DrawImage(bmpPlot, 0, 0);

                foreach (BL_ChartAxis axis in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(axis))
                    {
                        double min = axis.GetValue(panelAxisBottom.Width, clipStaX.X);
                        double max = axis.GetValue(panelAxisBottom.Width, clipEndX.X);

                        if (axis.RightToLeft)
                        {
                            axis.Minimum = max;
                            axis.Maximum = min;
                        }
                        else
                        {
                            axis.Minimum = min;
                            axis.Maximum = max;
                        }
                    }
                    else if (typeof(BL_ChartAxis_Left).IsInstanceOfType(axis))
                    {
                        double min = axis.GetValue(panelAxisLeft.Height, clipStaY.Y);
                        double max = axis.GetValue(panelAxisLeft.Height, clipEndY.Y);
                        if (min < max)
                        {
                            axis.Minimum = min;
                            axis.Maximum = max;
                        }
                        else
                        {
                            axis.Minimum = max;
                            axis.Maximum = min;
                        }
                    }
                    else if (typeof(BL_ChartAxis_Right).IsInstanceOfType(axis))
                    {
                        double min = axis.GetValue(panelAxisRight.Height, clipStaY.Y);
                        double max = axis.GetValue(panelAxisRight.Height, clipEndY.Y);
                        if (min < max)
                        {
                            axis.Minimum = min;
                            axis.Maximum = max;
                        }
                        else
                        {
                            axis.Minimum = max;
                            axis.Maximum = min;
                        }
                    }
                }

                clippingX = false;
                clippingY = false;

                Redraw();
            }
            else if (clippingX)
            {
                clipEndX = e.Location;

                if (clipEndX.X == clipStaX.X)
                {
                    clippingX = false;
                    return;
                }

                Graphics g = panelPlot.CreateGraphics();
                g.DrawImage(bmpPlot, 0, 0);

                foreach (BL_ChartAxis axis in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(axis))
                    {
                        double min = axis.GetValue(panelAxisBottom.Width, clipStaX.X);
                        double max = axis.GetValue(panelAxisBottom.Width, clipEndX.X);
                        axis.Minimum = min;
                        axis.Maximum = max;
                    }
                }

                clippingX = false;

                Redraw();
            }
            else if (clippingY)
            {
                clipEndY = e.Location;

                if (clipEndY.Y == clipStaY.Y)
                {
                    clippingY = false;
                    return;
                }

                Graphics g = panelPlot.CreateGraphics();
                g.DrawImage(bmpPlot, 0, 0);

                foreach (BL_ChartAxis axis in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Left).IsInstanceOfType(axis))
                    {
                        double max = axis.GetValue(panelAxisLeft.Height, clipStaY.Y);
                        double min = axis.GetValue(panelAxisLeft.Height, clipEndY.Y);

                        if (min < max)
                        {
                            axis.Minimum = min;
                            axis.Maximum = max;
                        }
                        else
                        {
                            axis.Minimum = max;
                            axis.Maximum = min;
                        }
                    }
                    else if (typeof(BL_ChartAxis_Right).IsInstanceOfType(axis))
                    {
                        double max = axis.GetValue(panelAxisRight.Height, clipStaY.Y);
                        double min = axis.GetValue(panelAxisRight.Height, clipEndY.Y);

                        if (min < max)
                        {
                            axis.Minimum = min;
                            axis.Maximum = max;
                        }
                        else
                        {
                            axis.Minimum = max;
                            axis.Maximum = min;
                        }
                    }
                }

                clippingY = false;

                Redraw();
            }
        }

        #endregion

        #region スクロール処理

        private int xscroll = 0;
        private int yscroll = 0;
        private BelicsClass.Common.BL_Stopwatch sw = new Common.BL_Stopwatch();

        private void panelAxisRight_MouseDown(object sender, MouseEventArgs e)
        {
            sw.Restart();
            xscroll = 1;
            timer1.Interval = 1;
            timer1.Enabled = true;
        }

        private void panelAxisRight_MouseUp(object sender, MouseEventArgs e)
        {
            xscroll = 0;
            timer1.Enabled = false;
        }

        private void panelAxisLeft_MouseDown(object sender, MouseEventArgs e)
        {
            sw.Restart();
            xscroll = -1;
            timer1.Interval = 1;
            timer1.Enabled = true;
        }

        private void panelAxisLeft_MouseUp(object sender, MouseEventArgs e)
        {
            xscroll = 0;
            timer1.Enabled = false;
        }

        private void panelAxisBottom_MouseDown(object sender, MouseEventArgs e)
        {
            sw.Restart();
            yscroll = 1;
            timer1.Interval = 1;
            timer1.Enabled = true;
        }

        private void panelAxisBottom_MouseUp(object sender, MouseEventArgs e)
        {
            yscroll = 0;
            timer1.Enabled = false;
        }

        private void labelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            sw.Restart();
            yscroll = -1;
            timer1.Interval = 1;
            timer1.Enabled = true;
        }

        private void labelTitle_MouseUp(object sender, MouseEventArgs e)
        {
            yscroll = 0;
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (0 < xscroll)
            {
                foreach (var v in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(v))
                    {
                        double w = (v.Maximum - v.Minimum) / 10;

                        if (v.RightToLeft)
                        {
                            v.Maximum -= w;
                            v.Minimum -= w;
                        }
                        else
                        {
                            v.Maximum += w;
                            v.Minimum += w;
                        }
                    }
                }
            }
            else if (xscroll < 0)
            {
                foreach (var v in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(v))
                    {
                        double w = (v.Maximum - v.Minimum) / 10;

                        if (v.RightToLeft)
                        {
                            v.Maximum += w;
                            v.Minimum += w;
                        }
                        else
                        {
                            v.Maximum -= w;
                            v.Minimum -= w;
                        }
                    }
                }
            }

            if (0 < yscroll)
            {
                foreach (var v in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Left).IsInstanceOfType(v) || typeof(BL_ChartAxis_Right).IsInstanceOfType(v))
                    {
                        double w = (v.Maximum - v.Minimum) / 10;

                        if (v.RightToLeft)
                        {
                            v.Maximum -= w;
                            v.Minimum -= w;
                        }
                        else
                        {
                            v.Maximum += w;
                            v.Minimum += w;
                        }
                    }
                }
            }
            else if (yscroll < 0)
            {
                foreach (var v in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Left).IsInstanceOfType(v) || typeof(BL_ChartAxis_Right).IsInstanceOfType(v))
                    {
                        double w = (v.Maximum - v.Minimum) / 10;

                        if (v.RightToLeft)
                        {
                            v.Maximum += w;
                            v.Minimum += w;
                        }
                        else
                        {
                            v.Maximum -= w;
                            v.Minimum -= w;
                        }
                    }
                }
            }

            Redraw();
            timer1.Interval = 200;
        }

        #endregion

        #region カーソル処理

        private void panelPlot_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //カーソル追加
                foreach (var v in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(v))
                    {
                        if (v.CursorPosition.Count < _cursorcount)
                        {
                            v.AddCursorPosition(panelPlot.Width, e.X);
                        }
                    }
                }

                Redraw();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //カーソル削除

                foreach (var v in dictAxises)
                {
                    if (typeof(BL_ChartAxis_Bottom).IsInstanceOfType(v))
                    {
                        v.RemoveLastCursorPosition();
                    }
                }

                Redraw();
            }
        }

        /// <summary>
        /// 指定したシリーズのカーソル位置の値を取得します
        /// </summary>
        /// <param name="series">シリーズ</param>
        /// <param name="cursorno">カーソルNo</param>
        /// <returns></returns>
        public BL_ChartPoint GetCursorPoint(BL_ChartSeries series, int cursorno)
        {
            return series.GetCursorPoints(panelPlot.Width, cursorno);
        }

        #endregion

		/// <summary>
		/// 
		/// </summary>
        public void RedrawAsync()
        {
            MethodInvoker process = (MethodInvoker)delegate()
            {
                timerRedraw.Interval = 100;
                timerRedraw.Enabled = true;
            };
            try
            {
            if (InvokeRequired) this.BeginInvoke(process);
            else process.Invoke();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        private void timerRedraw_Tick(object sender, EventArgs e)
        {
            Redraw();
            timerRedraw.Enabled = false;
        }
    }
}
