using SDBS3000.ViewModel;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// PageRtSet.xaml 的交互逻辑
    /// </summary>
    public partial class PageRtSet : Page
    {
        public PageRtSet()
        {
            InitializeComponent();
            if (jsms.SelectedIndex == 1)
            {
                A.Visibility = System.Windows.Visibility.Hidden;
                B.Visibility = System.Windows.Visibility.Hidden;
                C.Visibility = System.Windows.Visibility.Hidden;
            }
            Jiaodian.Focus();
        }

        private void Rt_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModelLocator.Cleanup<RtSetViewModel>();
        }

        private void Jsms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox c = sender as ComboBox;
            int jsms = c.SelectedIndex;
            GlobalVar.jsms = c.SelectedIndex;
            if (A is null)
                return;
            if (jsms == 1)
            {
                A.Visibility = System.Windows.Visibility.Hidden;
                B.Visibility = System.Windows.Visibility.Hidden;
                C.Visibility = System.Windows.Visibility.Hidden;
                //zcfs.Source = "/SDBS3000;component/Utils/Image/SupMet/stmr" + zc.ToString() + ".png";
            }
            if (jsms == 0)
            {
                A.Visibility = System.Windows.Visibility.Visible;
                B.Visibility = System.Windows.Visibility.Visible;
                C.Visibility = System.Windows.Visibility.Visible;
                //zcfs.Source = "/SDBS3000;component/Utils/Image/SupMet/stmr" + zc.ToString() + ".png";
            }
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var tbx = sender as TextBox;
            string t = tbx.Text.Substring(0, tbx.SelectionStart) + e.Text + tbx.Text.Substring(tbx.SelectionStart + tbx.SelectionLength);

            double val;
            if (!double.TryParse(t, out val))
                e.Handled = true;
            if (t.Contains(".") && (t.Length - t.IndexOf('.')) > 3)
                e.Handled = true;
        }
    }
}
