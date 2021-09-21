using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

#if !WindowsCE
using System.Security.AccessControl;
#endif

namespace BelicsClass.ProcessManage
{
    /// <summary>
    /// 共有メモリクラス
    /// </summary>
    public unsafe class BL_Memory
    {
        private WinControl win_control = null;
        private BL_Mutex mutex;
        private int Error_code;
        private string Error_message;

        //private object sync = new object();

        /// <summary>
        /// メモリー実体を取得します
        /// </summary>
        public object Memory { get { return win_control.Mem; } }

        public BL_Memory()
        {
            if (Environment.Is64BitProcess)
            {
                win_control = new Win64Control();
            }
            else
            {
                win_control = new Win32Control();
            }
        }

        //====================================================================================================
        // 共有メモリ作成
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	string			objectName			共有ﾒﾓﾘ名	
        //	uint			byteSize			共有ﾒﾓﾘｻｲｽﾞ
        //  string          groupName           アクセス許可を追加するグループ名またはユーザー名       
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常
        //
        //====================================================================================================
        /// <summary>
        /// 共有メモリを作成し､ オープンします。
        /// </summary>
        /// <param name="objectName">作成する共有メモリ名。</param>
        /// <param name="byteSize">作成する共有メモリのバイトサイズ。</param>
        /// <param name="groupName">アクセス許可を追加するグループ名。</param>
        /// <returns></returns>
        public bool CreateMemory(string objectName, uint byteSize, string groupName)
        {
            bool status = true;
            //#if !WindowsCE
            //            MutexSecurity security;
            //            MutexAccessRule rule;
            //#endif
            //            errors();

            //#if !WindowsCE
            //            mutex = new Mutex(false, objectName + "_MUTEX");
            //#else
            //            mutex = new Mutex(false);
            //#endif

            //#if !WindowsCE
            //            try
            //            {
            //                rule = new MutexAccessRule(groupName, MutexRights.FullControl, AccessControlType.Allow);

            //                security = mutex.GetAccessControl();
            //                security.AddAccessRule(rule);
            //                mutex.SetAccessControl(security);
            //            }
            //            catch (Exception ex)
            //            {
            //                status = false;
            //                errors(-2, ex.Message);
            //                mutex.Close();
            //            }
            //#endif
            mutex = new BL_Mutex(objectName.Trim() + "_MUTEX");

            if (status == true)
            {
                this.Lock();

                if (!win_control.CreateMemory(objectName.Trim(), byteSize, true))
                {
                    var error = win_control.GetError();

                    var code = error.Item1;
                    var message = error.Item2;

                    if (code == 0) errors(-1, "予約済");
                    else
                    {
                        string header = "";

                        if (win_control is Win32Control)
                        {
                            header += "Win32 API Error : ";
                        }
                        else
                        {
                            header+= "ExceptionMessage : ";
                        }

                        errors(code, header + message);
                    }

                    mutex.Close();
                }

                this.Unlock();
            }

            return Error_code == 0 ? true : false;
        }

        /// <summary>
        /// 共有メモリを作成し､ オープンします。
        /// </summary>
        /// <param name="objectName">作成する共有メモリ名。</param>
        /// <param name="byteSize">作成する共有メモリのバイトサイズ。</param>
        /// <returns></returns>
        public bool CreateMemory(string objectName, uint byteSize)
        {
            errors();

            //#if !WindowsCE
            //            mutex = new Mutex(false, objectName + "_MUTEX");
            //#else
            //            mutex = new Mutex(false);
            //#endif
            mutex = new BL_Mutex(objectName.Trim() + "_MUTEX");

            this.Lock();

            win_control.ReleaseMemory();

            if (!win_control.CreateMemory(objectName.Trim(), byteSize, false))
            {
                var error = win_control.GetError();

                var code = error.Item1;
                var message = error.Item2;

                if (code == 0) errors(-1, "予約済");
                else
                {
                    string header = "";

                    if (win_control is Win32Control)
                    {
                        header += "Win32 API Error : ";
                    }
                    else
                    {
                        header += "ExceptionMessage : ";
                    }

                    errors(code, header + message);
                }
                mutex.Close();

                return false;
            }

            this.Unlock();

            return Error_code == 0 ? true : false;
        }

