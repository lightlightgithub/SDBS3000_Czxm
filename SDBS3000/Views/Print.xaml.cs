using System;
using System.Configuration;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PrintDialog = System.Windows.Controls.PrintDialog;

namespace SDBS3000.Views
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Print : Window
    {
        PrintDialog printDialog = new PrintDialog();

        public static Print print = null;

        public static Print GetPrint()
        {
            if (print is null)
                print = new Print();
            print.Activate();
            return print;
        }

        private Print()
        {
            InitializeComponent();
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }


        private string title1 = "测1试";

        public string Title1
        {
            get { return title1; }
            set { title1 = value; }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            QuickPrint();
        }

        //orientation == System.Printing.PageOrientation.Portrait   纵向
        //orientation == System.Printing.PageOrientation.Landscape  横向

        public void QuickPrint()
        {
            LocalPrintServer printServer = new LocalPrintServer();
            //PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local });
            printDialog.PrintQueue = printServer.DefaultPrintQueue;
            if (printServer.DefaultPrintQueue.FullName == "printer")
            {
                var szie = printDialog.PrintTicket.PageMediaSize;
                printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
                printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;

                PrintVisualAutoFitPage(printcontrol);
            }
        }

        public static void AutoPrint()
        {

        }

        public bool PrintVisualAutoFitPage(FrameworkElement source)
        {
            PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);

            if (capabilities != null)
            {
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / source.ActualWidth,
                    capabilities.PageImageableArea.ExtentHeight / source.ActualHeight);

                Transform oldTransform = source.LayoutTransform;
                var oldSize = new System.Windows.Size(source.ActualWidth, source.ActualHeight);

                //Transform the Visual to scale
                this.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                //this.Measure(sz);
                //this.Arrange(new Rect(new System.Windows.Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.
                printDialog.PrintVisual(source, "PrintVisualAutoFitPage");

                source.LayoutTransform = oldTransform;
                source.Measure(oldSize);
                return true;
            }
            return false;
        }


        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            string file = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;      //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";     //弹窗的标题
            dialog.InitialDirectory = Directory.GetCurrentDirectory() + @"\config";       //默认打开的文件夹的位置
            dialog.Filter = "图片(*.gif;*.jpg;*.jpeg;*.bmp;*.jfif;*.png)|*.gif;*.jpg;*.jpeg;*.bmp;*.jfif;*.png";       //筛选文件
            dialog.ShowHelp = true;     //是否显示“帮助”按钮

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["url"].Value = file;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                printcontrol.logo.Source = (ImageSource)new BitmapImage(new Uri(file));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            print = null;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            string file = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = "请选择文件";
            dialog.InitialDirectory = Directory.GetCurrentDirectory() + @"\config";
            dialog.Filter = "图片(*.gif;*.jpg;*.jpeg;*.bmp;*.jfif;*.png)|*.gif;*.jpg;*.jpeg;*.bmp;*.jfif;*.png";
            dialog.ShowHelp = true;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["url2"].Value = file;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                printcontrol.logo2.Source = (ImageSource)new BitmapImage(new Uri(file));
            }
        }
    }
}
