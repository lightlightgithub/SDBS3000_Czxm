using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Communicate.Drive
{
    public class HardwareCon
    {
        public SerialPort serialPort = new SerialPort();
        public event Action<byte[]> OnEchoReceived;// 事件：收到回显报文
        public event Action<byte[]> OnResponseReceived;// 事件：收到响应报文
        private List<byte> receiveBuffer = new List<byte>();
        public bool IsConnected => this.serialPort?.IsOpen == true;
        public bool Connect(string portName)
        {
            try
            {
                //关闭之前打开的串口
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }

                // 按照通信规则设置
                serialPort.PortName = portName;//获取下拉框的串口
                serialPort.BaudRate = 38400;//波特率
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.Two;
                serialPort.Parity = Parity.None;
                serialPort.DataReceived += SerialPort_DataReceived;
                try
                {
                    if (!serialPort.IsOpen)
                    {
                        serialPort.Open();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 接收硬件传的报文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //读到01成功，02气缸异常，03伺服异常（02/03更新到报警页面）
            try
            {
                int count = serialPort.BytesToRead;
                byte[] buffer = new byte[count];
                serialPort.Read(buffer, 0, count);

                receiveBuffer.AddRange(buffer);
                ParseBuffer(); // 解析接收缓冲区
            }
            catch (Exception ex)
            {
            }
        }

        private void ParseBuffer()
        {
            while (receiveBuffer.Count >= 6) // 报文长度6
            {
                byte frameHeader = receiveBuffer[0];

                // 判断是否为 回显报文（C1/C2/C3 开头）
                if (frameHeader == 0xC1 || frameHeader == 0xC2 || frameHeader == 0xC3)
                {
                    int length = GetCommandLength(frameHeader);
                    if (receiveBuffer.Count >= length)
                    {
                        byte[] echoPacket = receiveBuffer.Take(length).ToArray();
                        receiveBuffer.RemoveRange(0, length);

                        string hex = BitConverter.ToString(echoPacket);
                        OnEchoReceived?.Invoke(echoPacket); // 触发事件
                    }
                    else
                    {
                        break; // 数据不够，等待下次
                    }
                }
                // 判断是否为 响应报文（D1/D2/D3/D4 开头）
                else if (frameHeader == 0xD1 || frameHeader == 0xD2 || frameHeader == 0xD3 || frameHeader == 0xD4)
                {
                    int length = 6; // D1/D2/D3/D4 响应都是 6 字节
                    if (receiveBuffer.Count >= length)
                    {
                        byte[] responsePacket = receiveBuffer.Take(length).ToArray();
                        receiveBuffer.RemoveRange(0, length);

                        string hex = BitConverter.ToString(responsePacket);
                        OnResponseReceived?.Invoke(responsePacket);                  
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
        /// 获取 C 命令报文的长度
        /// </summary>
        private int GetCommandLength(byte cmd)
        {
            if (cmd == 0xC1 || cmd == 0xC2 || cmd == 0xC3)
            {
                return 6;
            }
            else
                return 0;
        }
        /// <summary>
        /// 发送报文（十六进制）
        /// </summary>
        public bool SendCommand(byte[] data)
        {
            if (this.serialPort == null || !this.serialPort.IsOpen)
            {
                return false;
            }
            try
            {
                this.serialPort.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送启动命令
        /// </summary>
        /// <param name="speed">转速</param>
        public bool SendStart(ushort speed)
        {
            byte[] cmd = new byte[7];
            cmd[0] = 0xC1;
            cmd[1] = (byte)(speed >> 8);
            cmd[2] = (byte)(speed & 0xFF);
            cmd[3] = 0x01;
            cmd[4] = 0x00;
            cmd[5] = 0xEE;
            if (SendCommand(cmd))
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
            byte[] cmd = new byte[8];
            cmd[0] = 0xC2;
            cmd[1] = (byte)(Position ? 0x01 : 0x02); // 01开启，02关闭
            cmd[2] = (byte)(angle >> 8);
            cmd[3] = (byte)(angle & 0xFF);
            cmd[4] = 0x02;
            cmd[5] = 0xEE;
            if(SendCommand(cmd))
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
            if (SendCommand(cmd))
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
        public bool SendAandD(ushort fre)
        {
            byte[] cmd = new byte[7];
            cmd[0] = 0xC4;
            cmd[1] = (byte)(fre >> 8);
            cmd[2] = (byte)(fre & 0xFF);
            cmd[3] = 0x01;
            cmd[4] = 0x00;
            cmd[5] = 0xEE;
            if (SendCommand(cmd))
            {
                return true;
            }
            else
                return false;
        }
    }
}
