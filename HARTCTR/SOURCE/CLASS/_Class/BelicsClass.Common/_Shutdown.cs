using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BelicsClass.Common
{
    /// <summary>
    /// Windowsをシャットダウンするクラス
    /// </summary>
    public class BL_Shutdown
    {
        #region ShutdownFlag

        /// <summary>
        /// EWX_FORCE
        /// 上記パラメータと同時に設定します。
        /// プロセスを強制的に終了させます。
        /// このフラグを指定すると、システムは、現在実行されているアプリケーションへ 
        /// WM_QUERYENDSESSION メッセージや WM_ENDSESSION メッセージを送信しません。
        /// この結果、アプリケーションがデータを失う可能性もあります。
        /// したがって、このフラグは、緊急時にのみ指定してください。
        /// </summary>
        public const int Force = 0x00000004;

        /// <summary>
        /// EWX_FORCEIFHUNG
        /// Windows 2000：プロセスが WM_QUERYENDSESSION または WM_ENDSESSION メッセージに
        /// 応答しない場合、それらのプロセスを終了させます。EWX_FORCE フラグを指定すると、
        /// EWX_FORCEIFHUNG フラグは無視されます。    
        /// </summary>
        public const int ForceIfHung = 0x00000010;

        /// <summary>
        /// EWX_LOGOFF
        /// 呼び出し側のプロセスのセキュリティコンテキストで実行されている
        /// すべてのプロセスを終了し、現在のユーザーをログオフさせます。
        /// </summary>
        public const int Logoff = 0x00000000;

        /// <summary>
        /// EWX_POWEROFF 
        /// システムをシャットダウンした後、電源を切ります。
        /// システムは、パワーオフ機能をサポートしていなければなりません。 
        /// Windows NT/2000/XP：呼び出し側のプロセスに、SE_SHUTDOWN_NAME 特権が必要です。
        /// </summary>
        public const int Poweroff = 0x00000008;

        /// <summary>
        /// EWX_REBOOT
        /// システムをシャットダウンした後、システムを再起動します。
        /// Windows NT/2000/XP：呼び出し側のプロセスに、SE_SHUTDOWN_NAME 特権が必要です。
        /// </summary>
        public const int Reboot = 0x00000002;

        /// <summary>
        /// EWX_SHUTDOWN
        /// システムをシャットダウンして、電源を切っても安全な状態にします。
        /// すべてのバッファをディスクへフラッシュし（バッファの内容をディスクに書き込み）、
        /// 動作していたすべてのプロセスを停止します。    
        /// Windows NT/2000/XP：呼び出し側のプロセスに、SE_SHUTDOWN_NAME 特権が必要です。
        /// </summary>
        public const int Shutdown = 0x00000001;

        /// <summary>
        /// <param name="flag">シャットダウン操作</param>
        /// <param name="reserved">予約済み</param>
        /// <returns></returns>
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ExitWindowsEx(int flag, int reserved);

        #endregion

        #region Privileges related Structures
        // LUID locally unique identifier 
        // LUID は64bit なので、直接LUID_AND_ATTRIBUTES に long で宣言する。

        /// <summary>
        /// typedef struct _LUID_AND_ATTRIBUTES { 
        ///    LUID   Luid;
        ///    DWORD  Attributes;
        /// } LUID_AND_ATTRIBUTES, *PLUID_AND_ATTRIBUTES;
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct LUID_AND_ATTRIBUTES
        {
            /// <summary>
            /// ローカルユニーク
            /// </summary>
            public long Luid;
            /// <summary>
            /// アトリビュート
            /// </summary>
            public int Attributes;
        }

        /// <summary>
        /// typedef struct _TOKEN_PRIVILEGES {
        ///     DWORD PrivilegeCount;
        ///     LUID_AND_ATTRIBUTES Privileges[];
        /// } TOKEN_PRIVILEGES, *PTOKEN_PRIVILEGES; 
        /// StructLayout の Pack=4 は必要。これにより、
        /// 4byte 境界でパッキングする。
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TOKEN_PRIVILEGES
        {
            /// <summary>
            /// PrivilegeCount
            /// </summary>
            public int PrivilegeCount;
            /// <summary>
            /// Privileges
            /// </summary>
            public LUID_AND_ATTRIBUTES Privileges;
        }
        #endregion

        #region Privilege related APIs
        /// <summary>
        /// プロセスに関連付けられているアクセストークンを開きます。
        /// BOOL OpenProcessToken(
        ///     HANDLE ProcessHandle, // プロセスのハンドル
        ///     DWORD DesiredAccess,  // プロセスに対して希望するアクセス権
        ///     PHANDLE TokenHandle   // 開かれたアクセストークンのハンドルへのポインタ
        /// );
        /// </summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            int DesiredAccess,
            ref IntPtr TokenHandle);

        /// <summary>
        /// 指定されたシステムで使われているローカル一意識別子（LUID）を取得し、
        /// 指定された特権名をローカルで表現します。
        /// BOOL LookupPrivilegeValue(
        ///     LPCTSTR lpSystemName, // システムを指定する文字列のアドレス
        ///     LPCTSTR lpName,  // 特権を指定する文字列のアドレス
        ///     PLUID lpLuid     // ローカル一意識別子のアドレス
        /// );
        /// </summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(
            string lpSystemName,
            string lpName,
            ref long lpLuid);

        /// <summary>
        /// 指定したアクセストークン内の特権を有効または無効にします。
        /// TOKEN_ADJUST_PRIVILEGES アクセス権が必要です。
        /// BOOL AdjustTokenPrivileges(
        ///     HANDLE TokenHandle,  // 特権を保持するトークンのハンドル
        ///     BOOL DisableAllPrivileges,   // すべての特権を無効にするためのフラグ
        ///     PTOKEN_PRIVILEGES NewState,  // 新しい特権情報へのポインタ
        ///     DWORD BufferLength,  // PreviousState バッファのバイト単位のサイズ
        ///     PTOKEN_PRIVILEGES PreviousState, // 変更を加えられた特権の元の状態を受け取る
        ///     PDWORD ReturnLength  // PreviousState バッファが必要とするサイズを受け取る
        ///     );
        /// </summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(
            IntPtr TokenHandle,
            bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState,
            int BufferLength,
            IntPtr PreviousState,
            IntPtr ReturnLength
            );
        #endregion

        /// <summary>
        /// If the function succeeds, the return value is nonzero.
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool DoExitWindows(int flag)
        {
            try
            {
                // Windows NT/2000/XP　でシャットダウンするには、
                // SE_SHUTDOWN_NAME 特権が必要なので、その特権をセットする。
                if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                    SetShutdownPrivilege();

                // Windows Me 以前で Force シャットダウンするには、
                // Explorer のプロセスを終了させ、
                // EWX_LOGOFF および EWX_FORCE フラグを指定して、
                // ExitWindowsEx() をコールする。
                else if (System.Environment.OSVersion.Platform == PlatformID.Win32Windows &&
                    ((flag & Force) == Force))
                    KillExplorer();
            }
            catch (Exception)
            {
                return false;
            }
            bool result = ExitWindowsEx(flag, 0);
            return result;
        }

        /// <summary>
        /// Windows NT/2000/XP　でシャットダウンするには、
        /// SE_SHUTDOWN_NAME 特権が必要なので、その特権をセットする。
        /// </summary>
        private static void SetShutdownPrivilege()
        {
            const int TOKEN_QUERY = 0x00000008;
            const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
            const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
            const int SE_PRIVILEGE_ENABLED = 0x00000002;

            // プロセスのハンドルを取得する。
            IntPtr hproc = System.Diagnostics.Process.GetCurrentProcess().Handle;
            // IntPtr hproc = GetCurrentProcess(); // この方法でもＯＫ．

            // Token を取得する。
            IntPtr hToken = IntPtr.Zero;
            if (!OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken))
                throw new Exception("OpenProcessToken");

            // LUID を取得する。
            long luid = 0;
            if (!LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref luid))
                throw new Exception("LookupPrivilegeValue");

            // 特権を設定する。
            TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES();
            tp.PrivilegeCount = 1;
            tp.Privileges = new LUID_AND_ATTRIBUTES();
            tp.Privileges.Luid = luid;
            tp.Privileges.Attributes = SE_PRIVILEGE_ENABLED;

            // 特権をセットする。
            if (!AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
                throw new Exception("AdjustTokenPrivileges");
        }


        //////////////////////////////////////////////////////
        // Windows Me 以前の場合
        //////////////////////////////////////////////////////

        private static void KillExplorer()
        {
            // Windows Me 以前であれば、EWX_FORCE フラグを指定して ExitWindowsEx() をコールすると、
            // シェルの仕様により、ログオフに失敗します。
            // プログラムによって、ユーザーを強制的にログオフさせるには、必ずExplorer のプロセスを終了させ、
            // それから、EWX_LOGOFF および EWX_FORCE フラグを指定して、
            // ExitWindowsEx() をコールする必要があります。
            if (System.Environment.OSVersion.Platform == PlatformID.Win32Windows)
            {
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.ProcessName.StartsWith("explorer"))
                        process.Kill();
                }
            }
        }
    }
}
