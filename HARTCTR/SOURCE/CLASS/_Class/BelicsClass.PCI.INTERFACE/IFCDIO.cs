using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace InterfaceCorpDllWrap
{
	public class IFCDIO_ANY
	{
		private IFCDIO_ANY(){}

		// -----------------------------------------------------------------------
		//	Symbols and/or identifiers
		// -----------------------------------------------------------------------
		public const uint	FBIDIO_RSTIN_MASK	= 1;		// The symbols used when carrying out the mask of the RSTIN
		public const uint	FBIDIO_FLAG_SHARE	= 0x0002;	// The flag is applicable to the DioOpen function. This flag shows that the device is opened as shareable.

		public const int	FBIDIO_IN1_8	= 0;	// Read the data from IN1 through IN8.
		public const int	FBIDIO_IN9_16	= 1;	// Read the data from IN9 through IN16.
		public const int	FBIDIO_IN17_24	= 2;	// Read the data from IN17 through IN24.
		public const int	FBIDIO_IN25_32	= 3;	// Read the data from IN25 through IN32.
		public const int	FBIDIO_IN33_40	= 4;	// Read the data from IN33 through IN40.
		public const int	FBIDIO_IN41_48	= 5;	// Read the data from IN41 through IN48.
		public const int	FBIDIO_IN49_56	= 6;	// Read the data from IN49 through IN56.
		public const int	FBIDIO_IN57_64	= 7;	// Read the data from IN57 through IN64.


		public const int	FBIDIO_IN1_16	= 0;	// Read the data from IN1 through IN16.
		public const int	FBIDIO_IN17_32	= 2;	// Read the data from IN17 through IN32.
		public const int	FBIDIO_IN33_48	= 4;	// Read the data from IN33 through IN48.
		public const int	FBIDIO_IN49_64	= 6;	// Read the data from IN49 through IN64.

		public const int	FBIDIO_IN1_32	= 0;	// Read the data from IN1 through IN32.
		public const int	FBIDIO_IN33_64	= 4;	// Read the data from IN33 through IN64.

		public const int	FBIDIO_OUT1_8	= 0;	// Write the data to OUT1 through OUT8.
		public const int	FBIDIO_OUT9_16	= 1;	// Write the data to OUT9 through OUT16.
		public const int	FBIDIO_OUT17_24	= 2;	// Write the data to OUT17 through OUT24.
		public const int	FBIDIO_OUT25_32	= 3;	// Write the data to OUT25 through OUT32.
		public const int	FBIDIO_OUT33_40	= 4;	// Write the data to OUT33 through OUT40.
		public const int	FBIDIO_OUT41_48	= 5;	// Write the data to OUT41 through OUT48.
		public const int	FBIDIO_OUT49_56	= 6;	// Write the data to OUT49 through OUT56.
		public const int	FBIDIO_OUT57_64	= 7;	// Write the data to OUT57 through OUT64.


		public const int	FBIDIO_OUT1_16	= 0;	// Write the data to OUT1 through OUT16.
		public const int	FBIDIO_OUT17_32	= 2;	// Write the data to OUT17 through OUT32.
		public const int	FBIDIO_OUT33_48	= 4;	// Write the data to OUT33 through OUT48.
		public const int	FBIDIO_OUT49_64	= 6;	// Write the data to OUT49 through OUT64.

		public const int	FBIDIO_OUT1_32	= 0;	// Write the data to OUT1 through OUT32.
		public const int	FBIDIO_OUT33_64	= 4;	// Write the data to OUT33 through OUT64.

		public const uint	FBIDIO_STB1_ENABLE	=	0x01;	// The STB1 event is eabled.
		public const uint	FBIDIO_STB1_HIGH_EDGE	=	0x10;	// The rising edge for STB1 is enabled.

		public const uint	FBIDIO_ACK2_ENABLE	=	0x04;	// The ACK2 event is enabled.
		public const uint	FBIDIO_ACK2_HIGH_EDGE	=	0x40;	// The rising edge for ACK2 is enabled.

		public const uint	FBIDIO_WAIT_LAST_ACK2	=	0x100;	// Complete the function by the final ACK2.

		public const uint	FBIDIO_STB2_DELAY_10US	=	0x000A0000;	// The STB2 delay time.(10us)
		public const uint	FBIDIO_STB2_DELAY_30US	=	0x001E0000;	// The STB2 delay time.(30us)
		public const uint	FBIDIO_STB2_DELAY_50US	=	0x00320000;	// The STB2 delay time.(50us)
		public const uint	FBIDIO_STB2_DELAY_100US	=	0x00640000;	// The STB2 delay time.(100us)
		public const uint	FBIDIO_STB2_DELAY_300US	=	0x012C0000;	// The STB2 delay time.(300us)
		public const uint	FBIDIO_STB2_DELAY_500US	=	0x01F40000;	// The STB2 delay time.(500us)
		public const uint	FBIDIO_STB2_DELAY_1MS	=	0x03E80000;	// The STB2 delay time.(1ms)
		public const uint	FBIDIO_STB2_DELAY_3MS	=	0x0BB80000;	// The STB2 delay time.(3ms)
		public const uint	FBIDIO_STB2_DELAY_5MS	=	0x13880000;	// The STB2 delay time.(5ms)
		public const uint	FBIDIO_STB2_DELAY_10MS	=	0x27100000;	// The STB2 delay time.(10ms)
		public const uint	FBIDIO_STB2_DELAY_30MS	=	0x75300000;	// The STB2 delay time.(30ms)
		public const uint	FBIDIO_STB2_DELAY_50MS	=	0xC3500000;	// The STB2 delay time.(50ms)
		
		public const uint	FBIDIO_IRIN1_2_STB1 = 32;	// IR.IN1, IR.IN2, STB1
		
		public const uint	FBIDIO_NO_BUFFER_MODE = 0x00000001;
		
		public const uint	FBIDIO_SYNC1 = 0;
		public const uint	FBIDIO_SYNC2 = 1;
		
		public const uint	FBIDIO_SYNC_DISABLE = 0;
		public const uint	FBIDIO_SYNC_ENABLE  = 1;
		
		// -----------------------------------------------------------------------
		//	Return Value
		// -----------------------------------------------------------------------
		public const uint	FBIDIO_ERROR_SUCCESS				=	0;			// The process was successfully completed.
		public const uint	FBIDIO_ERROR_NOT_DEVICE				=	0xC0000001;	// The specified driver cannot be called.
		public const uint	FBIDIO_ERROR_NOT_OPEN				=	0xC0000002;	// The specified driver cannot be opened.
		public const uint	FBIDIO_ERROR_INVALID_HANDLE			=	0xC0000003;	// The device handle is invalid.
		public const uint	FBIDIO_ERROR_ALREADY_OPEN			= 	0xC0000004;	// The device has already been opened.
		public const uint	FBIDIO_ERROR_HANDLE_EOF				=	0xC0000005;	// End of file is reached.
		public const uint	FBIDIO_ERROR_MORE_DATA				=	0xC0000006;	// More available data exists.
		public const uint	FBIDIO_ERROR_INSUFFICIENT_BUFFER	= 	0xC0000007;	// The data area passed to the system call is too small.
		public const uint	FBIDIO_ERROR_IO_PENDING				= 	0xC0000008;	// Overlapped I/O operations are in progress.
		public const uint	FBIDIO_ERROR_NOT_SUPPORTED			= 	0xC0000009;	// The specified function is not supported.
		public const uint	FBIDIO_ERROR_MEMORY_NOTALLOCATED	= 	0xC0001000;	// Not enough memory.
		public const uint	FBIDIO_ERROR_PARAMETER				= 	0xC0001001;	// The specified parameter is invalid.
		public const uint	FBIDIO_ERROR_INVALID_CALL			= 	0xC0001002;	// Invalid function is called.
		public const uint	FBIDIO_ERROR_DRVCAL					=	0xC0001003;	// Failed to call the driver.
		public const uint	FBIDIO_ERROR_NULL_POINTER			= 	0xC0001004;	// A NULL pointer is passed between the driver and the DLL.
		public const uint	FBIDIO_ERROR_IO_INCOMPLETE			= 	0xC0001005;	// I/O event is not signaled asynchronously.

        public const uint	FBIDIO_ERROR_UNSAFE_USE				=	0xC0001006;

		// -----------------------------------------------------------------------
		//	Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct OVERLAPPED
		{
			public IntPtr Internal;
			public IntPtr InternalHigh;
			public int offset;
			public int OffsetHigh;
			public IntPtr hEvent;
		}

		public delegate void LPOVERLAPPED_COMPLETION_ROUTINE(
			uint dwErrorCode,					// completion code
			uint dwNumberOfBytesTransfered,		// number of bytes transferred
			ref OVERLAPPED lpOverlapped			// I/O information buffer
			);

		// -----------------------------------------------------------------------
		//	DllImport API
		// -----------------------------------------------------------------------
		[DllImport("fbidio.dll")]
		public static extern	IntPtr	DioOpen( string lpszName, uint fdwFlags );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioClose( IntPtr hDeviceHandle );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPoint( IntPtr hDeviceHandle, int[] pBuffer, uint dwStartNum, uint dwNum );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPoint( IntPtr hDeviceHandle, out int pBuffer, uint dwStartNum, uint dwNum );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPoint( IntPtr hDeviceHandle, int[] pBuffer, uint dwStartNum, uint dwNum );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPoint( IntPtr hDeviceHandle, ref int pBuffer, uint dwStartNum, uint dwNum );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetBackGroundUseTimer( IntPtr hDeviceHandle, out int pnUse );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetBackGroundUseTimer( IntPtr hDeviceHandle, int nUse);
		[DllImport("fbidio.dll")]
		public static extern	IntPtr	DioSetBackGround( IntPtr hDeviceHandle, uint dwStartPoint, uint dwPointNum, uint dwValueNum, uint dwCycle, uint dwCount, uint dwOption );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioFreeBackGround( IntPtr hDeviceHandle, IntPtr hBackGroundHandle );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioStopBackGround( IntPtr hDeviceHandle, IntPtr hBackGroundHandle );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetBackGroundStatus( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, out int pnStartPoint, out int pnPointNum, out int pnValueNum, out int pnCycle, out int pnCount, out int pnOption, out int pnExecute, out int pnExecCount, out int pnBufferOffset, out int pnOver );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, out int pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, IntPtr pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, out int pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, IntPtr pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, ref int pBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, IntPtr pBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, ref int pBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, IntPtr pBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, out int pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, IntPtr pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, out int pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( IntPtr hDeviceHandle, IntPtr hBackGroundHandle, IntPtr pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetInputHandShakeConfig( IntPtr hDeviceHandle, out int pnInputHandShakeConfig, out uint pdwBitMask1, out uint pdwBitMask2 );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetInputHandShakeConfig( IntPtr hDeviceHandle, int nInputHandShakeConfig, uint dwBitMask1, uint dwBitMask2 );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOutputHandShakeConfig( IntPtr hDeviceHandle, out int pnOutputHandShakeConfig, out int pdwBitMask1, out int pdwBitMask2 );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetOutputHandShakeConfig( IntPtr hDeviceHandle, int nOutputHandShakeConfig, uint dwBitMask1, uint dwBitMask2 );

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            lpNumOfbytesRead = 0; 
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            lpNumOfbytesRead = 0; 
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake(hDeviceHandle, out lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                lpNumOfbytesRead = 0; 
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake(hDeviceHandle, out lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped);
            }
            else
            {
                lpBuffer = 0;
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            {
                if (lpOverlapped.Equals(IntPtr.Zero))
                {
                    return IFCDIO_IMPORT.DioInputHandShake(hDeviceHandle, lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped);
                }
                else
                {
                    lpNumOfbytesRead = 0;
                    return FBIDIO_ERROR_UNSAFE_USE;
                }
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake(hDeviceHandle, lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped);
            }
            else
            {
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake(hDeviceHandle, lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped);
            }
            else
            {
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake(hDeviceHandle, lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped);
            }
            else
            {
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
		[DllImport("fbidio.dll")]
		public static extern uint DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
		
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
		[DllImport("fbidio.dll")]
		public static extern uint DioInputHandShakeEx( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
		
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
		
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
		
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
		
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
		[DllImport("fbidio.dll")]
		public static extern uint DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );


        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
		[DllImport("fbidio.dll")]
		public static extern uint DioOutputHandShakeEx( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioStopInputHandShake( IntPtr hDeviceHandle );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioStopOutputHandShake( IntPtr hDeviceHandle );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetHandShakeStatus( IntPtr hDeviceHandle, out uint pdwDeviceStatus, out uint pdwInputedBuffNum, out uint pdwOutputedBuffNum );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputByte( IntPtr hDeviceHandle, int nNo, out byte pbValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputWord( IntPtr hDeviceHandle, int nNo, out ushort pwValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputDword( IntPtr hDeviceHandle, int nNo, out uint pdwValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputByte( IntPtr hDeviceHandle, int nNo, byte bValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputWord( IntPtr hDeviceHandle, int nNo, ushort wValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputDword( IntPtr hDeviceHandle, int nNo, uint dwValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetAckStatus( IntPtr hDeviceHandle, out byte pbAckStatus );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetAckPulseCommand( IntPtr hDeviceHandle, byte bCommand );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetStbStatus( IntPtr hDeviceHandle, out byte pbStbStatus );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetStbPulseCommand( IntPtr hDeviceHandle, byte bCommand );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputUniversalPoint( IntPtr hDeviceHandle, out uint pdwUniversalPoint );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputUniversalPoint( IntPtr hDeviceHandle, uint dwUniversalPoint );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetTimeOut( IntPtr hDeviceHandle, uint dwInputTotalTimeout, uint dwInputIntervalTimeout, uint dwOutputTotalTimeout, uint dwOutputIntervalTimeout );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetTimeOut( IntPtr hDeviceHandle, out uint pdwInputTotalTimeout, out uint pdwInputIntervalTimeout, out uint pdwOutputTotalTimeout, out uint pdwOutputIntervalTimeout );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetIrqMask( IntPtr hDeviceHandle, byte bIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetIrqMask( IntPtr hDeviceHandle, out byte pbIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetIrqConfig( IntPtr hDeviceHandle, byte bIrqConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetIrqConfig( IntPtr hDeviceHandle, out byte pbIrqConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetDeviceConfig( IntPtr hDeviceHandle, out uint pdwDeviceConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetTimerConfig( IntPtr hDeviceHandle, byte bTimerConfigValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetTimerConfig( IntPtr hDeviceHandle, out byte pbTimerConfigValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetTimerCount( IntPtr hDeviceHandle, out byte pbTimerCount );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetLatchStatus( IntPtr hDeviceHandle, byte bLatchStatus );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetLatchStatus( IntPtr hDeviceHandle, out byte pbLatchStatus );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetResetInStatus( IntPtr hDeviceHandle, out byte pbResetInStatus );

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
        public static uint DioEventRequestPending(IntPtr hDeviceHandle, uint dwEventEnableMask, out uint pEventBuf, ref OVERLAPPED lpOverlapped)
        {
            pEventBuf = 0;
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
        public static uint DioEventRequestPending(IntPtr hDeviceHandle, uint dwEventEnableMask, out uint pEventBuf, IntPtr lpOverlapped)
        {
            pEventBuf = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, uint[] pEventBuf, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, uint[] pEventBuf, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioEventRequestPending(hDeviceHandle, dwEventEnableMask, pEventBuf, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]	
		public static uint	DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, IntPtr pEventBuf, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        [DllImport("fbidio.dll")]
		public static extern	uint	DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, IntPtr pEventBuf, IntPtr lpOverlapped );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetDeviceDesc( IntPtr hDeviceHandle, string pDeviceDesc, out int pLen );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOverlappedResult( IntPtr hDeviceHandle, ref OVERLAPPED lpOverlapped, out uint lpNumberOfbytes, bool bWait );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOverlappedResult( IntPtr hDeviceHandle, IntPtr lpOverlapped, out uint lpNumberOfbytes, bool bWait );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOverlappedResult( IntPtr hDeviceHandle, ref OVERLAPPED lpOverlapped, uint[] lpNumberOfbytes, bool bWait );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOverlappedResult( IntPtr hDeviceHandle, IntPtr lpOverlapped, uint[] lpNumberOfbytes, bool bWait );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOverlappedResult( IntPtr hDeviceHandle, ref OVERLAPPED lpOverlapped, IntPtr lpNumberOfbytes, bool bWait );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOverlappedResult( IntPtr hDeviceHandle, IntPtr lpOverlapped, IntPtr lpNumberOfbytes, bool bWait );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetIrqMask( IntPtr hDeviceHandle, uint dwSetIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetIrqMask( IntPtr hDeviceHandle, out uint pdwGetIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetEdgeConfig( IntPtr hDeviceHandle, uint dwSetFallEdgeConfig, uint dwSetRiseEdgeConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetEdgeConfig ( IntPtr hDeviceHandle, out uint pdwGetFallEdgeConfig, out uint pdwGetRiseEdgeConfig );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputPoint( IntPtr hDeviceHandle, out int pBuffer, uint dwStartNum, uint dwNum );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputPoint( IntPtr hDeviceHandle, int[] pBuffer, uint dwStartNum, uint dwNum );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputByte( IntPtr hDeviceHandle, int nNo, out byte pbFallValue, out byte pbRiseValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputWord( IntPtr hDeviceHandle, int nNo, out ushort pwFallValue, out ushort pwRiseValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputDword( IntPtr hDeviceHandle, int nNo, out uint pdwFallValue, out uint pdwRiseValue );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetFilterConfig( IntPtr hDeviceHandle, int nNo, int nSetFilterConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetFilterConfig( IntPtr hDeviceHandle,int nNo, out int pnGetFilterConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetDeviceConfigEx( IntPtr hDeviceHandle, out uint pdwDeviceConfig, out uint pdwDeviceConfigEx );

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
        public static uint DioEventRequestPendingEx(IntPtr hDeviceHandle, uint[] pdwEventEnableMask, uint[] pEventBuf, ref OVERLAPPED lpOverlapped)
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
        public static uint DioEventRequestPendingEx(IntPtr hDeviceHandle, uint[] pdwEventEnableMask, uint[] pEventBuf, IntPtr lpOverlapped)
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
        public static uint DioEventRequestPendingEx(IntPtr hDeviceHandle, uint[] pdwEventEnableMask, IntPtr pEventBuf, ref OVERLAPPED lpOverlapped)
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        [DllImport("fbidio.dll")]
		public static extern	uint	DioEventRequestPendingEx( IntPtr hDeviceHandle, uint[] pdwEventEnableMask, IntPtr pEventBuf, IntPtr lpOverlapped );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioCommonGetPciDeviceInfo( IntPtr hDeviceHandle, out uint pdwDeviceID, out uint pdwVenderID,
																out uint pdwClassCode, out uint pdwRevisionID, out uint pdwBaseAddress0,
																out uint pdwBaseAddress1, out uint pdwBaseAddress2, out uint pdwBaseAddress3,
																out uint pdwBaseAddress4, out uint pdwBaseAddress5, out uint pdwSubsystemID,
																out uint pdwSubsystemVenderID, out uint pdwInterruptLine, out uint pdwBoardID );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetRstinMask( IntPtr hDeviceHandle, uint dwRstinMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetRstinMask( IntPtr hDeviceHandle, out uint pdwRstinMask );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetIrqMaskEx( IntPtr DeviceHandle, int No, uint SetIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetIrqMaskEx( IntPtr DeviceHandle, int No, out uint GetIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetEdgeConfigEx( IntPtr DeviceHandle, int No, uint SetFallEdgeConfig, uint SetRiseEdgeConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetEdgeConfigEx ( IntPtr DeviceHandle, int No, out uint GetFallEdgeConfig, out uint GetRiseEdgeConfig );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetEventRequestMode( IntPtr DeviceHandle, uint EventMode );
		
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputSync( IntPtr DeviceHandle, uint TrgLine, uint UpEdge, uint DownEdge );
		
        private class IFCDIO_IMPORT
        {
            [DllImport("fbidio.dll")]
            public static extern uint DioInputHandShake(IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped);
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( IntPtr hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
            [DllImport("fbidio.dll")]
    		public static extern	uint	DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, out uint pEventBuf, ref OVERLAPPED lpOverlapped );
    		[DllImport("fbidio.dll")]
    		public static extern	uint	DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, out uint pEventBuf, IntPtr lpOverlapped );
    		[DllImport("fbidio.dll")]
    		public static extern	uint	DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, uint[] pEventBuf, ref OVERLAPPED lpOverlapped );
    		[DllImport("fbidio.dll")]
    		public static extern	uint	DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, uint[] pEventBuf, IntPtr lpOverlapped );
    		[DllImport("fbidio.dll")]
    		public static extern	uint	DioEventRequestPending( IntPtr hDeviceHandle, uint dwEventEnableMask, IntPtr pEventBuf, ref OVERLAPPED lpOverlapped );
            [DllImport("fbidio.dll")]
            public static extern    uint    DioEventRequestPendingEx(IntPtr hDeviceHandle, uint[] pdwEventEnableMask, uint[] pEventBuf, ref OVERLAPPED lpOverlapped);
            [DllImport("fbidio.dll")]
            public static extern    uint    DioEventRequestPendingEx(IntPtr hDeviceHandle, uint[] pdwEventEnableMask, uint[] pEventBuf, IntPtr lpOverlapped);
            [DllImport("fbidio.dll")]
            public static extern    uint    DioEventRequestPendingEx(IntPtr hDeviceHandle, uint[] pdwEventEnableMask, IntPtr pEventBuf, ref OVERLAPPED lpOverlapped);
            [DllImport("fbidio.dll")]
            public static extern    uint    DioEventRequestPendingEx(IntPtr hDeviceHandle, uint[] pdwEventEnableMask, IntPtr pEventBuf, IntPtr lpOverlapped);
        }
	}

	public class IFCDIO
	{
		private IFCDIO(){}

		// -----------------------------------------------------------------------
		//	Symbols and/or identifiers
		// -----------------------------------------------------------------------
		public const uint	INVALID_HANDLE_VALUE = 0xFFFFFFFF;

		public const uint	FBIDIO_RSTIN_MASK	= 1;		// The symbols used when carrying out the mask of the RSTIN
		public const uint	FBIDIO_FLAG_SHARE	= 0x0002;	// The flag is applicable to the DioOpen function. This flag shows that the device is opened as shareable.

		public const int	FBIDIO_IN1_8	= 0;	// Read the data from IN1 through IN8.
		public const int	FBIDIO_IN9_16	= 1;	// Read the data from IN9 through IN16.
		public const int	FBIDIO_IN17_24	= 2;	// Read the data from IN17 through IN24.
		public const int	FBIDIO_IN25_32	= 3;	// Read the data from IN25 through IN32.
		public const int	FBIDIO_IN33_40	= 4;	// Read the data from IN33 through IN40.
		public const int	FBIDIO_IN41_48	= 5;	// Read the data from IN41 through IN48.
		public const int	FBIDIO_IN49_56	= 6;	// Read the data from IN49 through IN56.
		public const int	FBIDIO_IN57_64	= 7;	// Read the data from IN57 through IN64.


		public const int	FBIDIO_IN1_16	= 0;	// Read the data from IN1 through IN16.
		public const int	FBIDIO_IN17_32	= 2;	// Read the data from IN17 through IN32.
		public const int	FBIDIO_IN33_48	= 4;	// Read the data from IN33 through IN48.
		public const int	FBIDIO_IN49_64	= 6;	// Read the data from IN49 through IN64.

		public const int	FBIDIO_IN1_32	= 0;	// Read the data from IN1 through IN32.
		public const int	FBIDIO_IN33_64	= 4;	// Read the data from IN33 through IN64.

		public const int	FBIDIO_OUT1_8	= 0;	// Write the data to OUT1 through OUT8.
		public const int	FBIDIO_OUT9_16	= 1;	// Write the data to OUT9 through OUT16.
		public const int	FBIDIO_OUT17_24	= 2;	// Write the data to OUT17 through OUT24.
		public const int	FBIDIO_OUT25_32	= 3;	// Write the data to OUT25 through OUT32.
		public const int	FBIDIO_OUT33_40	= 4;	// Write the data to OUT33 through OUT40.
		public const int	FBIDIO_OUT41_48	= 5;	// Write the data to OUT41 through OUT48.
		public const int	FBIDIO_OUT49_56	= 6;	// Write the data to OUT49 through OUT56.
		public const int	FBIDIO_OUT57_64	= 7;	// Write the data to OUT57 through OUT64.


		public const int	FBIDIO_OUT1_16	= 0;	// Write the data to OUT1 through OUT16.
		public const int	FBIDIO_OUT17_32	= 2;	// Write the data to OUT17 through OUT32.
		public const int	FBIDIO_OUT33_48	= 4;	// Write the data to OUT33 through OUT48.
		public const int	FBIDIO_OUT49_64	= 6;	// Write the data to OUT49 through OUT64.

		public const int	FBIDIO_OUT1_32	= 0;	// Write the data to OUT1 through OUT32.
		public const int	FBIDIO_OUT33_64	= 4;	// Write the data to OUT33 through OUT64.

		public const uint	FBIDIO_STB1_ENABLE	=	0x01;	// The STB1 event is eabled.
		public const uint	FBIDIO_STB1_HIGH_EDGE	=	0x10;	// The rising edge for STB1 is enabled.

		public const uint	FBIDIO_ACK2_ENABLE	=	0x04;	// The ACK2 event is enabled.
		public const uint	FBIDIO_ACK2_HIGH_EDGE	=	0x40;	// The rising edge for ACK2 is enabled.

		public const uint	FBIDIO_WAIT_LAST_ACK2	=	0x100;	// Complete the function by the final ACK2.

		public const uint	FBIDIO_STB2_DELAY_10US	=	0x000A0000;	// The STB2 delay time.(10us)
		public const uint	FBIDIO_STB2_DELAY_30US	=	0x001E0000;	// The STB2 delay time.(30us)
		public const uint	FBIDIO_STB2_DELAY_50US	=	0x00320000;	// The STB2 delay time.(50us)
		public const uint	FBIDIO_STB2_DELAY_100US	=	0x00640000;	// The STB2 delay time.(100us)
		public const uint	FBIDIO_STB2_DELAY_300US	=	0x012C0000;	// The STB2 delay time.(300us)
		public const uint	FBIDIO_STB2_DELAY_500US	=	0x01F40000;	// The STB2 delay time.(500us)
		public const uint	FBIDIO_STB2_DELAY_1MS	=	0x03E80000;	// The STB2 delay time.(1ms)
		public const uint	FBIDIO_STB2_DELAY_3MS	=	0x0BB80000;	// The STB2 delay time.(3ms)
		public const uint	FBIDIO_STB2_DELAY_5MS	=	0x13880000;	// The STB2 delay time.(5ms)
		public const uint	FBIDIO_STB2_DELAY_10MS	=	0x27100000;	// The STB2 delay time.(10ms)
		public const uint	FBIDIO_STB2_DELAY_30MS	=	0x75300000;	// The STB2 delay time.(30ms)
		public const uint	FBIDIO_STB2_DELAY_50MS	=	0xC3500000;	// The STB2 delay time.(50ms)
		
		public const uint	FBIDIO_IRIN1_2_STB1 = 32;	// IR.IN1, IR.IN2, STB1
		
		public const uint	FBIDIO_NO_BUFFER_MODE = 0x00000001;
		
		public const uint	FBIDIO_SYNC1 = 0;
		public const uint	FBIDIO_SYNC2 = 1;
		
		public const uint	FBIDIO_SYNC_DISABLE = 0;
		public const uint	FBIDIO_SYNC_ENABLE  = 1;
		
		// -----------------------------------------------------------------------
		//	Return Value
		// -----------------------------------------------------------------------
		public const uint	FBIDIO_ERROR_SUCCESS				=	0;			// The process was successfully completed.
		public const uint	FBIDIO_ERROR_NOT_DEVICE				=	0xC0000001;	// The specified driver cannot be called.
		public const uint	FBIDIO_ERROR_NOT_OPEN				=	0xC0000002;	// The specified driver cannot be opened.
		public const uint	FBIDIO_ERROR_INVALID_HANDLE			=	0xC0000003;	// The device handle is invalid.
		public const uint	FBIDIO_ERROR_ALREADY_OPEN			= 	0xC0000004;	// The device has already been opened.
		public const uint	FBIDIO_ERROR_HANDLE_EOF				=	0xC0000005;	// End of file is reached.
		public const uint	FBIDIO_ERROR_MORE_DATA				=	0xC0000006;	// More available data exists.
		public const uint	FBIDIO_ERROR_INSUFFICIENT_BUFFER	= 	0xC0000007;	// The data area passed to the system call is too small.
		public const uint	FBIDIO_ERROR_IO_PENDING				= 	0xC0000008;	// Overlapped I/O operations are in progress.
		public const uint	FBIDIO_ERROR_NOT_SUPPORTED			= 	0xC0000009;	// The specified function is not supported.
		public const uint	FBIDIO_ERROR_MEMORY_NOTALLOCATED	= 	0xC0001000;	// Not enough memory.
		public const uint	FBIDIO_ERROR_PARAMETER				= 	0xC0001001;	// The specified parameter is invalid.
		public const uint	FBIDIO_ERROR_INVALID_CALL			= 	0xC0001002;	// Invalid function is called.
		public const uint	FBIDIO_ERROR_DRVCAL					=	0xC0001003;	// Failed to call the driver.
		public const uint	FBIDIO_ERROR_NULL_POINTER			= 	0xC0001004;	// A NULL pointer is passed between the driver and the DLL.
		public const uint	FBIDIO_ERROR_IO_INCOMPLETE			= 	0xC0001005;	// I/O event is not signaled asynchronously.

        public const uint	FBIDIO_ERROR_UNSAFE_USE				=	0xC0001006;

		// -----------------------------------------------------------------------
		//	Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct OVERLAPPED
		{
			public int Internal;
			public int InternalHigh;
			public int offset;
			public int OffsetHigh;
			public int hEvent;
		}

		public delegate void LPOVERLAPPED_COMPLETION_ROUTINE(
			uint dwErrorCode,					// completion code
			uint dwNumberOfBytesTransfered,		// number of bytes transferred
			ref OVERLAPPED lpOverlapped			// I/O information buffer
			);

		// -----------------------------------------------------------------------
		//	DllImport API
		// -----------------------------------------------------------------------
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOpen( string lpszName, uint fdwFlags );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioClose( uint hDeviceHandle );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPoint( uint hDeviceHandle, int[] pBuffer, uint dwStartNum, uint dwNum );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPoint( uint hDeviceHandle, out int pBuffer, uint dwStartNum, uint dwNum );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPoint( uint hDeviceHandle, int[] pBuffer, uint dwStartNum, uint dwNum );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPoint( uint hDeviceHandle, ref int pBuffer, uint dwStartNum, uint dwNum );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetBackGroundUseTimer( uint hDeviceHandle, out int pnUse );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetBackGroundUseTimer( uint hDeviceHandle, int nUse);
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetBackGround( uint hDeviceHandle, uint dwStartPoint, uint dwPointNum, uint dwValueNum, uint dwCycle, uint dwCount, uint dwOption );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioFreeBackGround( uint hDeviceHandle, uint hBackGroundHandle );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioStopBackGround( uint hDeviceHandle, uint hBackGroundHandle );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetBackGroundStatus( uint hDeviceHandle, uint hBackGroundHandle, out int pnStartPoint, out int pnPointNum, out int pnValueNum, out int pnCycle, out int pnCount, out int pnOption, out int pnExecute, out int pnExecCount, out int pnBufferOffset, out int pnOver);

		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( uint hDeviceHandle, uint hBackGroundHandle, out int pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( uint hDeviceHandle, uint hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( uint hDeviceHandle, uint hBackGroundHandle, out int pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputPointBack( uint hDeviceHandle, uint hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( uint hDeviceHandle, uint hBackGroundHandle, ref int pBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( uint hDeviceHandle, uint hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( uint hDeviceHandle, uint hBackGroundHandle, ref int pBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputPointBack( uint hDeviceHandle, uint hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( uint hDeviceHandle, uint hBackGroundHandle, out int pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( uint hDeviceHandle, uint hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( uint hDeviceHandle, uint hBackGroundHandle, out int pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioWatchPointBack( uint hDeviceHandle, uint hBackGroundHandle, int[] pBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetInputHandShakeConfig( uint hDeviceHandle, out int pnInputHandShakeConfig, out uint pdwBitMask1, out uint pdwBitMask2 );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetInputHandShakeConfig( uint hDeviceHandle, int nInputHandShakeConfig, uint dwBitMask1, uint dwBitMask2 );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOutputHandShakeConfig( uint hDeviceHandle, out int pnOutputHandShakeConfig, out int pdwBitMask1, out int pdwBitMask2 );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetOutputHandShakeConfig( uint hDeviceHandle, int nOutputHandShakeConfig, uint dwBitMask1, uint dwBitMask2 );

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesRead = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, out lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpNumOfbytesRead = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0; 
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0; 
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0; 
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped )
        {
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
		public static uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped )
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, out lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                lpBuffer = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioInputHandShake(uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioInputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToRead, lpNumOfbytesRead, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
		[DllImport("fbidio.dll")]
		public static extern uint DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
		
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0; 
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
            lpBuffer = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioInputHandShakeEx( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
		[DllImport("fbidio.dll")]
		public static extern uint DioInputHandShakeEx( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
            lpNumOfbytesWritten = 0;
            return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, out lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                lpNumOfbytesWritten = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake(hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped);
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped)
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
		
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
		
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
		
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
		
        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, ref lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioOutputHandShake(uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioOutputHandShake( hDeviceHandle, lpBuffer, nNumOfbytesToWrite, lpNumOfbytesWritten, lpOverlapped );
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }
        
		[DllImport("fbidio.dll")]
		public static extern uint DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );


        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
		public static uint	DioOutputHandShakeEx( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine )
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }
        
		[DllImport("fbidio.dll")]
		public static extern uint DioOutputHandShakeEx( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioStopInputHandShake( uint hDeviceHandle );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioStopOutputHandShake( uint hDeviceHandle );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetHandShakeStatus( uint hDeviceHandle, out uint pdwDeviceStatus, out uint pdwInputedBuffNum, out uint pdwOutputedBuffNum );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputByte( uint hDeviceHandle, int nNo, out byte pbValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputWord( uint hDeviceHandle, int nNo, out ushort pwValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputDword( uint hDeviceHandle, int nNo, out uint pdwValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputByte( uint hDeviceHandle, int nNo, byte bValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputWord( uint hDeviceHandle, int nNo, ushort wValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputDword( uint hDeviceHandle, int nNo, uint dwValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetAckStatus( uint hDeviceHandle, out byte pbAckStatus );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetAckPulseCommand( uint hDeviceHandle, byte bCommand );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetStbStatus( uint hDeviceHandle, out byte pbStbStatus );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetStbPulseCommand( uint hDeviceHandle, byte bCommand );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioInputUniversalPoint( uint hDeviceHandle, out uint pdwUniversalPoint );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputUniversalPoint( uint hDeviceHandle, uint dwUniversalPoint );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetTimeOut( uint hDeviceHandle, uint dwInputTotalTimeout, uint dwInputIntervalTimeout, uint dwOutputTotalTimeout, uint dwOutputIntervalTimeout );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetTimeOut( uint hDeviceHandle, out uint pdwInputTotalTimeout, out uint pdwInputIntervalTimeout, out uint pdwOutputTotalTimeout, out uint pdwOutputIntervalTimeout );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetIrqMask( uint hDeviceHandle, byte bIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetIrqMask( uint hDeviceHandle, out byte pbIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetIrqConfig( uint hDeviceHandle, byte bIrqConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetIrqConfig( uint hDeviceHandle, out byte pbIrqConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetDeviceConfig( uint hDeviceHandle, out uint pdwDeviceConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetTimerConfig( uint hDeviceHandle, byte bTimerConfigValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetTimerConfig( uint hDeviceHandle, out byte pbTimerConfigValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetTimerCount( uint hDeviceHandle, out byte pbTimerCount );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetLatchStatus( uint hDeviceHandle, byte bLatchStatus );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetLatchStatus( uint hDeviceHandle, out byte pbLatchStatus );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetResetInStatus( uint hDeviceHandle, out byte pbResetInStatus );

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
        public static uint DioEventRequestPending(uint hDeviceHandle, uint dwEventEnableMask, out uint pEventBuf, ref OVERLAPPED lpOverlapped)
        {
            pEventBuf = 0;
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioEventRequestPending(uint hDeviceHandle, uint dwEventEnableMask, out uint pEventBuf, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioEventRequestPending( hDeviceHandle, dwEventEnableMask, out pEventBuf, lpOverlapped);
            }
            else
            {
                pEventBuf = 0;
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        [DllImport("fbidio.dll")]
        public static extern    uint    DioEventRequestPending( uint hDeviceHandle, uint dwEventEnableMask, IntPtr pEventBuf, IntPtr lpOverlapped);
        
        [DllImport("fbidio.dll")]
		public static extern	uint	DioGetDeviceDesc( uint hDeviceHandle, string pDeviceDesc, out int pLen );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOverlappedResult( uint hDeviceHandle, ref OVERLAPPED lpOverlapped, out uint lpNumberOfbytes, bool bWait );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetOverlappedResult( uint hDeviceHandle, IntPtr lpOverlapped, out uint lpNumberOfbytes, bool bWait );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetIrqMask( uint hDeviceHandle, uint dwSetIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetIrqMask( uint hDeviceHandle, out uint pdwGetIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetEdgeConfig( uint hDeviceHandle, uint dwSetFallEdgeConfig, uint dwSetRiseEdgeConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetEdgeConfig ( uint hDeviceHandle, out uint pdwGetFallEdgeConfig, out uint pdwGetRiseEdgeConfig );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputPoint( uint hDeviceHandle, out int pBuffer,uint dwStartNum, uint dwNum );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputPoint( uint hDeviceHandle, int[] pBuffer,uint dwStartNum, uint dwNum );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputByte( uint hDeviceHandle, int nNo,out byte pbFallValue, out byte pbRiseValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputWord( uint hDeviceHandle, int nNo,out ushort pwFallValue, out ushort pwRiseValue );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintInputDword( uint hDeviceHandle, int nNo,out uint pdwFallValue, out uint pdwRiseValue );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetFilterConfig( uint hDeviceHandle, int nNo, int nSetFilterConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetFilterConfig( uint hDeviceHandle, int nNo, out int pnGetFilterConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetDeviceConfigEx( uint hDeviceHandle, out uint pdwDeviceConfig, out uint pdwDeviceConfigEx );

        /// <summary>
        /// This function or variable is unsafe.
        /// </summary>
        [Obsolete("This function or variable is unsafe. See help_net.pdf for details.", true)]
        public static uint DioEventRequestPendingEx(uint hDeviceHandle, uint[] pdwEventEnableMask, uint[] pEventBuf, ref OVERLAPPED lpOverlapped)
        {
        	return FBIDIO_ERROR_UNSAFE_USE;
        }

        /// <summary>
        /// This function or variable may be unsafe.
        /// </summary>
        [Obsolete("This function or variable may be unsafe. See help_net.pdf for details.")]
        public static uint DioEventRequestPendingEx(uint hDeviceHandle, uint[] pdwEventEnableMask, uint[] pEventBuf, IntPtr lpOverlapped)
        {
            if (lpOverlapped.Equals(IntPtr.Zero))
            {
                return IFCDIO_IMPORT.DioEventRequestPendingEx(hDeviceHandle, pdwEventEnableMask, pEventBuf, lpOverlapped);
            }
            else
            {
                return FBIDIO_ERROR_UNSAFE_USE;
            }
        }

        [DllImport("fbidio.dll")]
        public static extern uint DioEventRequestPendingEx(uint hDeviceHandle, uint[] pdwEventEnableMask, IntPtr pEventBuf, IntPtr lpOverlapped);

		[DllImport("fbidio.dll")]
		public static extern	uint	DioCommonGetPciDeviceInfo( uint hDeviceHandle, out uint pdwDeviceID, out uint pdwVenderID,
																out uint pdwClassCode, out uint pdwRevisionID, out uint pdwBaseAddress0,
																out uint pdwBaseAddress1, out uint pdwBaseAddress2, out uint pdwBaseAddress3,
																out uint pdwBaseAddress4, out uint pdwBaseAddress5, out uint pdwSubsystemID,
																out uint pdwSubsystemVenderID, out uint pdwInterruptLine, out uint pdwBoardID );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetRstinMask( uint hDeviceHandle, uint dwRstinMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioGetRstinMask( uint hDeviceHandle, out uint pdwRstinMask );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetIrqMaskEx( uint DeviceHandle, int No, uint SetIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetIrqMaskEx( uint DeviceHandle, int No, out uint GetIrqMask );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintSetEdgeConfigEx( uint DeviceHandle, int No, uint SetFallEdgeConfig, uint SetRiseEdgeConfig );
		[DllImport("fbidio.dll")]
		public static extern	uint	DioEintGetEdgeConfigEx ( uint DeviceHandle, int No, out uint GetFallEdgeConfig, out uint GetRiseEdgeConfig );

		[DllImport("fbidio.dll")]
		public static extern	uint	DioSetEventRequestMode( uint DeviceHandle, uint EventMode );
		
		[DllImport("fbidio.dll")]
		public static extern	uint	DioOutputSync( IntPtr DeviceHandle, uint TrgLine, uint UpEdge, uint DownEdge );
		
        private class IFCDIO_IMPORT
        {
            [DllImport("fbidio.dll")]
            public static extern    uint    DioInputHandShake(uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped);
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, out uint lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, uint[] lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpNumOfbytesRead, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, out byte lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, out ushort lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, out uint lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioInputHandShakeEx( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToRead, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, out uint lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, uint[] lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, ref OVERLAPPED lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShake( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpNumOfbytesWritten, IntPtr lpOverlapped );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, ref OVERLAPPED lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, ref byte lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, ref ushort lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, ref uint lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, byte[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, ushort[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, uint[] lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );
			[DllImport("fbidio.dll")]
			public static extern	uint	DioOutputHandShakeEx( uint hDeviceHandle, IntPtr lpBuffer, uint nNumOfbytesToWrite, IntPtr lpOverlapped, LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine );

            [DllImport("fbidio.dll")]
            public static extern    uint    DioEventRequestPending(uint hDeviceHandle, uint dwEventEnableMask, out uint pEventBuf, ref OVERLAPPED lpOverlapped);
            [DllImport("fbidio.dll")]
            public static extern    uint    DioEventRequestPending(uint hDeviceHandle, uint dwEventEnableMask, out uint pEventBuf, IntPtr lpOverlapped);

            [DllImport("fbidio.dll")]
            public static extern uint DioEventRequestPendingEx(uint hDeviceHandle, uint[] pdwEventEnableMask, uint[] pEventBuf, ref OVERLAPPED lpOverlapped);
            [DllImport("fbidio.dll")]
            public static extern uint DioEventRequestPendingEx(uint hDeviceHandle, uint[] pdwEventEnableMask, uint[] pEventBuf, IntPtr lpOverlapped);
            [DllImport("fbidio.dll")]
            public static extern uint DioEventRequestPendingEx(uint hDeviceHandle, uint[] pdwEventEnableMask, IntPtr pEventBuf, ref OVERLAPPED lpOverlapped);
            [DllImport("fbidio.dll")]
            public static extern uint DioEventRequestPendingEx(uint hDeviceHandle, uint[] pdwEventEnableMask, IntPtr pEventBuf, IntPtr lpOverlapped);
        }
	}
}
