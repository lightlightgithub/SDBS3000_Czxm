using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SDBS3000.Resources;
using SDBS3000.Views;
using SDBSEntity;
using SDBSEntity.Model;

namespace SDBS3000.ViewModel
{
    public class MeasureViewModel : ViewModelBase
    {
        //标定开始标志，程序启动只运行一次
        bool balstartflag = true;
        TestBegin testBegin;
        bool isrecord = true;
        bool ismea = false;
        string[] ccdwdic = { "mm", "cm", "m", "inch", "foot" };
        string[] yxldwdic = { "g", "mg", "g.mm", "g.cm" };

        //438运行状态 0停止1正在测量2测量完成3测量定位完成
        //string[] state = { "停止", "正在测量", "测量完成", "测量定位完成" };
        string[] state = { LanguageManager.Instance["Stop"], LanguageManager.Instance["Measuringing"], LanguageManager.Instance["MeasuringCompleted"], LanguageManager.Instance["MeasuringPC"] };

        //string[] workstate = { "连续普通测量", "普通测量停止", "测量定位" };
        string[] workstate = { LanguageManager.Instance["workmode0"], LanguageManager.Instance["workmode1"], LanguageManager.Instance["workmode2"] };

        System.Timers.Timer timer = new System.Timers.Timer { Interval = 500 };
        System.Timers.Timer timeSF = new System.Timers.Timer { Interval = 30 };
        int flag;
        /// <summary>
        /// 主要负责初始化一些属性和注册消息中心的监听器以响应特定的消息。
        /// </summary>
        public MeasureViewModel()
        {
            GlobalVar.config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            flag = 1;
            if (DesignerProperties.GetIsInDesignMode(GlobalVar.mainWindow))
            {
                return;
            }
            
            MainViewModel.bal._runDB.OnStartEventTriggered += Run_OnChartEventTriggered;
            MainViewModel.bal.OnEventTriggered += Bal_OnEventTriggered;
            H = SystemParameters.FullPrimaryScreenWidth / 2;
            Messenger.Default.Register<ushort>(this, "decnum", arg =>
            {
                Decnum = "F" + arg.ToString(); RaisePropertyChanged("Decnum");
            });
            Decnum = "F" + MainViewModel.bal._runDB.set_test.Decimalnum.ToString();

          

            Messenger.Default.Register<bool>(this, "jbc", arg =>
            {
                Jbc = arg;
                RaisePropertyChanged("Jbc");
            });
            Messenger.Default.Register<bool>(this, "start", arg =>
            {
                ismea = arg;
            });

            Messenger.Default.Register<ushort>(this, "direction", arg =>
            {
                Direction = arg;
                RaisePropertyChanged("Direction");
            });
            Messenger.Default.Register<bool>(this, "Cut0", arg =>
            {
                Cut0 = arg;
            });
            Messenger.Default.Register<bool>(this, "Cut1", arg =>
            {
                Cut1 = arg;
            });
            Messenger.Default.Register<bool>(this, "Cut2", arg =>
            {
                Cut2 = arg;
            });
            WorkMode = MainViewModel.bal._runDB.set_test.WorkMode;
            Messenger.Default.Register<double>(this, "workmode", arg =>
            {
                WorkMode = arg;
            });
            Rotor = GlobalVar.main.Rotor;
            Data0 = new T_MeasureData();
            Cut0 = MainViewModel.bal._runDB.set_run.add_mode0;
            Cut1 = MainViewModel.bal._runDB.set_run.add_mode1;
            Cut2 = MainViewModel.bal._runDB.set_run.add_mode2;
            Data1 = new T_MeasureData();
            TotalNum = 0;
            OKNum = 0;
            NGNum = 0;
            Nee = 400;//光针角度大于360则隐藏
            RaisePropertyChanged("Nee");
            System.Timers.Timer time = new System.Timers.Timer { Interval = 1000 };
            time.Elapsed += Time_Elapsed;
            time.Start();
            //timeSF.Elapsed += TimeSF_Elapsed;
            //timeSF.Start();

        }

        private void Run_OnChartEventTriggered(int message)
        {
            if (message == 0)
            {
                Data1.fl = Data0.fl;
                Data1.fr = Data0.fr;
                Data1.ql = Data0.ql;
                Data1.qr = Data0.qr;
                Data1.fm = Data0.fm;
                Data1.qm = Data0.qm;
                RunState = 3;
                Times = 0;
                RaisePropertyChanged("Data1");
                if (MainViewModel.bal._runDB.set_runmode <= 0)
                {
                    dtbegin = DateTime.Now;
                }
            }
        }

        private void Time_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!MainViewModel.bal.IsConnected)
            {
                MainViewModel.bal.Connect(GlobalVar.portcjb);
            }
            CardIsonline = MainViewModel.bal.IsConnected;

