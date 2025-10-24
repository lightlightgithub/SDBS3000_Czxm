using SDBS3000.Utils.AppSettings;
using System;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SDBS3000.Views
{
    public partial class UserControl580 : UserControl, IComponentConnector
    {
        public UserControl580()
        {
            this.InitializeComponent();
            this.time.Content = (object)DateTime.Now.ToString();
            this.ope.Content = (object)GlobalVar.user;
            try
            {
                this.logo.Source = (ImageSource)new BitmapImage(new Uri(ConfigurationManager.AppSettings["url"].ToString()));
                this.logo2.Source = (ImageSource)new BitmapImage(new Uri(ConfigurationManager.AppSettings["url2"].ToString()));
            }
            catch
            {
            }
        }
    }
}
