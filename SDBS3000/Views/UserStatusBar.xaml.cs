using SDBS3000.Utils.AppSettings;
using SDBS3000.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SDBS3000.Views
{
    /// <summary>
    /// StatusBar.xaml 的交互逻辑
    /// </summary>
    public partial class UserStatusBar : UserControl
    {
        public UserStatusBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ClearProperty =
        DependencyProperty.Register("Clear", typeof(ICommand), typeof(UserStatusBar));

        public ICommand Clear
        {
            get { return (ICommand)GetValue(ClearProperty); }
            set { SetValue(ClearProperty, value); }
        }

        public string RotorInformation
        {
            get { return (string)GetValue(RotorInformationProperty); }
            set { SetValue(RotorInformationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RotorInformation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotorInformationProperty =
            DependencyProperty.Register("RotorInformation", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string RotorInformation1
        {
            get { return (string)GetValue(RotorInformation1Property); }
            set { SetValue(RotorInformation1Property, value); }
        }

        // Using a DependencyProperty as the backing store for RotorInformation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotorInformation1Property =
            DependencyProperty.Register("RotorInformation1", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string RotorInformation2
        {
            get { return (string)GetValue(RotorInformation2Property); }
            set { SetValue(RotorInformation2Property, value); }
        }

        // Using a DependencyProperty as the backing store for RotorInformation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotorInformation2Property =
            DependencyProperty.Register("RotorInformation2", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string RotorInformation3
        {
            get { return (string)GetValue(RotorInformation3Property); }
            set { SetValue(RotorInformation3Property, value); }
        }

        // Using a DependencyProperty as the backing store for RotorInformation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotorInformation3Property =
            DependencyProperty.Register("RotorInformation3", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string RotorInformation4
        {
            get { return (string)GetValue(RotorInformation4Property); }
            set { SetValue(RotorInformation4Property, value); }
        }

        // Using a DependencyProperty as the backing store for RotorInformation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotorInformation4Property =
            DependencyProperty.Register("RotorInformation4", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string RotorInformation5
        {
            get { return (string)GetValue(RotorInformation5Property); }
            set { SetValue(RotorInformation5Property, value); }
        }

        // Using a DependencyProperty as the backing store for RotorInformation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotorInformation5Property =
            DependencyProperty.Register("RotorInformation5", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string WorkMode
        {
            get { return (string)GetValue(WorkModeProperty); }
            set { SetValue(WorkModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkModeProperty =
            DependencyProperty.Register("WorkMode", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));


        public string CardState
        {
            get { return (string)GetValue(CardStateProperty); }
            set { SetValue(CardStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardStateProperty =
            DependencyProperty.Register("CardState", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string ConCardState
        {
            get { return (string)GetValue(ConCardStateProperty); }
            set { SetValue(ConCardStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConCardState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConCardStateProperty =
            DependencyProperty.Register("ConCardState", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));



        public bool Compesation
        {
            get { return (bool)GetValue(CompesationProperty); }
            set { SetValue(CompesationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Compesation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompesationProperty =
            DependencyProperty.Register("Compesation", typeof(bool), typeof(UserStatusBar), new PropertyMetadata(false));



        public string RunState
        {
            get { return (string)GetValue(RunStateProperty); }
            set { SetValue(RunStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RunStateProperty =
            DependencyProperty.Register("RunState", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public int Times
        {
            get { return (int)GetValue(TimesProperty); }
            set { SetValue(TimesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimesProperty =
            DependencyProperty.Register("Times", typeof(int), typeof(UserStatusBar), new PropertyMetadata(0));

        public string Clys
        {
            get { return (string)GetValue(ClysProperty); }
            set { SetValue(ClysProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Clys.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClysProperty =
            DependencyProperty.Register("Clys", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));



        public int OKNum
        {
            get { return (int)GetValue(OKNumProperty); }
            set { SetValue(OKNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OKNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OKNumProperty =
            DependencyProperty.Register("OKNum", typeof(int), typeof(UserStatusBar), new PropertyMetadata(0));

        public int NGNum
        {
            get { return (int)GetValue(NGNumProperty); }
            set { SetValue(NGNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NGNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NGNumProperty =
            DependencyProperty.Register("NGNum", typeof(int), typeof(UserStatusBar), new PropertyMetadata(0));


        public int TotalNum
        {
            get { return (int)GetValue(TotalNumProperty); }
            set { SetValue(TotalNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalNumProperty =
            DependencyProperty.Register("TotalNum", typeof(int), typeof(UserStatusBar), new PropertyMetadata(0));

        public string CheckBox_Content
        {
            get { return (string)GetValue(CheckBox_ContentProperty); }
            set { SetValue(CheckBox_ContentProperty, value); }
        }

        public static readonly DependencyProperty CheckBox_ContentProperty =
            DependencyProperty.Register("CheckBox_Content", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string Total
        {
            get { return (string)GetValue(Total_ContentProperty); }
            set { SetValue(Total_ContentProperty, value); }
        }

        public static readonly DependencyProperty Total_ContentProperty =
            DependencyProperty.Register("Total", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public string ClearButton_Content
        {
            get { return (string)GetValue(ClearButton_Content_ContentProperty); }
            set { SetValue(ClearButton_Content_ContentProperty, value); }
        }

        public static readonly DependencyProperty ClearButton_Content_ContentProperty =
            DependencyProperty.Register("ClearButton_Content", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));


        public bool Jj
        {
            get
            {
                return (bool)GetValue(JjProperty);
            }
            set
            {
                SetValue(JjProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Compesation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JjProperty =
            DependencyProperty.Register("Jj", typeof(bool), typeof(UserStatusBar), new PropertyMetadata(false));

        public string JjContent
        {
            get { return (string)GetValue(JjContentProperty); }
            set { SetValue(JjContentProperty, value); }
        }

        public static readonly DependencyProperty JjContentProperty =
            DependencyProperty.Register("JjContent", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        public bool J
        {
            get { return (bool)GetValue(JProperty); }
            set { SetValue(JProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Compesation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JProperty =
            DependencyProperty.Register("J", typeof(bool), typeof(UserStatusBar), new PropertyMetadata(false));

        public string JContent
        {
            get { return (string)GetValue(JContentProperty); }
            set { SetValue(JContentProperty, value); }
        }

        public static readonly DependencyProperty JContentProperty =
            DependencyProperty.Register("JContent", typeof(string), typeof(UserStatusBar), new PropertyMetadata(""));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clear?.Execute(null);
        }

        /// <summary>
        /// 电机急停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
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
                                resultStr = "气缸异常（电机急停）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（电机急停）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（电机急停）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（电机急停）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（电机急停）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（电机急停）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（电机急停）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("电机急停命令执行异常: " + ex.Message);
                }

            });        
        }


        /// <summary>
        /// 电机复位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    var (success, code) = await MainViewModel.macControl.AlarmResetAsync();
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（电机复位）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（电机复位）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（电机复位）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（电机复位）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（电机复位）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（电机复位）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（电机复位）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("电机复位命令执行异常: " + ex.Message);
                }

            });
        }
        /// <summary>
        /// 气缸松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    var (success, code) = await MainViewModel.macControl.CylinderCRAsync(false);
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（气缸松开）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（气缸松开）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（气缸松开）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（气缸松开）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（气缸松开）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（气缸松开）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（气缸松开）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("气缸松开命令执行异常: " + ex.Message);
                }
            });

        }

        /// <summary>
        /// 气缸夹紧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    var (success, code) = await MainViewModel.macControl.CylinderCRAsync(true);
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（气缸夹紧）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（气缸夹紧）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（气缸夹紧）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（气缸夹紧）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（气缸夹紧）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（气缸夹紧）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（气缸夹紧）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("气缸夹紧命令执行异常: " + ex.Message);
                }
            });
        }
    }
}
