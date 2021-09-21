using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BelicsClass.UI
{
    /// <summary>
    /// メインフォームの基本クラスです。
    /// ファンクションキーのボタン表示が大きめです
    /// </summary>
    public partial class BL_MainForm_Base_LargeFunctions : BelicsClass.UI.BL_MainForm_Base
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base_LargeFunctions()
            : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="enable_gesture"></param>
        public BL_MainForm_Base_LargeFunctions(bool enable_gesture)
            : base(enable_gesture)
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="wallpaper"></param>
        public BL_MainForm_Base_LargeFunctions(Bitmap wallpaper)
            : base(wallpaper)
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="wallpaper"></param>
        /// <param name="enable_gesture"></param>
        public BL_MainForm_Base_LargeFunctions(Bitmap wallpaper, bool enable_gesture)
            : base(wallpaper, enable_gesture)
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="icon"></param>
        public BL_MainForm_Base_LargeFunctions(Icon icon)
            : base(icon)
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="enable_gesture"></param>
        public BL_MainForm_Base_LargeFunctions(Icon icon, bool enable_gesture)
            : base(icon, enable_gesture)
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="wallpaper"></param>
        /// <param name="icon"></param>
        public BL_MainForm_Base_LargeFunctions(Bitmap wallpaper, Icon icon)
            : base(wallpaper, icon)
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="wallpaper"></param>
        /// <param name="icon"></param>
        /// <param name="enable_gesture"></param>
        public BL_MainForm_Base_LargeFunctions(Bitmap wallpaper, Icon icon, bool enable_gesture)
            : base(wallpaper, icon,  enable_gesture)
        {
            InitializeComponent();
        }
    }
}
