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
using System.Windows.Shapes;
using static SDBS3000.Log.Log;

namespace SDBS3000.Views
{
    /// <summary>
    /// ISO.xaml 的交互逻辑
    /// </summary>
    public partial class ISO : Window
    {
        public ISO()
        {
            InitializeComponent();
            ISO_Border.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tbx = sender as TextBox;
            string t = tbx.Text.Substring(0, tbx.SelectionStart) + e.Text + tbx.Text.Substring(tbx.SelectionStart + tbx.SelectionLength);

            double val;
            if (!double.TryParse(t, out val))
                e.Handled = true;
            if (t.Contains(".") && (t.Length - t.IndexOf('.')) > 3)
                e.Handled = true;
        }

        private void Phdj_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                var cbx = sender as ComboBox;
                var bd = VisualTreeHelper.GetChild(cbx, 0);
                Grid grid = VisualTreeHelper.GetChild(bd, 0) as Grid;
                var tbx = grid.Children[2] as TextBox;
                string t = tbx.Text.Substring(0, tbx.SelectionStart) + e.Text + tbx.Text.Substring(tbx.SelectionStart + tbx.SelectionLength);

                double val;
                if (!double.TryParse(t, out val))
                    e.Handled = true;
                if (t.Contains(".") && (t.Length - t.IndexOf('.')) > 3)
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Write(LogType.ERROR, ex.ToString());
            }
        }

        private void CmdBinding_CtrlX(object sender, ExecutedRoutedEventArgs e)
        {
            GuanBiBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}
