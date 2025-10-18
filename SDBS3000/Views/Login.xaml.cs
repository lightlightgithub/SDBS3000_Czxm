using SDBS3000.Log;
using SDBS3000.ViewModel;
using SDBSEntity;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static SDBS3000.Log.Log;

namespace SDBS3000.Views
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.Cleanup<LoginViewModel>();
        }

    }
}
