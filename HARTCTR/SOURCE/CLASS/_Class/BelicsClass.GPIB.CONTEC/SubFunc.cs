using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using CgpibCs;

namespace CSubFuncCs
{
    /// <summary>
    /// SubFunc �̊T�v�̐����ł��B
    /// </summary>
    public class CSubFunc
    {
        /// <summary></summary>
        public bool isBoardMounted = true;

        /// <summary></summary>
        public CSubFunc()
        {
            // 
            // TODO: �R���X�g���N�^ ���W�b�N�������ɒǉ����Ă��������B
            //
        }

        Cgpib gpib = new Cgpib();

        // ************************************************** [�`�F�b�N(���f)�֐�] ***
        /// <summary></summary>
        public int CheckRet(string Func, uint Ret, out string csBuf)
        {
            int RetCode;
            int RetTmp;

            csBuf = new String('0', 1);
            RetCode = 0;										// ���펞
            RetTmp = (int)Ret & 0xff;							// �}�X�N����
            if (RetTmp >= 3)
            {													// Ret��3�ȏ�̏ꍇ�̓G���[
                RetCode = 1;									// �ُ펞
                switch (RetTmp)
                {
                    case 80: csBuf = Func + " : I/O�A�h���X�G���[�ł��B[Config.exe]�Ŋm�F���Ă��������B"; break;	// 0x50
                    case 82: csBuf = Func + " : ���W�X�g���ݒ�G���[�ł��B[Config.exe]�Ŋm�F���Ă��������B"; break;	// 0x52
                    case 128: csBuf = Func + " : �w��f�[�^����M����[SRQ]�͏オ���Ă��܂���"; break;	// 0x80
                    case 140: csBuf = Func + " : �񓯊��֐��̎��s���ł�"; break;	// 0x8C
                    case 141: csBuf = Func + " : �񓯊��֐��������I������܂���"; break;	// 0x8D
                    case 200: csBuf = Func + " : �X���b�h���쐬�ł��܂���B"; break;	// 0xC8
                    case 201: csBuf = Func + " : ���̃C�x���g�����s���ł��"; break;	// 0xC9
                    case 210: csBuf = Func + " : DMA���ݒ�ł��܂���"; break;	// 0xD0
                    case 240: csBuf = Func + " : Esc�L�[��������܂����B"; break;	// 0xF0
                    case 241: csBuf = Func + " : �t�@�C�����o�̓G���[�ł��B"; break;	// 0xF1
                    case 242: csBuf = Func + " : �A�h���X�w�肪�Ԉ���Ă��܂��B"; break;	// 0xF2
                    case 243: csBuf = Func + " : �o�b�t�@�w��G���[�ł��B"; break;	// 0xF3
                    case 244: csBuf = Func + " : �z��T�C�Y�G���[�ł��B"; break;	// 0xF4
                    case 245: csBuf = Func + " : �o�b�t�@�����������܂��B"; break;	// 0xF5
                    case 246: csBuf = Func + " : �s���ȃI�u�W�F�N�g���ł��B"; break;	// 0xF6
                    case 247: csBuf = Func + " : �f�o�C�X���̉��̃`�F�b�N�������ł��B"; break;	// 0xF7
                    case 248: csBuf = Func + " : �s���ȃf�[�^�^�ł��B"; break;	// 0xF8
                    case 249: csBuf = Func + " : ����ȏ�f�o�C�X��ǉ��ł��܂���B"; break;	// 0xF9
                    case 250: csBuf = Func + " : �f�o�C�X����������܂���B"; break;	// 0xFA
                    case 251: csBuf = Func + " : �f���~�^���f�o�C�X�Ԃň���Ă��܂��B"; break;	// 0xFB
                    case 252: csBuf = Func + " : GPIB�G���[�ł��B"; break;	// 0xFC
                    case 253: csBuf = Func + " : �f���~�^�݂̂���M���܂����B"; break;	// 0xFD
                    case 254: csBuf = Func + " : �^�C���A�E�g���܂����B"; break;	// 0xFE
                    case 255: csBuf = Func + " : �p�����[�^�G���[�ł��B"; break;	// 0xFF
                    default: break;
                }
            }
            else
            {
                csBuf = "";
            }

            // -- [Ifc] [Srq] ����M�������̏��� -- //
            RetTmp = (int)Ret & 0xff00;							// �}�X�N����
            switch (RetTmp)
            {
                case 0x100: csBuf = csBuf + " -- [SRQ]����M<STATUS>"; break;		// 256(10)
                case 0x200: csBuf = csBuf + " -- [IFC]����M<STATUS>"; break;		// 512(10)
                case 0x300: csBuf = csBuf + " -- [SRQ]��[IFC]����M<STATUS>"; break;		// 768(10)
                default: break;
            }
            return RetCode;
        }

