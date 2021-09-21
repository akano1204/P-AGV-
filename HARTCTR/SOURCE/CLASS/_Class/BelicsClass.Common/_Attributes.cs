using System;
using System.Collections.Generic;
using System.Text;

namespace BelicsClass.Common
{
    /// <summary>
    /// BL_ObjectSyncクラス機能のためのメモリ管理属性を表します。
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class BL_ObjectSyncAttribute : System.Attribute
    {
        /// <summary></summary>
        public int Order { get; set; } = 99999;
    }


}
