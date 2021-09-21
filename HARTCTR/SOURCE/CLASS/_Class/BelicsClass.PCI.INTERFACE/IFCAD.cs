using System;
using System.Runtime.InteropServices;

namespace InterfaceCorpDllWrap
{
	public class IFCAD_ANY
	{
		private IFCAD_ANY(){}

		public const uint INVALID_HANDLE_VALUE = 0xFFFFFFFF;
		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Overlapped Process Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint FLAG_SYNC		=	1;	// Sampling as an non-overlapped operation
		public const uint FLAG_ASYNC	=	2;	// Sampling as overlapped operation

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 File Format Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint FLAG_BIN	 = 	1;	// Binary format file
		public const uint FLAG_CSV	 = 	2;	// CSV format file

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Sampling Status Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_STATUS_STOP_SAMPLING		 = 1;	// The sampling is stopped.
		public const uint AD_STATUS_WAIT_TRIGGER		 = 2;	// The sampling is waiting for a trigger.
		public const uint AD_STATUS_NOW_SAMPLING		 = 3;	// The sampling is running.

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Event Factor Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_EVENT_SMPLNUM			 = 1;	// The specified number of samples has been acquired.
		public const uint AD_EVENT_STOP_TRIGGER		 = 2;	// The sampling has been stopped because a trigger is asserted.
		public const uint AD_EVENT_STOP_FUNCTION	 = 3;	// The sampling has been stopped by software.
		public const uint AD_EVENT_STOP_TIMEOUT		 = 4;	// The sampling has been stopped because a time-out interval elapsed.
		public const uint AD_EVENT_STOP_SAMPLING	 = 5;	// The sampling is stopped.
		public const uint AD_EVENT_STOP_SCER		 = 6;	// The sampling is stopped by a clock error.
		public const uint AD_EVENT_STOP_ORER		 = 7;	// The sampling is stopped by an overrun error.
		public const uint AD_EVENT_SCER				 = 8;	// The sampling pacer clock error is occurred.
		public const uint AD_EVENT_ORER				 = 9;	// The overrun error is occurred.
		public const uint AD_EVENT_STOP_LV_1		 = 10;	// The channel 1 sampling is stopped. (Only applicable to the PCI-3179)
		public const uint AD_EVENT_STOP_LV_2		 = 11;	// The channel 2 sampling is stopped. (Only applicable to the PCI-3179)
		public const uint AD_EVENT_STOP_LV_3		 = 12;	// The channel 3 sampling is stopped. (Only applicable to the PCI-3179)
		public const uint AD_EVENT_STOP_LV_4		 = 13;	// The channel 4 sampling is stopped. (Only applicable to the PCI-3179)
		public const uint AD_EVENT_RANGE			 = 14;	// The AD conversion value reached the full-scale range.
		public const uint AD_EVENT_STOP_RANGE		 = 15;	// The sampling is stopped by the full-scale range detection.
		public const uint AD_EVENT_OVPM				 = 16;	// 過電圧入力時自動レンジ切替機能検出
		public const uint AD_EVENT_STOP_OVPM		 = 17;	// 過電圧入力時自動レンジ切替機能検出によりサンプリング終了

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Input Configuration Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_INPUT_SINGLE			 = 1;	// Single-ended input
		public const uint AD_INPUT_DIFF				 = 2;	// Differential input

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Volume Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_ADJUST_BIOFFSET		 = 1;	// Bipolar offset
		public const uint AD_ADJUST_UNIOFFSET		 = 2;	// Unipolar offset
		public const uint AD_ADJUST_BIGAIN			 = 3;	// Bipolar gain
		public const uint AD_ADJUST_UNIGAIN			 = 4;	// Unipolar gain

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Calibration Item Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_ADJUST_UP				 = 1;	// Increases the volume.
		public const uint AD_ADJUST_DOWN			 = 2;	// Decreases the volumde.
		public const uint AD_ADJUST_STORE			 = 3;	// Saves the present value to the non-volatile memory.
		public const uint AD_ADJUST_STANDBY			 = 4;	// Places the electronic volume device into the standby mode.
		public const uint AD_ADJUST_NOT_STORE		 = 5;	// Not save the value.

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Data Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_DATA_PHYSICAL			 = 1;	// Physical value (voltage [V] or current [mA])
		public const uint AD_DATA_BIN8				 = 2;	// 8-bit binary
		public const uint AD_DATA_BIN12				 = 3;	// 12-bit binary
		public const uint AD_DATA_BIN16				 = 4;	// 16-bit binary
		public const uint AD_DATA_BIN24				 = 5;	// 24-bit binary
		public const uint AD_DATA_BIN10				 = 6;	// 10-bit binary

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Data Conversion Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_CONV_SMOOTH			 = 1;		// Converts the data with interpolation.
		public const uint AD_CONV_AVERAGE1			 = 0x100;	// Converts the data with the simple averaging.
		public const uint AD_CONV_AVERAGE2			 = 0x200;	// Converts the data with the shifted averaging.

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Sampling Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_IO_SAMPLING			 = 1;	// Programmed I/O
		public const uint AD_FIFO_SAMPLING			 = 2;	// FIFO
		public const uint AD_MEM_SAMPLING			 = 4;	// Memory
		public const uint AD_BM_SAMPLING			 = 8;	// Bus master

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Trigger Timing Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_TRIG_START				 = 1;	// Start-trigger (default setting)
		public const uint AD_TRIG_STOP				 = 2;	// Stop-trigger
		public const uint AD_TRIG_START_STOP		 = 3;	// Start/stop-trigger

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Trigger Level Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_FREERUN			 = 1;	// No trigger (default setting)
		public const uint AD_EXTTRG				 = 2;	// External trigger
		public const uint AD_EXTTRG_DI			 = 3;	// External trigger with mask using general purpose digital input pin
		public const uint AD_LEVEL_P			 = 4;	// Analog trigger (low-to-high transition)
		public const uint AD_LEVEL_M			 = 5;	// Analog trigger (high-to-low transition)
		public const uint AD_LEVEL_D			 = 6;	// Analog trigger (low-to-high or high-to-low transition)
		public const uint AD_INRANGE			 = 7;	// Analog trigger (into the range)
		public const uint AD_OUTRANGE			 = 8;	// Analog trigger (out of the range)
		public const uint AD_ETERNITY			 = 9;	// Infinite sampling
		public const uint AD_SMPLNUM			 = 10;	// Specified number
		public const uint AD_START_SIGTIMER		 = 11;	// Interval timer
		public const uint AD_START_DA_START		 = 12;	// Analog output start (DaStartSampling)
		public const uint AD_START_DA_STOP		 = 13;	// Analog output stop
		public const uint AD_START_DA_IO		 = 14;	// Analog output (DaOutputDA)
		public const uint AD_START_DA_SMPLNUM	 = 15;	// Analog output number
		public const uint AD_STOP_DA_START		 = 12;	// Analog output start (DaStartSampling)
		public const uint AD_STOP_DA_STOP		 = 13;	// Analog output stop
		public const uint AD_STOP_DA_IO			 = 14;	// Analog output (DaOutputDA)
		public const uint AD_STOP_DA_SMPLNUM	 = 15;	// Analog output number
		public const uint AD_START_P1			 = 0x00000010;	// Start-trigger (Level 1): low-to- high transition
		public const uint AD_START_M1			 = 0x00000020;	// Start-trigger (Level 1): high-to-low transition
		public const uint AD_START_D1			 = 0x00000040;	// Start-trigger (Level 1): high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_START_P2			 = 0x00000080;	// Start-trigger (Level 2): low-to- high transition
		public const uint AD_START_M2			 = 0x00000100;	// Start-trigger (Level 2): high-to-low transition
		public const uint AD_START_D2			 = 0x00000200;	// Start-trigger (Level 2): high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_STOP_P1			 = 0x00000400;	// Stop-trigger (Level 1): low-to-high transition
		public const uint AD_STOP_M1			 = 0x00000800;	// Stop-trigger (Level 1): high-to-low transition
		public const uint AD_STOP_D1			 = 0x00001000;	// Stop-trigger (Level 1): high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_STOP_P2			 = 0x00002000;	// Stop-trigger (Level 2): low-to-high transition
		public const uint AD_STOP_M2			 = 0x00004000;	// Stop-trigger (Level 2): high-to-low transition
		public const uint AD_STOP_D2			 = 0x00008000;	// Stop-trigger (Level 2): high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_ANALOG_FILTER		 = 0x00010000;	// Uses an analog trigger filter.
		public const uint AD_START_CNT_EQ		 = 0x00020000;	// Start-trigger: Counter equal
		public const uint AD_STOP_CNT_EQ		 = 0x00040000;	// Stop-trigger: Counter equal
		public const uint AD_START_DI_EQ		 = 0x00080000;	// Stop-trigger: DI equal
		public const uint AD_STOP_DI_EQ			 = 0x00100000;	// Stop-trigger: DI equal
		public const uint AD_STOP_SOFT			 = 0x00200000;	// Stop-trigger: Soft stop
		public const uint AD_START_Z_CLR		 = 0x00400000;	// Start-trigger: Z clear
		public const uint AD_STOP_Z_CLR			 = 0x00800000;	// Stop-trigger: Z clear

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_DOWN_EDGE	 = 	1;	// Falling edge (default setting)
		public const uint AD_UP_EDGE	 = 	2;	// Rising edge
		public const uint AD_EXTRG_IN	 = 	3;	// External trigger input
		public const uint AD_EXCLK_IN	 = 	4;	// External clock input
		public const uint AD_LOW_LEVEL	 =  5;	// Negative level (default setting)
		public const uint AD_HIGH_LEVEL	 =  6;	// Positive level
		
