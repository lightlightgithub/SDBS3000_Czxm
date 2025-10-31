using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity.Model
{
    public class BalanceData
    {
        public int _recLen;
        public string _recData;
        public string _sendData;
        public byte[] _recBytes;
        public byte[] _sendBytes;
        /// <summary>
        /// 量值 弧度 量值  弧度
        /// </summary>
        public double[] val;
        public double[] valFl;
        public double[] valQl;
        public double[] valFr;
        public double[] valQr;
        public double[] val_AVE;
        public int start_AVE;//测量状态标志
        public int clamp_times;
        public bool set0_key_up;
        public bool set0_clamp_up;
        /// <summary>
        /// 测量次数
        /// </summary>
        public int ki = 0;
        /// <summary>
        /// 0标定空转  1标定左加量  2标定右加量
        /// </summary>
        public double[][] cal_sample;//定标采样
        public double[] bal_sample;

        public int PCBversion = 9;
        public bool pass;
        public string DeviceNo;
        public string License = "";
        public int remainDays;
        public readonly object _lock = new object();
        // 用于记录上一次的运行模式
        public int _lastRunMode = -1;
    }
}
