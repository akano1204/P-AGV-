using System;
using System.Runtime.InteropServices;

namespace InterfaceCorpDllWrap
{
	public class IFCDA_ANY
	{
		private IFCDA_ANY(){}
		
		public const uint INVALID_HANDLE_VALUE = 0xFFFFFFFF;
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Overlapped Process Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint FLAG_SYNC		= 1;	// The analog output update is performed as an non-overlapped operation.
		public const uint FLAG_ASYNC	= 2;	// The analog output update is performed as an overlapped operation.
		
		//-----------------------------------------------------------------------------------------------
		//
		//		File Format Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint FLAG_BIN	 = 1;	// Binary format file
		public const uint FLAG_CSV	 = 2;	// CSV format file
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Analog Output Status Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_STATUS_STOP_SAMPLING		 = 1;	// The analog output update is stopped.
		public const uint DA_STATUS_WAIT_TRIGGER		 = 2;	// The analog output update is waiting for a trigger.
		public const uint DA_STATUS_NOW_SAMPLING		 = 3;	// The analog output update is running.
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Event Factor Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EVENT_STOP_TRIGGER		 = 1;	// The analog output has been stopped because a trigger is asserted.
		public const uint DA_EVENT_STOP_FUNCTION	 = 2;	// The analog output has been stopped by software.
		public const uint DA_EVENT_STOP_SAMPLING	 = 3;	// The Analog output terminated.
		public const uint DA_EVENT_RESET_IN			 = 4;	// The reset input signal is asserted.
		public const uint DA_EVENT_CURRENT_OFF		 = 5;	// The current loop fault has been detected.
		public const uint DA_EVENT_COUNT			 = 6;	// The specified number of output has been acquired.
		public const uint DA_EVENT_FIFO_EMPTY        = 7;   // The fifo is Empty
		public const uint DA_EVENT_EX_INT			 = 8;   // EX INT
		public const uint DA_EVENT_EXOV_OFF			 = 9;   // EX Over Voktage off
		public const uint DA_EVENT_OV_OFF			 = 10;  // Over Voktage off
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Volume Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ADJUST_BIOFFSET	 = 	1;	// Bipolar offset calibration
		public const uint DA_ADJUST_UNIOFFSET	 = 	2;	// Unipolar offset calibration
		public const uint DA_ADJUST_BIGAIN		 = 	3;	// Bipolar gain calibration
		public const uint DA_ADJUST_UNIGAIN		 = 	4;	// Unipolar gain calibration
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Calibration Item Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ADJUST_UP			 = 	1;	// Increase the volume.
		public const uint DA_ADJUST_DOWN		 = 	2;	// Decrease the volume.
		public const uint DA_ADJUST_STORE		 = 	3;	// Save the present value 
														// to the non-volatile memory.
		public const uint DA_ADJUST_STANDBY		 = 	4;	// Place the electronic volume device into 
														// the standby mode.
		public const uint DA_ADJUST_NOT_STORE	 = 	5;	// Not save the value.
		
		//-----------------------------------------------------------------------------------------------
		//
		//     Read Adjust Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ADJUST_READ_FACTORY	= 	1;	// Factory Setting
		public const uint DA_ADJUST_READ_USER		= 	2;	// User Setting
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Data Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_DATA_PHYSICAL		 = 	1;	// Physical value (voltage [V], current [mA])
		public const uint DA_DATA_BIN8			 = 	2;	// 8-bit binary
		public const uint DA_DATA_BIN12			 = 	3;	// 12-bit binary
		public const uint DA_DATA_BIN16			 = 	4;	// 16-bit binary
		public const uint DA_DATA_BIN24			 = 	5;	// 24-bit binary

		//-----------------------------------------------------------------------------------------------
		//
		//		Data Conversion Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_CONV_SMOOTH		 = 		1;	// Convert the data with interpolation.
		public const uint DA_CONV_AVERAGE1		 = 	0x100;	// Convert the data with the simple averaging.
		public const uint DA_CONV_AVERAGE2		 = 	0x200;	// Convert the data with the shifted averaging.

		//-----------------------------------------------------------------------------------------------
		//
		//		Data Transfer Architecture Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_IO_SAMPLING		 = 	1;	// Programmed I/O
		public const uint DA_FIFO_SAMPLING		 = 	2;	// FIFO
		public const uint DA_MEM_SAMPLING		 = 	4;	// Memory

		//-----------------------------------------------------------------------------------------------
		//
		//		Trigger Point Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_TRIG_START			 = 	1;	// Start-trigger (default setting)
		public const uint DA_TRIG_STOP			 = 	2;	// Stop-trigger
		public const uint DA_TRIG_START_STOP	 = 	3;	// Start/stop-trigger

		//-----------------------------------------------------------------------------------------------
		//
		//		Trigger Level Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_FREERUN		 = 	1;	// No-trigger (default setting)
		public const uint DA_EXTTRG			 = 	2;	// External trigger
		public const uint DA_EXTTRG_DI		 = 	3;	// External trigger with DI masking
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_DOWN_EDGE		 = 	1;	// Falling edge (default)
		public const uint DA_UP_EDGE		 = 	2;	// Rising edge
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Range Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_0_1V		 = 	0x00000001;	// Voltage: unipolar 0 V to +1 V
		public const uint DA_0_2P5V		 = 	0x00000002;	// Voltage: unipolar 0 V to +2.5 V
		public const uint DA_0_5V		 = 	0x00000004;	// Voltage: unipolar 0 V to +5 V
		public const uint DA_0_10V		 = 	0x00000008;	// Voltage: unipolar 0 V to +10 V
		public const uint DA_1_5V		 = 	0x00000010;	// Voltage: unipolar +1 V to +5 V
		public const uint DA_0_20mA		 = 	0x00001000;	// Current: unipolar 0 mA to +20 mA
		public const uint DA_4_20mA		 = 	0x00002000;	// Current: unipolar +4 mA to +20 mA
		public const uint DA_0_1mA		 = 	0x00004000;	// Current: unipolar 0 mA to +1 mA
		public const uint DA_0_100mA	 = 	0x00008000;	// Current: unipolar 0 mA to +100 mA
		public const uint DA_1V			 = 	0x00010000;	// Voltage: bipolar +/-1 V
		public const uint DA_2P5V		 = 	0x00020000;	// Voltage: bipolar +/-2.5 V
		public const uint DA_5V			 = 	0x00040000;	// Voltage: bipolar +/-5 V
		public const uint DA_10V		 = 	0x00080000;	// Voltage: bipolar +/-10 V
		public const uint DA_20mA		 = 	0x01000000;	// Current: bipolar +/-20 mA
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Isolation Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ISOLATION			 = 	1;	// Photo-isolated board
		public const uint DA_NOT_ISOLATION		 = 	2;	// Not isolated board
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Range Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_RANGE_UNIPOLAR		 = 	1;	// Unipolar
		public const uint DA_RANGE_BIPOLAR		 = 	2;	// Bipolar
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Waveform Generation Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_MODE_CUT			 = 	1;	// Time-based waveform generation
		public const uint DA_MODE_SYNTHE		 = 	2;	// Frequency-based waveform generation
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Repeat Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_REPEAT_NONINTERVAL	 = 	1;	// Repeat without the wait state (default setting)
		public const uint DA_REPEAT_INTERVAL	 = 	2;	// Repeat with the wait state
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Counter Clear Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_COUNTER_CLEAR		 = 	1;	// Cleared (default setting)
		public const uint DA_COUNTER_NONCLEAR	 = 	2;	// Not cleared
		
