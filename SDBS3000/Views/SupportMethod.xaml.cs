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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SDBS3000.Views
{
    /// <summary>
    /// SupportMethod.xaml 的交互逻辑
    /// </summary>
    public partial class SupportMethod : Window
    {
        //软支撑隐藏部分，解算模式软硬支撑
        public SupportMethod(int input, int jsms)
        {
            InitializeComponent();
            Zcfs = input;
            RadioButton[] uIElements = new RadioButton[11];
            gropu.Children.CopyTo(uIElements, 0);
            uIElements[Zcfs].IsChecked = true;
            if (jsms == 1)
            {
                uIElements[1].Visibility = Visibility.Collapsed;
                //uIElements[2].IsEnabled = false; /SDBS3000;component/Utils/Image/SupMet/s1.png
                uIElements[3].Visibility = Visibility.Collapsed;
                uIElements[4].Visibility = Visibility.Collapsed;
                uIElements[6].Visibility = Visibility.Collapsed;
                uIElements[7].Visibility = Visibility.Collapsed;
                uIElements[8].Visibility = Visibility.Collapsed;
                uIElements[9].Visibility = Visibility.Collapsed;
                uIElements[0].Tag = "/SDBS3000;component/Utils/Image/SupMet/sr1.png";
                uIElements[2].Tag = "/SDBS3000;component/Utils/Image/SupMet/sr3.png";
                uIElements[10].Tag = "/SDBS3000;component/Utils/Image/SupMet/sr11.png";
            }
        }

        public int Zcfs { get; set; }

        private void Sup_Loaded(object sender, RoutedEventArgs e)
        {
            BlurEffect effect = new BlurEffect();
            effect.Radius = 3;
            effect.KernelType = KernelType.Gaussian;
            Application.Current.Windows[0].Effect = effect;
        }

        private void Sup_Closed(object sender, EventArgs e)
        {
            if (Application.Current.Windows.Count > 0)
            {
                Application.Current.Windows[0].Effect = null;
            }
            this.Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var a = gropu;
            RadioButton[] uIElements = new RadioButton[11];
            a.Children.CopyTo(uIElements, 0);
            int i = gropu.Children.IndexOf(uIElements.First(p => p.IsChecked == true));
        }
    }
}
