using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SDBS3000.Communicate.Drive;
using SDBS3000.Communicate.Servo;
using SDBS3000.Control;
using SDBS3000.Resources;
using SDBS3000.Utils.AppSettings;
using SDBS3000.Views;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using static SDBS3000.Log.Log;

namespace SDBS3000.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region 支撑方式映射字典
        int[] supportmethod = { 1, 2, 7, 5, 6, 0, 3, 4, 8, 9, 10 };
        #endregion

        bool authorizationshow = false;
        public static balance bal = new balance();
        public static ServoTrans svTrans = new ServoTrans();
        public static HardwareCon hc = new HardwareCon();
        public static MacControl macControl = new MacControl();
        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(GlobalVar.mainWindow))
            {
                return;
            }
            using (CodeFirstDbContext entity = new CodeFirstDbContext())
            {
                ObservableCollection<T_RotorSet> RotorSets = new ObservableCollection<T_RotorSet>(entity.T_RotorSet);
                int rtid = GlobalVar.Getushort("lastrotor");
                bal.License = ConfigurationManager.AppSettings["License"];
                bal.Decrypt();
                if (bal.remainDays == 0)
                {
                    NewMessageBox.Show("授权已到期");
                }
                else if (bal.remainDays < 10)
                {
                    NewMessageBox.Show("授权剩余天数：" + bal.remainDays);
                }
                if (rtid == 0 || RotorSets.Where(p => p.ID == rtid).Count() == 0)
                    Rotor = entity.T_RotorSet.FirstOrDefault();
                else
                    Rotor = RotorSets.Where(p => p.ID == rtid).First();

                int clms = 0;
                if (Rotor != null)
                    clms = Rotor.Clms;
                GlobalVar.FormulaResolvingMode = clms;
                //更新测量页面模式
                //第一次加载测量界面内嵌网页，假如转子设置里面没有测量模式，默认为0
                CurPage = (FormulaResolvingModePageEnum)clms + ".xaml";

                GlobalVar.setItemActive = new System.Collections.Generic.Dictionary<string, bool>();
                GlobalVar.setItemKey = new System.Collections.Generic.Dictionary<string, int>();
                foreach (T_Dictionary dictionary in entity.Dictionarys)
                {
                    GlobalVar.setItemActive.Add(dictionary.name, dictionary.active);
                    GlobalVar.setItemKey.Add(dictionary.name, dictionary.id);
                }
                bal._runDB.hard_run.zzid = Rotor.ID;
                bal._runDB.set_test.ki_max = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["测量次数"]).value);
                bal._runDB.set_test.refalsh = entity.Dictionarys.Find(GlobalVar.setItemKey["刷新频率"]).value;
                bal._runDB.set_test.Rev_difference = entity.Dictionarys.Find(GlobalVar.setItemKey["转差范围"]).value;
                bal._runDB.set_test.WorkMode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["工作方式"]).value);
                bal._runDB.set_test.PosMode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["定位模式"]).value);

                bal._runDB.set_test.Needle = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["光针模式"]).value);
                bal._runDB.set_test.SafeDoor = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["安全门模式"]).value);

                bal._runDB.set_test.Decimalnum = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["显示位数"]).value);
                bal._runDB.set_test.Direction = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["矢量图方向"]).value);
                bal._runDB.set_test.Algorithm = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["算法选择"]).value);
                bal._runDB.set_run.drive_mode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["工件驱动模式"]).value);
                bal._runDB.set_test.DataSaveMode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["数据保存方式"]).value);
                bal._runDB.set_test.PrintMode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["打印模式"]).value);
                bal._runDB.set_test.MultSpeedCal = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["多段标定"]).value);
                //bool.TryParse(GlobalVar.GetStr("ClampComp"), out bool clampopen);
                
                bal._runDB.set_clamp.compesation = Properties.Settings.Default.Jjbc;
                T_Caldata caldata = entity.T_Caldatas.OrderByDescending(o => o.ID).FirstOrDefault(p => p.rotorid == Rotor.ID && p.speed == Rotor.Speed);
                T_Clampdata clampdata = entity.T_Clampdatas.FirstOrDefault(p => p.rotorid == Rotor.ID);
                SyncBalData syncBalData = new SyncBalData();
                syncBalData.SyncCal(caldata);
                syncBalData.SyncClamp(clampdata);
            }

            try
            {
                
                Left1 = SystemParameters.FullPrimaryScreenWidth - 420;

            }
            catch (Exception e)
            {
                NewMessageBox.Show(e.Message);
            }
            GlobalVar.main = this;
            //时钟
            Timer time = new Timer { Interval = 1000 };
            time.Elapsed += Time_Elapsed;
            time.Start();
            Messenger.Default.Register<string>(this, "speed", arg =>
            {
                Speed = arg;
                RaisePropertyChanged("Speed");
            });
        }

        /// <summary>
        /// 右上角时钟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Time_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                TimeLabel = DateTime.Now.ToString("HH:mm:ss");
                DateLabel = DateTime.Now.ToString("yyyy/MM/dd");

            }));
            UpdateAlarmState();
        }
        private string lastAlarmMessage = null; // 记录上一次报警内容，防止重复写入

        public void UpdateAlarmState()
        {
            string alarmMessage = GlobalVar.Str?.Trim();
            bool hasAlarm = !string.IsNullOrEmpty(alarmMessage);
            bool isNewAlarm = hasAlarm && alarmMessage != lastAlarmMessage;

            // --- 处理报警状态 ---
            if (hasAlarm)
            {
                if (isNewAlarm)
                {
                    // 新报警：创建 Warn 并保存到数据库
                    currentWarn = new Warn
                    {
                        WarnDescription = alarmMessage,
                        Isopen = true // 自动记录时间
                    };
                    SaveAlarmToDatabase(currentWarn);
                    lastAlarmMessage = alarmMessage;
                }
                else
                {
                    // 老报警：保持状态
                    if (currentWarn != null)
                    {
                        currentWarn.Isopen = true;
                    }
                }

                WarnList = new ObservableCollection<Warn> { currentWarn };
            }
            else
            {
                // 无报警
                if (currentWarn != null)
                {
                    currentWarn.Isopen = false;
                }
                WarnList = new ObservableCollection<Warn>();
                lastAlarmMessage = null; // 重置
            }

            Application.Current?.Dispatcher.Invoke(() =>
            {
                Messenger.Default.Send(WarnList.Count, "iswarn");
            });

        }
        private void SaveAlarmToDatabase(Warn warn)
        {
            try
            {
                using (var ctx = new CodeFirstDbContext())
                {
                    var alarm = new T_Alarm
                    {
                        WarnDescription = warn.WarnDescription,
                        Date = warn.Date,
                        Time = warn.Time,
                    };

                    ctx.T_Alarms.Add(alarm);
                    ctx.SaveChanges(); // 写入数据库
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存报警失败: {ex.Message}");
            }
        }

        private T_RotorSet rotor;

        public T_RotorSet Rotor
        {
            get { return rotor; }
            set
            {
                Set(ref rotor, value);
                if (value != null)
                    UpdateFormula(value);
            }
        }

        
        /// <summary>
        /// 更新配方
        /// </summary>
        /// <param name="value"></param>
        private void UpdateFormula(T_RotorSet value)
        {
            try
            {
                bal._runDB.hard_run.A = Convert.ToDouble(value.A);
                bal._runDB.hard_run.B = Convert.ToDouble(value.B);
                bal._runDB.hard_run.C = Convert.ToDouble(value.C);
                bal._runDB.set_run.set_rpm = Convert.ToDouble(value.Speed);
                bal._runDB.hard_run.enable = value.Jsms == 0 ? true : false;
                //bal._runDB.set_run.add_mode1 = value.pmy;
                //bal._runDB.set_run.add_mode2 = value.pme;
                //bal._runDB.set_run.add_mode0 = value.;
            }
            catch (Exception ex)
            {
                Write(LogType.ERROR, "配方下载至PLC异常：" + ex.Message);
            }
        }

        #region 时钟计时器

        private string timeLabel;

        public string TimeLabel
        {
            get { return timeLabel; }
            set { Set(ref timeLabel, value); }
        }

        private string dateLabel;

        public string DateLabel
        {
            get { return dateLabel; }
            set { Set(ref dateLabel, value); }
        }

        // 视频窗口左边距        
        private double left;
        public double Left1
        {
            get { return left; }
            set { left = value; }
        }

        // 菜单切换语言
        public ICommand Lan
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    var app = Application.Current as App;
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    switch (obj.ToString())
                    {
                        case "中文":
                            LanguageManager.Instance.ChangeLanguage(new CultureInfo("zh-CN"));
                            config.AppSettings.Settings["Language"].Value = "0";
                            break;
                        case "English":
                            LanguageManager.Instance.ChangeLanguage(new CultureInfo("En"));
                            config.AppSettings.Settings["Language"].Value = "1";
                            break;
                        default:
                            break;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                });
        }

        public ICommand Auth
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    Authorization a = new Authorization();
                    a.ShowDialog();
                });
        }

        public ICommand Drilling
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    WindowDrilling a = new WindowDrilling();
                    a.ShowDialog();
                });
        }

        public ICommand Help
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    System.Windows.Forms.Help.ShowHelp(null, ".\\help.chm");
                });
        }

        private string curPage;

        public string CurPage
        {
            get { return curPage; }
            set
            {
                Set(ref curPage, value);
            }
        }

        private int formulaResolvingMode;

        public int FormulaResolvingMode
        {
            get { return formulaResolvingMode; }
            set { Set(ref formulaResolvingMode, value); }
        }
        #endregion

        #region 加去重  电补偿
        //0是静加去重 1是平面1  2是平面2
        public bool cut2;

        public bool Cut2
        {
            get
            {
                if (cut2)
                {
                    pm2jqz = LanguageManager.Instance["pm2qz"];
                }
                else
                {
                    pm2jqz = LanguageManager.Instance["pm2jz"];
                }
                RaisePropertyChanged("Pm2jqz");
                Messenger.Default.Send(cut2, "cut2");
                return cut2;
            }
            set
            {
                Set(ref cut2, value);
                bal._runDB.set_run.add_mode2 = !value;
                Messenger.Default.Send(!value, "Cut2");
            }
        }
        public bool cut1;

        public bool Cut1
        {
            get
            {
                if (cut1)
                {
                    pm1jqz = LanguageManager.Instance["pm1qz"];
                }
                else
                {
                    pm1jqz = LanguageManager.Instance["pm1jz"];
                }
                RaisePropertyChanged("Pm1jqz");
                Messenger.Default.Send(cut1, "cut1");
                return cut1;
            }
            set
            {
                Set(ref cut1, value);
                bal._runDB.set_run.add_mode1 = !value;
                Messenger.Default.Send(!value, "Cut1");
            }
        }
        public bool cut0;

        public bool Cut0
        {
            get
            {
                if (cut0)
                {
                    jjqz = LanguageManager.Instance["jqz"];
                }
                else
                {
                    jjqz = LanguageManager.Instance["jjz"];
                }
                RaisePropertyChanged("Jjqz");
                Messenger.Default.Send(cut0, "cut0");
                return cut0;
            }
            set
            {
                Set(ref cut0, value);
                bal._runDB.set_run.add_mode0 = !value;
                Messenger.Default.Send(!value, "Cut0");
            }
        }


        public string jjqz;
        public string Jjqz
        {
            get
            {
                return jjqz;
            }
            set
            {
                RaisePropertyChanged("Jjqz");
                Set(ref jjqz, value);
            }
        }

        public string pm1jqz;
        public string Pm1jqz
        {
            get { return pm1jqz; }
            set
            {
                RaisePropertyChanged("Pm1jqz");
                Set(ref pm1jqz, value);
            }
        }

        public string pm2jqz;
        public string Pm2jqz
        {
            get { return pm2jqz; }
            set
            {
                RaisePropertyChanged("Pm2jqz");
                Set(ref pm2jqz, value);
            }
        }
    

        private bool compesation;

        public bool Compesation
        {
            get { return compesation; }
            set
            {
                Set(ref compesation, value);
                bal._runDB.set_run.compesation = value;

            }
        }
        #endregion       

        private string speed;

        public string Speed
        {
            get
            {
                return bal._runDB.bal_result.rpm.ToString();
            }
            set
            {
                speed = value;
                RaisePropertyChanged("Speed");
            }
        }

        // public string Speed { get; set; }

        private Warn[] warns;

        public Warn[] Warns
        {
            get { return warns; }
            set
            {
                Set(ref warns, value);
            }
        }
        private Warn currentWarn;
        private ObservableCollection<Warn> warnList;

        public ObservableCollection<Warn> WarnList
        {
            get { return warnList; }
            set
            {
                Set(ref warnList, value);
            }
        }
    }
   
    public class Warn : ViewModelBase
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string WarnDescription { get; set; }
        private bool isopen;
        public bool Isopen
        {
            get { return isopen; }
            set
            {
                if (value != isopen)
                {
                    this.Date = DateTime.Now.ToShortDateString();
                    this.Time = DateTime.Now.ToLongTimeString();
                }
                Set(ref isopen, value);
            }
        }
        public bool Isknow { get; set; }
    }
}