		public const uint AD_EDGE_P1	 = 	0x0010;	// Level 1: low-to-high transition
		public const uint AD_EDGE_M1	 = 	0x0020;	// Level 1: high-to-low transition
		public const uint AD_EDGE_D1	 = 	0x0040;	// Level 1: high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_EDGE_P2	 = 	0x0080;	// Level 2: low-to-high transition
		public const uint AD_EDGE_M2	 = 	0x0100;	// Level 2: high-to-low transition
		public const uint AD_EDGE_D2	 = 	0x0200;	//  Level 2: high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_DISABLE	 = 	0x80000000;	// No pulse output (default setting)
		
		public const uint AD_TRIG_MODE	 = 2;	//
		public const uint AD_BUSY_MODE   = 3;	//
		public const uint AD_POST_MODE	 = 4;	//
		public const uint AD_ENABLE		 = 5;	// 
		public const uint AD_SMP1_MODE	 = 6;	//
		public const uint AD_SMP2_MODE	 = 7;	//

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Pulse Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_LOW_PULSE	 = 1;	// Negative pulse (default setting)
		public const uint AD_HIGH_PULSE	 = 2;	// Positive pulse

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Double-Clocked Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_NORMAL_MODE = 	1;	// Normal mode (default setting)
		public const uint AD_FAST_MODE	 = 	2;	// Double-clocked mode


		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Status Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_NO_STATUS	 = 1;	// Adds no bus master sampling status. (default setting)
		public const uint AD_ADD_STATUS	 = 2;	// Adds bus master sampling status.

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Error Control Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_STOP_SCER	 = 2;	// Stops sampling by a sampling clock error.
		public const uint AD_STOP_ORER	 = 4;	// Stops sampling by an overrun error.


		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Data Save Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_APPEND		 = 1;	// Adds new data at the end of the buffer. (default setting)
		public const uint AD_OVERWRITE	 = 2;	// Overwrites new data on existing data.

		//-----------------------------------------------------------------------------------------------
		//
		//		GPC/GPF-3100 Degital Filter Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_DF_8   = 0;	// 8 (default setting)
		public const uint AD_DF_16  = 1;	// 16
		public const uint AD_DF_32  = 2;	// 32
		public const uint AD_DF_64  = 3;	// 64
		public const uint AD_DF_128 = 4;	// 128
		public const uint AD_DF_256 = 5;	// 256

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Range Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_0_1V		 = 0x00000001;	// Voltage: unipolar 0 V to 1 V
		public const uint AD_0_2P5V		 = 0x00000002;	// Voltage: unipolar 0 V to 2.5 V
		public const uint AD_0_5V		 = 0x00000004;	// Voltage: unipolar 0 V to 5 V
		public const uint AD_0_10V		 = 0x00000008;	// Voltage: unipolar 0 V to 10 V
		public const uint AD_1_5V		 = 0x00000010;	// Voltage: unipolar 1 V to 5 V
		public const uint AD_0_2V		 = 0x00000020;	// Voltage: unipolar 0 V to 2 V
		public const uint AD_0_0P125V	 = 0x00000040;	// Voltage: unipolar 0 V to 0.125 V
		public const uint AD_0_1P25V	 = 0x00000080;	// Voltage: unipolar 0 V to 1.25 V
		public const uint AD_0_0P625V	 = 0x00000100;	// Voltage: unipolar 0 V to 0.625 V
		public const uint AD_0_0P156V	 = 0x00000200;	// Voltage: unipolar 0 V to 0.156 V
		public const uint AD_0_20mA		 = 0x00001000;	// Current: unipolar 0 mA to 20 mA
		public const uint AD_4_20mA		 = 0x00002000;	// Current: unipolar 4 mA to 20 mA
		public const uint AD_1V			 = 0x00010000;	// Voltage: bipolar +/- 1 V
		public const uint AD_2P5V		 = 0x00020000;	// Voltage: bipolar +/- 2.5 V
		public const uint AD_5V			 = 0x00040000;	// Voltage: bipolar +/- 5 V
		public const uint AD_10V		 = 0x00080000;	// Voltage: bipolar +/- 10 V
		public const uint AD_20V		 = 0x00100000;	// Voltage: bipolar +/- 20 V
		public const uint AD_50V		 = 0x00200000;	// Voltage: bipolar +/- 50 V
		public const uint AD_0P125V		 = 0x00400000;	// Voltage: bipolar +/- 0.125 V
		public const uint AD_1P25V		 = 0x00800000;	// Voltage: bipolar +/- 1.25 V
		public const uint AD_0P625V		 = 0x01000000;	// Voltage: bipolar +/- 0.625 V
		public const uint AD_0P156V		 = 0x02000000;	// Voltage: bipolar +/- 0.156 V
		public const uint AD_1P25V_AC	 = 0x04000000;	// Voltage: bipolar +/- 1.25 V(AC Coupling)
		public const uint AD_0P625V_AC	 = 0x08000000;	// Voltage: bipolar +/- 0.625 V(AC Coupling)
		public const uint AD_0P156V_AC	 = 0x10000000;	// Voltage: bipolar +/- 0.156 V(AC Coupling)
		public const uint AD_AC_COUPLING = 0x40000000;	// AC Coupling
		public const uint AD_GND		 = 0x80000000;	// Voltage: GND

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Isolation Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_ISOLATION		 = 1;	// Isolated
		public const uint AD_NOT_ISOLATION	 = 2;	// Not isolated

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Synchronous Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_MASTER_MODE	 = 1;	// Master mode
		public const uint AD_SLAVE_MODE		 = 2;	// Slave mode

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Synchronous Number Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_SYNC_NUM_1		 = 0x0100;
		public const uint AD_SYNC_NUM_2		 = 0x0200;
		public const uint AD_SYNC_NUM_3		 = 0x0400;
		public const uint AD_SYNC_NUM_4		 = 0x0800;
		public const uint AD_SYNC_NUM_5		 = 0x1000;
		public const uint AD_SYNC_NUM_6		 = 0x2000;
		public const uint AD_SYNC_NUM_7		 = 0x4000;

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Calibration Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_SELF_CALIBRATION		 = 1;	// Self calibration of the AD converter
		public const uint AD_ZEROSCALE_CALIBRATION	 = 2;	// Zero voltage calibration (system calibration)
		public const uint AD_FULLSCALE_CALIBRATION	 = 3;	// Full scale voltage calibration (system calibration)

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Full-scale Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_STATUS_NEGATIVE_FULL_SCALE	 = 1;	// Negative full-scale
		public const uint AD_STATUS_POSITIVE_FULL_SCALE	 = 2;	// Positive full-scale
		public const uint AD_STATUS_UNDER_RANGE			 = 1;	// Negative full-scale
		public const uint AD_STATUS_OVER_RANGE			 = 2;	// Positive full-scale

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 OVPM Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_STATUS_OVPM_NORMAL	   	     = 0;	// NormalRange
		public const uint AD_STATUS_OVPM_HIGH_RANGE	     = 1;	// Voltage: bipolar +/- 50 V Range
		public const uint AD_STATUS_OVPM_GND_RANGE	     = 2;	// GND

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 PCI-3525 CN3,4 Function Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_CN_FREE		= 0;	// Not used
		public const uint AD_CN_EXTRG_IN	= 1;	// External trigger input
		public const uint AD_CN_EXTRG_OUT	= 2;	// External trigger output
		public const uint AD_CN_EXCLK_IN	= 3;	// External clock input
		public const uint AD_CN_EXCLK_OUT	= 4;	// External clock output
		public const uint AD_CN_EXINT_IN	= 5;	// External interrupt input
		public const uint AD_CN_ATRG_OUT	= 6;	// Analog trigger out
		public const uint AD_CN_DI			= 7;	// Digital input
		public const uint AD_CN_DO			= 8;	// Digital output
		public const uint AD_CN_DAOUT		= 9;	// Analog output
		public const uint AD_CN_OPEN		= 10;	// Open
		public const uint AD_CN_EXSMP1_OUT	= 12;	// Sampling Status1
		public const uint AD_CN_EXSMP2_OUT	= 13;	// Sampling Status2
		public const uint AD_CN_DIO			= 1;	// DIO
		public const uint AD_CN_CONTROL		= 2;	// Control used
		public const uint AD_CN_CNT			= 3;	// Counter used
		
