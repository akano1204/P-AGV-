using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BelicsClass.ConsoleTools
{
    /// <summary>
    /// コンソール画面クラス
    /// </summary>
    public sealed class BL_ConsoleTools
    {
        private BL_ConsoleTools()
        {
        }

        #region Win32API
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetConsoleTitle(string titleName);

        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr handle, bool resetFlag);

        [DllImport("user32", SetLastError = true)]
        private static extern bool DeleteMenu(IntPtr handle, uint menuItem, uint menuFlag);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int deviceName);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool GetConsoleCursorInfo(IntPtr handle, out CONSOLE_CURSOR_INFO pcci);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetConsoleCursorInfo(IntPtr handle, ref CONSOLE_CURSOR_INFO pcci);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferSize(IntPtr handle, COORD coord);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetConsoleWindowInfo(IntPtr handle, bool absolute, ref SMALL_RECT windowRect);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetConsoleCursorPosition(IntPtr handle, COORD coord);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetConsoleTextAttribute(IntPtr handle, short textAttribute);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfo(IntPtr handle, out CONSOLE_SCREEN_BUFFER_INFO pcsb);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FillConsoleOutputCharacter(IntPtr handle, char fillChar, int charCell, COORD coordChar, out int writtenCount);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FillConsoleOutputAttribute(IntPtr handle, short fillAttribute, int charCell, COORD coordAttr, out int writtenCount);

        [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int _kbhit();

        [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int _getch();

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_CURSOR_INFO
        {
            public int dwSize;
            public bool bVisible;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        };

        private const int STD_OUTPUT_HANDLE = -11;

        private const int FOREGROUND_BLUE = 0x0001;
        private const int FOREGROUND_GREEN = 0x0002;
        private const int FOREGROUND_RED = 0x0004;
        private const int FOREGROUND_INTENSITY = 0x0008;
        private const int BACKGROUND_BLUE = 0x0010;
        private const int BACKGROUND_GREEN = 0x0020;
        private const int BACKGROUND_RED = 0x0040;
        private const int BACKGROUND_INTENSITY = 0x0080;
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
            ForegroundBlue = FOREGROUND_BLUE | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            ForegroundGreen = FOREGROUND_GREEN | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            ForegroundRed = FOREGROUND_RED | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            BackgroundBlue = BACKGROUND_BLUE | BACKGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            BackgroundGreen = BACKGROUND_GREEN | BACKGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            BackgroundRed = BACKGROUND_RED | BACKGROUND_INTENSITY,
        }

        /// <summary>
        /// 色テーブル
        /// </summary>
        public enum Table : short
        {
            /// <summary>
            /// 
            /// </summary>
            Black = 0,
            /// <summary>
            /// 
            /// </summary>
            Blue = FOREGROUND_BLUE | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            Green = FOREGROUND_GREEN | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            Cyan = FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            Red = FOREGROUND_RED | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            Magenta = FOREGROUND_BLUE | FOREGROUND_RED | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            Yellow = FOREGROUND_GREEN | FOREGROUND_RED | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            White = FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_RED | FOREGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            BlueBack = BACKGROUND_BLUE | BACKGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            GreenBack = BACKGROUND_GREEN | BACKGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            CyanBack = BACKGROUND_BLUE | BACKGROUND_GREEN | BACKGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            RedBack = BACKGROUND_RED | BACKGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            MagentaBack = BACKGROUND_BLUE | BACKGROUND_RED | BACKGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            YellowBack = BACKGROUND_GREEN | BACKGROUND_RED | BACKGROUND_INTENSITY,
            /// <summary>
            /// 
            /// </summary>
            WhiteBack = BACKGROUND_BLUE | BACKGROUND_GREEN | BACKGROUND_RED | BACKGROUND_INTENSITY,
        }
        #endregion

        /// <summary>
        /// コンソール画面のタイトルを設定します。
        /// </summary>
        /// <param name="titleName">タイトルが格納されている文字列。</param>
        /// <returns></returns>
        public static bool SetTitle(string titleName)
        {
            return SetConsoleTitle(titleName);
        }

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
        /// コンソール画面のサイズを設定します。
        /// </summary>
        /// <param name="x">水平方向。</param>
        /// <param name="y">垂直方向。</param>
        public static void WindowSize(int x, int y)
        {
            COORD coord = new COORD();
            SMALL_RECT rect = new SMALL_RECT();

            coord.X = (short)x;
            coord.Y = (short)y;

            SetConsoleScreenBufferSize(GetStdHandle(STD_OUTPUT_HANDLE), coord);

            rect.Left = 0;
            rect.Top = 0;
            rect.Right = (short)(x - 1);
            rect.Bottom = (short)(y - 1);

            SetConsoleWindowInfo(GetStdHandle(STD_OUTPUT_HANDLE), true, ref rect);
        }

        /// <summary>
        /// コンソール画面を初期化します。
        /// </summary>
        public static void ScreenClear()
        {
            CONSOLE_SCREEN_BUFFER_INFO info = new CONSOLE_SCREEN_BUFFER_INFO();
            COORD coord = new COORD();
            IntPtr handle;
            int length, write_count;

            handle = GetStdHandle(STD_OUTPUT_HANDLE);

            GetConsoleScreenBufferInfo(handle, out info);

            length = info.dwSize.X * info.dwSize.Y;
            coord.X = 0;
            coord.Y = 0;

            FillConsoleOutputCharacter(handle, ' ', length, coord, out write_count);
            FillConsoleOutputAttribute(handle, (short)Table.White, length, coord, out write_count);
        }

        /// <summary>
        /// カーソルの表示／非表示を設定します。
        /// </summary>
        public static bool CursorVisible
        {
            set
            {
                CONSOLE_CURSOR_INFO info = new CONSOLE_CURSOR_INFO();
                IntPtr handle;

                handle = GetStdHandle(STD_OUTPUT_HANDLE);

                GetConsoleCursorInfo(handle, out info);

                info.bVisible = value;

                SetConsoleCursorInfo(handle, ref info);
            }
        }

        /// <summary>
        /// カーソルの位置を設定します。
        /// </summary>
        /// <param name="x">水平座標。</param>
        /// <param name="y">垂直座標。</param>
        public static void Locate(int x, int y)
        {
            COORD pos = new COORD();

            pos.X = (short)x;
            pos.Y = (short)y;

            SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), pos);
        }

        /// <summary>
        /// 指定した位置に文字列を書き込みます。
        /// </summary>
        /// <param name="x">水平座標。</param>
        /// <param name="y">垂直座標。</param>
        /// <param name="data">書き込む文字列。</param>
        public static void LocateWrite(int x, int y, string data)
        {
            COORD pos = new COORD();

            pos.X = (short)x;
            pos.Y = (short)y;

            SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), pos);

            Console.Write(data);
        }

        /// <summary>
        /// カーソルの色を設定します。
        /// </summary>
        /// <param name="rgb">色属性。</param>
        public static void Color(RGB rgb)
        {
            SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), (short)rgb);
        }
        /// <summary>
        /// カーソルの色を設定します。
        /// </summary>
        /// <param name="colorTable">色テーブル。</param>
        public static void Color(Table colorTable)
        {
            SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), (short)colorTable);
        }

        /// <summary>
        /// コンソールの入力をチェックします。
        /// </summary>
        /// <returns></returns>
        public static int KeyDown()
        {
            return (_kbhit() == 0 ? 0 : _getch());
        }
    }
}
