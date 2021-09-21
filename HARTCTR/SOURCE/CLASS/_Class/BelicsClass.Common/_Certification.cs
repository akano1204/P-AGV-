using System;
using System.Text;
using System.Security.Cryptography;

namespace BelicsClass.Common
{
    /// <summary>
    /// 暗号化処理クラス
    /// </summary>
    public class BL_Certification
    {
        private string m_publickey;
        private string m_privatekey;

        /// <summary>
        /// コンストラクタ
        /// 暗号鍵を設定します
        /// </summary>
        public BL_Certification()
        {
            m_publickey = "<RSAKeyValue><Modulus>s/7s7cepfgfdiQ2+YSvKx5q6UD6dg0jLjKTRWtnGTM4X30446ytRWAMtWHvBK6l" +
             "DiwcNBf2ozvucwFTgq9xLXws6lqF+wJJFzWiDUH5JM5mnFfLGqYIz8P/OAg7H01/bRt2TVnK2Nq4kSSoBqhHSZOngtYhzoF7xJ" +
             "6F2q7RVHls=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            m_privatekey = "<RSAKeyValue><Modulus>s/7s7cepfgfdiQ2+YSvKx5q6UD6dg0jLjKTRWtnGTM4X30446ytRWAMtWHvBK6lD" +
              "iwcNBf2ozvucwFTgq9xLXws6lqF+wJJFzWiDUH5JM5mnFfLGqYIz8P/OAg7H01/bRt2TVnK2Nq4kSSoBqhHSZOngtYhzoF7xJ6F" +
              "2q7RVHls=</Modulus><Exponent>AQAB</Exponent><P>9Ix/iS6x78vZRWnD3sDo1TpRfTovoEgJOZ2h4AQTt3HqmSj88itR" +
              "YsiVLC7gM4LwLJ/AQOTks0rXvzDb02FCqQ==</P><Q>vGya7VR7+Il7jJthK1U7QIYWfHjcbHMmriyC+bR4UH4NG32gve2V8xWW" +
              "3wjEJeSqG1X5cDpSWQ/X6If8dwL/Yw==</Q><DP>Ch+a0/n74bpllysGsbz4poMQhoeXGyKAR0NQRS7GBi0QEiERP5EtgFJfYoH" +
              "aRCeie6ZtVgJjuUxa3A5Qu1JmaQ==</DP><DQ>G1ePK71EuA4LNZ4efZFCpdxPSwSmx7318PRYlS+Q/e0srb6PIsBlL/8EA51cC" +
              "TujS5AwQA7WgEICTKXfBs4SjQ==</DQ><InverseQ>ubzbLJkkWAUX/JVQGHrtaQw7Wx7LU+EJYyvyHFHzcB9UIXkaQX/0dHeyM" +
              "WdCwDdaeInwh5KARGft4MNIThEVVA==</InverseQ><D>OYh+ivKLBrHU6f3uoHiy9GJMs2GQT2pxRZ9ZG8UIUXx+vO3v25uZAJ" +
              "zjXakQ8bfE0yeJziRqVN6wF/oJZmWf4VL8I4hJqXOQXbsc0R7vQ0JR9cHrtyTZWD8o3Qgcvh0oJcFbCSOLzNTWdHBm+rUbzXE7/" +
              "wuw+4ledzLori7qwkE=</D></RSAKeyValue>";
        }

        /// <summary>
        /// 公開鍵と秘密鍵を作成して返す
        /// </summary>
        /// <param name="publicKey">作成された公開鍵(XML形式)</param>
        /// <param name="privateKey">作成された秘密鍵(XML形式)</param>
        public void CreateKeys(out string publicKey, out string privateKey)
        {
            //RSACryptoServiceProviderオブジェクトの作成
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //公開鍵をXML形式で取得
            publicKey = rsa.ToXmlString(false);
            //秘密鍵をXML形式で取得
            privateKey = rsa.ToXmlString(true);
        }

        /// <summary>
        /// 秘密鍵を使って文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化を行う文字列</param>
        /// <returns>暗号化された文字列</returns>
        public string Encrypt(string str)
        {
            //RSACryptoServiceProviderオブジェクトの作成
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //公開鍵を指定
            rsa.FromXmlString(m_publickey);

            //暗号化する文字列をバイト配列に
            byte[] data = Encoding.UTF8.GetBytes(str);
            //暗号化する
            //（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
            byte[] encryptedData = rsa.Encrypt(data, false);

            //Base64で結果を文字列に変換
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 秘密鍵を使って文字列を復号化する
        /// </summary>
        /// <param name="str">Encryptメソッドにより暗号化された文字列</param>
        /// <returns>復号化された文字列</returns>
        public string Decrypt(string str)
        {
            string retstr = "";
            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                //秘密鍵を指定
                rsa.FromXmlString(m_privatekey);

                //復号化する文字列をバイト配列に
                byte[] data = Convert.FromBase64String(str);
                //復号化する
                byte[] decryptedData = rsa.Decrypt(data, false);

                //結果を文字列に変換
                retstr = Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                string exstr = ex.ToString();
            }
            return retstr;
        }

        /// <summary>
        /// 文字列を暗号化する
        /// </summary>
        public string Encrypt64(string str)
        {
            //暗号化する文字列をバイト配列に
            byte[] data = Encoding.UTF8.GetBytes(str);

            //Base64で結果を文字列に変換
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 文字列を復号化する
        /// </summary>
        public string Decrypt64(string str)
        {
            //復号化する文字列をバイト配列に
            byte[] data = Convert.FromBase64String(str);

            //結果を文字列に変換
            return Encoding.UTF8.GetString(data);
        }
    }
}
