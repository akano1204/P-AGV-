using System;

namespace BelicsClass.Common
{
    /// <summary>
    /// ビット操作クラス
    /// </summary>
    [Serializable]
    public class BL_BitOperator
    {
        [BL_ObjectSyncAttribute]
        private ushort bit_value = 0;

        /// <summary>
        /// 現在値を取得または設定します
        /// </summary>
        public ushort Value
        {
            get { return bit_value; }
            set { bit_value = value; }
        }

        /// <summary>
        /// インデクサ
        /// 指定されたビット位置のON/OFF状態をboolで取得します
        /// 指定できるビット位置は0～15までの16ビットです
        /// 不正なビット位置が指定されると例外が発生します
        /// </summary>
        /// <param name="index">ビット位置</param>
        /// <returns>ON/OFF=true/falseを返します</returns>
        public bool this[int index]
        {
            get
            {
                if (index > 15) throw new Exception("16ビット以上を扱うことが出来ません");

                ushort i = (ushort)Math.Pow(2, index);
                if ((bit_value & i) > 0) return true;
                else return false;
            }

            set
            {
                if (index > 15) throw new Exception("16ビット以上を扱うことが出来ません");

                ushort i = (ushort)Math.Pow(2, index);
                if (value == true) bit_value |= i;
                else bit_value &= (ushort)(~i);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// 現在値は０で初期化されます
        /// </summary>
        public BL_BitOperator() { }

        /// <summary>
        /// コンストラクタ
        /// 指定した現在値で初期化されます
        /// </summary>
        /// <param name="value">現在値</param>
        public BL_BitOperator(ushort value) { bit_value = value; }

        /// <summary>
        /// 上位→下位でビット状態を表す文字列を生成します
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string ret = "";
            for (int i = 0; i < 16; i++)
            {
                if (this[i]) ret = "1" + ret;
                else ret = "0" + ret;
            }
            return ret;
        }
    }
}
