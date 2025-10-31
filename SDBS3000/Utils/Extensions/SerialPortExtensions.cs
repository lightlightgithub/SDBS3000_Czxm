using Modbus.Device;
using Newtonsoft.Json;
using SDBS3000.Communicate;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDBS3000.Log.Log;

namespace SDBS3000.Utils.Extensions
{
    public static class SerialPortExtensions
    {
        public static TaskCompletionSource<byte[]> responseTcs;
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="serialPort"></param>
        /// <returns></returns>
        public static bool Connect(this SerialPort serialPort)
        {
            try
            {
                //关闭之前打开的串口
                if (serialPort != null)
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();
                    if (!serialPort.IsOpen)
                    {
                        serialPort.Open();
                        //SDBS3000.Log.Log.Write(LogType.INFO, $"串口连接成功,端口：{serialPort.PortName}");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                SDBS3000.Log.Log.Write(LogType.ERROR, $"串口连接异常: {ex.Message}\r\n,端口：{serialPort.PortName}");
                return false;
            }
        }
        /// <summary>
        /// 检查连接
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected(this SerialPort serialPort)
        {
            if(serialPort == null || !serialPort.IsOpen) 
                return false;
            return true;
        }
        /// <summary>
        /// 写入指令
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Write(this SerialPort serialPort,byte[] data)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                SDBS3000.Log.Log.Write(LogType.INFO, $"指令写入失败,串口未连接，端口:{serialPort.PortName}");
                return false;
            }
            try
            {
                serialPort.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                SDBS3000.Log.Log.Write(LogType.ERROR, $"指令写入异常: {ex.Message}\r\n,端口：{serialPort.PortName}");
                return false;
            }
        }
        /// <summary>
        /// 关闭端口
        /// </summary>
        public static void Close(this SerialPort serialPort)
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        /// <summary>
        /// 异步方式写入指令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="TimeoutException"></exception>
        public static async Task<byte[]> WriteAsync(this SerialPort serialPort,byte[] command, int timeoutMs = 5000)
        {
            // 重置之前的任务
            responseTcs = new TaskCompletionSource<byte[]>();

            try
            {
                serialPort.Write(command, 0, command.Length);
            }
            catch (Exception ex)
            {
                // 如果发送失败，直接抛出异常
                throw new Exception($"发送命令失败: {ex.Message}");
            }

            // 等待响应任务完成，或者超时
            var completedTask = await Task.WhenAny(responseTcs.Task, Task.Delay(timeoutMs));

            if (completedTask == responseTcs.Task)
            {
                // 成功收到响应
                return await responseTcs.Task;
            }
            else
            {
                // 超时
                throw new TimeoutException("等待响应超时。");
            }
        }

    }
}
