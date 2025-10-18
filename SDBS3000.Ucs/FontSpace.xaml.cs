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

namespace SDBS3000.Ucs
{
    /// <summary>
    /// FontSpace.xaml 的交互逻辑
    /// </summary>
    public partial class FontSpace : UserControl
    {
        public FontSpace()
        {
            InitializeComponent();
        }

        public string Ctx
        {
            get { return (string)GetValue(CtxProperty); }
            set { SetValue(CtxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ctx.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CtxProperty =
            DependencyProperty.Register("Ctx", typeof(string), typeof(FontSpace), new PropertyMetadata(""));



        public double Fts
        {
            get { return (double)GetValue(FtsProperty); }
            set { SetValue(FtsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FtsProperty =
            DependencyProperty.Register("Fts", typeof(double), typeof(FontSpace), new PropertyMetadata(40.0));



        public Thickness Thickness
        {
            get { return (Thickness)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(Thickness), typeof(FontSpace), new PropertyMetadata(new Thickness(2)));


    }
}
