using SDBS3000.ViewModel;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media.Effects;

namespace SDBS3000.Views
{
    /// <summary>
    /// Authorization.xaml 的交互逻辑
    /// </summary>
    public partial class Authorization : Window
    {
        static Authorization autosingleton = null;

        public static Authorization Getautosingleton()
        {
            if (autosingleton is null)
                autosingleton = new Authorization();
            return autosingleton;
        }

        enum Sqxx
        {
            设备序列号或授权码为空 = 8051,
            授权码位数错误 = 8052,
            非本机授权码 = 8053,
            授权码使用期限到 = 8055,
            授权码已过期 = 8058,
            授权码已失效 = 8059,
            授权码验证通过 = 7000,
        }

        public Authorization()
        {
            InitializeComponent();
            sqm.Text = MainViewModel.bal._balanceData.License;
            xlh.Text = MainViewModel.bal._balanceData.DeviceNo;
            syts.Content = MainViewModel.bal._balanceData.remainDays;
            xkpd.Content = MainViewModel.bal._balanceData.remainDays > 0 ? "Pass" : "";
        }

        private void Sup_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.bal._balanceData.License = sqm.Text;
            MainViewModel.bal.Decrypt();
            syts.Content = MainViewModel.bal._balanceData.remainDays;
            xkpd.Content = MainViewModel.bal._balanceData.remainDays > 0 ? "Pass" : "";

            if (MainViewModel.bal._balanceData.remainDays > 0)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["License"].Value = sqm.Text;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
