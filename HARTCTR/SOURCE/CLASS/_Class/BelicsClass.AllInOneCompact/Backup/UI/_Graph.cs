using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using BelicsClass.Common;
using BelicsClass.ObjectSync;
using BelicsClass.UI.Controls;

namespace BelicsClass.UI.Graph
{
    /// <summary>
    /// グラフを描画するコントロールです
    /// </summary>
    public partial class BL_Graph : UserControl
    {
        #region グラフ描画のポイントデータを管理する基本クラスです

        /// <summary>
        /// グラフ描画のポイントデータを管理する基本クラスです
        /// </summary>
        public class BL_GraphData_Base : BL_ObjectSync
        {
            private BL_GraphData_Base prevvalue = null;
            /// <summary></summary>
            public BL_GraphData_Base PrevValue { get { return prevvalue; } set { prevvalue = value; } }

            private BL_GraphData_Base nextvalue = null;
            /// <summary></summary>
            public BL_GraphData_Base NextValue { get { return nextvalue; } set { nextvalue = value; } }

            private Pen brushpen = null;
            /// <summary>線の描画色を取得または設定します</summary>
            public Pen BrushPen { get { return brushpen; } set { brushpen = value; } }

            /// <summary></summary>
            [BL_ObjectSyncAttribute]
            private long x = 0;
            /// <summary></summary>
            public long X { get { return x; } set { x = value; } }

            /// <summary></summary>
            [BL_ObjectSyncAttribute]
            protected object y = new object();
            /// <summary></summary>
            public double PerX = 1.0;

            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            public BL_GraphData_Base()
            {
            }

