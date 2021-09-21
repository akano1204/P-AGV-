using System;
using System.ComponentModel;
using System.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace BelicsClass.Common
{
    /// <summary>
    /// Windows ユーザは、.NET Framework では WindowsIdentity クラスで表します。このクラスのメンバに、他のユーザに偽装する Impersonate メソッドがあります。
    /// しかし、ログインするという行為は、.NET Frameowork の外にある概念です。したがって、.NET Framwework では、ユーザ名とパスワードを確認するような仕組みは提供されていません。なので、Platform Invoke（プラットフォーム呼び出し P/Invoke）を行います。
    /// 今回使用するのは、LogonUser 関数です。第２引数で指定したドメインに属する、第１引数で指定したユーザに、第３引数で指定したパスワードを使ってログインを試みます。ローカル ユーザに偽装する場合、第２引数にはマシン名を入力します。
    /// 偽装を解除するには、偽装操作を行う前の Windows ユーザーを表す、WindowsImpersonationContext クラスのメンバ メソッドである、Undo メソッドを使います。
    /// なお、第４引数で指定した、LOGON32_LOGON_INTERACTIVE は、デスクトップとの対話処理をするために、偽装するユーザのプロファイルを読み込みます。これにより、レジストリの HKCU ハイブを操作したり、画面のあるプロセスを起動することが出来るようになります。これはまた、セキュリティ上のリスクともなり得ますので、第４引数の指定はよく注意しておこなってください。
    /// 一連の流れを説明します。
    ///  1.
    /// 現在偽装していないことを確認する（RevertToSelf）
    ///  2.
    /// 偽装するユーザの、ユーザ名とパスワードを確認する（LogonUser）
    ///  3.
    /// 現在ログイン中のユーザのコンテキストを複製する（DuplicateToken）
    ///  4.
    /// 偽装したユーザで処理を行う
    ///  5.
    /// 偽装を解除する（Undo）
    /// このうち、「偽装したユーザで処理を行う」以外は、定型処理になります。今回紹介する Impersonate クラスは、定型処理を１つのクラスとしてまとめました（Facade パターン？）。
    /// Impersonate クラスは IDisposable インターフェイスを実装します。ここで Dispose メソッドは、「偽装を解除する」処理を行います。
    /// ---使い方---
    /// Impersonate impersonate = null;
    /// try {
    ///     impersonate = Impersonate.ImpersonateValidUser("user", "domain", "password");
    ///     // 何らかの処理
    /// } finally {
    ///     if (impersonate != null) impersonate.Dispose();
    /// }
    /// </summary>
    public class BL_Impersonate : IDisposable
    {
        // これは、enum で定義するべき。。。
        private static int LOGON32_LOGON_INTERACTIVE = 2;
        private static int LOGON32_PROVIDER_DEFAULT = 0;
        private WindowsImpersonationContext impersonationContext;
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );
        [DllImport("advapi32.dll", SetLastError = true)]
        private extern static bool DuplicateToken(
            IntPtr ExistingTokenHandle,
            int SECURITY_IMPERSONATION_LEVEL,
            out IntPtr DuplicateTokenHandle
            );
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool RevertToSelf();
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
        private BL_Impersonate(WindowsIdentity tempWindowsIdentity)
        {
            impersonationContext = tempWindowsIdentity.Impersonate();
        }

        /// <summary>
        /// 別ユーザーの権限での処理を開始する
        /// Users権限しかないユーザーでログイン中に、Administrators権限が必要な処理を行う場合などに利用してください
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="domain"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static BL_Impersonate ImpersonateValidUser(
            string userName,
            string domain,
            string password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            BL_Impersonate retValue = null;
            try
            {
                if (RevertToSelf() == true)
                {
                    if (LogonUser(userName, domain, password,
                        LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out token) == true)
                    {
                        if (DuplicateToken(token, 2, out tokenDuplicate) == true)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            retValue = new BL_Impersonate(tempWindowsIdentity);
                            if (retValue.impersonationContext == null)
                            {
                                retValue = null;
                            }
                        }
                    }
                }
                return retValue;
            }
            finally
            {
                // try - finally は２重にするべき。。。
                if (!tokenDuplicate.Equals(IntPtr.Zero))
                {
                    CloseHandle(tokenDuplicate);
                }
                if (!token.Equals(IntPtr.Zero))
                {
                    CloseHandle(token);
                }
            }
        }

        /// <summary>
        /// 別ユーザーの権限での処理を終了する
        /// </summary>
        public virtual void Dispose()
        {
            if (impersonationContext != null)
            {
                impersonationContext.Undo();
                impersonationContext = null;
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~BL_Impersonate()
        {
            this.Dispose();
        }
    }
}
