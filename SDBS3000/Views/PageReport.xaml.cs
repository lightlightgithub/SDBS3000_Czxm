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
        //string[] jsmsdic = { "硬支撑", "软支撑" };        
        //string[] clmsdic = { "双面动平衡", "静平衡", "动静平衡", "立式双面动平衡", "立式动静平衡" };
        Dictionary<int, string> dic = new Dictionary<int, string>();
        Dictionary<int, string> dicclms = new Dictionary<int, string>();
        // public PageInfo pageInfo = new PageInfo();
        public int pageSize = 5;
        public PageReport()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.PageReportViewModel();
            Messenger.Default.Register<int>(this, "language", arg =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    // datagrid.ItemsSource = null;
                    GetMs();
                });
            });
        }

        private void GetMs()
        {
            string[] jsmsdic = { LanguageManager.Instance["Hard bearing"], LanguageManager.Instance["Soft bearing"] };
            string[] clmsdic = { LanguageManager.Instance["two plane dynamic balance"], LanguageManager.Instance["static balance"], LanguageManager.Instance["dynamic/static balance"], LanguageManager.Instance["vertical two plane dynamic balance"], LanguageManager.Instance["vertical dynamic/static balance"] };
            foreach (var d in Data0)
            {
                d.Jsms = jsmsdic[Convert.ToInt32(dic[d.ID])];
                d.Clms = clmsdic[Convert.ToInt32(dicclms[d.ID])];
            }
        }

        public ObservableCollection<T_MeasureData> Data0
        {
            get;
            set;
        }
        //动态加载列头
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            begintime.SelectedDate = DateTime.Now.Date;
            endtime.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            using (CodeFirstDbContext Entity = new CodeFirstDbContext())
            {
                var pageNumName = Convert.ToInt32(pageNum.Content.ToString().Trim());
                Data0 = new ObservableCollection<T_MeasureData>(Entity.T_MeasureData.Where(p => p.MODIFYTIME <= endtime.SelectedDate && p.MODIFYTIME >= begintime.SelectedDate && p.isclear == 0).OrderByDescending(p => p.ID));
                dic = Data0.ToDictionary(p => p.ID, p => p.Jsms);
                dicclms = Data0.ToDictionary(p => p.ID, p => p.Clms);
                GetMs();
                Type t = new T_MeasureData().GetType();
                PropertyInfo[] propertys = t.GetProperties();
                DataGridTextColumn col = null;
                for (int i = 0; i < propertys.Count(); i++)
                {
                    if (Attribute.IsDefined(propertys[i], (typeof(DescriptionAttribute))))
                    {
                        col = new DataGridTextColumn() { Binding = new Binding(propertys[i].Name) };
                        //Rotor Name
                        //des = Convert.ToString(((DescriptionAttribute)propertys[i].GetCustomAttribute(typeof(DescriptionAttribute))).Description);

                        StringBuilder sb = new StringBuilder();
                        sb.Append("[");
                        sb.Append(Convert.ToString(((DescriptionAttribute)propertys[i].GetCustomAttribute(typeof(DescriptionAttribute))).Description));
                        sb.Append("]");
                        Binding binding = new Binding(sb.ToString())
                        {
                            Source = LanguageManager.Instance
                        };
                        TextBlock textBlock = new TextBlock();
                        textBlock.SetBinding(TextBlock.TextProperty, binding);

                        col.Header = textBlock;
                        if (propertys[i].PropertyType.Name == "DateTime")
                            col.Binding.StringFormat = "s";
                        // datagrid.Columns.Add(col);
                    }
                }
                var result = Data0.Skip((pageNumName - 1) * pageSize).Take(pageSize).ToList();
                //   datagrid.ItemsSource = result;
                totalCount.Content  = Data0.Count.ToString();
                totalPage.Content = (Data0.Count % pageSize == 0 ? Data0.Count / pageSize : Data0.Count / pageSize + 1).ToString();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<int>(this);
        }

        private void WrapPanel_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button.Content.ToString() == "显示日历")
                return;
            DateTime datenow = DateTime.Now.Date;
            DateTime date1 = datenow;
            DateTime date2 = datenow;
            if (button.Name == "look")
            {
                date1 = begintime.SelectedDate.Value.Date;
                date2 = endtime.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);
                if (date1 > date2)
                {
                    NewMessageBox.Show(LanguageManager.Instance["SDate"]);
                    return;
                }
            }
            else if (button.Name == "look1")
            {
                date1 = datenow;
                date2 = datenow.AddDays(1).AddSeconds(-1);
            }
            else if (button.Name == "look2")
            {
                date1 = datenow.AddDays(-1);
                date2 = datenow.AddSeconds(-1);
            }
            else if (button.Name == "look3")
            {
                date1 = datenow.AddDays(1 - datenow.Day);
                date2 = datenow.AddDays(1).AddSeconds(-1);
            }
            else if (button.Name == "look4")
            {
                date1 = new DateTime(datenow.Year, 1, 1);
                date2 = datenow.AddDays(1).AddSeconds(-1);
            }
            else if (button.Name == "export")
            {
                if (Data0.Count == 0)
                {
                    NewMessageBox.Show(LanguageManager.Instance["NoData"]);
                    return;
                }

                string rt = Export.ExportToExcel<T_MeasureData>(Data0);
                NewMessageBox.Show(rt);
                return;
            }
            else if (button.Name == "clear")
            {
                using (CodeFirstDbContext Entity = new CodeFirstDbContext())
                {
                    DbSet<T_MeasureData> data = Entity.T_MeasureData;
                    foreach (var d in data)
                    {
                        d.isclear = 1;
                        Entity.Entry(d).State = System.Data.Entity.EntityState.Modified;

                    }
                    Entity.SaveChanges();
                    Data0 = new ObservableCollection<T_MeasureData>(Entity.T_MeasureData.Where(p => p.isclear == 0));
                }
            }
            using (CodeFirstDbContext Entity = new CodeFirstDbContext())
            {
                Data0 = new ObservableCollection<T_MeasureData>(Entity.T_MeasureData.Where(p => p.isclear == 0 && p.MODIFYTIME <= date2 && p.MODIFYTIME >= date1).OrderByDescending(p => p.ID));
            }
            dic = Data0.ToDictionary(p => p.ID, p => p.Jsms);
            dicclms = Data0.ToDictionary(p => p.ID, p => p.Clms);
            GetMs();
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