        //************************************************************ [�ׂ���֐�] ***
        /// <summary></summary>
        public int Pows(int x, int y)
        {
            int tmp = 1;

            while (y-- > 0)										// y �̉񐔕��J��Ԃ��܂��B
                tmp *= x;										// tmp �� x ���|���Ă����܂��B
            return (tmp);
        }

        //****************************************************** [������ -> 16�i��] ***
        /// <summary></summary>
        public uint chr2hex(string ch)
        {
            int length;
            int Count;
            int Ret;
            int RetTmp;

            RetTmp = 0;
            length = ch.Length;								// �������𒲂ׂ܂��B
            for (Count = 0; Count < length; Count++)
            {													// �������������J��Ԃ��܂��B
                if ((ch[Count] >= 0x30) && (ch[Count] <= 0x39))	// ASCII�R�[�h���琔�I�Ȓl���擾
                {
                    Ret = (ch[Count] - 0x30) * Pows(0x10, (length - (Count + 1)));	// 0 - 9
                }
                else if ((ch[Count] >= 0x41) && (ch[Count] <= 0x46))
                {
                    Ret = (ch[Count] - 0x37) * Pows(0x10, (length - (Count + 1)));	// A - F
                }
                else if ((ch[Count] >= 0x61) && (ch[Count] <= 0x66))
                {
                    Ret = (ch[Count] - 0x57) * Pows(0x10, (length - (Count + 1)));	// a - f
                }
                else
                {
                    Ret = 0xff;									// �s���ȏꍇFF(255)��Ԃ��܂��B
                }

                RetTmp = RetTmp + Ret;
            }
            return (uint)RetTmp;
        }

        //************************************************************ [�������֐�] ***
        /// <summary></summary>
        public uint GpibInit(out string TextRet)
        {
            //			uint	Delim,Eoi;
            uint Ret;
            uint Timeout, Ifctime;
            string csBuf;
            uint Master;

            Ret = 0;
            if (isBoardMounted) Ret = gpib.Exit();									// 2�d��������h���܂��B
            if (isBoardMounted) Ret = gpib.Ini();									// GPIB�����������܂��B
            csBuf = "GpIni";

            if ((Ret & 0xFF) != 0)
            {													// GpIni������ɍs�������`�F�b�N�B
                CheckRet("GpIni", Ret, out csBuf);
                TextRet = csBuf.ToString();
                return Ret;
            }

            Master = 0;
            if (isBoardMounted) gpib.Boardsts(0x0a, out Master);					// �}�X�^�̃A�h���X���擾���܂��B
            // �}�X�^�A�X���[�u�̔���
            if (Master == 0)
            {
                Ifctime = 1;									// �����ł�100��sec�ɂ��Ă��܂��B
                if (isBoardMounted) Ret = gpib.Ifc(Ifctime);
                csBuf = "GpIfc";
                if ((Ret & 0xFF) != 0)
                {												// GpIfc������ɍs�������`�F�b�N�B
                    CheckRet("GpIfc", Ret, out csBuf);
                    TextRet = csBuf.ToString();
                    return 1;
                }

                if (isBoardMounted) Ret = gpib.Ren();
                csBuf = "GpRen";
                if ((Ret & 0xFF) != 0)
                {												// GpRen������ɍs�������`�F�b�N�B
                    CheckRet("GpRen", Ret, out csBuf);
                    TextRet = csBuf.ToString();
                    return Ret;
                }
            }

            //			Delim = 1;										// �f���~�^�FCR+LF
            //			Eoi = 1;											// EOI	�F�g�p����
            //			Ret = gpib.Delim(Delim, Eoi);
            //			csBuf = "GpDelim";
            //			if ((Ret & 0xFF) != 0)
            //			{													// GpDelim������ɍs�������`�F�b�N�B
            //				CheckRet("GpDelim", Ret, out csBuf);
            //				TextRet = csBuf;
            //				return	1;
            //			}
            Timeout = 3000;									// 3�b
            if (isBoardMounted) Ret = gpib.Timeout(Timeout);
            csBuf = "GpTimeout";
            if ((Ret & 0xFF) != 0)
            {													// GpTimeout������ɍs�������`�F�b�N�B
                TextRet = csBuf.ToString();
                CheckRet("GpTimeout", Ret, out csBuf);
                return Ret;
            }

            TextRet = "";					// ����I��
            return Ret;
        }