		//-----------------------------------------------------------------------------------------------
		//
		//		DA Latch Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_LATCH_CLEAR		 = 	1;	// The voltage is set to the lowest voltage of the range.
		public const uint DA_LATCH_NONCLEAR		 = 	2;	// The voltage is held.
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Clock Source Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_CLOCK_TIMER		 = 	1;	// Internal programmable timer (8254 compatible)
		public const uint DA_CLOCK_FIXED		 = 	2;	// Fixed 5 MHz clock
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Configurations of the Connector CN3 Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EXTRG_IN			 = 	1;	// External trigger input (default setting)
		public const uint DA_EXTRG_OUT			 = 	2;	// External trigger output
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Configurations of the Connector CN4 Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EXCLK_IN			 = 	1;	// External clock input (default setting)
		public const uint DA_EXCLK_OUT			 = 	2;	// External clock output
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Reset Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_RESET_DOWN_EDGE	 =  0x04;	// Falling edge (default)
		public const uint DA_RESET_UP_EDGE		 =	0x08;	// Rising edge
		
		//-----------------------------------------------------------------------------------------------
		//
		//		External trigger Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EXTRG_DOWN_EDGE	 = 	0x10;	// Falling edge (default)
		public const uint DA_EXTRG_UP_EDGE		 =	0x20;	// Rising edge
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Reset Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_RESET_ON			 =	1;	//Used
		public const uint DA_RESET_OFF			 =	2;	// Not used (default setting)
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Filter Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_FILTER_OFF			 = 	1;	// Not used (default setting)
		public const uint DA_FILTER_ON			 = 	2;	// Used
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Synchronous Analog Output Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_MASTER_MODE		 = 	1;	// Master mode
		public const uint DA_SLAVE_MODE			 = 	2;	// Slave mode
		
		//-----------------------------------------------------------------------------------------------
		//
		//     Synchronous Number Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_SYNC_NUM_1			 = 	0x0100;
		public const uint DA_SYNC_NUM_2			 = 	0x0200;
		public const uint DA_SYNC_NUM_3			 = 	0x0400;
		public const uint DA_SYNC_NUM_4			 = 	0x0800;
		public const uint DA_SYNC_NUM_5			 = 	0x1000;
		public const uint DA_SYNC_NUM_6			 = 	0x2000;
		public const uint DA_SYNC_NUM_7			 = 	0x4000;
		
		//-----------------------------------------------------------------------------------------------
		//
		//		PCI-3525 channel 3 and channel 4 Function Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_CN_FREE			 = 	0;	// not used
		public const uint DA_CN_EXTRG_IN		 = 	1;	// External trigger input
		public const uint DA_CN_EXTRG_OUT		 = 	2;	// External trigger output
		public const uint DA_CN_EXCLK_IN		 = 	3;	// External clock input
		public const uint DA_CN_EXCLK_OUT		 = 	4;	// External clock output
		public const uint DA_CN_EXINT_IN		 = 	5;	// External interrupt input
		public const uint DA_CN_ATRG_OUT		 = 	6;	// Analog trigger out
		public const uint DA_CN_DI				 = 	7;	// Digital input
		public const uint DA_CN_DO				 = 	8;	// Digital output
		public const uint DA_CN_DAOUT			 = 	9;	// Analog output
		public const uint DA_CN_OPEN			 = 	10;	// open
		
		//-----------------------------------------------------------------------------------------------
		//
		//		GPC/GPF-3300 CPZ-360810 DIN/DOUT Function Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EX_DIO1			=	1;	// DIN/DOUT1
		public const uint DA_EX_DIO2			=	2;	// DIN/DOUT2
		public const uint DA_EX_DIO3			=	3;	// DIN/DOUT3
		public const uint DA_EX_DIO4			=	4;	// DIN/DOUT4
		public const uint DA_EX_DIO5			=	5;	// DIN/DOUT5
		
		//-----------------------------------------------------------------------------------------------
		//
		//		PCI-3525 External Trigger Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_START_DOWN_EDGE	 = 	1;	// Start external trigger falling edge
		public const uint DA_START_UP_EDGE		 = 	2;	// Start external trigger rising edge
		public const uint DA_STOP_DOWN_EDGE		 = 	4;	// Stop external trigger falling edge
		public const uint DA_STOP_UP_EDGE		 = 	8;	// Stop external trigger rising edge
		
		//-----------------------------------------------------------------------------------------------
		//
		//		PCI-3525 Trigger Level Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_TRG_FREERUN		 = 	0;	// No trigger
		public const uint DA_TRG_EXTTRG			 = 	1;	// External trigger
		public const uint DA_TRG_ATRG			 = 	2;	// Analog trigger
		public const uint DA_TRG_SIGTIMER		 = 	3;	// Interval timer
		public const uint DA_TRG_CNT_EQ			 =  4;	// Counter equal
		public const uint DA_TRG_Z_CLR			 =  5;	// Z clear
		public const uint DA_TRG_AD_START		 = 	5;	// Analog input start
		public const uint DA_TRG_AD_STOP		 = 	6;	// Analog input stop
		public const uint DA_TRG_AD_PRETRG		 = 	7;	// Analog input pre-trigger
		public const uint DA_TRG_AD_POSTTRG		 = 	8;	// Analog input post-trigger
		public const uint DA_TRG_SMPLNUM		 = 	9;	// Analog output stop number
		public const uint DA_TRG_FIFO_EMPTY		 = 	10;	// FIFO empty
		public const uint DA_TRG_SYNC1			 = 	14;	// Internel sync1 trigger
		public const uint DA_TRG_SYNC2			 = 	15;	// Internel sync2 trigger
		public const uint DA_FIFORESET			 = 	0x0100;	// FIFO reset
		public const uint DA_RETRG				 = 	0x0200;	// Retrigger
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Simultaneous Output Set Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_NORMAL_OUTPUT		 = 	1;	// Not simultaneous output
		public const uint DA_SYNC_OUTPUT		 = 	2;	// Simultaneous output
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Error Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ERROR_SUCCESS					 = 	0x00000000;
		public const uint DA_ERROR_NOT_DEVICE				 = 	0xC0000001;
		public const uint DA_ERROR_NOT_OPEN					 = 	0xC0000002;
		public const uint DA_ERROR_INVALID_HANDLE			 = 	0xC0000003;
		public const uint DA_ERROR_ALREADY_OPEN				 = 	0xC0000004;
		public const uint DA_ERROR_NOT_SUPPORTED			 = 	0xC0000009;
		public const uint DA_ERROR_NOW_SAMPLING				 = 	0xC0001001;
		public const uint DA_ERROR_STOP_SAMPLING			 = 	0xC0001002;
		public const uint DA_ERROR_START_SAMPLING			 = 	0xC0001003;
		public const uint DA_ERROR_SAMPLING_TIMEOUT			 = 	0xC0001004;
		public const uint DA_ERROR_INVALID_PARAMETER		 = 	0xC0001021;
		public const uint DA_ERROR_ILLEGAL_PARAMETER		 = 	0xC0001022;
		public const uint DA_ERROR_NULL_POINTER				 = 	0xC0001023;
		public const uint DA_ERROR_SET_DATA					 = 	0xC0001024;
		public const uint DA_ERROR_USED_AD					 = 	0xC0001025;
		public const uint DA_ERROR_FILE_OPEN				 = 	0xC0001041;
		public const uint DA_ERROR_FILE_CLOSE				 = 	0xC0001042;
		public const uint DA_ERROR_FILE_READ				 = 	0xC0001043;
		public const uint DA_ERROR_FILE_WRITE				 = 	0xC0001044;
		public const uint DA_ERROR_INVALID_DATA_FORMAT		 = 	0xC0001061;
		public const uint DA_ERROR_INVALID_AVERAGE_OR_SMOOTHING	 = 0xC0001062;
		public const uint DA_ERROR_INVALID_SOURCE_DATA			 = 0xC0001063;
		public const uint DA_ERROR_NOT_ALLOCATE_MEMORY			 = 	0xC0001081;
		public const uint DA_ERROR_NOT_LOAD_DLL					 = 	0xC0001082;
		public const uint DA_ERROR_CALL_DLL						 = 	0xC0001083;
		public const uint DA_ERROR_CALIBRATION					 = 	0xC0001084;
		public const uint DA_ERROR_USBIO_FAILED					 = 	0xC0001085;
		public const uint DA_ERROR_USBIO_TIMEOUT				 = 	0xC0001086;

