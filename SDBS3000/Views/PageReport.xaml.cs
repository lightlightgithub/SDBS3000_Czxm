using GalaSoft.MvvmLight.Messaging;
using SDBS3000.Log;
using SDBS3000.Resources;
using SDBS3000.ViewModel;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
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
    /// PageReport.xaml 的交互逻辑
    /// </summary>
    public partial class PageReport : Page
    {
        public PageReport()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.PageReportViewModel();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<int>(this);
        }

       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog print = new PrintDialog();
            Print(this, "Microsoft Print to PDF", "test print", 1);
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="userControl">要打印的控件</param>
        /// <param name="printer">打印机名称</param>
        /// <param name="description">打印描述</param>
        /// <param name="copyCount">打印个数</param>
        public static void Print(Page userControl, string printer, string description, int copyCount)
        {
            var localPrintServer = new LocalPrintServer();
            var printQueue = localPrintServer.GetPrintQueue(printer);
            if (printQueue.IsInError)
            {
                throw new Exception("打印机处于错误状态");
            }

            printQueue.UserPrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

            var printDialog = new PrintDialog
            {
                PrintQueue = printQueue, //打印队列
                PrintTicket = { CopyCount = copyCount } //打印个数
            };


            //设置纸张大小
            userControl.Width = (int)Math.Ceiling(printDialog.PrintableAreaWidth);
            userControl.Height = (int)Math.Ceiling(printDialog.PrintableAreaHeight);

            //打印
            printDialog.PrintVisual(userControl, description);
        }


        private void Datagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

    }
}