            /// <summary>
            /// ポイント間を描画するための基本仮想メソッド
            /// </summary>
            /// <param name="g"></param>
            /// <param name="series"></param>
            /// <param name="xaxis"></param>
            /// <param name="yaxis"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public virtual int Draw(Graphics g, BL_GraphSeries series, BL_GraphAxis xaxis, BL_GraphAxis yaxis, int width, int height) { return 0; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="yaxis"></param>
            /// <returns></returns>
            public virtual float GetPositionY(BL_GraphAxis yaxis)
            {
                return 0;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="xaxis"></param>
            /// <returns></returns>
            public virtual float GetPositionX(BL_GraphAxis xaxis)
            {
                return 0;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="yaxis"></param>
            /// <param name="format"></param>
            /// <returns></returns>
            public virtual string ToStringY(BL_GraphAxis yaxis, string format)
            {
                return y.ToString();
            }
        }

        #endregion

        #region 折れ線データのポイントを保持して、線描画処理を行います(斜線結線)

        /// <summary>
        /// 折れ線データのポイントを保持して、描画処理を行います
        /// </summary>
        [Serializable]
        public class BL_GraphLine : BL_GraphData_Base
        {
            /// <summary></summary>
            public double Y { get { return (double)y; } set { y = value; } }
            
            /// <summary></summary>
            new public BL_GraphLine PrevValue { get { return (BL_GraphLine)base.PrevValue; } set { base.PrevValue = value; } }
            /// <summary></summary>
            new public BL_GraphLine NextValue { get { return (BL_GraphLine)base.NextValue; } set { base.NextValue = value; } }

            /// <summary>
            /// 
            /// </summary>
            public BL_GraphLine() : base() { X = 0; Y = 0; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public BL_GraphLine(long x, double y) { X = x; Y = y; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="brushpen"></param>
            public BL_GraphLine(long x, double y, Pen brushpen) : this(x, y) { BrushPen = brushpen; }

            /// <summary>
            /// ポイント間を接続する線を描画します
            /// </summary>
            /// <param name="g"></param>
            /// <param name="series"></param>
            /// <param name="xaxis"></param>
            /// <param name="yaxis"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public override int Draw(Graphics g, BL_GraphSeries series, BL_GraphAxis xaxis, BL_GraphAxis yaxis, int width, int height)
            {
                int usedpoints = 0;
                if (NextValue == null) return usedpoints;
                if (NextValue == this) return usedpoints;

                if (xaxis.IsContinueLine(this, NextValue))
                {
                    Pen pen = series.BrushPen;
                    if (this.BrushPen != null) pen = this.BrushPen;

                    double pixelvolume = Math.Abs(xaxis.GetPixelVolume());

                    BL_GraphLine max = this;
                    BL_GraphLine min = this;
                    BL_GraphLine end = this;
                    BL_GraphLine sta = this;

                    BL_GraphLine fp = this;
                    while (true)
                    {
                        if (max.Y < fp.Y) max = fp;
                        if (fp.Y < min.Y) min = fp;

                        fp = fp.NextValue;
                        end = fp;

                        if (fp.NextValue == null) break;
                        if (fp.NextValue == fp) break;
                        if (pixelvolume < Math.Abs(fp.NextValue.X - this.X)) break;
                        if (pixelvolume < Math.Abs(fp.NextValue.X - fp.X)) break;

                        usedpoints++;
                    }

                    if (0 < usedpoints && max != null && min != null && sta != null && end != null)
                    {
                        if (min.X <= max.X)
                        {
                            float x1 = (float)xaxis.GetPosition(sta.X);
                            float y1 = (float)yaxis.GetPosition(min.Y);
                            float x2 = (float)xaxis.GetPosition(end.X);
                            float y2 = (float)yaxis.GetPosition(max.Y);
                            g.DrawLine(pen, x1, y1, x2, y2);
                        }
                        else
                        {
                            float x1 = (float)xaxis.GetPosition(sta.X);
                            float y1 = (float)yaxis.GetPosition(max.Y);
                            float x2 = (float)xaxis.GetPosition(end.X);
                            float y2 = (float)yaxis.GetPosition(min.Y);
                            g.DrawLine(pen, x1, y1, x2, y2);
                        }

                        usedpoints--;
                    }
                    else
                    {
                        float x1 = (float)xaxis.GetPosition(this.X);
                        float y1 = (float)yaxis.GetPosition(this.Y);
                        float x2 = (float)xaxis.GetPosition(NextValue.X);
                        float y2 = (float)yaxis.GetPosition(NextValue.Y);
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
                else
                {
                }

                return usedpoints;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="yaxis"></param>
            /// <returns></returns>
            public override float GetPositionY(BL_GraphAxis yaxis)
            {
                return (float)yaxis.GetPosition(Y);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="xaxis"></param>
            /// <returns></returns>
            public override float GetPositionX(BL_GraphAxis xaxis)
            {
                return (float)xaxis.GetPosition(X);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="yaxis"></param>
            /// <param name="format"></param>
            /// <returns></returns>
            public override string ToStringY(BL_GraphAxis yaxis, string format)
            {
                return yaxis.ToString(Y);
            }
        }

        #endregion

        #region 折れ線データのポイントを保持して、線描画処理を行います(２点間補完斜線結線)

        /// <summary>
        /// 折れ線データのポイントを保持して、描画処理を行います
        /// </summary>
        public class BL_GraphLineComplement : BL_GraphLine
        {
            /// <summary>
            /// 
            /// </summary>
            public BL_GraphLineComplement() : base(0, 0) { }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public BL_GraphLineComplement(long x, double y) : base(x, y) { }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="brushpen"></param>
            public BL_GraphLineComplement(long x, double y, Pen brushpen) : base(x, y, brushpen) { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="yaxis"></param>
            /// <returns></returns>
            public override float GetPositionY(BL_GraphAxis yaxis)
            {
                double YY = Y;
                if (NextValue != null && NextValue != this && PerX != 0.0)
                {
                    YY = Y + (NextValue.Y - Y) * PerX;
                }
                else if (PrevValue != null && PrevValue != this && PerX != 0.0)
                {
                    YY = PrevValue.Y + (Y - PrevValue.Y) * PerX;
                }

                return (float)yaxis.GetPosition(YY);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="xaxis"></param>
            /// <returns></returns>
            public override float GetPositionX(BL_GraphAxis xaxis)
            {
                return (float)xaxis.GetPosition(X);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="yaxis"></param>
            /// <param name="format"></param>
            /// <returns></returns>
            public override string ToStringY(BL_GraphAxis yaxis, string format)
            {
                double YY = Y;
                if (NextValue != null && NextValue != this && PerX != 0.0)
                {
                    YY = Y + (NextValue.Y - Y) * PerX;
                }
                else if (PrevValue != null && PrevValue != this && PerX != 0.0)
                {
                    YY = PrevValue.Y + (Y - PrevValue.Y) * PerX;
                }

                return yaxis.ToString(YY);
            }
        }

        #endregion

        #region 折れ線データのポイントを保持して、線描画処理を行います(立上矩形結線)

        /// <summary>
        /// 
        /// </summary>
        public class BL_GraphLineStep : BL_GraphLine
        {
            /// <summary>
            /// 
            /// </summary>
            public BL_GraphLineStep() : base() { }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public BL_GraphLineStep(long x, double y) : base(x, y) { }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="brushpen"></param>
            public BL_GraphLineStep(long x, double y, Pen brushpen) : base(x, y, brushpen) { }

            /// <summary>
            /// ポイント間を接続する線を描画します
            /// </summary>
            /// <param name="g"></param>
            /// <param name="series"></param>
            /// <param name="xaxis"></param>
            /// <param name="yaxis"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public override int Draw(Graphics g, BL_GraphSeries series, BL_GraphAxis xaxis, BL_GraphAxis yaxis, int width, int height)
            {
                int usedpoints = 0;
                if (NextValue == null) return usedpoints;
                if (NextValue == this) return usedpoints;

                if (xaxis.IsContinueLine(this, NextValue))
                {
                    Pen pen = series.BrushPen;
                    if (this.BrushPen != null) pen = this.BrushPen;

                    double pixelvolume = Math.Abs(xaxis.GetPixelVolume());

                    BL_GraphLine max = this;
                    BL_GraphLine min = this;
                    BL_GraphLine end = this;
                    BL_GraphLine sta = this;

                    BL_GraphLine fp = this;
                    while (true)
                    {
                        if (max.X < fp.X) max = fp;
                        if (fp.X < min.X) min = fp;

                        fp = fp.NextValue;
                        end = fp;

                        if (fp.NextValue == null) break;
                        if (fp.NextValue == fp) break;
                        if (pixelvolume < Math.Abs(fp.NextValue.X - this.X)) break;
                        if (pixelvolume < Math.Abs(fp.NextValue.X - fp.X)) break;

                        usedpoints++;
                    }

                    if (0 < usedpoints && max != null && min != null && sta != null && end != null)
                    {
                        if (min.X <= max.X)
                        {
                            float x1 = (float)xaxis.GetPosition(sta.X);
                            float y1 = (float)yaxis.GetPosition(min.Y);
                            float x2 = (float)xaxis.GetPosition(end.X);
                            float y2 = (float)yaxis.GetPosition(max.Y);
                            g.DrawLine(pen, x1, y1, x2, y1);
                            g.DrawLine(pen, x2, y1, x2, y2);
                        }
                        else
                        {
                            float x1 = (float)xaxis.GetPosition(sta.X);
                            float y1 = (float)yaxis.GetPosition(max.Y);
                            float x2 = (float)xaxis.GetPosition(end.X);
                            float y2 = (float)yaxis.GetPosition(min.Y);
                            g.DrawLine(pen, x1, y1, x2, y1);
                            g.DrawLine(pen, x2, y1, x2, y2);
                        }

                        usedpoints--;
                    }
                    else
                    {
                        float x1 = (float)xaxis.GetPosition(this.X);
                        float y1 = (float)yaxis.GetPosition(this.Y);
                        float x2 = (float)xaxis.GetPosition(NextValue.X);
                        float y2 = (float)yaxis.GetPosition(NextValue.Y);
                        g.DrawLine(pen, x1, y1, x2, y1);
                        g.DrawLine(pen, x2, y1, x2, y2);
                    }
                }
                else
                {
                }

                return usedpoints;
            }
        }

        #endregion

        #region 矩形データのポイントを保持して、積算矩形描画処理を行います

        /// <summary>
        /// 矩形データのポイントを保持して、矩形描画処理を行います
        /// </summary>
        public class BL_GraphBlock : BL_GraphData_Base
        {
            /// <summary></summary>
            public class BlockObject
            {
                /// <summary></summary>
                public long blockvolumeX = 0;
                /// <summary></summary>
                public double height = 0;
                /// <summary></summary>
                public Brush brush = new SolidBrush(Color.Transparent);

                /// <summary></summary>
                public BlockObject() { }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="blockvolume"></param>
                /// <param name="height"></param>
                /// <param name="brush"></param>
                public BlockObject(long blockvolume, double height, Brush brush)
                {
                    this.blockvolumeX = blockvolume;
                    this.height = height;
                    this.brush = brush;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="source"></param>
                public BlockObject(BlockObject source)
                {
                    blockvolumeX = source.blockvolumeX;
                    height = source.height;
                    brush = source.brush;
                }
            }

            /// <summary></summary>
            public double Y { get { return Block.height; } set { Block.height = value; } }

            /// <summary></summary>
            public BlockObject Block { get { return (BlockObject)y; } set { y = value; } }

            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            public BL_GraphBlock() : base() { y = new BlockObject(); }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="x"></param>
            /// <param name="block"></param>
            public BL_GraphBlock(long x, BlockObject block) { X = x; y = block; }

            /// <summary></summary>
            new public BL_GraphBlock PrevValue { get { return (BL_GraphBlock)base.PrevValue; } set { base.PrevValue = value; } }
            /// <summary></summary>
            new public BL_GraphBlock NextValue { get { return (BL_GraphBlock)base.NextValue; } set { base.NextValue = value; } }

            /// <summary>
            /// データ量によって、矩形を描画します
            /// </summary>
            /// <param name="g"></param>
            /// <param name="series"></param>
            /// <param name="xaxis"></param>
            /// <param name="yaxis"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public override int Draw(Graphics g, BL_GraphSeries series, BL_GraphAxis xaxis, BL_GraphAxis yaxis, int width, int height)
            {
                int usedpoints = 0;
                //int sumpoints = 0;
                //if (NextValue == null) return usedpoints;
                //if (NextValue == this) return usedpoints;

                Pen pen = series.BrushPen;
                if (this.BrushPen != null) pen = this.BrushPen;

                double sum = 0;
                //long totalX = 0;
                BL_GraphBlock fp = this;

                //while (fp.Block.blockvolumeX == 0)
                //{
                //    if (fp.PrevValue == null) break;
                //    if (fp.PrevValue == this) break;
                //    fp = fp.PrevValue;
                //    usedpoints--;
                //}

                long blockvolume = xaxis.RealX(fp.Block.blockvolumeX);
                //if (blockvolume == 0) return 0;

                BL_GraphBlock sta = fp;
                BL_GraphBlock end = fp;

                //while (true)
                //{
                //    sumpoints++;
                //    usedpoints++;
                //    sum += fp.Y;
                //    totalX += (fp.NextValue.X - fp.X);
                //    fp = fp.NextValue;
                //    end = fp;

                //    if (fp.NextValue == fp) break;
                //    if (0 < fp.Block.blockvolumeX) break;
                //}

                //if (0 < usedpoints)
                {
                    //sum /= sumpoints;
                    //if (totalX < blockvolume) blockvolume = totalX;

                    sum = sta.Y;
                    float x1 = (float)xaxis.GetPosition(sta.X);
                    float y1 = (float)yaxis.GetPosition((double)-1);

                    //if (blockvolume == 0) blockvolume = totalX;
                    float x2 = (float)xaxis.GetPosition(sta.X + blockvolume);
                    float y2 = (float)yaxis.GetPosition(sum);

                    PointF[] points = new PointF[] { new PointF(x1, y1), new PointF(x1, y2), new PointF(x2, y2), new PointF(x2, y1) };
                    g.FillPolygon(sta.Block.brush, points);
                    g.DrawPolygon(pen, points);

                    //usedpoints--;
                }

                return usedpoints;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="yaxis"></param>
            /// <param name="format"></param>
            /// <returns></returns>
            public override string ToStringY(BL_GraphAxis yaxis, string format)
            {
                return yaxis.ToString(Y);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="xaxis"></param>
            /// <returns></returns>
            public override float GetPositionX(BL_GraphAxis xaxis)
            {
                return base.GetPositionX(xaxis);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="yaxis"></param>
            /// <returns></returns>
            public override float GetPositionY(BL_GraphAxis yaxis)
            {
                return (float)yaxis.GetPosition(Y);
            }
        }
        
        #endregion

        #region 矩形データのポイントを保持して、積算矩形描画処理を行います

        /// <summary>
        /// 矩形データのポイントを保持して、矩形描画処理を行います
        /// </summary>
        public class BL_GraphBlock_MultiUp : BL_GraphBlock
        {
            /// <summary></summary>
            public class BlockObject_MultiUp : BlockObject
            {
                /// <summary></summary>
                public const int MultiUpMax = 32;

                /// <summary></summary>
                new public double[] height = new double[MultiUpMax];

                /// <summary></summary>
                public virtual double this[int index] { get { return height[index]; } set { height[index] = value; } }
            }

            /// <summary></summary>
            new public BlockObject_MultiUp Block { get { return (BlockObject_MultiUp)y; } set { y = value; } }

            /// <summary></summary>
            new public double[] Y { get { return Block.height; } set { Block.height = value; } }

            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            public BL_GraphBlock_MultiUp() { y = new BlockObject_MultiUp(); }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="x"></param>
            /// <param name="block"></param>
            public BL_GraphBlock_MultiUp(long x, BlockObject_MultiUp block) : base(x, block) { }

            /// <summary>
            /// データ量によって、矩形を描画します
            /// </summary>
            /// <param name="g"></param>
            /// <param name="series"></param>
            /// <param name="xaxis"></param>
            /// <param name="yaxis"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public override int Draw(Graphics g, BL_GraphSeries series, BL_GraphAxis xaxis, BL_GraphAxis yaxis, int width, int height)
            {
                int usedpoints = 0;
                int sumpoints = 0;
                if (NextValue == null) return usedpoints;
                if (NextValue == this) return usedpoints;

                BL_GraphBlock_MultiUp fp = this;

                while (fp.Block.blockvolumeX == 0)
                {
                    if (fp.PrevValue == null) break;
                    if (fp.PrevValue == this) break;
                    fp = (BL_GraphBlock_MultiUp)fp.PrevValue;
                    usedpoints--;
                }

                double[] sum = new double[BlockObject_MultiUp.MultiUpMax];
                long totalX = 0;
                long blockvolume = xaxis.RealX(fp.Block.blockvolumeX);
                if (blockvolume == 0) return 0;

                BL_GraphBlock_MultiUp sta = fp;
                BL_GraphBlock_MultiUp end = fp;

                while (true)
                {
                    sumpoints++;
                    usedpoints++;

                    for (int i = 0; i < fp.Y.Length; i++) sum[i] += fp.Y[i];
                    
                    totalX += (fp.NextValue.X - fp.X);
                    fp = (BL_GraphBlock_MultiUp)fp.NextValue;
                    end = fp;

                    if (fp.NextValue == fp) break;
                    if (0 < fp.Block.blockvolumeX) break;
                }

                if (0 < usedpoints)
                {
                    Pen pen = series.BrushPen;
                    if (this.BrushPen != null) pen = this.BrushPen;

                    for (int i = 0; i < fp.Y.Length; i++) sum[i] /= sumpoints;
                    if (totalX < blockvolume) blockvolume = totalX;

                    double sumpre = -1;
                    for (int i = 0; i < fp.Y.Length; i++)
                    {
                        float x1 = (float)xaxis.GetPosition(sta.X);
                        float y1 = (float)yaxis.GetPosition(sumpre);

                        if (blockvolume == 0) blockvolume = totalX;
                        float x2 = (float)xaxis.GetPosition(sta.X + blockvolume);
                        float y2 = (float)yaxis.GetPosition(sumpre + sum[i]);
                        sumpre = sum[i];

                        PointF[] points = new PointF[] { new PointF(x1, y1), new PointF(x1, y2), new PointF(x2, y2), new PointF(x2, y1) };
                        g.FillPolygon(sta.Block.brush, points);
                        g.DrawPolygon(pen, points);
                    }

                    usedpoints--;
                }

                return usedpoints;
            }
        }

        #endregion


        #region 数値軸の設定を管理するクラスです

        /// <summary>
        /// 軸の設定を管理するクラスです
        /// </summary>
        public class BL_GraphAxis
        {
            #region コンストラクタ

            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            public BL_GraphAxis() { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name">軸名称</param>
            /// <param name="range">軸レンジ幅</param>
            /// <param name="offset">軸シフト量</param>
            /// <param name="guidewidth_inner">ガイド線描画幅(内側)</param>
            /// <param name="guidewidth_outer">ガイド線描画幅(外側)</param>
            /// <param name="separate_volume">データが存在しない場合に線を切り離す基準量</param>
            /// <param name="position">軸ガイド表示位置</param>
            /// <param name="width">軸ガイド表示幅(高さ)</param>
            /// <param name="format">軸ガイド値表示フォーマット</param>
            /// <param name="righttoleft">右→左</param>
            /// <param name="zoom"></param>
            public BL_GraphAxis(string name, long range, long offset, int guidewidth_inner, int guidewidth_outer, long separate_volume, axisPosition position, int width, string format, bool righttoleft, float zoom)
            {
                this.name = name;
                this.range = range;
                this.offset = offset;
                this.holdoffset = offset;
                this.guidelinewidthinner = guidewidth_inner;
                this.guidelinewidthouter = guidewidth_outer;
                this.separatevolume = separate_volume;
                this.axisposition = position;
                this.axiswidth = width;
                this.axisformat = format;
                this.righttoleft = righttoleft;
                this.zoom = zoom;
            }

            #endregion

            #region 列挙体

            /// <summary>軸目盛の表示位置を表す列挙体</summary>
            public enum axisPosition
            {
                /// <summary>軸目盛表示なし</summary>
                None,
                /// <summary>左</summary>
                Left,
                /// <summary>右</summary>
                Right,
                /// <summary>下</summary>
                Bottom,
            }

            #endregion

            #region フィールドとプロパティ

            private string name = "";
            private long range = 100;
            private int guidelinewidthinner = 0;
            private int guidelinewidthouter = 0;
            private long separatevolume = 0;
            private axisPosition axisposition = axisPosition.None;
            private int axiswidth = 80;
            private string axisformat = "";
            private bool righttoleft = false;
            private float zoom = 1.0f;
            private long offset = 0;
            private bool smoothmove = true;
            private bool ishold = false;
            private long holdoffset = 0;
            private int cursorpixel = 0;
            private long localscale = 1;
            private int controlwidth = 0;
            private ScrollBar scrollbar = null;
            private BL_Graph parent = null;
            private long offsetrange = 0;
            private int scrollpos = 0;

            /// <summary>軸名称を取得または設定します</summary>
            public string Name { get { return name; } set { name = value; } }
            /// <summary>軸スケール(1pixelあたりのデータ量)を取得または設定します。100を設定すると1pixelあたり100として描画されます</summary>
            public long Range { get { return range; } set { range = value; } }
            /// <summary>チャート内のガイド目盛線を描画するデータ量を取得または設定します(0で描画しません)</summary>
            public int GuidelineWidthInner { get { return guidelinewidthinner; } set { guidelinewidthinner = value; } }
            /// <summary>チャート外の軸目盛線を描画するデータ量を取得または設定します(0で描画しません)</summary>
            public int GuidelineWidthOuter { get { return guidelinewidthouter; } set { guidelinewidthouter = value; } }
            /// <summary>チャート内のガイド目盛線を描画するデータ量を取得または設定します(0で描画しません)</summary>
            public long GuidelineWidthInnerVolume { get { return guidelinewidthinner * localscale; } }
            /// <summary>チャート外の軸目盛線を描画するデータ量を取得または設定します(0で描画しません)</summary>
            public long GuidelineWidthOuterVolume { get { return guidelinewidthouter * localscale; } }
            /// <summary>データ無しとして線を切り離す基準となるデータ量を取得または設定します。</summary>
            public long SeparateVolume { get { return separatevolume; } set { separatevolume = value; } }
            /// <summary>軸目盛の表示位置</summary>
            public axisPosition AxisPosition { get { return axisposition; } set { axisposition = value; } }
            /// <summary>軸目盛の表示幅（高さ）</summary>
            public int AxisWidth { get { return axiswidth; } set { axiswidth = value; } }
            /// <summary>軸目盛の表示フォーマット</summary>
            public string AxisFormat { get { return axisformat; } set { axisformat = value; } }
            /// <summary>右(上)から左(下)の方向を正とする</summary>
            public bool RightToLeft { get { return righttoleft; } set { righttoleft = value; } }
            /// <summary>軸値の倍率</summary>
            public float Zoom { get { return zoom; } set { zoom = value; } }
            /// <summary>軸値の倍率</summary>
            public float Scale { get { return (1 * zoom); } }
            /// <summary>最新の表示位置を表す値</summary>
            public long Offset
            {
                get
                {
                    if (ishold) return holdoffset;
                    return offset;
                }
                set
                {
                    offset = value;
                    if (!ishold) holdoffset = value;
                }
            }
            /// <summary>メモリ区切り時間を追いかけて表示を移動する</summary>
            public bool SmoothMove { get { return smoothmove; } set { smoothmove = value; } }
            /// <summary>表示が固定されているか否かを表すフラグ</summary>
            public bool IsHold { get { return ishold; } set { ishold = value; } }
            /// <summary>固定されている表示位置を表す値</summary>
            public long HoldOffset { get { return holdoffset; } set { holdoffset = value; } }
            /// <summary>カーソル位置を取得または設定します</summary>
            public int CursorPixel { get { return cursorpixel; } set { cursorpixel = value; } }
            /// <summary>固定されている表示位置を表す値</summary>
            public long LocalScale { get { return localscale; } set { localscale = value; } }
            /// <summary>軸目盛エリアの表示幅</summary>
            public int ControlWidth { get { return controlwidth; } set { controlwidth = value; } }
            /// <summary>スクロールバーコントロールを取得または設定します</summary>
            public ScrollBar Scrollbar { get { return scrollbar; } set { scrollbar = value; } }
            /// <summary>グラフコントロールを取得または設定します</summary>
            public BL_Graph ControlOwner { get { return parent; } set { parent = value; } }
            /// <summary>直前のスクロールバー位置を取得します</summary>
            public int ScrollPosition { get { return scrollpos; } }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <returns></returns>
            public long RealX(long x) { return x * localscale; }

            #endregion

            #region 描画メソッド

            /// <summary>
            /// 軸の目盛エリアを描画します
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="offset"></param>
            /// <param name="axisoffset"></param>
            /// <param name="series"></param>
            /// <param name="drawcursoronly"></param>
            /// <param name="cursorenabled"></param>
            public virtual void Draw(Graphics g, int width, int offset, int axisoffset, BL_GraphSeries series, bool drawcursoronly, bool cursorenabled)
            {
                ControlWidth = width;
                Scroll_Update();

                if (0 < Math.Abs(GuidelineWidthOuter))
                {
                    double dotscale = Range / (double)width;
                    
                    Font font;
                    if (ControlOwner != null) font = ControlOwner.Font;
                    else font = new Font("Meiryo UI", 8.0f, FontStyle.Regular);
                    StringFormat format = new StringFormat();

                    switch (axisposition)
                    {
                        case axisPosition.Bottom:

                            #region Bottom
                            {
                                format.Alignment = StringAlignment.Center;
                                float xx = 0;
                                g.ResetTransform();

                                xx = offset;
                                if (righttoleft) xx = offset + width;
                                g.TranslateTransform((float)xx, axisoffset, MatrixOrder.Append);

                                if (!drawcursoronly)
                                {
                                    g.DrawLine(new Pen(Color.Black), 0, 5, Position(width), 5);

                                    float xpre = 0;
                                    float xfin = 0;
                                    float guidepixels = Math.Abs(GetPixels(GuidelineWidthOuter));

                                    float startx = 0;
                                    if (SmoothMove) startx = Math.Abs(GetPixels(GetOffset() % GuidelineWidthOuter));

                                    if (0 < startx)
                                    {
                                        g.DrawLine(new Pen(Color.Black), 0, 5, 0, 10);

                                        string s = ToString(0.0f, width);
                                        SizeF size = g.MeasureString(s, font);
                                        xx = Position(0.0f) - size.Width / 2;

                                        if (RightToLeft)
                                        {
                                            if (0 < xx + size.Width) xx = 0 - size.Width;
                                            if (xx < Position(width) - 0) xx = Position(width) - 0;
                                        }
                                        else
                                        {
                                            if (xx < 0) xx = 0;
                                            if (Position(width) < xx + size.Width) xx = Position(width) - size.Width;
                                        }

                                        g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(xx, 9.0f, size.Width, size.Height), format);
                                        if (!righttoleft) xpre = xx + size.Width;
                                        if (righttoleft) xpre = xx;
                                    }

                                    {
                                        g.DrawLine(new Pen(Color.Black), Position(width), 5, Position(width), 10);

                                        string s = ToString((float)width, width);
                                        SizeF size = g.MeasureString(s, font);
                                        xx = Position((float)width) - size.Width / 2;

                                        if (RightToLeft)
                                        {
                                            if (16 < xx + size.Width) xx = 16 - size.Width;
                                            if (xx < Position(width) - 16) xx = Position(width) - 16;
                                        }
                                        else
                                        {
                                            if (xx < 0) xx = 0;
                                            if (Position(width) < xx + size.Width) xx = Position(width) - size.Width;
                                        }

                                        g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(xx, 9.0f, size.Width, size.Height), format);
                                        if (!righttoleft) xfin = xx;
                                        if (righttoleft) xfin = xx + size.Width;
                                    }

                                    for (float x = 0; x <= width; x += guidepixels)
                                    {
                                        g.DrawLine(new Pen(Color.Black), Position(x), 5, Position(x), 10);

                                        string s = ToString(x, width);
                                        SizeF size = g.MeasureString(s, font);
                                        xx = Position(x) - size.Width / 2;

                                        if (RightToLeft)
                                        {
                                            if (0 < xx + size.Width) xx = 0 - size.Width;
                                            if (xx < Position(width) - 0) xx = Position(width) - 0;
                                        }
                                        else
                                        {
                                            if (xx < 0) xx = 0;
                                            if (Position(width) < xx + size.Width) xx = Position(width) - size.Width;
                                        }

                                        if (x == 0.0 || (!righttoleft && xpre < xx) || (righttoleft && xx + size.Width < xpre))
                                        {
                                            if ((!righttoleft && xx + size.Width < xfin) || (righttoleft && xfin < xx))
                                            {
                                                g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(xx, 9.0f, size.Width, size.Height), format);
                                                if (!righttoleft) xpre = xx + size.Width;
                                                if (righttoleft) xpre = xx;
                                            }
                                        }
                                    }

                                    for (float x = 0; startx <= x; x -= guidepixels)
                                    {
                                        g.DrawLine(new Pen(Color.Black), Position(x), 5, Position(x), 10);

                                        string s = ToString(x, width);
                                        SizeF size = g.MeasureString(s, font);
                                        xx = Position(x) - size.Width / 2;

                                        if (RightToLeft)
                                        {
                                            if (0 < xx + size.Width) xx = 0 - size.Width;
                                            if (xx < Position(width) - 0) xx = Position(width) - 0;
                                        }
                                        else
                                        {
                                            if (xx < 0) xx = 0;
                                            if (Position(width) < xx + size.Width) xx = Position(width) - size.Width;
                                        }

                                        if (x == 0.0 || (!righttoleft && xpre < xx) || (righttoleft && xx + size.Width < xpre))
                                        {
                                            if ((!righttoleft && xx + size.Width < xfin) || (righttoleft && xfin < xx))
                                            {
                                                g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(xx, 9.0f, size.Width, size.Height), format);
                                                if (!righttoleft) xpre = xx + size.Width;
                                                if (righttoleft) xpre = xx;
                                            }
                                        }
                                    }

                                    if (cursorenabled)
                                    {
                                        if (RightToLeft) cursorpixel = width - cursorpixel;
                                        g.DrawLine(new Pen(Color.Black, 2), Position(cursorpixel), 5, Position(cursorpixel), 10);

                                        string s = ToString((float)cursorpixel, width);
                                        SizeF size = g.MeasureString(s, font);
                                        xx = Position(cursorpixel) - size.Width / 2;

                                        if (RightToLeft)
                                        {
                                            if (0 < xx + size.Width) xx = 0 - size.Width;
                                            if (xx < Position(width) - 16) xx = Position(width) - 16;
                                        }
                                        else
                                        {
                                            if (xx < 0) xx = 0;
                                            if (Position(width) < xx + size.Width) xx = Position(width) - size.Width;
                                        }

                                        BL_GraphData_Base[] list = series.GetCursorYValue();
                                        Pen pen = series.BrushPen;
                                        if (0 < list.Length)
                                        {
                                            if (list[0].BrushPen != null) pen = list[0].BrushPen;
                                        }


                                        RectangleF rect = new RectangleF(xx, 9.0f, size.Width, size.Height);
                                        g.FillRectangle(new SolidBrush(Color.White), rect.X, rect.Y, rect.Width, rect.Height);
                                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                                        g.DrawString(s, font, new SolidBrush(Color.Black), rect, format);

                                        if (typeof(BL_GraphSeries).IsInstanceOfType(Scrollbar.Tag))
                                        {
                                            if (((BL_GraphSeries)Scrollbar.Tag).AxisX == this)
                                            {
                                                rect.Inflate(1, 1);
                                                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                                                rect.Inflate(1, 1);
                                                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            break;

                        case axisPosition.Left:

                            #region Left
                            {
                                float yy = 0;
                                g.ResetTransform();

                                yy = offset;
                                if (righttoleft) yy = offset + width;
                                g.TranslateTransform(axisoffset, (float)yy, MatrixOrder.Append);

                                if (!drawcursoronly)
                                {
                                    SizeF size = g.MeasureString(Name, font);
                                    format.Alignment = StringAlignment.Center;
                                    g.DrawString(Name, font, new SolidBrush(Color.Black), new RectangleF(axiswidth - size.Width, (float)-yy, size.Width, size.Height), format);

                                    format.Alignment = StringAlignment.Far;

                                    g.DrawLine(new Pen(Color.Black), axiswidth - 5, 0, axiswidth - 5, Position(width));

                                    float ypre = 0;
                                    float yfin = 0;
                                    float guidepixels = Math.Abs(GetPixels(GuidelineWidthOuter));

                                    float starty = 0;
                                    if (SmoothMove) starty = Math.Abs(GetPixels(GetOffset() % GuidelineWidthOuter));

                                    if (0 < starty)
                                    {
                                        g.DrawLine(new Pen(Color.Black), axiswidth - 5, 0, axiswidth - 10, 0);

                                        string s = ToString(0.0f, width);
                                        size = g.MeasureString(s, font);
                                        yy = 0 - size.Height / 2;
                                        if (yy < -offset) yy = -offset;
                                        g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(axiswidth - 12.0f - size.Width, (float)yy, size.Width, size.Height), format);
                                        if (!righttoleft) ypre = yy + size.Height;
                                        if (righttoleft) ypre = yy;
                                    }

                                    {
                                        g.DrawLine(new Pen(Color.Black), axiswidth - 5, Position(width), axiswidth - 10, Position(width));

                                        string s = ToString((float)width, width);
                                        size = g.MeasureString(s, font);
                                        yy = Position(width) - size.Height / 2;
                                        g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(axiswidth - 12.0f - size.Width, (float)yy, size.Width, size.Height), format);
                                        if (!righttoleft) yfin = yy;
                                        if (righttoleft) yfin = yy + size.Height;
                                    }

                                    for (float y = starty; y <= width; y += guidepixels)
                                    {
                                        g.DrawLine(new Pen(Color.Black), axiswidth - 5, Position(y), axiswidth - 10, Position(y));

                                        string s = ToString(y, width);
                                        size = g.MeasureString(s, font);
                                        yy = Position(y) - size.Height / 2;

                                        if (y == 0.0 || (!righttoleft && ypre < yy) || (righttoleft && yy + size.Height < ypre))
                                        {
                                            if ((!righttoleft && yy + size.Height < yfin) || (righttoleft && yfin < yy))
                                            {
                                                g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(axiswidth - 12.0f - size.Width, yy, size.Width, size.Height), format);
                                                if (!righttoleft) ypre = yy + size.Height;
                                                if (righttoleft) ypre = yy;
                                            }
                                        }
                                    }
                                }

                                if (cursorenabled)
                                {
                                    if (0 < series.AxisX.ControlWidth)
                                    {
                                        if (series.AxisX.RightToLeft) cursorpixel = series.AxisX.ControlWidth - cursorpixel;
                                        long val = series.AxisX.GetValue((float)cursorpixel);
                                        BL_GraphData_Base[] list = series.GetYValue(val);
                                        for (int i = 0; i < list.Length; i++)
                                        {
                                            BL_GraphData_Base data = list[i];
                                            Pen pen = series.BrushPen;
                                            if (data.BrushPen != null) pen = data.BrushPen;

                                            float y = data.GetPositionY(this);
                                            g.DrawLine(new Pen(pen.Color, 2), axiswidth - 5, y, axiswidth - 10, y);

                                            string s = data.ToStringY(this, AxisFormat);
                                            SizeF size = g.MeasureString(s, font);
                                            yy = y - size.Height / 2;

                                            RectangleF rect = new RectangleF(axiswidth - 12.0f - size.Width, yy, size.Width, size.Height);
                                            g.FillRectangle(new SolidBrush(Color.White), rect.X, rect.Y, rect.Width, rect.Height);
                                            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                                            g.DrawString(s, font, new SolidBrush(Color.Black), rect, format);
                                        }
                                    }

                                }
                            }
                            #endregion

                            break;

                        case axisPosition.Right:

                            #region Right
                            {
                                float yy = 0;
                                g.ResetTransform();

                                yy = offset;
                                if (righttoleft) yy = offset + width;
                                g.TranslateTransform(axisoffset, (float)yy, MatrixOrder.Append);

                                if (!drawcursoronly)
                                {
                                    SizeF size = g.MeasureString(Name, font);
                                    format.Alignment = StringAlignment.Center;
                                    g.DrawString(Name, font, new SolidBrush(Color.Black), new RectangleF(0, (float)-yy, size.Width, size.Height), format);

                                    format.Alignment = StringAlignment.Far;

                                    g.DrawLine(new Pen(Color.Black), 5, 0, 5, Position(width));

                                    float ypre = 0;
                                    float yfin = 0;
                                    float guidepixels = Math.Abs(GetPixels(GuidelineWidthOuter));

                                    float starty = 0;
                                    if (SmoothMove) starty = Math.Abs(GetPixels(GetOffset() % GuidelineWidthOuter));

                                    if (0 < starty)
                                    {
                                        g.DrawLine(new Pen(Color.Black), 5, 0, 10, 0);

                                        string s = ToString(0.0f, width);
                                        size = g.MeasureString(s, font);
                                        yy = 0 - size.Height / 2;
                                        if (yy < -offset) yy = -offset;
                                        g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(12.0f, (float)yy, size.Width, size.Height), format);
                                        if (!righttoleft) ypre = yy + size.Height;
                                        if (righttoleft) ypre = yy;
                                    }

                                    {
                                        g.DrawLine(new Pen(Color.Black), 5, Position(width), 10, Position(width));

                                        string s = ToString((float)width, width);
                                        size = g.MeasureString(s, font);
                                        yy = Position(width) - size.Height / 2;
                                        g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(12.0f, (float)yy, size.Width, size.Height), format);
                                        if (!righttoleft) yfin = yy;
                                        if (righttoleft) yfin = yy + size.Height;
                                    }

                                    for (float y = starty; y <= width; y += guidepixels)
                                    {
                                        g.DrawLine(new Pen(Color.Black), 5, Position(y), 10, Position(y));

                                        string s = ToString(y, width);
                                        size = g.MeasureString(s, font);
                                        yy = Position(y) - size.Height / 2;

                                        if (y == 0.0 || (!righttoleft && ypre < yy) || (righttoleft && yy + size.Height < ypre))
                                        {
                                            if ((!righttoleft && yy + size.Height < yfin) || (righttoleft && yfin < yy))
                                            {
                                                g.DrawString(s, font, new SolidBrush(Color.Black), new RectangleF(12.0f, yy, size.Width, size.Height), format);
                                                if (!righttoleft) ypre = yy + size.Height;
                                                if (righttoleft) ypre = yy;
                                            }
                                        }
                                    }
                                }

                                if (cursorenabled)
                                {
                                    if (series.AxisX.RightToLeft) cursorpixel = series.AxisX.ControlWidth - cursorpixel;
                                    long val = series.AxisX.GetValue((float)cursorpixel);
                                    BL_GraphData_Base[] list = series.GetYValue(val);
                                    for (int i = 0; i < list.Length; i++)
                                    {
                                        BL_GraphData_Base data = list[i];
                                        Pen pen = series.BrushPen;
                                        if (data.BrushPen != null) pen = data.BrushPen;

                                        float y = data.GetPositionY(this);
                                        g.DrawLine(new Pen(pen.Color, 2), 5, y, 10, y);

                                        string s = data.ToStringY(this, AxisFormat);
                                        SizeF size = g.MeasureString(s, font);
                                        yy = y - size.Height / 2;

                                        RectangleF rect = new RectangleF(12.0f, yy, size.Width, size.Height);
                                        g.FillRectangle(new SolidBrush(Color.White), rect.X, rect.Y, rect.Width, rect.Height);
                                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                                        g.DrawString(s, font, new SolidBrush(Color.Black), rect, format);
                                    }

                                }
                            }
                            #endregion

                            break;
                    }
                }
            }

            #endregion

            #region 座標取得系メソッド

            /// <summary>
            /// 軸のオフセット位置を取得します
            /// </summary>
            /// <returns></returns>
            public virtual long GetOffset()
            {
                return (Offset + offsetrange) / localscale;
            }

            /// <summary>
            /// 1ピクセルあたりのデータ量を取得します
            /// </summary>
            /// <returns></returns>
            public virtual double GetPixelVolume()
            {
                return (double)Range * (double)localscale / (double)ControlWidth;
            }

            /// <summary>
            /// データ量に相当するピクセル数を取得します
            /// </summary>
            /// <param name="volume"></param>
            /// <returns></returns>
            public virtual float GetPixels(double volume)
            {
                return (float)(volume * (double)localscale / (GetPixelVolume()));
                //return (float)Math.Abs((volume * (double)localscale / (GetPixelVolume())));
            }

            /// <summary>
            /// データ量に相当する表示位置を取得します
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public virtual double GetPosition(long val)
            {
                double pixels = GetPixels((double)val / (double)localscale - (double)GetOffset());
                if (RightToLeft) return -pixels;
                return pixels;
            }

            /// <summary>
            /// データ量に相当する表示位置を取得します
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public virtual double GetPosition(double val)
            {
                double pixels = GetPixels((val / (double)localscale - (double)GetOffset()));
                if (RightToLeft) return -pixels;
                return pixels;
            }

            /// <summary>
            /// ピクセル位置を正規化します
            /// </summary>
            /// <param name="position"></param>
            /// <returns></returns>
            public virtual int Position(int position)
            {
                if (RightToLeft) return -position;
                return position;
            }

            /// <summary>
            /// ピクセル位置を正規化します
            /// </summary>
            /// <param name="position"></param>
            /// <returns></returns>
            public virtual float Position(float position)
            {
                if (RightToLeft) return -position;
                return position;
            }

            /// <summary>
            /// ピクセル位置を正規化します
            /// </summary>
            /// <param name="position"></param>
            /// <returns></returns>
            public virtual float Position(double position)
            {
                if (RightToLeft) return (float)-position;
                return (float)position;
            }

            #endregion

            #region データ値取得系メソッド

            /// <summary>
            /// ピクセル数に相当するデータ量を取得します
            /// </summary>
            /// <param name="pixel"></param>
            /// <returns></returns>
            public virtual long GetVolume(float pixel)
            {
                double volume = GetPixelVolume();
                return (long)((double)pixel * volume + 0.5);
            }
            
            /// <summary>
            /// ピクセル位置に相当するデータ量を取得します
            /// </summary>
            /// <param name="pixel"></param>
            /// <returns></returns>
            public virtual long GetValue(float pixel)
            {
                return GetOffset() * localscale + GetVolume(pixel);
            }

            /// <summary>
            /// データ量を示す文字列を取得します
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public virtual string ToString(long val)
            {
                if (zoom == 1.0) return val.ToString(axisformat);

                double ret = val / zoom;
                if (axisformat.IndexOf(".") < 0) ret = (double)((int)ret);
                return ret.ToString(axisformat);
            }

            /// <summary>
            /// データ量を示す文字列を取得します
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public virtual string ToString(double val)
            {
                if (zoom == 1.0) return val.ToString(axisformat);

                double ret = val / zoom;
                return ret.ToString(axisformat);
            }

            /// <summary>
            /// ピクセル位置に相当するデータ量を示す文字列を取得します
            /// </summary>
            /// <param name="pixel"></param>
            /// <param name="width"></param>
            /// <returns></returns>
            public virtual string ToString(float pixel, int width)
            {
                long val = GetValue(pixel);
                return ToString(val);
            }

            /// <summary>
            /// 線を結線するか否かを取得します
            /// </summary>
            /// <param name="now"></param>
            /// <param name="prev"></param>
            /// <returns></returns>
            public virtual bool IsContinueLine(BL_GraphData_Base now, BL_GraphData_Base prev)
            {
                if (0 == SeparateVolume ||
                   (Math.Abs(now.X - prev.X) / Zoom <= SeparateVolume * localscale))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// データ量が表示領域内であるか否かを取得します
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public virtual int IsVisible(long val)
            {
                long min = GetValue(0);
                long max = GetValue((float)ControlWidth);

                if (min < max)
                {
                    if (val < min) return -1;
                    if (max < val) return 1;
                }
                else
                {
                    if (val < max) return 1;
                    if (min < val) return -1;
                }

                return 0;
            }

            #endregion

            #region 対象軸スクロール系メソッド

            /// <summary>
            /// スクロール操作による描画位置を調整します
            /// </summary>
            /// <param name="NewValue"></param>
            public void Scroll_Changed(int NewValue)
            {
                //if (!IsHold) return;
                if (ControlWidth == 0) return;
                if (scrollbar == null) return;
                if (!typeof(BL_GraphSeries).IsInstanceOfType(scrollbar.Tag)) return;
                BL_GraphSeries series = (BL_GraphSeries)scrollbar.Tag;

                scrollpos = NewValue;

                switch (AxisPosition)
                {
                    case axisPosition.Bottom:
                        {
                            if (series.AxisX != this) return;

                            if (series.MinimumX != null && series.MaximumX != null)
                            {
                                offsetrange = GuidelineWidthInner * LocalScale * NewValue;
                                if (Range < 0) offsetrange = -offsetrange;
                            }
                        }
                        break;

                    case axisPosition.Left:
                    case axisPosition.Right:
                        {
                            if (series.AxisY != this) return;
                        }
                        break;
                }
            }

            /// <summary>
            /// スクロールバーを更新します
            /// </summary>
            public void Scroll_Update()
            {
                if (IsHold) return;
                if (ControlWidth == 0) return;
                if (scrollbar == null) return;
                if (!typeof(BL_GraphSeries).IsInstanceOfType(scrollbar.Tag)) return;
                BL_GraphSeries series = (BL_GraphSeries)scrollbar.Tag;

                switch (AxisPosition)
                {
                    case axisPosition.Bottom:
                        {
                            if (series.AxisX != this) return;
                            if (series.MinimumX != null && series.MaximumX != null)
                            {
                                long guiderange = GuidelineWidthInner * LocalScale;
                                long totalrange = series.MaximumX.X - series.MinimumX.X;
                                if (series.MaximumX.X < Offset) totalrange = Offset - series.MinimumX.X;
                                int maximum = (int)(totalrange / guiderange);

                                scrollbar.Minimum = 0;
                                scrollbar.Maximum = maximum;
                            }
                            else
                            {
                                scrollbar.Minimum = 0;
                                scrollbar.Maximum = 0;
                            }

                            int innerguidecount = (int)(ControlWidth / Math.Abs(GetPixels(series.AxisX.GuidelineWidthInner)) + 0.999999);
                            scrollbar.SmallChange = 1;
                            scrollbar.LargeChange = innerguidecount;
                        }
                        break;

                    case axisPosition.Left:
                    case axisPosition.Right:
                        {
                            if (series.AxisY != this) return;
                        }
                        break;
                }
            }

            #endregion
        }

        #endregion

        #region 時間軸の設定を管理するクラスです

        /// <summary>
        /// 軸の設定を管理するクラスです(時間用)
        /// </summary>
        public class BL_GraphAxisDateTime : BL_GraphAxis
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public BL_GraphAxisDateTime()
            {
                LocalScale = 10000;
                Offset = DateTime.Now.Ticks;
                HoldOffset = Offset;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="range"></param>
            /// <param name="guidewidth_inner"></param>
            /// <param name="guidewidth_outer"></param>
            /// <param name="separate_volume"></param>
            /// <param name="position"></param>
            /// <param name="width"></param>
            /// <param name="format"></param>
            /// <param name="righttoleft"></param>
            public BL_GraphAxisDateTime(string name, long range, int guidewidth_inner, int guidewidth_outer, long separate_volume, axisPosition position, int width, string format, bool righttoleft)
                : base(name, range, 0, guidewidth_inner, guidewidth_outer, separate_volume, position, width, format, righttoleft, 1.0f)
            {
                LocalScale = 10000;
                Offset = DateTime.Now.Ticks;
                HoldOffset = Offset;
            }

            /// <summary>
            /// 日時を表す文字列を取得します
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public override string ToString(long val)
            {
                DateTime now = new DateTime(val + LocalScale + 500);
                return now.ToString(AxisFormat);
            }
        }

        #endregion

        #region データを管理するクラスです

        /// <summary>
        /// 軸のデータを管理するクラスです
        /// </summary>
        public class BL_GraphSeries
        {
            private string name = "";
            private BL_GraphAxis xaxis = null;
            private BL_GraphAxis yaxis = null;
            private List<BL_GraphData_Base> pointCollection = new List<BL_GraphData_Base>();
            private BL_Graph parent = null;

            private BL_GraphData_Base minimumX = null;
            private BL_GraphData_Base maximumX = null;
            private BL_GraphData_Base minimumY = null;
            private BL_GraphData_Base maximumY = null;

            /// <summary>シリーズ名称を取得または設定します</summary>
            public string Name { get { return name; } set { name = value; } }
            /// <summary>X軸設定を取得または設定します</summary>
            public BL_GraphAxis AxisX { get { return xaxis; } set { xaxis = value; } }
            /// <summary>Y軸設定を取得または設定します</summary>
            public BL_GraphAxis AxisY { get { return yaxis; } set { yaxis = value; } }
            /// <summary>シリーズのデータを取得します</summary>
            public List<BL_GraphData_Base> PointCollection { get { return pointCollection; } }

            private Pen brushpen;
            /// <summary>線の描画色を取得または設定します</summary>
            public Pen BrushPen { get { return brushpen; } set { brushpen = value; } }



            /// <summary>X軸値の最小オブジェクトを取得します</summary>
            public BL_GraphData_Base MinimumX { get { return minimumX; } }
            /// <summary>X軸値の最大オブジェクトを取得します</summary>
            public BL_GraphData_Base MaximumX { get { return maximumX; } }
            /// <summary>Y軸値の最小オブジェクトを取得します</summary>
            public BL_GraphData_Base MinimumY { get { return minimumX; } }
            /// <summary>Y軸値の最大オブジェクトを取得します</summary>
            public BL_GraphData_Base MaximumY { get { return maximumX; } }

            ///// <summary>
            ///// 
            ///// </summary>
            //public BL_GraphSeries() { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="name"></param>
            /// <param name="axisX"></param>
            /// <param name="axisY"></param>
            /// <param name="brushpen"></param>
            /// <param name="pointCollection"></param>
            public BL_GraphSeries(BL_Graph parent, string name, BL_GraphAxis axisX, BL_GraphAxis axisY, Pen brushpen, BL_GraphData_Base[] pointCollection)
            {
                axisX.ControlOwner = parent;
                axisY.ControlOwner = parent;
                axisX.Scrollbar = parent.scrollBarX;
                axisY.Scrollbar = parent.scrollBarY;

                this.parent = parent;
                this.name = name;
                this.xaxis = axisX;
                this.yaxis = axisY;
                this.brushpen = brushpen;

                this.pointCollection = new List<BL_GraphData_Base>();
                if (pointCollection != null)
                {
                    for (int i = 0; i < pointCollection.Length; i++)
                    {
                        this.Add(pointCollection[i]);
                    }
                }
            }

            /// <summary>
            /// XYプロット領域を描画します
            /// </summary>
            /// <param name="g"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public virtual int Draw(Graphics g, int width, int height)
            {
                int drawcount = 0;
                double xx = 0;
                double yy = 0;
                g.ResetTransform();

                xx = 0;
                if (xaxis.RightToLeft) xx = width;
                yy = 0;
                if (yaxis.RightToLeft) yy = height;

                g.ResetTransform();
                g.TranslateTransform((float)xx, (float)yy, MatrixOrder.Append);
                if (!xaxis.RightToLeft) xx = width; else xx = -xx;
                if (!yaxis.RightToLeft) yy = height; else yy = -yy;

                List<float> xlines = new List<float>();
                List<float> ylines = new List<float>();
                Pen pen = (Pen)brushpen.Clone();
                pen.Color = Color.FromArgb(80, pen.Color);
                pen.Width = 0.1f;
                pen.DashStyle = DashStyle.Dot;

                if (0 < Math.Abs(xaxis.GuidelineWidthInner) && 0 < xaxis.ControlWidth)
                {
                    float guidepixels = Math.Abs(xaxis.GetPixels(xaxis.GuidelineWidthInner));

                    float startx = 0;
                    if (xaxis.SmoothMove) startx = Math.Abs(xaxis.GetPixels(xaxis.GetOffset() % xaxis.GuidelineWidthInner));

                    for (float x = startx; x < width; x += guidepixels)
                    {
                        if (!xlines.Contains(x))
                        {
                            xlines.Add(x);
                            g.DrawLine(pen, xaxis.Position(x), 0, xaxis.Position(x), (float)yy);
                        }
                    }
                }

                if (0 < Math.Abs(yaxis.GuidelineWidthInner) && 0 < yaxis.ControlWidth)
                {
                    float guidepixels = Math.Abs(yaxis.GetPixels(yaxis.GuidelineWidthInner));

                    float starty = 0;
                    if (yaxis.SmoothMove) starty = Math.Abs(yaxis.GetPixels(yaxis.GetOffset() % yaxis.GuidelineWidthInner));

                    for (float y = starty; y < height; y += guidepixels)
                    {
                        if (!ylines.Contains(y))
                        {
                            ylines.Add(y);
                            g.DrawLine(pen, 0, yaxis.Position(y), (float)xx, yaxis.Position(y));
                        }
                    }
                }

                lock (pointCollection)
                {
                    int startpos = 0;
                    int endpos = pointCollection.Count - 1;

                    //for (int i = 0; i < pointCollection.Count; i++)
                    //{
                    //    BL_GraphData_Base point = pointCollection[i];
                    //    if (0 == xaxis.IsVisible(point.X, width))
                    //    {
                    //        startpos = i;
                    //        break;
                    //    }
                    //}

                    //for (int i = pointCollection.Count-1; 0<=i; i--)
                    //{
                    //    BL_GraphData_Base point = pointCollection[i];
                    //    if (0 != xaxis.IsVisible(point.X, width))
                    //    {
                    //        endpos = i;
                    //        break;
                    //    }
                    //}

                    for (int i = endpos; startpos <= i; i--)
                    {
                        BL_GraphData_Base point = pointCollection[i];

                        bool isdraw = false;

                        if (0 == xaxis.IsVisible(point.X)) isdraw = true;
                        else if (0 == xaxis.IsVisible(point.NextValue.X)) isdraw = true;
                        else if (0 < xaxis.IsVisible(point.X) && xaxis.IsVisible(point.NextValue.X) < 0 &&
                            (xaxis.SeparateVolume == 0 || Math.Abs(point.NextValue.X - point.X) <= xaxis.SeparateVolume)) isdraw = true;

                        if (isdraw)
                        {
                            int procpoints = point.Draw(g, this, xaxis, yaxis, width, height);
                            i -= procpoints;
                            drawcount++;
                        }
                    }
                }

                return drawcount;
            }

            /// <summary>
            /// X軸値に対応するY軸値を取得します
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public BL_GraphData_Base[] GetYValue(long val)
            {
                List<BL_GraphData_Base> list = new List<BL_GraphData_Base>();
                List<BL_GraphData_Base> listTemp = new List<BL_GraphData_Base>();

                lock (pointCollection)
                {
                    int i = 0;
                    {
                        i = pointCollection.Count - 1;
                        while (0 <= i)
                        {
                            listTemp.Clear();
                            for (; 0 <= i; i--)
                            {
                                BL_GraphData_Base point = pointCollection[i];
                                if (point.NextValue != null)
                                {
                                    if (point.X <= point.NextValue.X && point.X <= val && val < point.NextValue.X)
                                    {
                                        if (AxisX.IsContinueLine(point, point.NextValue))
                                        {
                                            listTemp.Add(point);
                                        }
                                    }
                                    else if (0 < listTemp.Count) break;
                                }
                            }

                            if (0 < listTemp.Count)
                            {
                                BL_GraphData_Base point1 = listTemp[0];
                                BL_GraphData_Base point2 = listTemp[listTemp.Count - 1].NextValue;
                                if (0 != point2.X - point1.X) point1.PerX = ((double)val - (double)point1.X) / (point2.X - point1.X);
                                if (!list.Contains(point1))
                                {
                                    list.Add(point1);
                                }
                            }
                        }
                    }

                    {
                        i = pointCollection.Count - 1;
                        while (0 <= i)
                        {
                            listTemp.Clear();
                            for (; 0 <= i; i--)
                            {
                                BL_GraphData_Base point = pointCollection[i];
                                if (point.NextValue != null)
                                {
                                    if (point.X >= point.NextValue.X && point.X >= val && val > point.NextValue.X)
                                    {
                                        if (AxisX.IsContinueLine(point, point.NextValue))
                                        {
                                            listTemp.Add(point);
                                        }
                                    }
                                    else if (0 < listTemp.Count) break;
                                }
                            }

                            if (0 < listTemp.Count)
                            {
                                BL_GraphData_Base point1 = listTemp[0];
                                BL_GraphData_Base point2 = listTemp[listTemp.Count - 1].NextValue;
                                if (0 != point2.X - point1.X) point1.PerX = ((double)val - (double)point1.X) / (point2.X - point1.X);
                                if (!list.Contains(point1))
                                {
                                    list.Add(point1);
                                }
                            }
                        }
                    }
                }

                return list.ToArray();
            }

            /// <summary>
            /// カーソル位置のY軸値を表す文字列取得します
            /// </summary>
            /// <returns></returns>
            public BL_GraphData_Base[] GetCursorYValue()
            {
                if (AxisY.RightToLeft) AxisY.CursorPixel = AxisY.ControlWidth - AxisY.CursorPixel;
                long val = AxisY.GetValue((float)AxisY.CursorPixel);
                BL_GraphData_Base[] list = GetYValue(val);
                
                return list;
            }

            /// <summary>
            /// カーソル位置のY軸値を表す文字列取得します
            /// </summary>
            /// <returns></returns>
            public string[] GetCursorYValueText()
            {
                List<string> ret = new List<string>();

                if (AxisY.RightToLeft) AxisY.CursorPixel = AxisY.ControlWidth - AxisY.CursorPixel;
                long val = AxisY.GetValue((float)AxisY.CursorPixel);
                BL_GraphData_Base[] list = GetYValue(val);
                for (int i = 0; i < list.Length; i++)
                {
                    BL_GraphData_Base data = list[i];
                    float y = data.GetPositionY(AxisY);
                    ret.Add(data.ToStringY(AxisY, AxisY.AxisFormat));
                }

                return ret.ToArray();
            }

            /// <summary>
            /// XYデータを追加して、保持データ内での最大・最小値を更新します
            /// </summary>
            /// <param name="newpoint"></param>
            public void Add(BL_GraphData_Base newpoint)
            {
                lock (pointCollection)
                {
                    if (0 < pointCollection.Count)
                    {
                        pointCollection[0].NextValue = newpoint;
                        newpoint.PrevValue = pointCollection[0];
                    }
                    pointCollection.Insert(0, newpoint);
                    newpoint.NextValue = newpoint;
                }

                //X軸値の最大・最小を更新する
                if ((AxisX.Range < 0 && AxisX.RightToLeft) || (0 < AxisX.Range && !AxisX.RightToLeft))
                {
                    float xvalue = newpoint.GetPositionX(AxisX);
                    if (minimumX == null) minimumX = newpoint;
                    else if (xvalue < minimumX.GetPositionX(AxisX)) minimumX = newpoint;
                    if (maximumX == null) maximumX = newpoint;
                    else if (maximumX.GetPositionX(AxisX) < xvalue) maximumX = newpoint;
                }
                else
                {
                    float xvalue = newpoint.GetPositionX(AxisX);
                    if (minimumX == null) minimumX = newpoint;
                    else if (xvalue > minimumX.GetPositionX(AxisX)) minimumX = newpoint;
                    if (maximumX == null) maximumX = newpoint;
                    else if (maximumX.GetPositionX(AxisX) < xvalue) maximumX = newpoint;
                }

                //Y軸値の最大・最小を更新する
                float yvalue = newpoint.GetPositionY(AxisY);
                if (minimumY == null) minimumY = newpoint;
                else if (yvalue < minimumY.GetPositionY(AxisY)) minimumY = newpoint;
                if (maximumY == null) maximumY = newpoint;
                else if (maximumY.GetPositionX(AxisY) < yvalue) maximumY = newpoint;
            }
        }

        #endregion

        #region プロパティ

        /// <summary></summary>
        public bool VisibleHeader
        {
            get { return !splitContainerHeader.Panel1Collapsed; }
            set { splitContainerHeader.Panel1Collapsed = !value; }
        }
        /// <summary></summary>
        public bool VisibleFooter
        {
            get { return !splitContainerFooter.Panel2Collapsed; }
            set { splitContainerFooter.Panel2Collapsed = !value; }
        }
        ///// <summary></summary>
        //public bool VisibleVScroll
        //{
        //    get { return !splitContainerPlotRight.Panel2Collapsed; }
        //    set { splitContainerPlotRight.Panel2Collapsed = !value; }
        //}
        ///// <summary></summary>
        //public bool VisibleHScroll
        //{
        //    get { return !splitContainerPlotBottom.Panel2Collapsed; }
        //    set
        //    {
        //        splitContainerPlotBottom.Panel2Collapsed = !value;
        //        splitContainerPlotLeftBottom.Panel2Collapsed = !value;
        //        splitContainerPlotRightBottom.Panel2Collapsed = !value;
        //    }
        //}
        ///// <summary></summary>
        //public bool VisibleTrackerLeft
        //{
        //    get { return trackBarY.Visible; }
        //    set { trackBarY.Visible = value; }
        //}

        private bool visiblecursor = false;
        /// <summary></summary>
        public bool VisibleCursor
        {
            get { return visiblecursor; }
            set { visiblecursor = value; }
        }

        /// <summary>描画領域の背景色を取得または設定します</summary>
        public Color PlotBackColor { get { return panelPlot.BackColor; } set { panelPlot.BackColor = value; } }
        /// <summary>グラフ領域全域の背景色を取得または設定します</summary>
        public BorderStyle GraphBorderStyle { get { return this.BorderStyle; } set { this.BorderStyle = value; } }
        /// <summary>描画領域の背景色を取得または設定します</summary>
        public BorderStyle PlotBorderStyle { get { return panelPlot.BorderStyle; } set { panelPlot.BorderStyle = value; } }

        /// <summary>カーソル色を取得または設定します</summary>
        public Color CursorXColor { get { return panelCursorX.BackColor; } set { panelCursorX.BackColor = value; } }

        #endregion

        /// <summary>軸のデータを取得または設定します</summary>
        protected Dictionary<string, BL_GraphSeries> seriesCollection = new Dictionary<string, BL_GraphSeries>();

        #region インデクサ

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BL_GraphSeries this[string name]
        {
            get
            {
                if (seriesCollection.ContainsKey(name)) return seriesCollection[name];
                return null;
            }

            set
            {
                seriesCollection[name] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BL_GraphSeries this[int index]
        {
            get
            {
                if (0 <= index && index < seriesCollection.Count)
                {
                    int i = 0;
                    foreach (var v in seriesCollection.Values)
                    {
                        if (i == index) return v;
                        i++;
                    }
                    //return seriesCollection.ElementAt(index).Value;
                }

                return null;
            }
        }

        #endregion

        #region デフォルトコンストラクタ

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public BL_Graph()
        {
            InitializeComponent();

            splitContainerPlotBottom.Panel2MinSize = 0;
            splitContainerPlotBottom.SplitterDistance = splitContainerPlotBottom.Size.Height;
            splitContainerPlotLeftBottom.Panel2MinSize = 0;
            splitContainerPlotLeftBottom.SplitterDistance = splitContainerPlotLeftBottom.Size.Height;
            splitContainerPlotRightBottom.Panel2MinSize = 0;
            splitContainerPlotRightBottom.SplitterDistance = splitContainerPlotRightBottom.Size.Height;

            splitContainerPlotRight.Panel2MinSize = 0;
            splitContainerPlotRight.SplitterDistance = splitContainerPlotRight.Size.Width;

            seriesCollection = new Dictionary<string, BL_GraphSeries>();

            panelCursorX.BackColor = Color.FromArgb(80, panelCursorX.BackColor);
            panelCursorX.Width = 5;
            panelCursorX.Visible = false;

            trackBarX.Minimum = 0;
            trackBarX.Maximum = 0;
            trackBarY.Minimum = 0;
            trackBarY.Maximum = 0;
            trackBarX.Visible = false;
            trackBarY.Visible = false;
        }

        #endregion

        #region 描画イベント処理

        private void panelAxisBottom_Paint(object sender, PaintEventArgs e)
        {
            if (seriesCollection == null) return;
            if (seriesCollection.Count == 0) return;

            int totalaxiswidth = 0;

            Dictionary<string, int> list = new Dictionary<string, int>();

            foreach (var series in seriesCollection.Values)
            {
                if (series.AxisX.AxisPosition == BL_GraphAxis.axisPosition.Bottom)
                {
                    if (!list.ContainsKey(series.AxisX.Name))
                    {
                        list[series.AxisX.Name] = totalaxiswidth;
                        totalaxiswidth += series.AxisX.AxisWidth;
                    }
                }
            }

            splitContainerAxisBottom.SplitterDistance = splitContainerAxisBottom.Size.Height - totalaxiswidth;

            list.Clear();
            int axiswidth = 0;
            foreach (var series in seriesCollection.Values)
            {
                if (series.AxisX.AxisPosition == BL_GraphAxis.axisPosition.Bottom)
                {
                    if (!list.ContainsKey(series.AxisX.Name))
                    {
                        list[series.AxisX.Name] = axiswidth;
                        series.AxisX.CursorPixel = panelCursorX.Left + panelCursorX.Width / 2;

                        if (0 < bottom_clickedlocation.Y)
                        {
                            if (axiswidth <= bottom_clickedlocation.Y && bottom_clickedlocation.Y < axiswidth + series.AxisX.AxisWidth)
                            {
                                bottom_clickedlocation.Y = 0;
                                SetScrollXTarget(series.AxisX.Name);
                                checkBoxFixedX.Checked = series.AxisX.IsHold;
                                series.AxisX.IsHold = false;
                                series.AxisX.Scroll_Update();
                                series.AxisX.IsHold = checkBoxFixedX.Checked;
                                if (series.AxisX.ScrollPosition < series.AxisX.Scrollbar.Maximum) series.AxisX.Scrollbar.Value = series.AxisX.ScrollPosition;
                                else series.AxisX.Scrollbar.Value = series.AxisX.Scrollbar.Maximum;
                            }
                        }

                        series.AxisX.Draw(e.Graphics, panelPlot.Width, splitContainerPlotLeft.SplitterDistance + splitContainerPlotLeft.SplitterWidth, list[series.AxisX.Name], series, false, visiblecursor);

                        axiswidth += series.AxisX.AxisWidth;
                    }
                    else
                    {
                        series.AxisX.CursorPixel = panelCursorX.Left + panelCursorX.Width / 2;
                        series.AxisX.Draw(e.Graphics, panelPlot.Width, splitContainerPlotLeft.SplitterDistance + splitContainerPlotLeft.SplitterWidth, list[series.AxisX.Name], series, true, visiblecursor);
                    }
                }
            }

            list.Clear();
        }

        private void panelAxisLeft_Paint(object sender, PaintEventArgs e)
        {
            if (seriesCollection == null) return;
            if (seriesCollection.Count == 0) return;

            int totalaxiswidth = 0;

            Dictionary<string, int> list = new Dictionary<string, int>();
            foreach (var v in seriesCollection.Values)
            {
                BL_GraphSeries series = v;

                if (series.AxisY.AxisPosition == BL_GraphAxis.axisPosition.Left)
                {
                    if (!list.ContainsKey(series.AxisY.Name))
                    {
                        list[series.AxisY.Name] = totalaxiswidth;
                        totalaxiswidth += series.AxisY.AxisWidth;
                    }
                }
            }

            splitContainerAxisLeft.SplitterDistance = totalaxiswidth;
            splitContainerAxisBottomLeft.SplitterDistance = totalaxiswidth;

            list.Clear();
            int axiswidth = totalaxiswidth;
            foreach (var series in seriesCollection.Values)
            {
                if (series.AxisY.AxisPosition == BL_GraphAxis.axisPosition.Left)
                {
                    if (!list.ContainsKey(series.AxisY.Name))
                    {
                        axiswidth -= series.AxisY.AxisWidth;
                        list[series.AxisY.Name] = axiswidth;
                        series.AxisY.CursorPixel = panelCursorX.Left + panelCursorX.Width / 2;
                        series.AxisY.Draw(e.Graphics, panelPlot.Height, splitContainerPlotTop.SplitterDistance + splitContainerPlotTop.SplitterWidth, list[series.AxisY.Name], series, false, visiblecursor);
                    }
                    else
                    {
                        series.AxisY.CursorPixel = panelCursorX.Left + panelCursorX.Width / 2;
                        series.AxisY.Draw(e.Graphics, panelPlot.Height, splitContainerPlotTop.SplitterDistance + splitContainerPlotTop.SplitterWidth, list[series.AxisY.Name], series, true, visiblecursor);
                    }
                }
            }

            list.Clear();
        }

        private void panelAxisRight_Paint(object sender, PaintEventArgs e)
        {
            if (seriesCollection == null) return;
            if (seriesCollection.Count == 0) return;

            int totalaxiswidth = 0;

            Dictionary<string, int> list = new Dictionary<string, int>();
            foreach (var series in seriesCollection.Values)
            {
                if (series.AxisY.AxisPosition == BL_GraphAxis.axisPosition.Right)
                {
                    if (!list.ContainsKey(series.AxisY.Name))
                    {
                        list[series.AxisY.Name] = totalaxiswidth;
                        totalaxiswidth += series.AxisY.AxisWidth;
                    }
                }
            }

            splitContainerAxisRight.SplitterDistance = splitContainerAxisRight.Width - totalaxiswidth;
            splitContainerAxisBottomRight.SplitterDistance = splitContainerAxisBottomRight.Width - totalaxiswidth;

            list.Clear();
            int axiswidth = 0;
            foreach (var series in seriesCollection.Values)
            {
                if (series.AxisY.AxisPosition == BL_GraphAxis.axisPosition.Right)
                {
                    if (!list.ContainsKey(series.AxisY.Name))
                    {
                        list[series.AxisY.Name] = axiswidth;
                        series.AxisY.CursorPixel = panelCursorX.Left + panelCursorX.Width / 2;
                        series.AxisY.Draw(e.Graphics, panelPlot.Height, splitContainerPlotTop.SplitterDistance + splitContainerPlotTop.SplitterWidth, list[series.AxisY.Name], series, false, visiblecursor);
                        axiswidth += series.AxisY.AxisWidth;
                    }
                    else
                    {
                        series.AxisY.CursorPixel = panelCursorX.Left + panelCursorX.Width / 2;
                        series.AxisY.Draw(e.Graphics, panelPlot.Height, splitContainerPlotTop.SplitterDistance + splitContainerPlotTop.SplitterWidth, list[series.AxisY.Name], series, true, visiblecursor);
                    }
                }
            }

            list.Clear();
        }

        private void panelPlot_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;

            if (seriesCollection == null) return;
            if (seriesCollection.Count == 0) return;

            List<BL_GraphSeries> list = new List<BL_GraphSeries>();
            foreach (var v in seriesCollection.Values) list.Add(v);

            for (int i = list.Count - 1; 0 <= i; i--)
            {
                BL_GraphSeries series = list[i];
                int drawpoints = series.Draw(e.Graphics, panelPlot.Width, panelPlot.Height);

                e.Graphics.ResetTransform();

                Brush br = series.BrushPen.Brush;
                if (0 < series.PointCollection.Count)
                {
                    if (typeof(BL_GraphBlock).IsInstanceOfType(series.PointCollection[0]))
                    {
                        BL_GraphBlock gb = (BL_GraphBlock)series.PointCollection[0];
                        br = gb.Block.brush;

                        e.Graphics.FillRectangle(gb.Block.brush, 10, 10 + i * this.Font.Height, 100, this.Font.Height);
                        e.Graphics.DrawRectangle(new Pen(series.BrushPen.Brush), 10, 10 + i * this.Font.Height, 100, this.Font.Height);
                        e.Graphics.DrawString(series.Name, this.Font, series.BrushPen.Brush, 10, 10 + i * this.Font.Height);
                    }
                    else
                    {
                        e.Graphics.DrawString("data count:" + series.PointCollection.Count.ToString() + "/drawpoints:" + drawpoints.ToString(), this.Font, series.BrushPen.Brush, 10, 10 + i * this.Font.Height);
                    }
                }
            }
        }

        #endregion

        #region Xカーソル操作

        private bool iscursorXdrag = false;
        private void panelCursorX_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                iscursorXdrag = true;

                panelAxisBottom.Refresh();
                panelAxisLeft.Refresh();
                panelAxisRight.Refresh();
                panelPlot.Refresh();
                panelCursorX.Refresh();
            }
        }

        private void panelCursorX_MouseMove(object sender, MouseEventArgs e)
        {
            if (iscursorXdrag)
            {
                Point screenpoint = panelCursorX.PointToScreen(e.Location);
                int newLeft = panelPlot.PointToClient(screenpoint).X - panelCursorX.Width / 2;
                
                if (panelCursorX.Left != newLeft)
                {
                    panelCursorX.Left = newLeft;
                    if (panelCursorX.Left < -panelCursorX.Width / 2) panelCursorX.Left = -panelCursorX.Width / 2;
                    if (panelPlot.Width - panelCursorX.Width / 2 < panelCursorX.Left) panelCursorX.Left = panelPlot.Width - panelCursorX.Width / 2;

                    panelAxisBottom.Refresh();
                    panelAxisLeft.Refresh();
                    panelAxisRight.Refresh();
                    panelPlot.Refresh();
                    panelCursorX.Refresh();
                }
            }
        }

        private void panelCursorX_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                panelAxisBottom.Refresh();
                panelAxisLeft.Refresh();
                panelAxisRight.Refresh();
                panelPlot.Refresh();
                panelCursorX.Refresh();

                iscursorXdrag = false;
            }
        }

        private void panelCursorX_Paint(object sender, PaintEventArgs e)
        {
            if (iscursorXdrag)
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(200, panelCursorX.BackColor)), panelCursorX.Width / 2, 0, panelCursorX.Width / 2, panelCursorX.Height);
            }
            else
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(128, panelCursorX.BackColor)), panelCursorX.Width / 2, 0, panelCursorX.Width / 2, panelCursorX.Height);
            }
        }

        private void BL_Graph_Resize(object sender, EventArgs e)
        {
            if (panelCursorX.Left < -panelCursorX.Width / 2) panelCursorX.Left = -panelCursorX.Width / 2;
            if (panelPlot.Width - panelCursorX.Width / 2 < panelCursorX.Left) panelCursorX.Left = panelPlot.Width - panelCursorX.Width / 2;
            panelAxisLeft.Refresh();
        }

        #endregion

        #region 軸データ操作

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="xaxis"></param>
        /// <param name="yaxis"></param>
        /// <param name="brushpen"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public BL_GraphSeries AddSeries(string name, BL_GraphAxis xaxis, BL_GraphAxis yaxis, Pen brushpen, BL_GraphData_Base[] points)
        {
            BL_GraphSeries series = new BL_GraphSeries(this, name, xaxis, yaxis, brushpen, points);
            if (!seriesCollection.ContainsKey(name))
            {
                seriesCollection[name] = series;
                return series;
            }

            return null;
        }

        #endregion

        #region スクロール操作関連

        /// <summary>
        /// 横スクロールバーの操作対称軸を設定します
        /// </summary>
        /// <param name="axis_name"></param>
        public bool SetScrollXTarget(string axis_name)
        {
            bool first = true;
            if (scrollBarX.Tag != null) first = false;
            
            string series_name = "";
            foreach (var s in seriesCollection.Values)
            {
                if (axis_name == s.AxisX.Name)
                {
                    series_name = s.Name;
                    break;
                }
            }

            if (!seriesCollection.ContainsKey(series_name)) return false;

            BL_GraphSeries series = seriesCollection[series_name];
            series.AxisX.ControlWidth = panelPlot.Width;
            scrollBarX.Tag = series;

            splitContainerPlotBottom.SplitterDistance = splitContainerPlotBottom.Size.Height - 20;
            splitContainerPlotLeftBottom.SplitterDistance = splitContainerPlotLeftBottom.Size.Height - 20;
            splitContainerPlotRightBottom.SplitterDistance = splitContainerPlotRightBottom.Size.Height - 20;
            splitContainerPlotBottom.Panel2.BackColor = series.BrushPen.Color;

            scrollBarX.RightToLeft = System.Windows.Forms.RightToLeft.No;
            if (series.AxisX.RightToLeft) scrollBarX.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            scrollBarX.Dock = DockStyle.None;
            scrollBarX.Left = 2;
            scrollBarX.Top = 2;
            scrollBarX.Width = splitContainerPlotBottom.Panel2.Width - 4 - checkBoxFixedX.Width - 8;
            scrollBarX.Height = splitContainerPlotBottom.Panel2.Height - 4;
            scrollBarX.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            int innerguidecount = (int)(panelPlot.Width / Math.Abs(series.AxisX.GetPixels(series.AxisX.GuidelineWidthInner)) + 0.999999);
            scrollBarX.SmallChange = 1;
            scrollBarX.LargeChange = innerguidecount;
            scrollBarX.Visible = true;

            if (first)
            {
                panelCursorX.BackColor = Color.FromArgb(80, panelCursorX.BackColor);
                panelCursorX.Width = 7;
                if (visiblecursor) panelCursorX.Visible = true;

                if (series.AxisX.RightToLeft)
                {
                    panelCursorX.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
                    panelCursorX.Left = panelPlot.Width - panelCursorX.Width / 2;
                }
                else
                {
                    panelCursorX.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
                    panelCursorX.Left = -panelCursorX.Width / 2;
                }
            }

            return true;
        }

        /// <summary>
        /// 縦スクロールバーの操作対称軸を設定します
        /// </summary>
        /// <param name="axis_name"></param>
        public bool SetScrollYTarget(string axis_name)
        {
            string series_name = "";

            foreach (var s in seriesCollection.Values)
            {
                if (axis_name == s.AxisY.Name)
                {
                    series_name = s.Name;
                    break;
                }
            }

            if (!seriesCollection.ContainsKey(series_name)) return false;

            BL_GraphSeries series = seriesCollection[series_name];
            series.AxisY.ControlWidth = panelPlot.Width;
            scrollBarY.Tag = series;

            splitContainerPlotRight.SplitterDistance = splitContainerPlotRight.Size.Width - 20;
            splitContainerPlotRightBottom.Panel1.BackColor = series.BrushPen.Color;

            scrollBarY.RightToLeft = System.Windows.Forms.RightToLeft.No;
            if (series.AxisY.RightToLeft) scrollBarY.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            scrollBarY.Dock = DockStyle.None;
            scrollBarY.Left = 2;
            scrollBarY.Top = 2;
            scrollBarY.Width = splitContainerPlotRightBottom.Panel1.Width - 4;
            scrollBarY.Height = splitContainerPlotRightBottom.Panel1.Height - 4 - checkBoxFixedY.Height - 8;
            scrollBarY.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            int innerguidecount = (int)(panelPlot.Height / Math.Abs(series.AxisY.GetPixels(series.AxisY.GuidelineWidthInner)) + 0.999999);
            scrollBarY.SmallChange = 1;
            scrollBarY.LargeChange = innerguidecount;
            scrollBarY.Visible = true;

            return true;
        }

        private void scrollBarX_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrollBarX.Tag == null) return;
            if (!typeof(BL_GraphSeries).IsInstanceOfType(scrollBarX.Tag)) return;
            BL_GraphAxis axis = ((BL_GraphSeries)scrollBarX.Tag).AxisX;

            axis.Scroll_Changed(e.NewValue);
            Refresh();
        }

        private void scrollBarY_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrollBarY.Tag == null) return;
            if (!typeof(BL_GraphSeries).IsInstanceOfType(scrollBarY.Tag)) return;
            BL_GraphAxis axis = ((BL_GraphSeries)scrollBarY.Tag).AxisY;

            axis.Scroll_Changed(e.NewValue);
            Refresh();
        }

