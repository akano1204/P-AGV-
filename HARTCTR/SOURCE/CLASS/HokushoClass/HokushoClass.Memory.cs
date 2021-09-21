using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Security.AccessControl;

namespace HokushoClass.SharedMemory
{
	//====================================================================================================
	// SharedMemory CLass
	//====================================================================================================
	/// <summary>
	/// ���L�������N���X
	/// </summary>
	public class H_Memory
	{
		private Mutex Mutex;
		private MemoryMappedFile Mmf;
        private MemoryMappedViewAccessor Accs = null;
        private int Error_code;
		private string Error_message;

		//====================================================================================================
		// ���L�������쐬
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			objectName			���L��ؖ�	
		//	uint			byteSize			���L��ػ���
		//  string          groupName           ��������ǉ������ٰ�ߖ��܂���հ�ް��       
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// ���L���������쐬��� �I�[�v�����܂��B
		/// </summary>
		/// <param name="objectName">�쐬���鋤�L���������B</param>
		/// <param name="byteSize">�쐬���鋤�L�������̃o�C�g�T�C�Y�B</param>
		/// <param name="groupName">�A�N�Z�X����ǉ�����O���[�v���B</param>
		/// <returns></returns>
		public bool CreateMemory(string objectName, uint byteSize, string groupName)
		{
			bool status = true;
            bool cr;

			errors();


			if (groupName.Trim() != "")
			{

                try
                {
                    MutexSecurity security = new MutexSecurity();
                    
                    security.AddAccessRule(new MutexAccessRule(groupName, MutexRights.FullControl, AccessControlType.Allow));

                    Mutex = new Mutex(false, objectName + "_MUTEX", out cr, security);
				}
				catch (Exception ex)
				{
					status = false;

					errors(-1000, ex.Message);
				}

				try
				{
                    

                    //*********************************************************************************
                    AccessRule access = new  AccessRule<MemoryMappedFileRights>(groupName, MemoryMappedFileRights.FullControl, AccessControlType.Allow);
                    MemoryMappedFileSecurity security = new MemoryMappedFileSecurity();

                    security.SetAccessRule(new AccessRule<MemoryMappedFileRights>(groupName, 
                        MemoryMappedFileRights.FullControl, AccessControlType.Allow));
                    
                    Mutex.WaitOne();

                    Mmf = MemoryMappedFile.CreateOrOpen(objectName, byteSize, 
                        MemoryMappedFileAccess.ReadWrite
                        ,MemoryMappedFileOptions.None
                        , security
                        , HandleInheritability.Inheritable

                        );

                    Accs = Mmf.CreateViewAccessor();

                    Mutex.ReleaseMutex();

                    //*********************************************************************************


				}
				catch (Exception ex)
				{
					status = false;

					errors(-1000, ex.Message);
				}
            }
            else
            {
                try
                {
                    Mutex = new Mutex(false, objectName + "_MUTEX");
                }
                catch (Exception ex)
                {
                    status = false;

                    errors(-1000, ex.Message);
                }


                if (status)
                {
                    try
                    {
                        Mutex.WaitOne();

                        Mmf = MemoryMappedFile.CreateOrOpen(objectName, byteSize, MemoryMappedFileAccess.ReadWrite);

                        Accs = Mmf.CreateViewAccessor();

                        Mutex.ReleaseMutex();
                    }
                    catch (Exception ex)
                    {
                        errors(-1000, ex.Message);
                    }
                }
            }
            return Error_code == 0 ? true : false;
		}
		/// <summary>
		/// ���L���������쐬��� �I�[�v�����܂��B
		/// </summary>
		/// <param name="objectName">�쐬���鋤�L���������B</param>
		/// <param name="byteSize">�쐬���鋤�L�������̃o�C�g�T�C�Y�B</param>
		/// <returns></returns>
		public bool CreateMemory(string objectName, uint byteSize)
		{
			return CreateMemory(objectName, byteSize, "");
		}

		//====================================================================================================
		// ���L���������
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
		/// <summary>
		/// ���L��������������܂��B
		/// </summary>
		public void ReleaseMemory()
		{
			Mutex.Close();

            Accs.Dispose();

            Mmf.Dispose();

			Accs = null;
		}

		//====================================================================================================
		// �ǂݍ���
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				index   			�ǂݍ��݊J�n���ޯ��
		//	uint			byteSize			�ǂݍ��ݻ���
		//	bool			lockFlag  			ۯ��׸�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				�ǂݍ����ް�	
		//
		//====================================================================================================
		/// <summary>
		/// ���L����������o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="index">���L�������̓ǂݍ��݂��J�n����I�t�Z�b�g�l�B</param>
		/// <param name="byteSize">���L����������ǂݍ��ރo�C�g���B</param>
		/// <param name="lockFlag">���L�������ɔr�����b�N����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <returns></returns>
		public byte[] ReadMemory(int index, uint byteSize, bool lockFlag)
		{
			byte[] data = new byte[byteSize];

			if (lockFlag)
			{
				Mutex.WaitOne();
			}

			try
			{
                Accs.ReadArray(index, data, 0, (int)byteSize);
			}
			catch (Exception ex)
			{
				errors(-1000, ex.Message);
			}

			if (lockFlag)
			{
				Mutex.ReleaseMutex();
			}

			return data;
		}
		/// <summary>
		/// ���L����������o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="index">���L�������̓ǂݍ��݂��J�n����I�t�Z�b�g�l�B</param>
		/// <param name="byteSize">���L����������ǂݍ��ރo�C�g���B</param>
		/// <returns></returns>
		public byte[] ReadMemory(int index, uint byteSize)
		{
			return ReadMemory(index, byteSize, true);
		}