		//-----------------------------------------------------------------------------------------------
		//
		//		GPC/GPF-3100 CPZ-360810 DIN/DOUT Function Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_EX_DIO1 = 	1;	// DIN/DOUT1
		public const uint AD_EX_DIO2 = 	2;	// DIN/DOUT2
		public const uint AD_EX_DIO3 = 	3;	// DIN/DOUT3
		public const uint AD_EX_DIO4 = 	4;	// DIN/DOUT4
		public const uint AD_EX_DIO5 = 	5;	// DIN/DOUT5
		public const uint AD_EX_DIO6 = 	6;	// DIN/DOUT6
		public const uint AD_EX_DIO7 = 	7;	// DIN/DOUT7
		public const uint AD_EX_DIO8 = 	8;	// DIN/DOUT8
		
		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Error Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_ERROR_SUCCESS				 = 	0x00000000;
		public const uint AD_ERROR_NOT_DEVICE			 = 	0xC0000001;
		public const uint AD_ERROR_NOT_OPEN				 = 	0xC0000002;
		public const uint AD_ERROR_INVALID_HANDLE		 = 	0xC0000003;
		public const uint AD_ERROR_ALREADY_OPEN			 = 	0xC0000004;
		public const uint AD_ERROR_NOT_SUPPORTED		 = 	0xC0000009;
		public const uint AD_ERROR_NOW_SAMPLING			 = 	0xC0001001;
		public const uint AD_ERROR_STOP_SAMPLING		 = 	0xC0001002;
		public const uint AD_ERROR_START_SAMPLING		 = 	0xC0001003;
		public const uint AD_ERROR_SAMPLING_TIMEOUT		 = 	0xC0001004;
		public const uint AD_ERROR_SAMPLING_FREQ		 = 	0xC0001005;
		public const uint AD_ERROR_INVALID_PARAMETER	 = 	0xC0001021;
		public const uint AD_ERROR_ILLEGAL_PARAMETER	 = 	0xC0001022;
		public const uint AD_ERROR_NULL_POINTER			 = 	0xC0001023;
		public const uint AD_ERROR_GET_DATA				 = 	0xC0001024;
		public const uint AD_ERROR_USED_DA				 = 	0xC0001025;
		public const uint AD_ERROR_FILE_OPEN			 = 	0xC0001041;
		public const uint AD_ERROR_FILE_CLOSE			 = 	0xC0001042;
		public const uint AD_ERROR_FILE_READ			 = 	0xC0001043;
		public const uint AD_ERROR_FILE_WRITE			 = 	0xC0001044;
		public const uint AD_ERROR_INVALID_DATA_FORMAT	 = 	0xC0001061;
		public const uint AD_ERROR_INVALID_AVERAGE_OR_SMOOTHING	 = 0xC0001062;
		public const uint AD_ERROR_INVALID_SOURCE_DATA	 = 	0xC0001063;
		public const uint AD_ERROR_NOT_ALLOCATE_MEMORY	 = 	0xC0001081;
		public const uint AD_ERROR_NOT_LOAD_DLL			 = 	0xC0001082;
		public const uint AD_ERROR_CALL_DLL				 = 	0xC0001083;

		// -----------------------------------------------------------------------
		//
		//     User-supplied Function
		//
		// -----------------------------------------------------------------------
		public delegate void LPCONVPROC(
		short wCh,
			uint dwCount,
			IntPtr lpData
			);

		public delegate void LPADCALLBACK(uint dwUser);

		// -----------------------------------------------------------------------
		//     Sampling Condition Structure for Each Channel
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADSMPLCHREQ
		{
			public uint	ulChNo;
			public uint	ulRange;
		}

		// -----------------------------------------------------------------------
		//     Analog Trigger Condition Structure for Each Channel
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADTRIGCHREQ
		{
			public uint		ulChNo;
			public float	fTrigLevel;
			public float	fHysteresis;
		}

