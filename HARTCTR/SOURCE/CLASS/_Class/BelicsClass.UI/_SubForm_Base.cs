using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.PowerPacks;

using BelicsClass.Common;
using BelicsClass.ProcessManage;
using System.Security.Permissions;

namespace BelicsClass.UI
{
	/// <summary>
	/// BLUIフレームワークのサブフォーム基本クラスです
	/// BL_MainForm_Baseのサブフォームパネル内に組み込み表示を行います
	/// 
	/// </summary>
	public partial class BL_SubForm_Base : Form
	{
		#region 派生サブフォームへ追加してください

		/// <summary>
		/// ファンクションキー文字列をMainFormに取得させるために必要です。
		/// 派生サブフォームでは、virtual を override に変更してください。
		/// </summary>
		/// <returns></returns>
		virtual public string[] FunctionStrings()
		{
			return new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "" };

			#region 自動化しようとした残骸
			////ボタンをファンクションに登録する(AttachButton_to_Function)呼び出しよりも先にMainFormから参照されるためうまくいかなかった。
			//string[] func = new string[13];
			//for (int functionno = 0; functionno <= 12; functionno++)
			//{
			//    func[functionno] = "";
			//    if (dictFunctions.ContainsKey(functionno))
			//    {
			//        object obj = dictFunctions[functionno];
			//        switch (obj.GetType().Name)
			//        {
			//            case "Button": func[functionno] = ((Button)obj).Text;
			//                break;
			//        }

			//        if (func[functionno] != "")
			//        {
			//            if (func[functionno].IndexOf(":") < 0 && func[functionno].IndexOf("[") < 0 && func[functionno].IndexOf("]") < 0)
			//            {
			//                func[functionno] = "[F" + functionno.ToString() + "]:" + func[functionno];
			//            }
			//            if (func[functionno].IndexOf(":") < 0 && 0 <= func[functionno].IndexOf("[") && 0 <= func[functionno].IndexOf("]"))
			//            {
			//                if (0 <= func[functionno].IndexOf("] ")) func[functionno] = func[functionno].Replace("] ", "]:");
			//                else func[functionno] = func[functionno].Replace("]", "]:");
			//            }
			//        }
			//    }
			//}
			//return func;
			#endregion
		}
		/// <summary>
		/// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
		/// サブフォームでは、virtual を override に変更してください。
		/// </summary>
		/// <returns></returns>
		virtual public string _TitleString
		{
			get { return this.TitleString(); }
			set { this.SetTitleString(value); }
		}
		/// <summary>
		/// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
		/// サブフォームでは、virtual を override に変更してください。
		/// </summary>
		/// <returns></returns>
		virtual public string TitleString()
		{
			return this.Text;
		}

		/// <summary>
		/// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
		/// サブフォームでは、virtual を override に変更してください。
		/// </summary>
		/// <returns></returns>
		virtual public void SetTitleString(string title)
		{
			this.Text = title;
			if (m_Mainform != null)
			{
				m_Mainform.Title = title;
			}
		}

		#endregion

		#region 自動スケーリングオブジェクト

		/// <summary>
		/// 自動スケーリングオブジェクト
		/// </summary>
		protected BL_MainForm_Base.ViewResize ViewResizer = null;

		#endregion

		#region IME制御

		[DllImport("Imm32.DLL")]
		private extern static int ImmSetOpenStatus(IntPtr hWnd, int fOpen);

		[DllImport("Imm32.DLL")]
		private extern static IntPtr ImmGetContext(IntPtr hWnd);

		[DllImport("Imm32.dll")]
		private extern static bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

		#endregion

		#region オーバーロードして実装してください

		/// <summary></summary>
		protected virtual void SubForm_Base_Load(object sender, EventArgs e)
		{
			Log(this.TitleString() + " - LOAD");
			//if (EnableResize) Resizer_Initialize();
		}

