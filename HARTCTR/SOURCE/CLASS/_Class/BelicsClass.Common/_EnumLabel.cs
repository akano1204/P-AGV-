using System;
using System.Collections.Generic;

namespace BelicsClass.Common
{
    /// <summary>
    /// 列挙型のフィールドにラベル文字列を付加するカスタム属性です。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class BL_EnumLabel
        : Attribute
    {
        private string label;

        /// <summary>
        /// BL_EnumLabel クラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="label">ラベル文字列</param>
        public BL_EnumLabel(string label)
        {
            this.label = label;
        }

        /// <summary>
        /// 属性で指定されたラベル文字列を取得する。
        /// </summary>
        /// <param name="value">ラベル付きフィールド</param>
        /// <returns>ラベル文字列</returns>
        public static string GetLabel(Enum value)
        {
            Type enumType = value.GetType();
            string name = Enum.GetName(enumType, value);

            try
            {
                BL_EnumLabel[] attrs =
                    (BL_EnumLabel[])enumType.GetField(name)
                    .GetCustomAttributes(typeof(BL_EnumLabel), false);
                return attrs[0].label;
            }
            catch { }

            return "(" + name + ")";
        }

        /// <summary>
        /// ラベル文字列のインデックスを取得する。
        /// </summary>
        /// <param name="label"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static object GetValue(string label, Type enumType)
        {
            foreach (object value in Enum.GetValues(enumType))
            {
                if (BL_EnumLabel.GetLabel((Enum)Enum.Parse(enumType, value.ToString())) == label)
                {
                    return value;
                }
            }

            throw new Exception("Enum内に存在しません");
        }
    }
}
