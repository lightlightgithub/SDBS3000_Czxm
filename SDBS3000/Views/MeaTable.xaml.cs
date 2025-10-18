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
    /// MeaTable.xaml 的交互逻辑
    /// </summary>
    public partial class MeaTable : UserControl
    {
        public MeaTable()
        {
            InitializeComponent();
        }

        public string LabelContent
        {
            get { return (string)GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }

        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(string), typeof(MeaTable), new PropertyMetadata(""));

        public int Logotype
        {
            get { return (int)GetValue(LogotypeProperty); }
            set { SetValue(LogotypeProperty, value); }
        }

        public static readonly DependencyProperty LogotypeProperty =
            DependencyProperty.Register("Logotype", typeof(int), typeof(MeaTable), new PropertyMetadata(0));

        //只有卧式3圆是433 其余都是665
        public double SliderWidth
        {
            get { return (double)GetValue(SliderWidthProperty); }
            set { SetValue(SliderWidthProperty, value); }
        }

        public static readonly DependencyProperty SliderWidthProperty =
            DependencyProperty.Register("SliderWidth", typeof(double), typeof(MeaTable), new PropertyMetadata(665.0));

        public double SliderValue
        {
            get { return (double)GetValue(SliderValueProperty); }
            set { SetValue(SliderValueProperty, value); }
        }

        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register("SliderValue", typeof(double), typeof(MeaTable), new PropertyMetadata(0.0));

        //new PropertyMetadata(0d, PropertyChanged)

        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(MeaTable), new PropertyMetadata(0d, PropertyChanged));

        public double LastValue
        {
            get { return (double)GetValue(LastValueProperty); }
            set { SetValue(LastValueProperty, value); }
        }

        public static readonly DependencyProperty LastValueProperty =
            DependencyProperty.Register("LastValue", typeof(double), typeof(MeaTable), new PropertyMetadata(0d, PropertyChanged));

        private static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MeaTable c = obj as MeaTable;
            try
            {
                if (c.CurrentValue > c.Yxl)
                {
                    c.cv.Foreground = new SolidColorBrush(Colors.Red);
                    c.ca.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    c.cv.SetValue(ForegroundProperty, Application.Current.Resources["measurezs"]);
                    c.ca.SetValue(ForegroundProperty, Application.Current.Resources["measurezs"]);
                }
                if (c.LastValue > c.Yxl)
                {
                    c.lv.Foreground = new SolidColorBrush(Colors.Red);
                    c.la.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    c.lv.SetValue(ForegroundProperty, Application.Current.Resources["measurezs"]);
                    c.la.SetValue(ForegroundProperty, Application.Current.Resources["measurezs"]);
                }
            }
            catch
            {
                c.cv.SetValue(ForegroundProperty, Application.Current.Resources["measurezs"]);
                c.ca.SetValue(ForegroundProperty, Application.Current.Resources["measurezs"]);
                c.lv.SetValue(ForegroundProperty, Application.Current.Resources["measurezs"]);
                c.la.SetValue(ForegroundProperty, Application.Current.Resources["measurezs"]);
            }
        }

        public double CurrentAngle
        {
            get { return (double)GetValue(CurrentAngleProperty); }
            set { SetValue(CurrentAngleProperty, value); }
        }

        public static readonly DependencyProperty CurrentAngleProperty =
            DependencyProperty.Register("CurrentAngle", typeof(double), typeof(MeaTable), new PropertyMetadata());

        public double LastAngle
        {
            get { return (double)GetValue(LastAngleProperty); }
            set { SetValue(LastAngleProperty, value); }
        }

        public static readonly DependencyProperty LastAngleProperty =
            DependencyProperty.Register("LastAngle", typeof(double), typeof(MeaTable), new PropertyMetadata());


        public int ShowUnit
        {
            get { return (int)GetValue(ShowUnitProperty); }
            set { SetValue(ShowUnitProperty, value); }
        }

        public static readonly DependencyProperty ShowUnitProperty =
            DependencyProperty.Register("ShowUnit", typeof(int), typeof(MeaTable), new PropertyMetadata());

        public float Yxl
        {
            get { return (float)GetValue(YxlProperty); }
            set { SetValue(YxlProperty, value); }
        }

        public static readonly DependencyProperty YxlProperty =
            DependencyProperty.Register("Yxl", typeof(float), typeof(MeaTable), new PropertyMetadata(0f, PropertyChanged));

        public string Decinum
        {
            get { return (string)GetValue(DecinumProperty); }
            set { SetValue(DecinumProperty, value); }
        }

        public static readonly DependencyProperty DecinumProperty =
            DependencyProperty.Register("Decinum", typeof(string), typeof(MeaTable), new PropertyMetadata("F3"));
    }
}
