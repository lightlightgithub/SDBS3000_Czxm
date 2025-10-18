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

namespace SDBS3000.Ucs.Controls
{
    /// <summary>
    /// SetItem.xaml 的交互逻辑
    /// </summary>
    public partial class SetItem : UserControl
    {
        public SetItem()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tbx = sender as TextBox;
            string t = tbx.Text.Substring(0, tbx.SelectionStart) + e.Text + tbx.Text.Substring(tbx.SelectionStart + tbx.SelectionLength);
            if (Limit == 1)
            {
                int val;
                if (!Int32.TryParse(t, out val))
                    e.Handled = true;
            }
            else if (Limit == 2)
            {
                double val;
                if (!double.TryParse(t, out val))
                    e.Handled = true;
            }
            //两位小数
            else if (Limit == 3)
            {
                double val;
                if (!double.TryParse(t, out val))
                    e.Handled = true;
                if (t.Contains(".") && (t.Length - t.IndexOf('.')) > 3)
                    e.Handled = true;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(SetItem), new PropertyMetadata(""));

        public int Limit
        {
            get { return (int)GetValue(LimitProperty); }
            set { SetValue(LimitProperty, value); }
        }

        public static readonly DependencyProperty LimitProperty =
            DependencyProperty.Register("Limit", typeof(int), typeof(SetItem), new PropertyMetadata());

        public double TextWidth
        {
            get { return (double)GetValue(TextWidthProperty); }
            set { SetValue(TextWidthProperty, value); }
        }

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(SetItem), new PropertyMetadata(220.0));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SetItem), new PropertyMetadata(""));
    }
}