		// -----------------------------------------------------------------------
		//     Sampling Condition Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADSMPLREQ
		{
			public uint			ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public uint			ulSamplingMode;
			public uint			ulSingleDiff;
			public uint			ulSmplNum;
			public uint			ulSmplEventNum;
			public float		fSmplFreq;
			public uint			ulTrigPoint;
			public uint			ulTrigMode;
			public int			lTrigDelay;
			public uint			ulTrigCh;
			public float		fTrigLevel1;
			public float		fTrigLevel2;
			public uint			ulEClkEdge;
			public uint			ulATrgPulse;
			public uint			ulTrigEdge;
			public uint			ulTrigDI;
			public uint			ulFastMode;

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
		//     Sampling Condition Structure for Bus Master
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADBMSMPLREQ
		{
			public uint			ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public uint			ulSingleDiff;
			public uint			ulSmplNum;
			public uint			ulSmplEventNum;
			public uint			ulSmplRepeat;
			public uint			ulBufferMode;
			public float		fSmplFreq;
			public float		fScanFreq;
			public uint			ulStartMode;
			public uint			ulStopMode;
			public uint			ulPreTrigDelay;
			public uint			ulPostTrigDelay;
			public uint			ulChNo1;           // Flatizing TrigChReq[0].ulChNo
			public float		fTrigLevel1;       // Flatizing TrigChReq[0].fTrigLevel
			public float		fHysteresis1;      // Flatizing TrigChReq[0].fHysteresis
			public uint			ulChNo2;           // Flatizing TrigChReq[1].ulChNo
			public float		fTrigLevel2;       // Flatizing TrigChReq[1].fTrigLevel
			public float		fHysteresis2;      // Flatizing TrigChReq[1].fHysteresis
			public uint			ulATrgMode;
			public uint			ulATrgPulse;
			public uint			ulStartTrigEdge;
			public uint			ulStopTrigEdge;
			public uint			ulTrigDI;
			public uint			ulEClkEdge;
			public uint			ulFastMode;
			public uint			ulStatusMode;
			public uint			ulErrCtrl;

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
		//     Sampling Condition Structure for Memory
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
			public struct ADMEMSMPLREQ
		{
			public uint		ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public uint		ulSingleDiff;
			public float	fSmplFreq;
			public uint		ulStopMode;
			public uint		ulPreTrigDelay;
			public uint		ulPostTrigDelay;
			public uint		ulChNo1;           // Flatizing TrigChReq[0].ulChNo
			public float	fTrigLevel1;       // Flatizing TrigChReq[0].fTrigLevel
			public float	fHysteresis1;      // Flatizing TrigChReq[0].fHysteresis
			public uint		ulChNo2;           // Flatizing TrigChReq[1].ulChNo
			public float	fTrigLevel2;       // Flatizing TrigChReq[1].fTrigLevel
			public float	fHysteresis2;      // Flatizing TrigChReq[1].fHysteresis
			public uint		ulATrgMode;
			public uint		ulATrgPulse;
			public uint		ulStopTrigEdge;
			public uint		ulEClkEdge;
			public uint		ulFastMode;
			public uint		ulStatusMode;
			public uint		ulErrCtrl;

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
		//     Board Specification Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADBOARDSPEC
		{
			public uint ulBoardType;
			public uint	ulBoardID;
			public uint	dwSamplingMode;
			public uint	ulChCountS;
			public uint	ulChCountD;
			public uint	ulResolution;
			public uint	dwRange;
			public uint	ulIsolation;
			public uint	ulDi;
			public uint	ulDo;
		}


		//-----------------------------------------------------------------------------------------------
		//
		//   GPC/GPF-3100 Function Declaration
		//
		//-----------------------------------------------------------------------------------------------
		[DllImport("fbiad.dll")]
		public static extern IntPtr AdOpen(string szName);
		[DllImport("fbiad.dll")]
		public static extern int AdClose(IntPtr hDevice);
		
		[DllImport("fbiad.dll")]
		public static extern int AdGetDeviceInfo(IntPtr hDevice, out ADBOARDSPEC BoardSpec);
		
		[DllImport("fbiad.dll")]
		public static extern int AdSetBoardConfig(IntPtr hDevice, IntPtr hEvent, LPADCALLBACK lpCallBackProc, uint dwUser);
		[DllImport("fbiad.dll")]
		public static extern int AdSetBoardConfig(IntPtr hDevice, IntPtr hEvent, uint lpCallBackProc, uint dwUser);
		
		[DllImport("fbiad.dll")]
		public static extern int AdGetBoardConfig(IntPtr hDevice, out uint ulAdSmplEventFactor);
		
		[DllImport("fbiad.dll")]
		public static extern int AdSetSamplingConfig(IntPtr hDevice, ref ADSMPLREQ AdSmplConfig);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingConfig(IntPtr hDevice, out ADSMPLREQ AdSmplConfig);
		
		[DllImport("fbiad.dll")]
		public static extern int AdBmSetSamplingConfig(IntPtr hDevice, ref ADBMSMPLREQ AdBmSmplConfig);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingConfig(IntPtr hDevice, out ADBMSMPLREQ AdBmSmplConfig);
		
		[DllImport("fbiad.dll")]
		public static extern int AdMemSetSamplingConfig(IntPtr hDevice, ref ADMEMSMPLREQ AdMemSmplConfig);
		[DllImport("fbiad.dll")]
		public static extern int AdMemGetSamplingConfig(IntPtr hDevice, out ADMEMSMPLREQ AdMemSmplConfig);
		
		[DllImport("fbiad.dll")]
		public static extern int AdLvSetSamplingConfig(IntPtr hDevice, uint ulChNo, uint ulSmplNum, float fSmplFreq, uint ulRange, uint hEvent, LPADCALLBACK lpCallBackProc, uint dwUser);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingConfig(IntPtr hDevice, uint ulChNo, out uint ulSmplNum, out float fSmplFreq, out uint ulRange);
		
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(IntPtr hDevice, out byte pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(IntPtr hDevice, out ushort pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(IntPtr hDevice, out uint pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(IntPtr hDevice, byte[] pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(IntPtr hDevice, ushort[] pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(IntPtr hDevice, uint[] pSmplData, ref uint ulSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(IntPtr hDevice, out byte pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(IntPtr hDevice, out ushort pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(IntPtr hDevice, out uint pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(IntPtr hDevice, byte[] pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(IntPtr hDevice, ushort[] pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(IntPtr hDevice, uint[] pBmSmplData, ref uint ulBmSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(IntPtr hDevice, uint ulChNo, ref byte pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(IntPtr hDevice, uint ulChNo, ref ushort pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(IntPtr hDevice, uint ulChNo, ref uint pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(IntPtr hDevice, uint ulChNo, byte[] pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(IntPtr hDevice, uint ulChNo, ushort[] pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(IntPtr hDevice, uint ulChNo, uint[] pSmplData, ref uint pulSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(IntPtr hDevice, out byte pSmplData, ref byte pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(IntPtr hDevice, out ushort pSmplData, ref byte pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(IntPtr hDevice, out uint pSmplData, ref byte pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(IntPtr hDevice, byte[] pSmplData, byte[] pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(IntPtr hDevice, ushort[] pSmplData, byte[] pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(IntPtr hDevice, uint[] pSmplData, byte[] pDiData, ref uint ulSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(IntPtr hDevice, uint lOffset, ref uint ulSmplNum, out byte pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(IntPtr hDevice, uint lOffset, ref uint ulSmplNum, out ushort pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(IntPtr hDevice, uint lOffset, ref uint ulSmplNum, out uint pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(IntPtr hDevice, uint lOffset, ref uint ulSmplNum, byte[] pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(IntPtr hDevice, uint lOffset, ref uint ulSmplNum, ushort[] pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(IntPtr hDevice, uint lOffset, ref uint ulSmplNum, uint[] pSmplData);
		
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(IntPtr hDevice, out byte pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(IntPtr hDevice, out ushort pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(IntPtr hDevice, out uint pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(IntPtr hDevice, byte[] pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(IntPtr hDevice, ushort[] pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(IntPtr hDevice, uint[] pSmplData, uint ulSize);
		
		[DllImport("fbiad.dll")]
		public static extern int AdClearSamplingData(IntPtr hDevice);
		[DllImport("fbiad.dll")]
		public static extern int AdStartSampling(IntPtr hDevice, uint ulSyncFlag);
		[DllImport("fbiad.dll")]
		public static extern int AdStartFileSampling(IntPtr hDevice, string pszPathName, uint ulFileFlag);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartFileSampling(IntPtr hDevice, string szPathName, uint ulFileFlag);
		[DllImport("fbiad.dll")]
		public static extern int AdSyncSampling(IntPtr hDevice, uint ulMode);
		[DllImport("fbiad.dll")]
		public static extern int AdLvStartSampling(IntPtr hDevice, uint ulChNo);
		[DllImport("fbiad.dll")]
		public static extern int AdStopSampling(IntPtr hDevice);
		[DllImport("fbiad.dll")]
		public static extern int AdLvStopSampling(IntPtr hDevice, uint ulChNo);
		[DllImport("fbiad.dll")]
		public static extern int AdTriggerSampling(IntPtr hDevice, uint ulChNo, uint ulRange, uint ulSingleDiff, uint ulTriggerMode, uint ulTrigEdge, uint ulSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int AdMemTriggerSampling(IntPtr hDevice, uint ulChCount, ref ADSMPLCHREQ lpSmplChReq, uint ulSmplNum, uint ulRepeatCount, uint ulTrigEdge, float fSmplFreq, uint ulEClkEdge, uint ulFastMode);
		[DllImport("fbiad.dll")]
		public static extern int AdMemTriggerSampling(IntPtr hDevice, uint ulChCount, ADSMPLCHREQ[] lpSmplChReq, uint ulSmplNum, uint ulRepeatCount, uint ulTrigEdge, float fSmplFreq, uint ulEClkEdge, uint ulFastMode);
		
		[DllImport("fbiad.dll")]
		public static extern int AdGetStatus(IntPtr hDevice, out uint ulAdSmplStatus, out uint ulAdSmplCount, out uint ulAdAvailCount);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetStatus(IntPtr hDevice, uint ulChNo, out uint ulAdSmplStatus, out uint ulAdSmplCount, out uint ulAdAvailCount);
		
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(IntPtr hDevice, uint ulCh, uint ulSingleDiff, ref ADSMPLCHREQ AdSmplChReq, out byte lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(IntPtr hDevice, uint ulCh, uint ulSingleDiff, ref ADSMPLCHREQ AdSmplChReq, out ushort lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(IntPtr hDevice, uint ulCh, uint ulSingleDiff, ref ADSMPLCHREQ AdSmplChReq, out uint lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(IntPtr hDevice, uint ulCh, uint ulSingleDiff, ADSMPLCHREQ[] AdSmplChReq, byte[] lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(IntPtr hDevice, uint ulCh, uint ulSingleDiff, ADSMPLCHREQ[] AdSmplChReq, ushort[] lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(IntPtr hDevice, uint ulCh, uint ulSingleDiff, ADSMPLCHREQ[] AdSmplChReq, uint[] lpData);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdSetRangeEvent(IntPtr hDevice, uint dwEventMask, uint dwStopMode);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetRangeEventStatus(IntPtr hDevice, uint[] ulEventChNo, uint[] ulEventStatus);
		[DllImport("fbiad.dll")]
		public static extern int  AdResetRangeEvent(IntPtr hDevice);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdGetOverRangeChStatus(IntPtr hDevice, out uint ulEventStatus);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetOverRangeChStatus(IntPtr hDevice, uint[] ulEventStatus);
		[DllImport("fbiad.dll")]
		public static extern int  AdResetOverRangeCh(IntPtr hDevice);
		
		[DllImport("fbiad.dll")]
		public static extern int AdLvCalibration(IntPtr hDevice, uint ulChNo, uint ulCalibration);
		
		[DllImport("fbiad.dll")]
		public static extern int AdMeasureTemperature(IntPtr hDevice, out float fTemperature);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdSetInterval(IntPtr hDevice, uint ulInterval);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetInterval(IntPtr hDevice, out uint ulInterval);
		[DllImport("fbiad.dll")]
		public static extern int  AdSetFunction(IntPtr hDevice, uint ulChNo, uint ulFunction);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetFunction(IntPtr hDevice, uint ulChNo, out uint ulFunction);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdSetFilter(IntPtr hDevice, uint ulFilter);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetFilter(IntPtr hDevice, out uint ulFilter);
		
		[DllImport("fbiad.dll")]
		public static extern int AdSetOutMode(IntPtr hDevice, uint ulExTrgMode, uint ulExClkMode);
		[DllImport("fbiad.dll")]
		public static extern int AdGetOutMode(IntPtr hDevice, out uint ulExTrgMode , out uint ulExClkMode);
		[DllImport("fbiad.dll")]
		public static extern int AdMemSetDiPattern(IntPtr hDevice, uint ulCh, out uint ulPatternTrig);
		[DllImport("fbiad.dll")]
		public static extern int AdMemGetDiPattern(IntPtr hDevice, out uint ulPatternTrig);
		[DllImport("fbiad.dll")]
		public static extern int AdMemGetDiPattern(IntPtr hDevice, uint[] ulPatternTrig);
		
		[DllImport("fbiad.dll")]
		public static extern int AdInputDI(IntPtr hDevice, out uint dwData);
		[DllImport("fbiad.dll")]
		public static extern int AdOutputDO(IntPtr hDevice, uint dwData);
		[DllImport("fbiad.dll")]
		public static extern int AdAdjustVR(IntPtr hDevice, uint ulAdjustCh, uint ulSingleDiff, uint ulSelVolume, uint ulControl, uint ulTap);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdAllocateSamplingBuffer(IntPtr hDevice);
		
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, out byte pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, out ushort pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, out uint pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, out float pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, byte[] pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, ushort[] pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, uint[] pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, float[] pSmplData, uint uFormCode);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdCommonGetPciDeviceInfo(IntPtr hDevice, out uint dwDeviceID, out uint dwVendorID, out uint dwClassCode, out uint dwRevisionID, out uint dwBaseAddress0, out uint dwBaseAddress1, out uint dwBaseAddress2, out uint dwBaseAddress3, out uint dwBaseAddress4, out uint dwBaseAddress5, out uint dwSubsystemID, out uint dwSubsystemVendorID, out uint dwInterruptLine, out uint dwBoardID);

		[DllImport("kernel32.dll",EntryPoint="CreateEventA")]
		public static extern IntPtr CreateEvent(uint lpEventAttributes, bool ManualReset, bool bInitialState, string lpName);
		[DllImport("kernel32.dll")]
		public static extern uint  WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);
		[DllImport("kernel32.dll")]
		public static extern bool ResetEvent(IntPtr hEvent);
	}
	
	public class IFCAD
	{
		private IFCAD(){}

		public const uint INVALID_HANDLE_VALUE = 0xFFFFFFFF;
		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Overlapped Process Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint FLAG_SYNC		=	1;	// Sampling as an non-overlapped operation
		public const uint FLAG_ASYNC	=	2;	// Sampling as overlapped operation

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 File Format Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint FLAG_BIN	 = 	1;	// Binary format file
		public const uint FLAG_CSV	 = 	2;	// CSV format file

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Sampling Status Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_STATUS_STOP_SAMPLING		 = 1;	// The sampling is stopped.
		public const uint AD_STATUS_WAIT_TRIGGER		 = 2;	// The sampling is waiting for a trigger.
		public const uint AD_STATUS_NOW_SAMPLING		 = 3;	// The sampling is running.

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Event Factor Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_EVENT_SMPLNUM			 = 1;	// The specified number of samples has been acquired.
		public const uint AD_EVENT_STOP_TRIGGER		 = 2;	// The sampling has been stopped because a trigger is asserted.
		public const uint AD_EVENT_STOP_FUNCTION	 = 3;	// The sampling has been stopped by software.
		public const uint AD_EVENT_STOP_TIMEOUT		 = 4;	// The sampling has been stopped because a time-out interval elapsed.
		public const uint AD_EVENT_STOP_SAMPLING	 = 5;	// The sampling is stopped.
		public const uint AD_EVENT_STOP_SCER		 = 6;	// The sampling is stopped by a clock error.
		public const uint AD_EVENT_STOP_ORER		 = 7;	// The sampling is stopped by an overrun error.
		public const uint AD_EVENT_SCER				 = 8;	// The sampling pacer clock error is occurred.
		public const uint AD_EVENT_ORER				 = 9;	// The overrun error is occurred.
		public const uint AD_EVENT_STOP_LV_1		 = 10;	// The channel 1 sampling is stopped. (Only applicable to the PCI-3179)
		public const uint AD_EVENT_STOP_LV_2		 = 11;	// The channel 2 sampling is stopped. (Only applicable to the PCI-3179)
		public const uint AD_EVENT_STOP_LV_3		 = 12;	// The channel 3 sampling is stopped. (Only applicable to the PCI-3179)
		public const uint AD_EVENT_STOP_LV_4		 = 13;	// The channel 4 sampling is stopped. (Only applicable to the PCI-3179)
		public const uint AD_EVENT_RANGE			 = 14;	// The AD conversion value reached the full-scale range.
		public const uint AD_EVENT_STOP_RANGE		 = 15;	// The sampling is stopped by the full-scale range detection.
		public const uint AD_EVENT_OVPM				 = 16;	// 過電圧入力時自動レンジ切替機能検出
		public const uint AD_EVENT_STOP_OVPM		 = 17;	// 過電圧入力時自動レンジ切替機能検出によりサンプリング終了

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Input Configuration Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_INPUT_SINGLE			 = 1;	// Single-ended input
		public const uint AD_INPUT_DIFF				 = 2;	// Differential input

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Volume Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_ADJUST_BIOFFSET		 = 1;	// Bipolar offset
		public const uint AD_ADJUST_UNIOFFSET		 = 2;	// Unipolar offset
		public const uint AD_ADJUST_BIGAIN			 = 3;	// Bipolar gain
		public const uint AD_ADJUST_UNIGAIN			 = 4;	// Unipolar gain

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Calibration Item Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_ADJUST_UP				 = 1;	// Increases the volume.
		public const uint AD_ADJUST_DOWN			 = 2;	// Decreases the volumde.
		public const uint AD_ADJUST_STORE			 = 3;	// Saves the present value to the non-volatile memory.
		public const uint AD_ADJUST_STANDBY			 = 4;	// Places the electronic volume device into the standby mode.
		public const uint AD_ADJUST_NOT_STORE		 = 5;	// Not save the value.

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Data Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_DATA_PHYSICAL			 = 1;	// Physical value (voltage [V] or current [mA])
		public const uint AD_DATA_BIN8				 = 2;	// 8-bit binary
		public const uint AD_DATA_BIN12				 = 3;	// 12-bit binary
		public const uint AD_DATA_BIN16				 = 4;	// 16-bit binary
		public const uint AD_DATA_BIN24				 = 5;	// 24-bit binary
		public const uint AD_DATA_BIN10				 = 6;	// 10-bit binary

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Data Conversion Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_CONV_SMOOTH			 = 1;		// Converts the data with interpolation.
		public const uint AD_CONV_AVERAGE1			 = 0x100;	// Converts the data with the simple averaging.
		public const uint AD_CONV_AVERAGE2			 = 0x200;	// Converts the data with the shifted averaging.

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Sampling Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_IO_SAMPLING			 = 1;	// Programmed I/O
		public const uint AD_FIFO_SAMPLING			 = 2;	// FIFO
		public const uint AD_MEM_SAMPLING			 = 4;	// Memory
		public const uint AD_BM_SAMPLING			 = 8;	// Bus master

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Trigger Timing Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_TRIG_START				 = 1;	// Start-trigger (default setting)
		public const uint AD_TRIG_STOP				 = 2;	// Stop-trigger
		public const uint AD_TRIG_START_STOP		 = 3;	// Start/stop-trigger

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Trigger Level Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_FREERUN			 = 1;	// No trigger (default setting)
		public const uint AD_EXTTRG				 = 2;	// External trigger
		public const uint AD_EXTTRG_DI			 = 3;	// External trigger with mask using general purpose digital input pin
		public const uint AD_LEVEL_P			 = 4;	// Analog trigger (low-to-high transition)
		public const uint AD_LEVEL_M			 = 5;	// Analog trigger (high-to-low transition)
		public const uint AD_LEVEL_D			 = 6;	// Analog trigger (low-to-high or high-to-low transition)
		public const uint AD_INRANGE			 = 7;	// Analog trigger (into the range)
		public const uint AD_OUTRANGE			 = 8;	// Analog trigger (out of the range)
		public const uint AD_ETERNITY			 = 9;	// Infinite sampling
		public const uint AD_SMPLNUM			 = 10;	// Specified number
		public const uint AD_START_SIGTIMER		 = 11;	// Interval timer
		public const uint AD_START_DA_START		 = 12;	// Analog output start (DaStartSampling)
		public const uint AD_START_DA_STOP		 = 13;	// Analog output stop
		public const uint AD_START_DA_IO		 = 14;	// Analog output (DaOutputDA)
		public const uint AD_START_DA_SMPLNUM	 = 15;	// Analog output number
		public const uint AD_STOP_DA_START		 = 12;	// Analog output start (DaStartSampling)
		public const uint AD_STOP_DA_STOP		 = 13;	// Analog output stop
		public const uint AD_STOP_DA_IO			 = 14;	// Analog output (DaOutputDA)
		public const uint AD_STOP_DA_SMPLNUM	 = 15;	// Analog output number
		public const uint AD_START_P1			 = 0x00000010;	// Start-trigger (Level 1): low-to- high transition
		public const uint AD_START_M1			 = 0x00000020;	// Start-trigger (Level 1): high-to-low transition
		public const uint AD_START_D1			 = 0x00000040;	// Start-trigger (Level 1): high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_START_P2			 = 0x00000080;	// Start-trigger (Level 2): low-to- high transition
		public const uint AD_START_M2			 = 0x00000100;	// Start-trigger (Level 2): high-to-low transition
		public const uint AD_START_D2			 = 0x00000200;	// Start-trigger (Level 2): high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_STOP_P1			 = 0x00000400;	// Stop-trigger (Level 1): low-to-high transition
		public const uint AD_STOP_M1			 = 0x00000800;	// Stop-trigger (Level 1): high-to-low transition
		public const uint AD_STOP_D1			 = 0x00001000;	// Stop-trigger (Level 1): high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_STOP_P2			 = 0x00002000;	// Stop-trigger (Level 2): low-to-high transition
		public const uint AD_STOP_M2			 = 0x00004000;	// Stop-trigger (Level 2): high-to-low transition
		public const uint AD_STOP_D2			 = 0x00008000;	// Stop-trigger (Level 2): high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_ANALOG_FILTER		 = 0x00010000;	// Uses an analog trigger filter.
		public const uint AD_START_CNT_EQ		 = 0x00020000;	// Start-trigger: Counter equal
		public const uint AD_STOP_CNT_EQ		 = 0x00040000;	// Stop-trigger: Counter equal
		public const uint AD_START_DI_EQ		 = 0x00080000;	// Stop-trigger: DI equal
		public const uint AD_STOP_DI_EQ			 = 0x00100000;	// Stop-trigger: DI equal
		public const uint AD_STOP_SOFT			 = 0x00200000;	// Stop-trigger: Soft stop
		public const uint AD_START_Z_CLR		 = 0x00400000;	// Start-trigger: Z clear
		public const uint AD_STOP_Z_CLR			 = 0x00800000;	// Stop-trigger: Z clear

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_DOWN_EDGE	 = 	1;	// Falling edge (default setting)
		public const uint AD_UP_EDGE	 = 	2;	// Rising edge
		public const uint AD_EXTRG_IN	 = 	3;	// External trigger input
		public const uint AD_EXCLK_IN	 = 	4;	// External clock input
		public const uint AD_LOW_LEVEL	 =  5;	// Negative level (default setting)
		public const uint AD_HIGH_LEVEL	 =  6;	// Positive level
		
		public const uint AD_EDGE_P1	 = 	0x0010;	// Level 1: low-to-high transition
		public const uint AD_EDGE_M1	 = 	0x0020;	// Level 1: high-to-low transition
		public const uint AD_EDGE_D1	 = 	0x0040;	// Level 1: high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_EDGE_P2	 = 	0x0080;	// Level 2: low-to-high transition
		public const uint AD_EDGE_M2	 = 	0x0100;	// Level 2: high-to-low transition
		public const uint AD_EDGE_D2	 = 	0x0200;	//  Level 2: high-to-low or low-to-high transition (direction DON'T CARE)
		public const uint AD_DISABLE	 = 	0x80000000;	// No pulse output (default setting)
		
		public const uint AD_TRIG_MODE	 = 2;	//
		public const uint AD_BUSY_MODE   = 3;	//
		public const uint AD_POST_MODE	 = 4;	//
		public const uint AD_ENABLE		 = 5;	// 
		public const uint AD_SMP1_MODE	 = 6;	//
		public const uint AD_SMP2_MODE	 = 7;	//

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Pulse Polarity Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_LOW_PULSE	 = 1;	// Negative pulse (default setting)
		public const uint AD_HIGH_PULSE	 = 2;	// Positive pulse

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Double-Clocked Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_NORMAL_MODE = 	1;	// Normal mode (default setting)
		public const uint AD_FAST_MODE	 = 	2;	// Double-clocked mode


		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Status Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_NO_STATUS	 = 1;	// Adds no bus master sampling status. (default setting)
		public const uint AD_ADD_STATUS	 = 2;	// Adds bus master sampling status.

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Error Control Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_STOP_SCER	 = 2;	// Stops sampling by a sampling clock error.
		public const uint AD_STOP_ORER	 = 4;	// Stops sampling by an overrun error.


		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Data Save Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_APPEND		 = 1;	// Adds new data at the end of the buffer. (default setting)
		public const uint AD_OVERWRITE	 = 2;	// Overwrites new data on existing data.

		//-----------------------------------------------------------------------------------------------
		//
		//		GPC/GPF-3100 Degital Filter Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_DF_8   = 0;	// 8 (default setting)
		public const uint AD_DF_16  = 1;	// 16
		public const uint AD_DF_32  = 2;	// 32
		public const uint AD_DF_64  = 3;	// 64
		public const uint AD_DF_128 = 4;	// 128
		public const uint AD_DF_256 = 5;	// 256

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Range Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_0_1V		 = 0x00000001;	// Voltage: unipolar 0 V to 1 V
		public const uint AD_0_2P5V		 = 0x00000002;	// Voltage: unipolar 0 V to 2.5 V
		public const uint AD_0_5V		 = 0x00000004;	// Voltage: unipolar 0 V to 5 V
		public const uint AD_0_10V		 = 0x00000008;	// Voltage: unipolar 0 V to 10 V
		public const uint AD_1_5V		 = 0x00000010;	// Voltage: unipolar 1 V to 5 V
		public const uint AD_0_2V		 = 0x00000020;	// Voltage: unipolar 0 V to 2 V
		public const uint AD_0_0P125V	 = 0x00000040;	// Voltage: unipolar 0 V to 0.125 V
		public const uint AD_0_1P25V	 = 0x00000080;	// Voltage: unipolar 0 V to 1.25 V
		public const uint AD_0_0P625V	 = 0x00000100;	// Voltage: unipolar 0 V to 0.625 V
		public const uint AD_0_0P156V	 = 0x00000200;	// Voltage: unipolar 0 V to 0.156 V
		public const uint AD_0_20mA		 = 0x00001000;	// Current: unipolar 0 mA to 20 mA
		public const uint AD_4_20mA		 = 0x00002000;	// Current: unipolar 4 mA to 20 mA
		public const uint AD_1V			 = 0x00010000;	// Voltage: bipolar +/- 1 V
		public const uint AD_2P5V		 = 0x00020000;	// Voltage: bipolar +/- 2.5 V
		public const uint AD_5V			 = 0x00040000;	// Voltage: bipolar +/- 5 V
		public const uint AD_10V		 = 0x00080000;	// Voltage: bipolar +/- 10 V
		public const uint AD_20V		 = 0x00100000;	// Voltage: bipolar +/- 20 V
		public const uint AD_50V		 = 0x00200000;	// Voltage: bipolar +/- 50 V
		public const uint AD_0P125V		 = 0x00400000;	// Voltage: bipolar +/- 0.125 V
		public const uint AD_1P25V		 = 0x00800000;	// Voltage: bipolar +/- 1.25 V
		public const uint AD_0P625V		 = 0x01000000;	// Voltage: bipolar +/- 0.625 V
		public const uint AD_0P156V		 = 0x02000000;	// Voltage: bipolar +/- 0.156 V
		public const uint AD_1P25V_AC	 = 0x04000000;	// Voltage: bipolar +/- 1.25 V(AC Coupling)
		public const uint AD_0P625V_AC	 = 0x08000000;	// Voltage: bipolar +/- 0.625 V(AC Coupling)
		public const uint AD_0P156V_AC	 = 0x10000000;	// Voltage: bipolar +/- 0.156 V(AC Coupling)
		public const uint AD_AC_COUPLING = 0x40000000;	// AC Coupling
		public const uint AD_GND		 = 0x80000000;	// Voltage: GND

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Isolation Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_ISOLATION		 = 1;	// Isolated
		public const uint AD_NOT_ISOLATION	 = 2;	// Not isolated

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Synchronous Mode Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_MASTER_MODE	 = 1;	// Master mode
		public const uint AD_SLAVE_MODE		 = 2;	// Slave mode

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Synchronous Number Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_SYNC_NUM_1		 = 0x0100;
		public const uint AD_SYNC_NUM_2		 = 0x0200;
		public const uint AD_SYNC_NUM_3		 = 0x0400;
		public const uint AD_SYNC_NUM_4		 = 0x0800;
		public const uint AD_SYNC_NUM_5		 = 0x1000;
		public const uint AD_SYNC_NUM_6		 = 0x2000;
		public const uint AD_SYNC_NUM_7		 = 0x4000;

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Calibration Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_SELF_CALIBRATION		 = 1;	// Self calibration of the AD converter
		public const uint AD_ZEROSCALE_CALIBRATION	 = 2;	// Zero voltage calibration (system calibration)
		public const uint AD_FULLSCALE_CALIBRATION	 = 3;	// Full scale voltage calibration (system calibration)

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Full-scale Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_STATUS_NEGATIVE_FULL_SCALE	 = 1;	// Negative full-scale
		public const uint AD_STATUS_POSITIVE_FULL_SCALE	 = 2;	// Positive full-scale
		public const uint AD_STATUS_UNDER_RANGE			 = 1;	// Negative full-scale
		public const uint AD_STATUS_OVER_RANGE			 = 2;	// Positive full-scale

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 OVPM Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_STATUS_OVPM_NORMAL	   	     = 0;	// NormalRange
		public const uint AD_STATUS_OVPM_HIGH_RANGE	     = 1;	// Voltage: bipolar +/- 50 V Range
		public const uint AD_STATUS_OVPM_GND_RANGE	     = 2;	// GND

		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 PCI-3525 CN3,4 Function Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_CN_FREE		 = 	0;	// Not used
		public const uint AD_CN_EXTRG_IN	 = 	1;	// External trigger input
		public const uint AD_CN_EXTRG_OUT	 = 	2;	// External trigger output
		public const uint AD_CN_EXCLK_IN	 = 	3;	// External clock input
		public const uint AD_CN_EXCLK_OUT	 = 	4;	// External clock output
		public const uint AD_CN_EXINT_IN	 = 	5;	// External interrupt input
		public const uint AD_CN_ATRG_OUT	 = 	6;	// Analog trigger out
		public const uint AD_CN_DI			 = 	7;	// Digital input
		public const uint AD_CN_DO			 = 	8;	// Digital output
		public const uint AD_CN_DAOUT		 = 	9;	// Analog output
		public const uint AD_CN_OPEN		 = 	10;	// Open
		public const uint AD_CN_EXSMP1_OUT	 =  12;	// Sampling Status1
		public const uint AD_CN_EXSMP2_OUT	 =  13;	// Sampling Status2
		
		//-----------------------------------------------------------------------------------------------
		//
		//		GPC/GPF-3100 CPZ-360810 DIN/DOUT Function Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_EX_DIO1 = 	1;	// DIN/DOUT1
		public const uint AD_EX_DIO2 = 	2;	// DIN/DOUT2
		public const uint AD_EX_DIO3 = 	3;	// DIN/DOUT3
		public const uint AD_EX_DIO4 = 	4;	// DIN/DOUT4
		public const uint AD_EX_DIO5 = 	5;	// DIN/DOUT5
		
		//-----------------------------------------------------------------------------------------------
		//
		//     GPC/GPF-3100 Error Identifier
		//
		//-----------------------------------------------------------------------------------------------
		public const uint AD_ERROR_SUCCESS				 = 	0x00000000;
		public const uint AD_ERROR_NOT_DEVICE			 = 	0xC0000001;
		public const uint AD_ERROR_NOT_OPEN				 = 	0xC0000002;
		public const uint AD_ERROR_INVALID_HANDLE		 = 	0xC0000003;
		public const uint AD_ERROR_ALREADY_OPEN			 = 	0xC0000004;
		public const uint AD_ERROR_NOT_SUPPORTED		 = 	0xC0000009;
		public const uint AD_ERROR_NOW_SAMPLING			 = 	0xC0001001;
		public const uint AD_ERROR_STOP_SAMPLING		 = 	0xC0001002;
		public const uint AD_ERROR_START_SAMPLING		 = 	0xC0001003;
		public const uint AD_ERROR_SAMPLING_TIMEOUT		 = 	0xC0001004;
		public const uint AD_ERROR_SAMPLING_FREQ		 = 	0xC0001005;
		public const uint AD_ERROR_INVALID_PARAMETER	 = 	0xC0001021;
		public const uint AD_ERROR_ILLEGAL_PARAMETER	 = 	0xC0001022;
		public const uint AD_ERROR_NULL_POINTER			 = 	0xC0001023;
		public const uint AD_ERROR_GET_DATA				 = 	0xC0001024;
		public const uint AD_ERROR_USED_DA				 = 	0xC0001025;
		public const uint AD_ERROR_FILE_OPEN			 = 	0xC0001041;
		public const uint AD_ERROR_FILE_CLOSE			 = 	0xC0001042;
		public const uint AD_ERROR_FILE_READ			 = 	0xC0001043;
		public const uint AD_ERROR_FILE_WRITE			 = 	0xC0001044;
		public const uint AD_ERROR_INVALID_DATA_FORMAT	 = 	0xC0001061;
		public const uint AD_ERROR_INVALID_AVERAGE_OR_SMOOTHING	 = 0xC0001062;
		public const uint AD_ERROR_INVALID_SOURCE_DATA	 = 	0xC0001063;
		public const uint AD_ERROR_NOT_ALLOCATE_MEMORY	 = 	0xC0001081;
		public const uint AD_ERROR_NOT_LOAD_DLL			 = 	0xC0001082;
		public const uint AD_ERROR_CALL_DLL				 = 	0xC0001083;

		// -----------------------------------------------------------------------
		//
		//     User-supplied Function
		//
		// -----------------------------------------------------------------------
		public delegate void LPCONVPROC(
		short wCh,
			uint dwCount,
			IntPtr lpData
			);

		public delegate void LPADCALLBACK(uint dwUser);

		// -----------------------------------------------------------------------
		//     Sampling Condition Structure for Each Channel
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADSMPLCHREQ
		{
			public uint	ulChNo;
			public uint	ulRange;
		}

		// -----------------------------------------------------------------------
		//     Analog Trigger Condition Structure for Each Channel
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADTRIGCHREQ
		{
			public uint		ulChNo;
			public float	fTrigLevel;
			public float	fHysteresis;
		}

		// -----------------------------------------------------------------------
		//     Sampling Condition Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADSMPLREQ
		{
			public uint			ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public uint			ulSamplingMode;
			public uint			ulSingleDiff;
			public uint			ulSmplNum;
			public uint			ulSmplEventNum;
			public float		fSmplFreq;
			public uint			ulTrigPoint;
			public uint			ulTrigMode;
			public int			lTrigDelay;
			public uint			ulTrigCh;
			public float		fTrigLevel1;
			public float		fTrigLevel2;
			public uint			ulEClkEdge;
			public uint			ulATrgPulse;
			public uint			ulTrigEdge;
			public uint			ulTrigDI;
			public uint			ulFastMode;

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
		//     Sampling Condition Structure for Bus Master
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADBMSMPLREQ
		{
			public uint			ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public uint			ulSingleDiff;
			public uint			ulSmplNum;
			public uint			ulSmplEventNum;
			public uint			ulSmplRepeat;
			public uint			ulBufferMode;
			public float		fSmplFreq;
			public float		fScanFreq;
			public uint			ulStartMode;
			public uint			ulStopMode;
			public uint			ulPreTrigDelay;
			public uint			ulPostTrigDelay;
			public uint			ulChNo1;           // Flatizing TrigChReq[0].ulChNo
			public float		fTrigLevel1;       // Flatizing TrigChReq[0].fTrigLevel
			public float		fHysteresis1;      // Flatizing TrigChReq[0].fHysteresis
			public uint			ulChNo2;           // Flatizing TrigChReq[1].ulChNo
			public float		fTrigLevel2;       // Flatizing TrigChReq[1].fTrigLevel
			public float		fHysteresis2;      // Flatizing TrigChReq[1].fHysteresis
			public uint			ulATrgMode;
			public uint			ulATrgPulse;
			public uint			ulStartTrigEdge;
			public uint			ulStopTrigEdge;
			public uint			ulTrigDI;
			public uint			ulEClkEdge;
			public uint			ulFastMode;
			public uint			ulStatusMode;
			public uint			ulErrCtrl;

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
		//     Sampling Condition Structure for Memory
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
			public struct ADMEMSMPLREQ
		{
			public uint		ulChCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
			public uint[]		ulChNoRange;
			public uint		ulSingleDiff;
			public float	fSmplFreq;
			public uint		ulStopMode;
			public uint		ulPreTrigDelay;
			public uint		ulPostTrigDelay;
			public uint		ulChNo1;           // Flatizing TrigChReq[0].ulChNo
			public float	fTrigLevel1;       // Flatizing TrigChReq[0].fTrigLevel
			public float	fHysteresis1;      // Flatizing TrigChReq[0].fHysteresis
			public uint		ulChNo2;           // Flatizing TrigChReq[1].ulChNo
			public float	fTrigLevel2;       // Flatizing TrigChReq[1].fTrigLevel
			public float	fHysteresis2;      // Flatizing TrigChReq[1].fHysteresis
			public uint		ulATrgMode;
			public uint		ulATrgPulse;
			public uint		ulStopTrigEdge;
			public uint		ulEClkEdge;
			public uint		ulFastMode;
			public uint		ulStatusMode;
			public uint		ulErrCtrl;

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
		//     Board Specification Structure
		// -----------------------------------------------------------------------
		[StructLayout(LayoutKind.Sequential)]
		public struct ADBOARDSPEC
		{
			public uint ulBoardType;
			public uint	ulBoardID;
			public uint	dwSamplingMode;
			public uint	ulChCountS;
			public uint	ulChCountD;
			public uint	ulResolution;
			public uint	dwRange;
			public uint	ulIsolation;
			public uint	ulDi;
			public uint	ulDo;
		}


		//-----------------------------------------------------------------------------------------------
		//
		//   GPC/GPF-3100 Function Declaration
		//
		//-----------------------------------------------------------------------------------------------
		[DllImport("fbiad.dll")]
		public static extern uint AdOpen(string szName);
		[DllImport("fbiad.dll")]
		public static extern int AdClose(uint hDevice);
		
		[DllImport("fbiad.dll")]
		public static extern int AdGetDeviceInfo(uint hDevice, out ADBOARDSPEC BoardSpec);
		
		[DllImport("fbiad.dll")]
		public static extern int AdSetBoardConfig(uint hDevice, uint hEvent, LPADCALLBACK lpCallBackProc, uint dwUser);
		[DllImport("fbiad.dll")]
		public static extern int AdSetBoardConfig(uint hDevice, uint hEvent, uint lpCallBackProc, uint dwUser);
		
		[DllImport("fbiad.dll")]
		public static extern int AdGetBoardConfig(uint hDevice, out uint ulAdSmplEventFactor);
		
		[DllImport("fbiad.dll")]
		public static extern int AdSetSamplingConfig(uint hDevice, ref ADSMPLREQ AdSmplConfig);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingConfig(uint hDevice, out ADSMPLREQ AdSmplConfig);
		
		[DllImport("fbiad.dll")]
		public static extern int AdBmSetSamplingConfig(uint hDevice, ref ADBMSMPLREQ AdBmSmplConfig);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingConfig(uint hDevice, out ADBMSMPLREQ AdBmSmplConfig);
		
		[DllImport("fbiad.dll")]
		public static extern int AdMemSetSamplingConfig(uint hDevice, ref ADMEMSMPLREQ AdMemSmplConfig);
		[DllImport("fbiad.dll")]
		public static extern int AdMemGetSamplingConfig(uint hDevice, out ADMEMSMPLREQ AdMemSmplConfig);
		
		[DllImport("fbiad.dll")]
		public static extern int AdLvSetSamplingConfig(uint hDevice, uint ulChNo, uint ulSmplNum, float fSmplFreq, uint ulRange, uint hEvent, LPADCALLBACK lpCallBackProc, uint dwUser);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingConfig(uint hDevice, uint ulChNo, out uint ulSmplNum, out float fSmplFreq, out uint ulRange);
		
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(uint hDevice, out byte pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(uint hDevice, out ushort pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(uint hDevice, out uint pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(uint hDevice, byte[] pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(uint hDevice, ushort[] pSmplData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdGetSamplingData(uint hDevice, uint[] pSmplData, ref uint ulSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(uint hDevice, out byte pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(uint hDevice, out ushort pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(uint hDevice, out uint pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(uint hDevice, byte[] pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(uint hDevice, ushort[] pBmSmplData, ref uint ulBmSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdBmGetSamplingData(uint hDevice, uint[] pBmSmplData, ref uint ulBmSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(uint hDevice, uint ulChNo, ref byte pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(uint hDevice, uint ulChNo, ref ushort pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(uint hDevice, uint ulChNo, ref uint pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(uint hDevice, uint ulChNo, byte[] pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(uint hDevice, uint ulChNo, ushort[] pSmplData, ref uint pulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetSamplingData(uint hDevice, uint ulChNo, uint[] pSmplData, ref uint pulSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(uint hDevice, out byte pSmplData, ref byte pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(uint hDevice, out ushort pSmplData, ref byte pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(uint hDevice, out uint pSmplData, ref byte pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(uint hDevice, byte[] pSmplData, byte[] pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(uint hDevice, ushort[] pSmplData, byte[] pDiData, ref uint ulSmplNum);
		[DllImport("fbiad.dll")]
		public static extern int  AdFifoGetSamplingData(uint hDevice, uint[] pSmplData, byte[] pDiData, ref uint ulSmplNum);

		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(uint hDevice, uint lOffset, ref uint ulSmplNum, out byte pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(uint hDevice, uint lOffset, ref uint ulSmplNum, out ushort pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(uint hDevice, uint lOffset, ref uint ulSmplNum, out uint pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(uint hDevice, uint lOffset, ref uint ulSmplNum, byte[] pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(uint hDevice, uint lOffset, ref uint ulSmplNum, ushort[] pSmplData);
		[DllImport("fbiad.dll")]
		public static extern int  AdReadSamplingBuffer(uint hDevice, uint lOffset, ref uint ulSmplNum, uint[] pSmplData);

		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(uint hDevice, out byte pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(uint hDevice, out ushort pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(uint hDevice, out uint pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(uint hDevice, byte[] pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(uint hDevice, ushort[] pSmplData, uint ulSize);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartSampling(uint hDevice, uint[] pSmplData, uint ulSize);

		[DllImport("fbiad.dll")]
		public static extern int AdClearSamplingData(uint hDevice);
		[DllImport("fbiad.dll")]
		public static extern int AdStartSampling(uint hDevice, uint ulSyncFlag);
		[DllImport("fbiad.dll")]
		public static extern int AdStartFileSampling(uint hDevice, string pszPathName, uint ulFileFlag);
		[DllImport("fbiad.dll")]
		public static extern int AdBmStartFileSampling(uint hDevice, string szPathName, uint ulFileFlag);
		[DllImport("fbiad.dll")]
		public static extern int AdSyncSampling(uint hDevice, uint ulMode);
		[DllImport("fbiad.dll")]
		public static extern int AdLvStartSampling(uint hDevice, uint ulChNo);
		[DllImport("fbiad.dll")]
		public static extern int AdStopSampling(uint hDevice);
		[DllImport("fbiad.dll")]
		public static extern int AdLvStopSampling(uint hDevice, uint ulChNo);
		[DllImport("fbiad.dll")]
		public static extern int AdTriggerSampling(uint hDevice, uint ulChNo, uint ulRange, uint ulSingleDiff, uint ulTriggerMode, uint ulTrigEdge, uint ulSmplNum);
		
		[DllImport("fbiad.dll")]
		public static extern int AdMemTriggerSampling(uint hDevice, uint ulChCount, ref ADSMPLCHREQ lpSmplChReq, uint ulSmplNum, uint ulRepeatCount, uint ulTrigEdge, float fSmplFreq, uint ulEClkEdge, uint ulFastMode);
		[DllImport("fbiad.dll")]
		public static extern int AdMemTriggerSampling(uint hDevice, uint ulChCount, ADSMPLCHREQ[] lpSmplChReq, uint ulSmplNum, uint ulRepeatCount, uint ulTrigEdge, float fSmplFreq, uint ulEClkEdge, uint ulFastMode);
		
		[DllImport("fbiad.dll")]
		public static extern int AdGetStatus(uint hDevice, out uint ulAdSmplStatus, out uint ulAdSmplCount, out uint ulAdAvailCount);
		[DllImport("fbiad.dll")]
		public static extern int AdLvGetStatus(uint hDevice, uint ulChNo, out uint ulAdSmplStatus, out uint ulAdSmplCount, out uint ulAdAvailCount);
		
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(uint hDevice, uint ulCh, uint ulSingleDiff, ref ADSMPLCHREQ AdSmplChReq, out byte lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(uint hDevice, uint ulCh, uint ulSingleDiff, ref ADSMPLCHREQ AdSmplChReq, out ushort lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(uint hDevice, uint ulCh, uint ulSingleDiff, ref ADSMPLCHREQ AdSmplChReq, out uint lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(uint hDevice, uint ulCh, uint ulSingleDiff, ADSMPLCHREQ[] AdSmplChReq, byte[] lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(uint hDevice, uint ulCh, uint ulSingleDiff, ADSMPLCHREQ[] AdSmplChReq, ushort[] lpData);
		[DllImport("fbiad.dll")]
		public static extern int AdInputAD(uint hDevice, uint ulCh, uint ulSingleDiff, ADSMPLCHREQ[] AdSmplChReq, uint[] lpData);

		[DllImport("fbiad.dll")]
		public static extern int  AdSetRangeEvent(uint hDevice, uint dwEventMask, uint dwStopMode);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetRangeEventStatus(uint hDevice, uint[] ulEventChNo, uint[] ulEventStatus);
		[DllImport("fbiad.dll")]
		public static extern int  AdResetRangeEvent(uint hDevice);

		[DllImport("fbiad.dll")]
		public static extern int  AdGetOverRangeChStatus(uint hDevice, out uint ulEventStatus);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetOverRangeChStatus(uint hDevice, uint[] ulEventStatus);
		[DllImport("fbiad.dll")]
		public static extern int  AdResetOverRangeCh(uint hDevice);

		[DllImport("fbiad.dll")]
		public static extern int AdLvCalibration(uint hDevice, uint ulChNo, uint ulCalibration);

		[DllImport("fbiad.dll")]
		public static extern int AdMeasureTemperature(uint hDevice, out float fTemperature);

		[DllImport("fbiad.dll")]
		public static extern int  AdSetInterval(uint hDevice, uint ulInterval);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetInterval(uint hDevice, out uint ulInterval);
		[DllImport("fbiad.dll")]
		public static extern int  AdSetFunction(uint hDevice, uint ulChNo, uint ulFunction);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetFunction(uint hDevice, uint ulChNo, out uint ulFunction);

		[DllImport("fbiad.dll")]
		public static extern int  AdSetFilter(uint hDevice, uint ulFilter);
		[DllImport("fbiad.dll")]
		public static extern int  AdGetFilter(uint hDevice, out uint ulFilter);

		[DllImport("fbiad.dll")]
		public static extern int AdSetOutMode(uint hDevice, uint ulExTrgMode, uint ulExClkMode);
		[DllImport("fbiad.dll")]
		public static extern int AdGetOutMode(uint hDevice, out uint ulExTrgMode , out uint ulExClkMode);
		[DllImport("fbiad.dll")]
		public static extern int AdMemSetDiPattern(uint hDevice, uint ulCh, out uint ulPatternTrig);
		[DllImport("fbiad.dll")]
		public static extern int AdMemGetDiPattern(uint hDevice, out uint ulPatternTrig);
		[DllImport("fbiad.dll")]
		public static extern int AdMemGetDiPattern(uint hDevice, uint[] ulPatternTrig);

		[DllImport("fbiad.dll")]
		public static extern int AdInputDI(uint hDevice, out uint dwData);
		[DllImport("fbiad.dll")]
		public static extern int AdOutputDO(uint hDevice, uint dwData);
		[DllImport("fbiad.dll")]
		public static extern int AdAdjustVR(uint hDevice, uint ulAdjustCh, uint ulSingleDiff, uint ulSelVolume, uint ulControl, uint ulTap);

		[DllImport("fbiad.dll")]
		public static extern int  AdAllocateSamplingBuffer(uint hDevice);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out byte pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out ushort pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out uint pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, out float pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, byte[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, ushort[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, uint[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref byte pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref ushort pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref uint pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ref float pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, byte[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, ushort[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, uint[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdDataConv(uint uSrcFormCode, float[] pSrcData, uint uSrcSmplDataNum, ref ADSMPLREQ SrcSmplReq, uint uDestFormCode, float[] pDestData, out uint uDestSmplDataNum, ref ADSMPLREQ DestSmplReq, uint uEffect, uint uCount, LPCONVPROC lpfnConv);

		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, out byte pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, out ushort pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, out uint pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, out float pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, byte[] pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, ushort[] pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, uint[] pSmplData, uint uFormCode);
		[DllImport("fbiaddc.dll")]
		public static extern int  AdReadFile(string szPathName, float[] pSmplData, uint uFormCode);

		[DllImport("fbiad.dll")]
		public static extern int  AdCommonGetPciDeviceInfo(uint hDevice, out uint dwDeviceID, out uint dwVendorID, out uint dwClassCode, out uint dwRevisionID, out uint dwBaseAddress0, out uint dwBaseAddress1, out uint dwBaseAddress2, out uint dwBaseAddress3, out uint dwBaseAddress4, out uint dwBaseAddress5, out uint dwSubsystemID, out uint dwSubsystemVendorID, out uint dwInterruptLine, out uint dwBoardID);

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
