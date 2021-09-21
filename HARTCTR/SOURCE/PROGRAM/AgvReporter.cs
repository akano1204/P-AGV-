using System;
using System.IO;
using System.Text;
//using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
//using System.Linq;
//using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.File;
using BelicsClass.ProcessManage;
using BelicsClass.Database;


namespace PROGRAM
{
    public class AgvReporter : BL_ThreadController_Base
    {
        private ConcurrentQueue<ReportInfo> AgvQue = new ConcurrentQueue<ReportInfo>();
        public static BelicsClass.File.BL_IniFile ini_hokusho = new BelicsClass.File.BL_IniFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\DATA\HOKUSHO.INI"));
        public BL_Database Db = new BL_OleDb();
        public bool Dbcon = false;
        public string FilePath1 = "";
        public string FilePath2 = "";
        BL_IniFile t_Integrated = null;
        BL_IniFile t_Ability = null;

        public enum enMark
        {
            [BL_EnumLabel("")]
            UNKNOWN,
            [BL_EnumLabel("S")]
            START_RUN,
            [BL_EnumLabel("s")]
            STOP_RUN,
            [BL_EnumLabel("U")]
            RACK_UP,
            [BL_EnumLabel("u")]
            RACK_DOWN,
            [BL_EnumLabel("A")]
            START_STATION,
            [BL_EnumLabel("a")]
            FINISH_STATION,
            [BL_EnumLabel("C")]
            START_CHARGE,
            [BL_EnumLabel("c")]
            STOP_CHARGE,
            [BL_EnumLabel("X")]
            CRASH_STOP,
        }

