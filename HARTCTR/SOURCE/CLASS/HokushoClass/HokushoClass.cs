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
	#region �V�X�e���֘A�N���X
	/// <summary>
	/// �V�X�e���֘A�N���X
	/// </summary>
	public class H_System
	{
		/// <summary>
		/// �v���Z�X���I������܂Ō��݂̃X���b�h�̎��s���u���b�N���܂��B
		/// </summary>
		public class WaitForExit
		{
			private Thread	Sub_thread;
			private ProcessStartInfo	Mode;
		
			//====================================================================================================
			// �R���X�g���N�^
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	string			fileName			̧�ٖ�
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//====================================================================================================
			/// <summary>
			/// �v���Z�X���I������܂Ō��݂̃X���b�h�̎��s���u���b�N���܂��B
			/// </summary>
			/// <param name="fileName">�v���Z�X���N������Ƃ��Ɏg�p����t�@�C�������w�肵�܂��B</param>
			public WaitForExit(string fileName)
			{
				Sub_thread = new Thread(new ThreadStart(wait));
				Mode = new ProcessStartInfo(fileName);

				check();
			}
			/// <summary>
			/// �v���Z�X���I������܂Ō��݂̃X���b�h�̎��s���u���b�N���܂��B
			/// </summary>
			/// <param name="fileName">�v���Z�X���N������Ƃ��Ɏg�p����t�@�C�������w�肵�܂��B</param>
			/// <param name="option">�v���Z�X���N������Ƃ��Ɏg�p����R�}���h���C���������w�肵�܂��B</param>
			public WaitForExit(string fileName, string option)
			{
				Sub_thread = new Thread(new ThreadStart(wait));
				Mode = new ProcessStartInfo(fileName);
				Mode.Arguments = option;

				check();
			}
			/// <summary>
			/// �v���Z�X���I������܂Ō��݂̃X���b�h�̎��s���u���b�N���܂��B
			/// </summary>
			/// <param name="mode">�v���Z�X���N������Ƃ��Ɏg�p����l�̃Z�b�g���w�肵�܂��B</param>
			public WaitForExit(ProcessStartInfo	mode)
			{
				Sub_thread = new Thread(new ThreadStart(wait));
				Mode = mode;

				check();
			}
	
			//====================================================================================================
			// �I���ҋ@�i�X���b�h�j
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//====================================================================================================
			private void wait()
			{
				Process.Start(Mode).WaitForExit();
			}
	
			//====================================================================================================
			// �I���m�F
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	�Ȃ�
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
		/// �v���Z�X�̋N�����`�F�b�N���܂��B
		/// </summary>
		public class Only
		{
			private Mutex	Mutex_flag;
			
			//====================================================================================================
			// �R���X�g���N�^
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	string			mutexName			�׸ޖ�
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//====================================================================================================
			/// <summary>
			/// �v���Z�X�̋N�����`�F�b�N���܂��B
			/// </summary>
			/// <param name="mutexName">�v���Z�X�𔻒f����Ƃ��Ɏg�p���郆�j�[�N�����w�肵�܂��B</param>
			public Only(string mutexName)
			{
				Mutex_flag = new Mutex(false, mutexName);
			}

			//====================================================================================================
			// �f�X�g���N�^
			//====================================================================================================
			/// <summary>
			/// 
			/// </summary>
			~Only()
			{
				Mutex_flag.Close();
			}

			//====================================================================================================
			// �N���`�F�b�N�v���p�e�B
			//====================================================================================================
			/// <summary>
			/// �v���Z�X�̋N�����擾���܂��B�N�����Ă��Ȃ��ꍇ�� true�B����ȊO�̏ꍇ�� false�B 
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
		/// �X�g�b�v�E�H�b�`�N���X
		/// </summary>
		public class StopWatch
		{
			private long s_time;
			private long e_time;
			private bool start;

			/// <summary>
			/// �R���X�g���N�^
			/// </summary>
			public StopWatch()
			{
				Reset();
			}

			/// <summary>
			/// �X�g�b�v�E�H�b�`�����Z�b�g���܂��B
			/// </summary>
			public void Reset()
			{
				s_time = 0;
				e_time = 0;
				start = false;
			}

			/// <summary>
			/// �X�g�b�v�E�H�b�`���J�n���܂��B
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
			/// �X�g�b�v�E�H�b�`���~���܂��B
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
			/// �o�߃~���b���擾���܂��B
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
		/// �X�g�b�v�E�H�b�`�U�S�N���X
		/// </summary>
		public class StopWatch64
		{
			#region �t�B�[���h

			private long s_time;
			private long keep_millseconds;
			private bool start;

			#endregion

			#region �R���X�g���N�^

			/// <summary>
			/// �R���X�g���N�^
			/// </summary>
			public StopWatch64()
			{
				Reset();
			}

			#endregion

			#region ���\�b�h

			/// <summary>
			/// �X�g�b�v�E�H�b�`�����Z�b�g���܂��B
			/// </summary>
			public void Reset()
			{
				s_time = 0;
				keep_millseconds = 0;
				start = false;
			}

			/// <summary>
			/// �X�g�b�v�E�H�b�`���J�n���܂��B
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
			/// �X�g�b�v�E�H�b�`���~���܂��B
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

			#region �v���p�e�B�[

			/// <summary>
			/// �o�߃~���b���擾���܂��B
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

	#region �g���o�C�g�z��N���X
	/// <summary>
	/// �g���o�C�g�z��N���X
	/// </summary>
	public class Bytes
	{
		//====================================================================================================
		// �o�C�g�z��̏�����
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte			sourceByte			�������Ɏg�p�����޲Ēl
		//	int				length      		�����������޲Đ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	byte[]			destinationBytes	�����������޲Ĕz��
		//	int				destinationIndex	�����������޲Ĕz��̊J�n���ޯ��
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �o�C�g�z������������܂��B
		/// </summary>
		/// <param name="sourceByte">�������Ɏg�p����o�C�g�l�B</param>
		/// <param name="destinationBytes">����������o�C�g�z��B</param>
		/// <param name="length">����������o�C�g���B</param>
		public static void Clear(byte sourceByte, byte[] destinationBytes, int length)
		{
			clear(sourceByte, destinationBytes, 0, length);
		}		
		/// <summary>
		/// �o�C�g�z������������܂��B
		/// </summary>
		/// <param name="sourceByte">�������Ɏg�p����o�C�g�l�B</param>
		/// <param name="destinationBytes">����������o�C�g�z��B</param>
		/// <param name="destinationIndex">destinationBytes���̏��������J�n����C���f�b�N�X�B</param>
		/// <param name="length">����������o�C�g���B</param>
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
		// �o�C�g�z��̃R�s�[
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			��߰�����޲Ĕz��
		//	int				sourceIndex			��߰�����޲Ĕz��̊J�n���ޯ��
		//	int				length      		��߰�����޲Đ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	byte[]			destinationBytes	��߰����޲Ĕz��
		//	int				destinationIndex	��߰����޲Ĕz��̊J�n���ޯ��
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �o�C�g�z����R�s�[���܂��B
		/// </summary>
		/// <param name="sourceBytes">�R�s�[���̃o�C�g�z��B</param>
		/// <param name="destinationBytes">�R�s�[��̃o�C�g�z��B</param>
		/// <param name="length">�R�s�[����o�C�g���B</param>
		public static void Copy(byte[] sourceBytes, byte[] destinationBytes, int length)
		{
			copy(sourceBytes, 0, destinationBytes, 0, length);
		}
		/// <summary>
		/// �o�C�g�z����R�s�[���܂��B
		/// </summary>
		/// <param name="sourceBytes">�R�s�[���̃o�C�g�z��B</param>
		/// <param name="sourceIndex">sourceBytes���̃R�s�[���J�n����C���f�b�N�X�B</param>
		/// <param name="destinationBytes">�R�s�[��̃o�C�g�z��B</param>
		/// <param name="destinationIndex">destinationBytes���̃R�s�[���J�n����C���f�b�N�X�B</param>
		/// <param name="length">�R�s�[����o�C�g���B</param>
		public static void Copy(byte[] sourceBytes, int sourceIndex, byte[] destinationBytes, int destinationIndex, int length)
		{
			copy(sourceBytes, sourceIndex, destinationBytes, destinationIndex, length);
		}
		private static void copy(byte[] data1, int index1, byte[] data2, int index2, int length)
		{
			Array.Copy(data1, index1, data2, index2, length);
		}

		//====================================================================================================
		// �o�C�g�z��̔�r
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			��r�����޲Ĕz��
		//	int				sourceIndex			��r�����޲Ĕz��̊J�n���ޯ��
		//	byte[]			destinationBytes	��r����޲Ĕz��
		//	int				destinationIndex	��r����޲Ĕz��̊J�n���ޯ��
		//	int				length      		��r�����޲Đ�
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
		/// �o�C�g�z����r���܂��B
		/// </summary>
		/// <param name="sourceBytes">��r���̃o�C�g�z��B</param>
		/// <param name="destinationBytes">��r��̃o�C�g�z��B</param>
		/// <param name="length">��r����o�C�g���B</param>
		public static int Compare(byte[] sourceBytes, byte[] destinationBytes, int length)
		{
			return compare(sourceBytes, 0, destinationBytes, 0, length);
		}
		/// <summary>
		/// �o�C�g�z����r���܂��B
		/// </summary>
		/// <param name="sourceBytes">��r���̃o�C�g�z��B</param>
		/// <param name="sourceIndex">sourceBytes���̔�r���J�n����C���f�b�N�X�B</param>
		/// <param name="destinationBytes">��r��̃o�C�g�z��B</param>
		/// <param name="destinationIndex">destinationBytes���̔�r���J�n����C���f�b�N�X�B</param>
		/// <param name="length">��r����o�C�g���B</param>
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
		// �o�C�g�z��̌���
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			���������޲Ĕz��
		//	int				sourceIndex			���������޲Ĕz��̊J�n���ޯ��
		//	int				length      		���������޲Đ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	ref byte[]		destinationBytes	��������޲Ĕz��
		//	int				destinationIndex	��������޲Ĕz��̊J�n���ޯ��
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �o�C�g�z����������܂��B
		/// </summary>
		/// <param name="sourceBytes">�������̃o�C�g�z��B</param>
		/// <param name="destinationBytes">������̃o�C�g�z��B</param>
		/// <param name="length">��������o�C�g���B</param>
		public static void Join(byte[] sourceBytes, ref byte[] destinationBytes, int length)
		{
			join(sourceBytes, 0, ref destinationBytes, destinationBytes.Length, length);
		}
		/// <summary>
		/// �o�C�g�z����������܂��B
		/// </summary>
		/// <param name="sourceBytes">�������̃o�C�g�z��B</param>
		/// <param name="sourceIndex">sourceBytes���̌������J�n����C���f�b�N�X�B</param>
		/// <param name="destinationBytes">������̃o�C�g�z��B</param>
		/// <param name="destinationIndex">destinationBytes���̌������J�n����C���f�b�N�X�B</param>
		/// <param name="length">��������o�C�g���B</param>
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
		// �o�C�g�z��̒���
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			���������޲Ĕz��
		//	int				sourceIndex			���������޲Ĕz��̊J�n���ޯ��
		//	int				length      		���������޲Đ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]								���������޲Ĕz��
		//
		//====================================================================================================
		/// <summary>
		/// �o�C�g�z��𒲐����܂��B
		/// </summary>
		/// <param name="sourceBytes">�������̃o�C�g�z��B</param>
		/// <param name="length">��������o�C�g���B</param>
		public static byte[] Trim(byte[] sourceBytes, int length)
		{
			return trim(sourceBytes, 0, length);
		}
		/// <summary>
		/// �o�C�g�z��𒲐����܂��B
		/// </summary>
		/// <param name="sourceBytes">�������̃o�C�g�z��B</param>
		/// <param name="sourceIndex">sourceBytes���̒������J�n����C���f�b�N�X�B</param>
		/// <param name="length">��������o�C�g���B</param>
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
		// �o�C�g�z��֕ϊ�
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			sourceString		�ϊ����̕�����
		//	int				length      		�ϊ�����޲Đ�
		//	byte			sourceByte			���ߍ��݂Ɏg�p�����޲Ēl
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]								�ϊ������޲Ĕz��
		//
		//====================================================================================================
		/// <summary>
		/// �o�C�g�z��ɕϊ����č��񂹂��܂��B
		/// </summary>
		/// <param name="sourceString">�������̕�����B</param>
		/// <param name="length">�ϊ���̃o�C�g���B</param>
		public static byte[] PadRight(string sourceString, int length)
		{
			return pad(1, sourceString, length, (byte)' ');
		}
		/// <summary>
		/// �o�C�g�z��ɕϊ����č��񂹂��܂��B
		/// </summary>
		/// <param name="sourceString">�ϊ����̕�����B</param>
		/// <param name="length">�ϊ���̃o�C�g���B</param>
		/// <param name="sourceByte">���ߍ��݂Ɏg�p����o�C�g�l�B</param>
		public static byte[] PadRight(string sourceString, int length, byte sourceByte)
		{
			return pad(1, sourceString, length, sourceByte);
		}
		/// <summary>
		/// �o�C�g�z��ɕϊ����ĉE�񂹂��܂��B
		/// </summary>
		/// <param name="sourceString">�������̕�����B</param>
		/// <param name="length">�ϊ���̃o�C�g���B</param>
		public static byte[] PadLeft(string sourceString, int length)
		{
			return pad(2, sourceString, length, (byte)' ');
		}
		/// <summary>
		/// �o�C�g�z��ɕϊ����ĉE�񂹂��܂��B
		/// </summary>
		/// <param name="sourceString">�ϊ����̕�����B</param>
		/// <param name="length">�ϊ���̃o�C�g���B</param>
		/// <param name="sourceByte">���ߍ��݂Ɏg�p����o�C�g�l�B</param>
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
		// �o�C�g�z�񂩂�o�C�g�z�������
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			sourceBytes			���������޲Ĕz��
		//	int				sourceIndex			���������޲Ĕz��̊J�n���ޯ��
		//	byte[]			targetBytes			���������޲Ĕz��
		//	int				destinationIndex	���������޲Ĕz��̊J�n���ޯ��
		//	int				length      		���������޲Đ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int				>=0					�������擪�̃C���f�b�N�X
		//					-1					������Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �o�C�g�z�񂩂�o�C�g�z����������܂��B
		/// </summary>
		/// <param name="sourceBytes">�����ΏۂƂȂ�o�C�g�z��B</param>
		/// <param name="sourceIndex">sourceBytes���̌������J�n����C���f�b�N�X�B</param>
		/// <param name="targetBytes">��������o�C�g�z��B</param>
		/// <param name="targetIndex">targetBytes���̌������J�n����C���f�b�N�X�B</param>
		/// <param name="length">��������o�C�g���B</param>
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
		// �^�ϊ�(byte[4] -> Int32)
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				�ϊ����ް�
		//	int				Index				�ϊ����ް��̊J�n���ޯ��
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	int									����
		//
		//====================================================================================================
		/// <summary>
		/// byte[4]��Int32�֕ϊ�
		/// </summary>
		/// <param name="data">�ϊ�����o�C�g�z��B</param>
		public static int BytesToInt32(byte[] data)
		{
			return bytes_to_int32(data, 0);
		}
		/// <summary>
		/// byte[4]��Int32�֕ϊ�
		/// </summary>
		/// <param name="data">�ϊ�����o�C�g�z��B</param>
		/// <param name="Index">data���̕ϊ����J�n����C���f�b�N�X�B</param>
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
		// �^�ϊ�(Int32 -> byte[4])
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int									�ϊ����ް�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				����
		//
		//====================================================================================================
		/// <summary>
		/// Int32��byte[4]�֕ϊ�
		/// </summary>
		/// <param name="data">�ϊ�����l�B</param>
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
		// �^�ϊ�(byte[2] -> Int16)
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				�ϊ����ް�
		//	int				Index				�ϊ����ް��̊J�n���ޯ��
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	short								����
		//
		//====================================================================================================
		/// <summary>
		/// byte[2]��Int16�֕ϊ�
		/// </summary>
		/// <param name="data"></param>
		public static short BytesToInt16(byte[] data)
		{
			return bytes_to_int16(data, 0);
		}
		/// <summary>
		/// byte[2]��Int16�֕ϊ�
		/// </summary>
		/// <param name="data">�ϊ�����o�C�g�z��B</param>
		/// <param name="Index">data���̕ϊ����J�n����C���f�b�N�X�B</param>
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
		// �^�ϊ�(Int16 -> byte[2])
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	short								�ϊ����ް�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				����
		//
		//====================================================================================================
		/// <summary>
		/// Int16��byte[2]�֕ϊ�
		/// </summary>
		/// <param name="data">�ϊ�����l�B</param>
		public static byte[] Int16ToBytes(short data)
		{
			byte[]	buff = new byte[2];
			
			buff[0] = (byte)(data &0xFF);
			buff[1] = (byte)((data >>8) &0xFF);

			return buff;
		}

		/// <summary>
		/// �o�C�g�z��̘A���ϊ��N���X
		/// </summary>
		public class Target�@
		{
			private int		index = 0;
			private byte[]	data;

			//====================================================================================================
			// �R���X�g���N�^
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	ref byte[]		base_data			�Ώۂ��޲Ĕz��
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//====================================================================================================
			/// <summary>
			/// �R���X�g���N�^
			/// </summary>
			/// <param name="base_data">�f�[�^���i�[����(���Ă���)�o�C�g�z��B</param>
			public Target(ref byte[] base_data)
			{
				index = 0;
				data = base_data;
			}

			//====================================================================================================
			// �o�C�g�z��֒ǉ�
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	object			source				�ǉ������ް�
			//	int				size				�ǉ������޲Đ�
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//====================================================================================================
			/// <summary>
			/// �o�C�g�z��փf�[�^��ǉ����܂��B
			/// </summary>
			/// <param name="source">�ǉ�����f�[�^���i�[����Ă��镶����B</param>
			/// <param name="size">�ǉ�����o�C�g���B</param>
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
			/// �o�C�g�z��փf�[�^��ǉ����܂��B
			/// </summary>
			/// <param name="source">�ǉ�����f�[�^���i�[����Ă���o�C�g�z��B</param>
			/// <param name="size">�ǉ�����o�C�g���B</param>
			public void Add(byte[] source, int size)
			{
				if (source != null)
				{
					Bytes.Copy(source, 0, data, index, source.Length < size ? source.Length : size);
				}

				index += size;
			}
			/// <summary>
			/// �o�C�g�z��փf�[�^��ǉ����܂��B
			/// </summary>
			/// <param name="source">�ǉ�����f�[�^���i�[����Ă���Int32�B</param>
			public void Add(int source)
			{
				byte[]	temp;

				temp = Bytes.Int32ToBytes(source);
			
				Bytes.Copy(temp, 0, data, index, Marshal.SizeOf(source));
			
				index += Marshal.SizeOf(source);
			}
			/// <summary>
			/// �o�C�g�z��փf�[�^��ǉ����܂��B
			/// </summary>
			/// <param name="source">�ǉ�����f�[�^���i�[����Ă���Int16�B</param>
			public void Add(short source)
			{
				byte[]	temp;

				temp = Bytes.Int16ToBytes(source);
			
				Bytes.Copy(temp, 0, data, index, Marshal.SizeOf(source));
			
				index += Marshal.SizeOf(source);
			}

			//====================================================================================================
			// �o�C�g�z�񂩂璊�o
			//
			//- INPUT --------------------------------------------------------------------------------------------
			//	int				size				���o�����޲Đ�
			//
			//- OUTPUT -------------------------------------------------------------------------------------------
			//	ref object		source				���o�����ް�
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	�Ȃ�
			//
			//====================================================================================================
			/// <summary>
			/// �o�C�g�z�񂩂�f�[�^�𒊏o���܂��B
			/// </summary>
			/// <param name="source">���o�����f�[�^���i�[���镶����B</param>
			/// <param name="size">���o����o�C�g���B</param>
			public void Split(ref string source, int size)
			{
				source = Encoding.Default.GetString(data, index, size);
			
				index += size;
			}
			/// <summary>
			/// �o�C�g�z�񂩂�f�[�^�𒊏o���܂��B
			/// </summary>
			/// <param name="source">���o�����f�[�^���i�[����o�C�g�z��B</param>
			/// <param name="size">���o����o�C�g���B</param>
			public void Split(ref byte[] source, int size)
			{
				Bytes.Copy(data, index, source, 0, size);
			
				index += size;
			}
			/// <summary>
			/// �o�C�g�z�񂩂�f�[�^�𒊏o���܂��B
			/// </summary>
			/// <param name="source">���o�����f�[�^���i�[����Int32�B</param>
			public void Split(ref int source)
			{
				source = Bytes.BytesToInt32(data, index);
			
				index += Marshal.SizeOf(source);
			}
			/// <summary>
			/// �o�C�g�z�񂩂�f�[�^�𒊏o���܂��B
			/// </summary>
			/// <param name="source">���o�����f�[�^���i�[����Int16�B</param>
			public void Split(ref short source)
			{
				source = Bytes.BytesToInt16(data, index);
			
				index += Marshal.SizeOf(source);
			}
		}
	}
	#endregion

	#region Win32API�N���X
	/// <summary>
	/// Win32API�N���X
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
		/// �T�E���h���Đ����܂��B
		/// </summary>
		/// <param name="file_name">�t�@�C�������i�[����������B</param>
		public static void PlaySound(string file_name)
		{
			API_PlaySound(file_name, IntPtr.Zero, SND_ASYNC | SND_FILENAME);
		}

		/// <summary>
		/// �T�E���h���Đ����܂��B
		/// </summary>
		/// <param name="stream">�A�Z���u�����̎w�肳�ꂽ�}�j�t�F�X�g���\�[�X�B</param>
		public static void PlaySound(Stream stream)
		{
			byte[]	byte_data = new byte[stream.Length];
			
			stream.Read(byte_data, 0, (int)stream.Length);
			
			API_PlaySound(byte_data, IntPtr.Zero, SND_ASYNC | SND_MEMORY);
		}

		/// <summary>
		/// �T�E���h��A���Đ����܂��B
		/// </summary>
		/// <param name="file_name">�t�@�C�������i�[����������B</param>
		public static void PlaySoundEndless(string file_name)
		{
			API_PlaySound(file_name, IntPtr.Zero, SND_ASYNC | SND_FILENAME | SND_LOOP);
		}

		/// <summary>
		/// �T�E���h��A���Đ����܂��B
		/// </summary>
		/// <param name="stream">�A�Z���u�����̎w�肳�ꂽ�}�j�t�F�X�g���\�[�X�B</param>
		public static void PlaySoundEndless(Stream stream)
		{
			byte[]	byte_data = new byte[stream.Length];
			
			stream.Read(byte_data, 0, (int)stream.Length);
			
			API_PlaySound(byte_data, IntPtr.Zero, SND_ASYNC | SND_MEMORY | SND_LOOP);
		}

		/// <summary>
		/// �T�E���h���~���܂��B
		/// </summary>
		public static void PlaySoundStop()
		{
			API_PlaySound(IntPtr.Zero, IntPtr.Zero, 0);
		}

		/// <summary>
		/// �r�[�v����炵�܂��B
		/// </summary>
		public static void Beep()
		{
			API_MessageBeep(0xFFFFFFFF);
		}

		/// <summary>
		/// �r�[�v����炵�܂��B
		/// </summary>
		/// <param name="frequency">�r�[�v���̎��g���B (�w���c�P��)</param>
		/// <param name="duration">�r�[�v���̎����B (�~���b)</param>
		public static void Beep(int frequency, int duration)
		{
			API_Beep((uint)frequency, (uint)duration);
		}

		/// <summary>
		/// �w�肵���~���b���̊Ԍ��݂̃X���b�h���u���b�N���܂��B
		/// </summary>
		/// <param name="millisecondsTimeout">�X���b�h���u���b�N�����~���b�����w�肵�܂��B</param>
		public static void Sleep(int millisecondsTimeout)
		{
			Thread.Sleep(millisecondsTimeout);
		}

		/// <summary>
		/// �V�X�e���N����̃~���b�P�ʂ̌o�ߎ��Ԃ��擾���܂��B
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
		/// �V�X�e���N����̃~���b�P�ʂ̌o�ߎ��Ԃ��擾���܂��B
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
		/// �l�b�g���[�N�h���C�u�����蓖�Ă܂��B
		/// </summary>
		/// <param name="LocalName">�h���C�u��</param>
		/// <param name="RemoteName">�l�b�g���[�N��</param>
		/// <param name="Username">���[�U�[��</param>
		/// <param name="Password">�p�X���[�h</param>
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
		/// �l�b�g���[�N�h���C�u�����蓖�Ă܂��B
		/// </summary>
		/// <param name="LocalName">�h���C�u��</param>
		/// <param name="RemoteName">�l�b�g���[�N��</param>
		public static uint NetAdd(string LocalName, string RemoteName)
		{
			return NetAdd(LocalName, RemoteName, null, null);
		}

		/// <summary>
		/// �l�b�g���[�N�h���C�u��ؒf���܂��B
		/// </summary>
		/// <param name="LocalName">�h���C�u��</param>
		public static uint NetCancel(string LocalName)
		{
			return API_WNetCancelConnection2(LocalName, 0, false);
		}

		/// <summary>
		/// ���݂̃V�X�e��������ݒ肵�܂��B
		/// </summary>
		/// <param name="time">�ݒ肷�����</param>
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

	#region �R���\�[����ʃN���X

	/// <summary>
	/// �R���\�[����ʃN���X
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

		#region �񋓌^

		/// <summary>
		/// �F����
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
		/// �F�e�[�u��
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
		/// ����{�^���𖳌��ɂ��܂��B
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
		/// �R���\�[����ʂ̃^�C�g����ݒ肵�܂��B
		/// </summary>
		/// <param name="titleName">�^�C�g�����i�[����Ă��镶����B</param>
		/// <returns></returns>
		public static bool SetTitle(string titleName)
		{
			Console.Title = titleName;

			return true;
		}

		/// <summary>
		/// �R���\�[����ʂ̃T�C�Y��ݒ肵�܂��B
		/// </summary>
		/// <param name="x">���������B</param>
		/// <param name="y">���������B</param>
		public static void WindowSize(int x, int y)
		{
			Console.SetWindowSize(x, y);
			Console.SetBufferSize(x, y);
		}

		/// <summary>
		/// �R���\�[����ʂ����������܂��B
		/// </summary>
		public static void ScreenClear()
		{
			Color();

			Console.Clear();
		}

		/// <summary>
		/// �J�[�\���̕\���^��\����ݒ肵�܂��B
		/// </summary>
		public static bool CursorVisible
		{
			set
			{
				Console.CursorVisible = value;
			}
		}

		/// <summary>
		/// �J�[�\���̈ʒu��ݒ肵�܂��B
		/// </summary>
		/// <param name="x">�������W�B</param>
		/// <param name="y">�������W�B</param>
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
		/// �w�肵���ʒu�ɕ�������������݂܂��B
		/// </summary>
		/// <param name="x">�������W�B</param>
		/// <param name="y">�������W�B</param>
		/// <param name="data">�������ޕ�����B</param>
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
		/// �J�[�\���̐F��ݒ肵�܂��B
		/// </summary>
		/// <param name="rgb">�F�����B</param>
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
		/// �J�[�\���̐F��ݒ肵�܂��B
		/// </summary>
		/// <param name="colorTable">�F�e�[�u���B</param>
		public static void Color(Table colorTable)
		{
			Console.ForegroundColor = (ConsoleColor)colorTable;
		}
		/// <summary>
		/// �J�[�\���̐F��ݒ肵�܂��B
		/// </summary>
		/// <param name="foregroundColor">�O�i�F</param>
		/// <param name="backgroundColor">�w�i�F</param>
		public static void Color(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
		{
			Console.ForegroundColor = foregroundColor;
			Console.BackgroundColor = backgroundColor;
		}
		/// <summary>
		/// �J�[�\���̐F��ݒ肵�܂��B
		/// </summary>
		public static void Color()
		{
			Color(ConsoleColor.White, ConsoleColor.Black);
		}

		/// <summary>
		/// �R���\�[���̓��͂��`�F�b�N���܂��B
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
		/// �R���\�[����ʂ��쐬�ς݂��ǂ������擾���܂��B
		/// </summary>
		public static bool IsCreated
		{
			get { return Created; }
		}

		/// <summary>
		/// �R���\�[����ʂ��쐬���܂��B
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
		/// �R���\�[����ʂ�������܂��B
		/// </summary>
		public static void ReleaseConsole()
		{
			FreeConsole();

			Created = false;
		}
	}

	#endregion

	#region ���ёւ��N���X(ListView)

	/// <summary>
	/// ListView�̍��ڂ̕��ёւ��Ɏg�p����N���X
	/// </summary>
	public class ListViewItemComparer : IComparer
	{
		#region �񋓑�

		/// <summary>
		/// ��r������@
		/// </summary>
		public enum ComparerMode
		{
			/// <summary>
			/// ������Ƃ��Ĕ�r
			/// </summary>
			String,
			/// <summary>
			/// ���l�iInt32�^�j�Ƃ��Ĕ�r
			/// </summary>
			Integer,
			/// <summary>
			/// �����iDataTime�^�j�Ƃ��Ĕ�r
			/// </summary>
			DateTime,
		}

		#endregion

		private int _column;
		private SortOrder _order;
		private ComparerMode _mode;
		private ComparerMode[] _columnModes;

		#region �v���p�e�B

		/// <summary>
		/// ���ёւ���ListView��̔ԍ�
		/// </summary>
		public int Column
		{
			set
			{
				//���݂Ɠ�����̎��́A�����~����؂�ւ���
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
		/// �������~����
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
		/// ���ёւ��̕��@
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
		/// �񂲂Ƃ̕��ёւ��̕��@
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
		/// ListViewItemComparer�N���X�̃R���X�g���N�^
		/// </summary>
		/// <param name="columnNo">���ёւ����̔ԍ�</param>
		/// <param name="sortOrder">�������~����</param>
		/// <param name="comparerMode">���ёւ��̕��@</param>
		public ListViewItemComparer(int columnNo, SortOrder sortOrder, ComparerMode comparerMode)
		{
			_column = columnNo;
			_order = sortOrder;
			_mode = comparerMode;
		}
		/// <summary>
		/// ListViewItemComparer�N���X�̃R���X�g���N�^
		/// </summary>
		public ListViewItemComparer()
		{
			_column = 0;
			_order = SortOrder.Ascending;
			_mode = ComparerMode.String;
		}

		/// <summary>
		/// ��r
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>x��y��菬�����Ƃ��̓}�C�i�X�̐��A�傫���Ƃ��̓v���X�̐��A�����Ƃ���0��Ԃ�</returns>
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
				//���ёւ��̕��@�ʂɁAx��y���r����
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

				//�~���̎��͌��ʂ�+-�t�ɂ���
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
