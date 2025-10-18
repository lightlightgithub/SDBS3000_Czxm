using SDBS3000.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SDBS3000.Views
{
    /// <summary>
    /// PageCal.xaml 的交互逻辑
    /// </summary>
    public partial class PageCal : Page
    {
        public PageCal()
        {
            InitializeComponent();
            main.Focus();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tbx = sender as TextBox;
            string t = tbx.Text + e.Text;
            {
                double val;
                if (!double.TryParse(t, out val))
                    e.Handled = true;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var viewModel = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<CalViewModel>();
            //viewModel.Cleanup();

            //ViewModelLocator.Cleanup<CalViewModel>();
        }
       
    }
}
