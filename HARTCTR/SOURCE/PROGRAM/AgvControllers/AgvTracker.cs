using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using BelicsClass.Common;
using BelicsClass.File;
using AgvController;

namespace PROGRAM
{
    public class AgvTracker
    {
        AgvControlManager.FloorAGV owner_agv = null;

        /// <summary>総走行時間</summary>
        public long total_running_ms = 0;
        /// <summary>総走行時間(棚あり)</summary>
        public long total_running_with_rack_ms = 0;
        
        /// <summary>総走行距離</summary>
        public double total_running_distance = 0;
        /// <summary>総走行距離(棚あり)</summary>
        public double total_running_with_rack_distance = 0;

        /// <summary>総走行開始回数</summary>
        public int total_run_start_count = 0;
        /// <summary>総走行停止回数</summary>
        public int total_run_stop_count = 0;

        /// <summary>総旋回角度</summary>
        public int total_turn_degree = 0;
        /// <summary>総旋回回数</summary>
        public int total_turn_count = 0;

        /// <summary>総棚旋回角度</summary>
        public int total_rack_turn_degree = 0;
        /// <summary>総棚旋回回数</summary>
        public int total_rack_turn_count = 0;

        /// <summary>総棚上昇回数</summary>
        public int total_rackup_count = 0;
        /// <summary>総棚下降回数</summary>
        public int total_rackdown_count = 0;

        /// <summary>総充電時間</summary>
        public long total_charge_ms = 0;
        /// <summary>総充電回数</summary>
        public int total_charge_count = 0;

        /// <summary>総ステーション作業時間</summary>
        public long total_station_ms = 0;
        /// <summary>総ステーション作業回数</summary>
        public int total_station_count = 0;

        /// <summary>AGV異常発生回数</summary>
        public int total_error_count = 0;

        public AgvCommunicator.State state_pre = new AgvCommunicator.State(); 

        BL_Stopwatch running_timer = new BL_Stopwatch();
        BL_Stopwatch running_with_rack_timer = new BL_Stopwatch();
        BL_Stopwatch charge_timer = new BL_Stopwatch();
        BL_Stopwatch station_timer = new BL_Stopwatch();
        
        long running_ms = 0;
        long running_with_rack_ms = 0;
        double running_distance = 0;
        double running_with_rack_distance = 0;

        BL_IniFile ini = null;
        BL_Log log = null;

        public AgvTracker(AgvControlManager.FloorAGV agv, AgvCommunicator.State state)
        {
            this.owner_agv = agv;

            state_pre.SetBytes(state.GetBytes());

            ini = new BL_IniFile(Path.Combine(Application.StartupPath, @"..\DATA\AGV_TRACKING_TOTAL_" + agv.id.Trim() + ".ini"));

            total_running_ms = ini.Read("TOTAL", "総走行時間", total_running_ms);
            total_running_with_rack_ms = ini.Read("TOTAL", "総走行時間(棚あり)", total_running_with_rack_ms);
            total_running_distance = ini.Read("TOTAL", "総走行距離", total_running_distance);
            total_running_with_rack_distance = ini.Read("TOTAL", "総走行距離(棚あり)", total_running_with_rack_distance);
            total_run_start_count = ini.Read("TOTAL", "総走行開始回数", total_run_start_count);
            total_run_stop_count = ini.Read("TOTAL", "総走行停止回数", total_run_stop_count);
            total_turn_degree = ini.Read("TOTAL", "総旋回角度", total_turn_degree);
            total_turn_count = ini.Read("TOTAL", "総旋回回数", total_turn_count);
            total_rack_turn_degree = ini.Read("TOTAL", "総棚旋回角度", total_rack_turn_degree);
            total_rack_turn_count = ini.Read("TOTAL", "総棚旋回回数", total_rack_turn_count);
            total_rackup_count = ini.Read("TOTAL", "総棚上昇回数", total_rackup_count);
            total_rackdown_count = ini.Read("TOTAL", "総棚下降回数", total_rackdown_count);
            total_charge_ms = ini.Read("TOTAL", "総充電時間", total_charge_ms);
            total_charge_count = ini.Read("TOTAL", "総充電回数", total_charge_count);
            total_station_ms = ini.Read("TOTAL", "総ステーション作業時間", total_station_ms);
            total_station_count = ini.Read("TOTAL", "総ステーション作業回数", total_station_count);
            total_error_count = ini.Read("TOTAL", "総異常回数", total_error_count);

            //ini = new BL_IniFile(Path.Combine(Application.StartupPath, @"..\DATA\AGV_TRACKING_TOTAL_" + agv.id.Trim() + ".ini"));

            log = new BL_Log("", "AGV_TRACKING_" + agv.id.Trim() + ".TXT");
            log.Add(",*");
        }

