using System;
using System.Text;
using System.Collections.Generic;

namespace BelicsClass.Common
{
    /// <summary>
    /// カンマ区切り文字列を操作します。<br/>
    /// 引用符のエスケープ方法はExcelのCSV形式に準拠して、
    /// 引用符(ダブルクォーテーション)を2つ連続して並べます。
    /// </summary>
    public class BL_CommaText : List<String>
    {
        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        public BL_CommaText()
            : base()
        {
        }

        /// <summary>
        /// デリミタと引用符を指定できるコンストラクタです。
        /// </summary>
        /// <param name="delimiter">デリミタを指定します。</param>
        /// <param name="quoted">引用符を指定します。</param>
        public BL_CommaText(char delimiter, char quoted)
            : base()
        {
            m_Delimiter = delimiter;
            m_Quoted = quoted;
        }

        /// <summary>引用符です。</summary>
        protected char m_Quoted = '"';

        /// <summary>デリミタです。</summary>
        protected char m_Delimiter = ',';

        /// <summary>
        /// カンマ区切り文字列の取得/設定を行います。<br/>
        /// 取得した文字列は引用符でくくられた形式となります。<br/>
        /// 設定する場合は、ExcelのCSV形式で保存した時の行と同等の
        /// フォーマットを設定してください。Excel互換で無い文字列を
        /// 代入すると例外を投げます。
        /// </summary>
        public string Text
        {
            get { return getCommaStr(); }
            set { setCommaStr(value); }
        }

        /// <summary>
        /// 引用符の取得します。<br/>
        /// デフォルトはダブルクォーテーション(半角)です。
        /// </summary>
        public char Quoted
        {
            get
            {
                return m_Quoted;
            }
        }

        /// <summary>
        /// デリミタ(カンマ)の取得します。<br/>
        /// デフォルトはカンマ(半角)です。
        /// </summary>
        public char Delimiter
        {
            get
            {
                return m_Delimiter;
            }
        }

        /// <summary>
        /// カンマで区切られた文字列を取得/設定します。<br/>
        /// indexは0から指定します。
        /// </summary>
        new public string this[int index]
        {
            get { return base[index]; }
            set { base[index] = value; }
        }

        /// <summary>
        /// 区切り文字列の最後尾が文字列無しの場合、削除します。
        /// </summary>
        public void Trim()
        {
            //最後尾から文字列の入っていない区切りを検索する
            for (int cnt = this.Count - 1; cnt >= 0; --cnt)
            {
                //最後尾に文字列の入っていない区切りが存在するので削除する
                if (this[cnt] == "")
                {
                    this.RemoveAt(cnt);
                }
                else
                {
                    //最後尾の区切りに文字列があるのでトリム終了
                    break;
                }
            }
        }

        /// <summary>保持している文字列をカンマ区切り文字列にして取得します。</summary>
        /// <returns>カンマ区切り文字列を返します。</returns>
        private string getCommaStr()
        {
            //文字列が無い
            if (this.Count <= 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (string stringWork in this)
            {
                //開始の引用符を付加
                sb.Append(m_Quoted);

                //文字列を一字ずつ代入する
                //文字列中に引用符が含まれる場合はエスケープコードを挿入する
                foreach (char charWork in stringWork)
                {
                    //引用符であれば、エスケープコード(引用符)を挿入
                    if (charWork == m_Quoted)
                    {
                        sb.Append(m_Quoted);
                    }

                    //文字を代入
                    sb.Append(charWork);
                }

                //終了の引用符を付加
                sb.Append(m_Quoted);

                //デリミタを付加
                sb.Append(m_Delimiter);
            }

            //最後のカンマを削除してから返す
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        /// <summary>カンマ区切り文字列を設定します。</summary>
        /// <param name="commaString">設定するカンマ区切り文字列を指定します。</param>
        private void setCommaStr(string commaString)
        {
            //インスタンス初期化
            this.Clear();
            if (commaString.Length == 0) return;

            //文字列を先頭から抜き出して記憶する
            string extractString;
            while (commaString.Length > 0)
            {
                //先頭の文字列を抜き出して記憶
                DivideTopText(ref commaString, out extractString, m_Quoted, m_Delimiter);
                base.Add(extractString);
            }
        }

        /// <summary>
        /// 指定されたカンマ区切り文字列の先頭文字列を取得できます。
        /// </summary>
        /// <param name="baseString"></param>
        /// <param name="topString"></param>
        /// <param name="quoted"></param>
        /// <param name="delimiter"></param>
        public static void DivideTopText(ref string baseString, out string topString, char quoted, char delimiter)
        {
            //カンマ区切り文字列無し
            if (baseString.Length <= 0)
            {
                topString = "";
                return;
            }

            baseString = baseString.Trim();

            //先頭は引用符である
            if (baseString[0] == quoted)
            {
                //文字列の区切りを見つける
                topString = "";
                int checkIndex = 1;
                bool quating = true;

                for (; ; ++checkIndex)
                {
                    //引用符でくくられていない
                    if (checkIndex >= baseString.Length)
                    {
                        throw new Exception("引用符で開始された文字列：引用符で終了していない");
                    }

                    //引用符以外なら、抜き出し文字列として記憶
                    if (baseString[checkIndex] != quoted && quating)
                    {
                        topString += baseString[checkIndex];
                        continue;
                    }
                    if (baseString[checkIndex] == quoted && quating)
                    {
                        quating = false;
                    }

                    //引用符の次は、引用符かデリミタまたは最終文字列が来るはず

                    //引用符の次に文字列無し
                    if ((checkIndex + 1) >= baseString.Length)
                    {
                        ++checkIndex;
                        break;
                    }

                    //引用符の次は引用符
                    if (baseString[checkIndex + 1] == quoted)
                    {
                        topString += quoted;
                        ++checkIndex;
                        continue;
                    }

                    //引用符の次はデリミタ
                    if (baseString[checkIndex + 1] == delimiter)
                    {
                        checkIndex += 2;
                        break;
                    }

                    ////引用符の後に規定外の文字列
                    //throw new Exception("引用符でくくられた文字列・引用符の後に、規定外の文字が存在する");
                }

                //規定文字列から今回抜き出した文字列を削除
                baseString = baseString.Remove(0, checkIndex);

                //返却
                return;
            }

            //先頭はデリミタ
            if (baseString[0] == delimiter)
            {
                baseString = baseString.Remove(0, 1);
                topString = "";
                return;
            }

            //先頭は引用符でもデリミタでも無い文字である

            //カンマを発見するまで
            int commaIndex = 0;
            topString = "";
            for (; ; ++commaIndex)
            {
                //文字列おしまい
                if (commaIndex >= baseString.Length)
                {
                    break;
                }

                //デリミタ発見
                if (baseString[commaIndex] == delimiter)
                {
                    ++commaIndex;
                    break;
                }

                //引用符発見(エラー)
                if (baseString[commaIndex] == quoted)
                {
                    throw new Exception("引用符で開始しない文字列中に引用符が含まれている");
                }

                //文字列を記憶
                topString += baseString[commaIndex];
            }

            //規定文字列から今回抜き出した文字列を削除
            baseString = baseString.Remove(0, commaIndex);
        }
    }
}
