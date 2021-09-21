using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using BelicsClass.Common;

namespace BelicsClass.File
{
    /// <summary>
    /// CSVファイルを扱うクラス
    /// </summary>
    public class BL_CsvFile
    {
        /// <summary>ファイルの保存先を保持します。</summary>
        protected string filepath = "";
        /// <summary>ファイルの内容を保持します。</summary>
        protected List<BL_CommaText> list = new List<BL_CommaText>();

        /// <summary>引用符です。</summary>
        protected char m_Quoted = '"';

        /// <summary>デリミタです。</summary>
        protected char m_Delimiter = ',';

        /// <summary>ファイルパスを取得します</summary>
        public string FilePath { get { return filepath; } }

        /// <summary>
        /// 
        /// </summary>
        public BL_CsvFile() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delimiter"></param>
        /// <param name="quoted"></param>
        public BL_CsvFile(char delimiter, char quoted)
        {
            m_Delimiter = delimiter;
            m_Quoted = quoted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="delimiter"></param>
        /// <param name="quoted"></param>
        public BL_CsvFile(string filepath, char delimiter, char quoted)
            : this(delimiter, quoted)
        {
            Open(filepath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        virtual public bool Open(string filepath)
        {
            return Load(filepath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        virtual public bool Load(string filepath)
        {
            try
            {
                StreamReader sr = new StreamReader(filepath, Encoding.Default);

                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null) break;

                    BL_CommaText csv = new BL_CommaText(m_Delimiter, m_Quoted);
                    csv.Text = line;
                    list.Add(csv);
                }

                sr.Close();

                this.filepath = filepath;

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        virtual public bool Save(string filepath)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filepath, false, Encoding.Default);
                foreach (var v in list)
                {
                    sw.Write(v.Text);
                    sw.WriteLine();
                }
                sw.Close();

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        virtual public bool Save_Append(string filepath)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filepath, true, Encoding.Default);
                foreach (var v in list)
                {
                    sw.Write(v.Text);
                    sw.WriteLine();
                }
                sw.Close();

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 読み込んだCSVファイルのデータを取得します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        virtual public BL_CommaText this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        /// <summary>
        /// 読み込んだCSVファイルのレコード数を取得します
        /// </summary>
        virtual public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commatext"></param>
        virtual public void Add(BL_CommaText commatext)
        {
            list.Add(commatext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        virtual public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        virtual public void Clear()
        {
            list.Clear();
        }
    }
}
