using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity.Model
{
    public class RUN_DB
    {
        public bool bal_start;//平衡测试启动
        public RUN_DB.TestResult bal_result;//平衡测试结果
        public RUN_DB.Hard_Mode hard_run;//硬支撑模式参数
        public RUN_DB.Set_test set_test;//测试参数设置
        public RUN_DB.Set_run set_run;//运行参数设置
        public RUN_DB.Set0 set0;//标定参数
        public int set_runmode;//运行模式
        public RUN_DB.Clamp_Key set_clamp;//夹具补偿参数
        public RUN_DB.Clamp_Key set_key;
        public double[] cps_val;//空测数值
        public delegate void StartEventHandler(int message = 0);
        public event StartEventHandler OnStartEventTriggered;
        public bool Bal_start
        {
            get { return bal_start; }
            set
            {
                if (!bal_start & value)
                {
                    OnStartEventTriggered(0);
                }
                bal_start = value;
            }
        }

        public RUN_DB()
        {
            this.hard_run.enable = false;
            this.hard_run.bear = 1;
            this.hard_run.A = 60.0;
            this.hard_run.B = 1000.0;
            this.hard_run.C = 60.0;
            this.hard_run.r1 = 60.0;
            this.hard_run.r2 = 60.0;
            this.set_test.ki_max = 6;
            this.set_test.pre_amp = 1;
            this.set_test.aft_amp = 0;
            this.set_test.Rev_difference = 2.0;
            this.set_test.refalsh = 20.0;
            this.set_run.set_rpm = 2000.0;
            this.set_run.set_rpm = 2000.0;
            this.set_run.drive_mode = 1;
            this.set_test.WorkMode = 1;
            this.set_run.m_rotor = 35;
            this.set_run.ccdw = 0;
            this.set_run.Yxldw = 0;

            this.set_test.delayMea = 1;
            this.set_run.add_mode1 = true;
            this.set_run.add_mode2 = true;
            this.set_run.add_mode0 = true;
            this.set_run.compesation = false;
            this.set0.set0_val = new double[4];
            this.set0.cal_h = new double[4];
            this.set0.cal_ai = new double[4];
            this.set0.cal_ar = new double[4];
            this.set_runmode = 0;
            this.cps_val = new double[4];
            this.set_clamp.compesation = false;
            this.set_clamp.cps_val = new double[4];
            this.set_clamp.test_times = 2;
            this.set_clamp.start_test = false;
            this.set_key.compesation = false;
            this.set_key.cps_val = new double[4];
            this.set_key.test_times = 0;
            this.set_key.start_test = false;
        }

        public struct TestResult
        {
            public double fl;
            public double ql;
            public double fr;
            public double qr;
            public double fm;
            public double qm;
            public int rpm;
            public int singleL;
            public int singleR;
            public double fl_1;//存储上次数据
            public double ql_1;
            public double fr_1;
            public double qr_1;
            public double fm_1;
            public double qm_1;
        }

        public struct Hard_Mode
        {
            public int zzid;
            /// <summary>
            /// 硬支撑
            /// </summary>
            public bool enable;
            /// <summary>
            /// 1-6双面 7-9单面静平衡
            /// </summary>
            public int bear;

            public double A;
            public double B;
            public double C;
            public double r1;
            public double r2;
        }

        public struct Set_test
        {
            public int ki_max;//测量次数
            public int pre_amp;//前置放大0,1,2,3
            public int aft_amp;//后置放大0-7
            /// <summary>
            /// 死区滤波
            /// </summary>
            public int delayMea;
            public double refalsh;//刷新频率
            public double Rev_difference;//允许转差
            public double WorkMode;
            public double PosMode;

            public double Needle;
            public double SafeDoor;

            public double Decimalnum;
            public double Direction;
            public double Algorithm;
            public double DriverMode;
            public double DataSaveMode;
            public double PrintMode;
            public double MultSpeedCal;

        }

        public struct Set_run
        {
            /// <summary>
            /// 电补偿
            /// </summary>
            public bool compesation;
            /// <summary>
            /// true  去重
            /// </summary>
            public bool add_mode1;//平面1去重
            public bool add_mode2;//平面2去重
            public bool add_mode0;//静平面去重
            /// <summary>
            /// 是否单面测量
            /// </summary>
            public bool single_side;
            public double set_rpm;//设置转速
            public int drive_mode;//驱动模式
            public int work_mode;//工作方式
            public double m_rotor;//转子质量kg
            public double ccdw;
            public double Yxldw;
        }

        public struct Set0
        {
            /// <summary>
            /// 标定左感量 标定左角度 标定右量值 右角度
            /// </summary>
            public double[] set0_val;//设置感量
            public double[] cal_h;//硬支撑定标值
            public double[] cal_ar;
            public double[] cal_ai;
        }

        //夹具结构体
        public struct Clamp_Key
        {
            public bool start_test;//检测夹具补偿
            public bool compesation;//夹具补偿模式启动结束后清零
            public int test_times;//设置夹具翻转次数
            public double[] cps_val;//补偿值
        }
    }
}