        public void ChangeState(AgvCommunicator.State state)
        {
            long _total_running_ms = total_running_ms;
            long _total_running_with_rack_ms = total_running_with_rack_ms;
            double _total_running_distance = total_running_distance;
            double _total_running_with_rack_distance = total_running_with_rack_distance;
            int _total_run_start_count = total_run_start_count;
            int _total_run_stop_count = total_run_stop_count;
            int _total_turn_degree = total_turn_degree;
            int _total_turn_count = total_turn_count;
            int _total_rack_turn_degree = total_rack_turn_degree;
            int _total_rack_turn_count = total_rack_turn_count;
            int _total_rackup_count = total_rackup_count;
            int _total_rackdown_count = total_rackdown_count;
            long _total_charge_ms = total_charge_ms;
            int _total_charge_count = total_charge_count;
            long _total_station_ms = total_station_ms;
            int _total_station_count = total_station_count;
            int _total_error_count = total_error_count;

            bool is_rack = false;

            if (state_pre.error_code != state.error_code)
            {
                if (state.error_code == 0)
                {
                    log.Add(",e,0");
                }
                else
                {
                    total_error_count++;
                    log.Add(",E," + state.error_code);
                }
            }

            if (state_pre.Location != state.Location)
            {
                running_ms += running_timer.Restart();
                double distance = owner_agv.Distance(state_pre.Location, state.Location);
                running_distance += distance;
                log.Add(",L," + running_distance + "," + running_ms + "," + state_pre.map + "," + state_pre.Location.X + "," + state_pre.Location.Y + "," + state.map + "," + state.Location.X + "," + state.Location.Y);

                is_rack = state.sta_rack;

                if (is_rack)
                {
                    running_with_rack_ms += running_with_rack_timer.Restart();
                    running_with_rack_distance += distance;
                    log.Add(",LR," + running_with_rack_distance + "," + running_with_rack_ms + "," + state_pre.map + "," + state_pre.Location.X + "," + state_pre.Location.Y + "," + state.map + "," + state.Location.X + "," + state.Location.Y);
                }
            }

            if (state_pre.deg != state.deg)
            {
                //@@@これでいい？
                total_turn_degree += (Math.Abs(state.deg - state_pre.deg) + 180) % 180;
                total_turn_count++;

                log.Add(",R,"+ total_turn_degree + "," + state_pre.deg + "," + state.deg);
            }

            if (state_pre.sta_rack != state.sta_rack)
            {
                if (state.sta_rack)
                {
                    total_rackup_count++;
                    log.Add(",U," + total_rackup_count + "," + state.racktype + state.rack_no);
                }
                else
                {
                    total_rackdown_count++;
                    log.Add(",u," + total_rackdown_count);
                }
            }

            if (state_pre.sta_rack && state.sta_rack)
            {
                if (state_pre.rack_deg != state.rack_deg)
                {
                    //@@@これでいい？
                    total_rack_turn_degree += (Math.Abs(state.rack_deg - state_pre.rack_deg) + 180) % 180;
                    total_rack_turn_count++;
                    log.Add(",r," + total_rackup_count);
                }
            }

            if (state_pre.cmd != (ushort)AgvCommunicator.State.CMD.STATE && state.cmd == (ushort)AgvCommunicator.State.CMD.STATE)
            {
                //走行開始
                total_run_start_count++;
                running_distance = 0;
                running_with_rack_distance = 0;

                log.Add(",S," + total_run_start_count + "," + total_running_ms + "," + total_running_distance);

                if (!running_timer.IsRunning)
                {
                    running_timer.Restart();
                }

                if (station_timer.IsRunning)
                {
                    //ステーション到着後の走行開始
                    station_timer.Stop();

                    total_station_count++;
                    total_station_ms += station_timer.ElapsedMilliseconds;

                    log.Add(",w," + total_station_count + "," + total_station_ms);
                    station_timer.Reset();
                }
            }

            if (state_pre.cmd != (ushort)AgvCommunicator.State.CMD.REQUEST && state.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST)
            {
                //走行停止
                total_run_stop_count++;

                if (running_timer.IsRunning)
                {
                    running_timer.Stop();

                    long running_seconds = running_timer.ElapsedMilliseconds;
                    total_running_ms += running_seconds;
                    total_running_distance += running_distance;
                    total_running_with_rack_ms += running_with_rack_ms;
                    total_running_with_rack_distance += running_with_rack_distance;

                    log.Add(",s," + total_run_stop_count + "," + total_running_ms + "," + total_running_distance + "," + total_running_with_rack_distance);

                    running_timer.Reset();
                }

                if (owner_agv.on_qr.station_id != "")
                {
                    if (owner_agv.rack != null && owner_agv.rack.req != null && owner_agv.rack.req.station == owner_agv.on_qr.station_id)
                    {
                        log.Add(",W," + total_station_count + "," + total_station_ms + ","+ owner_agv.on_qr.station_id);
                        //ステーション到着
                        station_timer.Restart();
                    }
                }
            }

            if (state_pre.sta_charge != state.sta_charge)
            {
                if (state.sta_charge)
                {
                    if (!charge_timer.IsRunning)
                    {
                        charge_timer.Restart();
                        total_charge_count++;

                        log.Add(",C," + total_charge_count + "," + total_charge_ms);
                    }
                }
                else
                {
                    if (charge_timer.IsRunning)
                    {
                        charge_timer.Stop();
                        total_charge_ms += charge_timer.ElapsedMilliseconds;
                        charge_timer.Reset();

                        log.Add(",c," + total_charge_count + "," + total_charge_ms);
                    }
                }
            }

            if (_total_running_ms != total_running_ms) ini.Write("TOTAL", "総走行時間", total_running_ms);
            //if (_total_running_with_rack_ms != total_running_with_rack_ms) ini.Write("TOTAL", "総走行時間(棚あり)", total_running_with_rack_ms);
            if (_total_running_distance != total_running_distance) ini.Write("TOTAL", "総走行距離", total_running_distance);
            if (_total_running_with_rack_distance != total_running_with_rack_distance) ini.Write("TOTAL", "総走行距離(棚あり)", total_running_with_rack_distance);
            if (_total_run_start_count != total_run_start_count) ini.Write("TOTAL", "総走行開始回数", total_run_start_count);
            if (_total_run_stop_count != total_run_stop_count) ini.Write("TOTAL", "総走行停止回数", total_run_stop_count);
            if (_total_turn_degree != total_turn_degree) ini.Write("TOTAL", "総旋回角度", total_turn_degree);
            if (_total_turn_count != total_turn_count) ini.Write("TOTAL", "総旋回回数", total_turn_count);
            if (_total_rack_turn_degree != total_rack_turn_degree) ini.Write("TOTAL", "総棚旋回角度", total_rack_turn_degree);
            if (_total_rack_turn_count != total_rack_turn_count) ini.Write("TOTAL", "総棚旋回回数", total_rack_turn_count);
            if (_total_rackup_count != total_rackup_count) ini.Write("TOTAL", "総棚上昇回数", total_rackup_count);
            if (_total_rackdown_count != total_rackdown_count) ini.Write("TOTAL", "総棚下降回数", total_rackdown_count);
            if (_total_charge_ms != total_charge_ms) ini.Write("TOTAL", "総充電時間", total_charge_ms);
            if (_total_charge_count != total_charge_count) ini.Write("TOTAL", "総充電回数", total_charge_count);
            if (_total_station_ms != total_station_ms) ini.Write("TOTAL", "総ステーション作業時間", total_station_ms);
            if (_total_station_count != total_station_count) ini.Write("TOTAL", "総ステーション作業回数", total_station_count);
            if (_total_error_count != total_error_count) ini.Write("TOTAL", "総異常回数", total_error_count);
            
            
            state_pre.SetBytes(state.GetBytes());
        }




