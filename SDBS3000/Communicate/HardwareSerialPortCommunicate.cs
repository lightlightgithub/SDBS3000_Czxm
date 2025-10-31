using SDBS3000.Utils.AppSettings;
using SDBS3000.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Communicate
{
    /// <summary>
    /// 端口COM2
    /// </summary>
    public class HardwareSerialPortCommunicate
    {
        public  event Action<string> OnEchoReceived;// 事件：收到回显报文
        private  TaskCompletionSource<string> echoTcs;
        public  event Action<byte[]> OnResponseReceived;// 事件：收到响应报文
        private  List<byte> receiveBuffer = new List<byte>();
        private  SerialPort _serialPort;
        public HardwareSerialPortCommunicate() 
        {
            _serialPort = GlobalVar.HardwareSerialPort;
            _serialPort.DataReceived += SerialPort_DataReceived;
        }
        /// <summary>
        /// 接收硬件传的报文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private  void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //读到01成功，02气缸异常，03伺服异常（02/03更新到报警页面）
            try
            {
                // 循环读取，直到没有更多数据
                while (_serialPort.BytesToRead > 0)
                {
                    int count = _serialPort.BytesToRead;
                    byte[] buffer = new byte[count];
                    _serialPort.Read(buffer, 0, count);
                    receiveBuffer.AddRange(buffer);
                }
                ParseBuffer(); // 解析所有累积数据
            }
            catch (Exception ex)
            {
                // 记录异常
                Console.WriteLine($"接收数据异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 解析所有累积数据
        /// </summary>
        /// <param name="receiveBuffer"></param>
        private  void ParseBuffer()
        {
            while (receiveBuffer.Count >= 6) // 报文长度6
            {
                byte frameHeader = receiveBuffer[0];

                // 判断是否为 回显报文（C1/C2/C3 开头）
                if (frameHeader == 0xC1 || frameHeader == 0xC2 || frameHeader == 0xC3 || frameHeader == 0xC4 || frameHeader == 0xC5 || frameHeader == 0xC6)
                {
                    int length = 6;
                    if (receiveBuffer.Count >= length)
                    {
                        byte[] echoPacket = receiveBuffer.Take(length).ToArray();
                        receiveBuffer.RemoveRange(0, length);

                        string hex = BitConverter.ToString(echoPacket);
                        OnEchoReceived?.Invoke(hex); // 触发事件
                        echoTcs?.TrySetResult(hex);
                    }
                    else
                    {
                        break; // 数据不够，等待下次
                    }
                }
                // 判断是否为 响应报文（D1/D2/D3/D4 开头）
                else if (frameHeader == 0xD1 || frameHeader == 0xD2 || frameHeader == 0xD3 || frameHeader == 0xD4 || frameHeader == 0xD5 || frameHeader == 0xD6)
                {
                    int length = 6;
                    if (receiveBuffer.Count >= length)
                    {
                        byte[] responsePacket = receiveBuffer.Take(length).ToArray();
                        receiveBuffer.RemoveRange(0, length);

                        string hex = BitConverter.ToString(responsePacket);
                        OnResponseReceived?.Invoke(responsePacket);
                        SerialPortExtensions.responseTcs?.TrySetResult(responsePacket);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    // 不是有效帧头，丢弃第一个字节
                    receiveBuffer.RemoveAt(0);
                }
            }
        }
        /// <summary>
        /// 发送启动命令
        /// </summary>
        /// <param name="speed">转速</param>
        public bool SendStart(ushort speed)
        {
            byte[] cmd = new byte[6];
            cmd[0] = 0xC1;
            cmd[1] = (byte)(speed >> 8);
            cmd[2] = (byte)(speed & 0xFF);
            cmd[3] = 0x01;
            cmd[4] = 0x00;
            cmd[5] = 0xEE;
            if (_serialPort.Write(cmd))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 发送停止命令
        /// </summary>
        /// <param name="Position">是否开启定位功能</param>
        /// <param name="angle">停止角度</param>
        public bool SendStop(bool Position, ushort angle)
        {
            byte[] cmd = new byte[6];
            cmd[0] = 0xC2;
            cmd[1] = (byte)(Position ? 0x01 : 0x02); // 01开启，02关闭
            cmd[2] = (byte)(angle >> 8);
            cmd[3] = (byte)(angle & 0xFF);
            cmd[4] = 0x02;
            cmd[5] = 0xEE;
            if (_serialPort.Write(cmd))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 发送查找脉冲命令
        /// </summary>
        public bool SendFindPulse()
        {
            byte[] cmd = { 0xC3, 0x01, 0x00, 0x00, 0x00, 0xEE };
            if (_serialPort.Write(cmd))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 加减速时间
        /// </summary>
        /// <param name="fre"></param>
        /// <returns></returns>
        public bool SendAandD(bool write, ushort fre)
        {
            byte[] cmd = new byte[6];
            cmd[0] = 0xC4;
            cmd[1] = (byte)(write ? 0x01 : 0x02);
            cmd[2] = (byte)(fre >> 8);
            cmd[3] = (byte)(fre & 0xFF);
            cmd[4] = 0x00;
            cmd[5] = 0xEE;
            if (_serialPort.Write(cmd))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 报警复位
        /// </summary>
        /// <returns></returns>
        public  bool AlarmReset()
        {
            byte[] cmd = { 0xC5, 0x01, 0x00, 0x00, 0x00, 0xEE };
            if (_serialPort.Write(cmd))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 气缸夹紧松开
        /// </summary>
        /// <param name="release">false松开 true夹紧</param>
        /// <returns></returns>
        public bool Cylinder(bool clamp)
        {
            byte[] cmd = new byte[6];
            cmd[0] = 0xC6;
            cmd[1] = (byte)(clamp ? 0x01 : 0x02); // 01夹紧，02松开
            cmd[2] = 0x00;
            cmd[3] = 0x00;
            cmd[4] = 0x00;
            cmd[5] = 0xEE;
            if (_serialPort.Write(cmd))
            {
                return true;
            }
            else
                return false;
        }
    }
}
