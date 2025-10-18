using NPOI.SS.Formula.Functions;
using SDBS3000.ViewModel;
using SDBS3000.Views;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDBS3000.Control
{
    public class MacControl
    { 
        
        static List<Task> tasks = new List<Task>();
        byte result ;
        byte[] ret;
        public MacControl()
        {
           MainViewModel.hc.OnEchoReceived += Hc_OnEchoReceived;
            MainViewModel.hc.OnResponseReceived += Hc_OnResponseReceived;
        }
        
        /// <summary>
        /// 收到响应报文
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Hc_OnResponseReceived(byte[] obj)
        {
            if (obj.Length >= 6 && (obj[0] == 0xD1 || obj[0] == 0xD2 || obj[0] == 0xD3))
            {
                result = obj[1];
            }
        }

        /// <summary>
        /// 收到回显报文
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Hc_OnEchoReceived(byte[] obj)
        {
            ret = obj;
        }

        public bool ServoStart(int drivemode, ushort rpm, out byte resultCode)
        {
            bool rt = false;
            if (drivemode == 0)
            {
                if (MainViewModel.hc.SendStart(rpm))
                {
                    if (ret != null)
                    {
                        Console.WriteLine(ret);
                        if (result == 0x01)
                        {
                            rt = true;
                        }                       
                        else
                        {
                            rt = false;
                        }
                    }
                }
            }
            //手动控制
            else if (drivemode == 3)
            {
                rt = true;
            }
            resultCode = result;
            return rt;
        }
        /// <summary>
        /// 伺服控制启停
        /// </summary>
        public bool ServoStop(int drivemode,double workmode,ushort angle, out byte resultCode)
        {
            bool rt = false;
            if (drivemode == 0)
            {
                bool pos = false;//是否开启定位
                if (workmode == 2)
                {
                    pos = true;
                }
                if (MainViewModel.hc.SendStop(pos, angle))
                {
                    if (ret != null)
                    {
                        Console.WriteLine(ret);
                        if (result == 0x01)
                        {
                            rt = true;
                        }                       
                        else
                        {
                            rt = false;
                        }
                    }
                }
            }
            //手动控制
            else if (drivemode == 3)
            {
                rt = true;
            }
            resultCode = result;
            return rt;
        }

        /// <summary>
        /// 查找脉冲（伺服标定）
        /// </summary>
        /// <returns></returns>
        public bool SearchPulse(out byte resultCode)
        {
            bool b = false;
            if (MainViewModel.hc.SendFindPulse())
            {
                if (ret != null)
                {
                    Console.WriteLine(ret);
                    if (result == 0x01)
                    {
                        b = true;
                    }                   
                    else
                    {
                        b = false;
                    }
                }
            }
            resultCode = result;
            return b;
        }

        public static async Task<bool> Start(int drivemode, double rpm)
        {
            Task.WaitAll(tasks.ToArray());
            bool b = await Task.Run<bool>(() =>
            {
                bool rt = false;
                if (drivemode == 0)
                {
                    if (MainViewModel.svTrans.WriteSingleRegister32(0x16, 2))
                    {
                        Thread.Sleep(200);
                        int zsnum = (int)((ushort)rpm * Convert.ToDouble(GlobalVar.GetStr("SpeedFactor")));
                        if (MainViewModel.svTrans.WriteSingleRegister32(0x63E, zsnum))
                        {
                            Thread.Sleep(50);
                            rt = MainViewModel.svTrans.SetSVON(0);
                            Thread.Sleep(50);

                        }
                    }

                }
                //手动控制
                else if (drivemode == 3)
                {
                    rt = true;
                }
                return rt;
            });
            return b;
        }

        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="drivemode"></param>
        public static void Stop(int drivemode)
        {
            Task tstop = Task.Run(() =>
            {
                if (drivemode == 0)
                {
                    MainViewModel.svTrans.SetSVON(3);
                    Thread.Sleep(2000);
                    MainViewModel.svTrans.SetSVON(2);
                    Thread.Sleep(200);
                    MainViewModel.svTrans.SetSVON(5);
                    Thread.Sleep(200);
                    MainViewModel.svTrans.WriteSingleRegister32(0x16, 1);
                    Thread.Sleep(200);
                    MainViewModel.bal._runDB.bal_result.rpm = 0;

                }
            });
            tasks.Add(tstop);
        }

        public static void PosStop(int angel)
        {
            Task tstop = Task.Run(() =>
            {
                MainViewModel.svTrans.WriteSingleRegister32(0x63A, angel);
                Thread.Sleep(50);
                MainViewModel.svTrans.SetSVON(1);
                while (true)
                {
                    if (GlobalVar.FlagofSFSpeed == 0)
                    {
                        Thread.Sleep(10);
                        MainViewModel.svTrans.SetSVON(5);
                        Thread.Sleep(200);
                        MainViewModel.svTrans.WriteSingleRegister32(0x16, 1);
                        Thread.Sleep(200);
                        MainViewModel.bal._runDB.bal_result.rpm = 0;
                        return;
                    }

                }
            });
            tasks.Add(tstop);
        }
    }
}
