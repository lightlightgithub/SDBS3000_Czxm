using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using SDBS3000.Views;
using System.Windows.Threading;
using static SDBS3000.Log.Log;
using System.Drawing;

namespace SDBS3000
{
    /// <summary>
    /// 
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.PrimaryScreen;
            GlobalVar.doublehei = screen.Bounds.Height;
            GlobalVar.doublewit = screen.Bounds.Width;
            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
            GlobalVar.scale = graphics.DpiX / 96f;

            //防止程序多开
            string MName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
            string PName = System.IO.Path.GetFileNameWithoutExtension(MName);
            System.Diagnostics.Process[] myProcess = System.Diagnostics.Process.GetProcessesByName(PName);
            if (myProcess.Length > 1)
            {
                Application.Current.Shutdown();
                return;
            }
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                int lan = Convert.ToInt32(ConfigurationManager.AppSettings["Language"]);
                SDBS3000.Resources.LanguageManager.Instance.ChangeLanguage(new System.Globalization.CultureInfo("zh-CN"));
                if (lan == 0)
                    SDBS3000.Resources.LanguageManager.Instance.ChangeLanguage(new System.Globalization.CultureInfo("zh-CN"));
                else if (lan == 1)
                    SDBS3000.Resources.LanguageManager.Instance.ChangeLanguage(new System.Globalization.CultureInfo("En"));
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.ToString());
            }
            Login w = new Login();
            w.ShowDialog();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //处理UI线程上的未处理异常
            e.Handled = true;
            //MessageBox.Show("发生错误：" + e.Exception.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Write(LogType.ERROR, "UI异常：" + e.Exception.Message);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //处理非UI线程上的未处理异常
            Exception ex = (Exception)e.ExceptionObject;
            //MessageBox.Show("发生错误：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Write(LogType.ERROR, "非UI线程：" + ex.Message);
        }

        /// <summary>
        /// Task线程内未捕获异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs ex)
        {
            //MessageBox.Show("Task线程异常：" + ex.Exception.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //设置该异常已察觉（这样处理后就不会引起程序崩溃）                   
            Write(LogType.ERROR, "Task异常：" + ex.Exception.InnerException.Message);
            ex.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