		// -----------------------------------------------------------------------
		//
		//     User-supplied Function
		//
		// -----------------------------------------------------------------------
		public delegate void LPDACONVPROC(
			ushort wCh,
			uint dwCount,
			IntPtr lpData
			);
		
		public delegate void LPDACALLBACK(uint dwUser);
		
		// -----------------------------------------------------------------------
		//
		//		Structure Declaration
		//
		// -----------------------------------------------------------------------
		
		// -----------------------------------------------------------------------
		//	Analog Output Request Condition Structure for Each Channel
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DASMPLCHREQ
		{
			public uint	ulChNo;
			public uint	ulRange;
		}
		
		// -----------------------------------------------------------------------
		//	Analog Output Request Condition Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DASMPLREQ
		{
			public uint		ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public uint		ulSamplingMode;
			public float	fSmplFreq;
			public uint		ulSmplRepeat;
			public uint		ulTrigMode;
			public uint		ulTrigPoint;
			public uint		ulTrigDelay;
			public uint		ulEClkEdge;
			public uint		ulTrigEdge;
			public uint		ulTrigDI;

			public void InitializeArray()
			{
				ulChNoRange = new uint [512];
			}

			public void SetChNo(uint ulIndex , uint ulNumber)
			{
				ulChNoRange[ulIndex * 2] = ulNumber;
			}

			public uint GetChNo(uint ulIndex)
			{
				return(ulChNoRange[ulIndex * 2]);
			}

			public void	SetChRange(uint ulIndex, uint ulRange)
			{
				ulChNoRange[(ulIndex * 2) + 1] = ulRange;
			}
			
			public uint GetChRange(uint ulIndex)
			{
				return(ulChNoRange[(ulIndex * 2) + 1]);
			}
		}
		
		// -----------------------------------------------------------------------
		//	Board Specification Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DABOARDSPEC
		{
			public uint		ulBoardType;
			public uint		ulBoardID;
			public uint		ulSamplingMode;
			public uint		ulChCount;
			public uint		ulResolution;
			public uint		ulRange;
			public uint		ulIsolation;
			public uint		ulDi;
			public uint		ulDo;
		}
		
		// -----------------------------------------------------------------------
		//	Output Range Configurations Structure for Each Channel (for the PCI-3305)
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DAMODECHREQ
		{
			public uint 	ulRange;
			public float	fVolt;
			public uint		ulFilter;
		}
		
		// -----------------------------------------------------------------------
		//	Waveform Generation Mode Structure (for the PCI-3305)
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DAMODEREQ
		{
			public uint 	ulRange1;	//DAMODECHREQ.ulRange
			public float	fVolt1;		//DAMODECHREQ.fVolt
			public uint		ulFilter1;	//DAMODECHREQ.ulFilter
			public uint 	ulRange2;	//DAMODECHREQ.ulRange
			public float	fVolt2;		//DAMODECHREQ.fVolt
			public uint		ulFilter2;	//DAMODECHREQ.ulFilter
			public uint		ulPulseMode;
			public uint		ulSyntheOut;
			public uint		ulInterval;
			public float	fIntervalCycle;
			public uint		ulCounterClear;
			public uint		ulDaLatch;
			public uint		ulSamplingClock;
			public uint		ulExControl;
			public uint		ulExClock;
		}
		
