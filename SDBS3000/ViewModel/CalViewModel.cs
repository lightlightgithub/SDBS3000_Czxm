using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SDBS3000.Views;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using static SDBS3000.Log.Log;
using System.Linq;
using System.Collections.ObjectModel;
using SDBS3000.Resources;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using NPOI.POIFS.FileSystem;
using SDBS3000.Control;

namespace SDBS3000.ViewModel
{
    public class CalViewModel : ViewModelBase
    {
        System.Timers.Timer time = new System.Timers.Timer();
        SerEventHandler serEvent;

        private Visibility jPH;

        public Visibility JPH
        {
            get { return jPH; }
            set { jPH = value; }
        }

        private bool ischecked;

        public bool Ischecked
        {
            get { return ischecked; }
            set { ischecked = value; }
        }

        public CalViewModel()
        {
            time.Interval = 500;
            time.AutoReset = true;

            JPH = GlobalVar.RotorStruct1.clms == 1 ? Visibility.Collapsed : Visibility.Visible;

            pmyenable = true;
            pmeenable = true;
            noweightenable = true;
            MainViewModel.bal.OnEventTriggered += Bal_OnEventTriggered;
            Set0_val = MainViewModel.bal._runDB.set0.set0_val;
            RaisePropertyChanged("Set0_val[0]");
            RaisePropertyChanged("Set0_val[1]");
            RaisePropertyChanged("Set0_val[2]");
            RaisePropertyChanged("Set0_val[3]");

        }

