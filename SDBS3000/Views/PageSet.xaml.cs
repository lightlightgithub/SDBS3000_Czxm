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
    /// PageSet.xaml 的交互逻辑
    /// </summary>
    public partial class PageSet : Page
    {
        public PageSet()
        {
            InitializeComponent();
            PageSetScrollViewer_Fcs.Focus();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.setItemActive["测量次数"])
                clcs.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["刷新频率"])
                sxpl.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["转差范围"])
                zcfw.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["工作方式"])
                gzfs.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["定位模式"])
                dwms.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["光针模式"])
                gzms.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["安全门模式"])
                aqm.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["矢量图方向"])
                sltfx.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["显示位数"])
                xsws.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["算法选择"])
                sfxz.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["工件驱动模式"])
                gjqdms.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["数据保存方式"])
                sjbcfs.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["打印模式"])
                dyms.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["扫码功能"])
                smgn.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["三色报警灯"])
                ssbjd.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["双手启动"] )
                ssqd.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["气压保护"])
                qybh.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["避让算法"] )
                brsf.Visibility = Visibility.Collapsed;
            if (!GlobalVar.setItemActive["固定偏量"])
                gdpl.Visibility = Visibility.Collapsed;

        }
    }
}
