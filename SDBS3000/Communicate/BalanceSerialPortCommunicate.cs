using SDBS3000.Services;
using SDBS3000.Utils.AppSettings;
using SDBS3000.Utils.Extensions;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Communicate
{
    public delegate void SerEventHandler(int message = 0);
    /// <summary>
    /// 端口COM1
    /// </summary>
    public class BalanceSerialPortCommunicate
    {
        /// <summary>
        /// 1测量中  3停止  6 测量准备 7预热期过滤 死区滤波 8夹具补偿完成  
        /// </summary>
        public  event SerEventHandler OnEventTriggered;
        public  BalanceData _balanceData = new BalanceData();
        private  Queue<double[]> dataQueue = new Queue<double[]>();
        private  double[] valAverages = new double[4];
        public  RUN_DB _runDB;
        private  List<byte> _buffer = new List<byte>();
        private  SerialPort _serialPort;
        private static readonly object _lock = new object();
        public BalanceSerialPortCommunicate() 
        {
            _serialPort = GlobalVar.BalanceSerialPort;
            _serialPort.DataReceived += SerialPort_DataReceived;
            initializeTag(); 
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
                lock (_lock)
                {
                    int bytesCount = _serialPort.BytesToRead;
                    if (bytesCount == 0) return;

                    byte[] buffer = new byte[bytesCount];
                    _serialPort.Read(buffer, 0, bytesCount);
                    _buffer.AddRange(buffer); // 缓存所有收到的字节
                }
                // 尝试解析完整帧
                TryParseFrames();
            }
            catch (Exception ex)
            {
                // 记录异常
                Console.WriteLine($"接收数据异常: {ex.Message}");
            }
        }
        private void initializeTag()
        {
            _balanceData.val = new double[4];
            _balanceData.val_AVE = new double[6];
            _balanceData.valFl = new double[22];
            _balanceData.valQl = new double[22];
            _balanceData.valFr = new double[22];
            _balanceData.valQr = new double[22];
            _balanceData.cal_sample = new double[5][];
            _balanceData.cal_sample[0] = new double[4];
            _balanceData.cal_sample[1] = new double[4];
            _balanceData.cal_sample[2] = new double[4];
            _balanceData.cal_sample[3] = new double[4];
            _balanceData.cal_sample[4] = new double[4];
            _balanceData.bal_sample = new double[4];
            //this.set0_key_up = false;
            _balanceData.set0_clamp_up = false;
            _runDB = new RUN_DB();
            _balanceData.DeviceNo = "S V-R2D45405";
            //if (this.remainDays > 0)
            _balanceData.pass = true;
            //else
            //    this.pass = false;
            // GetXlh();

        }
        /// <summary>
        /// 解析缓冲区中的数据，寻找并处理完整帧
        /// </summary>
        public  void TryParseFrames()
        {
            lock (_lock)
            {
                while (_buffer.Count >= 3)
                {
                    int num1 = 0; // 状态：0=FF, 1=3A, 2=pcb版本, 3=接收数据
                    int num2 = 0; // 剩余待接收的数据字节数
                    List<int> intList = new List<int>(); // 临时存储当前帧的整数形式

                    // 检查帧头
                    if (_buffer[0] != 0xFF || _buffer[1] != 0x3A)
                    {
                        _buffer.RemoveAt(0);
                        continue;
                    }

                    int version = _buffer[2];
                    if (version == 3)
                    {
                        // PCB10
                        num1 = 3; // 直接进入接收数据状态
                        num2 = 7; // 还剩7个数据字节
                        _balanceData.PCBversion = 10;
                        // 清空并添加帧头
                        intList.Clear();
                        intList.Add(0xFF); // byte.MaxValue
                        intList.Add(0x3A); // 58
                        intList.Add(3);
                    }
                    else if (version == 1 || version == 17)
                    {
                        // PCB9 或 PCB11
                        num1 = 3;
                        num2 = 21; // 还剩21个数据字节
                        if (version == 17)
                            _balanceData.PCBversion = 11;
                        else
                            _balanceData.PCBversion = 9; // version == 1

                        intList.Clear();
                        intList.Add(0xFF);
                        intList.Add(0x3A);
                        intList.Add(version);
                    }
                    else
                    {
                        // 未知类型，跳过第一个字节
                        _buffer.RemoveAt(0);
                        continue;
                    }

                    // 检查是否有足够字节组成完整帧
                    int totalLength = 3 + num2; // 头3字节 + 数据字节数
                    if (_buffer.Count >= totalLength)
                    {
                        // 从第3个字节开始，连续读取 num2 个字节
                        for (int i = 0; i < num2; i++)
                        {
                            intList.Add(_buffer[3 + i]); // _buffer[3] 是第一个数据字节
                        }

                        // 移除已处理的整个帧
                        _buffer.RemoveRange(0, totalLength);

                        // 处理完整帧
                        ProcessCompleteFrame(intList);
                    }
                    else
                    {
                        break; // 帧不完整，等待更多数据
                    }
                }
            }
        }
        /// <summary>
        /// 处理一个完整的数据帧（由整数列表表示）
        /// </summary>
        /// <param name="intList">包含完整帧所有字节的整数列表</param>
        private  void ProcessCompleteFrame(List<int> intList)
        {

            // 将整数列表转换为十六进制字符串数组
            string[] array = intList.ConvertAll<string>(i => Convert.ToByte(i).ToString("X2")).ToArray();

            // 将整数列表转换为字节数组
            _balanceData._recBytes = intList.ConvertAll<byte>(i => Convert.ToByte(i)).ToArray();

            // 构建十六进制字符串 _recData
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in array)
                stringBuilder.Append(str + " ");
            _balanceData._recData = stringBuilder.ToString().Trim();

            // 更新接收到的数据长度
            _balanceData._recLen = _balanceData._recBytes.Length;

            // 调用业务处理函数
            if (_balanceData.pass)
            {
                putd();
            }

        }
        /// <summary>
        /// 动平衡加标定：对应runmode  0测量1定标空转2定标左加量3定标右加量
        /// </summary>
        private  void putd()
        {
            _balanceData._sendBytes = new byte[10];
            int num1 = 0;
            double[] numArray1 = new double[8]
            {
                1.0,
                2.0,
                4.0,
                8.0,
                16.0,
                32.0,
                64.0,
                128.0
            };
            double[] numArray2 = new double[4]
            {
                1E-06,
                0.0001,
                0.0004,
                0.001521
            };
            double[] numArray3 = new double[4]
            {
                0.525,
                5.0,
                10.0,
                19.5
            };
            double[] numArray4 = new double[4]
            {
                0.001,
                0.01,
                0.02,
                0.039
            };
            double[] br0 = new double[2];
            double[] bi0 = new double[2];
            //计算转差范围
            double num2 = _runDB.set_test.Rev_difference / 2.0 * 0.01 * _runDB.set_run.set_rpm;
            for (int index = 2; index < _balanceData._recLen; ++index)
            {
                num1 += (int)_balanceData._recBytes[index];
                if (num1 > (int)byte.MaxValue)
                    num1 -= 256;
            }
            if (num1 != 0)
                return;

            if (_balanceData._recBytes[2] == (byte)3)
            {
                _runDB.bal_result.rpm = Convert.ToInt32(_balanceData._recBytes[5]) * 65536 + Convert.ToInt32(_balanceData._recBytes[4]) * 256 + Convert.ToInt32(_balanceData._recBytes[3]);
                if (_runDB.bal_start && (!_runDB.bal_start || (double)_runDB.bal_result.rpm >= _runDB.set_run.set_rpm - num2 && (double)_runDB.bal_result.rpm <= _runDB.set_run.set_rpm + num2))
                    return;
                _balanceData.ki = 0;// 重置采样计数器
                _balanceData.start_AVE = 0;// 重置平均启动标志 
            }
            else//第三个字节0x01
            {
                _runDB.bal_result.singleL = Convert.ToInt32(_balanceData._recBytes[4]);
                _runDB.bal_result.singleR = Convert.ToInt32(_balanceData._recBytes[5]);
                if (_runDB.bal_result.singleL > (int)sbyte.MaxValue)//负数处理
                    _runDB.bal_result.singleL = (int)byte.MaxValue - _runDB.bal_result.singleL;
                if (_runDB.bal_result.singleR > (int)sbyte.MaxValue)
                    _runDB.bal_result.singleR = (int)byte.MaxValue - _runDB.bal_result.singleR;
                _runDB.bal_result.rpm = Convert.ToInt32(_balanceData._recBytes[8]) * 65536 + Convert.ToInt32(_balanceData._recBytes[7]) * 256 + Convert.ToInt32(_balanceData._recBytes[6]);
                _runDB.bal_result.rpm /= 10;
                double rpm = (double)_runDB.bal_result.rpm;
                OnEventTriggered?.Invoke();
                //if (!_runDB.bal_start)
                //    return;
                double num3 = (double)(Convert.ToInt32(_balanceData._recBytes[11]) * 65536 + Convert.ToInt32(_balanceData._recBytes[10]) * 256 + Convert.ToInt32(_balanceData._recBytes[9]));
                if (_balanceData._recBytes[11] > (byte)127)
                    num3 -= 16777216.0;//左实部
                double num4 = (double)(Convert.ToInt32(_balanceData._recBytes[14]) * 65536 + Convert.ToInt32(_balanceData._recBytes[13]) * 256 + Convert.ToInt32(_balanceData._recBytes[12]));
                if (_balanceData._recBytes[14] > (byte)127)
                    num4 -= 16777216.0;//左虚部
                double num5 = (double)(Convert.ToInt32(_balanceData._recBytes[17]) * 65536 + Convert.ToInt32(_balanceData._recBytes[16]) * 256 + Convert.ToInt32(_balanceData._recBytes[15]));
                if (_balanceData._recBytes[17] > (byte)127)
                    num5 -= 16777216.0;
                double num6 = (double)(Convert.ToInt32(_balanceData._recBytes[20]) * 65536 + Convert.ToInt32(_balanceData._recBytes[19]) * 256 + Convert.ToInt32(_balanceData._recBytes[18]));

                if (_balanceData._recBytes[20] > (byte)127)
                    num6 -= 16777216.0;

                if (_balanceData.PCBversion == 9 || _balanceData.PCBversion == 11)
                {
                    //帧头
                    _balanceData._sendBytes[0] = byte.MaxValue;
                    _balanceData._sendBytes[1] = (byte)58;

                    _balanceData._sendBytes[2] = (byte)0;
                    int num7 = Convert.ToInt32(_runDB.set_test.refalsh * 0.1 * _runDB.set_run.set_rpm / 60.0);
                    if (num7 > 64 && _balanceData.PCBversion == 9)
                        num7 = 64;
                    if (num7 > (int)byte.MaxValue)
                        num7 = (int)byte.MaxValue;
                    _balanceData._sendBytes[3] = Convert.ToByte(num7);
                    _balanceData._sendBytes[4] = _balanceData.PCBversion != 11 ? Convert.ToByte(_runDB.set_test.pre_amp) : (_balanceData.start_AVE != 2 ? (byte)0 : (byte)1);
                    _balanceData._sendBytes[5] = Convert.ToByte(_runDB.set_test.aft_amp);
                    _balanceData._sendBytes[6] = (byte)0;
                    _balanceData._sendBytes[7] = (byte)0;
                    _balanceData._sendBytes[8] = (byte)0;
                    int num8 = 256;
                    for (int index = 2; index < 9; ++index)
                    {
                        num8 -= (int)_balanceData._sendBytes[index];
                        if (num8 < 0)
                            num8 += 256;
                    }
                    _balanceData._sendBytes[9] = Convert.ToByte(num8);
                    if (_serialPort.Write(_balanceData._sendBytes))
                    {
                        List<string> str = new List<string>();
                        foreach (var sendByte in _balanceData._sendBytes)
                        {
                            str.Add(sendByte.ToString("X2"));
                        }
                        _balanceData._sendData = string.Join(" ", str);
                    }
                    else
                        _balanceData._sendData = "";
                }


                if (rpm > _runDB.set_run.set_rpm + num2 || rpm < _runDB.set_run.set_rpm - num2 || _runDB.set_test.pre_amp > 3)
                {
                    _balanceData.ki = 0;
                    _balanceData.start_AVE = 0;
                    OnEventTriggered?.Invoke(6);
                }
                //在转速范围内  第一次进来
                else if (_balanceData.ki == 0 && _balanceData.start_AVE < 2)
                {
                    _balanceData.start_AVE = 1;
                    _runDB.Bal_start = true;

                }


                if (_balanceData.start_AVE == 0 || !_runDB.bal_start)
                {
                    _balanceData.ki = 0;

                }
                else//转速在转差范围内
                {
                    ++_balanceData.ki;
                    if (_balanceData.start_AVE == 1)// 延迟测量，等待系统稳定
                    {
                        if (_balanceData.ki < _runDB.set_test.delayMea)
                        {
                            OnEventTriggered?.Invoke(7);
                            return;
                        }
                        _balanceData.ki = 1;
                        _balanceData.start_AVE = 2;
                    }
                    double num9 = Math.PI / 30.0;
                    double num10 = rpm * num9;//角频率
                    if (num10 < 10.0)
                        num10 = 10.0;
                    double num11 = num10 * num10;
                    double num12 = 1.0;
                    if (num10 > 0.0 && _balanceData.PCBversion < 11)//统一化
                        num12 = 1.0 / Math.Sqrt(1.0 + 0.001521 * num11) * (numArray3[_runDB.set_test.pre_amp] / Math.Sqrt(1.0 + numArray2[_runDB.set_test.pre_amp] * num11)) * (0.39 * num10 / Math.Sqrt(1.0 + 0.1521 * num11)) * numArray1[_runDB.set_test.aft_amp];
                    double fl0 = num3 / num12;
                    double fr0 = num5 / num12;
                    double ql0 = num4 / num12;
                    double qr0 = num6 / num12;
                    int cntMax = _runDB.set_test.ki_max <= 20 && _runDB.set_runmode <= 0 ? _runDB.set_test.ki_max : 20;//确定最大采样次数
                    filter(0, _balanceData.ki, cntMax, fl0, ql0, fr0, qr0);

                    _balanceData.valFl[20] = _balanceData.val[0];//滤波后的值
                    _balanceData.valQl[20] = _balanceData.val[1];
                    _balanceData.valFr[20] = _balanceData.val[2];
                    _balanceData.valQr[20] = _balanceData.val[3];

                    if (_balanceData.ki < cntMax)
                        cntMax = _balanceData.ki;
                    for (int index = 0; index < cntMax; ++index)
                    {
                        if (index == 0)
                        {
                            _balanceData.valFl[21] = Math.Abs(_balanceData.valFl[index] - _balanceData.valFl[20]) / (double)cntMax;//波动值（历史数据与平均值偏差）
                            _balanceData.valQl[21] = Math.Abs(_balanceData.valQl[index] - _balanceData.valQl[20]) / (double)cntMax;
                            _balanceData.valFr[21] = Math.Abs(_balanceData.valFr[index] - _balanceData.valFr[20]) / (double)cntMax;
                            _balanceData.valQr[21] = Math.Abs(_balanceData.valQr[index] - _balanceData.valQr[20]) / (double)cntMax;
                        }
                        else
                        {
                            _balanceData.valFl[21] += Math.Abs(_balanceData.valFl[index] - _balanceData.valFl[20]) / (double)cntMax;
                            _balanceData.valQl[21] += Math.Abs(_balanceData.valQl[index] - _balanceData.valQl[20]) / (double)cntMax;
                            _balanceData.valFr[21] += Math.Abs(_balanceData.valFr[index] - _balanceData.valFr[20]) / (double)cntMax;
                            _balanceData.valQr[21] += Math.Abs(_balanceData.valQr[index] - _balanceData.valQr[20]) / (double)cntMax;
                        }
                    }
                    double x1 = _balanceData.valFl[20];
                    double y1 = _balanceData.valQl[20];
                    double x2 = _balanceData.valFr[20];
                    double y2 = _balanceData.valQr[20];
                    if (_runDB.set_run.compesation)//电补偿
                    {
                        x1 -= _runDB.cps_val[0];//空测值
                        y1 -= _runDB.cps_val[1];
                        x2 -= _runDB.cps_val[2];
                        y2 -= _runDB.cps_val[3];
                    }
                    xyzq(ref x1, ref y1);//幅值 相位
                    xyzq(ref x2, ref y2);
                    _balanceData.bal_sample[0] = x1;
                    _balanceData.bal_sample[1] = angle(y1);
                    _balanceData.bal_sample[2] = x2;
                    _balanceData.bal_sample[3] = angle(y2);

                    _balanceData.valFl[21] = Math.Sqrt(_balanceData.valFl[21] * _balanceData.valFl[21] + _balanceData.valQl[21] * _balanceData.valQl[21]);//综合波动指标
                    _balanceData.valQl[21] = x1 / x2 / Math.Cos(y1 - y2);// 左右分离比
                    _balanceData.valFr[21] = Math.Sqrt(_balanceData.valFr[21] * _balanceData.valFr[21] + _balanceData.valQr[21] * _balanceData.valQr[21]);
                    _balanceData.valQr[21] = x2 / x1 / Math.Cos(y2 - y1);
                    


                    double num13 = _balanceData.val[0];//均值滤波后
                    double num14 = _balanceData.val[1];
                    x2 = _balanceData.val[2];
                    double num15 = _balanceData.val[3];

                    if (_runDB.set_runmode > 0)//标定
                    {
                        if (_runDB.set_runmode != _balanceData._lastRunMode || (_balanceData.ki == 1 && _balanceData.start_AVE == 2))
                        {
                            // 清空队列，重新开始计算
                            dataQueue.Clear();
                            // 更新上一次运行模式记录
                            _balanceData._lastRunMode = _runDB.set_runmode;
                        }

                        if (dataQueue.Count >= _runDB.set_test.ki_max)
                        {
                            // 获取最早的数据用于平均值计算
                            double[] oldestData = dataQueue.Dequeue();
                            // 对每个值进行增量更新：减去移除的值，加上新值，再除以最大数量
                            for (int i = 0; i < 4; i++)
                            {
                                valAverages[i] = (valAverages[i] * _runDB.set_test.ki_max - oldestData[i] + _balanceData.val[i]) / _runDB.set_test.ki_max;
                            }
                        }
                        else
                        {
                            int currentCount = dataQueue.Count;

                            // 对每个值进行增量更新：基于当前数量计算新平均值
                            for (int i = 0; i < 4; i++)
                            {
                                if (currentCount == 0)
                                {
                                    // 如果队列是空的，新值就是平均值
                                    valAverages[i] = _balanceData.val[i];
                                }
                                else
                                {
                                    // 否则按照比例计算新平均值
                                    valAverages[i] = valAverages[i] * (currentCount / (double)(currentCount + 1)) +
                                                    _balanceData.val[i] / (currentCount + 1);
                                }
                            }
                        }
                        dataQueue.Enqueue((double[])_balanceData.val.Clone());
                        if (_runDB.set_runmode <= 5)
                        {
                            for (int index = 0; index < 4; ++index)
                            {
                                _balanceData.cal_sample[_runDB.set_runmode - 1][index] = valAverages[index];
                                if (_runDB.set_runmode == 1)//标定空转
                                    _runDB.cps_val[index] = _balanceData.val[index];
                            }
                        }
                        double x3 = _balanceData.val[0];
                        double y3 = _balanceData.val[1];
                        x2 = _balanceData.val[2];
                        double y4 = _balanceData.val[3];
                        double x4 = x3 + x2;
                        double y5 = y3 + y4;
                        xyzq(ref x3, ref y3);
                        xyzq(ref x2, ref y4);
                        xyzq(ref x4, ref y5);
                        _runDB.bal_result.fl = x3 / 100000.0;//幅值
                        _runDB.bal_result.ql = y3 * 57.29578;//相位 弧度转角度
                        _runDB.bal_result.fr = x2 / 100000.0;
                        _runDB.bal_result.qr = y4 * 57.29578;
                        _runDB.bal_result.fm = x4 / 100000.0;
                        _runDB.bal_result.qm = y5 / 57.29578;
                        OnEventTriggered?.Invoke();
                    }
                    else
                    {
                        #region 键、夹具补偿
                        //键补偿
                        if (_runDB.set_key.start_test && !_balanceData.set0_key_up)
                        {
                            _runDB.set_key.compesation = false;
                            _balanceData.set0_key_up = true;
                            _runDB.set_key.test_times = 0;
                            for (int index = 0; index < 4; ++index)
                                _runDB.set_key.cps_val[index] = 0.0;
                        }
                        if (!_runDB.set_key.start_test && _balanceData.set0_key_up)
                        {
                            _balanceData.set0_key_up = false;
                            _runDB.set_key.test_times = 0;
                            for (int index = 0; index < 4; ++index)
                                _runDB.set_key.cps_val[index] = 0.0;
                        }
                        if (_runDB.set_key.start_test && _balanceData.ki == _runDB.set_test.ki_max)
                        {
                            ++_runDB.set_key.test_times;
                            if (_runDB.set_key.test_times == 1)
                            {
                                for (int index = 0; index < 4; ++index)
                                    _runDB.set_key.cps_val[index] = _balanceData.val[index];
                            }
                            else
                            {
                                for (int index = 0; index < 4; ++index)
                                    _runDB.set_key.cps_val[index] = _balanceData.val[index] - _runDB.set_key.cps_val[index];
                                _runDB.set_key.start_test = false;
                                _balanceData.set0_key_up = false;
                            }
                        }
                        //夹具补偿
                        if (_runDB.set_clamp.start_test && !_balanceData.set0_clamp_up)
                        {
                            _balanceData.set0_clamp_up = true;
                            _runDB.set_clamp.compesation = false;
                            _balanceData.clamp_times = 0;
                            for (int index = 0; index < 4; ++index)
                                _runDB.set_clamp.cps_val[index] = 0.0;
                        }
                        if (!_runDB.set_clamp.start_test && _balanceData.set0_clamp_up)
                        {
                            _balanceData.set0_clamp_up = false;
                            _balanceData.clamp_times = _runDB.set_clamp.test_times;
                            for (int index = 0; index < 4; ++index)
                                _runDB.set_clamp.cps_val[index] = 0.0;
                        }
                        if (_runDB.set_clamp.start_test && _balanceData.ki == _runDB.set_test.ki_max)
                        {
                            ++_balanceData.clamp_times;
                            for (int index = 0; index < 4; ++index)
                                _runDB.set_clamp.cps_val[index] += _balanceData.val[index];
                            if (_balanceData.clamp_times >= _runDB.set_clamp.test_times)
                            {
                                _balanceData.set0_clamp_up = false;
                                _runDB.set_clamp.start_test = false;
                                for (int index = 0; index < 4; ++index)
                                    _runDB.set_clamp.cps_val[index] /= (double)_runDB.set_clamp.test_times;
                                OnEventTriggered?.Invoke(8);
                            }
                        }
                        #endregion



                        //左量值                
                        double num16 = _balanceData.val[0];
                        double num17 = _balanceData.val[1];
                        x2 = _balanceData.val[2];
                        double num18 = _balanceData.val[3];

                        if (_runDB.set_clamp.compesation)
                        {
                            num16 -= _runDB.set_clamp.cps_val[0];
                            num17 -= _runDB.set_clamp.cps_val[1];
                            x2 -= _runDB.set_clamp.cps_val[2];
                            num18 -= _runDB.set_clamp.cps_val[3];
                        }
                        if (_runDB.set_key.compesation)
                        {
                            num16 -= _runDB.set_key.cps_val[0];
                            num17 -= _runDB.set_key.cps_val[1];
                            x2 -= _runDB.set_key.cps_val[2];
                            num18 -= _runDB.set_key.cps_val[3];
                        }

                        if (_runDB.set_run.compesation)
                        {
                            num16 -= _runDB.cps_val[0];
                            num17 -= _runDB.cps_val[1];
                            x2 -= _runDB.cps_val[2];
                            num18 -= _runDB.cps_val[3];
                        }
                        else
                        {
                            _runDB.cps_val[0] = _balanceData.val[0] - (_runDB.set_clamp.compesation ? _runDB.set_clamp.cps_val[0] : 0);
                            _runDB.cps_val[1] = _balanceData.val[1] - (_runDB.set_clamp.compesation ? _runDB.set_clamp.cps_val[1] : 0);
                            _runDB.cps_val[2] = _balanceData.val[2] - (_runDB.set_clamp.compesation ? _runDB.set_clamp.cps_val[2] : 0);
                            _runDB.cps_val[3] = _balanceData.val[3] - (_runDB.set_clamp.compesation ? _runDB.set_clamp.cps_val[3] : 0);
                        }

                        double num19 = 0.0;
                        if (num10 >= 0.0 && _balanceData.PCBversion < 11)//相位补偿
                            num19 = Math.Atan(100.0 / 39.0 / num10) - Math.Atan(0.039 * num10) - Math.Atan(numArray4[_runDB.set_test.pre_amp] * num10);
                        if (_runDB.hard_run.enable)
                        {
                            //静平衡
                            if (_runDB.set_run.single_side)
                            {
                                double num20 = (num16 + x2) * _runDB.set0.cal_h[0];
                                double num21 = (num17 + num18) * _runDB.set0.cal_h[0];
                                if (_balanceData.ki == 1)
                                {
                                    _balanceData.val_AVE[4] = num20;
                                    _balanceData.val_AVE[5] = num21;
                                }
                                else//均值滤波
                                {
                                    _balanceData.val_AVE[4] = _balanceData.val_AVE[4] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num20 / (double)_balanceData.ki;
                                    _balanceData.val_AVE[5] = _balanceData.val_AVE[5] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num21 / (double)_balanceData.ki;
                                }
                                double x5 = _balanceData.val_AVE[4];
                                double y6 = _balanceData.val_AVE[5];
                                xyzq(ref x5, ref y6);
                                double num22 = x5 / num11 / _runDB.hard_run.r1;//不平衡量大小 num11角频率平方
                                y6 = y6 + _runDB.set0.cal_h[1] + num19;//角度
                                if (!_runDB.set_run.add_mode0)//静加重
                                    y6 += 3.1415926;
                                _runDB.bal_result.fl = 0.0;
                                _runDB.bal_result.ql = 0.0;
                                _runDB.bal_result.fr = 0.0;
                                _runDB.bal_result.qr = 0.0;
                                _runDB.bal_result.fm = num22;
                                _runDB.bal_result.qm = angle(y6);

                            }
                            else//双面
                            {
                                double x6 = num16 * _runDB.set0.cal_h[0];
                                x2 *= _runDB.set0.cal_h[2];
                                double y7 = num17 * _runDB.set0.cal_h[0];
                                double y8 = num18 * _runDB.set0.cal_h[2];
                                xyzq(ref x6, ref y7);
                                xyzq(ref x2, ref y8);
                                double num23 = y7 + _runDB.set0.cal_h[1] + num19;
                                double num24 = y8 + _runDB.set0.cal_h[3] + num19;
                                bi0[0] = x6 * Math.Cos(num23);//面1量值
                                br0[0] = x2 * Math.Cos(num24);
                                bi0[1] = x6 * Math.Sin(num23);//面1角度
                                br0[1] = x2 * Math.Sin(num24);
                                double num25 = bi0[0];
                                x2 = br0[0];
                                double num26 = bi0[1];
                                double num27 = br0[1];
                                if (_runDB.hard_run.bear <= 6)    //双面
                                {
                                    double a = _runDB.hard_run.A;
                                    double b = _runDB.hard_run.B;
                                    double c = _runDB.hard_run.C;
                                    if (BEAR(_runDB.hard_run.bear, ref a, ref b, ref c))
                                    {
                                        num25 = bi0[0] + (a * bi0[0] - c * br0[0]) / b;
                                        x2 = br0[0] - (a * bi0[0] - c * br0[0]) / b;
                                        num26 = bi0[1] + (a * bi0[1] - c * br0[1]) / b;
                                        num27 = br0[1] - (a * bi0[1] - c * br0[1]) / b;
                                    }
                                }
                                double num28 = num25 + x2;
                                double num29 = num26 + num27;
                                if (_balanceData.ki == 1)
                                {
                                    _balanceData.val_AVE[0] = num25;
                                    _balanceData.val_AVE[1] = num26;
                                    _balanceData.val_AVE[2] = x2;
                                    _balanceData.val_AVE[3] = num27;
                                    _balanceData.val_AVE[4] = num28;
                                    _balanceData.val_AVE[5] = num29;
                                }
                                else//均值滤波
                                {
                                    _balanceData.val_AVE[0] = _balanceData.val_AVE[0] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num25 / (double)_balanceData.ki;
                                    _balanceData.val_AVE[1] = _balanceData.val_AVE[1] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num26 / (double)_balanceData.ki;
                                    _balanceData.val_AVE[2] = _balanceData.val_AVE[2] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + x2 / (double)_balanceData.ki;
                                    _balanceData.val_AVE[3] = _balanceData.val_AVE[3] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num27 / (double)_balanceData.ki;
                                    _balanceData.val_AVE[4] = _balanceData.val_AVE[4] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num28 / (double)_balanceData.ki;
                                    _balanceData.val_AVE[5] = _balanceData.val_AVE[5] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num29 / (double)_balanceData.ki;
                                }
                                double x7 = _balanceData.val_AVE[0];
                                double y9 = _balanceData.val_AVE[1];
                                x2 = _balanceData.val_AVE[2];
                                double y10 = _balanceData.val_AVE[3];
                                double x8 = _balanceData.val_AVE[4];
                                double y11 = _balanceData.val_AVE[5];
                                xyzq(ref x7, ref y9);
                                xyzq(ref x2, ref y10);
                                xyzq(ref x8, ref y11);
                                /*double num9 = Math.PI / 30.0;
                                double num10 = rpm * num9;
                                if (num10 < 10.0)
                                    num10 = 10.0;
                                double num11 = num10 * num10;*/
                                double num30 = x7 / num11 / _runDB.hard_run.r1;//面1量值
                                x2 = x2 / num11 / _runDB.hard_run.r2;//面2量值
                                double num31 = _runDB.hard_run.bear < 7 || _runDB.hard_run.bear > 9 ? 2.0 * x8 / num11 / (_runDB.hard_run.r1 + _runDB.hard_run.r2) : x8 / num11 / _runDB.hard_run.r1;//双面还是单面
                                if (!_runDB.set_run.add_mode1)//加重
                                {
                                    y9 += 3.1415926;
                                }
                                if (!_runDB.set_run.add_mode2)
                                {
                                    y10 += 3.1415926;
                                }
                                if (!_runDB.set_run.add_mode0)
                                {
                                    y11 += 3.1415926;
                                }
                                _runDB.bal_result.fl = num30;
                                _runDB.bal_result.ql = angle(y9);
                                _runDB.bal_result.fr = x2;
                                _runDB.bal_result.qr = angle(y10);
                                _runDB.bal_result.fm = num31;
                                _runDB.bal_result.qm = angle(y11);
                            }
                        }
                        else if (_runDB.set_run.single_side)
                        {
                            double num32 = (num16 + x2) * _runDB.set0.cal_ar[0];
                            double num33 = (num17 + num18) * _runDB.set0.cal_ar[0];
                            if (_balanceData.ki == 1)
                            {
                                _balanceData.val_AVE[4] = num32;
                                _balanceData.val_AVE[5] = num33;
                            }
                            else
                            {
                                _balanceData.val_AVE[4] = _balanceData.val_AVE[4] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num32 / (double)_balanceData.ki;
                                _balanceData.val_AVE[5] = _balanceData.val_AVE[5] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num33 / (double)_balanceData.ki;
                            }
                            double x9 = _balanceData.val_AVE[4];
                            double y12 = _balanceData.val_AVE[5];
                            xyzq(ref x9, ref y12);
                            y12 = y12 + _runDB.set0.cal_ai[0] + num19;
                            if (!_runDB.set_run.add_mode0)
                                y12 += 3.1415926;
                            _runDB.bal_result.fl = 0.0;
                            _runDB.bal_result.ql = 0.0;
                            _runDB.bal_result.fr = 0.0;
                            _runDB.bal_result.qr = 0.0;
                            _runDB.bal_result.fm = x9;
                            _runDB.bal_result.qm = angle(y12);
                        }
                        else
                        {
                            br0[0] = num16;
                            br0[1] = x2;
                            bi0[0] = num17;
                            bi0[1] = num18;
                            if (!acgas(_runDB.set0.cal_ar, _runDB.set0.cal_ai, ref br0, ref bi0))
                                return;
                            double num34 = br0[0] * 0.1;
                            x2 = br0[1] * 0.1;
                            double num35 = bi0[0] * 0.1;
                            double num36 = bi0[1] * 0.1;
                            double num37 = num34 + x2;
                            double num38 = num35 + num36;
                            if (_balanceData.ki == 1)
                            {
                                _balanceData.val_AVE[0] = num34;
                                _balanceData.val_AVE[1] = num35;
                                _balanceData.val_AVE[2] = x2;
                                _balanceData.val_AVE[3] = num36;
                                _balanceData.val_AVE[4] = num37;
                                _balanceData.val_AVE[5] = num38;
                            }
                            else
                            {
                                _balanceData.val_AVE[0] = _balanceData.val_AVE[0] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num34 / (double)_balanceData.ki;
                                _balanceData.val_AVE[1] = _balanceData.val_AVE[1] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num35 / (double)_balanceData.ki;
                                _balanceData.val_AVE[2] = _balanceData.val_AVE[2] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + x2 / (double)_balanceData.ki;
                                _balanceData.val_AVE[3] = _balanceData.val_AVE[3] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num36 / (double)_balanceData.ki;
                                _balanceData.val_AVE[4] = _balanceData.val_AVE[4] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num37 / (double)_balanceData.ki;
                                _balanceData.val_AVE[5] = _balanceData.val_AVE[5] * (double)(_balanceData.ki - 1) / (double)_balanceData.ki + num38 / (double)_balanceData.ki;
                            }
                            double x10 = _balanceData.val_AVE[0];
                            double y13 = _balanceData.val_AVE[1];
                            x2 = _balanceData.val_AVE[2];
                            double y14 = _balanceData.val_AVE[3];
                            double x11 = _balanceData.val_AVE[4];
                            double y15 = _balanceData.val_AVE[5];
                            xyzq(ref x10, ref y13);
                            xyzq(ref x2, ref y14);
                            xyzq(ref x11, ref y15);
                            double rad1 = y13 + num19;
                            double rad2 = y14 + num19;
                            double rad3 = y15 + num19;
                            if (!_runDB.set_run.add_mode1)//加重
                            {
                                rad1 += 3.1415926;
                            }
                            if (!_runDB.set_run.add_mode2)
                            {
                                rad2 += 3.1415926;
                            }
                            if (!_runDB.set_run.add_mode0)
                            {
                                rad3 += 3.1415926;
                            }



                            _runDB.bal_result.fl = x10;
                            _runDB.bal_result.ql = angle(rad1);
                            _runDB.bal_result.fr = x2;
                            _runDB.bal_result.qr = angle(rad2);
                            _runDB.bal_result.fm = x11;
                            _runDB.bal_result.qm = angle(rad3);
                            OnEventTriggered?.Invoke(1);
                        }
                        if (_balanceData.ki == _runDB.set_test.ki_max + 1)
                        {
                            _runDB.bal_result.fl_1 = _runDB.bal_result.fl;
                            _runDB.bal_result.ql_1 = _runDB.bal_result.ql;
                            _runDB.bal_result.fr_1 = _runDB.bal_result.fr;
                            _runDB.bal_result.qr_1 = _runDB.bal_result.qr;
                            _runDB.bal_result.fm_1 = _runDB.bal_result.fm;
                            _runDB.bal_result.qm_1 = _runDB.bal_result.qm;
                        }
                        if (_balanceData.ki <= _runDB.set_test.ki_max)
                            return;

                        OnEventTriggered?.Invoke(3);
                        _runDB.Bal_start = false;

                        _balanceData.ki = 0;
                        //_balanceData.start_AVE = 0;

                    }
                }
            }
        }
        private  void filter(
          int aveType,
          int cnt,//当前采样次数
          int cntMax,//最大采样次数
          double fl0,
          double ql0,
          double fr0,
          double qr0)
        {
            if (cnt == 1)
            {
                _balanceData.val[0] = fl0;//当前值
                _balanceData.val[1] = ql0;
                _balanceData.val[2] = fr0;
                _balanceData.val[3] = qr0;
                for (int index = 0; index < 20; ++index)
                {
                    _balanceData.valFl[index] = 0.0;
                    _balanceData.valQl[index] = 0.0;
                    _balanceData.valFr[index] = 0.0;
                    _balanceData.valQr[index] = 0.0;
                }
                _balanceData.valFl[0] = fl0;//历史数据存储数组
                _balanceData.valQl[0] = ql0;
                _balanceData.valFr[0] = fr0;
                _balanceData.valQr[0] = qr0;
            }
            else if (aveType == 0)
            {
                int index = cnt % cntMax - 1;
                if (index == -1)
                    index = cntMax - 1;
                if (cnt > cntMax)
                {
                    _balanceData.val[0] = (_balanceData.val[0] * (double)cntMax - _balanceData.valFl[index]) / (double)cntMax + fl0 / (double)cntMax;
                    _balanceData.val[1] = (_balanceData.val[1] * (double)cntMax - _balanceData.valQl[index]) / (double)cntMax + ql0 / (double)cntMax;
                    _balanceData.val[2] = (_balanceData.val[2] * (double)cntMax - _balanceData.valFr[index]) / (double)cntMax + fr0 / (double)cntMax;
                    _balanceData.val[3] = (_balanceData.val[3] * (double)cntMax - _balanceData.valQr[index]) / (double)cntMax + qr0 / (double)cntMax;
                }
                else
                {
                    _balanceData.val[0] = _balanceData.val[0] * (double)(cnt - 1) / (double)cnt + fl0 / (double)cnt;
                    _balanceData.val[1] = _balanceData.val[1] * (double)(cnt - 1) / (double)cnt + ql0 / (double)cnt;
                    _balanceData.val[2] = _balanceData.val[2] * (double)(cnt - 1) / (double)cnt + fr0 / (double)cnt;
                    _balanceData.val[3] = _balanceData.val[3] * (double)(cnt - 1) / (double)cnt + qr0 / (double)cnt;
                }
                _balanceData.valFl[index] = fl0;
                _balanceData.valQl[index] = ql0;
                _balanceData.valFr[index] = fr0;
                _balanceData.valQr[index] = qr0;
            }
            else
            {
                if (cnt > cntMax)
                {
                    for (int index = 0; index < cntMax; ++index)
                    {
                        _balanceData.valFl[index] = _balanceData.valFl[index + 1];
                        _balanceData.valQl[index] = _balanceData.valQl[index + 1];
                        _balanceData.valFr[index] = _balanceData.valFr[index + 1];
                        _balanceData.valQr[index] = _balanceData.valQr[index + 1];
                    }
                    cnt = cntMax;
                }
                _balanceData.valFl[cnt] = fl0;
                _balanceData.valQl[cnt] = ql0;
                _balanceData.valFr[cnt] = fr0;
                _balanceData.valQr[cnt] = qr0;
                double[] numArray1 = new double[cnt];
                int[] numArray2 = new int[cnt];
                double[] numArray3 = new double[cnt];
                int[] numArray4 = new int[cnt];
                for (int index = 0; index < cnt; ++index)
                {
                    numArray1[index] = Math.Sqrt(_balanceData.valFl[index] * _balanceData.valFl[index] + _balanceData.valQl[index] * _balanceData.valQl[index]);
                    numArray2[index] = index;
                    numArray3[index] = Math.Sqrt(_balanceData.valFr[index] * _balanceData.valFr[index] + _balanceData.valQr[index] * _balanceData.valQr[index]);
                    numArray4[index] = index;
                }
                for (int index1 = 0; index1 < cnt - 1; ++index1)
                {
                    for (int index2 = index1 + 1; index2 < cnt; ++index2)
                    {
                        if (numArray1[index1] > numArray1[index2])
                        {
                            double num1 = numArray1[index1];
                            numArray1[index1] = numArray1[index2];
                            numArray1[index2] = num1;
                            int num2 = numArray2[index1];
                            numArray2[index1] = numArray2[index2];
                            numArray2[index2] = num2;
                        }
                        if (numArray3[index1] > numArray3[index2])
                        {
                            double num3 = numArray3[index1];
                            numArray3[index1] = numArray3[index2];
                            numArray3[index2] = num3;
                            int num4 = numArray4[index1];
                            numArray4[index1] = numArray4[index2];
                            numArray4[index2] = num4;
                        }
                    }
                }
                if (aveType == 2)
                {
                    int index = cnt / 2;
                    if (cnt % 2 == 0)
                    {
                        _balanceData.val[1] = _balanceData.val[1] * (double)(cnt - 1) / (double)cnt + (_balanceData.valFl[numArray2[index - 1]] + _balanceData.valFl[numArray2[index]]) / 2.0 / (double)cnt;
                        _balanceData.val[2] = _balanceData.val[2] * (double)(cnt - 1) / (double)cnt + (_balanceData.valQl[numArray2[index - 1]] + _balanceData.valQl[numArray2[index]]) / 2.0 / (double)cnt;
                        _balanceData.val[3] = _balanceData.val[3] * (double)(cnt - 1) / (double)cnt + (_balanceData.valFr[numArray4[index - 1]] + _balanceData.valFr[numArray4[index]]) / 2.0 / (double)cnt;
                        _balanceData.val[4] = _balanceData.val[4] * (double)(cnt - 1) / (double)cnt + (_balanceData.valQr[numArray4[index - 1]] + _balanceData.valQr[numArray4[index]]) / 2.0 / (double)cnt;
                    }
                    else
                    {
                        _balanceData.val[1] = _balanceData.val[1] * (double)(cnt - 1) / (double)cnt + _balanceData.valFl[numArray2[index]] / (double)cnt;
                        _balanceData.val[2] = _balanceData.val[2] * (double)(cnt - 1) / (double)cnt + _balanceData.valQl[numArray2[index]] / (double)cnt;
                        _balanceData.val[3] = _balanceData.val[3] * (double)(cnt - 1) / (double)cnt + _balanceData.valFr[numArray4[index]] / (double)cnt;
                        _balanceData.val[4] = _balanceData.val[4] * (double)(cnt - 1) / (double)cnt + _balanceData.valQr[numArray4[index]] / (double)cnt;
                    }
                }
                else
                {
                    int index = cnt / 2;
                    if (cnt % 2 == 0)
                    {
                        _balanceData.val[1] = (_balanceData.valFl[numArray2[index - 1]] + _balanceData.valFl[numArray2[index]]) / 2.0;
                        _balanceData.val[2] = (_balanceData.valQl[numArray2[index - 1]] + _balanceData.valQl[numArray2[index]]) / 2.0;
                        _balanceData.val[3] = (_balanceData.valFr[numArray4[index - 1]] + _balanceData.valFr[numArray4[index]]) / 2.0;
                        _balanceData.val[4] = (_balanceData.valQr[numArray4[index - 1]] + _balanceData.valQr[numArray4[index]]) / 2.0;
                    }
                    else
                    {
                        _balanceData.val[1] = _balanceData.valFl[numArray2[index]];
                        _balanceData.val[2] = _balanceData.valQl[numArray2[index]];
                        _balanceData.val[3] = _balanceData.valFr[numArray4[index]];
                        _balanceData.val[4] = _balanceData.valQr[numArray4[index]];
                    }
                }
            }
        }
        /// <summary>
        /// 返回模 弧度
        /// </summary>
        /// <param name="x">模</param>
        /// <param name="y">弧度</param>
        private  void xyzq(ref double x, ref double y)
        {
            double num1 = Math.Sqrt(x * x + y * y);
            double num2 = Math.Abs(x) >= 0.0001 ? Math.Atan(y / x) : 1.5708;
            if (x < 0.0)
                num2 += 3.1415926;
            while (num2 < 0.0)
                num2 += 6.2831852;
            while (num2 > 6.2831852)
                num2 -= 6.2831852;
            x = num1;
            y = num2;
        }
        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        private  double angle(double rad)
        {
            double num = rad * 57.29578;
            while (num >= 360.0)
                num -= 360.0;
            while (num < 0.0)
                num += 360.0;
            return num;
        }
        private  bool BEAR(int bear0, ref double AAA, ref double BBB, ref double CCC)
        {
            bool flag = true;
            switch (bear0)
            {
                case 1:
                    return flag;
                case 2:
                    AAA = -AAA;
                    CCC = -CCC;
                    goto case 1;
                case 3:
                    AAA = -AAA;
                    goto case 1;
                case 4:
                    CCC = -CCC;
                    goto case 1;
                case 5:
                    AAA = -AAA;
                    goto case 1;
                case 6:
                    CCC = -CCC;
                    goto case 1;
                case 7:
                    BBB = 0.0;
                    goto case 1;
                case 8:
                    AAA = -AAA;
                    BBB = 0.0;
                    goto case 1;
                case 9:
                    BBB = 0.0;
                    CCC = -CCC;
                    goto case 1;
                default:
                    flag = false;
                    goto case 1;
            }
        }
        private  bool acgas(double[] ar, double[] ai, ref double[] br0, ref double[] bi0)
        {
            int[] numArray1 = new int[2];
            double[] numArray2 = new double[4];
            double[] numArray3 = new double[4];
            double[] numArray4 = new double[2];
            int num1 = 2;
            int index1 = 0;
            int index2 = 0;
            for (int index3 = 0; index3 <= 3; ++index3)
            {
                numArray2[index3] = ar[index3];
                numArray3[index3] = ai[index3];
            }
            double num2 = 0.0;
            for (int index4 = index1; index4 <= num1 - 1; ++index4)
            {
                for (int index5 = index1; index5 <= num1 - 1; ++index5)
                {
                    int index6 = index4 * num1 + index5;
                    double num3 = numArray2[index6] * numArray2[index6] + numArray3[index6] * numArray3[index6];
                    if (num3 > num2)
                    {
                        num2 = num3;
                        numArray1[index1] = index5;
                        index2 = index4;
                    }
                }
            }
            if (num2 + 1.0 == 1.0)
                return false;
            if (index2 != index1)
            {
                for (int index7 = index1; index7 <= num1 - 1; ++index7)
                {
                    int index8 = index1 * num1 + index7;
                    int index9 = index2 * num1 + index7;
                    double num4 = numArray2[index8];
                    numArray2[index8] = numArray2[index9];
                    numArray2[index9] = num4;
                    double num5 = numArray3[index8];
                    numArray3[index8] = numArray3[index9];
                    numArray3[index9] = num5;
                }
                double num6 = br0[index1];
                br0[index1] = br0[index2];
                br0[index2] = num6;
                double num7 = bi0[index1];
                bi0[index1] = bi0[index2];
                bi0[index2] = num7;
            }
            if (numArray1[index1] != index1)
            {
                for (int index10 = 0; index10 <= num1 - 1; ++index10)
                {
                    int index11 = index10 * num1 + index1;
                    int index12 = index10 * num1 + numArray1[index1];
                    double num8 = numArray2[index11];
                    numArray2[index11] = numArray2[index12];
                    numArray2[index12] = num8;
                    double num9 = numArray3[index11];
                    numArray3[index11] = numArray3[index12];
                    numArray3[index12] = num9;
                }
            }
            int index13 = index1 * num1 + index1;
            for (int index14 = index1 + 1; index14 <= num1 - 1; ++index14)
            {
                int index15 = index1 * num1 + index14;
                double num10 = numArray2[index15] * numArray2[index13];
                double num11 = -numArray3[index15] * numArray3[index13];
                double num12 = (numArray2[index13] - numArray3[index13]) * (numArray2[index15] + numArray3[index15]);
                numArray2[index15] = (num10 - num11) / num2;
                numArray3[index15] = (num12 - num10 - num11) / num2;
            }
            double num13 = br0[index1] * numArray2[index13];
            double num14 = -bi0[index1] * numArray3[index13];
            double num15 = (numArray2[index13] - numArray3[index13]) * (br0[index1] + bi0[index1]);
            br0[index1] = (num13 - num14) / num2;
            bi0[index1] = (num15 - num13 - num14) / num2;
            for (int index16 = index1 + 1; index16 <= num1 - 1; ++index16)
            {
                int index17 = index16 * num1 + index1;
                for (int index18 = index1 + 1; index18 <= num1 - 1; ++index18)
                {
                    int index19 = index1 * num1 + index18;
                    int index20 = index16 * num1 + index18;
                    double num16 = numArray2[index17] * numArray2[index19];
                    double num17 = numArray3[index17] * numArray3[index19];
                    double num18 = (numArray2[index17] + numArray3[index17]) * (numArray2[index19] + numArray3[index19]);
                    numArray2[index20] = numArray2[index20] - num16 + num17;
                    numArray3[index20] = numArray3[index20] - num18 + num16 + num17;
                }
                double num19 = numArray2[index17] * br0[index1];
                double num20 = numArray3[index17] * bi0[index1];
                double num21 = (numArray2[index17] + numArray3[index17]) * (br0[index1] + bi0[index1]);
                br0[index16] = br0[index16] - num19 + num20;
                bi0[index16] = bi0[index16] - num21 + num19 + num20;
            }
            int index21 = (num1 - 1) * num1 + num1 - 1;
            double num22 = numArray2[index21] * numArray2[index21] + numArray3[index21] * numArray3[index21];
            if (num22 + 1.0 == 1.0)
                return false;
            double num23 = numArray2[index21] * br0[num1 - 1];
            double num24 = -numArray3[index21] * bi0[num1 - 1];
            double num25 = (numArray2[index21] - numArray3[index21]) * (br0[num1 - 1] + bi0[num1 - 1]);
            br0[num1 - 1] = (num23 - num24) / num22;
            bi0[num1 - 1] = (num25 - num23 - num24) / num22;
            for (int index22 = 0; index22 >= 0; --index22)
            {
                for (int index23 = index22 + 1; index23 <= num1 - 1; ++index23)
                {
                    int index24 = index22 * num1 + index23;
                    double num26 = numArray2[index24] * br0[index23];
                    double num27 = numArray3[index24] * bi0[index23];
                    double num28 = (numArray2[index24] + numArray3[index24]) * (br0[index23] + bi0[index23]);
                    br0[index22] = br0[index22] - num26 + num27;
                    bi0[index22] = bi0[index22] - num28 + num26 + num27;
                }
            }
            numArray1[num1 - 1] = num1 - 1;
            for (int index25 = num1 - 1; index25 >= 0; --index25)
            {
                if (numArray1[index25] != index25)
                {
                    double num29 = br0[index25];
                    br0[index25] = br0[numArray1[index25]];
                    br0[numArray1[index25]] = num29;
                    double num30 = bi0[index25];
                    bi0[index25] = bi0[numArray1[index25]];
                    bi0[numArray1[index25]] = num30;
                }
            }
            return true;
        }
        public  bool set0_cal()
        {
            bool flag = true;
            try
            {
                double[] numArray1 = new double[4]
                {
        0.001,
        0.01,
        0.02,
        0.039
                };
                double[] numArray2 = new double[4];
                double[] numArray3 = new double[4];
                double[] numArray4 = new double[4];
                double[] numArray5 = new double[4];
                double[] numArray6 = new double[4];
                double[] numArray7 = new double[4];
                for (int index = 0; index < 4; ++index)
                {
                    numArray2[index] = _balanceData.cal_sample[1][index] - _balanceData.cal_sample[0][index];
                    numArray3[index] = _balanceData.cal_sample[2][index] - _balanceData.cal_sample[0][index];
                    _runDB.set0.cal_ai[index] = 0.0;
                    _runDB.set0.cal_ar[index] = 0.0;
                    _runDB.set0.cal_h[index] = 0.0;

                }
                double num1 = _runDB.set_run.set_rpm * (Math.PI / 30.0);
                double num2 = 0.0;
                if (num1 > 0.0 && _balanceData.PCBversion < 11)
                    num2 = Math.Atan(100.0 / 39.0 / num1) - Math.Atan(0.039 * num1) - Math.Atan(numArray1[_runDB.set_test.pre_amp] * num1);
                if (_runDB.hard_run.enable)
                {
                    if (_runDB.set_run.single_side)
                    {
                        double x = numArray2[0] + numArray2[2];
                        double y = numArray2[1] + numArray2[3];
                        xyzq(ref x, ref y);
                        _runDB.set0.cal_h[0] = _runDB.set0.set0_val[0] * num1 * num1 * _runDB.hard_run.r1 / x;
                        _runDB.set0.cal_h[1] = 6.283185 - (y - _runDB.set0.set0_val[1] / 57.29578 + num2);
                        if (_runDB.set0.cal_h[0] < 0.0)
                        {
                            _runDB.set0.cal_h[0] = -_runDB.set0.cal_h[0];
                            _runDB.set0.cal_h[1] = _runDB.set0.cal_h[1] + 3.1415926;
                        }
                        if (_runDB.set0.cal_h[1] >= 6.2831852)
                            _runDB.set0.cal_h[1] = _runDB.set0.cal_h[1] - 6.2831852;
                        if (_runDB.set0.cal_h[1] >= 0.0)
                            return false;
                        _runDB.set0.cal_h[1] = _runDB.set0.cal_h[1] + 6.2831852;
                    }
                    else
                    {
                        double a = _runDB.hard_run.A;
                        double b = _runDB.hard_run.B;
                        double c = _runDB.hard_run.C;
                        if (BEAR(_runDB.hard_run.bear, ref a, ref b, ref c))
                        {
                            if (_runDB.hard_run.bear >= 7 && _runDB.hard_run.bear <= 9)
                            {
                                _runDB.set0.cal_h[0] = _runDB.set0.set0_val[0] * c / (a + c);
                                _runDB.set0.cal_h[2] = _runDB.set0.set0_val[0] * a / (a + c);
                            }
                            else
                            {
                                _runDB.set0.cal_h[0] = _runDB.set0.set0_val[0] * (b + c) / (a + b + c);
                                _runDB.set0.cal_h[2] = _runDB.set0.set0_val[2] * (b + a) / (a + b + c);
                            }
                        }
                        double x1 = numArray2[0];
                        double y1 = numArray2[1];
                        xyzq(ref x1, ref y1);
                        //double num1 = _runDB.set_run.set_rpm * (Math.PI / 30.0);
                        _runDB.set0.cal_h[0] = _runDB.set0.cal_h[0] * num1 * num1 * _runDB.hard_run.r1 / x1;
                        _runDB.set0.cal_h[1] = 6.283185 - (y1 - _runDB.set0.set0_val[1] / 57.29578 + num2);
                        if (_runDB.set0.cal_h[0] < 0.0)
                        {
                            _runDB.set0.cal_h[0] = -_runDB.set0.cal_h[0];
                            _runDB.set0.cal_h[1] = _runDB.set0.cal_h[1] + 3.1415926;
                        }
                        if (_runDB.set0.cal_h[1] >= 6.2831852)
                            _runDB.set0.cal_h[1] = _runDB.set0.cal_h[1] - 6.2831852;
                        if (_runDB.set0.cal_h[1] < 0.0)
                            _runDB.set0.cal_h[1] = _runDB.set0.cal_h[1] + 6.2831852;
                        if (!_runDB.set_run.single_side)
                        {
                            double x2 = numArray3[2];
                            double y2 = numArray3[3];
                            xyzq(ref x2, ref y2);
                            _runDB.set0.cal_h[2] = _runDB.set0.cal_h[2] * num1 * num1 * _runDB.hard_run.r2 / x2;
                            _runDB.set0.cal_h[3] = 6.283185 - (y2 - _runDB.set0.set0_val[3] / 57.29578 + num2);
                            if (_runDB.set0.cal_h[2] < 0.0)
                            {
                                _runDB.set0.cal_h[2] = -_runDB.set0.cal_h[2];
                                _runDB.set0.cal_h[3] = _runDB.set0.cal_h[3] + 3.1415926;
                            }
                            if (_runDB.set0.cal_h[3] >= 6.2831852)
                                _runDB.set0.cal_h[3] = _runDB.set0.cal_h[3] - 6.2831852;
                            if (_runDB.set0.cal_h[3] < 0.0)
                                _runDB.set0.cal_h[3] = _runDB.set0.cal_h[3] + 6.2831852;
                        }
                    }
                }
                else if (_runDB.set_run.single_side)
                {
                    double x = numArray2[0] + numArray2[2];
                    double y = numArray2[1] + numArray2[3];
                    xyzq(ref x, ref y);
                    _runDB.set0.cal_ar[0] = _runDB.set0.set0_val[0] / x;
                    _runDB.set0.cal_ai[0] = 6.283185 - (y - _runDB.set0.set0_val[1] / 57.29578 + num2);
                    if (_runDB.set0.cal_ar[0] < 0.0)
                    {
                        _runDB.set0.cal_ar[0] = -_runDB.set0.cal_ar[0];
                        _runDB.set0.cal_ai[0] = _runDB.set0.cal_ai[0] + 3.1415926;
                    }
                    if (_runDB.set0.cal_ai[0] >= 6.283185)
                        _runDB.set0.cal_ai[0] = _runDB.set0.cal_ai[0] - 6.283185;
                    if (_runDB.set0.cal_ai[0] < 0.0)
                        _runDB.set0.cal_ai[0] = _runDB.set0.cal_ai[0] + 6.283185;
                }
                else
                {
                    numArray4[0] = _runDB.set0.set0_val[0] * Math.Cos(_runDB.set0.set0_val[1] / 57.29578);
                    numArray4[1] = _runDB.set0.set0_val[2] * Math.Cos(_runDB.set0.set0_val[3] / 57.29578);
                    numArray4[2] = _runDB.set0.set0_val[0] * Math.Cos(_runDB.set0.set0_val[1] / 57.29578);
                    numArray4[3] = _runDB.set0.set0_val[2] * Math.Cos(_runDB.set0.set0_val[3] / 57.29578);
                    numArray5[0] = _runDB.set0.set0_val[0] * Math.Sin(_runDB.set0.set0_val[1] / 57.29578);
                    numArray5[1] = _runDB.set0.set0_val[2] * Math.Sin(_runDB.set0.set0_val[3] / 57.29578);
                    numArray5[2] = _runDB.set0.set0_val[0] * Math.Sin(_runDB.set0.set0_val[1] / 57.29578);
                    numArray5[3] = _runDB.set0.set0_val[2] * Math.Sin(_runDB.set0.set0_val[3] / 57.29578);
                    numArray6[0] = numArray2[0] / 1000.0;
                    numArray6[1] = numArray3[0] / 1000.0;
                    numArray6[2] = numArray2[2] / 1000.0;
                    numArray6[3] = numArray3[2] / 1000.0;
                    numArray7[0] = numArray2[1] / 1000.0;
                    numArray7[1] = numArray3[1] / 1000.0;
                    numArray7[2] = numArray2[3] / 1000.0;
                    numArray7[3] = numArray3[3] / 1000.0;
                    for (int index = 0; index < 4; ++index)
                    {
                        double num3 = numArray6[index] * numArray4[index];
                        double num4 = -numArray7[index] * numArray5[index];
                        double num5 = (numArray6[index] + numArray7[index]) * (numArray4[index] - numArray5[index]);
                        double num6 = numArray4[index] * numArray4[index] + numArray5[index] * numArray5[index];
                        double x;
                        double y;
                        if (num6 == 0.0)
                        {
                            x = 1E+35 * numArray6[index] / Math.Abs(numArray6[index]);
                            y = 1E+35 * numArray7[index] / Math.Abs(numArray7[index]);
                        }
                        else
                        {
                            x = (num3 - num4) / num6;
                            y = (num5 - num3 - num4) / num6;
                        }
                        xyzq(ref x, ref y);
                        double num7 = y + num2;
                        _runDB.set0.cal_ar[index] = x * Math.Cos(num7) * 100.0;
                        _runDB.set0.cal_ai[index] = x * Math.Sin(num7) * 100.0;
                    }
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        private  byte[] Decrypt(byte[] encryptedData, byte[] key, byte[] iv)
        {
            using (DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider())
            {
                desProvider.Key = key;
                desProvider.IV = iv;

                using (ICryptoTransform decryptor = desProvider.CreateDecryptor())
                {
                    using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (MemoryStream decryptedMemoryStream = new MemoryStream())
                            {
                                int data;
                                while ((data = cryptoStream.ReadByte()) != -1)
                                {
                                    decryptedMemoryStream.WriteByte((byte)data);
                                }
                                return decryptedMemoryStream.ToArray();
                            }
                        }
                    }
                }
            }
        }
        public  void Decrypt()
        {
            try
            {
                byte[] key = { 0x23, 0x11, 0x23, 0x11, 0x23, 0x11, 0x2a, 0x16 }; // 密钥数据
                byte[] iv = { 0x23, 0x11, 0x23, 0x11, 0x23, 0x11, 0x3a, 0x16 }; // 初始化向量数据

                string[] arrSplit =_balanceData.License.Split('-');
                byte[] byteTemp = new byte[arrSplit.Length];
                for (int i = 0; i < byteTemp.Length; i++)
                {
                    byteTemp[i] = byte.Parse(arrSplit[i], System.Globalization.NumberStyles.AllowHexSpecifier);

                }
                byte[] decryptedData = Decrypt(byteTemp, key, iv);
                string mw = System.Text.Encoding.UTF8.GetString(decryptedData);

                DateTime dqr = DateTime.ParseExact(mw.Substring(17, 8), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

                if (DateTime.Now <= dqr && _balanceData.DeviceNo == mw.Substring(0, 17))
                {
                    _balanceData.remainDays = (dqr - DateTime.Now).Days;
                    _balanceData.pass = true;
                }
                else
                {

                }
            }
            catch
            {
                _balanceData.remainDays = 0;
                _balanceData.pass = false;
            }
        }
    }

}