        private void Bal_OnEventTriggered(int message = 0)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (GlobalVar.mainWindow.frame.Source.ToString().Contains("PageCal"))
                {
                    if (MainViewModel.bal._runDB.set_runmode == 0)
                        return;

                    SingleL = MainViewModel.bal._runDB.bal_result.singleL;
                    SingleR = MainViewModel.bal._runDB.bal_result.singleR;
                    Data1 = MainViewModel.bal._runDB.bal_result.fl;
                    Data2 = MainViewModel.bal._runDB.bal_result.ql;
                    Data3 = MainViewModel.bal._runDB.bal_result.fr;
                    Data4 = MainViewModel.bal._runDB.bal_result.qr;

                    Progress = 20 * MainViewModel.bal.ki / MainViewModel.bal._runDB.set_test.ki_max;
                    RaisePropertyChanged("SingleL");
                    RaisePropertyChanged("SingleR");
                    RaisePropertyChanged("Progress");
                    RaisePropertyChanged("Data1");
                    RaisePropertyChanged("Data2");
                    RaisePropertyChanged("Data3");
                    RaisePropertyChanged("Data4");

                    Messenger.Default.Send(MainViewModel.bal._runDB.bal_result.rpm.ToString(), "speed");
                }

            }));
        }

        private float calSpeed;

        public float CalSpeed
        {
            get { return calSpeed; }
            set { calSpeed = value; }
        }

        public double Data1 { get; set; }
        public double Data2 { get; set; }
        public double Data3 { get; set; }
        public double Data4 { get; set; }
        public int SingleL { get; set; }
        public int SingleR { get; set; }
        public int Progress { get; set; }//   progress比20  等于 读出来的比测量次数  

        private ObservableCollection<T_Caldata> calData;

        public ObservableCollection<T_Caldata> CalData
        {
            get { return calData; }
            set { calData = value; }
        }



        public ICommand CalBegin
        {
            get => new RelayCommand<object>(
                async obj => // 注意这里要加 async
                {
                    GlobalVar.main.Compesation = false;
                    MainViewModel.bal._runDB.set_runmode = Convert.ToUInt16(obj);

                    try
                    {
                        var (success, code) = await MainViewModel.macControl.ServoStartAsync(
                            MainViewModel.bal._runDB.set_run.drive_mode,
                            (ushort)MainViewModel.bal._runDB.set_run.set_rpm);

                        string resultStr;
                        if (!success)
                        {
                            switch (code)
                            {
                                case 0x02:
                                    resultStr = "气缸异常（标定开始）";
                                    break;
                                case 0x03:
                                    resultStr = "伺服异常（标定开始）";
                                    break;
                                case 0x04:
                                    resultStr = "伺服报警（标定开始）";
                                    break;
                                case 0x05:
                                    resultStr = "伺服忙（标定开始）";
                                    break;
                                case 0x06:
                                    resultStr = "伺服未启动（标定开始）";
                                    break;
                                case 0xFF:
                                    resultStr = "等待响应超时（标定开始）";
                                    break;
                                case 0xFE:
                                    resultStr = "发送命令失败（标定开始）";
                                    break;
                                default:
                                    resultStr = $"未知错误 (Code: {code:X2})（标定开始）";
                                    break;
                            }
                            GlobalVar.Str = resultStr;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("标定开始命令执行异常: " + ex.Message);
                    }
                });
        }

        public ICommand CalStop
        {
              get => new RelayCommand<object>(
                async  obj =>
                {
                    int bz = Convert.ToUInt16(obj);
                    if (bz == 1)
                        noweightenable = false;
                    else if (bz == 2)
                        pmyenable = false;
                    else if (bz == 3)
                        pmeenable = false;
                    RaisePropertyChanged("pmyenable");
                    RaisePropertyChanged("pmeenable");
                    RaisePropertyChanged("noweightenable");

                    try
                    {
                        var (success, code) = await MainViewModel.macControl.ServoStopAsync(MainViewModel.bal._runDB.set_run.drive_mode, 1,  0); 
                        string resultStr;
                        if (!success)
                        {
                            switch (code)
                            {
                                case 0x02:
                                    resultStr = "气缸异常（标定停止）";
                                    break;
                                case 0x03:
                                    resultStr = "伺服异常（标定停止）";
                                    break;
                                case 0x04:
                                    resultStr = "伺服报警（标定停止）";
                                    break;
                                case 0x05:
                                    resultStr = "伺服忙（标定停止）";
                                    break;
                                case 0x06:
                                    resultStr = "伺服未启动（标定停止）";
                                    break;
                                case 0xFF:
                                    resultStr = "等待响应超时（标定停止）";
                                    break;
                                default:
                                    resultStr = $"未知错误 (Code: {code:X2})（标定停止）";
                                    break;
                            }
                            GlobalVar.Str = resultStr;
                        }                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("标定停止命令执行异常: " + ex.Message);
                    }
                  
                });
        }

        public ICommand SetCalSpeed
        {
            get => new RelayCommand<object>(
                obj =>
                {
                });
        }

        public ICommand Clear
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    pmyenable = true;
                    pmeenable = true;
                    noweightenable = true;
                    RaisePropertyChanged("pmyenable");
                    RaisePropertyChanged("pmeenable");
                    RaisePropertyChanged("noweightenable");
                });
        }

        public ICommand Del
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    CodeFirstDbContext Entity = new CodeFirstDbContext();
                    T_Caldata caldata = obj as T_Caldata;
                    try
                    {
                        CalData.Remove(caldata);
                        Entity.T_Caldatas.Attach(caldata);
                        Entity.T_Caldatas.Remove(caldata);
                        Entity.SaveChanges();
                    }
                    catch
                    { }
                    finally
                    {
                        Entity.Dispose();
                    }
                });
        }

        //1和3是角度
        public double[] set0_val;

        public double[] Set0_val
        {
            get
            {
                return MainViewModel.bal._runDB.set0.set0_val;
            }
            set
            {
                Set(ref set0_val, value); RaisePropertyChanged("Set0_val");
            }
        }

        /// <summary>
        /// 解算
        /// </summary>
        public ICommand Cal => new RelayCommand<object>(
                obj =>
                {
                    MainViewModel.bal._runDB.set0.set0_val = Set0_val;
                    if (MainViewModel.bal.set0_cal())
                    {
                        CodeFirstDbContext Entity = new CodeFirstDbContext();
                        T_Caldata T_Caldata = new T_Caldata
                        {
                            speed = MainViewModel.bal._runDB.set_run.set_rpm,
                            rotorid = GlobalVar.main.Rotor.ID,
                            v0 = Set0_val[0],
                            v1 = Set0_val[1],
                            v2 = Set0_val[2],
                            v3 = Set0_val[3],

                            h0 = MainViewModel.bal._runDB.set0.cal_h[0],
                            h1 = MainViewModel.bal._runDB.set0.cal_h[0],
                            h2 = MainViewModel.bal._runDB.set0.cal_h[0],
                            h3 = MainViewModel.bal._runDB.set0.cal_h[0],
                            ar0 = MainViewModel.bal._runDB.set0.cal_ar[0],
                            ar1 = MainViewModel.bal._runDB.set0.cal_ar[1],
                            ar2 = MainViewModel.bal._runDB.set0.cal_ar[2],
                            ar3 = MainViewModel.bal._runDB.set0.cal_ar[3],
                            ai0 = MainViewModel.bal._runDB.set0.cal_ai[0],
                            ai1 = MainViewModel.bal._runDB.set0.cal_ai[1],
                            ai2 = MainViewModel.bal._runDB.set0.cal_ai[2],
                            ai3 = MainViewModel.bal._runDB.set0.cal_ai[3]
                        };
                        Entity.T_Caldatas.Add(T_Caldata);
                        Entity.SaveChanges();
                        NewMessageBox.Show("解算成功");
                        Entity.Dispose();
                    }
                    else
                    {
                        NewMessageBox.Show("标定失败");
                    }

                });

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public bool pmyenable { get; set; }
        public bool pmeenable { get; set; }
        public bool noweightenable { get; set; }
    }

    public struct Measure0
    {
        public float lasttime_data1;
        public float lasttime_data2;
        public float lasttime_data3;
        public float lasttime_data4;
        public float lasttime_jdata1;
        public float lasttime_jdata2;

        public float rpm;
        public float data1;
        public float data2;
        public float data3;
        public float data4;
        public float jdata1;
        public float jdata2;
        public ushort pmy;
        public ushort pme;
        public ushort j;
        public ushort pro;
        /// <summary>
        /// 合格不合格标志
        /// </summary>
        public ushort isok;
        /// <summary>
        /// 运行状态
        /// </summary>
        public ushort state;
    }

    public struct Set0_val
    {
        public float val0;
        public float val1;
        public float val2;
        public float val3;
    }
}