		//====================================================================================================
		// ��������
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				index   			�������݊J�n���ޯ��
		//	byte[]			data				���������ް�	
		//	uint			byteSize			�������ݻ���
		//	bool			lockFlag  			ۯ��׸�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// ���L�������Ƀo�C�g�z��̃f�[�^���������݂܂��B
		/// </summary>
		/// <param name="index">���L�������̏������݂��J�n����I�t�Z�b�g�l�B</param>
		/// <param name="data">���L�������ɏ������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <param name="byteSize">���L�������ɏ������ރo�C�g���B</param>
		/// <param name="lockFlag">���L�������ɔr�����b�N����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public void WriteMemory(int index, byte[] data, uint byteSize, bool lockFlag)
		{
			if (lockFlag)
			{
				Mutex.WaitOne();
			}

			try
			{
                Accs.WriteArray(index, data, 0, (int)byteSize);
            }
			catch (Exception ex)
			{
				errors(-1000, ex.Message);
			}

			if (lockFlag)
			{
				Mutex.ReleaseMutex();
			}
		}
		/// <summary>
		/// ���L�������Ƀo�C�g�z��̃f�[�^���������݂܂��B
		/// </summary>
		/// <param name="index">���L�������̏������݂��J�n����I�t�Z�b�g�l�B</param>
		/// <param name="data">���L�������ɏ������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <param name="byteSize">���L�������ɏ������ރo�C�g���B</param>
		public void WriteMemory(int index, byte[] data, uint byteSize)
		{
			WriteMemory(index, data, byteSize, true);
		}

		//====================================================================================================
		// �����I���b�N
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
		/// <summary>
		/// ���L�������ւ̔r�����b�N���s���܂��B
		/// </summary>
		public void Lock()
		{
			Mutex.WaitOne();
		}

		//====================================================================================================
		// �����I���b�N����
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
		/// <summary>
		/// ���L�������ւ̔r�����b�N�������s���܂��B
		/// </summary>
		public void Unlock()
		{
			Mutex.ReleaseMutex();
		}

		//====================================================================================================
		// �ُ�R�[�h�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ�R�[�h���擾���܂��B
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return Error_code;
			}
		}

		//====================================================================================================
		// �ُ���e�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ���e���擾���܂��B
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return Error_message;
			}
		}

		//====================================================================================================
		// �ُ�̐ݒ�
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				code				�ُ���
		//	string			message				�ُ���e
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		private void errors()
		{
			Error_code = 0;
			Error_message = "";
		}
		private void errors(int error_code, string comment)
		{
			Error_code = error_code;
			Error_message = comment;
		}

		//====================================================================================================
		// �^�ϊ�(byte[4] -> Int32)
		//====================================================================================================
		/// <summary>
		/// �S�o�C�g�̃o�C�g�z��� Int32 �ɕϊ����܂��B
		/// </summary>
		/// <param name="data">�ϊ�����f�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns></returns>
		protected int BytesToInt32(byte[] data)
		{
			return BitConverter.ToInt32(data, 0);
		}

		//====================================================================================================
		// �^�ϊ�(Int32 -> byte[4])
		//====================================================================================================
		/// <summary>
		/// Int32 ���S�o�C�g�̃o�C�g�z��ɕϊ����܂��B
		/// </summary>
		/// <param name="data">�ϊ�����l�B</param>
		/// <returns></returns>
		protected byte[] Int32ToBytes(int data)
		{
			return BitConverter.GetBytes(data);
		}

		//====================================================================================================
		// �^�ϊ�(byte[2] -> Int16)
		//====================================================================================================
		/// <summary>
		/// �Q�o�C�g�̃o�C�g�z��� Int16 �ɕϊ����܂��B
		/// </summary>
		/// <param name="data">�ϊ�����f�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns></returns>
		protected short BytesToInt16(byte[] data)
		{
			return BitConverter.ToInt16(data, 0);
		}

		//====================================================================================================
		// �^�ϊ�(Int16 -> byte[2])
		//====================================================================================================
		/// <summary>
		/// Int16 ���Q�o�C�g�̃o�C�g�z��ɕϊ����܂��B
		/// </summary>
		/// <param name="data">�ϊ�����l�B</param>
		/// <returns></returns>
		protected byte[] Int16ToBytes(short data)
		{
			return BitConverter.GetBytes(data);
		}
	}
}