        private void checkBoxFixedX_CheckedChanged(object sender, EventArgs e)
        {
            if (scrollBarX.Tag == null) return;
            if (!typeof(BL_GraphSeries).IsInstanceOfType(scrollBarX.Tag)) return;
            BL_GraphAxis axis = ((BL_GraphSeries)scrollBarX.Tag).AxisX;

            axis.IsHold = checkBoxFixedX.Checked;

            if (checkBoxFixedX.Checked) checkBoxFixedX.Text = "●";
            else checkBoxFixedX.Text = "";

            if (!axis.IsHold) axis.HoldOffset = axis.Offset;
        }

        private void checkBoxFixedY_CheckedChanged(object sender, EventArgs e)
        {
            if (scrollBarX.Tag == null) return;
            if (!typeof(BL_GraphSeries).IsInstanceOfType(scrollBarY.Tag)) return;
            BL_GraphAxis axis = ((BL_GraphSeries)scrollBarY.Tag).AxisY;

            axis.IsHold = checkBoxFixedY.Checked;

            if (checkBoxFixedY.Checked) checkBoxFixedY.Text = "●";
            else checkBoxFixedY.Text = "";

            if (!axis.IsHold) axis.HoldOffset = axis.Offset;
        }

        private Point bottom_clickedlocation = new Point(0, 0);