        //******************************************************** [GpTalk()�̉��p] ***
        /// <summary></summary>
        public int GpibPrint(uint DevAddr, string Str)
        {
            string srbuf = new string(' ', 10000);				// ���M������
            uint MyAddr;										// �}�C�A�h���X�p
            uint[] Cmd = new uint[16];							// �R�}���h�p
            string ErrText;									// �G���[������
            uint Ret;										// �߂�l
            int RetTmp;										// �\���߂�l
            uint srlen;										// ������̒���

            Ret = gpib.Boardsts(0x08, out MyAddr);				// �}�C�A�h���X�擾
            Cmd[0] = 2;											// �R�}���h�̐�
            Cmd[1] = MyAddr;									// �}�C�A�h���X(PC)
            Cmd[2] = DevAddr;									// �X���[�u�@��

            srlen = (uint)Str.Length;							// �����𑪒�
            srbuf = Str;										// CString -> char
            if (isBoardMounted) Ret = gpib.Talk(Cmd, srlen, srbuf);					// ���ۂ̑��M

            if (Ret >= 3)
            {													// �G���[�`�F�b�N
                RetTmp = CheckRet("GpTalk", Ret, out ErrText);
                //ErrText += " �p�����܂����H";
                //DialogResult Resul = MessageBox.Show(ErrText, null, MessageBoxButtons.YesNo);
                //if (Resul == DialogResult.No)
                //{
                return 1;								// �ُ�
                //}
            }
            return 0;											// ����
        }

        //****************************************************** [GpListen()�̉��p] ***
        /// <summary></summary>
        public int GpibInput(uint DevAddr, StringBuilder Str)
        {
            uint MyAddr = 0;
            uint srlen;								            // �}�C�A�h���X�A������̒����p
            uint[] Cmd = new uint[16];							// �R�}���h�p
            string	/*TmpStr, */ErrText;						// �\��������A�G���[������
            uint Ret = 0;										// �߂�l
            int RetTmp;										    // �\���߂�l

            if (isBoardMounted) Ret = gpib.Boardsts(0x08, out MyAddr);				// �}�C�A�h���X�擾
            Cmd[0] = 2;											// �R�}���h�̐�
            Cmd[1] = DevAddr;									// �X���[�u�@��
            Cmd[2] = MyAddr;									// �}�C�A�h���X(PC)
            srlen = 10000;										// ��M�����f�[�^�̒����𑪂��Ă��܂��B
            if (isBoardMounted) Ret = gpib.Listen(Cmd, ref srlen, Str);				// ���ۂ̑��M
            if (Ret >= 3)
            {													// �G���[�`�F�b�N
                RetTmp = CheckRet("GpListen", Ret, out ErrText);
                //ErrText += " �p�����܂����H";
                //DialogResult Resul = MessageBox.Show(ErrText, null, MessageBoxButtons.YesNo);
                //if (Resul == DialogResult.No) return 1;			// �ُ� = 1��Ԃ�
                return 1;
            }
            return 0;											// ���� = 0��Ԃ�
        }

