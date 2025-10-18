using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDBS3000.BalTest
{
    public class OP_Servo
    {

        private IModbusMaster Master;
        byte slaveAddress = 01;
        SerialPort serialPort;//串口号
        public bool IsOpen => serialPort.IsOpen;
        int m_PlsToMm = 1;
        public OP_Servo(int PlsToMm)
        {
            m_PlsToMm = PlsToMm;
        }
        /// <summary>
        /// 连接伺服
        /// </summary>
        /// <param name="portName">端口名称</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="slaveAddress">从站地址</param>
        /// <returns></returns>
        public bool ConnectServo(string portName, int BaudRate = 57600, byte slaveAddress = 1)//57600
        {
            try
            {
                if (serialPort != null)
                {
                    serialPort.Dispose();
                }
                serialPort = new SerialPort(portName, BaudRate);
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Open();
                if (Master != null)
                {
                    Master.Dispose();
                }

                Master = ModbusSerialMaster.CreateRtu(serialPort);
                //master.Transport.ReadTimeout = 2000;
                //master.Transport.WriteTimeout = 2000;
                Master.Transport.ReadTimeout = 2000;//读取数据超时100ms
                Master.Transport.WriteTimeout = 2000;//写入数据超时100ms
                Master.Transport.Retries = 3;//重试次数
                Master.Transport.WaitToRetryMilliseconds = 10;//重试间隔
                                                              //Master.Transport.Retries = 10;

                System.Threading.Thread.Sleep(50);
                //创建线程
                Task.Run(new Action(() =>
                {
                    while (true)
                    {
                        GetSVStates();
                        System.Threading.Thread.Sleep(50);
                    }
                }));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        object locked = new object();
        void GetSVStates()
        {

            lock (locked)
            {
                ushort[] buff_VDI_state = Master.ReadHoldingRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), 1);
                mVDI_state = buff_VDI_state[0];

                //目标位置
                ushort[] buff_MovTo_Pos = Master.ReadHoldingRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x1100", 16) + 12), 4);
                mMovTo_Pos = UshortToInt32(buff_MovTo_Pos);
                mMovTo_rpm = buff_MovTo_Pos[2];
                mMovTo_AccDecTime = buff_MovTo_Pos[3];
                ushort[] buff_VDO_state = Master.ReadHoldingRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x1700", 16) + 32), 1);
                mVDO_state = buff_VDO_state[0];
                ushort[] buff_Curr_Pos = Master.ReadHoldingRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x0B00", 16) + 7), 2);
                mCurr_Pos = UshortToInt32(buff_Curr_Pos);

                mJOG_param = Master.ReadHoldingRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x0600", 16) + 3), 3);


            }

        }

        /// <summary>
        /// 打开伺服使能
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetSVON(bool state)
        {
            ushort[] registerBuffer = new ushort[1];

            if (state)
            {
                registerBuffer[0] = 1;
            }
            else
            {
                registerBuffer[0] = 0;
            }
            //Master.read
            Thread.Sleep(1000);
            Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), registerBuffer);
            return true;
        }

        /// <summary>
        /// 反向点动
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetSVONRe(bool state)
        {
            ushort[] registerBuffer = new ushort[1];
            ushort[] registerBuffer1 = new ushort[1];
            registerBuffer1[0] = 0;
            if (state)
            {
                registerBuffer[0] = 1;
            }
            else
            {
                registerBuffer[0] = 0;
            }
            //Master.read           
            Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), registerBuffer1);
            Master.WriteMultipleRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x3100", 16) + 2), registerBuffer);
            return true;
        }

        /// <summary>
        /// 正向点动
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetSVONFor(bool state)
        {
            ushort[] registerBuffer = new ushort[1];
            ushort[] registerBuffer1 = new ushort[1];
            registerBuffer1[0] = 1;
            if (state)
            {
                registerBuffer[0] = 1;
            }
            else
            {
                registerBuffer[0] = 0;
            }
            //Master.read           
            Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), registerBuffer1);
            Master.WriteMultipleRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x3100", 16) + 1), registerBuffer);
            return true;
        }
        Int32 mCurr_Pos, mMovTo_Pos;
        ushort mVDO_state, mVDI_state, mMovTo_rpm, mMovTo_AccDecTime;
        ushort[] mJOG_param;

        public double Curr_Pos
        {
            get { return (double)mCurr_Pos / m_PlsToMm; }
            // set { SetValue(Curr_PosProperty, value); }
        }

        public double 目标位置
        {
            get { return (double)mMovTo_Pos / m_PlsToMm; }
            // set { SetValue(Curr_PosProperty, value); }
        }

        public ushort 绝对定位转速
        {
            get { return mMovTo_rpm; }
            // set { SetValue(Curr_PosProperty, value); }
        }
        public ushort 绝对定位加减速时间
        {
            get { return mMovTo_AccDecTime; }
            // set { SetValue(Curr_PosProperty, value); }
        }






        public bool 零速信号
        {
            get
            {
                if ((mVDO_state >> 0 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }

        }
        public bool 位置到达
        {
            get
            {
                if ((mVDO_state >> 1 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }

        }
        public bool 故障报警
        {
            get
            {
                if ((mVDO_state >> 2 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }

        }
        public bool 原点回零到位
        {
            get
            {
                if ((mVDO_state >> 3 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }

        }


        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public bool 私服使能状态
        {
            get
            {
                if ((mVDI_state >> 0 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }

        public bool 正向点动
        {
            get
            {
                if ((mVDI_state >> 1 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            set
            {
                MoveStop();

                if (value)
                {
                    GetVDIStates();
                    //反向点动 = false;
                    ushort[] rBuffer1 = new ushort[1];
                    rBuffer1[0] = (ushort)(mVDI_state & System.Convert.ToUInt16("1111111111111011", 2));
                    Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), rBuffer1);



                    GetVDIStates();
                    ushort[] rBuffer = new ushort[1];
                    rBuffer[0] = (ushort)(mVDI_state | System.Convert.ToUInt16("10", 2));
                    Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), rBuffer);

                }
                else
                {

                    GetVDIStates();
                    ushort[] rBuffer = new ushort[1];
                    rBuffer[0] = (ushort)(mVDI_state & System.Convert.ToUInt16("1111111111111101", 2));
                    Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), rBuffer);
                }

            }

        }

        public bool 反向点动
        {
            get
            {
                if ((mVDI_state >> 2 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            set
            {

                MoveStop();


                if (value)
                {
                    GetVDIStates();
                    //正向点动 = false;
                    ushort[] rBuffer1 = new ushort[1];
                    rBuffer1[0] = (ushort)(mVDI_state & System.Convert.ToUInt16("1111111111111101", 2));
                    Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), rBuffer1);
                    System.Threading.Thread.Sleep(100);

                    GetVDIStates();
                    ushort[] rBuffer = new ushort[1];
                    rBuffer[0] = (ushort)(mVDI_state | System.Convert.ToUInt16("100", 2));
                    Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), rBuffer);

                }
                else
                {
                    GetVDIStates();
                    ushort[] rBuffer = new ushort[1];
                    rBuffer[0] = (ushort)(mVDI_state & System.Convert.ToUInt16("1111111111111011", 2));
                    Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), rBuffer);
                    Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), rBuffer);
                }

            }



        }

        public bool MoveStop()
        {
            // GetSVStates();
            GetVDIStates();

            ushort[] rBuffer = new ushort[1];
            rBuffer[0] = (ushort)(mVDI_state & System.Convert.ToUInt16("1111111111110111", 2));
            Master.WriteMultipleRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), rBuffer);

            // System.Threading.Thread.Sleep(10);
            return true;
        }

        void GetVDIStates()
        {

            lock (locked)
            {
                ushort[] buff_VDI_state = Master.ReadHoldingRegisters(slaveAddress, System.Convert.ToUInt16("0x3100", 16), 1);
                mVDI_state = buff_VDI_state[0];
            }
        }


        /// <summary>
        /// 转速上限6000
        /// </summary>
        public ushort 点动转速
        {
            get
            {

                return mJOG_param[0];


            }

            set
            {
                if (value <= 6000)
                {

                    ushort[] rBuffer = new ushort[1];
                    rBuffer[0] = value;
                    Master.WriteMultipleRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x0600", 16) + 3), rBuffer);
                }





            }

        }


        /// <summary>
        /// 单位ms
        /// </summary>
        public ushort 点动加速度时间
        {
            get
            {

                return mJOG_param[1];


            }

            set
            {

                ushort[] rBuffer = new ushort[1];
                rBuffer[0] = value;
                Master.WriteMultipleRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x0600", 16) + 5), rBuffer);//加速
                                                                                                                          //  Master.WriteMultipleRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x0600", 16) + 6), rBuffer);//减速

            }

        }

        /// <summary>
        /// 单位ms
        /// </summary>
        public ushort 点动减速度时间
        {
            get
            {

                return mJOG_param[2];


            }

            set
            {

                ushort[] rBuffer = new ushort[1];
                rBuffer[0] = value;
                Master.WriteMultipleRegisters(slaveAddress, (ushort)(System.Convert.ToUInt16("0x0600", 16) + 6), rBuffer);


            }

        }











        public bool 多段位置使能状态
        {
            get
            {
                if ((mVDI_state >> 3 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }
        public bool 原点回归使能状态
        {
            get
            {
                if ((mVDI_state >> 4 & 1) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }


        }







        Int32 UshortToInt32(ushort[] val)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(val[0] & 0xFF);
            bytes[1] = (byte)(val[0] >> 8);
            bytes[2] = (byte)(val[1] & 0xFF);
            bytes[3] = (byte)(val[1] >> 8);

            return BitConverter.ToInt32(bytes, 0);
        }


        ushort[] Int32ToUshort(Int32 val)
        {

            byte[] bytes = BitConverter.GetBytes(val);
            ushort[] b = new ushort[2];
            b[0] = BitConverter.ToUInt16(bytes, 0);
            b[1] = BitConverter.ToUInt16(bytes, 2);

            return b;
        }





    }
}