        long ability_total_running_ms = 0;
        BL_Stopwatch abirity_total_running_ms_timer = new BL_Stopwatch();

        long ability_rack_running_ms = 0;
        long ability_rack_running_count = 0;
        BL_Stopwatch abirity_rack_running_ms_timer = new BL_Stopwatch();

        long ability_charge_ms = 0;
        long ability_charge_count = 0;
        BL_Stopwatch ability_charge_timer = new BL_Stopwatch();

        public void Ability_Start(AgvCommunicator.State state, AgvController.AgvOrderCommunicator.RequestBase req)
        {
            if (!abirity_total_running_ms_timer.IsRunning)
            {
                abirity_total_running_ms_timer.Restart();
            }

            {
                var r = req as AgvController.AgvOrderCommunicator.RequestDelivery;
                if (r != null && r.rack.Trim() != "")
                {
                    if (!abirity_rack_running_ms_timer.IsRunning)
                    {
                        abirity_rack_running_ms_timer.Restart();
                    }
                }
            }

            {
                var r = req as AgvController.AgvOrderCommunicator.RequestMove;
                if (r != null)
                {
                    if (r != null && r.rack_action.Trim() != "0")
                    {
                        if (!abirity_rack_running_ms_timer.IsRunning)
                        {
                            abirity_rack_running_ms_timer.Restart();
                        }
                    }
                }
            }
        }

