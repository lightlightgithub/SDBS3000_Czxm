using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging; 
using SDBS3000.Resources;
using SDBS3000.ViewModel;
using SDBSEntity;
using static SDBS3000.Views.Main;

namespace SDBS3000.Views
{
    public delegate void TestBegin();
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public event TestBegin testBegin;
        public Main()
        {
            GlobalVar.mainWindow = this;
            InitializeComponent();
            this.Loaded += BtmTool_Loaded;
            Messenger.Default.Register<int>(this, "iswarn", arg =>
            {
                if (arg > 0)
                    TAlarm.SetValue(BackgroundProperty, Application.Current.Resources["warnpng"]);
                else
                    TAlarm.SetValue(BackgroundProperty, Brushes.Transparent);
            });
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Messenger.Default.Register<bool>(this, "cut1", arg =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Binding b;
                    b = arg ? new Binding("[pm1qz]") : new Binding("[pm1jz]");
                    b.Source = LanguageManager.Instance;
                    pmy.SetBinding(MenuItem.HeaderProperty, b);
                }));
            });
            Messenger.Default.Register<bool>(this, "cut2", arg =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Binding b;
                    b = arg ? new Binding("[pm2qz]") : new Binding("[pm2jz]");
                    b.Source = LanguageManager.Instance;
                    pme.SetBinding(MenuItem.HeaderProperty, b);
                }));
            });
            Messenger.Default.Register<bool>(this, "cut0", arg =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Binding b;
                    b = arg ? new Binding("[jqz]") : new Binding("[jjz]");
                    b.Source = LanguageManager.Instance;
                    jjz.SetBinding(MenuItem.HeaderProperty, b);
                }));
            });
        }
        //帮助
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            help.Command.Execute(null);
        }
        //菜单扩展
        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            //menu.RaiseEvent(new RoutedEventArgs(MenuItem.CheckedEvent)); // 触发点击事件

            cd.IsSubmenuOpen = !cd.IsSubmenuOpen;
        }

        private void CommandBinding_Executed_2(object sender, ExecutedRoutedEventArgs e)
        {
            jqz.IsSubmenuOpen = !jqz.IsSubmenuOpen;
        }

        private void CommandBinding_Executed_3(object sender, ExecutedRoutedEventArgs e)
        {
            dbc.IsChecked = !dbc.IsChecked;
        }

        private void CommandBinding_Executed_4(object sender, ExecutedRoutedEventArgs e)
        {
            pmy.IsChecked = !pmy.IsChecked;
        }

        private void CommandBinding_Executed_5(object sender, ExecutedRoutedEventArgs e)
        {
            pme.IsChecked = !pme.IsChecked;
        }

        private void CommandBinding_Executed_6(object sender, ExecutedRoutedEventArgs e)
        {
            jjz.IsChecked = !jjz.IsChecked;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        #region BtmTool内容
        private bool edit;
        public bool Edit
        {
            get { return edit; }
            set { edit = value; }
        }
        private void BtmTool_Loaded(object sender, RoutedEventArgs e)
        {
            u.Focusable = true;
            u.Focus();
            string str = ConfigurationManager.AppSettings["OperatorLimit"];
            try
            {
                string[] c = str.Split(',');
                if (c[0] == "0")
                    TClampComp.Visibility = Visibility.Collapsed;
                if (c[1] == "0")
                    TKeyComp.Visibility = Visibility.Collapsed;
                if (c[2] == "0")
                    TPositioning.Visibility = Visibility.Collapsed;
                if (c[3] == "0")
                    TPrint.Visibility = Visibility.Collapsed;
            }
            catch { }
            if (GlobalVar.user.PERMISSION != 0)
                TUserMgt.Visibility = Visibility.Collapsed;
            if (GlobalVar.user.PERMISSION == 2)
            {
                TCal.IsEnabled = false;
                TRotorSet.IsEnabled = false;
                TMachSet.IsEnabled = false;
                TPositioning.IsEnabled = false;
                TClampComp.IsEnabled = false;
                TKeyComp.IsEnabled = false;
                TPrint.IsEnabled = false;
                TKeyComp.IsEnabled = false;
            }
         }

        private void U_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            switch (btn.Name)
            {
                //退出
                case "TExit":
                    //WindowVideo界面（用了winform插件）会阻塞整个程序退出
                    Application.Current.Shutdown();
                    break;
                //标定
                case "TCal":
                    if (frame.Source.ToString().Contains("PageCal"))
                        return;
                    break;
                case "TAlarm":
                    if (frame.Source.ToString().Contains("PageWarn"))
                        return;
                    break;
                //用户管理
                case "TUserMgt":
                    if (frame.Source.ToString().Contains("PageUser"))
                        return;
                    break;
                case "TRotorSet":
                    if (frame.Source.ToString().Contains("PageRtSet"))
                        return;
                    break;
                case "TMachSet":
                    if (frame.Source.ToString().Contains("PageSet"))
                        return;
                    break;
                case "TPositioning":
                    if (frame.Source.ToString().Contains("PagePos"))
                        return;
                    break;
            }

            if (frame.Source.ToString().Contains("PageSet"))
            {
                SetViewModel s = SimpleIoc.Default.GetInstance<SetViewModel>();
                if (!(s.Ki_max == MainViewModel.bal._runDB.set_test.ki_max &&
                    s.Rev_difference == MainViewModel.bal._runDB.set_test.Rev_difference &&
                     s.Refalsh == MainViewModel.bal._runDB.set_test.refalsh
                    //&&
                    //GlobalVar.Set.set1.posmode == s.PosMode &&
                    //GlobalVar.Set.set1.workmode == s.WorkMode &&
                    //GlobalVar.Set.set2.needle == s.Needle &&
                    //GlobalVar.Set.set2.safeDoor == s.SafeDoor &&
                    //GlobalVar.Set.Decimalnum == s.Decimalnum &&
                    //GlobalVar.Set.Direction == s.Direction &&
                    //GlobalVar.Set.Algorithm == s.Algorithm &&
                    //GlobalVar.Set.DataSaveMode == s.DataSaveMode &&
                    //GlobalVar.Set.DriverMode == s.DriverMode &&
                    //GlobalVar.Set.PrintMode == s.PrintMode
                    ))
                {
                    string rt = NewMessageBox.Show(LanguageManager.Instance["UnsavedItemsExistInTheSetting"], "Tip", CMessageBoxButton.YesNO).ToString();
                    if (rt == "Yes")
                    {
                        s.SetSave1.Execute(s);
                    }
                }
            }
            if (frame.Source.ToString().Contains("PageRtSet"))
            {
                if (GlobalVar.main.Rotor.ID == 0)
                {
                    NewMessageBox.Show(LanguageManager.Instance["CurrentRotorIsNotSaved"]);
                    return;
                }
                else
                {
                    PageRtSet rtSet = frame.Content as PageRtSet;
                    RtSetViewModel s = SimpleIoc.Default.GetInstance<RtSetViewModel>();
                    if (s.Rotor is null)
                        return;
                    if (!(s.Rotor.NAME == GlobalVar.main.Rotor.NAME
                        && s.Rotor.Jsms == GlobalVar.main.Rotor.Jsms
                        && s.Rotor.Ccdw == GlobalVar.main.Rotor.Ccdw
                        && s.Rotor.Clms == GlobalVar.main.Rotor.Clms
                        && s.Rotor.Yxldw == GlobalVar.main.Rotor.Yxldw
                        && s.Rotor.Jyxl == GlobalVar.main.Rotor.Jyxl
                        && s.Rotor.Pmyyxl == GlobalVar.main.Rotor.Pmyyxl
                        && s.Rotor.Pmeyxl == GlobalVar.main.Rotor.Pmeyxl
                        && s.Rotor.Zcfs == GlobalVar.main.Rotor.Zcfs
                        && s.Rotor.A == GlobalVar.main.Rotor.A
                        && s.Rotor.B == GlobalVar.main.Rotor.B
                        && s.Rotor.C == GlobalVar.main.Rotor.C
                        && s.Rotor.R1 == GlobalVar.main.Rotor.R1
                        && s.Rotor.R2 == GlobalVar.main.Rotor.R2))
                    {
                        string rt = NewMessageBox.Show(LanguageManager.Instance["WhetherOrNot"] + s.Rotor.NAME + "?", "Tip", CMessageBoxButton.YesNO).ToString();
                        if (rt == "Yes")
                        {
                            s.RtSetSave.Execute(rtSet);
                        }
                    }
                }
            }

            if (btn.Name == "TStart")
            {
                if (!(frame.Source.ToString().Contains("PageHor") || frame.Source.ToString().Contains("PageVer") || frame.Source.ToString().Contains("PageOne")))
                {
                    frame.Navigate(new Uri("Views/" + (FormulaResolvingModePageEnum)GlobalVar.FormulaResolvingMode + ".xaml", UriKind.Relative));
                }
                else if ((ushort)MainViewModel.bal._runDB.set_run.set_rpm == 0) //判断当前转子是否设置转速
                {
                    NewMessageBox.Show("当前转子未设置转速");
                }
                //判断当前转子是否标定
                else if (MainViewModel.bal._runDB.set0.cal_ai.All(x => x == 0) || MainViewModel.bal._runDB.set0.cal_ar.All(x => x == 0))
                {
                    NewMessageBox.Show("当前转子尚未标定");
                }
                else if (MainViewModel.bal._runDB.Bal_start && MainViewModel.bal._runDB.bal_result.rpm > 0) //判断开始测量是否进行中
                {
                    NewMessageBox.Show("当前转子仍在测量中");
                }
                else
                {
                    Task.Run(async () =>
                    {
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
                                        resultStr = "气缸异常（测量开始）";
                                        break;
                                    case 0x03:
                                        resultStr = "伺服异常（测量开始）";
                                        break;
                                    case 0xFF:
                                        resultStr = "等待响应超时（测量开始）";
                                        break;
                                    case 0xFE:
                                        resultStr = "发送命令失败（测量开始）";
                                        break;
                                    default:
                                        resultStr = $"未知错误 (Code: {code:X2})（测量开始）";
                                        break;
                                }
                                GlobalVar.Str = resultStr;
                            }
                            else
                            {
                                testBegin?.Invoke();
                                MainViewModel.bal._runDB.set_runmode = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("测量开始命令执行异常: " + ex.Message);
                        }
                    });
                }
            }
            else if (btn.Name == "TStop")
            { 
                Task.Run(async () =>
                {
                    try
                    {
                        var (success, code) = await MainViewModel.macControl.ServoStopAsync(MainViewModel.bal._runDB.set_run.drive_mode, MainViewModel.bal._runDB.set_run.work_mode, 0);
                        string resultStr;
                        if (!success)
                        {
                            switch (code)
                            {
                                case 0x02:
                                    resultStr = "气缸异常（停止）";
                                    break;
                                case 0x03:
                                    resultStr = "伺服异常（停止）";
                                    break;
                                case 0xFF:
                                    resultStr = "等待响应超时（停止）";
                                    break;
                                default:
                                    resultStr = $"未知错误 (Code: {code:X2})（停止）";
                                    break;
                            }
                            GlobalVar.Str = resultStr;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("停止命令执行异常: " + ex.Message);
                    }
                });
            }
            else
            {
                switch (btn.Name)
                {
                    //标定
                    case "TCal":
                        frame.Navigate(new Uri("Views/PageCal.xaml", UriKind.Relative));
                        break;
                    //用户管理
                    case "TUserMgt":
                        frame.Navigate(new Uri("Views/PageUser.xaml", UriKind.Relative));
                        break;
                    case "TRotorSet":
                        frame.Navigate(new Uri("Views/PageRtSet.xaml", UriKind.Relative));
                        break;
                    case "TMachSet":
                        frame.Navigate(new Uri("Views/PageSet.xaml", UriKind.Relative));
                        break;
                    case "TPositioning":
                        frame.Navigate(new Uri("Views/PagePos.xaml", UriKind.Relative));
                        break;
                    case "TClampComp":

                        frame.Navigate(new Uri("Views/PageClamp.xaml", UriKind.Relative));
                        break;
                    case "TSingleStep":
                        frame.Navigate(new Uri("Views/SingleStep.xaml", UriKind.Relative));
                        break;
                    case "TKeyComp":
                        frame.Navigate(new Uri("Views/PageKey.xaml", UriKind.Relative));
                        break;
                    case "TRecord":
                        frame.Navigate(new Uri("Views/PageReport.xaml", UriKind.Relative));
                        break;
                    case "TAlarm":
                        frame.Navigate(new Uri("Views/PageWarn.xaml", UriKind.Relative));
                        break;
                    case "TPrint":
                        Print print = Print.GetPrint();
                        print.Show();
                        break;
                    default:
                        var a = NewMessageBox.Show(btn.Name, "打开工具栏", CMessageBoxButton.YesNO, CMessageBoxImage.None);
                        break;
                }
            }
        }

        private void Ut_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var b_btm in u.Children)
            {
                var button = b_btm as Button;
            }
        }

        #endregion

        #region BtmTool内容：添加快捷键时将命令和事件绑定到一起。
        private void CmdBinding_Executed_KaiShi(object sender, ExecutedRoutedEventArgs e)
        {
            TStart.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // 触发点击事件
        }

        private void CmdBinding_Executed_TingZhi(object sender, ExecutedRoutedEventArgs e)
        {
            TStop.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // 触发点击事件
        }

        private void CmdBinding_Executed_BiaoDing(object sender, ExecutedRoutedEventArgs e)
        {
            TCal.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Executed_ZhuanZi(object sender, ExecutedRoutedEventArgs e)
        {
            TRotorSet.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Executed_JiQi(object sender, ExecutedRoutedEventArgs e)
        {
            TMachSet.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Executed_JiaJu(object sender, ExecutedRoutedEventArgs e)
        {
            TClampComp.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Executed_Jian(object sender, ExecutedRoutedEventArgs e)
        {
            TKeyComp.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Executed_DingWai(object sender, ExecutedRoutedEventArgs e)
        {
            TPositioning.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Executed_YongHu(object sender, ExecutedRoutedEventArgs e)
        {
            TUserMgt.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void CmdBinding_Executed_DaYin(object sender, ExecutedRoutedEventArgs e)
        {
            TPrint.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void CmdBinding_Executed_JiLu(object sender, ExecutedRoutedEventArgs e)
        {
            TRecord.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void CmdBinding_Executed_BaoJin(object sender, ExecutedRoutedEventArgs e)
        {
            TAlarm.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void CmdBinding_Executed_TuiChu(object sender, ExecutedRoutedEventArgs e)
        {
            TExit.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        #endregion


        //变浅色主题
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary resource = new ResourceDictionary();
            {
                resource.Source = new Uri("pack://application:,,,/SDBS3000;component/Utils/Themes/LightStyle.xaml");
            }
            Application.Current.Resources.MergedDictionaries[5] = resource;
        }

        //变深色主题
        private void MenuItem_Click1(object sender, RoutedEventArgs e)
        {
            ResourceDictionary resource = new ResourceDictionary();
            {
                resource.Source = new Uri("pack://application:,,,/SDBS3000;component/Utils/Themes/DarkStyle.xaml");
            }
            Application.Current.Resources.MergedDictionaries[5] = resource;
        }
    }
}