        // ************************************************* [ �o�C�i����M�p�֐� ] ***
        /// <summary></summary>
        public int GpibInputB(uint DevAddr, StringBuilder IntData)
        {
            StringBuilder szData = new StringBuilder(10000);
            uint Ret = 0, MyAddr = 0, srlen;
            int RetTmp;
            uint[] Cmd = new uint[8];
            string ErrText = "";
            string szDataVal;
            int i;

            if (isBoardMounted) Ret = gpib.Delim(0, 1);								// �f���~�^�𑊎�@��ƍ��킹�܂��B
            if (isBoardMounted) Ret = gpib.Boardsts(0x08, out MyAddr);				// �}�C�A�h���X�擾
            Cmd[0] = 2;											// �R�}���h�̐�
            Cmd[1] = DevAddr;									// �X���[�u�@��
            Cmd[2] = MyAddr;									// �}�C�A�h���X(PC)
            srlen = 2;											// ��M�����f�[�^�̒����𑪂��Ă��܂��B
            if (isBoardMounted) Ret = gpib.Listen(Cmd, ref srlen, szData);
            if (Ret != 128)
            {													// �r���Ńf�[�^��؂��Ă��邽��Ret=128�ɂȂ�܂��B
                if (Ret >= 3)
                {												// �G���[�`�F�b�N
                    RetTmp = CheckRet("GpListen", Ret, out ErrText);
                    //ErrText += " �p�����܂����H";
                    //DialogResult Resul = MessageBox.Show(ErrText, null, MessageBoxButtons.YesNo);
                    //if (Resul == DialogResult.No)
                    {
                        return 1;							// �ُ� = 1��Ԃ�
                    }
                }
            }
            Cmd[0] = 0;
            szDataVal = szData.ToString().Substring(1, 1);
            for (i = 0; i < szDataVal.Length; i++)
            {
                if (Char.IsDigit(szDataVal, i) == false)
                {
                    break;
                }
            }
            szDataVal = szDataVal.Substring(0, i);
            if (i == 0)
            {
                srlen = 0;
            }
            else
            {
                srlen = uint.Parse(szDataVal);
            }

            if (isBoardMounted) Ret = gpib.Listen(Cmd, ref srlen, szData);

            for (i = 0; i < szData.Length; i++)
            {
                if (Char.IsDigit(szData.ToString(), i) == false)
                {
                    break;
                }
            }
            szDataVal = szData.ToString().Substring(0, i);
            if (i == 0)
            {
                srlen = 1;
            }
            else
            {
                srlen = uint.Parse(szDataVal) + 1;
            }

            Ret = gpib.Listen(Cmd, ref srlen, IntData);
            Ret = gpib.Delim(3, 1);								// �f���~�^��߂��܂��B
            return 0;
        }

        //****************************************************** [�R�}���h���M�֐�] ***
        /// <summary></summary>
        public int GpibCommand(uint DevAddr)
        {
            uint[] Cmd = new uint[16];
            string ErrText;
            uint Ret = 0;
            int RetTmp;

            Cmd[0] = 2;
            Cmd[1] = 0x3F;
            Cmd[2] = 0x5F;

            if (isBoardMounted) Ret = gpib.Comand(Cmd);

            if (Ret != 0)
            {
                RetTmp = CheckRet("GpComand", Ret, out ErrText);
                //MessageBox.Show(ErrText, null, MessageBoxButtons.OK);
                return 1;
            }
            return 0;
        }

        //**************************************************************** [�I���֐�] ***
        /// <summary></summary>
        public void GpibExit()
        {
            uint Master;
            uint[] Cmd = new uint[16];
            uint Ret = 0;

            Master = 0;
            if (isBoardMounted) Ret = gpib.Boardsts(0x0a, out Master);				// �}�X�^�̃A�h���X���擾���܂��B
            if (Ret == 80)
            {
                return;											// ����������Ă��Ȃ��ꍇ���������ɖ߂�܂��B
            }

            if (Master == 0)
            {													// �}�X�^�̏ꍇ
                Cmd[0] = 2;										// �R�}���h(���b�Z�[�W)��
                Cmd[1] = 0x3f;									// �A�����X��(���X�i����)
                Cmd[2] = 0x5f;									// �A���g�[�N��(�g�[�J����)

                if (isBoardMounted) Ret = gpib.Comand(Cmd);							// �R�}���h�𑗐M���܂��B
                if (isBoardMounted) Ret = gpib.Resetren();							// ����@��̃����[�g���������܂��B
            }

            if (isBoardMounted) Ret = gpib.Exit();
        }