        public void Ability_RackTake(AgvCommunicator.State state, AgvController.AgvOrderCommunicator.RequestBase req)
        {
            if (abirity_total_running_ms_timer.IsRunning)
            {
                ability_total_running_ms += abirity_total_running_ms_timer.ElapsedMilliseconds;
                abirity_total_running_ms_timer.Restart();
            }

            if (abirity_rack_running_ms_timer.IsRunning)
            {
                ability_rack_running_ms += abirity_rack_running_ms_timer.ElapsedMilliseconds;
                abirity_rack_running_ms_timer.Restart();
                ability_rack_running_count++;
            }
        }

        public void Ability_RackReturn(AgvCommunicator.State state, AgvController.AgvOrderCommunicator.RequestBase req)
        {
            if (abirity_total_running_ms_timer.IsRunning)
            {
                ability_total_running_ms += abirity_total_running_ms_timer.ElapsedMilliseconds;
                abirity_total_running_ms_timer.Restart();
                ability_rack_running_count++;
            }

            if (abirity_rack_running_ms_timer.IsRunning)
            {
                ability_rack_running_ms += abirity_rack_running_ms_timer.ElapsedMilliseconds;
                abirity_rack_running_ms_timer.Restart();
                ability_rack_running_count++;
            }
        }

        public void Ability_StationArrive(AgvCommunicator.State state, AgvController.AgvOrderCommunicator.RequestBase req)
        {
            if (abirity_total_running_ms_timer.IsRunning)
            {
                ability_total_running_ms += abirity_total_running_ms_timer.ElapsedMilliseconds;
                ability_rack_running_count++;
            }

            if (abirity_rack_running_ms_timer.IsRunning)
            {
                ability_rack_running_ms += abirity_rack_running_ms_timer.ElapsedMilliseconds;
                ability_rack_running_count++;
            }
        }

        public void Ability_StationLeave(AgvCommunicator.State state, AgvController.AgvOrderCommunicator.RequestBase req)
        {
            abirity_total_running_ms_timer.Restart();

            if (state.sta_rack)
            {
                abirity_rack_running_ms_timer.Restart();
            }
            else
            {
                abirity_rack_running_ms_timer.Stop();
                abirity_rack_running_ms_timer.Reset();
            }
        }

        public void Ability_ChargeStart(AgvCommunicator.State state, AgvController.AgvOrderCommunicator.RequestBase req)
        {
            if (!ability_charge_timer.IsRunning)
            {
                ability_charge_timer.Restart();
                ability_charge_count++;
            }
        }

        public void Ability_ChargeStop(AgvCommunicator.State state, AgvController.AgvOrderCommunicator.RequestBase req)
        {
            if (ability_charge_timer.IsRunning)
            {
                ability_charge_timer.Stop();
                ability_charge_ms += ability_charge_timer.ElapsedMilliseconds;
                ability_charge_timer.Reset();
            }
        }
    }
}