		/// <summary></summary>
		protected virtual void SubForm_Base_FunctionESC_Clicked(object sender)
		{
			Log(this.TitleString() + " - ESC");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function01_Clicked(object sender)
		{
			Log(this.TitleString() + " - F01");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function02_Clicked(object sender)
		{
			Log(this.TitleString() + " - F02");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function03_Clicked(object sender)
		{
			Log(this.TitleString() + " - F03");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function04_Clicked(object sender)
		{
			Log(this.TitleString() + " - F04");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function05_Clicked(object sender)
		{
			Log(this.TitleString() + " - F05");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function06_Clicked(object sender)
		{
			Log(this.TitleString() + " - F06");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function07_Clicked(object sender)
		{
			Log(this.TitleString() + " - F07");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function08_Clicked(object sender)
		{
			Log(this.TitleString() + " - F08");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function09_Clicked(object sender)
		{
			Log(this.TitleString() + " - F09");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function10_Clicked(object sender)
		{
			Log(this.TitleString() + " - F10");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function11_Clicked(object sender)
		{
			Log(this.TitleString() + " - F11");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function12_Clicked(object sender)
		{
			Log(this.TitleString() + " - F12");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_FunctionElse_Clicked(object sender)
		{
			Log(this.TitleString() + " - F13");
		}

		/// <summary></summary>
		protected virtual void SubForm_Base_FunctionESC_MouseDown(object sender)
		{
			Log(this.TitleString() + " - ESC");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function01_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F01");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function02_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F02");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function03_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F03");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function04_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F04");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function05_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F05");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function06_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F06");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function07_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F07");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function08_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F08");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function09_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F09");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function10_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F10");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function11_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F11");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function12_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F12");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_FunctionElse_MouseDown(object sender)
		{
			Log(this.TitleString() + " - F13");
		}

		/// <summary></summary>
		protected virtual void SubForm_Base_FunctionESC_MouseUp(object sender)
		{
			Log(this.TitleString() + " - ESC");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function01_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F01");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function02_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F02");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function03_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F03");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function04_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F04");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function05_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F05");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function06_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F06");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function07_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F07");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function08_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F08");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function09_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F09");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function10_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F10");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function11_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F11");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_Function12_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F12");
		}
		/// <summary></summary>
		protected virtual void SubForm_Base_FunctionElse_MouseUp(object sender)
		{
			Log(this.TitleString() + " - F13");
		}

		/// <summary></summary>
		protected virtual void SubForm_Base_MouseMove(object sender, MouseEventArgs e)
		{
		}

		/// <summary></summary>
		public virtual void SubForm_Base_KeyDown(object sender, KeyEventArgs e)
		{
			if (!((e.Modifiers & Keys.Shift) == Keys.Shift))
			{
				if (ActiveControl != null)
				{
					if (ActiveControl.Parent != null)
					{
						bool nocontrol = false;
						if (0 <= ActiveControl.GetType().Name.IndexOf("EnumCellEditor"))
						{
							ComboBox cb = (ComboBox)ActiveControl;
							if (cb.DroppedDown)
							{
								if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
								{
									nocontrol = true;
								}
							}
							else if (e.KeyCode == Keys.Enter)
							{
								SendKeys.SendWait("{F4}");
								e.Handled = true;
								return;
							}
						}

						if (!nocontrol)
						{
							if (0 <= ActiveControl.Parent.GetType().Name.IndexOf("ObjectListView"))
							{
								switch (e.KeyCode)
								{
									case Keys.Up:
										SendKeys.SendWait("+{ENTER}");
										e.Handled = true;
										return;

									case Keys.Down:
										SendKeys.SendWait("{ENTER}");
										e.Handled = true;
										return;

									case Keys.Left:
										SendKeys.SendWait("+{TAB}");
										e.Handled = true;
										return;

									case Keys.Right:
										SendKeys.SendWait("{TAB}");
										e.Handled = true;
										return;
								}
							}
						}
					}
				}
			}
		}

		/// <summary></summary>
		public virtual void SubForm_Base_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (m_Mainform != null)
			{
				if (m_Mainform.IsTopLevelSubForm(this))
				{
					if (e.KeyChar == '\r')
					{
						if (typeof(DataGridView).IsInstanceOfType(ActiveControl))
						{
							//...DataGridViewは制御しない
						}
						else
						{
							SendKeys.Send("{TAB}");

							if (typeof(TextBox).IsInstanceOfType(ActiveControl))
							{
								TextBox tb = ActiveControl as TextBox;
								tb.SelectAll();
							}

							e.Handled = true;
						}
					}
					else if (e.KeyChar == ' ')
					{
						if (typeof(CheckBox).IsInstanceOfType(ActiveControl))
						{
							CheckBox cb = ActiveControl as CheckBox;
							cb.Checked = !cb.Checked;
						}
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual public void SubForm_Base_KeyUp(object sender, KeyEventArgs e)
		{
			if (m_Functions != null)
			{
				m_Functions.BL_SubForm_Functions_KeyUp(sender, e);
			}
		}

		/// <summary>
		/// デフォルト500msの定周期タイマーです。
		/// サブフォームがアクティブな間、自動的に駆動します。
		/// サブフォームが非アクティブになると、自動的に停止します。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual protected void timerClock_Tick(object sender, EventArgs e)
		{
			//Application.DoEvents();
			if (m_bLoadCancel)
			{
				Log(this.TitleString() + "LOAD CANCEL");
				Close();
			}
		}

		/// <summary>
		/// サブフォームがアクティブになった時に呼び出されます。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual public void SubForm_Base_Activated(object sender, EventArgs e)
		{
			if (this.IsHandleCreated)
			{
				var hIMC = ImmGetContext(this.Handle);
				var status = ImmSetOpenStatus(hIMC, 0);
				ImmReleaseContext(this.Handle, hIMC);
			}

			if (ViewResizer != null)
			{
				if (ViewResizer.Count == 0) ViewResizer.Resize(Width, Height);

				if (m_Mainform != null)
				{
					if (m_Mainform.panelSubForm.Contains(this))
					{
						Width = m_Mainform.panelSubForm.Width;
						Height = m_Mainform.panelSubForm.Height;
						Application.DoEvents();
					}
				}

				ViewResizer.Resize(this);
			}
			//else
			//{
			//	if (m_Mainform != null)
			//	{
			//		m_Mainform.Width--;
			//		m_Mainform.Width++;
			//	}
			//}

			timerClock.Enabled = true;

			if (m_bLoaded)
			{
				timerClock_Tick(sender, e);
			}

			Log(this.TitleString() + " - ACTIVATE");
		}

		/// <summary>
		/// サブフォームが非アクティブになった時に呼び出されます。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual public void SubForm_Base_Deactivate(object sender, EventArgs e)
		{
			timerClock.Enabled = false;

			Log(this.TitleString() + " - DEACTIVATE");
		}

		/// <summary>
		/// メインフォームがアクティブになった時に呼び出されます。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual public void SubForm_Base_MainformActivated(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// メインフォームが非アクティブになった時に呼び出されます。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual public void SubForm_Base_MainformDeactivate(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// 必要に応じて終了できないメッセージを表示します。
		/// メッセージが不要の場合、オーバーロード処理前に呼び出してください。
		/// （この場合、timerClock.Enabled = falseに変更されます。）
		/// メッセージが必要な場合、オーバーロード処理後に呼び出してください。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual protected void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.Cancel)
			{
				BL_MessageBox mb = new BL_MessageBox(this);
				mb.ShowMessage("処理中のため終了できません。", "処理中", null);
				return;
			}

			SubForm_Base_Deactivate(this, new EventArgs());
			timerClock.Enabled = false;

			Log(this.TitleString() + " - CLOSING");
		}

		/// <summary>
		/// スレッドコレクションを捜査して終了可能かどうかを返します。
		/// </summary>
		virtual public string CanExit
		{
			get
			{
				for (int i = 0; i < BL_ThreadCollector.Count; i++) if (!BL_ThreadCollector.Get(i).CanFinish) return "処理中のため終了できません。";
				return "";
			}
		}

		/// <summary>
		/// クローズ時の処理（オーバーロード処理後、呼び出してください）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual protected void SubForm_Base_FormClosed(object sender, FormClosedEventArgs e)
		{
			Resizer_Finalize();

			foreach (KeyValuePair<int, object> kv in dictFunctions)
			{
				object obj = kv.Value;

				if (typeof(Button).IsInstanceOfType(obj))
				{
					((Button)obj).Click -= new EventHandler(btnFunctions_Click);
				}
			}

			dictFunctions.Clear();

			if (m_Mainform != null)
			{
				m_Mainform.RemoveSubForm(this);
			}

			m_bClosed = true;

			Log(this.TitleString() + " - CLOSED");
		}

		#endregion

		#region 共通機能

		private bool m_bClosed = false;

		/// <summary>
		/// アプリケーション終了操作を無効にする
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				if (!isExitableWindow)
				{
					cp.ClassStyle = cp.ClassStyle | 0x200;
				}

				return cp;
			}
		}

		/// <summary>
		/// メインフォームへの参照を保持します。
		/// </summary>
		public BL_MainForm_Base m_Mainform = null;

		/// <summary>
		/// 
		/// </summary>
		public BL_SeparatedFunctions m_Functions = null;

		/// <summary>
		/// 経過時間を計測します。
		/// </summary>
		protected Stopwatch m_swElapsed = new Stopwatch();

		/// <summary>
		/// フォームのロードをキャンセルして閉じます。
		/// フォームロードのタイミングでフォームを閉じたい時に、
		/// trueに設定することでフォームを開かずに閉じることができます。
		/// </summary>
		protected bool m_bLoadCancel = false;

		/// <summary>
		/// Load処理が行われたかどうかをあらわします
		/// </summary>
		public bool m_bLoaded = false;

		/// <summary>
		/// 連続キー入力制御フラグ
		/// </summary>
		public bool key_input_accept = true;

		/// <summary>
		/// 
		/// </summary>
		public Dictionary<int, object> dictFunctions = new Dictionary<int, object>();

		/// <summary>
		/// メインフォームのタイトル文字列を取得・設定します。
		/// </summary>
		public string TitleText
		{
			get { return this.Text; }
			set
			{
				this.Text = value;
				if (m_Mainform != null) if (this.Text != "") m_Mainform.labelTitle.Text = this.Text;
			}
		}

		///// <summary>
		///// 画面の拡大縮小機能の有無を取得・設定します。
		///// </summary>
		//public bool EnableResize { get; set; }

		private bool enableSeparation = false;

		/// <summary>
		/// サブフォームをメインフォームから切り離す操作を有効にするかどうかを取得または設定します
		/// </summary>
		public bool EnableSeparation { get { return enableSeparation; } set { enableSeparation = value; } }

		/// <summary>
		/// 終了(ウィンドウ右上の×ボタン)を有効するか否かを保持します
		/// </summary>
		public bool isExitableWindow = false;

		/// <summary>
		/// 終了(ウィンドウ右上の×ボタン)を有効するか否かを取得および設定します
		/// </summary>
		public bool IsExitableWindow { get { return isExitableWindow; } set { isExitableWindow = value; } }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BL_SubForm_Base()
		{
			InitializeComponent();
			DoubleBuffered = true;
		}

		#region 子画面表示

		delegate void ShowMe_Delegate(BL_MainForm_Base main);

		/// <summary>
		/// 子画面表示を行います。
		/// 子画面が閉じられるまで、他のフォームの動作は原則として停止します。
		/// </summary>
		/// <param name="main"></param>
		virtual public void ShowMeModal(BL_MainForm_Base main)
		{
			if (InvokeRequired)
			{
				// 別スレッドから呼び出された場合
				Invoke(new ShowMe_Delegate(ShowMeModal), main);
				return;
			}

			m_bClosed = false;
			ShowMe(main);

			this.Select();

			while (!m_bClosed)
			{
				Application.DoEvents();
				System.Threading.Thread.Sleep(20);
			}
		}

		/// <summary>
		/// 子画面表示を行います。
		/// </summary>
		/// <param name="sub">既存のサブフォーム</param>
		virtual public void ShowMe(BL_SubForm_Base sub)
		{
			if (sub.m_Mainform != null)
			{
				if (0 < sub.m_Mainform.panelSubForm.Controls.Count)
				{
					if (sub.m_Mainform.panelSubForm.Controls[0] != sub) return;
				}
			}

			ShowMe(sub.m_Mainform);
		}

		/// <summary>
		/// 子画面表示を行います。
		/// </summary>
		/// <param name="sub">既存のサブフォーム</param>
		/// <param name="activated">バックグラウンドで動作させる場合、falseを指定します。</param>
		virtual public void ShowMe(BL_SubForm_Base sub, bool activated)
		{
			if (sub.m_Mainform != null)
			{
				if (0 < sub.m_Mainform.panelSubForm.Controls.Count)
				{
					if (sub.m_Mainform.panelSubForm.Controls[0] != sub) return;
				}
			}

			ShowMe(sub.m_Mainform, activated);
		}

		/// <summary>
		/// 子画面表示を行います。
		/// </summary>
		/// <param name="main">既存のメインフォーム</param>
		virtual public void ShowMe(BL_MainForm_Base main)
		{
			ShowMe(main, true);
		}

		/// <summary>
		/// 子画面の分離表示を行います。
		/// </summary>
		virtual public void ShowMe()
		{
			ShowMe((BL_MainForm_Base)null, true);
		}

		/// <summary>
		/// 子画面表示を行います。
		/// </summary>
		/// <param name="main">既存のメインフォーム</param>
		/// <param name="activated">バックグラウンドで動作させる場合、falseを指定します。</param>
		virtual public void ShowMe(BL_MainForm_Base main, bool activated)
		{
			BL_SubForm_Base target = this;

			int subcount = 0;
			if (main != null)
			{
				m_Mainform = (BL_MainForm_Base)main;
				this.TopLevel = false;
				subcount = m_Mainform.SubForms_Count;
				//target = m_Mainform.CheckSubForm(this);
				target = m_Mainform.AddSubForm(this, true);
				this.Size = m_Mainform.SubForm_Size;

				Log(this.TitleString() + " - SHOW", 2);

				subcount = m_Mainform.SubForms_Count;
			}

			try
			{
				if (target == this)
				{
					target.Show();
					target.SubForm_Base_Activated(main, new EventArgs());

					target.m_swElapsed.Reset();
					target.m_swElapsed.Start();
					target.m_bLoadCancel = false;
					target.m_bClosed = false;
					target.m_bLoaded = true;

					target.timerClock_Tick(this, new EventArgs());

					if (main == null)
					{
						BL_SeparatedFunctions func = new BL_SeparatedFunctions(target);
						func.Show(target);
					}
				}
			}
			catch (ObjectDisposedException)
			{
				m_bLoadCancel = true;
			}
		}

		#endregion

		/// <summary>
		/// ファンクションキー押し下げ時の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual public void btnFunctions_Click(object sender, EventArgs e)
		{
			if (typeof(Button).IsInstanceOfType(sender))
			{
				Button btn = (Button)sender;
				int num;
				int.TryParse(btn.Name.Substring(btn.Name.Length - 2), out num);

				foreach (KeyValuePair<int, object> kv in dictFunctions)
				{
					if (kv.Value == sender)
					{
						num = kv.Key;
						break;
					}
				}

				if (btn.Enabled)
				{
					{
						BelicsClass.UI.Controls.BL_FlatButton fb = btn as BelicsClass.UI.Controls.BL_FlatButton;
						if (fb != null)
						{
							if (fb.CheckMode)
							{
								m_Mainform.btnFunctions[num].Checked = fb.Checked;
							}
						}
					}

					switch (num)
					{
						case 0: SubForm_Base_FunctionESC_Clicked(sender);
							break;
						case 1: SubForm_Base_Function01_Clicked(sender);
							break;
						case 2: SubForm_Base_Function02_Clicked(sender);
							break;
						case 3: SubForm_Base_Function03_Clicked(sender);
							break;
						case 4: SubForm_Base_Function04_Clicked(sender);
							break;
						case 5: SubForm_Base_Function05_Clicked(sender);
							break;
						case 6: SubForm_Base_Function06_Clicked(sender);
							break;
						case 7: SubForm_Base_Function07_Clicked(sender);
							break;
						case 8: SubForm_Base_Function08_Clicked(sender);
							break;
						case 9: SubForm_Base_Function09_Clicked(sender);
							break;
						case 10: SubForm_Base_Function10_Clicked(sender);
							break;
						case 11: SubForm_Base_Function11_Clicked(sender);
							break;
						case 12: SubForm_Base_Function12_Clicked(sender);
							break;
						default: SubForm_Base_FunctionElse_Clicked(sender);
							break;
					}
				}
			}
		}


		/// <summary>
		/// ファンクションキー押し下げ時の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual public void btnFunctions_MouseDown(object sender, EventArgs e)
		{
			if (typeof(Button).IsInstanceOfType(sender))
			{
				Button btn = (Button)sender;
				int num;
				int.TryParse(btn.Name.Substring(btn.Name.Length - 2), out num);

				foreach (KeyValuePair<int, object> kv in dictFunctions)
				{
					if (kv.Value == sender)
					{
						num = kv.Key;
						break;
					}
				}

				if (btn.Enabled)
				{
					switch (num)
					{
						case 0:
							SubForm_Base_FunctionESC_MouseDown(sender);
							break;
						case 1:
							SubForm_Base_Function01_MouseDown(sender);
							break;
						case 2:
							SubForm_Base_Function02_MouseDown(sender);
							break;
						case 3:
							SubForm_Base_Function03_MouseDown(sender);
							break;
						case 4:
							SubForm_Base_Function04_MouseDown(sender);
							break;
						case 5:
							SubForm_Base_Function05_MouseDown(sender);
							break;
						case 6:
							SubForm_Base_Function06_MouseDown(sender);
							break;
						case 7:
							SubForm_Base_Function07_MouseDown(sender);
							break;
						case 8:
							SubForm_Base_Function08_MouseDown(sender);
							break;
						case 9:
							SubForm_Base_Function09_MouseDown(sender);
							break;
						case 10:
							SubForm_Base_Function10_MouseDown(sender);
							break;
						case 11:
							SubForm_Base_Function11_MouseDown(sender);
							break;
						case 12:
							SubForm_Base_Function12_MouseDown(sender);
							break;
						default:
							SubForm_Base_FunctionElse_MouseDown(sender);
							break;
					}
				}
			}
		}

		/// <summary>
		/// ファンクションキー押し下げ時の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual public void btnFunctions_MouseUp(object sender, EventArgs e)
		{
			if (typeof(Button).IsInstanceOfType(sender))
			{
				Button btn = (Button)sender;
				int num;
				int.TryParse(btn.Name.Substring(btn.Name.Length - 2), out num);

				foreach (KeyValuePair<int, object> kv in dictFunctions)
				{
					if (kv.Value == sender)
					{
						num = kv.Key;
						break;
					}
				}

				if (btn.Enabled)
				{
					switch (num)
					{
						case 0:
							SubForm_Base_FunctionESC_MouseUp(sender);
							break;
						case 1:
							SubForm_Base_Function01_MouseUp(sender);
							break;
						case 2:
							SubForm_Base_Function02_MouseUp(sender);
							break;
						case 3:
							SubForm_Base_Function03_MouseUp(sender);
							break;
						case 4:
							SubForm_Base_Function04_MouseUp(sender);
							break;
						case 5:
							SubForm_Base_Function05_MouseUp(sender);
							break;
						case 6:
							SubForm_Base_Function06_MouseUp(sender);
							break;
						case 7:
							SubForm_Base_Function07_MouseUp(sender);
							break;
						case 8:
							SubForm_Base_Function08_MouseUp(sender);
							break;
						case 9:
							SubForm_Base_Function09_MouseUp(sender);
							break;
						case 10:
							SubForm_Base_Function10_MouseUp(sender);
							break;
						case 11:
							SubForm_Base_Function11_MouseUp(sender);
							break;
						case 12:
							SubForm_Base_Function12_MouseUp(sender);
							break;
						default:
							SubForm_Base_FunctionElse_MouseUp(sender);
							break;
					}
				}
			}
		}

		/// <summary>
		/// 子フォーム上のボタンをファンクションに割り当てます。
		/// 任意のボタンとファンクション操作を共通化する際に呼び出します。
		/// </summary>
		/// <param name="button">ボタン</param>
		/// <param name="functionno">ファンクション番号 0:ESC/F1～F12</param>
		virtual public bool AttachButton_to_Functions(Button button, int functionno)
		{
			//対象ボタンの重複チェック
			foreach (var v in dictFunctions)
			{
				if (typeof(Button).IsInstanceOfType(v.Value))
				{
					if (v.Value == button)
					{
						return false;
					}
				}
			}


			Button mbutton = null;
			if (m_Mainform != null)
			{
				if (m_Mainform.IsTopLevelSubForm(this))
				{
					mbutton = m_Mainform.btnFunctions[functionno];
				}
			}

			button.Click += new EventHandler(btnFunctions_Click);

			//if (button.Tag != null)
			//{
			//    if (button.Tag.GetType() == typeof(bool))
			//    {
			//        if ((bool)button.Tag) mbutton.BackColor = button.BackColor;
			//        else mbutton.BackColor = Color.Blue;
			//    }
			//}

			if (mbutton != null)
			{
				{
					BelicsClass.UI.Controls.BL_FlatButton fb = mbutton as BelicsClass.UI.Controls.BL_FlatButton;
					BelicsClass.UI.Controls.BL_FlatButton b = button as BelicsClass.UI.Controls.BL_FlatButton;
					if (fb != null && b != null)
					{
						fb.CheckMode = b.CheckMode;
						fb.Checked = b.Checked;
					}
				}


				if (button.Text != "" && button.Enabled /*&& button.Visible*/)
				{
					mbutton.Enabled = button.Enabled = true;
					//button.BackColor = Color.FromArgb(255, button.BackColor.R, button.BackColor.G, button.BackColor.B);
					//mbutton.BackColor = Color.FromArgb(255, mbutton.BackColor.R, mbutton.BackColor.G, mbutton.BackColor.B);
					//mbutton.Text = button.Text.Replace("] ", "]\n");
					//mbutton.Text = mbutton.Text.Replace(":", "\n");
				}
				else
				{
					mbutton.Enabled = button.Enabled = false;
					//button.BackColor = Color.FromArgb(100, button.BackColor.R, button.BackColor.G, button.BackColor.B);
					//mbutton.BackColor = Color.FromArgb(100, mbutton.BackColor.R, mbutton.BackColor.G, mbutton.BackColor.B);
					//mbutton.Text = button.Text.Replace("] ", "]\n");
					//mbutton.Text = mbutton.Text.Replace(":", "\n");
				}
			}

			if (dictFunctions.ContainsKey(functionno))
			{
				object obj = dictFunctions[functionno];
				if (typeof(Button).IsInstanceOfType(obj))
				{
					((Button)obj).Click -= new EventHandler(btnFunctions_Click);
				}
			}
			dictFunctions[functionno] = button;

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="functionno"></param>
		/// <param name="enabled"></param>
		public void Function_Enabled(int functionno, bool enabled)
		{
			if (m_Mainform == null) return;
			if (m_Functions != null)
			{
				m_Functions.Function_Enabled(functionno, enabled);
			}
			else
			{
				m_Mainform.Function_Enabled(functionno, enabled);
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void SubForm_Base_Enter(object sender, EventArgs e)
		{
			Log(this.TitleString() + " - ENTER");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void SubForm_Base_Leave(object sender, EventArgs e)
		{
			Log(this.TitleString() + " - LEAVE");
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		/// <summary>
		/// ウィンドウの位置とサイズを変更する
		/// </summary>
		/// <param name="rect">変更後のウィンドウの位置とサイズ</param>
		public void SetWindowBounds(Rectangle rect)
		{
			if (this.Location.X != rect.Location.X || this.Location.Y != rect.Location.Y || this.Size.Width != rect.Size.Width || this.Size.Height != rect.Size.Height)
			{
				//MaximumSizeを大きくしておく
				if (this.MaximumSize.Width < rect.Width) this.MaximumSize = new Size(rect.Width, this.MaximumSize.Height);
				if (this.MaximumSize.Height < rect.Height) this.MaximumSize = new Size(this.MaximumSize.Width, rect.Height);

				MoveWindow(this.Handle, rect.X, rect.Y, rect.Width, rect.Height, true);
				this.UpdateBounds();
			}
		}

		/// <summary>
		/// サブフォームの自動スケーリング機能を開始します
		/// </summary>
		public void Resizer_Initialize()
		{
			Resizer_Initialize(true, true);
		}

		/// <summary>
		/// サブフォームの自動スケーリング機能を開始します
		/// </summary>
		public void Resizer_Initialize(bool horizontal, bool virtical)
		{
			Resizer_Finalize();
			ViewResizer = new BL_MainForm_Base.ViewResize(this, horizontal, virtical);
		}

		/// <summary>
		/// サブフォームの自動スケーリング機能を終了します
		/// スケーリング状態は現在のままで終了されます
		/// </summary>
		public void Resizer_Finalize()
		{
			if (ViewResizer != null)
			{
				ViewResizer.Clear();
				ViewResizer = null;
			}
		}

		/// <summary></summary>
		public double Resizer_LastScaleWidth { get { return ViewResizer.lastScaleWidth; } }

		/// <summary></summary>
		public double Resizer_LastScaleHeight { get { return ViewResizer.lastScaleHeight; } }

		/// <summary>
		/// リサイズ中
		/// </summary>
		public bool isResizing = false;

		/// <summary>
		/// サブフォームのサイズが変更された時に呼び出されます。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void SubForm_Base_Resize(object sender, EventArgs e)
		{
			isResizing = true;

			if (m_Functions != null)
			{
				m_Functions.Left = Left;
				m_Functions.Top = Top + Height;
				m_Functions.Width = Width;
			}

			//if (ModifierKeys == Keys.LButton) return;

			if (m_Mainform != null && m_Functions == null)
			{
				if (m_Mainform.ActiveSubForm == this)
				{
					if (ViewResizer != null) ViewResizer.Resize(this);
				}
			}
			else
			{
				if (ViewResizer != null) ViewResizer.Resize(this);
			}

			isResizing = false;
		}

		/// <summary>
		/// アプリケーションを終了します。
		/// </summary>
		virtual public void ExitApplication()
		{
			if (m_Mainform != null)
			{
				m_Mainform.ExitApplication();
			}
		}

		/// <summary>
		/// アプリケーションを終了します。
		/// </summary>
		virtual public void ExitApplication(string message)
		{
			if (m_Mainform != null)
			{
				m_Mainform.ExitApplication(message);
			}
		}

		/// <summary>
		/// アプリケーションを終了します。
		/// パスワード確認機能付き
		/// </summary>
		/// <param name="message"></param>
		/// <param name="password"></param>
		virtual public void ExitApplication(string message, string password)
		{
			if (m_Mainform != null)
			{
				m_Mainform.ExitApplication(message, password);
			}
		}

		/// <summary>
		/// コントロール名を指定して対象コントロールを取得します
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		virtual public Control GetControlFromName(string name)
		{
			return GetControlFromName(this, name);
		}

		/// <summary>
		/// コントロール名を指定して対象コントロールを取得します
		/// </summary>
		/// <param name="source"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private Control GetControlFromName(Control source, string name)
		{
			Control found = null;
			if (source.Name == name) found = source;

			if (found == null)
			{
				if (source.HasChildren)
				{
					foreach (Control ctr in source.Controls)
					{
						found = GetControlFromName(ctr, name);
						if (found != null) break;
					}
				}
			}

			return found;
		}

		/// <summary>
		/// ログを追加します
		/// </summary>
		/// <param name="description"></param>
		public void Log(string description)
		{
			if (m_Mainform == null) return;
			m_Mainform.Log(description);
		}

		/// <summary>
		/// ログを追加します
		/// </summary>
		/// <param name="description"></param>
		/// <param name="level"></param>
		public void Log(string description, int level)
		{
			if (m_Mainform == null) return;
			m_Mainform.Log(description, level);
		}

		private void BL_SubForm_Base_Move(object sender, EventArgs e)
		{
			if (m_Functions != null)
			{
				m_Functions.Left = Left;
				m_Functions.Top = Top + Height;
			}
		}
	}
}
