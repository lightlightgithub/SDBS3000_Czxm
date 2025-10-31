using NPOI.SS.Formula.Functions;
using SDBS3000.Communicate;
using SDBS3000.Communicate;
using SDBS3000.Utils.AppSettings;
using SDBS3000.Utils.Extensions;
using SDBS3000.ViewModel;
using SDBS3000.Views;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDBS3000.Services
{
    public class MacControl
    {
        static List<Task> tasks = new List<Task>();
        byte result;
        string ret;
        public MacControl()
        {
            MainViewModel.hc.OnEchoReceived += Hc_OnEchoReceived;
            MainViewModel.hc.OnResponseReceived += Hc_OnResponseReceived;
        }

        private void Hc_OnEchoReceived(string obj)
        {
            ret = obj;
            Console.WriteLine("回显报文" + ret);
        }


        /// <summary>
        /// 收到响应报文
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Hc_OnResponseReceived(byte[] obj)
        {
            if (obj.Length >= 6 && (obj[0] == 0xD1 || obj[0] == 0xD2 || obj[0] == 0xD3 || obj[0] == 0xD4 || obj[0] == 0xD5 || obj[0] == 0xD6))
            {
                string hex = BitConverter.ToString(obj);
                Console.WriteLine("响应报文" + hex);
                result = obj[1];
            }
        }

        /// <summary>
        /// 异步启动伺服并等待响应
        /// </summary>
        /// <param name="drivemode"></param>
        /// <param name="rpm"></param>
        /// <returns>是否成功 (Success) ,结果码 (ResultCode)</returns>
        public async Task<(bool Success, byte ResultCode)> ServoStartAsync(int drivemode, ushort rpm)
        {
            if (drivemode == 3)
            {
                return (true, 0x01);
            }

            if (drivemode == 0)
            {
                ushort zsnum = ((ushort)(rpm * Convert.ToDouble(GlobalVar.GetStr("SpeedFactor"))));


                byte[] startCommand = new byte[] { 0xC1, (byte)(zsnum >> 8), (byte)(zsnum & 0xFF), 0x01, 0x00, 0xee };

                try
                {
                    // 使用新的异步方法发送命令并等待响应
                    byte[] response = await GlobalVar.HardwareSerialPort.WriteAsync(startCommand);

                    if (response != null && response.Length >= 6)
                    {
                        byte resultCode = response[1]; // 从响应中获取结果码
                        return (resultCode == 0x01, resultCode);
                    }
                    else
                    {
                        return (false, 0x00); // 响应格式不正确
                    }
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine("启动伺服超时: " + ex.Message);
                    return (false, 0xFF); // 返回失败和一个自定义的超时错误码
                }
                catch (Exception ex)
                {
                    Console.WriteLine("启动伺服失败: " + ex.Message);
                    return (false, 0xFE); // 返回失败和一个自定义的其他错误码
                }
            }
            return (false, 0xFD); // 返回失败和一个未知模式错误码
        }

 
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="drivemode"></param>
        /// <param name="workmode"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public async Task<(bool Success, byte ResultCode)> ServoStopAsync(int drivemode, double workmode, ushort angle)
        {
            if (drivemode == 3)
            {
                return (true, 0x01);
            }

            if (drivemode == 0)
            {
                bool pos = false;//是否开启定位
                if (workmode == 2)
                {
                    pos = true;
                }
                byte[] startCommand = new byte[] { 0xC2, (byte)(pos ? 0x01 : 0x02), (byte)(angle >> 8), (byte)(angle & 0xFF), 0x02, 0xee };

                try
                {
                    byte[] response = await GlobalVar.HardwareSerialPort.WriteAsync(startCommand);

                    if (response != null && response.Length >= 6)
                    {
                        byte resultCode = response[1]; // 从响应中获取结果码
                        return (resultCode == 0x01, resultCode);
                    }
                    else
                    {
                        return (false, 0x00); // 响应格式不正确
                    }
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine("停止伺服超时: " + ex.Message);
                    return (false, 0xFF); // 返回失败和一个自定义的超时错误码
                }
                catch (Exception ex)
                {
                    Console.WriteLine("停止伺服失败: " + ex.Message);
                    return (false, 0xFE); // 返回失败和一个自定义的其他错误码
                }
            }
            return (false, 0xFD); // 返回失败和一个未知模式错误码
        }


 
        /// <summary>
        /// 异步查找脉冲（伺服标定）
        /// </summary>
        /// <returns>是否成功 (Success) 和结果码 (ResultCode)</returns>
        public async Task<(bool Success, byte ResultCode)> SearchPulseAsync()
        {
            byte[] command = new byte[] { 0xC3, 0x01, 0x00, 0x00, 0x00, 0xEE };

            try
            {
                byte[] response = await GlobalVar.HardwareSerialPort.WriteAsync(command);

                if (response != null && response.Length >= 6)
                {
                    byte resultCode = response[1];
                    return (resultCode == 0x01, resultCode);
                }
                else
                {
                    return (false, 0x00); // 响应格式不正确
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("查找脉冲超时: " + ex.Message);
                return (false, 0xFF);
            }
            catch (Exception ex)
            {
                Console.WriteLine("查找脉冲失败: " + ex.Message);
                return (false, 0xFE);
            }
        }

        /// <summary>
        /// 异步设置启停加减速
        /// </summary>
        /// <param name="readwrite">读写标志</param>
        /// <param name="fre">频率</param>
        /// <returns></returns>
        public async Task<(bool Success, byte ResultCode)> AccDecelAsync(bool readwrite, ushort fre)
        {
            // 构建加减速命令
            byte[] command = new byte[] { 0xC4, (byte)(readwrite ? 0x01 : 0x02), (byte)(fre >> 8), (byte)(fre & 0xFF), 0x00, 0xEE };

            try
            {
                byte[] response = await GlobalVar.HardwareSerialPort.WriteAsync(command);

                if (response != null && response.Length >= 6)
                {
                    byte resultCode = response[1];
                    return (resultCode == 0x01, resultCode);
                }
                else
                {
                    return (false, 0x00);
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("设置加减速超时: " + ex.Message);
                return (false, 0xFF);
            }
            catch (Exception ex)
            {
                Console.WriteLine("设置加减速失败: " + ex.Message);
                return (false, 0xFE);
            }
        }


        /// <summary>
        /// 异步报警复位
        /// </summary>
        /// <returns></returns>
        public async Task<(bool Success, byte ResultCode)> AlarmResetAsync()
        {
            // 构建报警复位命令
            byte[] command = new byte[] { 0xC5, 0x01, 0x00, 0x00, 0x00, 0xEE };

            try
            {
                byte[] response = await GlobalVar.HardwareSerialPort.WriteAsync(command);

                if (response != null && response.Length >= 6)
                {
                    byte resultCode = response[1];
                    return (resultCode == 0x01, resultCode);
                }
                else
                {
                    return (false, 0x00);
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("报警复位超时: " + ex.Message);
                return (false, 0xFF);
            }
            catch (Exception ex)
            {
                Console.WriteLine("报警复位失败: " + ex.Message);
                return (false, 0xFE);
            }
        }

        /// <summary>
        /// 报警复位
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        public bool AlarmReset(out byte resultCode)
        {
            bool b = false;
            if (MainViewModel.hc.AlarmReset())
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


        /// <summary>
        /// 异步控制气缸夹紧/松开
        /// </summary>
        /// <param name="clamp">true: 夹紧, false: 松开</param>
        /// <returns></returns>
        public async Task<(bool Success, byte ResultCode)> CylinderCRAsync(bool clamp)
        {
            byte[] command = new byte[] { 0xC6, (byte)(clamp ? 0x01 : 0x02), 0x00, 0x00, 0x00, 0xEE };

            try
            {
                byte[] response = await GlobalVar.HardwareSerialPort.WriteAsync(command);

                if (response != null && response.Length >= 6)
                {
                    byte resultCode = response[1];
                    return (resultCode == 0x01, resultCode);
                }
                else
                {
                    return (false, 0x00);
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("气缸操作超时: " + ex.Message);
                return (false, 0xFF);
            }
            catch (Exception ex)
            {
                Console.WriteLine("气缸操作失败: " + ex.Message);
                return (false, 0xFE);
            }
        }
    }
}