        // OPC�̃`�F�b�N ////////////////////////////////////////////////////////////////////////////////
        /// <summary></summary>
        public void WaitOPC(uint Dev)
        {
            int Ret;
            StringBuilder RdData = new StringBuilder(10000);

            if (isBoardMounted) Ret = GpibPrint(Dev, "*OPC?");						// �H����Ƃ��������Ă��邩
            if (isBoardMounted) Ret = GpibInput(Dev, RdData);
        }

        //**************************************************** [������𐔎��ɕϊ�] ***
        /// <summary></summary>
        public void Str2Num(string str, uint str_len, ref int[] num, uint num_len)
        {
            int i, j = 0, cnt;
            string start;

            start = str;
            cnt = 0;
            for (i = 0; i < str_len; i++)
            {
                /* string to integer */
                if (str[i] == ',')
                {
                    if (i == 1)
                    {
                        start = str.Substring(j, i - j);
                    }
                    else
                    {
                        start = str.Substring(j, i - j);
                    }
                    try
                    {
                        num[cnt] = Convert.ToInt32(start);
                    }
                    catch (System.FormatException)
                    {
                        num[cnt] = 0;
                    }
                    j = i + 1;
                    cnt++;
                    if (cnt >= num_len)
                    {
                        break;
                    }
                }
            }
            if (cnt >= num_len)
            {
                try
                {
                    num[cnt] = Convert.ToInt32(start);
                }
                catch (System.FormatException)
                {
                    num[cnt] = 0;
                }
            }
        }

        //********************************************************** [�O���t��`��] ***
        // ��Q�����ŃO���t�p�s�N�`���[�_�C�A���O���g�p���Ă��܂��B
        /// <summary></summary>
        public void DrawGraph(Control Picture, int[] num, uint num_len, int min, int max)
        {
            Rectangle Rect = Picture.ClientRectangle;
            Pen hPen_Black, hPen_Red, hPen_White;
            Point[] point = new Point[4];
            int x_max, y_max;
            int x_width, y_width;
            float x_unit, y_unit;
            uint i;

            /* Initialize */
            x_max = Rect.Right;
            y_max = Rect.Bottom;
            Graphics g = Picture.CreateGraphics();

            hPen_Black = new Pen(Color.Black, 1);				/* Black */
            hPen_Red = new Pen(Color.Red, 1);				/* Red   */
            hPen_White = new Pen(Color.White, 1);				/* White */
            /* Draw Structure */
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            g.FillRectangle(whiteBrush, Picture.ClientRectangle);
            for (i = 0; i <= 10; i++)
            {
                g.DrawLine(hPen_Black, (x_max / 10) * i, 0, (x_max / 10) * i, y_max);
            }
            for (i = 0; i <= 10; i++)
            {
                g.DrawLine(hPen_Black, 0, (y_max / 10) * i, x_max, (y_max / 10) * i);
            }
            point[0].X = (x_max / 10) * 5 - 1; point[0].Y = 0;
            point[1].X = (x_max / 10) * 5 + 1; point[1].Y = 0;
            point[2].X = (x_max / 10) * 5 + 1; point[2].Y = y_max;
            point[3].X = (x_max / 10) * 5 - 1; point[3].Y = y_max;
            g.DrawLines(hPen_Red, point);
            point[0].X = 0; point[0].Y = (y_max / 10) * 5 - 1;
            point[1].X = 0; point[1].Y = (y_max / 10) * 5 + 1;
            point[2].X = x_max; point[2].Y = (y_max / 10) * 5 + 1;
            point[3].X = x_max; point[3].Y = (y_max / 10) * 5 - 1;
            g.DrawLines(hPen_Red, point);
            /* Draw Graph */
            x_width = (int)num_len;
            y_width = max - min;
            x_unit = (float)((float)x_max / (float)x_width);
            y_unit = (float)((float)y_max / (float)y_width);
            for (i = 0; i < (num_len - 1); i++)
            {
                g.DrawLine(hPen_Black, x_unit * i, (y_width - (num[i] - min)) * y_unit, x_unit * (i + 1), (y_width - (num[i + 1] - min)) * y_unit);
            }
        }
    }
}