		// -----------------------------------------------------------------------
		//	Fifo Analog Output Request Condition Structure (for the PCI-3525)
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DAFIFOREQ
		{
			public uint		ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public float	fSmplFreq;
			public uint		ulSmplRepeat;
			public uint		ulSmplNum;
			public uint		ulStartTrigCondition;
			public uint		ulStopTrigCondition;
			public uint		ulEClkEdge;
			public uint		ulTrigEdge;
			
			public void InitializeArray()
			{
				ulChNoRange = new uint [512];
			}
			
			public void SetChNo(uint ulIndex , uint ulNumber)
			{
				ulChNoRange[ulIndex * 2] = ulNumber;
			}
			
			public uint GetChNo(uint ulIndex)
			{
				return(ulChNoRange[ulIndex * 2]);
			}
			
			public void	SetChRange(uint ulIndex, uint ulRange)
			{
				ulChNoRange[(ulIndex * 2) + 1] = ulRange;
			}
			
			public uint GetChRange(uint ulIndex)
			{
				return(ulChNoRange[(ulIndex * 2) + 1]);
			}
		}
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Function Declaration
		//
		//-----------------------------------------------------------------------------------------------
		[DllImport("fbida.dll")]
		public static extern IntPtr  DaOpen(string szDevice);
		[DllImport("fbida.dll")]
		public static extern int  DaClose(IntPtr hDeviceHandleHandle);
		[DllImport("fbida.dll")]
		public static extern int  DaGetDeviceInfo(IntPtr hDeviceHandleHandle, out DABOARDSPEC DaBoardSpec);
		[DllImport("fbida.dll")]
		public static extern int  DaSetBoardConfig(IntPtr hDeviceHandle, uint ulSmplBufferSize, IntPtr hEvent, LPDACALLBACK lpCallBackProc, uint dwUser);
		[DllImport("fbida.dll")]
		public static extern int  DaSetCountEvent(IntPtr hDeviceHandle, uint ulEventNum, IntPtr hEvent, LPDACALLBACK lpCallBackProc, uint dwUser);
		[DllImport("fbida.dll")]
		public static extern int  DaGetBoardConfig(IntPtr hDeviceHandle, out uint ulSmplBufferSize, out uint ulDaSmplEventFactor);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingConfig(IntPtr hDeviceHandle, ref DASMPLREQ DaSmplConfig);
		[DllImport("fbida.dll")]
		public static extern int  DaGetSamplingConfig(IntPtr hDeviceHandle, out DASMPLREQ DaSmplConfig);
		[DllImport("fbida.dll")]
		public static extern int  DaSetMode(IntPtr hDeviceHandle, ref DAMODEREQ DaMode);
		[DllImport("fbida.dll")]
		public static extern int  DaGetMode(IntPtr hDeviceHandle, out DAMODEREQ DaMode);
		
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(IntPtr hDeviceHandle, ref byte pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(IntPtr hDeviceHandle, ref ushort pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(IntPtr hDeviceHandle, ref uint pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(IntPtr hDeviceHandle, byte[] pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(IntPtr hDeviceHandle, ushort[] pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(IntPtr hDeviceHandle, uint[] pSmplData, uint ulSmplDataNum);
		
		[DllImport("fbida.dll")]
		public static extern int  DaClearSamplingData(IntPtr hDeviceHandle);
		[DllImport("fbida.dll")]
		public static extern int  DaStartSampling(IntPtr hDeviceHandle, uint ulSyncFlag);
		[DllImport("fbida.dll")]
		public static extern int  DaSyncSampling(IntPtr hDeviceHandle, uint ulMode);
		[DllImport("fbida.dll")]
		public static extern int  DaStartFileSampling(IntPtr hDeviceHandle, string szPathName, uint ulFileFlag, uint ulSmplNum);
		[DllImport("fbida.dll")]
		public static extern int  DaStopSampling(IntPtr hDeviceHandle);
		[DllImport("fbida.dll")]
		public static extern int  DaGetStatus(IntPtr hDeviceHandle, out uint ulDaSmplStatus, out uint ulDaSmplCount, out uint ulDaAvailCount, out uint ulDaAvailRepeat);
		[DllImport("fbida.dll")]
		public static extern int  DaSetOutputMode(IntPtr hDeviceHandle, uint ulMode);
		[DllImport("fbida.dll")]
		public static extern int  DaGetOutputMode(IntPtr hDeviceHandle, out uint ulMode);

		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(IntPtr hDeviceHandle, uint nCh, ref DASMPLCHREQ DaSmplChReq, ref byte pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(IntPtr hDeviceHandle, uint nCh, ref DASMPLCHREQ DaSmplChReq, ref ushort pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(IntPtr hDeviceHandle, uint nCh, ref DASMPLCHREQ DaSmplChReq, ref uint pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(IntPtr hDeviceHandle, uint nCh, DASMPLCHREQ[] DaSmplChReq, byte[] pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(IntPtr hDeviceHandle, uint nCh, DASMPLCHREQ[] DaSmplChReq, ushort[] pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(IntPtr hDeviceHandle, uint nCh, DASMPLCHREQ[] DaSmplChReq, uint[] pData);
		
		[DllImport("fbida.dll")]
		public static extern int  DaInputDI(IntPtr hDeviceHandle, out uint dwData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDO(IntPtr hDeviceHandle, uint dwData);
		
		[DllImport("fbida.dll")]
		public static extern int  DaAdjustVR(IntPtr hDeviceHandle, uint ulAdjustCh, uint ulSelVolume, uint ulDirection, uint ulTap);
		[DllImport("fbida.dll")]
		public static extern int  DaReadAdjustVR(IntPtr hDeviceHandle, uint ulAdjustCh);
		[DllImport("fbida.dll")]
		public static extern int  DaReadAdjustVREx(IntPtr hDeviceHandle, uint ulAdjustCh, uint ulControl);
		
		[DllImport("fbida.dll")]
		public static extern int  DaCalibration(IntPtr hDeviceHandle, uint ulAdjustCh, uint ulRange);
		
		[DllImport("fbida.dll")]
		public static extern int  DaSetFifoConfig(IntPtr hDeviceHandle, ref DAFIFOREQ DaFifoConfig);
		[DllImport("fbida.dll")]
		public static extern int  DaGetFifoConfig(IntPtr hDeviceHandle, out DAFIFOREQ DaFifoConfig);
		[DllImport("fbida.dll")]
		public static extern int  DaSetInterval(IntPtr hDeviceHandle, uint ulInterval);
		[DllImport("fbida.dll")]
		public static extern int  DaGetInterval(IntPtr hDeviceHandle, out uint ulInterval);
		[DllImport("fbida.dll")]
		public static extern int  DaSetFunction(IntPtr hDeviceHandle, uint ulChNo, uint ulFunction);
		[DllImport("fbida.dll")]
		public static extern int  DaGetFunction(IntPtr hDeviceHandle, uint ulChNo, out uint ulFunction);
		
		[DllImport("fbida.dll")]
		public static extern int  DaSetCurrentDir(IntPtr hDeviceHandle, uint dwData);
		[DllImport("fbida.dll")]
		public static extern int  DaGetCurrentDir(IntPtr hDeviceHandle, out uint dwData);
		[DllImport("fbida.dll")]
		public static extern int  DaSetPowerSupply(IntPtr hDeviceHandle, uint ExOnOff);
		[DllImport("fbida.dll")]
		public static extern int  DaGetPowerSupply(IntPtr hDeviceHandle, out uint ExOnOff);
		[DllImport("fbida.dll")]
		public static extern int  DaGetOVStatus(IntPtr hDeviceHandle, out uint LowStatus, out uint HighStatus);
		[DllImport("fbida.dll")]
		public static extern int  DaSetExcessVoltage(IntPtr hDeviceHandle, uint ExOnOff);
		[DllImport("fbida.dll")]
		public static extern int  DaGetRelayStatus(IntPtr hDeviceHandle, out uint Status);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ref byte pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ref ushort pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ref uint pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ref float pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, byte[] pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ushort[] pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, uint[] pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, float[] pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);

		[DllImport("kernel32.dll",EntryPoint="CreateEventA")]
		public static extern IntPtr CreateEvent(uint lpEventAttributes, bool ManualReset, bool bInitialState, string lpName);
		[DllImport("kernel32.dll")]
		public static extern uint  WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);
		[DllImport("kernel32.dll")]
		public static extern bool ResetEvent(IntPtr hEvent);
	}
	
	public class IFCDA
	{
		private IFCDA(){}

		public const uint INVALID_HANDLE_VALUE = 0xFFFFFFFF;

		//-----------------------------------------------------------------------------------------------
		//
		//		Overlapped Process Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint FLAG_SYNC		= 1;	// The analog output update is performed as an non-overlapped operation.
		public const uint FLAG_ASYNC	= 2;	// The analog output update is performed as an overlapped operation.

		//-----------------------------------------------------------------------------------------------
		//
		//		File Format Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint FLAG_BIN	 = 1;	// Binary format file
		public const uint FLAG_CSV	 = 2;	// CSV format file

		//-----------------------------------------------------------------------------------------------
		//
		//		Analog Output Status Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_STATUS_STOP_SAMPLING		 = 1;	// The analog output update is stopped.
		public const uint DA_STATUS_WAIT_TRIGGER		 = 2;	// The analog output update is waiting for a trigger.
		public const uint DA_STATUS_NOW_SAMPLING		 = 3;	// The analog output update is running.

		//-----------------------------------------------------------------------------------------------
		//
		//		Event Factor Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EVENT_STOP_TRIGGER		 = 1;	// The analog output has been stopped because a trigger is asserted.
		public const uint DA_EVENT_STOP_FUNCTION	 = 2;	// The analog output has been stopped by software.
		public const uint DA_EVENT_STOP_SAMPLING	 = 3;	// The Analog output terminated.
		public const uint DA_EVENT_RESET_IN			 = 4;	// The reset input signal is asserted.
		public const uint DA_EVENT_CURRENT_OFF		 = 5;	// The current loop fault has been detected.
		public const uint DA_EVENT_COUNT			 = 6;	// The specified number of output has been acquired.
		public const uint DA_EVENT_FIFO_EMPTY        = 7;   // The fifo is Empty
		public const uint DA_EVENT_EX_INT			 = 8;   // EX INT
		public const uint DA_EVENT_EXOV_OFF			 = 9;   // EX Over Voktage off
		public const uint DA_EVENT_OV_OFF			 = 10;  // Over Voktage off
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Volume Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ADJUST_BIOFFSET	 = 	1;	// Bipolar offset calibration
		public const uint DA_ADJUST_UNIOFFSET	 = 	2;	// Unipolar offset calibration
		public const uint DA_ADJUST_BIGAIN		 = 	3;	// Bipolar gain calibration
		public const uint DA_ADJUST_UNIGAIN		 = 	4;	// Unipolar gain calibration
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Calibration Item Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ADJUST_UP			 = 	1;	// Increase the volume.
		public const uint DA_ADJUST_DOWN		 = 	2;	// Decrease the volume.
		public const uint DA_ADJUST_STORE		 = 	3;	// Save the present value 
														// to the non-volatile memory.
		public const uint DA_ADJUST_STANDBY		 = 	4;	// Place the electronic volume device into 
														// the standby mode.
		public const uint DA_ADJUST_NOT_STORE	 = 	5;	// Not save the value.
		
		//-----------------------------------------------------------------------------------------------
		//
		//     Read Adjust Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ADJUST_READ_FACTORY	= 	1;	// Factory Setting
		public const uint DA_ADJUST_READ_USER		= 	2;	// User Setting

		//-----------------------------------------------------------------------------------------------
		//
		//		Data Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_DATA_PHYSICAL		 = 	1;	// Physical value (voltage [V], current [mA])
		public const uint DA_DATA_BIN8			 = 	2;	// 8-bit binary
		public const uint DA_DATA_BIN12			 = 	3;	// 12-bit binary
		public const uint DA_DATA_BIN16			 = 	4;	// 16-bit binary
		public const uint DA_DATA_BIN24			 = 	5;	// 24-bit binary

		//-----------------------------------------------------------------------------------------------
		//
		//		Data Conversion Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_CONV_SMOOTH		 = 		1;	// Convert the data with interpolation.
		public const uint DA_CONV_AVERAGE1		 = 	0x100;	// Convert the data with the simple averaging.
		public const uint DA_CONV_AVERAGE2		 = 	0x200;	// Convert the data with the shifted averaging.

		//-----------------------------------------------------------------------------------------------
		//
		//		Data Transfer Architecture Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_IO_SAMPLING		 = 	1;	// Programmed I/O
		public const uint DA_FIFO_SAMPLING		 = 	2;	// FIFO
		public const uint DA_MEM_SAMPLING		 = 	4;	// Memory

		//-----------------------------------------------------------------------------------------------
		//
		//		Trigger Point Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_TRIG_START			 = 	1;	// Start-trigger (default setting)
		public const uint DA_TRIG_STOP			 = 	2;	// Stop-trigger
		public const uint DA_TRIG_START_STOP	 = 	3;	// Start/stop-trigger

		//-----------------------------------------------------------------------------------------------
		//
		//		Trigger Level Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_FREERUN		 = 	1;	// No-trigger (default setting)
		public const uint DA_EXTTRG			 = 	2;	// External trigger
		public const uint DA_EXTTRG_DI		 = 	3;	// External trigger with DI masking

		//-----------------------------------------------------------------------------------------------
		//
		//		Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_DOWN_EDGE		 = 	1;	// Falling edge (default)
		public const uint DA_UP_EDGE		 = 	2;	// Rising edge

		//-----------------------------------------------------------------------------------------------
		//
		//		Range Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_0_1V		 = 	0x00000001;	// Voltage: unipolar 0 V to +1 V
		public const uint DA_0_2P5V		 = 	0x00000002;	// Voltage: unipolar 0 V to +2.5 V
		public const uint DA_0_5V		 = 	0x00000004;	// Voltage: unipolar 0 V to +5 V
		public const uint DA_0_10V		 = 	0x00000008;	// Voltage: unipolar 0 V to +10 V
		public const uint DA_1_5V		 = 	0x00000010;	// Voltage: unipolar +1 V to +5 V
		public const uint DA_0_20mA		 = 	0x00001000;	// Current: unipolar 0 mA to +20 mA
		public const uint DA_4_20mA		 = 	0x00002000;	// Current: unipolar +4 mA to +20 mA
		public const uint DA_0_1mA		 =	0x00004000;	// Current: unipolar +0 mA to +1 mA
		public const uint DA_0_100mA	 =	0x00008000;	// Current: unipolar +0 mA to +100 mA
		public const uint DA_1V			 = 	0x00010000;	// Voltage: bipolar +/-1 V
		public const uint DA_2P5V		 = 	0x00020000;	// Voltage: bipolar +/-2.5 V
		public const uint DA_5V			 = 	0x00040000;	// Voltage: bipolar +/-5 V
		public const uint DA_10V		 = 	0x00080000;	// Voltage: bipolar +/-10 V
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Isolation Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ISOLATION			 = 	1;	// Photo-isolated board
		public const uint DA_NOT_ISOLATION		 = 	2;	// Not isolated board
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Range Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_RANGE_UNIPOLAR		 = 	1;	// Unipolar
		public const uint DA_RANGE_BIPOLAR		 = 	2;	// Bipolar

		//-----------------------------------------------------------------------------------------------
		//
		//		Waveform Generation Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_MODE_CUT			 = 	1;	// Time-based waveform generation
		public const uint DA_MODE_SYNTHE		 = 	2;	// Frequency-based waveform generation

		//-----------------------------------------------------------------------------------------------
		//
		//		Repeat Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_REPEAT_NONINTERVAL	 = 	1;	// Repeat without the wait state (default setting)
		public const uint DA_REPEAT_INTERVAL	 = 	2;	// Repeat with the wait state

		//-----------------------------------------------------------------------------------------------
		//
		//		Counter Clear Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_COUNTER_CLEAR		 = 	1;	// Cleared (default setting)
		public const uint DA_COUNTER_NONCLEAR	 = 	2;	// Not cleared

		//-----------------------------------------------------------------------------------------------
		//
		//		DA Latch Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_LATCH_CLEAR		 = 	1;	// The voltage is set to the lowest voltage of the range.
		public const uint DA_LATCH_NONCLEAR		 = 	2;	// The voltage is held.

		//-----------------------------------------------------------------------------------------------
		//
		//		Clock Source Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_CLOCK_TIMER		 = 	1;	// Internal programmable timer (8254 compatible)
		public const uint DA_CLOCK_FIXED		 = 	2;	// Fixed 5 MHz clock

		//-----------------------------------------------------------------------------------------------
		//
		//		Configurations of the Connector CN3 Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EXTRG_IN			 = 	1;	// External trigger input (default setting)
		public const uint DA_EXTRG_OUT			 = 	2;	// External trigger output

		//-----------------------------------------------------------------------------------------------
		//
		//		Configurations of the Connector CN4 Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EXCLK_IN			 = 	1;	// External clock input (default setting)
		public const uint DA_EXCLK_OUT			 = 	2;	// External clock output

		//-----------------------------------------------------------------------------------------------
		//
		//		Reset Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_RESET_DOWN_EDGE	 =  0x04;	// Falling edge (default)
		public const uint DA_RESET_UP_EDGE		 =	0x08;	// Rising edge

		//-----------------------------------------------------------------------------------------------
		//
		//		External trigger Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EXTRG_DOWN_EDGE	 = 	0x10;	// Falling edge (default)
		public const uint DA_EXTRG_UP_EDGE		 =	0x20;	// Rising edge

		//-----------------------------------------------------------------------------------------------
		//
		//		Reset Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_RESET_ON			 =	1;	//Used
		public const uint DA_RESET_OFF			 =	2;	// Not used (default setting)

		//-----------------------------------------------------------------------------------------------
		//
		//		Filter Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_FILTER_OFF			 = 	1;	// Not used (default setting)
		public const uint DA_FILTER_ON			 = 	2;	// Used

		//-----------------------------------------------------------------------------------------------
		//
		//		Synchronous Analog Output Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_MASTER_MODE		 = 	1;	// Master mode
		public const uint DA_SLAVE_MODE			 = 	2;	// Slave mode

		//-----------------------------------------------------------------------------------------------
		//
		//     Synchronous Number Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_SYNC_NUM_1			 = 	0x0100;
		public const uint DA_SYNC_NUM_2			 = 	0x0200;
		public const uint DA_SYNC_NUM_3			 = 	0x0400;
		public const uint DA_SYNC_NUM_4			 = 	0x0800;
		public const uint DA_SYNC_NUM_5			 = 	0x1000;
		public const uint DA_SYNC_NUM_6			 = 	0x2000;
		public const uint DA_SYNC_NUM_7			 = 	0x4000;

		//-----------------------------------------------------------------------------------------------
		//
		//		PCI-3525 channel 3 and channel 4 Function Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_CN_FREE			 = 	0;	// not used
		public const uint DA_CN_EXTRG_IN		 = 	1;	// External trigger input
		public const uint DA_CN_EXTRG_OUT		 = 	2;	// External trigger output
		public const uint DA_CN_EXCLK_IN		 = 	3;	// External clock input
		public const uint DA_CN_EXCLK_OUT		 = 	4;	// External clock output
		public const uint DA_CN_EXINT_IN		 = 	5;	// External interrupt input
		public const uint DA_CN_ATRG_OUT		 = 	6;	// Analog trigger out
		public const uint DA_CN_DI				 = 	7;	// Digital input
		public const uint DA_CN_DO				 = 	8;	// Digital output
		public const uint DA_CN_DAOUT			 = 	9;	// Analog output
		public const uint DA_CN_OPEN			 = 	10;	// open
		
		//-----------------------------------------------------------------------------------------------
		//
		//		GPC/GPF-3300 CPZ-360810 DIN/DOUT Function Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_EX_DIO1			=	1;	// DIN/DOUT1
		public const uint DA_EX_DIO2			=	2;	// DIN/DOUT2
		public const uint DA_EX_DIO3			=	3;	// DIN/DOUT3
		public const uint DA_EX_DIO4			=	4;	// DIN/DOUT4
		public const uint DA_EX_DIO5			=	5;	// DIN/DOUT5
		
		//-----------------------------------------------------------------------------------------------
		//
		//		PCI-3525 External Trigger Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_START_DOWN_EDGE	 = 	1;	// Start external trigger falling edge
		public const uint DA_START_UP_EDGE		 = 	2;	// Start external trigger rising edge
		public const uint DA_STOP_DOWN_EDGE		 = 	4;	// Stop external trigger falling edge
		public const uint DA_STOP_UP_EDGE		 = 	8;	// Stop external trigger rising edge

		//-----------------------------------------------------------------------------------------------
		//
		//		PCI-3525 Trigger Level Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_TRG_FREERUN		 = 0;	// No trigger
		public const uint DA_TRG_EXTTRG			 = 1;	// External trigger
		public const uint DA_TRG_ATRG			 = 2;	// Analog trigger
		public const uint DA_TRG_SIGTIMER		 = 3;	// Interval timer
		public const uint DA_TRG_CNT_EQ			 = 4;	// Counter equal
		public const uint DA_TRG_Z_CLR			 = 5;	// Z clear
		public const uint DA_TRG_AD_START		 = 5;	// Analog input start
		public const uint DA_TRG_AD_STOP		 = 6;	// Analog input stop
		public const uint DA_TRG_AD_PRETRG		 = 7;	// Analog input pre-trigger
		public const uint DA_TRG_AD_POSTTRG		 = 8;	// Analog input post-trigger
		public const uint DA_TRG_SMPLNUM		 = 9;	// Analog output stop number
		public const uint DA_TRG_FIFO_EMPTY		 = 10;	// FIFO empty
		public const uint DA_TRG_SYNC1			 = 14;	// Internel sync1 trigger
		public const uint DA_TRG_SYNC2			 = 15;	// Internel sync2 trigger
		public const uint DA_FIFORESET			 = 0x0100;	// FIFO reset
		public const uint DA_RETRG				 = 0x0200;	// Retrigger
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Simultaneous Output Set Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_NORMAL_OUTPUT		 = 	1;	// Not simultaneous output
		public const uint DA_SYNC_OUTPUT		 = 	2;	// Simultaneous output
		
		//-----------------------------------------------------------------------------------------------
		//
		//		Error Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint DA_ERROR_SUCCESS					 = 	0x00000000;
		public const uint DA_ERROR_NOT_DEVICE				 = 	0xC0000001;
		public const uint DA_ERROR_NOT_OPEN					 = 	0xC0000002;
		public const uint DA_ERROR_INVALID_HANDLE			 = 	0xC0000003;
		public const uint DA_ERROR_ALREADY_OPEN				 = 	0xC0000004;
		public const uint DA_ERROR_NOT_SUPPORTED			 = 	0xC0000009;
		public const uint DA_ERROR_NOW_SAMPLING				 = 	0xC0001001;
		public const uint DA_ERROR_STOP_SAMPLING			 = 	0xC0001002;
		public const uint DA_ERROR_START_SAMPLING			 = 	0xC0001003;
		public const uint DA_ERROR_SAMPLING_TIMEOUT			 = 	0xC0001004;
		public const uint DA_ERROR_INVALID_PARAMETER		 = 	0xC0001021;
		public const uint DA_ERROR_ILLEGAL_PARAMETER		 = 	0xC0001022;
		public const uint DA_ERROR_NULL_POINTER				 = 	0xC0001023;
		public const uint DA_ERROR_SET_DATA					 = 	0xC0001024;
		public const uint DA_ERROR_USED_AD					 = 	0xC0001025;
		public const uint DA_ERROR_FILE_OPEN				 = 	0xC0001041;
		public const uint DA_ERROR_FILE_CLOSE				 = 	0xC0001042;
		public const uint DA_ERROR_FILE_READ				 = 	0xC0001043;
		public const uint DA_ERROR_FILE_WRITE				 = 	0xC0001044;
		public const uint DA_ERROR_INVALID_DATA_FORMAT		 = 	0xC0001061;
		public const uint DA_ERROR_INVALID_AVERAGE_OR_SMOOTHING	 = 0xC0001062;
		public const uint DA_ERROR_INVALID_SOURCE_DATA			 = 0xC0001063;
		public const uint DA_ERROR_NOT_ALLOCATE_MEMORY			 = 0xC0001081;
		public const uint DA_ERROR_NOT_LOAD_DLL					 = 0xC0001082;
		public const uint DA_ERROR_CALL_DLL						 = 0xC0001083;
		public const uint DA_ERROR_CALIBRATION					 = 0xC0001084;
		public const uint DA_ERROR_USBIO_FAILED					 = 0xC0001085;
		public const uint DA_ERROR_USBIO_TIMEOUT				 = 0xC0001086;

		// -----------------------------------------------------------------------
		//
		//     User-supplied Function
		//
		// -----------------------------------------------------------------------
		public delegate void LPDACONVPROC(
			ushort wCh,
			uint dwCount,
			IntPtr lpData
			);

		public delegate void LPDACALLBACK(uint dwUser);

		
		// -----------------------------------------------------------------------
		//
		//		Structure Declaration
		//
		// -----------------------------------------------------------------------

		// -----------------------------------------------------------------------
		//	Analog Output Request Condition Structure for Each Channel
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DASMPLCHREQ
		{
			public uint	ulChNo;
			public uint	ulRange;
		}

		// -----------------------------------------------------------------------
		//	Analog Output Request Condition Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DASMPLREQ
		{
			public uint		ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public uint		ulSamplingMode;
			public float	fSmplFreq;
			public uint		ulSmplRepeat;
			public uint		ulTrigMode;
			public uint		ulTrigPoint;
			public uint		ulTrigDelay;
			public uint		ulEClkEdge;
			public uint		ulTrigEdge;
			public uint		ulTrigDI;

			public void InitializeArray()
			{
				ulChNoRange = new uint [512];
			}

			public void SetChNo(uint ulIndex , uint ulNumber)
			{
				ulChNoRange[ulIndex * 2] = ulNumber;
			}

			public uint GetChNo(uint ulIndex)
			{
				return(ulChNoRange[ulIndex * 2]);
			}

			public void	SetChRange(uint ulIndex, uint ulRange)
			{
				ulChNoRange[(ulIndex * 2) + 1] = ulRange;
			}

			public uint GetChRange(uint ulIndex)
			{
				return(ulChNoRange[(ulIndex * 2) + 1]);
			}		
		}

		// -----------------------------------------------------------------------
		//	Board Specification Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DABOARDSPEC
		{
			public uint		ulBoardType;
			public uint		ulBoardID;
			public uint		ulSamplingMode;
			public uint		ulChCount;
			public uint		ulResolution;
			public uint		ulRange;
			public uint		ulIsolation;
			public uint		ulDi;
			public uint		ulDo;
		}

		// -----------------------------------------------------------------------
		//	Output Range Configurations Structure for Each Channel (for the PCI-3305)
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DAMODECHREQ
		{
			public uint 	ulRange;
			public float	fVolt;
			public uint		ulFilter;
		}

		// -----------------------------------------------------------------------
		//	Waveform Generation Mode Structure (for the PCI-3305)
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DAMODEREQ
		{
			public uint 	ulRange1;	//DAMODECHREQ.ulRange
			public float	fVolt1;		//DAMODECHREQ.fVolt
			public uint		ulFilter1;	//DAMODECHREQ.ulFilter
			public uint 	ulRange2;	//DAMODECHREQ.ulRange
			public float	fVolt2;		//DAMODECHREQ.fVolt
			public uint		ulFilter2;	//DAMODECHREQ.ulFilter
			public uint		ulPulseMode;
			public uint		ulSyntheOut;
			public uint		ulInterval;
			public float	fIntervalCycle;
			public uint		ulCounterClear;
			public uint		ulDaLatch;
			public uint		ulSamplingClock;
			public uint		ulExControl;
			public uint		ulExClock;
		}

		// -----------------------------------------------------------------------
		//	Fifo Analog Output Request Condition Structure (for the PCI-3525)
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct DAFIFOREQ
		{
			public uint		ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public float	fSmplFreq;
			public uint		ulSmplRepeat;
			public uint		ulSmplNum;
			public uint		ulStartTrigCondition;
			public uint		ulStopTrigCondition;
			public uint		ulEClkEdge;
			public uint		ulTrigEdge;

			public void InitializeArray()
			{
				ulChNoRange = new uint [512];
			}

			public void SetChNo(uint ulIndex , uint ulNumber)
			{
				ulChNoRange[ulIndex * 2] = ulNumber;
			}

			public uint GetChNo(uint ulIndex)
			{
				return(ulChNoRange[ulIndex * 2]);
			}

			public void	SetChRange(uint ulIndex, uint ulRange)
			{
				ulChNoRange[(ulIndex * 2) + 1] = ulRange;
			}

			public uint GetChRange(uint ulIndex)
			{
				return(ulChNoRange[(ulIndex * 2) + 1]);
			}		
		}

		//-----------------------------------------------------------------------------------------------
		//
		//		Function Declaration
		//
		//-----------------------------------------------------------------------------------------------
		[DllImport("fbida.dll")]
		public static extern uint  DaOpen(string szDevice);
		[DllImport("fbida.dll")]
		public static extern int  DaClose(uint hDeviceHandle);
		[DllImport("fbida.dll")]
		public static extern int  DaGetDeviceInfo(uint hDeviceHandle, out DABOARDSPEC DaBoardSpec);
		[DllImport("fbida.dll")]
		public static extern int  DaSetBoardConfig(uint hDeviceHandle, uint ulSmplBufferSize, uint hEvent, LPDACALLBACK lpCallBackProc, uint dwUser);
		[DllImport("fbida.dll")]
		public static extern int  DaSetCountEvent(uint hDeviceHandle, uint ulEventNum, uint hEvent, LPDACALLBACK lpCallBackProc, uint dwUser);
		[DllImport("fbida.dll")]
		public static extern int  DaGetBoardConfig(uint hDeviceHandle, out uint ulSmplBufferSize, out uint ulDaSmplEventFactor);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingConfig(uint hDeviceHandle, ref DASMPLREQ DaSmplConfig);
		[DllImport("fbida.dll")]
		public static extern int  DaGetSamplingConfig(uint hDeviceHandle, out DASMPLREQ DaSmplConfig);
		[DllImport("fbida.dll")]
		public static extern int  DaSetMode(uint hDeviceHandle, ref DAMODEREQ DaMode);
		[DllImport("fbida.dll")]
		public static extern int  DaGetMode(uint hDeviceHandle, out DAMODEREQ DaMode);

		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(uint hDeviceHandle, ref byte pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(uint hDeviceHandle, ref ushort pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(uint hDeviceHandle, ref uint pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(uint hDeviceHandle, byte[] pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(uint hDeviceHandle, ushort[] pSmplData, uint ulSmplDataNum);
		[DllImport("fbida.dll")]
		public static extern int  DaSetSamplingData(uint hDeviceHandle, uint[] pSmplData, uint ulSmplDataNum);

		
		[DllImport("fbida.dll")]
		public static extern int  DaClearSamplingData(uint hDeviceHandle);
		[DllImport("fbida.dll")]
		public static extern int  DaStartSampling(uint hDeviceHandle, uint ulSyncFlag);
		[DllImport("fbida.dll")]
		public static extern int  DaSyncSampling(uint hDeviceHandle, uint ulMode);
		[DllImport("fbida.dll")]
		public static extern int  DaStartFileSampling(uint hDeviceHandle, string szPathName, uint ulFileFlag, uint ulSmplNum);
		[DllImport("fbida.dll")]
		public static extern int  DaStopSampling(uint hDeviceHandle);
		[DllImport("fbida.dll")]
		public static extern int  DaGetStatus(uint hDeviceHandle, out uint ulDaSmplStatus, out uint ulDaSmplCount, out uint ulDaAvailCount, out uint ulDaAvailRepeat);
		[DllImport("fbida.dll")]
		public static extern int  DaSetOutputMode(uint hDeviceHandle, uint ulMode);
		[DllImport("fbida.dll")]
		public static extern int  DaGetOutputMode(uint hDeviceHandle, out uint ulMode);

		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(uint hDeviceHandle, uint nCh, ref DASMPLCHREQ DaSmplChReq, ref byte pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(uint hDeviceHandle, uint nCh, ref DASMPLCHREQ DaSmplChReq, ref ushort pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(uint hDeviceHandle, uint nCh, ref DASMPLCHREQ DaSmplChReq, ref uint pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(uint hDeviceHandle, uint nCh, DASMPLCHREQ[] DaSmplChReq, byte[] pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(uint hDeviceHandle, uint nCh, DASMPLCHREQ[] DaSmplChReq, ushort[] pData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDA(uint hDeviceHandle, uint nCh, DASMPLCHREQ[] DaSmplChReq, uint[] pData);
		
		[DllImport("fbida.dll")]
		public static extern int  DaInputDI(uint hDeviceHandle, out uint dwData);
		[DllImport("fbida.dll")]
		public static extern int  DaOutputDO(uint hDeviceHandle, uint dwData);
		[DllImport("fbida.dll")]
		public static extern int  DaAdjustVR(uint hDeviceHandle, uint ulAdjustCh, uint ulSelVolume, uint ulDirection, uint ulTap);
		[DllImport("fbida.dll")]
		public static extern int  DaReadAdjustVR(uint hDeviceHandle, uint ulAdjustCh);
		[DllImport("fbida.dll")]
		public static extern int  DaReadAdjustVREx(uint hDeviceHandle, uint ulAdjustCh, uint ulControl);
		
		[DllImport("fbida.dll")]
		public static extern int  DaCalibration(IntPtr hDeviceHandle, uint ulAdjustCh, uint ulRange);
		
		[DllImport("fbida.dll")]
		public static extern int  DaSetFifoConfig(uint hDeviceHandle, ref DAFIFOREQ DaFifoConfig);
		[DllImport("fbida.dll")]
		public static extern int  DaGetFifoConfig(uint hDeviceHandle, out DAFIFOREQ DaFifoConfig);
		[DllImport("fbida.dll")]
		public static extern int  DaSetInterval(uint hDeviceHandle, uint ulInterval);
		[DllImport("fbida.dll")]
		public static extern int  DaGetInterval(uint hDeviceHandle, out uint ulInterval);
		[DllImport("fbida.dll")]
		public static extern int  DaSetFunction(uint hDeviceHandle, uint ulChNo, uint ulFunction);
		[DllImport("fbida.dll")]
		public static extern int  DaGetFunction(uint hDeviceHandle, uint ulChNo, out uint ulFunction);
		
		[DllImport("fbida.dll")]
		public static extern int  DaSetCurrentDir(uint hDeviceHandle, uint dwData);
		[DllImport("fbida.dll")]
		public static extern int  DaGetCurrentDir(uint hDeviceHandle, out uint dwData);
		[DllImport("fbida.dll")]
		public static extern int  DaSetPowerSupply(uint hDeviceHandle, uint ExOnOff);
		[DllImport("fbida.dll")]
		public static extern int  DaGetPowerSupply(uint hDeviceHandle, out uint ExOnOff);
		[DllImport("fbida.dll")]
		public static extern int  DaGetOVStatus(uint hDeviceHandle, out uint LowStatus, out uint HighStatus);
		[DllImport("fbida.dll")]
		public static extern int  DaSetExcessVoltage(IntPtr hDeviceHandle, uint ExOnOff);
		[DllImport("fbida.dll")]
		public static extern int  DaGetRelayStatus(uint hDeviceHandle, out uint Status);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		[DllImport("fbidadc.dll")]
		public static extern int  DaDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref DASMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint puDestSmplDataNum, ref DASMPLREQ DestSmplReq, uint uEffect, uint uCount, LPDACONVPROC pfnConv);
		
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ref byte pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ref ushort pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ref uint pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ref float pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, byte[] pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, ushort[] pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, uint[] pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		[DllImport("fbidadc.dll")]
		public static extern int  DaWriteFile(string szPathName, float[] pSmplData, uint ulFormCode, uint ulSmplNum, uint ulChCount);
		
		[DllImport("kernel32.dll",EntryPoint="CreateEventA")]
		public static extern uint CreateEvent(uint lpEventAttributes, bool ManualReset, bool bInitialState, string lpName);
		[DllImport("kernel32.dll")]
		public static extern uint  WaitForSingleObject(uint hHandle, uint dwMilliseconds);
		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(uint hObject);
		[DllImport("kernel32.dll")]
		public static extern bool ResetEvent(uint hEvent);
	}
}