        /// <summary>
        /// 共有メモリの存在をチェックします
        /// </summary>
        /// <param name="objectName">共有メモリ名称</param>
        /// <returns></returns>
        public bool IsExist(string objectName)
        {
            return win_control.IsExist(objectName.Trim());
        }

        //====================================================================================================
        // 共有メモリ解放
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
        /// <summary>
        /// 共有メモリを解放します。
        /// </summary>
        public void ReleaseMemory()
        {
            win_control.ReleaseMemory();
            mutex.Close();
        }

        //====================================================================================================
        // 読み込み
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				index   			読み込み開始ｲﾝﾃﾞｯｸｽ
        //	uint			byteSize			読み込みｻｲｽﾞ
        //	bool			lockFlag  			ﾛｯｸﾌﾗｸﾞ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	byte[]			data				読み込みﾃﾞｰﾀ	
        //
        //====================================================================================================
        /// <summary>
        /// 共有メモリからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="index">共有メモリの読み込みを開始するオフセット値。</param>
        /// <param name="byteSize">共有メモリから読み込むバイト数。</param>
        /// <param name="lockFlag">共有メモリに排他ロックする場合は true。それ以外の場合は false。</param>
        /// <returns></returns>
        public byte[] ReadMemory(int index, uint byteSize, bool lockFlag)
        {
            if (byteSize <= 0) return new byte[0];

            byte[] data = new byte[byteSize];

            try
            {
                //bool locked = IsLock;
                //if (lockFlag && !locked)
                {
                    this.Lock();
                }

                win_control.ReadMemory(data, index, byteSize);

                //if (lockFlag && !locked)
                {
                    this.Unlock();
                }

                return data;
            }
            catch (Exception ex)
            {
                errors(-1, "サイズ不足(" + ex.Message + ")");
            }

            return null;
        }

        /// <summary>
        /// 共有メモリからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="index">共有メモリの読み込みを開始するオフセット値。</param>
        /// <param name="byteSize">共有メモリから読み込むバイト数。</param>
        /// <returns></returns>
        public byte[] ReadMemory(int index, uint byteSize)
        {
            return ReadMemory(index, byteSize, true);
        }

        //====================================================================================================
        // 書き込み
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				index   			書き込み開始ｲﾝﾃﾞｯｸｽ
        //	byte[]			data				書き込みﾃﾞｰﾀ	
        //	uint			byteSize			書き込みｻｲｽﾞ
        //	bool			lockFlag  			ﾛｯｸﾌﾗｸﾞ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// 共有メモリにバイト配列のデータを書き込みます。
        /// </summary>
        /// <param name="index">共有メモリの書き込みを開始するオフセット値。</param>
        /// <param name="data">共有メモリに書き込むデータが格納されているバイト配列。</param>
        /// <param name="byteSize">共有メモリに書き込むバイト数。</param>
        /// <param name="lockFlag">共有メモリに排他ロックする場合は true。それ以外の場合は false。</param>
        public void WriteMemory(int index, byte[] data, uint byteSize, bool lockFlag)
        {
            if (byteSize <= 0) return;
            byte[] sub = new byte[byteSize];

            if (data.Length < byteSize) Array.Copy(data, sub, data.Length);
            else Array.Copy(data, sub, (int)byteSize);

            //bool locked = IsLock;
            //if (lockFlag && !locked)
            if (lockFlag)
            {
                this.Lock();
            }

            win_control.WriteMemory(index, sub, byteSize);

            //if (lockFlag && !locked)
            if (lockFlag)
            {
                this.Unlock();
            }
        }
        /// <summary>
        /// 共有メモリにバイト配列のデータを書き込みます。
        /// </summary>
        /// <param name="index">共有メモリの書き込みを開始するオフセット値。</param>
        /// <param name="data">共有メモリに書き込むデータが格納されているバイト配列。</param>
        /// <param name="byteSize">共有メモリに書き込むバイト数。</param>
        public void WriteMemory(int index, byte[] data, uint byteSize)
        {
            WriteMemory(index, data, byteSize, true);
        }