            if (!MainViewModel.hc.IsConnected)
            {
                MainViewModel.hc.Connect(GlobalVar.portkzb);
            }
            ConCardIsonline = MainViewModel.hc.IsConnected;
            
        }

        System.DateTime dtbegin;
        private void MainWindow_testBegin()
        {
            Data1.fl = Data0.fl;
            Data1.fr = Data0.fr;
            Data1.ql = Data0.ql;
            Data1.qr = Data0.qr;
            Data1.fm = Data0.fm;
            Data1.qm = Data0.qm;
            RunState = 3;
            Times = 0;
            RaisePropertyChanged("Data1");
            dtbegin = DateTime.Now;
            MainViewModel.bal.start_AVE = 0;
        }
        

        private void Bal_OnEventTriggered(int message = 0)
        {

            Application.Current.Dispatcher.Invoke(async () =>
            {
                RaisePropertyChanged("RunState");
                if (message == 3 || (GlobalVar.mainWindow.frame.Source.ToString().Contains("PageHor") || GlobalVar.mainWindow.frame.Source.ToString().Contains("PageVer") || GlobalVar.mainWindow.frame.Source.ToString().Contains("PageOne")))
                {
                    Messenger.Default.Send(MainViewModel.bal._runDB.bal_result.rpm.ToString(), "speed");
                    if (MainViewModel.bal._runDB.bal_start && MainViewModel.bal._runDB.set_runmode <= 0)
                        Times = (DateTime.Now - dtbegin).Seconds;
                    RaisePropertyChanged("Times");
                    if (message == 3 || message == 1)
                    {
                        if (MainViewModel.bal._runDB.set_runmode != 0)
                            return;
                        int xs = 1;
                        if (Rotor.Yxldw == 1)
                            xs = 1000;
                        Data0.singleL = MainViewModel.bal._runDB.bal_result.singleL;
                        Data0.singleR = MainViewModel.bal._runDB.bal_result.singleR;
                        Data0.fl = Math.Round(xs * MainViewModel.bal._runDB.bal_result.fl, 3);
                        Data0.ql = Math.Round(MainViewModel.bal._runDB.bal_result.ql, 3);
                        Data0.fr = Math.Round(xs * MainViewModel.bal._runDB.bal_result.fr, 3);
                        Data0.qr = Math.Round(MainViewModel.bal._runDB.bal_result.qr, 3);
                        Data0.fm = Math.Round(xs * MainViewModel.bal._runDB.bal_result.fm, 3);
                        Data0.qm = Math.Round(MainViewModel.bal._runDB.bal_result.qm, 3);

                        Progress = 20 * MainViewModel.bal.ki / MainViewModel.bal._runDB.set_test.ki_max;
                        Jjbc = MainViewModel.bal._runDB.set_clamp.compesation;
                        RunState = 1;
                        RaisePropertyChanged("Progress");
                        RaisePropertyChanged("Data0");

                        RaisePropertyChanged("Jjbc");
                    }

                    if (message == 3 && !(GlobalVar.mainWindow.frame.Source.ToString().Contains("PagePos")) && !(GlobalVar.mainWindow.frame.Source.ToString().Contains("PageClamp")))
                    {

                        Times = (DateTime.Now - dtbegin).Seconds;
                        if (Rotor.Clms == 1)
                            OK = Data0.fm <= Rotor.Jyxl;
                        else if (Rotor.Clms == 0 || Rotor.Clms == 3)
                            OK = Data0.fl <= Rotor.Pmyyxl && Data0.fr <= Rotor.Pmeyxl;
                        else
                            OK = Data0.fm <= Rotor.Jyxl && Data0.fl <= Rotor.Pmyyxl && Data0.fr <= Rotor.Pmeyxl;
                        NG = !OK;
                        if (OK)
                            OKNum++;
                        else
                            NGNum++;
                        TotalNum++;
                        RunState = 0;
                        Data0.RotorID = Rotor.ID;
                        Data0.UserID = GlobalVar.user.ID;
                        Data0.NAME = Rotor.NAME;
                        Data0.Zcfs = Rotor.Zcfs;
                        Data0.Pmeyxl = Rotor.Pmeyxl;
                        Data0.Pmyyxl = Rotor.Pmyyxl;
                        Data0.Jyxl = Rotor.Jyxl;
                        Data0.A = Rotor.A;
                        Data0.B = Rotor.B;
                        Data0.C = Rotor.C;
                        Data0.R1 = Rotor.R1;
                        Data0.R2 = Rotor.R2;
                        Data0.Speed = Rotor.Speed;
                        Data0.Jsms = Rotor.Jsms.ToString();//string[] jsmsdic = { "硬支撑", "软支撑" };
                        Data0.Clms = Rotor.Clms.ToString();//string[] clmsdic = { "双面动平衡", "静平衡", "动静平衡", "立式双面动平衡", "立式动静平衡" };
                        Data0.Ccdw = ccdwdic[Rotor.Ccdw];
                        Data0.Yxldw = yxldwdic[Rotor.Yxldw];

                        Data0.Duringtime = Times;
                        Data0.MODIFYTIME = System.DateTime.Now;
                        if (MainViewModel.bal._runDB.set_test.DataSaveMode == 1 ||
                            (MainViewModel.bal._runDB.set_test.DataSaveMode == 2 && OK) ||
                            (MainViewModel.bal._runDB.set_test.DataSaveMode == 3 && !OK))
                        {
                            CodeFirstDbContext Entity = new CodeFirstDbContext();
                            Entity.T_MeasureData.Add(Data0);
                            int i = Entity.SaveChanges();
                            Entity.Dispose();
                        }
                        Thread.Sleep(500);
                        Progress = 0;
                        Data0.singleL = 0;
                        Data0.singleR = 0;

                        RaisePropertyChanged("Progress");
                        RaisePropertyChanged("Data0");
                        //if (MainViewModel.bal._runDB.set_test.WorkMode == 2)
                        //{
                        //    int angel = 0;
                        //    if (MainViewModel.bal._runDB.set_test.PosMode == 2)
                        //        angel = (int)MainViewModel.bal._runDB.bal_result.qr * 100;
                        //    else if (MainViewModel.bal._runDB.set_test.PosMode == 3)
                        //        angel = 0;
                        //    else
                        //        angel = (int)MainViewModel.bal._runDB.bal_result.ql * 100;
                        //    Task tstop = Task.Run(() =>
                        //    {

                        //        MacControl.PosStop(angel);
                        //    });
                        //    //tasks.Add(tstop);

                        //    //MacControl.Stop(MainViewModel.bal._runDB.set_run.drive_mode);
                        //}
                        //else
                        //{
                        //    MacControl.Stop(MainViewModel.bal._runDB.set_run.drive_mode);
                        //}
                        int angle = 0;
                        if (MainViewModel.bal._runDB.set_test.PosMode == 2)
                            angle = (int)MainViewModel.bal._runDB.bal_result.qr;
                        else if (MainViewModel.bal._runDB.set_test.PosMode == 3)
                            angle = 0;
                        else
                            angle = (int)MainViewModel.bal._runDB.bal_result.ql;

                        try
                        {
                            var (success, code) = await MainViewModel.macControl.ServoStopAsync(MainViewModel.bal._runDB.set_run.drive_mode, MainViewModel.bal._runDB.set_test.WorkMode, (ushort)angle);
                            string resultStr;
                            if (!success)
                            {
                                switch (code)
                                {
                                    case 0x02:
                                        resultStr = "气缸异常（测量停止）";
                                        break;
                                    case 0x03:
                                        resultStr = "伺服异常（测量停止）";
                                        break;
                                    case 0x04:
                                        resultStr = "伺服报警（测量停止）";
                                        break;
                                    case 0x05:
                                        resultStr = "伺服忙（测量停止）";
                                        break;
                                    case 0x06:
                                        resultStr = "伺服未启动（测量停止）";
                                        break;
                                    case 0xFF:
                                        resultStr = "等待响应超时（测量停止）";
                                        break;
                                    default:
                                        resultStr = $"未知错误 (Code: {code:X2})（测量停止）";
                                        break;
                                }
                                GlobalVar.Str = resultStr;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("测量停止命令执行异常: " + ex.Message);
                        }                       
                    }

                    if (message == 7)
                    {
                        RunState = 4;
                    }
                    if (message == 6 && MainViewModel.bal._runDB.bal_start && MainViewModel.bal._runDB.set_runmode <= 0)//非标定
                    {
                        RunState = 3;
                        Times = 0;
                        Times = (DateTime.Now - dtbegin).Seconds;
                        RaisePropertyChanged("Times");
                    }
                    if (MainViewModel.bal._runDB.bal_result.rpm == 0)
                    {
                        RunState = 0;
                        Progress = 0;
                        Data0.singleL = 0;
                        Data0.singleR = 0;

                        RaisePropertyChanged("Progress");
                        RaisePropertyChanged("Data0");
                    }


                }
            });

        }

        private T_RotorSet rotor;
        public T_RotorSet Rotor
        {
            get { return rotor; }
            set
            {
                Set(ref rotor, value);
                if (value is null)
                    return;
            }
        }

        public string RotorInformation { get; set; }

        public double workMode;

        public double WorkMode
        {
            get { return workMode; }
            set
            {
                Set(ref workMode, value);
            }
        }

        private T_MeasureData data0;

        public T_MeasureData Data0
        {
            get { return data0; }
            set
            {
                Set(ref data0, value);
            }
        }

        private T_MeasureData data1;

        public T_MeasureData Data1
        {
            get { return data1; }
            set
            {
                Set(ref data1, value);
            }
        }

        private int progress;

        public int Progress
        {
            get { return progress; }
            set { Set(ref progress, value); }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Messenger.Default.Unregister(this);
        }

        private string title1 = "测试字体间距zjis";
        public string Title1
        {
            get { return title1; }
            set
            {
                Set(ref title1, value);
            }
        }

        private double h;

        public double H
        {
            get { return h; }
            set { h = value; }
        }

        private float nee;

        public float Nee
        {
            get { return nee; }
            set { nee = value; }
        }

        private ushort direction;
        public ushort Direction
        {
            get { return direction; }
            set { Set(ref direction, value); }
        }

        private bool isExp;
        public bool IsExp
        {
            get { return isExp; }
            set
            {
                Set(ref isExp, value);
            }
        }

        private ObservableCollection<JsData> jsDatas;
        public ObservableCollection<JsData> JsDatas
        {
            get { return jsDatas; }
            set { Set(ref jsDatas, value); }
        }

        private ObservableCollection<DivisionItem> jsDivisionDatas;
        public ObservableCollection<DivisionItem> JsDivisionDatas
        {
            get { return jsDivisionDatas; }
            set { Set(ref jsDivisionDatas, value); }
        }

        private DivisionItems items;

        public DivisionItems Items
        {
            get { return items; }
            set { items = value; }
        }

        private int runState;
        public int RunState
        {
            get { return runState; }
            set
            {
                Set(ref runState, value);
            }
        }

        private int times;
        /// <summary>
        /// 节拍时间
        /// </summary>
        public int Times
        {
            get { return times; }
            set
            {
                Set(ref times, value);
            }
        }
        private bool jj;
        public bool Jj
        {
            get { return jj; }
            set
            {
                Set(ref jj, value);
            }
        }
        private int oKNum;
        public int OKNum
        {
            get { return oKNum; }
            set
            {
                Set(ref oKNum, value);
            }
        }

        private int nGNum;
        public int NGNum
        {
            get { return nGNum; }
            set
            {
                Set(ref nGNum, value);
            }
        }

        private int totalNum;
        public int TotalNum
        {
            get { return totalNum; }
            set
            {
                Set(ref totalNum, value);
            }
        }

        public string CardState { get; set; }
        public string ConCardState { get; set; }

        public ICommand ClearNum
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    OKNum = 0;
                    NGNum = 0;
                    TotalNum = 0;
                });
        }
        /// <summary>
        /// 测量用时
        /// </summary>
        public string Clys { get; set; }
        //public string Decnum { get; set; }
        private string decnum;

        public string Decnum
        {
            get
            {
                return "F" + MainViewModel.bal._runDB.set_test.Decimalnum.ToString();
            }
            set { decnum = value; RaisePropertyChanged("Decnum"); }
        }

        private bool cut0;
        public bool Cut0
        {
            get { return cut0; }
            set
            {
                Set(ref cut0, value);
            }
        }

        private bool cut1;
        public bool Cut1
        {
            get { return cut1; }
            set
            {
                Set(ref cut1, value);
            }
        }

        private bool cut2;
        public bool Cut2
        {
            get { return cut2; }
            set
            {
                Set(ref cut2, value);
            }
        }

        private bool oK;
        public bool OK
        {
            get { return oK; }
            set
            {
                Set(ref oK, value);
            }
        }

        private bool nG;
        public bool NG
        {
            get { return nG; }
            set
            {
                Set(ref nG, value);
            }
        }

        private bool jjbc;
        public bool Jjbc
        {
            get
            {
                Properties.Settings.Default.Jjbc = MainViewModel.bal._runDB.set_clamp.compesation;
                Properties.Settings.Default.Save();
                return MainViewModel.bal._runDB.set_clamp.compesation;
            }
            set
            {
                Set(ref jjbc, value);
                RaisePropertyChanged("Jjbc");
            }
        }

        private bool jbc;
        public bool Jbc
        {
            get { return jbc; }
            set
            {
                Set(ref jbc, value);
            }
        }

        private bool isonline;
        public bool Isonline
        {
            get { return isonline; }
            set
            {
                Set(ref isonline, value);
            }
        }
        private bool cardIsonline;
        public bool CardIsonline
        {
            get { return cardIsonline; }
            set
            {
                Set(ref cardIsonline, value);
            }
        }
        private bool conCardIsonline;
        public bool ConCardIsonline
        {
            get { return conCardIsonline; }
            set
            {
                Set(ref conCardIsonline, value);
            }
        }

        public bool Compesation { get; set; }
    }
}
