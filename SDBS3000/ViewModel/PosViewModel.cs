using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SDBS3000.Resources;
using SDBS3000.Utils.AppSettings;
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
        ushort fre = 1;
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
           

           
           



        }

        private async void Time_Elapsed(object sender, ElapsedEventArgs e)
        {

            double.TryParse(Line[10], out double seconds);
            RemainingSeconds = (dtbegin1.AddSeconds(seconds) - DateTime.Now).Seconds;
            if (RemainingSeconds == 0)
            {
                time.Stop();
                try
                {
                    var (success, code) = await MainViewModel.macControl.ServoStopAsync(MainViewModel.bal._runDB.set_run.drive_mode, 1, 0);
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（热机停止）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（热机停止）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（热机停止）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（热机停止）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（热机停止）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（热机停止）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（热机停止）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("热机停止命令执行异常: " + ex.Message);
                }
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
                    AccDecel();
                    
                    GlobalVar.config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    NewMessageBox.Show(LanguageManager.Instance["Saved"]);                  
                });
        }

        public async void AccDecel()
        {
            ushort.TryParse(Line[6], out fre);
            try
            {
                var (success, code) = await MainViewModel.macControl.AccDecelAsync(true, fre);
                string resultStr;
                if (!success)
                {
                    switch (code)
                    {
                        case 0x02:
                            resultStr = "气缸异常（启停加减速）";
                            break;
                        case 0x03:
                            resultStr = "伺服异常（启停加减速）";
                            break;
                        case 0x04:
                            resultStr = "伺服报警（启停加减速）";
                            break;
                        case 0x05:
                            resultStr = "伺服忙（启停加减速）";
                            break;
                        case 0x06:
                            resultStr = "伺服未启动（启停加减速）";
                            break;
                        case 0xFF:
                            resultStr = "等待响应超时（启停加减速）";
                            break;
                        default:
                            resultStr = $"未知错误 (Code: {code:X2})（启停加减速）";
                            break;
                    }
                    GlobalVar.Str = resultStr;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("启停加减速命令执行异常: " + ex.Message);
            }

            
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
           async obj =>
            {

                if (int.TryParse(Line[10], out int parsedSeconds))
                {
                    _remainingSeconds = parsedSeconds;
                }
                dtbegin1 = DateTime.Now;
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
                                resultStr = "气缸异常（热机开始）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（热机开始）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（热机开始）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（热机开始）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（热机开始）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（热机开始）";
                                break;
                            case 0xFE:
                                resultStr = "发送命令失败（热机开始）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（热机开始）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                        return;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("热机开始命令执行异常: " + ex.Message);
                }               
                time.Elapsed += Time_Elapsed;

                time.Start();
               

            });
        }

        public ICommand SFCalStart
        {
            get => new RelayCommand<object>(
            async obj =>
            {
                try
                {
                    var (success, code) = await MainViewModel.macControl.SearchPulseAsync();
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（查找脉冲）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（查找脉冲）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（查找脉冲）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（查找脉冲）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（查找脉冲）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（查找脉冲）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（查找脉冲）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("查找脉冲命令执行异常: " + ex.Message);
                }

               
            });
        }

        System.DateTime dtbegin1;

    }
}
