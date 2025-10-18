using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SDBS3000.Ucs
{
    /// <summary>
    /// Circle.xaml 的交互逻辑
    /// </summary>
    public partial class pCircle : UserControl
    {
        public pCircle()
        {
            InitializeComponent();
        }

        double maxr;
        double cirstr = 2;
        double cirredstr = 2;
        double cirredzj = 23;//红色圆直径
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UserControl u = sender as UserControl;
            double d = u.ActualHeight > u.ActualWidth ? u.ActualWidth : u.ActualHeight;
            d = d < 0 ? 0 : d;
            maxr = d;
            grid.Height = d;
            grid.Width = d;
            cir.Height = d;
            cir.Width = d;

            cir1.Height = d * 0.8;
            cir1.Width = d * 0.8;

            cir2.Height = d * 0.6;
            cir2.Width = d * 0.6;

            cir3.Height = d * 0.4;
            cir3.Width = d * 0.4;

            //label0.Margin = new Thickness(2, (u.ActualHeight - d) / 2 - 28, 0, 0);
            //label180.Margin = new Thickness(2, 0, 0, (u.ActualHeight - d) / 2 - 28);
            //外弧基于357 5
            cir.StrokeThickness = d / 357 * cirstr;

            //刻度线处理M5,178.5 L352 178.5M178.5,5 L178.5 352
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            string path = string.Format("M{0},{1} L{2},{1} M{1},{0} L{1},{2}", cir.StrokeThickness, d / 2, d - cir.StrokeThickness);
            var geometry = (Geometry)(converter.ConvertFrom(path));
            p1.Data = geometry;
            p2.Data = geometry;
            p3.Data = geometry;

            //处理合格圆
            cirOK.Width = 0.2 * d;
            cirOK.Height = 0.2 * d;

            //处理红色圆M7,11.5 L16,11.5M11.5,7 L11.5,16
            cirred.StrokeThickness = d / 357 * cirredstr;
            cirvalue.Height = cirredzj * d / 357;
            cirvalue.Width = cirredzj * d / 357;
            string pathred = string.Format("M{0},{1} L{2},{1} M{1},{0} L{1},{2}", cirvalue.Width / 3.3, cirvalue.Width / 2, cirvalue.Width - cirvalue.Width / 3.3);
            var geometryred = (Geometry)(converter.ConvertFrom(pathred));
            pred.Data = geometryred;

            //处理扇形M178.5,178.5L175,6A178,178,0,0,1,182,6L178.5,178.5
            double r = d / 2;
            string pathsec = string.Format("M{0},{0} L{1},{2} A{0},{0},0,0,1,{3},{2}L{0},{0}", r, r * 175 / 178.5, d / 357 * cirstr + 1, r * 182 / 178.5);
            var geometrysec = (Geometry)(converter.ConvertFrom(pathsec));
            sector.Data = geometrysec;
            
            try
            {
                double actualAngel = this.Angel;
                double left = this.Todb(this.MeaValue) / (this.MaxValue * 5) * this.maxr * Math.Cos(actualAngel * Math.PI / 180);
                double top = this.Todb(this.MeaValue) / (this.MaxValue * 5) * this.maxr * Math.Sin(actualAngel * Math.PI / 180);
                this.cirvalue.Margin = new Thickness(left, top, 0, 0);
            }
            catch { }
        }

        public double Angel
        {
            get { return (double)GetValue(AngelProperty); }
            set { SetValue(AngelProperty, value); }
        }

        public static readonly DependencyProperty AngelProperty =
            DependencyProperty.Register("Angel", typeof(double), typeof(pCircle), new PropertyMetadata(0d, PropertyChanged));

        public double MeaValue
        {
            get { return (double)GetValue(MeaValueProperty); }
            set { SetValue(MeaValueProperty, value); }
        }

        public static readonly DependencyProperty MeaValueProperty =
            DependencyProperty.Register("MeaValue", typeof(double), typeof(pCircle), new PropertyMetadata(0d, PropertyChanged));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(pCircle), new PropertyMetadata(0d, PropertyChanged));

        private static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            pCircle c = obj as pCircle;
            try
            {
                double actualAngel = c.Angel;
                double left = c.Todb(c.MeaValue) / (c.MaxValue * 5) * c.maxr * Math.Cos(actualAngel * Math.PI / 180);
                double top = c.Todb(c.MeaValue) / (c.MaxValue * 5) * c.maxr * Math.Sin(actualAngel * Math.PI / 180);
                c.cirvalue.Margin = new Thickness(left, top, 0, 0);
            }
            catch { }
        }

        public double Todb<T>(T value)
        {
            if (value == null)
                return 0;
            return double.Parse(value.ToString()) > MaxValue * 5 ? MaxValue * 5 : double.Parse(value.ToString());
        }

        public int Direction
        {
            get { return (int)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(int), typeof(pCircle), new PropertyMetadata(0, DirectionPropertyChanged));

        private static void DirectionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            pCircle c = obj as pCircle;
            try
            {
                if (c.Direction == 1)
                {
                    System.Windows.Media.TransformGroup group = c.circlegrid.RenderTransform as TransformGroup;
                    foreach (var tra in group.Children)
                    {
                        if (tra is ScaleTransform)
                        {
                            ScaleTransform scale = tra as ScaleTransform;
                            scale.ScaleX = -1;
                        }
                    }
                }
            }
            catch { }
        }
    }
}