        private void panelAxisBottom_MouseClick(object sender, MouseEventArgs e)
        {
            bottom_clickedlocation = e.Location;
            panelAxisBottom.Refresh();
        }

        #endregion

        #region トラックバー操作関連

        private class BL_TrackbarInfo
        {
            public BL_GraphSeries series;
            public long range;
            public int guideinner;
            public int guideouter;

            public BL_TrackbarInfo(BL_GraphSeries series, long range, int guideinner, int guideouter)
            {
                this.series = series;
                this.range = range;
                this.guideinner = guideinner;
                this.guideouter = guideouter;
            }

            public void UpdateX()
            {
                series.AxisX.Range = range;
                series.AxisX.GuidelineWidthInner = guideinner;
                series.AxisX.GuidelineWidthOuter = guideouter;

                bool ishold = series.AxisX.IsHold;
                series.AxisX.IsHold = false;
                series.AxisX.Scroll_Update();
                series.AxisX.IsHold = ishold;
            }

            public void UpdateY()
            {
                series.AxisY.Range = range;
                series.AxisY.GuidelineWidthInner = guideinner;
                series.AxisY.GuidelineWidthOuter = guideouter;

                bool ishold = series.AxisY.IsHold;
                series.AxisY.IsHold = false;
                series.AxisY.Scroll_Update();
                series.AxisY.IsHold = ishold;
            }
        }

