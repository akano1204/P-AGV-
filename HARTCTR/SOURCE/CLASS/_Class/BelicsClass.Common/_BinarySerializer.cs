using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BelicsClass.Common
{
    /// <summary>
    /// シリアライズクラス
    /// シリアライズ対象のインスタンスは[Serializable]属性である必要があります
    /// バイナリーシリアライズを行います
    /// </summary>
    public class BL_BinarySerializer
    {
        /// <summary>
        /// オブジェクトをメモリーストリームへシリアライズします
        /// </summary>
        /// <param name="objectType">シリアライズ対象</param>
        /// <returns>メモリーストリームを返します</returns>
        public static MemoryStream SerializeToStream(object objectType)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objectType);
            return stream;
        }

        /// <summary> 
        /// メモリーストリームからオブジェクトをデシリアライズします
        /// </summary> 
        /// <param name="stream"></param> 
        /// <returns></returns> 
        public static object DeserializeFromStream(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object objectType = formatter.Deserialize(stream);
            return objectType;
        }

        /// <summary>
        /// 型指定でメモリーストリームからオブジェクトをデシリアライズします
        /// デシリアライズされたオブジェクトが指定した型にキャストできない場合、例外が発生します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns>指定した型のオブジェクトを返します</returns>
        public static T DeserializeFromStream<T>(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            T objectType = (T)formatter.Deserialize(stream);
            return objectType;
        }

        /// <summary>
        /// ストリームから読み込んだデータでバイト配列を生成します
        /// </summary>
        /// <param name="stream">読み込み対象のストリーム</param>
        /// <returns>生成されたバイト配列</returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] buff = new byte[stream.Length];
            stream.Read(buff, 0, (int)stream.Length);
            return buff;
        }

        /// <summary>
        /// バイト配列をメモリーストリームへ書き込みます
        /// </summary>
        /// <param name="data">書き込むデータ</param>
        /// <returns>メモリーストリームを返します</returns>
        public static MemoryStream StreamFromBytes(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            return ms;
        }

        /// <summary>
        /// オブジェクトをバイナリーシリアライズして、バイト配列を取得します
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static byte[] SerializeToBytes(object objectType)
        {
            return BL_BinarySerializer.StreamToBytes(BL_BinarySerializer.SerializeToStream(objectType));
        }

        /// <summary>
        /// バイト配列をデシリアライズして、オブジェクトを取得します
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DeserializeFromBytes(byte[] data)
        {
            return BL_BinarySerializer.DeserializeFromStream(BL_BinarySerializer.StreamFromBytes(data));
        }

        /// <summary>
        /// バイト配列をデシリアライズして、指定した型のオブジェクトを取得します
        /// 指定した型へキャストできない場合、例外が発生します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeserializeFromBytes<T>(byte[] data)
        {
            return (T)BL_BinarySerializer.DeserializeFromStream(BL_BinarySerializer.StreamFromBytes(data));
        }

        /// <summary>
        /// 指定した型オブジェクトのクローンを生成します。
        /// </summary>
        /// <typeparam name="T">クラスの型</typeparam>
        /// <param name="obj">クローン元</param>
        /// <returns>生成されたクローンのインスタンス</returns>
        public static T Clone<T>(T obj)
        {
            return (T)BL_BinarySerializer.DeserializeFromStream<T>(BL_BinarySerializer.SerializeToStream(obj));
        }

        /// <summary>
        /// 指定した型オブジェクトをシリアライズしてファイルへ保存します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Save<T>(string path, T obj)
        {
            try
            {
                MemoryStream ms = BL_BinarySerializer.SerializeToStream(obj);

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    catch { }
                }

                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                ms.WriteTo(fs);
                fs.Close();
                ms.Close();

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// ファイルからデシリアライズして指定した型オブジェクトを取得します
        /// 指定した型にキャストできない場合、例外が発生します
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                T newinstance = BL_BinarySerializer.DeserializeFromStream<T>(fs);
                fs.Close();
                return newinstance;
            }
            catch { }

            return default(T);
        }
    }
}
