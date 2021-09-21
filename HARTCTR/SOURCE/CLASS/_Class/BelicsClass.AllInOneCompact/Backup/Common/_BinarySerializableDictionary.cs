using System;
using System.Collections.Generic;

using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BelicsClass.Common
{
    /// <summary>
    /// Dictionaryをシリアライズ可能にするためのラッパークラス
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    [XmlRoot("dictionary")]
    public class BL_BinarySerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members

        /// <summary>
        /// スキーマ取得
        /// </summary>
        /// <returns></returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// XMLを読み込む
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// XMLで書き込みます
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion

        /// <summary>
        /// ファイルへ書き出します
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Write(string path)
        {
            try
            {
                var xmlSerializer1 = new XmlSerializer(typeof(BL_BinarySerializableDictionary<TKey, TValue>));
                using (var streamWriter = new System.IO.StreamWriter(path, false, Encoding.UTF8))
                {
                    xmlSerializer1.Serialize(streamWriter, this);
                    streamWriter.Flush();
                }

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// ファイルから読み込みます
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Read(string path)
        {
            try
            {

                BL_BinarySerializableDictionary<TKey, TValue> newDict = new BL_BinarySerializableDictionary<TKey, TValue>();

                var xmlSerializer2 = new XmlSerializer(typeof(BL_BinarySerializableDictionary<TKey, TValue>));
                var xmlSettings = new System.Xml.XmlReaderSettings()
                {
                    CheckCharacters = false,
                };
                using (var streamReader = new System.IO.StreamReader(path, Encoding.UTF8))
                using (var xmlReader = System.Xml.XmlReader.Create(streamReader, xmlSettings))
                {
                    newDict = (BL_BinarySerializableDictionary<TKey, TValue>)xmlSerializer2.Deserialize(xmlReader);

                    this.Clear();
                    foreach (var kv in newDict)
                    {
                        this.Add(kv.Key, kv.Value);
                    }
                }

                return true;
            }
            catch { }

            return false;
        }
    }
}