        private Dictionary<string, List<BL_TrackbarInfo>> listRangesX = new Dictionary<string, List<BL_TrackbarInfo>>();
        private Dictionary<string, List<BL_TrackbarInfo>> listRangesY = new Dictionary<string, List<BL_TrackbarInfo>>();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis_name"></param>
        /// <param name="range"></param>
        /// <param name="guideinner"></param>
        /// <param name="guideouter"></param>
        /// <returns></returns>
        public bool AddXRange(string axis_name, long range, int guideinner, int guideouter)
        {
            string series_name = "";
            foreach (var s in seriesCollection.Values)
            {
                if (axis_name == s.AxisX.Name)
                {
                    series_name = s.Name;
                    break;
                }
            }

            if (!seriesCollection.ContainsKey(series_name)) return false;

            BL_GraphSeries series = seriesCollection[series_name];
            trackBarX.Visible = true;

            BL_TrackbarInfo info = new BL_TrackbarInfo(series, range, guideinner, guideouter);

            if (!listRangesX.ContainsKey(axis_name))
            {
                info.UpdateX();
                listRangesX[axis_name] = new List<BL_TrackbarInfo>();
            }

            listRangesX[axis_name].Add(info);
            if (trackBarX.Maximum < listRangesX[axis_name].Count - 1) trackBarX.Maximum = listRangesX[axis_name].Count - 1;

            trackBarX.RightToLeft = System.Windows.Forms.RightToLeft.No;
            if (series.AxisX.RightToLeft) trackBarX.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarX_Scroll(object sender, EventArgs e)
        {
            foreach (var x in listRangesX.Values)
            {
                if (trackBarX.Value < x.Count)
                {
                    BL_TrackbarInfo info = x[trackBarX.Value];
                    info.UpdateX();
                }
            }
            Refresh();
        }

        #endregion
    }
}
