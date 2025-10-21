using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SDBS3000.Resources;
using SDBS3000.Views;
using SDBSEntity;
using SDBSEntity.Model;

namespace SDBS3000.ViewModel
{
    public class ClampViewModel : ViewModelBase
    {
        int clcs;
        int clms;//1是静平衡

        public ClampViewModel()
        {
            MainViewModel.bal.OnEventTriggered += Bal_OnEventTriggered1;
            Dqbz = 1;
            Stepkey = 0;
        }

        private void Bal_OnEventTriggered1(int message = 0)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if ((GlobalVar.mainWindow.frame.Source.ToString().Contains("PageClamp") || GlobalVar.mainWindow.frame.Source.ToString().Contains("PageKey")))
                {
                    if (message == 3)
                    {
                        try
                        {
                            var (success, code) = await MainViewModel.macControl.ServoStopAsync(MainViewModel.bal._runDB.set_run.drive_mode, 1, 0);
                            string resultStr;
                            if (!success)
                            {
                                switch (code)
                                {
                                    case 0x02:
                                        resultStr = "气缸异常（夹具补偿停止）";
                                        break;
                                    case 0x03:
                                        resultStr = "伺服异常（夹具补偿停止）";
                                        break;
                                    case 0x04:
                                        resultStr = "伺服报警（夹具补偿停止）";
                                        break;
                                    case 0x05:
                                        resultStr = "伺服忙（夹具补偿停止）";
                                        break;
                                    case 0x06:
                                        resultStr = "伺服未启动（夹具补偿停止）";
                                        break;
                                    case 0xFF:
                                        resultStr = "等待响应超时（夹具补偿停止）";
                                        break;
                                    default:
                                        resultStr = $"未知错误 (Code: {code:X2})（夹具补偿停止）";
                                        break;
                                }
                                GlobalVar.Str = resultStr;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("夹具补偿停止命令执行异常: " + ex.Message);
                        }

                    }
                    //夹具补偿完成,同步至数据库
                    if (message == 8)
                    {
                        Task.Run(() =>
                        {
                            CodeFirstDbContext Entity = new CodeFirstDbContext();
                            T_Clampdata clamp = Entity.T_Clampdatas.FirstOrDefault(f => f.rotorid == GlobalVar.main.Rotor.ID);
                            if (clamp is null)
                            {
                                T_Clampdata clampdata = new T_Clampdata
                                {
                                    test_times = MainViewModel.bal._runDB.set_clamp.test_times,
                                    cps_val_1 = MainViewModel.bal._runDB.set_clamp.cps_val[0],
                                    cps_val_2 = MainViewModel.bal._runDB.set_clamp.cps_val[1],
                                    cps_val_3 = MainViewModel.bal._runDB.set_clamp.cps_val[2],
                                    cps_val_4 = MainViewModel.bal._runDB.set_clamp.cps_val[3],
                                    rotorid = GlobalVar.main.Rotor.ID,
                                };
                                Entity.T_Clampdatas.Add(clampdata);
                            }
                            else
                            {
                                clamp.test_times = MainViewModel.bal._runDB.set_clamp.test_times;
                                clamp.cps_val_1 = MainViewModel.bal._runDB.set_clamp.cps_val[0];
                                clamp.cps_val_2 = MainViewModel.bal._runDB.set_clamp.cps_val[1];
                                clamp.cps_val_3 = MainViewModel.bal._runDB.set_clamp.cps_val[2];
                                clamp.cps_val_4 = MainViewModel.bal._runDB.set_clamp.cps_val[3];
                            }
                            Entity.SaveChanges();
                            Entity.Dispose();
                        });
                        return;
                    }

                    SingleL = MainViewModel.bal._runDB.bal_result.singleL;
                    SingleR = MainViewModel.bal._runDB.bal_result.singleR;
                    Progress = 20 * MainViewModel.bal.ki / MainViewModel.bal._runDB.set_test.ki_max;
                    Rpm = MainViewModel.bal._runDB.bal_result.rpm;
                    Data1 = MainViewModel.bal._runDB.bal_result.fl;
                    Data2 = MainViewModel.bal._runDB.bal_result.ql;
                    Data3 = MainViewModel.bal._runDB.bal_result.fr;
                    Data4 = MainViewModel.bal._runDB.bal_result.qr;
                    Messenger.Default.Send(MainViewModel.bal._runDB.bal_result.rpm.ToString(), "speed");
                    RaisePropertyChanged("SingleL");
                    RaisePropertyChanged("SingleR");
                    RaisePropertyChanged("Rpm");
                    RaisePropertyChanged("Data1");
                    RaisePropertyChanged("Data2");
                    RaisePropertyChanged("Data3");
                    RaisePropertyChanged("Data4");
                    RaisePropertyChanged("Progress");
                }
            });


        }

        private int stepkey;

        public int Stepkey
        {
            get { return stepkey; }
            set
            {
                stepkey = value;
                RaisePropertyChanged("Stepkey");
            }
        }

        private int description;
        /// <summary>
        /// 状态提示
        /// </summary>
        public int Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        private int descriptionKey;
        /// <summary>
        /// 状态提示
        /// </summary>
        public int DescriptionKey
        {
            get { return descriptionKey; }
            set
            {
                descriptionKey = value;
                RaisePropertyChanged("DescriptionKey");
            }
        }


        private ushort clamp_times;

        public ushort Clamp_times
        {
            get { return (ushort)MainViewModel.bal._runDB.set_clamp.test_times; }
            set
            {
                Set(ref clamp_times, value);
                MainViewModel.bal._runDB.set_clamp.test_times = value;
                Angel0 = 360 / Clamp_times;
                RaisePropertyChanged("Angel0");
                RaisePropertyChanged("Clamp_times");
            }
        }

        private string angel;

        public string Angel
        {
            get { return angel; }
            set { angel = value; }
        }
        public int Angel0 { get; set; }

        private int dqbz;

        public int Dqbz
        {
            get { return dqbz; }
            set
            {
                Set(ref dqbz, value);
                //2.安放置好工件 & quot; 启动 & quot; 夹具自动夹紧，待数据稳定后自动停止运转
                if (dqbz == 1)
                {
                    Dqbzms = 0;
                    Angel = "0°";
                }
                else if (dqbz > 1 && dqbz != Clamp_times + 1)
                {
                    Dqbzms = 1;
                    Angel = (360 / Clamp_times * (dqbz - 1)).ToString() + "°";
                }

                else
                    Dqbzms = 2;
                RaisePropertyChanged("Dqbzms");
                RaisePropertyChanged("Angel");
                RaisePropertyChanged("Dqbz");
            }
        }

        private int dqbzms;

        public int Dqbzms
        {
            get { return dqbzms; }
            set { dqbzms = value; }
        }



        //16-32   216-232
        public ICommand ClampOpen
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    MainViewModel.bal._runDB.set_clamp.compesation = true;
                });
        }

        public ICommand ClampClose
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    MainViewModel.bal._runDB.set_clamp.compesation = false;
                });
        }


        public ICommand ClampMea
        {
            get => new RelayCommand<object>(
               async obj =>
                {
                    MainViewModel.bal._runDB.set_runmode = 0;
                    MainViewModel.bal._runDB.set_clamp.test_times = Clamp_times;
                    Angel0 = 360 / Clamp_times;
                    RaisePropertyChanged("Angel");
                    if (Dqbz == Clamp_times + 1)
                    {
                        NewMessageBox.Show(LanguageManager.Instance["This"]);
                        return;
                    }
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
                                    resultStr = "气缸异常（夹具补偿开始）";
                                    break;
                                case 0x03:
                                    resultStr = "伺服异常（夹具补偿开始）";
                                    break;
                                case 0x04:
                                    resultStr = "伺服报警（夹具补偿开始）";
                                    break;
                                case 0x05:
                                    resultStr = "伺服忙（夹具补偿开始）";
                                    break;
                                case 0x06:
                                    resultStr = "伺服未启动（夹具补偿开始）";
                                    break;
                                case 0xFF:
                                    resultStr = "等待响应超时（夹具补偿开始）";
                                    break;
                                case 0xFE:
                                    resultStr = "发送命令失败（夹具补偿开始）";
                                    break;
                                default:
                                    resultStr = $"未知错误 (Code: {code:X2})（夹具补偿开始）";
                                    break;
                            }
                            GlobalVar.Str = resultStr;
                        }
                        else
                        {
                            MainViewModel.bal._runDB.set_clamp.start_test = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("夹具补偿开始命令执行异常: " + ex.Message);
                    }
                
                   
                    // Dqbz = MainViewModel.bal.clamp_times;
                    Dqbz += 1;
                    RaisePropertyChanged("Dqbz");
                });
        }

        public ICommand Cancel
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    Dqbz = 1;
                    RaisePropertyChanged("Dqbz");
                    MainViewModel.bal._runDB.set_clamp.start_test = false;
                });
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }



        #region 键
        private string key_step;

        public string Key_step
        {
            get { return key_step; }
            set { Set(ref key_step, value); }
        }

        private string btn_key_test = "检测";

        public string Btn_key_test
        {
            get { return btn_key_test; }
            set { Set(ref btn_key_test, value); }
        }

        private double[] cps_val_key;

        public double[] Cps_val_key
        {
            get { return cps_val_key; }
            set { Set(ref cps_val_key, value); }
        }

        //144-160 232-248
        public ICommand KeyOpen
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    Messenger.Default.Send(true, "jbc");
                    NewMessageBox.Show(LanguageManager.Instance["TKCTurnon"]);

                    RtSetViewModel s = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<RtSetViewModel>();
                    //s.Rotor.key1 = key[0];
                    //s.Rotor.key2 = key[1];
                    //s.Rotor.key3 = key[2];
                    //s.Rotor.key4 = key[3];
                    CodeFirstDbContext Entity = new CodeFirstDbContext();
                    Entity.Entry(s.Rotor).State = System.Data.Entity.EntityState.Modified;
                    Entity.Entry(s.Rotor).CurrentValues.SetValues(s.Rotor);
                    int res = Entity.SaveChanges();
                    Entity.Dispose();
                });
        }

        public ICommand KeyClose
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    Messenger.Default.Send(false, "jbc");
                    NewMessageBox.Show(LanguageManager.Instance["TKCTurnOff"]);
                });
        }

        public ICommand KeyMea
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    if (Stepkey < 2)
                        Stepkey++;
                    else
                    {
                        NewMessageBox.Show(LanguageManager.Instance["Reset"]);
                        return;
                    }
                });
        }

        public ICommand CancelKey
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    Stepkey = 0;
                });
        }
        #endregion

        public double Rpm { get; set; }
        public double Data1 { get; set; }
        public double Data2 { get; set; }
        public double Data3 { get; set; }
        public double Data4 { get; set; }
        public int SingleL { get; set; }
        public int SingleR { get; set; }
        public int Progress { get; set; }//   progress比20  等于 读出来的比测量次数
    }
}
