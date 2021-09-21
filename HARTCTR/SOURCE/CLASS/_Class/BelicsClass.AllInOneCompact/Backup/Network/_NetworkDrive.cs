using BelicsClass.Common;

namespace BelicsClass.Network
{
    /// <summary>
    /// ネットワークドライブのマウントを制御するクラス
    /// </summary>
    public class BL_NetworkDrive
    {
        /// <summary>
        /// ネットワークドライブを割り当てます。
        /// </summary>
        /// <param name="LocalName">ドライブ名</param>
        /// <param name="RemoteName">ネットワーク名</param>
        /// <param name="Username">ユーザー名</param>
        /// <param name="Password">パスワード</param>
        public static uint NetAdd(string LocalName, string RemoteName, string Username, string Password)
        {
            BL_Win32API.NETRESOURCE net = new BL_Win32API.NETRESOURCE();

            net.dwScope = BL_Win32API.RESOURCE_GLOBALNET;
            net.dwType = BL_Win32API.RESOURCETYPE_DISK;
            net.dwDisplayType = BL_Win32API.RESOURCEDISPLAYTYPE_GENERIC;
            net.dwUsage = BL_Win32API.RESOURCEUSAGE_CONNECTABLE;
            net.lpLocalName = LocalName;
            net.lpRemoteName = RemoteName;
            net.lpComment = null;
            net.lpProvider = null;

            return BL_Win32API.API_WNetAddConnection2(ref net, Password, Username, 0);
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
            return BL_Win32API.API_WNetCancelConnection2(LocalName, 0, false);
        }
    }
}
