using SDBS3000.Resources;
using SDBS3000.ViewModel;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SDBS3000.Views
{
    /// <summary>
    /// PageHorTwo.xaml 的交互逻辑
    /// </summary>
    public partial class PageHorTwo : Page
    {
        public PageHorTwo()
        {
            InitializeComponent();
            //zkexp.Visibility = Visibility.Collapsed;
            //fdexp.Visibility = Visibility.Collapsed;
            //if (GlobalVar.Set.Algorithm == 2)
            //    zkexp.Visibility = Visibility.Visible;
            //if (GlobalVar.Set.Algorithm == 1)
            //    fdexp.Visibility = Visibility.Visible;
        }
    }
}
