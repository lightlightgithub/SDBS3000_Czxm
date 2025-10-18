using SDBS3000.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace SDBS3000.Views
{
    /// <summary>
    /// PageClampKey.xaml 的交互逻辑
    /// </summary>
    public partial class PageClamp : Page
    {
        public PageClamp()
        {
            InitializeComponent();
            main.Focus();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tbx = sender as TextBox;
            string t = tbx.Text.Substring(0, tbx.SelectionStart) + e.Text + tbx.Text.Substring(tbx.SelectionStart + tbx.SelectionLength);
            int val;
            if (!int.TryParse(t, out val))
                e.Handled = true;
            else if (val > 10)
            {
                tbx.Text = "10";
                tbx.CaretIndex = tbx.Text.Length;
                e.Handled = true;
            }
            else if (val < 1)
            {
                tbx.Text = "2";
                tbx.CaretIndex = tbx.Text.Length;
                e.Handled = true;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

            var viewModel = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<ClampViewModel>();
            //viewModel.Cleanup();

            //ViewModelLocator.Cleanup<ClampViewModel>();
        }
    }
}
