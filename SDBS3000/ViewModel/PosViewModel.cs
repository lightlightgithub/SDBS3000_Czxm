using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SDBS3000.Resources;
using SDBS3000.Views;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SDBS3000.ViewModel
{
    public class PosViewModel : ViewModelBase
    {
        Binding binding1 = new Binding("[pluperrev]") { Source = LanguageManager.Instance };
        Binding binding2 = new Binding("[candet]") { Source = LanguageManager.Instance };
        TextBlock textBlock;
        public System.Timers.Timer timer;
        System.Timers.Timer time = new System.Timers.Timer { Interval = 1000 };

        public PosViewModel()
        {
            //Pulsecontent = "脉冲检测";
            
            Line = new string[12];
            GlobalVar.config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Line[1] = GlobalVar.GetStr("Poszfz");
            Line[2] = GlobalVar.GetStr("Poskglx");
            Line[3] = GlobalVar.GetStr("Posnum");
            Line[4] = GlobalVar.GetStr("Poscs");
            Line[5] = GlobalVar.GetStr("Posspeed");
            Line[6] = GlobalVar.GetStr("Posatime");
            Line[7] = GlobalVar.GetStr("Posdtime");
            Line[8] = GlobalVar.GetStr("Posbc");
            Line[9] = GlobalVar.GetStr("Poszcbc");
            Line[10] = GlobalVar.GetStr("Warmtime");
            Line[11] = GlobalVar.GetStr("SpeedFactor");
           
            if (int.TryParse(Line[10], out int parsedSeconds))
            {
                _remainingSeconds = parsedSeconds;
            }
            
           
           



        }

        private void Time_Elapsed(object sender, ElapsedEventArgs e)
        {
            double.TryParse(Line[10], out double seconds);
            RemainingSeconds = (dtbegin1.AddSeconds(seconds) - DateTime.Now).Seconds;
            if (RemainingSeconds == 0)
            {
                time.Stop();
                MainViewModel.svTrans.SetSVON(3);
                MainViewModel.svTrans.SetSVON(2);
                MainViewModel.svTrans.SetSVON(5);
                RemainingSeconds = 0;
            }
        }

        private string[] line;

        public string[] Line
        {
            get { return line; }
            set { line = value; }
        }

        private int angel;
        public int Angel
        {
            get { return angel; }
            set { Set(ref angel, value); }
        }
        
        private string pulsecontent;

        public string Pulsecontent
        {
            get { return pulsecontent; }
            set { Set(ref pulsecontent, value);
                RaisePropertyChanged("Pulsecontent");
            }
        }
         private int _remainingSeconds;
        /// <summary>
        /// 剩余热机时间
        /// </summary>
        public int RemainingSeconds
        {
            get { return _remainingSeconds; }
            set { Set(ref _remainingSeconds, value);
                RaisePropertyChanged("Pulsecontent");
            }
        }

        public ICommand SetSave
        {
            get => new RelayCommand<object>(
                obj =>
                {

                    GlobalVar.config.AppSettings.Settings["Poszfz"].Value = Line[1];
                    GlobalVar.config.AppSettings.Settings["Poskglx"].Value = Line[2];
                    GlobalVar.config.AppSettings.Settings["Posnum"].Value = Line[3];
                    GlobalVar.config.AppSettings.Settings["Poscs"].Value = Line[4];
                    GlobalVar.config.AppSettings.Settings["Posspeed"].Value = Line[5];
                    GlobalVar.config.AppSettings.Settings["Posatime"].Value = Line[6];
                    GlobalVar.config.AppSettings.Settings["Posdtime"].Value = Line[7];
                    GlobalVar.config.AppSettings.Settings["Posbc"].Value = Line[8];
                    GlobalVar.config.AppSettings.Settings["Poszcbc"].Value = Line[9];
                    GlobalVar.config.AppSettings.Settings["Warmtime"].Value = Line[10];
                    GlobalVar.config.AppSettings.Settings["SpeedFactor"].Value = Line[11];                   
                    GlobalVar.config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    NewMessageBox.Show(LanguageManager.Instance["Saved"]);                  
                });
        }

        public ICommand Pulse
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    textBlock = obj as TextBlock;
                    //Pulsecontent == "脉冲检测"
                    if (textBlock.Text== "检测脉冲数(C)"|| textBlock.Text == "Pluse Per Rev(C)")
                    {
                        timer.Enabled = true;
                        timer.Start();

                        textBlock.SetBinding(TextBlock.TextProperty, binding2);
                    }
                    else
                    {
                        //Pulsecontent = "脉冲检测";
                        textBlock.SetBinding(TextBlock.TextProperty, binding1);
                        timer.Enabled = false;
                    }
                });
        }

        public ICommand Heat
        {
            get => new RelayCommand<object>(
            obj =>
            {
                dtbegin1 = DateTime.Now;
                MainViewModel.svTrans.SetSVON(0);
                time.Elapsed += Time_Elapsed;

                time.Start();
               

            });
        }

        public ICommand SFCalStart
        {
            get => new RelayCommand<object>(
            obj =>
            {
                //Task tstop = Task.Run(() =>
                //{
                //    if (MainViewModel.svTrans.WriteSingleRegister32(0x16, 2))
                //    {
                //        Thread.Sleep(200);
                //        if (MainViewModel.svTrans.Balstart(1))
                //        {
                //            while (true)
                //            {
                //                Thread.Sleep(400);
                //                if (GlobalVar.FlagofSFSpeed == 0)
                //                {
                //                    MainViewModel.svTrans.WriteSingleRegister32(0x16, 1);
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //});
                byte code;
                bool success = MainViewModel.macControl.SearchPulse(out code);
                string resultStr = "成功";
                if (!success)
                {
                    switch (code)
                    {
                        case 0x02:
                            resultStr = "气缸异常";//传给报警页面
                            break;
                        case 0x03:
                            resultStr = "伺服异常";
                            break;
                        default:
                            resultStr = "未知错误";
                            break;
                    }
                    GlobalVar.Str = resultStr;
                }
            });
        }

        System.DateTime dtbegin1;

    }
}