        public class ReportInfo
        {
            /// <summary>マーク</summary>
            public enMark mark = enMark.UNKNOWN;
            /// <summary>AGV ID</summary>
            public string agvid = "";
            /// <summary>発生日時</summary>
            public DateTime timestamp = DateTime.Now;
            /// <summary>座標</summary>
            public Point point = new Point();
            /// <summary>棚ID</summary>
            public string rackid = "";
            /// <summary>面ID</summary>
            public string faceid = "";
            /// <summary>ステーションID</summary>
            public string stationid = "";
            /// <summary>走行距離</summary>
            public float distance = 0f;
            /// <summary>通過QR個数</summary>
            public int qrcount = 0;
            /// <summary>経過時間(秒)</summary>
            public int elapsed_seconds = 0;
            /// <summary>バッテリー残量</summary>
            public int battery = 0;
            /// <summary>衝突検知AGV ID</summary>
            public string crash_agvid = "";
        }
        public class ReportResult
        {
            /// <summary>登録日</summary>
            public DateTime regdate = DateTime.MinValue;
            /// <summary>AGV ID</summary>
            public string agvid = "";
            /// <summary>走行距離</summary>
            public float distance = 0f;
            /// <summary>経過時間(秒)</summary>
            public int elapsed_seconds = 0;
            /// <summary>旋回角度</summary>
            public int SwingAangle = 0;
            /// <summary>充電回数</summary>
            public int NumberCharges = 0;
            /// <summary>充電時間</summary>
            public int TimeCharges = 0;
            /// <summary>棚上昇回数</summary>
            public int ShelfUp = 0;
            /// <summary>棚下降回数</summary>
            public int ShelfDown = 0;
            /// <summary>棚旋回角度</summary>
            public float ShelAngle = 0f;
            /// <summary>ステーション作業回数</summary>
            public int NumberWork = 0;
            /// <summary>ステーション作業時間</summary>
            public int WorkHours = 0;
            /// <summary>搬送回数</summary>
            public int NumberHansou = 0;
            /// <summary>搬送作業時間</summary>
            public int HansouHours = 0;

        }
        public class ReportAbility
        {
            /// <summary>AGV ID</summary>
            public string agvid = "";
            /// <summary>走行時間(秒)</summary>
            public int elapsed_seconds = 0;
            /// <summary>充電時間</summary>
            public int TimeCharges = 0;
            /// <summary>充電回数</summary>
            public int NumberCharges = 0;
            /// <summary>搬送作業時間</summary>
            public int HansouHours = 0;
            /// <summary>搬送回数</summary>
            public int NumberHansou = 0;
            /// <summary>搬送能力</summary>
            public int NouryokuHansou = 0;
            /// <summary>待機時間</summary>
            public int TimeTaiki = 0;

        }
        public class AgvMonitor
        {
            /// <summary>AGV 名</summary>
            public string name = "";
            /// <summary>ipアドレス</summary>
            public float Ip_Adr = 0f;
            /// <summary>通信状態</summary>
            public int ComStat = 0;
            /// <summary>運行状態</summary>
            public int OpeStat = 0;
            /// <summary>充電残量</summary>
            public int ChargeRemain = 0;
            /// <summary>現在座標</summary>
            public Point Point;
        }
        public AgvReporter()
        {
            if (Program.DborFile == 1)
            {
                string initial_catalog = Program.ini_hokusho.Read("REPORTER", "INITIAL_CATALOG", "");
                string data_source = Program.ini_hokusho.Read("REPORTER", "DATA_SOURCE", "");
                string userid = Program.ini_hokusho.Read("REPORTER", "USERID", "");
                string password = Program.ini_hokusho.Read("REPORTER", "PASSWORD", "");
                if (!Db.Connect(initial_catalog, data_source, userid, password))
                {
                    Dbcon = false;
                    //yone  BL_MessageBox.Show(Db.ErrorMessage);
                    //return;
                }
                else
                {
                    Dbcon = true;
                    Create_Table();     //テーブルが存在しなければ作成する
                }
            }
            else
            {
                FilePath1 = Program.ini_hokusho.Read("REPORTER", "FILE1PATH", "");
                FilePath2 = Program.ini_hokusho.Read("REPORTER", "FILE2PATH", "");
            }
        }
        public void Create_Table()
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath) + @"\CREATE_elapsed.sql";
            bool check = CheckTable("table_elapsed");
            if (check == false)
            {
                string script = File.ReadAllText(path);
                if (!Db.Execute(script))
                {
                    string err = "DB table_elapsed CREATE[" + Db.ErrorMessage + "]";
                    Log(script);
                    Log(err);
                    return;
                }
            }
            path = Path.GetDirectoryName(Application.ExecutablePath) + @"\CREATE_Integrated.sql";
            check = CheckTable("table_Integrated");
            if (check == false)
            {
                string script = File.ReadAllText(path);
                if (!Db.Execute(script))
                {
                    string err = "DB table_Integrated CREATE[" + Db.ErrorMessage + "]";
                    Log(script);
                    Log(err);
                    return;
                }
            }
            path = Path.GetDirectoryName(Application.ExecutablePath) + @"\CREATE_Ability.sql";
            check = CheckTable("table_Ability");
            if (check == false)
            {
                string script = File.ReadAllText(path);
                if (!Db.Execute(script))
                {
                    string err = "DB table_Ability CREATE[" + Db.ErrorMessage + "]";
                    Log(script);
                    Log(err);
                    return;
                }
            }

        }
        public ReportInfo StartRun(DateTime OccurTime, string agvid, Point point, string rackid, int battery)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.START_RUN;
            ri.timestamp = OccurTime;
            ri.agvid = agvid;
            ri.point = point;
            ri.rackid = rackid;
            ri.battery = battery;
            RegistQue(ri);
            return ri;
        }
        public ReportInfo StopRun(DateTime OccurTime, string agvid, Point point, string rackid, int battery, float distance, int qrcount, int elapsed_seconds)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.STOP_RUN;
            ri.agvid = agvid;
            ri.point = point;
            ri.rackid = rackid;
            ri.battery = battery;
            ri.distance = distance;
            ri.qrcount = qrcount;
            ri.elapsed_seconds = elapsed_seconds;
            RegistQue(ri);
            return ri;
        }

        public ReportInfo RackUp(string agvid, Point point, string rackid, int battery)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.RACK_UP;
            ri.agvid = agvid;
            ri.point = point;
            ri.rackid = rackid;
            ri.battery = battery;
            RegistQue(ri);
            return ri;
        }
        public ReportInfo RackDown(string agvid, Point point, string rackid, int battery)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.RACK_DOWN;
            ri.agvid = agvid;
            ri.point = point;
            ri.rackid = rackid;
            ri.battery = battery;
            RegistQue(ri);
            return ri;
        }
        public ReportInfo StartStation(string agvid, Point point, string rackid, string faceid, string stationid)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.START_STATION;
            ri.agvid = agvid;
            ri.point = point;
            ri.rackid = rackid;
            ri.faceid = faceid;
            ri.stationid = stationid;
            RegistQue(ri);
            return ri;
        }
        public ReportInfo FinishStation(string agvid, Point point, string rackid, string faceid, string stationid, int elapsed_seconds)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.FINISH_STATION;
            ri.agvid = agvid;
            ri.point = point;
            ri.rackid = rackid;
            ri.faceid = faceid;
            ri.stationid = stationid;
            ri.elapsed_seconds = elapsed_seconds;
            RegistQue(ri);
            return ri;
        }
        public ReportInfo StartCharge(string agvid, Point point, int battery)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.START_CHARGE;
            ri.agvid = agvid;
            ri.point = point;
            ri.battery = battery;
            RegistQue(ri);
            return ri;
        }
        public ReportInfo StopCharge(string agvid, Point point, int battery, int elapsed_seconds)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.STOP_CHARGE;
            ri.agvid = agvid;
            ri.point = point;
            ri.battery = battery;
            ri.elapsed_seconds = elapsed_seconds;
            RegistQue(ri);
            return ri;
        }
        public ReportInfo CrashStop(string agvid, Point point, string crash_agvid)
        {
            ReportInfo ri = new ReportInfo();
            ri.mark = enMark.CRASH_STOP;
            ri.agvid = agvid;
            ri.point = point;
            ri.crash_agvid = crash_agvid;
            RegistQue(ri);
            return ri;
        }

        public void RegistQue(ReportInfo info)
        {
            AgvQue.Enqueue(info);
        }
        protected override bool DoControl(object message)
        {
            //string sql = "";
            //string sqlv = "";
            string bmark = "";
            string err = "";

            ReportInfo info;

            while (AgvQue.Count > 0)
            {
                if (AgvQue.TryDequeue(out info) )
                {
                    err=Insert_elapsed(info);

                    if (err == "")
                    {
                        switch (info.mark)
                        {
                            case enMark.STOP_RUN:
                                bmark = BL_EnumLabel.GetLabel(enMark.START_RUN);
                                break;
                            case enMark.RACK_UP:
                                bmark = BL_EnumLabel.GetLabel(enMark.RACK_DOWN);
                                break;
                            case enMark.RACK_DOWN:
                                bmark = BL_EnumLabel.GetLabel(enMark.RACK_UP);
                                break;
                            case enMark.FINISH_STATION:
                                bmark = BL_EnumLabel.GetLabel(enMark.START_STATION);
                                break;
                            case enMark.STOP_CHARGE:
                                bmark = BL_EnumLabel.GetLabel(enMark.START_CHARGE);
                                break;
                            default:
                                bmark = BL_EnumLabel.GetLabel(enMark.UNKNOWN);
                                break;
                        }
                        if (bmark != BL_EnumLabel.GetLabel(enMark.UNKNOWN))
                        {
                            int flag = 0;
                            ReportResult rptr = new ReportResult();
                            ReportAbility abli = new ReportAbility();

                            string sRtn = Get_Integrated(ref flag, info.timestamp, info.agvid, ref rptr);
                            rptr.agvid = info.agvid;
                            abli.agvid = info.agvid;
                            switch (info.mark)
                            {
                                case enMark.STOP_RUN:
                                    rptr.elapsed_seconds += info.elapsed_seconds;                               //経過時間
                                    rptr.distance += info.distance;                                             //走行距離
                                    abli.elapsed_seconds = info.elapsed_seconds;
                                    if (info.rackid != "")  //棚IDがあれば搬送回数、搬送時間を加算
                                    {
                                        abli.NumberHansou = 1;
                                        abli.HansouHours = info.elapsed_seconds;
                                    }
                                    break;
                                case enMark.RACK_UP:
                                    rptr.ShelfUp += 1;                                                          //棚下降回数
                                    break;
                                case enMark.RACK_DOWN:
                                    rptr.ShelfDown += 1;                                                        //棚下降回数
                                    break;
                                case enMark.FINISH_STATION:
                                    rptr.NumberWork += 1;                                                        //ステーション作業回数
                                    rptr.WorkHours += info.elapsed_seconds;                                      //ステーション作業時間
                                    break;
                                case enMark.STOP_CHARGE:
                                    rptr.NumberCharges += 1;                                                     //充電回数
                                    rptr.TimeCharges += info.elapsed_seconds;                                    //充電時間
                                    abli.NumberCharges = 1;
                                    abli.TimeCharges = info.elapsed_seconds;

                                    break;
                            }
                            err = Set_Integrated(info.mark,flag, info.timestamp, rptr);
                            err = Set_Ability(abli);
                        }
                    }
                }
                Thread.Sleep(1);
            }
            return base.DoControl(message);
        }
        int ConvTsToSec(TimeSpan ts)
        {
            int sec = 0;
            sec = ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
            return sec;
        }

        string Get_Integrated(ref int flag, DateTime day, string agvid, ref ReportResult rptr)
        {
            string sRtn = "";
            if (Program.DborFile == 1)
            {
                if (Dbcon == true)
                {
                    string sql = "select * from table_Integrated";
                    sql += " where 登録日 = '@day'";
                    sql += "   and agvid = '@agvid'";
                    sql = sql.Replace("@day", day.ToString("yyyy/MM/dd"));
                    sql = sql.Replace("@agvid", agvid);
                    if (!Db.Execute(sql, "table_Integrated"))
                    {
                        string err = "SQL実行エラー[" + Db.ErrorMessage + "]";
                        Log(sql);
                        Log(err);
                        sRtn = err;
                    }
                    else
                    {
                        if (Db["table_Integrated"].Rows.Count == 0)
                        {
                            flag = 0;
                            rptr.agvid = agvid;
                            DateTime tim = DateTime.Now;
                            rptr.regdate = DateTime.Parse(tim.ToString("yyyy/MM/dd"));
                            rptr.distance = 0f;
                            rptr.elapsed_seconds = 0;
                            rptr.SwingAangle = 0;
                            rptr.NumberCharges = 0;
                            rptr.TimeCharges = 0;
                            rptr.ShelfUp = 0;
                            rptr.ShelfDown = 0;
                            rptr.ShelAngle = 0f;
                            rptr.NumberWork = 0;
                            rptr.WorkHours = 0;
                        }
                        else
                        {
                            flag = 1;
                            DataRow row = Db["table_Integrated"].Rows[0];
                            DateTime tim;
                            DateTime.TryParse(row["登録日"].ToString(), out tim);
                            rptr.regdate = DateTime.Parse(tim.ToString("yyyyMMdd"));
                            rptr.agvid = row["AGVID"].ToString();
                            rptr.distance = 0f; float.TryParse(row["走行距離"].ToString(), out rptr.distance);
                            rptr.elapsed_seconds = 0; int.TryParse(row["走行時間"].ToString(), out rptr.elapsed_seconds);
                            rptr.SwingAangle = 0; int.TryParse(row["旋回角度"].ToString(), out rptr.SwingAangle);
                            rptr.NumberCharges = 0; int.TryParse(row["充電回数"].ToString(), out rptr.NumberCharges);
                            rptr.TimeCharges = 0; int.TryParse(row["充電時間"].ToString(), out rptr.TimeCharges);
                            rptr.ShelfUp = 0; int.TryParse(row["棚上昇回数"].ToString(), out rptr.ShelfUp);
                            rptr.ShelfDown = 0; int.TryParse(row["棚下降回数"].ToString(), out rptr.ShelfDown);
                            rptr.ShelAngle = 0f; float.TryParse(row["棚旋回角度"].ToString(), out rptr.ShelAngle);
                            rptr.NumberWork = 0; int.TryParse(row["ステーション作業回数"].ToString(), out rptr.NumberWork);
                            rptr.WorkHours = 0; int.TryParse(row["ステーション作業時間"].ToString(), out rptr.WorkHours);
                        }
                    }
                }
            }
            else
            {
                t_Integrated = new BL_IniFile(Path.Combine(Application.StartupPath, @"..\DATA\t_Integrated"+ day.ToString("yyyyMMdd")+".ini"));

                string Tim = t_Integrated.Read("REPORTER", "登録日", "");
                DateTime tim;
                DateTime.TryParse(Tim.Trim(), out tim);
                rptr.regdate = DateTime.Parse(tim.ToString("yyyy/MM/dd"));
                rptr.agvid = agvid;
                rptr.distance = 0f;
                bool brtn=float.TryParse(t_Integrated.Read(agvid, "走行距離", "").Trim(), out rptr.distance);
                rptr.elapsed_seconds = 0; int.TryParse(t_Integrated.Read(agvid, "走行時間", "").Trim(), out rptr.elapsed_seconds);
                rptr.SwingAangle = 0; int.TryParse(t_Integrated.Read(agvid, "旋回角度", "").Trim(), out rptr.SwingAangle);
                rptr.NumberCharges = 0; int.TryParse(t_Integrated.Read(agvid, "充電回数", "").Trim(), out rptr.NumberCharges);
                rptr.TimeCharges = 0; int.TryParse(t_Integrated.Read(agvid, "充電時間", "").Trim(), out rptr.TimeCharges);
                rptr.ShelfUp = 0; int.TryParse(t_Integrated.Read(agvid, "棚上昇回数", "").Trim(), out rptr.ShelfUp);
                rptr.ShelfDown = 0; int.TryParse(t_Integrated.Read(agvid, "棚下降回数", "").Trim(), out rptr.ShelfDown);
                rptr.ShelAngle = 0f; float.TryParse(t_Integrated.Read(agvid, "棚旋回角度", "").Trim(), out rptr.ShelAngle);
                rptr.NumberWork = 0; int.TryParse(t_Integrated.Read(agvid, "ステーション作業回数", "").Trim(), out rptr.NumberWork);
                rptr.WorkHours = 0; int.TryParse(t_Integrated.Read(agvid, "ステーション作業時間", "").Trim(), out rptr.WorkHours);
            }
            return sRtn;

        }
        string Set_Integrated(enMark mark, int flag, DateTime day, ReportResult rpt)
        {
            string sql = "";
            string sqlv = "";
            string err = "";
            if (Program.DborFile == 1)
            {
                if (Dbcon == true)
                {

                    if (flag == 0)
                    {
                        sql = "insert into table_Integrated (";
                        sqlv = "values (";
                        sql += " 登録日"; sqlv += "'" + rpt.regdate.ToString("yyyy/MM/dd") + "'";
                        sql += " ,AGVID"; sqlv += ",'" + rpt.agvid + "'";
                        sql += ",走行距離"; sqlv += "," + rpt.distance;
                        sql += ",走行時間"; sqlv += "," + rpt.elapsed_seconds;
                        sql += ",旋回角度"; sqlv += "," + rpt.SwingAangle;
                        sql += ",充電回数"; sqlv += "," + rpt.NumberCharges;
                        sql += ",充電時間"; sqlv += "," + rpt.TimeCharges;
                        sql += ",棚上昇回数"; sqlv += "," + rpt.ShelfUp;
                        sql += ",棚下降回数"; sqlv += "," + rpt.ShelfDown;
                        sql += ",棚旋回角度"; sqlv += "," + rpt.ShelAngle;
                        sql += ",ステーション作業回数"; sqlv += "," + rpt.NumberWork;
                        sql += ",ステーション作業時間"; sqlv += "," + rpt.WorkHours;
                        sql += ")"; sqlv += ")";
                        if (!Db.Execute(sql + sqlv))
                        {
                            err = "DB更新エラー[" + Db.ErrorMessage + "]";
                            Log(sql);
                            Log(err);
                        }
                    }
                    else
                    {
                        sql = "UPDATE table_Integrated  SET ";

                        switch (mark)
                        {
                            case enMark.STOP_RUN:
                                sql += "走行距離 =" + rpt.distance;
                                sql += ",走行時間 =" + rpt.elapsed_seconds;
                            break;

                            case enMark.RACK_UP:
                                sql += "棚上昇回数=" + rpt.ShelfUp;
                                break;
                            case enMark.RACK_DOWN:
                                sql += "棚下降回数=" + rpt.ShelfDown;
                                break;
                            case enMark.FINISH_STATION:
                                sql += "ステーション作業回数= " + rpt.NumberWork;
                                sql += ",ステーション作業時間= " + rpt.WorkHours;
                                break;
                            case enMark.STOP_CHARGE:
                                sql += "充電回数 =" + rpt.NumberCharges;
                                sql += ",充電時間 =" + rpt.TimeCharges;
                                break;
                        }
                        //sql += ",旋回角度 =" + rpt.SwingAangle;
                        //sql += ",棚旋回角度=" + rpt.ShelAngle;
                        sql += " where 登録日 = '@day'";
                        sql += "   and agvid = '@agvid'";
                        sql = sql.Replace("@day", day.ToString("yyyy/MM/dd"));
                        sql = sql.Replace("@agvid", rpt.agvid);

                        if (!Db.Execute(sql))
                        {
                            err = "DB更新エラー[" + Db.ErrorMessage + "]";
                            Log(sql);
                            Log(err);
                        }
                    }
                }
            }
            else
            {
                t_Integrated = new BL_IniFile(Path.Combine(Application.StartupPath, @"..\DATA\t_Integrated" + day.ToString("yyyyMMdd") + ".ini"));
                switch (mark)
                {
                    case enMark.STOP_RUN:
                        t_Integrated.Write(rpt.agvid, "走行距離", rpt.distance);
                        t_Integrated.Write(rpt.agvid, "走行時間", rpt.elapsed_seconds);
                        break;

                    case enMark.RACK_UP:
                        t_Integrated.Write(rpt.agvid, "棚上昇回数", rpt.ShelfUp);
                        break;
                    case enMark.RACK_DOWN:
                        t_Integrated.Write(rpt.agvid, "棚下降回数", rpt.ShelfDown);
                        break;
                    case enMark.FINISH_STATION:
                        t_Integrated.Write(rpt.agvid, "ステーション作業回数", rpt.NumberWork);
                        t_Integrated.Write(rpt.agvid, "ステーション作業時間", rpt.WorkHours);
                        break;
                    case enMark.STOP_CHARGE:
                        t_Integrated.Write(rpt.agvid, "充電回数", rpt.NumberCharges);
                        t_Integrated.Write(rpt.agvid, "充電時間", rpt.TimeCharges);
                        break;
                }

                //t_Integrated.Write(rpt.agvid, "旋回角度", rpt.SwingAangle);
                //t_Integrated.Write(rpt.agvid, "棚旋回角度", rpt.ShelAngle);
            }
            return err;
        }

        public bool CheckTable(string tableName)
        {
            string sql = "select * from " + tableName;
            if (!Db.Execute(sql))
            {
                return false;
            }
            return true;
        }
        string Set_Ability(ReportAbility abli)
        {
            string sqlv = "";
            string err = "";
            if (Program.DborFile == 1)
            {
                if (Dbcon == true)
                {

                    string sql = "select * from table_Ability";
                    sql += " where agvid = '@agvid'";
                    sql = sql.Replace("@agvid", abli.agvid);
                    if (!Db.Execute(sql, "table_Ability"))
                    {
                        err = "SQL実行エラー[" + Db.ErrorMessage + "]";
                        Log(sql);
                        Log(err);
                    }
                    else
                    {
                        if (Db["table_Ability"].Rows.Count == 0)
                        {
                            sql = "insert into table_Ability (";
                            sqlv = "values (";
                            sql += " AGVID"; sqlv += "'" + abli.agvid + "'";
                            sql += ",走行時間"; sqlv += "," + abli.elapsed_seconds;
                            sql += ",充電時間"; sqlv += "," + abli.TimeCharges;
                            sql += ",搬送時間"; sqlv += "," + abli.HansouHours;
                            sql += ",搬送回数"; sqlv += "," + abli.NumberHansou;
                            //sql += ",搬送能力"; sqlv += "," + rpt.HansouHours;
                            //sql += ",待機時間"; sqlv += "," + rpt.ShelAngle;
                            sql += ")"; sqlv += ")";
                            if (!Db.Execute(sql + sqlv))
                            {
                                err = "DB更新エラー[" + Db.ErrorMessage + "]";
                                Log(sql);
                                Log(err);
                                return err;
                            }
                        }
                        else
                        {
                            DataRow row = Db["table_Ability"].Rows[0];
                            string swk = row["走行時間"].ToString();
                            int soukoujikan = 0;
                            int.TryParse(swk, out soukoujikan);
                            swk = row["充電時間"].ToString();
                            int jyudenjikan = 0;
                            int.TryParse(swk, out jyudenjikan);
                            swk = row["搬送時間"].ToString();
                            int hansoujikan = 0;
                            int.TryParse(swk, out hansoujikan);
                            swk = row["搬送回数"].ToString();
                            int hansoukaisuu = 0;
                            int.TryParse(swk, out hansoukaisuu);


                            sql = "UPDATE table_Ability  SET ";
                            sql += "走行時間 =" + (abli.elapsed_seconds + soukoujikan);
                            sql += ",充電時間 =" + (abli.TimeCharges + jyudenjikan);
                            sql += ",搬送時間 =" + (abli.HansouHours + hansoujikan);
                            sql += ",搬送回数 =" + (abli.NumberHansou + hansoukaisuu);
                            ////sql += ",搬送能力=" + rpt.ShelAngle;
                            ////sql += ",待機時間= " + rpt.NumberWork;
                            sql += " where agvid = '@agvid'";
                            sql = sql.Replace("@agvid", abli.agvid);

                            if (!Db.Execute(sql))
                            {
                                err = "DB更新エラー[" + Db.ErrorMessage + "]";
                                Log(sql);
                                Log(err);
                                return err;
                            }
                        }
                    }
                }
            }
            else
            {
                t_Ability =  new BL_IniFile(Path.Combine(Application.StartupPath, @"..\DATA\t_Ability.ini"));
                int iwk = 0; int.TryParse(t_Ability.Read(abli.agvid, "走行時間", "").Trim(), out iwk);
                t_Ability.Write(abli.agvid, "走行時間", abli.elapsed_seconds + iwk);
                iwk = 0; int.TryParse(t_Ability.Read(abli.agvid, "充電時間", "").Trim(), out iwk);
                t_Ability.Write(abli.agvid, "充電時間", abli.TimeCharges + iwk);
                iwk = 0; int.TryParse(t_Ability.Read(abli.agvid, "搬送時間", "").Trim(), out iwk);
                t_Ability.Write(abli.agvid, "搬送時間", abli.HansouHours + iwk);
                iwk = 0; int.TryParse(t_Ability.Read(abli.agvid, "搬送回数", "").Trim(), out iwk);
                t_Ability.Write(abli.agvid, "搬送回数", abli.NumberHansou + iwk);
                //t_Ability.Write(abli.agvid, "搬送能力", abli.elapsed_seconds + iwk);
                //t_Ability.Write(abli.agvid, "待機時間", abli.elapsed_seconds + iwk);
            }
            return err;
        }
        public string Reset_Integrated(int type,DateTime day)
        {
            string err = "";
            if (Program.DborFile == 1)
            {
                if (Dbcon == true)
                {

                    string sql = "UPDATE table_Integrated  SET ";

                    switch (type)
                    {
                        case 1:     //走行距離                    
                            sql += "走行距離 = 0";
                            break;
                        case 2:     //走行距離                    
                            sql += "走行時間 = 0";
                            break;
                        case 3:     //旋回角度                    
                            sql += "旋回角度 = 0.0";
                            break;
                        case 4:    //充電回数                    
                            sql += "充電回数 = 0";
                            break;
                        case 5:    //充電時間                    
                            sql += "充電時間 = 0";
                            break;
                        case 6:    //棚上昇回数                    
                            sql += "棚上昇回数 = 0";
                            break;
                        case 7:    //棚下降回数                    
                            sql += "棚下降回数 = 0";
                            break;
                        case 8:   //棚旋回角度                    
                            sql += "棚旋回角度 = 0.0";
                            break;
                        case 9:    //ステーション作業回数                    
                            sql += "ステーション作業回数 = 0";
                            break;
                        case 10:   //ステーション作業時間                    
                            sql += "ステーション作業時間 = 0";
                            break;

                    }
                    sql += " WHERE 登録日 = '"+ day.ToString("yyyy/MM/dd")+"'";
                    if (!Db.Execute(sql))
                    {
                        err = "DB更新エラー[" + Db.ErrorMessage + "]";
                        Log(sql);
                        Log(err);
                    }
                }
            }
            else
            {
                if (File.Exists(Path.Combine(Application.StartupPath, @"..\DATA\t_Integrated" + day.ToString("yyyyMMdd") + ".ini")))
                {
                    t_Integrated = new BL_IniFile(Path.Combine(Application.StartupPath, @"..\DATA\t_Integrated" + day.ToString("yyyyMMdd") + ".ini"));
                    for(int ii=1;ii<99;ii++)
                    {
                        int iwk = 0;
                        if (!int.TryParse(t_Integrated.Read(ii.ToString(), "走行時間", "").Trim(), out iwk)) continue;

                        switch (type)
                        {
                            case 1:     //走行距離                    
                                t_Integrated.Write(ii.ToString(), "走行距離", 0);
                                break;
                            case 2:     //走行距離                    
                                t_Integrated.Write(ii.ToString(), "走行時間", 0);
                                break;
                            case 3:     //旋回角度    
                                t_Integrated.Write(ii.ToString(), "旋回角度", 0);
                                break;
                            case 4:    //充電回数                    
                                t_Integrated.Write(ii.ToString(), "充電回数", 0);
                                break;
                            case 5:    //充電時間                    
                                t_Integrated.Write(ii.ToString(), "充電時間", 0);
                                break;
                            case 6:    //棚上昇回数                    
                                t_Integrated.Write(ii.ToString(), "棚上昇回数", 0);
                                break;
                            case 7:    //棚下降回数                    
                                t_Integrated.Write(ii.ToString(), "棚下降回数", 0);
                                break;
                            case 8:   //棚旋回角度                    
                                t_Integrated.Write(ii.ToString(), "棚旋回角度", 0);
                                break;
                            case 9:    //ステーション作業回数                    
                                t_Integrated.Write(ii.ToString(), "ステーション作業回数", 0);
                                break;
                            case 10:   //ステーション作業時間                    
                                t_Integrated.Write(ii.ToString(), "ステーション作業時間", 0);
                                break;
                        }
                    }

                }
            }
            return err;
        }
        public string Insert_elapsed(ReportInfo info)
        {
            string sql = "";
            string sqlv = "";
            string err = "";

            if (Program.DborFile == 1 )
            {
                if (Dbcon == true)
                {
                    sql = "insert into table_elapsed (";
                    sqlv = "values (";
                    sql += " mark"; sqlv += "'" + BL_EnumLabel.GetLabel(info.mark) + "'";
                    sql += ",agvid"; sqlv += ",'" + info.agvid + "'";
                    sql += ",time_stamp"; sqlv += ",'" + info.timestamp.ToString() + "'";
                    sql += ",pointX"; sqlv += "," + info.point.X.ToString();
                    sql += ",pointY"; sqlv += "," + info.point.Y.ToString();
                    sql += ",rackid"; sqlv += ",'" + info.rackid.ToString() + "'";
                    sql += ",faceid"; sqlv += ",'" + info.faceid.ToString() + "'";
                    sql += ",stationid"; sqlv += ",'" + info.stationid.ToString() + "'";
                    sql += ",distance"; sqlv += "," + info.distance.ToString();
                    sql += ",qrcount"; sqlv += "," + info.qrcount.ToString();
                    sql += ",elapsed_seconds"; sqlv += "," + info.elapsed_seconds.ToString();
                    sql += ",battery"; sqlv += "," + info.battery.ToString();
                    sql += ",crash_agvid"; sqlv += ",'" + info.crash_agvid + "'";
                    sql += ")"; sqlv += ")";
                    if (!Db.Execute(sql + sqlv))
                    {
                        err = "DB更新エラー[" + Db.ErrorMessage + "]";
                        Log(sql);
                        Log(err);
                    }
                }
            }
            else
            {
                var append = true;
                bool FileExist = System.IO.File.Exists(FilePath1);
                using (var sw = new System.IO.StreamWriter(FilePath1, append, Encoding.Default))
                {
                    #region タイトル行を書込み
                    string title = "";
                    if (!FileExist)
                    {
                        title = " mark" + ",";
                        title += "agvid" + ",";
                        title += "time_stamp" + ",";
                        title += "pointX" + ",";
                        title += "pointY" + ",";
                        title += "rackid" + ",";
                        title += "faceid" + ",";
                        title += "stationid" + ",";
                        title += "distance" + ",";
                        title += "qrcount" + ",";
                        title += "elapsed_seconds" + ",";
                        title += "battery" + ",";
                        title += "crash_agvid";
                        sw.WriteLine(title);
                    }

                    #endregion
                    string line = "";
                    line = BL_EnumLabel.GetLabel(info.mark) + ",";
                    line += info.agvid + ",";
                    line += info.timestamp.ToString() + ",";
                    line += info.point.X.ToString() + ",";
                    line += info.point.Y.ToString() + ",";
                    line += info.rackid.ToString() + ",";
                    line += info.faceid.ToString() + ",";
                    line += info.stationid.ToString() + ",";
                    line += info.distance.ToString() + ",";
                    line += info.qrcount.ToString() + ",";
                    line += info.elapsed_seconds.ToString() + ",";
                    line += info.battery.ToString() + ",";
                    line += info.crash_agvid;

                    #region 行データを書込み

                    sw.WriteLine(line);

                    #endregion
                }
            }
                return err;
        }

    }
}
