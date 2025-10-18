using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDBS3000.Communicate.Servo
{
    /// <summary>
    /// 伺服通讯
    /// </summary>
    public class ServoTrans
    {
        /// <summary>
        /// 与沙工对接的伺服
        /// </summary>
        public SerialPort serialPort = new SerialPort();
        private byte _slaveAddress = 1;          // 从机地址
        private IModbusSerialMaster _master;
        // 存储最后一次发送和接收的报文
        public byte[] LastSentPacket { get; private set; }
        public byte[] LastReceivedPacket { get; private set; }
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
                try
                {
                    serialPort.Open();
                    if (!serialPort.IsOpen)
                    {
                        return false;
                    }

                    _master = ModbusSerialMaster.CreateRtu(serialPort);
                    _master.Transport.ReadTimeout = 2000;//读取数据超时100ms
                    _master.Transport.WriteTimeout = 2000;//写入数据超时100ms
                    _master.Transport.Retries = 0;//重试次数
                    _master.Transport.WaitToRetryMilliseconds = 500;//重试间隔
                    return true;
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

        public bool Balstart(int state)
        {
            if (_master == null || !serialPort.IsOpen)
            {
                return false;
            }
            int stateValue = state;
            ushort[] registerBuffer = new ushort[2];
            registerBuffer[0] = (ushort)(stateValue & 0xFFFF);          // 低16位
            registerBuffer[1] = (ushort)((stateValue >> 16) & 0xFFFF);  // 高16位

            _master.WriteMultipleRegisters(2, 0xA875, registerBuffer);
            return true;
        }
        /// <summary>
        /// 3急停
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetSVON(int state)
        {
            try
            { 
                // 将状态值转换为32位数据（两个16位寄存器）
                uint stateValue = 0;

                switch (state)
                {
                    case 0:                      
                        stateValue = 1; // 启动  
                        break;
                    case 1:
                        stateValue = 2; // 停止
                        break;
                    case 2:
                        stateValue = 32;//报警复位
                        break;
                    case 3:
                        stateValue = 8; //急停
                        break;
                    case 4:
                        stateValue = 64;//二次定位启动
                        break;
                    case 5:
                        stateValue = 16;//回原
                        break;
                    case 6:
                        stateValue = 0;
                        break;
                    default:
                        return false;
                }
               
                ushort[] registerBuffer = new ushort[2];
                registerBuffer[0] = (ushort)(stateValue & 0xFFFF);          // 低16位
                registerBuffer[1] = (ushort)((stateValue >> 16) & 0xFFFF);  // 高16位
                
                _master.WriteMultipleRegisters(2, 0xA864, registerBuffer);
                 
                return true;
            }
            catch (Exception ex)
            { 
                return false;
            }
        }

        // 读取硬件数据
        public ushort ReadHoldingRegisters(ushort startAddress, ushort count)
        {
            if (_master == null || !serialPort.IsOpen)
            {
                return 0;
            }
            try
            {
                // 读取从机的保持寄存器
                ushort[] buff_VDI_state = _master.ReadHoldingRegisters(_slaveAddress, startAddress, count);
                Thread.Sleep(100);
                return buff_VDI_state[0];
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        // 写入硬件数据

        public bool WriteSingleRegister32(ushort address, int value)
        {
            if (_master == null || !serialPort.IsOpen)
            {
                return false;
            }

            try
            {
                ushort[] registerBuffer = new ushort[2];
                registerBuffer[0] = (ushort)(value & 0xFFFF);          // 低16位
                registerBuffer[1] = (ushort)((value >> 16) & 0xFFFF);  // 高16位

                _master.WriteMultipleRegisters(_slaveAddress, address, registerBuffer);
         
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // 断开连接
        public void Disconnect()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
        }


    }
}