        //====================================================================================================
        // 明示的ロック
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
        /// <summary>
        /// 共有メモリへの排他ロックを行います。
        /// </summary>
        public void Lock()
        {
            //lock (sync)
            {
#if !WindowsCE
                try
                {
                    mutex.Lock();
                }
                catch { }
#else
                //mutex.WaitOne();
                mutex.Lock();

#endif
            }
        }

        //====================================================================================================
        // 明示的ロック解除
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
        /// <summary>
        /// 共有メモリへの排他ロック解除を行います。
        /// </summary>
        public void Unlock()
        {
            //lock (sync)
            {
                mutex.Unlock();
            }
        }

        ///// <summary>
        ///// ロック中かどうかを検査します。
        ///// </summary>
        //public bool IsLock { get { return mutex.IsLocked; } }

        //====================================================================================================
        // 異常コードプロパティ
        //====================================================================================================
        /// <summary>
        /// 異常コードを取得します。
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return Error_code;
            }
        }

        //====================================================================================================
        // 異常内容プロパティ
        //====================================================================================================
        /// <summary>
        /// 異常内容を取得します。
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return Error_message;
            }
        }

        //====================================================================================================
        // 異常の設定
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				code				異常ｺｰﾄﾞ
        //	string			message				異常内容
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
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
        // Win32Control CLass
        //====================================================================================================
        private abstract class WinControl
        {
            public abstract object Mem { get; }

            internal abstract bool CreateMemory(string objectName, uint byteSize, bool attributeEntry);

            internal abstract bool IsExist(string objectName);

            internal abstract void ReleaseMemory();

            internal abstract void ReadMemory(byte[] destination, int position, uint length);

            internal abstract void WriteMemory(int position, byte[] source, uint length);

            internal abstract Tuple<int, string> GetError();
        }

        private class Win32Control : WinControl
        {
            #region Win32API
            [DllImport("kernel32", SetLastError = true)]
            private static extern unsafe IntPtr CreateFileMapping(IntPtr hFile, SECURITY_ATTRIBUTES* attributes, uint protect, uint sizeHigh, uint sizeLow, string objectName);

            [DllImport("kernel32", SetLastError = true)]
            private static extern unsafe bool CloseHandle(IntPtr hFile);

            [DllImport("kernel32", SetLastError = true)]
            private static extern unsafe int MapViewOfFile(IntPtr hMapObject, uint accessMode, uint offsetHigh, uint offsetLow, uint mapSize);

            [DllImport("kernel32", SetLastError = true)]
            private static extern unsafe bool UnmapViewOfFile(void* baseAddress);

            [DllImport("kernel32", SetLastError = true)]
            private static extern unsafe void CopyMemory(void* destination, void* source, uint length);

            [DllImport("kernel32", SetLastError = true)]
            private static extern int GetLastError();

            [DllImport("kernel32", SetLastError = true)]
            private static extern uint FormatMessage(
                uint dwFlags,
                IntPtr lpSource,
                uint dwMessageId,
                uint dwLanguageId,
                StringBuilder lpBuffer,
                int nSize,
                IntPtr Arguments);

            [DllImport("advapi32", SetLastError = true)]
            private static extern bool InitializeSecurityDescriptor(ref SECURITY_DESCRIPTOR psd, uint dwRevision);

            [DllImport("advapi32", SetLastError = true)]
            private static extern bool SetSecurityDescriptorDacl(ref SECURITY_DESCRIPTOR psd, uint fDaclPresent, ACL* pAcl, uint fDaclDefaulted);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr OpenFileMapping(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);

            [StructLayout(LayoutKind.Sequential)]
            private struct ACL
            {
                public byte AclRevision;
                public byte Sbz1;
                public ushort AclSize;
                public ushort AceCount;
                public ushort Sbz2;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SECURITY_DESCRIPTOR
            {
                public byte Revision;
                public byte Sbz1;
                public uint Control;
                public uint Owner;
                public uint Group;
                public ACL Sacl;
                public ACL Dacl;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SECURITY_ATTRIBUTES
            {
                public uint nLength;
                public SECURITY_DESCRIPTOR* lpSecurityDescriptor;
                public uint bInheritHandle;
            }

#if !WindowsCE
            [DllImport("kernel32.dll")]
            static extern uint VirtualQuery(
                IntPtr lpAddress, // 領域のアドレス
                ref MEMORY_BASIC_INFORMATION lpBuffer, // 情報バッファのアドレス
                int dwLength // バッファのサイズ
            );

            [DllImport("kernel32.dll")]
            static extern uint VirtualQuery(
                IntPtr lpAddress, // 領域のアドレス
                ref MEMORY_BASIC_INFORMATION64 lpBuffer, // 情報バッファのアドレス
                int dwLength // バッファのサイズ
            );

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct MEMORY_BASIC_INFORMATION
            {
                public IntPtr BaseAddress;
                public IntPtr AllocationBase;
                public uint AllocationProtect;
                public uint RegionSize;
                public uint State;
                public uint Protect;
                public uint Type;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 16)]
            struct MEMORY_BASIC_INFORMATION64
            {
                public IntPtr BaseAddress;
                public IntPtr AllocationBase;
                public UInt32 AllocationProtect;
                public UInt32 __alignment1;
                public uint RegionSize;
                public UInt32 State;
                public UInt32 Protect;
                public UInt32 Type;
                public UInt32 __alignment2;
            }
#endif

            #endregion

            public override object Mem => mem;

            private IntPtr mem;
            private IntPtr handle = IntPtr.Zero;
            private uint Size = 0;

            internal override bool CreateMemory(string objectName, uint byteSize, bool attributeEntry)
            {
                Size = byteSize;
#if !WindowsCE
                SECURITY_DESCRIPTOR sd;
                SECURITY_ATTRIBUTES sa;

                const uint SECURITY_DESCRIPTOR_REVISION = 0x00000001;
                const uint PAGE_READWRITE = 0x00000004;
                const uint FILE_MAP_ALL_ACCESS = 0x000F001F;
#endif

                if (handle != IntPtr.Zero) return false;

#if !WindowsCE
                if (attributeEntry == true)
                {
                    sd = new SECURITY_DESCRIPTOR();
                    sa = new SECURITY_ATTRIBUTES();
                    InitializeSecurityDescriptor(ref sd, SECURITY_DESCRIPTOR_REVISION);
                    SetSecurityDescriptorDacl(ref sd, 1, (ACL*)null, 0);
                    sa.nLength = (uint)sizeof(SECURITY_ATTRIBUTES);
                    sa.lpSecurityDescriptor = &sd;
                    sa.bInheritHandle = 1;
                }
                if (attributeEntry == true)
                {
                    handle = CreateFileMapping((IntPtr)(-1), &sa, PAGE_READWRITE, 0, byteSize, objectName);
                }
                else
                {
                    handle = CreateFileMapping((IntPtr)(-1), (SECURITY_ATTRIBUTES*)null, PAGE_READWRITE, 0, byteSize, objectName);
                }
#else
                handle = CreateFileMapping((IntPtr)(-1), (SECURITY_ATTRIBUTES*)null, 4, 0, byteSize, objectName);
#endif

                if (handle == IntPtr.Zero) return false;

#if !WindowsCE
                mem = (IntPtr)MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
#else
                Mem = (IntPtr)MapViewOfFile(handle, 983071, 0, 0, 0);
#endif

#if !WindowsCE
                if (IntPtr.Size == 4)
                {
                    MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();
                    uint size = VirtualQuery((IntPtr)mem, ref mbi, Marshal.SizeOf(mbi));

                    if (mbi.RegionSize < byteSize)
                    {
                        ReleaseMemory();
                        return false;
                    }
                }
                else if (IntPtr.Size == 8)
                {
                    //MEMORY_BASIC_INFORMATION64 mbi = new MEMORY_BASIC_INFORMATION64();
                    //uint size = VirtualQuery((IntPtr)Mem, ref mbi, Marshal.SizeOf(mbi));

                    //if (mbi.RegionSize < byteSize)
                    //{
                    //    ReleaseMemory();
                    //    return false;
                    //}
                }
#endif
                return true;
            }

            internal override bool IsExist(string objectName)
            {
                IntPtr NULL = IntPtr.Zero;
                const uint PAGE_READ = 0x00000004;

                try
                {
                    bool b = false;
                    IntPtr h = OpenFileMapping(PAGE_READ, false, objectName);
                    if (h != NULL)
                    {
                        CloseHandle(h);
                        b = true;
                    }
                    return b;
                }
                catch
                {
                    return false;
                }
            }

            internal override void ReleaseMemory()
            {
                if (handle != IntPtr.Zero)
                {
                    UnmapViewOfFile((void*)mem);
                    CloseHandle(handle);

                    handle = IntPtr.Zero;
                }
            }

            internal override void ReadMemory(byte[] destination, int position, uint length)
            {
                if (Size < position + length) throw new Exception("共有メモリアクセス違反");

                fixed (byte* pointer = &destination[0])
                {
                    CopyMemory(pointer, (byte*)mem + position, length);
                }
            }

            internal override void WriteMemory(int position, byte[] source, uint length)
            {
                if (Size < position + length) throw new Exception("共有メモリアクセス違反");

                fixed (byte* pointer = &source[0])
                {
                    CopyMemory((byte*)mem + position, pointer, length);
                }
            }

            internal override Tuple<int, string> GetError()
            {
                int error_code = GetLastError();

                const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

                StringBuilder error_message = new StringBuilder(1024);

                FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM,
                              IntPtr.Zero,
                              (uint)error_code,
                              0,
                              error_message,
                              error_message.Capacity,
                              IntPtr.Zero);

                return new Tuple<int, string>(error_code, error_message.ToString().Trim());
            }
        }

        private class Win64Control : WinControl
        {
            public override object Mem => mem;

            private MemoryMappedFile mem = null;
            private MemoryMappedViewAccessor handle = null;

            private int error_code = 0;
            private string error_message = "";

            internal override bool CreateMemory(string objectName, uint byteSize, bool attributeEntry)
            {
                errors();

                if (attributeEntry)
                {
                    try
                    {
                        MemoryMappedFileSecurity security = new MemoryMappedFileSecurity();

                        security.SetAccessRule(new AccessRule<MemoryMappedFileRights>("",
                            MemoryMappedFileRights.FullControl, AccessControlType.Allow));

                        mem = MemoryMappedFile.CreateOrOpen(objectName,
                                                            byteSize,
                                                            MemoryMappedFileAccess.ReadWrite,
                                                            MemoryMappedFileOptions.None,
                                                            security,
                                                            HandleInheritability.Inheritable);

                        handle = mem.CreateViewAccessor();
                    }
                    catch (Exception ex)
                    {
                        errors(ex);
                    }
                }
                else
                {
                    mem = MemoryMappedFile.CreateOrOpen(objectName, byteSize, MemoryMappedFileAccess.ReadWrite);

                    handle = mem.CreateViewAccessor();
                }

                return true;
            }

            internal override bool IsExist(string objectName)
            {
                try
                {
                    bool b = false;

                    var m = MemoryMappedFile.OpenExisting(objectName);

                    if (m != null)
                    {
                        m.Dispose();
                        b = true;
                    }
                    return b;
                }
                catch
                {
                    return false;
                }
            }

            internal override void ReleaseMemory()
            {
                if (handle != null)
                {
                    handle.Dispose();

                    mem.Dispose();

                    handle = null;
                }
            }

            internal override void ReadMemory(byte[] destination, int position, uint length)
            {
                try
                {
                    handle.ReadArray(position, destination, 0, (int)length);
                }
                catch (Exception ex)
                {
                    errors(ex);
                }
            }

            internal override void WriteMemory(int position, byte[] source, uint length)
            {
                try
                {
                    handle.WriteArray(position, source, 0, (int)length);
                }
                catch (Exception ex)
                {
                    errors(ex);
                }
            }

            internal override Tuple<int, string> GetError()
            {
                return new Tuple<int, string>(error_code, error_message);
            }

            private void errors(Exception ex = null)
            {
                //error_code = ex?.HResult ?? 0;
                error_code = ex == null ? 0 : -1000;
                error_message = ex?.Message.Trim() ?? "";
            }
        }
    }
}
