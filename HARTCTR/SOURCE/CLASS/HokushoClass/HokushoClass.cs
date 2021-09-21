using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HokushoClass
{
	#region システム関連クラス
	/// <summary>
	/// システム関連クラス
	/// </summary>
	public class H_System
	{
		/// <summary>
		/// プロセスが終了するまで現在のスレッドの実行をブロックします。
		/// </summary>
		public class WaitForExit
		{
			private Thread	Sub_thread;
			private ProcessStartInfo	Mode;
		
			//====================================================================================================
			// コンストラクタ
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	string			fileName			ﾌｧｲﾙ名
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	なし
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	なし
			//
			//====================================================================================================
			/// <summary>
			/// プロセスが終了するまで現在のスレッドの実行をブロックします。
			/// </summary>
			/// <param name="fileName">プロセスを起動するときに使用するファイル名を指定します。</param>
			public WaitForExit(string fileName)
			{
				Sub_thread = new Thread(new ThreadStart(wait));
				Mode = new ProcessStartInfo(fileName);

				check();
			}
			/// <summary>
			/// プロセスが終了するまで現在のスレッドの実行をブロックします。
			/// </summary>
			/// <param name="fileName">プロセスを起動するときに使用するファイル名を指定します。</param>
			/// <param name="option">プロセスを起動するときに使用するコマンドライン引数を指定します。</param>
			public WaitForExit(string fileName, string option)
			{
				Sub_thread = new Thread(new ThreadStart(wait));
				Mode = new ProcessStartInfo(fileName);
				Mode.Arguments = option;

				check();
			}
			/// <summary>
			/// プロセスが終了するまで現在のスレッドの実行をブロックします。
			/// </summary>
			/// <param name="mode">プロセスを起動するときに使用する値のセットを指定します。</param>
			public WaitForExit(ProcessStartInfo	mode)
			{
				Sub_thread = new Thread(new ThreadStart(wait));
				Mode = mode;

				check();
			}
	
			//====================================================================================================
			// 終了待機（スレッド）
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	なし
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	なし
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	なし
			//
			//====================================================================================================
			private void wait()
			{
				Process.Start(Mode).WaitForExit();
			}
	
			//====================================================================================================
			// 終了確認
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	なし
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	なし
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	なし
			//
			//====================================================================================================
			private void check()
			{
				Sub_thread.Start();

				while (Sub_thread.IsAlive)
				{
					Application.DoEvents();

					Thread.Sleep(100);
				}
			}
		}

		/// <summary>
		/// プロセスの起動をチェックします。
		/// </summary>
		public class Only
		{
			private Mutex	Mutex_flag;
			
			//====================================================================================================
			// コンストラクタ
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	string			mutexName			ﾌﾗｸﾞ名
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	なし
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	なし
			//
			//====================================================================================================
			/// <summary>
			/// プロセスの起動をチェックします。
			/// </summary>
			/// <param name="mutexName">プロセスを判断するときに使用するユニーク名を指定します。</param>
			public Only(string mutexName)
			{
				Mutex_flag = new Mutex(false, mutexName);
			}

			//====================================================================================================
			// デストラクタ
			//====================================================================================================
			/// <summary>
			/// 
			/// </summary>
			~Only()
			{
				Mutex_flag.Close();
			}

			//====================================================================================================
			// 起動チェックプロパティ
			//====================================================================================================
			/// <summary>
			/// プロセスの起動を取得します。起動していない場合は true。それ以外の場合は false。 
			/// </summary>
			public bool IsOnly
			{
				get
				{
					return Mutex_flag.WaitOne(0, false);
				}
			}
		}

		/// <summary>
		/// ストップウォッチクラス
		/// </summary>
		public class StopWatch
		{
			private long s_time;
			private long e_time;
			private bool start;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public StopWatch()
			{
				Reset();
			}

			/// <summary>
			/// ストップウォッチをリセットします。
			/// </summary>
			public void Reset()
			{
				s_time = 0;
				e_time = 0;
				start = false;
			}

			/// <summary>
			/// ストップウォッチを開始します。
			/// </summary>
			public void Start()
			{
				if (s_time == 0)
				{
					s_time = Win32.TickCount64;
					e_time = s_time;
				}

				start = true;
			}

			/// <summary>
			/// ストップウォッチを停止します。
			/// </summary>
			public void Stop()
			{
				if (e_time != 0)
				{
					e_time = Win32.TickCount64;
				}

				start = false;
			}

			/// <summary>
			/// 経過ミリ秒を取得します。
			/// </summary>
			public int ElapsedMilliseconds
			{
				get
				{
					long tick;

					if (start == true)
					{
						tick = Win32.TickCount64 - s_time;
					}
					else
					{
						tick = e_time - s_time;
					}

					if (tick > int.MaxValue)
					{
						tick = int.MaxValue;
					}

					return (int)tick;
				}
			}
		}

		/// <summary>
		/// ストップウォッチ６４クラス
		/// </summary>
		public class StopWatch64
		{
			#region フィールド

			private long s_time;
			private long keep_millseconds;
			private bool start;

			#endregion

			#region コンストラクタ

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public StopWatch64()
			{
				Reset();
			}

			#endregion

			#region メソッド

			/// <summary>
			/// ストップウォッチをリセットします。
			/// </summary>
			public void Reset()
			{
				s_time = 0;
				keep_millseconds = 0;
				start = false;
			}

			/// <summary>
			/// ストップウォッチを開始します。
			/// </summary>
			public void Start()
			{
				if (start == false)
				{
					s_time = Win32.TickCount64;
				}

				start = true;
			}

			/// <summary>
			/// ストップウォッチを停止します。
			/// </summary>
			public void Stop()
			{
				if (start == true)
				{
					keep_millseconds += (Win32.TickCount64 - s_time);
					s_time = 0;
				}

				start = false;
			}

			#endregion

			#region プロパティー

			/// <summary>
			/// 経過ミリ秒を取得します。
			/// </summary>
			public long ElapsedMilliseconds
			{
				get
				{
					if (start == true)
					{
						return keep_millseconds + (Win32.TickCount64 - s_time);
					}
					else
					{
						return keep_millseconds;
					}
				}
			}

			#endregion
		}
	}
	#endregion

	#region 拡張バイト配列クラス
	/// <summary>
	/// 拡張バイト配列クラス
	/// </summary>
	public class Bytes
	{
		//====================================================================================================
		// バイト配列の初期化
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte			sourceByte			初期化に使用するﾊﾞｲﾄ値
		//	int				length      		初期化するﾊﾞｲﾄ数
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	byte[]			destinationBytes	初期化するﾊﾞｲﾄ配列
		//	int				destinationIndex	初期化するﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	なし
		//
		//====================================================================================================
		/// <summary>
		/// バイト配列を初期化します。
		/// </summary>
		/// <param name="sourceByte">初期化に使用するバイト値。</param>
		/// <param name="destinationBytes">初期化するバイト配列。</param>
		/// <param name="length">初期化するバイト数。</param>
		public static void Clear(byte sourceByte, byte[] destinationBytes, int length)
		{
			clear(sourceByte, destinationBytes, 0, length);
		}		
		/// <summary>
		/// バイト配列を初期化します。
		/// </summary>
		/// <param name="sourceByte">初期化に使用するバイト値。</param>
		/// <param name="destinationBytes">初期化するバイト配列。</param>
		/// <param name="destinationIndex">destinationBytes内の初期化を開始するインデックス。</param>
		/// <param name="length">初期化するバイト数。</param>
		public static void Clear(byte sourceByte, byte[] destinationBytes, int destinationIndex, int length)
		{
			clear(sourceByte, destinationBytes, destinationIndex, length);
		}
		private static void clear(byte data1, byte[] data2, int index, int length)
		{
			int		count;
			
			for (count = 0 ; count < length; count++)
			{
				data2[index +count] = data1;
			}
		}

		//====================================================================================================
		// バイト配列のコピー
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			ｺﾋﾟｰ元のﾊﾞｲﾄ配列
		//	int				sourceIndex			ｺﾋﾟｰ元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//	int				length      		ｺﾋﾟｰするﾊﾞｲﾄ数
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	byte[]			destinationBytes	ｺﾋﾟｰ先のﾊﾞｲﾄ配列
		//	int				destinationIndex	ｺﾋﾟｰ先のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	なし
		//
		//====================================================================================================
		/// <summary>
		/// バイト配列をコピーします。
		/// </summary>
		/// <param name="sourceBytes">コピー元のバイト配列。</param>
		/// <param name="destinationBytes">コピー先のバイト配列。</param>
		/// <param name="length">コピーするバイト数。</param>
		public static void Copy(byte[] sourceBytes, byte[] destinationBytes, int length)
		{
			copy(sourceBytes, 0, destinationBytes, 0, length);
		}
		/// <summary>
		/// バイト配列をコピーします。
		/// </summary>
		/// <param name="sourceBytes">コピー元のバイト配列。</param>
		/// <param name="sourceIndex">sourceBytes内のコピーを開始するインデックス。</param>
		/// <param name="destinationBytes">コピー先のバイト配列。</param>
		/// <param name="destinationIndex">destinationBytes内のコピーを開始するインデックス。</param>
		/// <param name="length">コピーするバイト数。</param>
		public static void Copy(byte[] sourceBytes, int sourceIndex, byte[] destinationBytes, int destinationIndex, int length)
		{
			copy(sourceBytes, sourceIndex, destinationBytes, destinationIndex, length);
		}
		private static void copy(byte[] data1, int index1, byte[] data2, int index2, int length)
		{
			Array.Copy(data1, index1, data2, index2, length);
		}

		//====================================================================================================
		// バイト配列の比較
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			比較元のﾊﾞｲﾄ配列
		//	int				sourceIndex			比較元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//	byte[]			destinationBytes	比較先のﾊﾞｲﾄ配列
		//	int				destinationIndex	比較先のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//	int				length      		比較するﾊﾞｲﾄ数
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int				0					sourceBytes = destinationBytes
		//					1					sourceBytes > destinationBytes
		//					-1					sourceBytes < destinationBytes
		//
		//====================================================================================================
		/// <summary>
		/// バイト配列を比較します。
		/// </summary>
		/// <param name="sourceBytes">比較元のバイト配列。</param>
		/// <param name="destinationBytes">比較先のバイト配列。</param>
		/// <param name="length">比較するバイト数。</param>
		public static int Compare(byte[] sourceBytes, byte[] destinationBytes, int length)
		{
			return compare(sourceBytes, 0, destinationBytes, 0, length);
		}
		/// <summary>
		/// バイト配列を比較します。
		/// </summary>
		/// <param name="sourceBytes">比較元のバイト配列。</param>
		/// <param name="sourceIndex">sourceBytes内の比較を開始するインデックス。</param>
		/// <param name="destinationBytes">比較先のバイト配列。</param>
		/// <param name="destinationIndex">destinationBytes内の比較を開始するインデックス。</param>
		/// <param name="length">比較するバイト数。</param>
		public static int Compare(byte[] sourceBytes, int sourceIndex, byte[] destinationBytes, int destinationIndex, int length)
		{
			return compare(sourceBytes, sourceIndex, destinationBytes, destinationIndex, length);
		}
		private static int compare(byte[] data1, int index1, byte[] data2, int index2, int length)
		{
			int		status = 0, count;
			
			for (count = 0 ; count < length; count++)
			{
				if (data1[index1 +count] > data2[index2 +count])
				{
					status = 1;
					break;
				}
				else if (data1[index1 +count] < data2[index2 +count])
				{
					status = -1;
					break;
				}
			}

			return status;
		}

		//====================================================================================================
		// バイト配列の結合
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			結合元のﾊﾞｲﾄ配列
		//	int				sourceIndex			結合元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//	int				length      		結合するﾊﾞｲﾄ数
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	ref byte[]		destinationBytes	結合先のﾊﾞｲﾄ配列
		//	int				destinationIndex	結合先のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	なし
		//
		//====================================================================================================
		/// <summary>
		/// バイト配列を結合します。
		/// </summary>
		/// <param name="sourceBytes">結合元のバイト配列。</param>
		/// <param name="destinationBytes">結合先のバイト配列。</param>
		/// <param name="length">結合するバイト数。</param>
		public static void Join(byte[] sourceBytes, ref byte[] destinationBytes, int length)
		{
			join(sourceBytes, 0, ref destinationBytes, destinationBytes.Length, length);
		}
		/// <summary>
		/// バイト配列を結合します。
		/// </summary>
		/// <param name="sourceBytes">結合元のバイト配列。</param>
		/// <param name="sourceIndex">sourceBytes内の結合を開始するインデックス。</param>
		/// <param name="destinationBytes">結合先のバイト配列。</param>
		/// <param name="destinationIndex">destinationBytes内の結合を開始するインデックス。</param>
		/// <param name="length">結合するバイト数。</param>
		public static void Join(byte[] sourceBytes, int sourceIndex, ref byte[] destinationBytes, int destinationIndex, int length)
		{
			join(sourceBytes, sourceIndex, ref destinationBytes, destinationIndex, length);
		}
		private static void join(byte[] data1, int index1, ref byte[] data2, int index2, int length)
		{
			byte[]	data;

			data = new byte[index2 +length];

			Array.Copy(data2, 0, data, 0, index2);
			Array.Copy(data1, index1, data, index2, length);

			data2 = data;
		}

		//====================================================================================================
		// バイト配列の調整
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			調整元のﾊﾞｲﾄ配列
		//	int				sourceIndex			調整元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//	int				length      		調整するﾊﾞｲﾄ数
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]								調整したﾊﾞｲﾄ配列
		//
		//====================================================================================================
		/// <summary>
		/// バイト配列を調整します。
		/// </summary>
		/// <param name="sourceBytes">調整元のバイト配列。</param>
		/// <param name="length">調整するバイト数。</param>
		public static byte[] Trim(byte[] sourceBytes, int length)
		{
			return trim(sourceBytes, 0, length);
		}
		/// <summary>
		/// バイト配列を調整します。
		/// </summary>
		/// <param name="sourceBytes">調整元のバイト配列。</param>
		/// <param name="sourceIndex">sourceBytes内の調整を開始するインデックス。</param>
		/// <param name="length">調整するバイト数。</param>
		public static byte[] Trim(byte[] sourceBytes, int sourceIndex, int length)
		{
			return trim(sourceBytes, sourceIndex, length);
		}
		private static byte[] trim(byte[] data1, int index1, int length)
		{
			byte[]	data;

			data = new byte[length];

			Array.Copy(data1, index1, data, 0, length);

			return data;
		}

		//====================================================================================================
		// バイト配列へ変換
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			sourceString		変換元の文字列
		//	int				length      		変換後のﾊﾞｲﾄ数
		//	byte			sourceByte			埋め込みに使用するﾊﾞｲﾄ値
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]								変換したﾊﾞｲﾄ配列
		//
		//====================================================================================================
		/// <summary>
		/// バイト配列に変換して左寄せします。
		/// </summary>
		/// <param name="sourceString">調整元の文字列。</param>
		/// <param name="length">変換後のバイト数。</param>
		public static byte[] PadRight(string sourceString, int length)
		{
			return pad(1, sourceString, length, (byte)' ');
		}
		/// <summary>
		/// バイト配列に変換して左寄せします。
		/// </summary>
		/// <param name="sourceString">変換元の文字列。</param>
		/// <param name="length">変換後のバイト数。</param>
		/// <param name="sourceByte">埋め込みに使用するバイト値。</param>
		public static byte[] PadRight(string sourceString, int length, byte sourceByte)
		{
			return pad(1, sourceString, length, sourceByte);
		}
		/// <summary>
		/// バイト配列に変換して右寄せします。
		/// </summary>
		/// <param name="sourceString">調整元の文字列。</param>
		/// <param name="length">変換後のバイト数。</param>
		public static byte[] PadLeft(string sourceString, int length)
		{
			return pad(2, sourceString, length, (byte)' ');
		}
		/// <summary>
		/// バイト配列に変換して右寄せします。
		/// </summary>
		/// <param name="sourceString">変換元の文字列。</param>
		/// <param name="length">変換後のバイト数。</param>
		/// <param name="sourceByte">埋め込みに使用するバイト値。</param>
		public static byte[] PadLeft(string sourceString, int length, byte sourceByte)
		{
			return pad(2, sourceString, length, sourceByte);
		}
		private static byte[] pad(int mode, string data1, int length, byte data2)
		{
			int		size;
			byte[]	data, temp;

			data = new byte[length];

			clear(data2, data, 0, length);

			temp = Encoding.Default.GetBytes(data1);
			size = temp.Length < length ? temp.Length : length;

			if (mode == 1)
			{
				Array.Copy(temp, 0, data, 0, size);
			}
			else
			{
				Array.Copy(temp, 0, data, length -size, size);
			}

			return data;
		}

		//====================================================================================================
		// バイト配列からバイト配列を検索
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			検索元のﾊﾞｲﾄ配列
		//	int				sourceIndex			検索元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//	byte[]			targetBytes			検索するﾊﾞｲﾄ配列
		//	int				destinationIndex	検索するﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
		//	int				length      		検索するﾊﾞｲﾄ数
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int				>=0					見つけた先頭のインデックス
		//					-1					見つからない
		//
		//====================================================================================================
		/// <summary>
		/// バイト配列からバイト配列を検索します。
		/// </summary>
		/// <param name="sourceBytes">検索対象となるバイト配列。</param>
		/// <param name="sourceIndex">sourceBytes内の検索を開始するインデックス。</param>
		/// <param name="targetBytes">検索するバイト配列。</param>
		/// <param name="targetIndex">targetBytes内の検索を開始するインデックス。</param>
		/// <param name="length">検索するバイト数。</param>
		/// <returns></returns>
		public static int IndexOf(byte[] sourceBytes, int sourceIndex, byte[] targetBytes, int targetIndex, int length)
		{
			int pos = -1;
			int pos1, pos2;

			if (length <= 0)	return pos;
			if (sourceIndex < 0 || targetIndex < 0)	return pos;
			if (sourceIndex >= sourceBytes.Length)	return pos;
			if (targetIndex >= targetBytes.Length)	return pos;
			if (targetIndex + length > targetBytes.Length)	return pos;

			for (pos1 = sourceIndex; pos1 < sourceBytes.Length; pos1++)
			{
				if (sourceBytes.Length - pos1 < length)	break;

				for (pos2 = 0; pos2 < length; pos2++)
				{
					if (sourceBytes[pos1 + pos2] == targetBytes[targetIndex + pos2])	continue;
					else break;
				}

				if (pos2 == length)
				{
					pos = pos1;
					break;
				}
			}

			return pos;
		}

		//====================================================================================================
		// 型変換(byte[4] -> Int32)
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				変換元ﾃﾞｰﾀ
		//	int				Index				変換元ﾃﾞｰﾀの開始ｲﾝﾃﾞｯｸｽ
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int									結果
		//
		//====================================================================================================
		/// <summary>
		/// byte[4]をInt32へ変換
		/// </summary>
		/// <param name="data">変換するバイト配列。</param>
		public static int BytesToInt32(byte[] data)
		{
			return bytes_to_int32(data, 0);
		}
		/// <summary>
		/// byte[4]をInt32へ変換
		/// </summary>
		/// <param name="data">変換するバイト配列。</param>
		/// <param name="Index">data内の変換を開始するインデックス。</param>
		public static int BytesToInt32(byte[] data, int Index)
		{
			return bytes_to_int32(data, Index);
		}
		private static int bytes_to_int32(byte[] data, int index)
		{
			int		count = 0;
			
			count += data[index];
			count += data[index +1] <<8;
			count += data[index +2] <<16;
			count += data[index +3] <<24;

			return count;
		}

		//====================================================================================================
		// 型変換(Int32 -> byte[4])
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int									変換元ﾃﾞｰﾀ
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				結果
		//
		//====================================================================================================
		/// <summary>
		/// Int32をbyte[4]へ変換
		/// </summary>
		/// <param name="data">変換する値。</param>
		public static byte[] Int32ToBytes(int data)
		{
			byte[]	buff = new byte[4];
			
			buff[0] = (byte)(data &0xFF);
			buff[1] = (byte)((data >>8) &0xFF);
			buff[2] = (byte)((data >>16) &0xFF);
			buff[3] = (byte)((data >>24) &0xFF);

			return buff;
		}

		//====================================================================================================
		// 型変換(byte[2] -> Int16)
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				変換元ﾃﾞｰﾀ
		//	int				Index				変換元ﾃﾞｰﾀの開始ｲﾝﾃﾞｯｸｽ
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	short								結果
		//
		//====================================================================================================
		/// <summary>
		/// byte[2]をInt16へ変換
		/// </summary>
		/// <param name="data"></param>
		public static short BytesToInt16(byte[] data)
		{
			return bytes_to_int16(data, 0);
		}
		/// <summary>
		/// byte[2]をInt16へ変換
		/// </summary>
		/// <param name="data">変換するバイト配列。</param>
		/// <param name="Index">data内の変換を開始するインデックス。</param>
		public static short BytesToInt16(byte[] data, int Index)
		{
			return bytes_to_int16(data, Index);
		}
		private static short bytes_to_int16(byte[] data, int index)
		{
			short	count = 0;
			
			count += data[index];
			count += (short)(data[index +1] <<8);

			return count;
		}

		//====================================================================================================
		// 型変換(Int16 -> byte[2])
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	short								変換元ﾃﾞｰﾀ
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				結果
		//
		//====================================================================================================
		/// <summary>
		/// Int16をbyte[2]へ変換
		/// </summary>
		/// <param name="data">変換する値。</param>
		public static byte[] Int16ToBytes(short data)
		{
			byte[]	buff = new byte[2];
			
			buff[0] = (byte)(data &0xFF);
			buff[1] = (byte)((data >>8) &0xFF);

			return buff;
		}

		/// <summary>
		/// バイト配列の連続変換クラス
		/// </summary>
		public class Target　
		{
			private int		index = 0;
			private byte[]	data;

			//====================================================================================================
			// コンストラクタ
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	ref byte[]		base_data			対象のﾊﾞｲﾄ配列
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	なし
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	なし
			//
			//====================================================================================================
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="base_data">データを格納する(している)バイト配列。</param>
			public Target(ref byte[] base_data)
			{
				index = 0;
				data = base_data;
			}

			//====================================================================================================
			// バイト配列へ追加
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	object			source				追加するﾃﾞｰﾀ
			//	int				size				追加するﾊﾞｲﾄ数
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	なし
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	なし
			//
			//====================================================================================================
			/// <summary>
			/// バイト配列へデータを追加します。
			/// </summary>
			/// <param name="source">追加するデータが格納されている文字列。</param>
			/// <param name="size">追加するバイト数。</param>
			public void Add(string source, int size)
			{
				byte[]	temp;

				if (source != null)
				{
					temp = Encoding.Default.GetBytes(source);
			
					Bytes.Copy(temp, 0, data, index, temp.Length < size ? temp.Length : size);
				}

				index += size;
			}
			/// <summary>
			/// バイト配列へデータを追加します。
			/// </summary>
			/// <param name="source">追加するデータが格納されているバイト配列。</param>
			/// <param name="size">追加するバイト数。</param>
			public void Add(byte[] source, int size)
			{
				if (source != null)
				{
					Bytes.Copy(source, 0, data, index, source.Length < size ? source.Length : size);
				}

				index += size;
			}
			/// <summary>
			/// バイト配列へデータを追加します。
			/// </summary>
			/// <param name="source">追加するデータが格納されているInt32。</param>
			public void Add(int source)
			{
				byte[]	temp;

				temp = Bytes.Int32ToBytes(source);
			
				Bytes.Copy(temp, 0, data, index, Marshal.SizeOf(source));
			
				index += Marshal.SizeOf(source);
			}
			/// <summary>
			/// バイト配列へデータを追加します。
			/// </summary>
			/// <param name="source">追加するデータが格納されているInt16。</param>
			public void Add(short source)
			{
				byte[]	temp;

				temp = Bytes.Int16ToBytes(source);
			
				Bytes.Copy(temp, 0, data, index, Marshal.SizeOf(source));
			
				index += Marshal.SizeOf(source);
			}

			//====================================================================================================
			// バイト配列から抽出
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	int				size				抽出するﾊﾞｲﾄ数
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	ref object		source				抽出したﾃﾞｰﾀ
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	なし
			//
			//====================================================================================================
			/// <summary>
			/// バイト配列からデータを抽出します。
			/// </summary>
			/// <param name="source">抽出したデータを格納する文字列。</param>
			/// <param name="size">抽出するバイト数。</param>
			public void Split(ref string source, int size)
			{
				source = Encoding.Default.GetString(data, index, size);
			
				index += size;
			}
			/// <summary>
			/// バイト配列からデータを抽出します。
			/// </summary>
			/// <param name="source">抽出したデータを格納するバイト配列。</param>
			/// <param name="size">抽出するバイト数。</param>
			public void Split(ref byte[] source, int size)
			{
				Bytes.Copy(data, index, source, 0, size);
			
				index += size;
			}
			/// <summary>
			/// バイト配列からデータを抽出します。
			/// </summary>
			/// <param name="source">抽出したデータを格納するInt32。</param>
			public void Split(ref int source)
			{
				source = Bytes.BytesToInt32(data, index);
			
				index += Marshal.SizeOf(source);
			}
			/// <summary>
			/// バイト配列からデータを抽出します。
			/// </summary>
			/// <param name="source">抽出したデータを格納するInt16。</param>
			public void Split(ref short source)
			{
				source = Bytes.BytesToInt16(data, index);
			
				index += Marshal.SizeOf(source);
			}
		}
	}
	#endregion

	#region Win32APIクラス
	/// <summary>
	/// Win32APIクラス
	/// </summary>
	public sealed class Win32
	{
		private Win32()
		{
		}

		#region Win32API
		[DllImport("winmm", EntryPoint="PlaySound", SetLastError=true)]
		private static extern bool API_PlaySound(string szSound, IntPtr hMod, uint flags);

		[DllImport("winmm", EntryPoint="PlaySound", SetLastError=true)]
		private static extern bool API_PlaySound(byte[] szSound, IntPtr hMod, uint flags);

		[DllImport("winmm", EntryPoint="PlaySound", SetLastError=true)]
		private static extern bool API_PlaySound(IntPtr szSound, IntPtr hMod, uint flags);

		[DllImport("user32", EntryPoint="MessageBeep", SetLastError=true)]
		private static extern bool API_MessageBeep(uint uType);

		[DllImport("kernel32", EntryPoint="Beep", SetLastError=true)]
		private static extern bool API_Beep(uint frequency, uint duration);

		[DllImport("mpr", EntryPoint = "WNetAddConnection2", SetLastError = true)]
		private static extern uint API_WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, int dwFlags);

		[DllImport("mpr", EntryPoint = "WNetCancelConnection2", SetLastError = true)]
		private static extern uint API_WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool SetLocalTime(ref SYSTEMTIME systemtime);

		private const uint	SND_ASYNC = 0x00000001;
		private const uint	SND_FILENAME = 0x00020000;
		private const uint	SND_MEMORY = 0x00000004;
		private const uint	SND_LOOP = 0x00000008;

		private const uint RESOURCE_CONNECTED = 0x00000001;
		private const uint RESOURCE_GLOBALNET = 0x00000002;
		private const uint RESOURCE_REMEMBERED = 0x00000003;

		private const uint RESOURCETYPE_ANY = 0x00000000;
		private const uint RESOURCETYPE_DISK = 0x00000001;
		private const uint RESOURCETYPE_PRINT = 0x00000002;

		private const uint RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
		private const uint RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
		private const uint RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
		private const uint RESOURCEDISPLAYTYPE_SHARE = 0x00000003;

		private const uint RESOURCEUSAGE_CONNECTABLE = 0x00000001;
		private const uint RESOURCEUSAGE_CONTAINER = 0x00000002;

		private const uint CONNECT_UPDATE_PROFILE = 0x00000001;

		[StructLayout(LayoutKind.Sequential)]
		private struct NETRESOURCE
		{
			public uint dwScope;
			public uint dwType;
			public uint dwDisplayType;
			public uint dwUsage;
			public string lpLocalName;
			public string lpRemoteName;
			public string lpComment;
			public string lpProvider;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SYSTEMTIME
		{
			public ushort wYear;
			public ushort wMonth;
			public ushort wDayOfWeek;
			public ushort wDay;
			public ushort wHour;
			public ushort wMinute;
			public ushort wSecond;
			public ushort wMilliseconds;
		}
		#endregion

		/// <summary>
		/// サウンドを再生します。
		/// </summary>
		/// <param name="file_name">ファイル名を格納した文字列。</param>
		public static void PlaySound(string file_name)
		{
			API_PlaySound(file_name, IntPtr.Zero, SND_ASYNC | SND_FILENAME);
		}

		/// <summary>
		/// サウンドを再生します。
		/// </summary>
		/// <param name="stream">アセンブリ内の指定されたマニフェストリソース。</param>
		public static void PlaySound(Stream stream)
		{
			byte[]	byte_data = new byte[stream.Length];
			
			stream.Read(byte_data, 0, (int)stream.Length);
			
			API_PlaySound(byte_data, IntPtr.Zero, SND_ASYNC | SND_MEMORY);
		}

		/// <summary>
		/// サウンドを連続再生します。
		/// </summary>
		/// <param name="file_name">ファイル名を格納した文字列。</param>
		public static void PlaySoundEndless(string file_name)
		{
			API_PlaySound(file_name, IntPtr.Zero, SND_ASYNC | SND_FILENAME | SND_LOOP);
		}

		/// <summary>
		/// サウンドを連続再生します。
		/// </summary>
		/// <param name="stream">アセンブリ内の指定されたマニフェストリソース。</param>
		public static void PlaySoundEndless(Stream stream)
		{
			byte[]	byte_data = new byte[stream.Length];
			
			stream.Read(byte_data, 0, (int)stream.Length);
			
			API_PlaySound(byte_data, IntPtr.Zero, SND_ASYNC | SND_MEMORY | SND_LOOP);
		}

		/// <summary>
		/// サウンドを停止します。
		/// </summary>
		public static void PlaySoundStop()
		{
			API_PlaySound(IntPtr.Zero, IntPtr.Zero, 0);
		}

		/// <summary>
		/// ビープ音を鳴らします。
		/// </summary>
		public static void Beep()
		{
			API_MessageBeep(0xFFFFFFFF);
		}

		/// <summary>
		/// ビープ音を鳴らします。
		/// </summary>
		/// <param name="frequency">ビープ音の周波数。 (ヘルツ単位)</param>
		/// <param name="duration">ビープ音の周期。 (ミリ秒)</param>
		public static void Beep(int frequency, int duration)
		{
			API_Beep((uint)frequency, (uint)duration);
		}

		/// <summary>
		/// 指定したミリ秒数の間現在のスレッドをブロックします。
		/// </summary>
		/// <param name="millisecondsTimeout">スレッドがブロックされるミリ秒数を指定します。</param>
		public static void Sleep(int millisecondsTimeout)
		{
			Thread.Sleep(millisecondsTimeout);
		}

		/// <summary>
		/// システム起動後のミリ秒単位の経過時間を取得します。
		/// </summary>
		public static int TickCount
		{
			get
			{
				return System.Environment.TickCount;
			}
		}

		private static long Offset = 0;
		private static int LastTick = 0;

		/// <summary>
		/// システム起動後のミリ秒単位の経過時間を取得します。
		/// </summary>
		public static long TickCount64
		{
			get
			{
				int tick;

				tick = System.Environment.TickCount & int.MaxValue;

				if (LastTick > tick)
				{
					Offset++;
				}

				LastTick = tick;

				return tick + int.MaxValue * Offset;
			}
		}

		/// <summary>
		/// ネットワークドライブを割り当てます。
		/// </summary>
		/// <param name="LocalName">ドライブ名</param>
		/// <param name="RemoteName">ネットワーク名</param>
		/// <param name="Username">ユーザー名</param>
		/// <param name="Password">パスワード</param>
		public static uint NetAdd(string LocalName, string RemoteName, string Username, string Password)
		{
			NETRESOURCE net = new NETRESOURCE();

			net.dwScope = RESOURCE_GLOBALNET;
			net.dwType = RESOURCETYPE_DISK;
			net.dwDisplayType = RESOURCEDISPLAYTYPE_GENERIC;
			net.dwUsage = RESOURCEUSAGE_CONNECTABLE;
			net.lpLocalName = LocalName;
			net.lpRemoteName = RemoteName;
			net.lpComment = null;
			net.lpProvider = null;

			return API_WNetAddConnection2(ref net, Password, Username, 0);
		}

		/// <summary>
		/// ネットワークドライブを割り当てます。
		/// </summary>
		/// <param name="LocalName">ドライブ名</param>
		/// <param name="RemoteName">ネットワーク名</param>
		public static uint NetAdd(string LocalName, string RemoteName)
		{
			return NetAdd(LocalName, RemoteName, null, null);
		}

		/// <summary>
		/// ネットワークドライブを切断します。
		/// </summary>
		/// <param name="LocalName">ドライブ名</param>
		public static uint NetCancel(string LocalName)
		{
			return API_WNetCancelConnection2(LocalName, 0, false);
		}

		/// <summary>
		/// 現在のシステム日時を設定します。
		/// </summary>
		/// <param name="time">設定する日時</param>
		public static void SetLocalTime(DateTime time)
		{
			SYSTEMTIME now = new SYSTEMTIME();

			now.wYear = (ushort)time.Year;
			now.wMonth = (ushort)time.Month;
			now.wDay = (ushort)time.Day;
			now.wHour = (ushort)time.Hour;
			now.wMinute = (ushort)time.Minute;
			now.wSecond = (ushort)time.Second;
			now.wMilliseconds = (ushort)time.Millisecond;

			SetLocalTime(ref now);
		}
	}
	#endregion

	#region コンソール画面クラス

	/// <summary>
	/// コンソール画面クラス
	/// </summary>
	public sealed class ConsoleTools
	{
		private ConsoleTools()
		{
		}

		#region Win32API

		[DllImport("user32", SetLastError = true)]
		private static extern IntPtr GetSystemMenu(IntPtr handle, bool resetFlag);

		[DllImport("user32", SetLastError = true)]
		private static extern bool DeleteMenu(IntPtr handle, uint menuItem, uint menuFlag);

		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();

		[DllImport("kernel32.dll")]
		private static extern bool AttachConsole(uint dwProcessId);

		#endregion

		#region 列挙型

		/// <summary>
		/// 色属性
		/// </summary>
		[Flags]
		public enum RGB : short
		{
			/// <summary>
			/// 
			/// </summary>
			ForegroundBlue = 0x0001,
			/// <summary>
			/// 
			/// </summary>
			ForegroundGreen = 0x0002,
			/// <summary>
			/// 
			/// </summary>
			ForegroundRed = 0x0004,
			/// <summary>
			/// 
			/// </summary>
			BackgroundBlue = 0x0010,
			/// <summary>
			/// 
			/// </summary>
			BackgroundGreen = 0x0020,
			/// <summary>
			/// 
			/// </summary>
			BackgroundRed = 0x0040,
		}

		/// <summary>
		/// 色テーブル
		/// </summary>
		public enum Table
		{
			/// <summary>
			/// 
			/// </summary>
			Black = ConsoleColor.Black,
			/// <summary>
			/// 
			/// </summary>
			Blue = ConsoleColor.Blue,
			/// <summary>
			/// 
			/// </summary>
			Green = ConsoleColor.Green,
			/// <summary>
			/// 
			/// </summary>
			Cyan = ConsoleColor.Cyan,
			/// <summary>
			/// 
			/// </summary>
			Red = ConsoleColor.Red,
			/// <summary>
			/// 
			/// </summary>
			Magenta = ConsoleColor.Magenta,
			/// <summary>
			/// 
			/// </summary>
			Yellow = ConsoleColor.Yellow,
			/// <summary>
			/// 
			/// </summary>
			White = ConsoleColor.White,
			/// <summary>
			/// 
			/// </summary>
			BlueBack = ConsoleColor.DarkBlue,
			/// <summary>
			/// 
			/// </summary>
			GreenBack = ConsoleColor.DarkGreen,
			/// <summary>
			/// 
			/// </summary>
			CyanBack = ConsoleColor.DarkCyan,
			/// <summary>
			/// 
			/// </summary>
			RedBack = ConsoleColor.DarkRed,
			/// <summary>
			/// 
			/// </summary>
			MagentaBack = ConsoleColor.DarkMagenta,
			/// <summary>
			/// 
			/// </summary>
			YellowBack = ConsoleColor.DarkYellow,
			/// <summary>
			/// 
			/// </summary>
			WhiteBack = ConsoleColor.Gray,
		}

		#endregion

		/// <summary>
		/// 閉じるボタンを無効にします。
		/// </summary>
		public static void ButtonDisable()
		{
			const uint SC_SIZE = 0xF000;
			const uint SC_MAXIMIZE = 0xF030;
			const uint SC_CLOSE = 0xF060;
			const uint MF_BYCOMMAND = 0x00000000;

			Process process;
			IntPtr handle;

			process = Process.GetCurrentProcess();

			handle = GetSystemMenu(process.MainWindowHandle, false);
			DeleteMenu(handle, SC_CLOSE, MF_BYCOMMAND);
			DeleteMenu(handle, SC_MAXIMIZE, MF_BYCOMMAND);
			DeleteMenu(handle, SC_SIZE, MF_BYCOMMAND);
		}

		/// <summary>
		/// コンソール画面のタイトルを設定します。
		/// </summary>
		/// <param name="titleName">タイトルが格納されている文字列。</param>
		/// <returns></returns>
		public static bool SetTitle(string titleName)
		{
			Console.Title = titleName;

			return true;
		}

		/// <summary>
		/// コンソール画面のサイズを設定します。
		/// </summary>
		/// <param name="x">水平方向。</param>
		/// <param name="y">垂直方向。</param>
		public static void WindowSize(int x, int y)
		{
			Console.SetWindowSize(x, y);
			Console.SetBufferSize(x, y);
		}

		/// <summary>
		/// コンソール画面を初期化します。
		/// </summary>
		public static void ScreenClear()
		{
			Color();

			Console.Clear();
		}

		/// <summary>
		/// カーソルの表示／非表示を設定します。
		/// </summary>
		public static bool CursorVisible
		{
			set
			{
				Console.CursorVisible = value;
			}
		}

		/// <summary>
		/// カーソルの位置を設定します。
		/// </summary>
		/// <param name="x">水平座標。</param>
		/// <param name="y">垂直座標。</param>
		public static void Locate(int x, int y)
		{
			try
			{
				Console.CursorTop = y;
				Console.CursorLeft = x;
			}
			catch
			{
			}
		}

		/// <summary>
		/// 指定した位置に文字列を書き込みます。
		/// </summary>
		/// <param name="x">水平座標。</param>
		/// <param name="y">垂直座標。</param>
		/// <param name="data">書き込む文字列。</param>
		public static void LocateWrite(int x, int y, string data)
		{
			Locate(x, y);

			try
			{
				Console.Write(data);
			}
			catch
			{
				StreamWriter standard = new StreamWriter(Console.OpenStandardOutput(), Encoding.Default);

				standard.AutoFlush = true;

				Console.SetOut(standard);

				Console.Write(data);
			}
		}

		/// <summary>
		/// カーソルの色を設定します。
		/// </summary>
		/// <param name="rgb">色属性。</param>
		public static void Color(RGB rgb)
		{
			ConsoleColor color = ConsoleColor.Black;

			switch ((short)rgb)
			{
			case 0x0001:
				color = ConsoleColor.Blue;
				break;
			case 0x0002:
				color = ConsoleColor.Green;
				break;
			case 0x0003:
				color = ConsoleColor.Cyan;
				break;
			case 0x0004:
				color = ConsoleColor.Red;
				break;
			case 0x0005:
				color = ConsoleColor.Magenta;
				break;
			case 0x0006:
				color = ConsoleColor.Yellow;
				break;
			case 0x0007:
				color = ConsoleColor.White;
				break;
			case 0x0010:
				color = ConsoleColor.DarkBlue;
				break;
			case 0x0020:
				color = ConsoleColor.DarkGreen;
				break;
			case 0x0030:
				color = ConsoleColor.DarkCyan;
				break;
			case 0x0040:
				color = ConsoleColor.DarkRed;
				break;
			case 0x0050:
				color = ConsoleColor.DarkMagenta;
				break;
			case 0x0060:
				color = ConsoleColor.DarkYellow;
				break;
			case 0x0070:
				color = ConsoleColor.Gray;
				break;
			}

			Console.ForegroundColor = color;
		}
		/// <summary>
		/// カーソルの色を設定します。
		/// </summary>
		/// <param name="colorTable">色テーブル。</param>
		public static void Color(Table colorTable)
		{
			Console.ForegroundColor = (ConsoleColor)colorTable;
		}
		/// <summary>
		/// カーソルの色を設定します。
		/// </summary>
		/// <param name="foregroundColor">前景色</param>
		/// <param name="backgroundColor">背景色</param>
		public static void Color(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
		{
			Console.ForegroundColor = foregroundColor;
			Console.BackgroundColor = backgroundColor;
		}
		/// <summary>
		/// カーソルの色を設定します。
		/// </summary>
		public static void Color()
		{
			Color(ConsoleColor.White, ConsoleColor.Black);
		}

		/// <summary>
		/// コンソールの入力をチェックします。
		/// </summary>
		/// <returns></returns>
		public static int KeyDown()
		{
			int status = 0;

			if (Console.KeyAvailable)
			{
				status = (int)Console.ReadKey(true).KeyChar;
			}

			return status;
		}

		private static bool Created = false;

		/// <summary>
		/// コンソール画面が作成済みかどうかを取得します。
		/// </summary>
		public static bool IsCreated
		{
			get { return Created; }
		}

		/// <summary>
		/// コンソール画面を作成します。
		/// </summary>
		public static void CreateConsole()
		{
			FreeConsole();
			AllocConsole();

			StreamWriter standard = new StreamWriter(Console.OpenStandardOutput(), Encoding.Default);

			standard.AutoFlush = true;

			Console.SetOut(standard);

			Created = true;
		}

		/// <summary>
		/// コンソール画面を解放します。
		/// </summary>
		public static void ReleaseConsole()
		{
			FreeConsole();

			Created = false;
		}
	}

	#endregion

	#region 並び替えクラス(ListView)

	/// <summary>
	/// ListViewの項目の並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparer : IComparer
	{
		#region 列挙体

		/// <summary>
		/// 比較する方法
		/// </summary>
		public enum ComparerMode
		{
			/// <summary>
			/// 文字列として比較
			/// </summary>
			String,
			/// <summary>
			/// 数値（Int32型）として比較
			/// </summary>
			Integer,
			/// <summary>
			/// 日時（DataTime型）として比較
			/// </summary>
			DateTime,
		}

		#endregion

		private int _column;
		private SortOrder _order;
		private ComparerMode _mode;
		private ComparerMode[] _columnModes;

		#region プロパティ

		/// <summary>
		/// 並び替えるListView列の番号
		/// </summary>
		public int Column
		{
			set
			{
				//現在と同じ列の時は、昇順降順を切り替える
				if (_column == value)
				{
					if (_order == SortOrder.Ascending)
					{
						_order = SortOrder.Descending;
					}
					else if (_order == SortOrder.Descending)
					{
						_order = SortOrder.Ascending;
					}
				}
				_column = value;
			}
			get
			{
				return _column;
			}
		}

		/// <summary>
		/// 昇順か降順か
		/// </summary>
		public SortOrder Order
		{
			set
			{
				_order = value;
			}
			get
			{
				return _order;
			}
		}

		/// <summary>
		/// 並び替えの方法
		/// </summary>
		public ComparerMode Mode
		{
			set
			{
				_mode = value;
			}
			get
			{
				return _mode;
			}
		}

		/// <summary>
		/// 列ごとの並び替えの方法
		/// </summary>
		public ComparerMode[] ColumnModes
		{
			set
			{
				_columnModes = value;
			}
		}

		#endregion

		/// <summary>
		/// ListViewItemComparerクラスのコンストラクタ
		/// </summary>
		/// <param name="columnNo">並び替える列の番号</param>
		/// <param name="sortOrder">昇順か降順か</param>
		/// <param name="comparerMode">並び替えの方法</param>
		public ListViewItemComparer(int columnNo, SortOrder sortOrder, ComparerMode comparerMode)
		{
			_column = columnNo;
			_order = sortOrder;
			_mode = comparerMode;
		}
		/// <summary>
		/// ListViewItemComparerクラスのコンストラクタ
		/// </summary>
		public ListViewItemComparer()
		{
			_column = 0;
			_order = SortOrder.Ascending;
			_mode = ComparerMode.String;
		}

		/// <summary>
		/// 比較
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す</returns>
		public int Compare(object x, object y)
		{
			int status = 0;

			if (_order == SortOrder.None)
			{
				return status;
			}

			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			if (_columnModes != null && _columnModes.Length > _column)
			{
				_mode = _columnModes[_column];
			}

			try
			{
				//並び替えの方法別に、xとyを比較する
				switch (_mode)
				{
				case ComparerMode.String:
					status = string.Compare(itemx.SubItems[_column].Text, itemy.SubItems[_column].Text);
					break;

				case ComparerMode.Integer:
					status = int.Parse(itemx.SubItems[_column].Text).CompareTo(int.Parse(itemy.SubItems[_column].Text));
					break;

				case ComparerMode.DateTime:
					status = DateTime.Compare(DateTime.Parse(itemx.SubItems[_column].Text), DateTime.Parse(itemy.SubItems[_column].Text));
					break;
				}

				//降順の時は結果を+-逆にする
				if (_order == SortOrder.Descending)
				{
					status = -status;
				}
			}
			catch
			{
			}

			return status;
		}
	}

	#endregion
}
