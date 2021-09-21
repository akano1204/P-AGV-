using System;
using System.IO;
using System.Xml.Serialization;

namespace BelicsClass.ObjectSync
{
    /// <summary>
    /// XMLシリアライズ可能オブジェクトの基本クラス
    /// </summary>
    [Serializable]
    public class BL_XmlSync : BL_ObjectSync
    {
        /// <summary>
        /// XMLシリアライズ
        /// </summary>
        /// <returns></returns>
        public byte[] XmlSerialize()
        {
            MemoryStream ms = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(this.GetType());
            xs.Serialize(ms, this, new XmlSerializerNamespaces());

            byte[] buff = ms.GetBuffer();

            ms.Close();
            ms.Dispose();
            ms = null;
            xs = null;

            return buff;
        }

        /// <summary>
        /// XMLシリアライズ
        /// </summary>
        /// <returns></returns>
        public byte[] XmlSerialize<T>()
        {
            MemoryStream ms = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(T).GetType());
            xs.Serialize(ms, this, new XmlSerializerNamespaces());

            byte[] buff = ms.GetBuffer();

            ms.Close();
            ms.Dispose();
            ms = null;
            xs = null;

            return buff;
        }


        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="buffer"></param>
        public BL_XmlSync XmlDeserialize(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            XmlSerializer xs = new XmlSerializer(this.GetType());
            BL_XmlSync obj = (BL_XmlSync)xs.Deserialize(ms);
            obj.ObjectAdjustment();
            byte[] a = obj.GetBytes();

            ms.Close();
            ms.Dispose();
            ms = null;
            xs = null;

            return obj;
        }

        /// <summary>
        /// デシリアライズ
        /// </summary>
        /// <param name="buffer"></param>
        public object XmlDeserialize<T>(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            XmlSerializer xs = new XmlSerializer(typeof(T).GetType());
            BL_XmlSync obj = (BL_XmlSync)xs.Deserialize(ms);
            obj.ObjectAdjustment();
            byte[] a = obj.GetBytes();

            ms.Close();
            ms.Dispose();
            ms = null;
            xs = null;

            return obj;
        }
    }
}
