using System;
using System.IO;

namespace BelicsClass.Common
{
    /// <summary>
    /// サウンドを操作するクラス
    /// </summary>
    public class BL_Sound
    {
        /// <summary>
        /// サウンドを再生します。
        /// </summary>
        /// <param name="file_name">ファイル名を格納した文字列。</param>
        public static void PlaySound(string file_name)
        {
            BL_Win32API.API_PlaySound(file_name, IntPtr.Zero, BL_Win32API.SND_ASYNC | BL_Win32API.SND_FILENAME);
        }

        /// <summary>
        /// サウンドを再生します。
        /// </summary>
        /// <param name="stream">アセンブリ内の指定されたマニフェストリソース。</param>
        public static void PlaySound(Stream stream)
        {
            byte[] byte_data = new byte[stream.Length];

            stream.Read(byte_data, 0, (int)stream.Length);

            BL_Win32API.API_PlaySound(byte_data, IntPtr.Zero, BL_Win32API.SND_ASYNC | BL_Win32API.SND_MEMORY);
        }

        /// <summary>
        /// サウンドを連続再生します。
        /// </summary>
        /// <param name="file_name">ファイル名を格納した文字列。</param>
        public static void PlaySoundEndless(string file_name)
        {
            BL_Win32API.API_PlaySound(file_name, IntPtr.Zero, BL_Win32API.SND_ASYNC | BL_Win32API.SND_FILENAME | BL_Win32API.SND_LOOP);
        }

        /// <summary>
        /// サウンドを連続再生します。
        /// </summary>
        /// <param name="stream">アセンブリ内の指定されたマニフェストリソース。</param>
        public static void PlaySoundEndless(Stream stream)
        {
            byte[] byte_data = new byte[stream.Length];

            stream.Read(byte_data, 0, (int)stream.Length);

            BL_Win32API.API_PlaySound(byte_data, IntPtr.Zero, BL_Win32API.SND_ASYNC | BL_Win32API.SND_MEMORY | BL_Win32API.SND_LOOP);
        }

        /// <summary>
        /// サウンドを停止します。
        /// </summary>
        public static void PlaySoundStop()
        {
            BL_Win32API.API_PlaySound(IntPtr.Zero, IntPtr.Zero, 0);
        }

        /// <summary>
        /// ビープ音を鳴らします。
        /// </summary>
        public static void Beep()
        {
            BL_Win32API.API_MessageBeep(0xFFFFFFFF);
        }

        /// <summary>
        /// ビープ音を鳴らします。
        /// </summary>
        /// <param name="frequency">ビープ音の周波数。 (ヘルツ単位)</param>
        /// <param name="duration">ビープ音の周期。 (ミリ秒)</param>
        public static void Beep(int frequency, int duration)
        {
            BL_Win32API.API_Beep((uint)frequency, (uint)duration);
        }
    }
}
