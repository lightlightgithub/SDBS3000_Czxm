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
        #region ֧�ŷ�ʽӳ���ֵ�
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
                    NewMessageBox.Show("��Ȩ�ѵ���");
                }
                else if (bal.remainDays < 10)
                {
                    NewMessageBox.Show("��Ȩʣ��������" + bal.remainDays);
                }
                if (rtid == 0 || RotorSets.Where(p => p.ID == rtid).Count() == 0)
                    Rotor = entity.T_RotorSet.FirstOrDefault();
                else
                    Rotor = RotorSets.Where(p => p.ID == rtid).First();

                int clms = 0;
                if (Rotor != null)
                    clms = Rotor.Clms;
                GlobalVar.FormulaResolvingMode = clms;
                //���²���ҳ��ģʽ
                //��һ�μ��ز���������Ƕ��ҳ������ת����������û�в���ģʽ��Ĭ��Ϊ0
                CurPage = (FormulaResolvingModePageEnum)clms + ".xaml";

                GlobalVar.setItemActive = new System.Collections.Generic.Dictionary<string, bool>();
                GlobalVar.setItemKey = new System.Collections.Generic.Dictionary<string, int>();
                foreach (T_Dictionary dictionary in entity.Dictionarys)
                {
                    GlobalVar.setItemActive.Add(dictionary.name, dictionary.active);
                    GlobalVar.setItemKey.Add(dictionary.name, dictionary.id);
                }
                bal._runDB.hard_run.zzid = Rotor.ID;
                bal._runDB.set_test.ki_max = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["��������"]).value);
                bal._runDB.set_test.refalsh = entity.Dictionarys.Find(GlobalVar.setItemKey["ˢ��Ƶ��"]).value;
                bal._runDB.set_test.Rev_difference = entity.Dictionarys.Find(GlobalVar.setItemKey["ת�Χ"]).value;
                bal._runDB.set_test.WorkMode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["������ʽ"]).value);
                bal._runDB.set_test.PosMode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["��λģʽ"]).value);

                bal._runDB.set_test.Needle = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["����ģʽ"]).value);
                bal._runDB.set_test.SafeDoor = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["��ȫ��ģʽ"]).value);

                bal._runDB.set_test.Decimalnum = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["��ʾλ��"]).value);
                bal._runDB.set_test.Direction = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["ʸ��ͼ����"]).value);
                bal._runDB.set_test.Algorithm = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["�㷨ѡ��"]).value);
                bal._runDB.set_run.drive_mode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["��������ģʽ"]).value);
                bal._runDB.set_test.DataSaveMode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["���ݱ��淽ʽ"]).value);
                bal._runDB.set_test.PrintMode = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["��ӡģʽ"]).value);
                bal._runDB.set_test.MultSpeedCal = Convert.ToUInt16(entity.Dictionarys.Find(GlobalVar.setItemKey["��α궨"]).value);
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
            //ʱ��
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
        /// ���Ͻ�ʱ��
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
        private string lastAlarmMessage = null; // ��¼��һ�α������ݣ���ֹ�ظ�д��

        public void UpdateAlarmState()
        {
            string alarmMessage = GlobalVar.Str?.Trim();
            bool hasAlarm = !string.IsNullOrEmpty(alarmMessage);
            bool isNewAlarm = hasAlarm && alarmMessage != lastAlarmMessage;

            // --- ������״̬ ---
            if (hasAlarm)
            {
                if (isNewAlarm)
                {
                    // �±��������� Warn �����浽���ݿ�
                    currentWarn = new Warn
                    {
                        WarnDescription = alarmMessage,
                        Isopen = true // �Զ���¼ʱ��
                    };
                    SaveAlarmToDatabase(currentWarn);
                    lastAlarmMessage = alarmMessage;
                }
                else
                {
                    // �ϱ���������״̬
                    if (currentWarn != null)
                    {
                        currentWarn.Isopen = true;
                    }
                }

                WarnList = new ObservableCollection<Warn> { currentWarn };
            }
            else
            {
                // �ޱ���
                if (currentWarn != null)
                {
                    currentWarn.Isopen = false;
                }
                WarnList = new ObservableCollection<Warn>();
                lastAlarmMessage = null; // ����
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
                    ctx.SaveChanges(); // д�����ݿ�
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"���汨��ʧ��: {ex.Message}");
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
        /// �����䷽
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
                Write(LogType.ERROR, "�䷽������PLC�쳣��" + ex.Message);
            }
        }

        #region ʱ�Ӽ�ʱ��

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

        // ��Ƶ������߾�        
        private double left;
        public double Left1
        {
            get { return left; }
            set { left = value; }
        }

        // �˵��л�����
        public ICommand Lan
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    var app = Application.Current as App;
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    switch (obj.ToString())
                    {
                        case "����":
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

        #region ��ȥ��  �粹��
        //0�Ǿ���ȥ�� 1��ƽ��1  2��ƽ��2
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