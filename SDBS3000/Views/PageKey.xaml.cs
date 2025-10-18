using SDBS3000.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// PageClampKey.xaml 的交互逻辑
    /// </summary>
    public partial class PageKey : Page
    {
        public PageKey()
        {
            InitializeComponent();
            main.Focus();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tbx = sender as TextBox;
            string t = tbx.Text + e.Text;

            {
                int val;
                if (!int.TryParse(t, out val))
                    e.Handled = true;
                else if (val > 10 || val < 0)
                {                    
                    tbx.Text = "10";
                    tbx.CaretIndex = tbx.Text.Length;
                    e.Handled = true;
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var viewModel = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<ClampViewModel>();
            viewModel.Cleanup();

            ViewModelLocator.Cleanup<ClampViewModel>();
        }
    }
}
