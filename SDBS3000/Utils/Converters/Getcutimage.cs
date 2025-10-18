using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SDBS3000.Utils.Converters
{
    public class Getcutimage : IValueConverter
    {
        #region Converter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool cutmode = (bool)value;
            if (cutmode)
            {
                return new BitmapImage(new Uri("/SDBS3000;component/Utils/Image/Measure/去重.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                return new BitmapImage(new Uri("/SDBS3000;component/Utils/Image/Measure/加重.png", UriKind.RelativeOrAbsolute));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

    public class Getcutimage1 : IValueConverter
    {
        #region Converter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool cutmode = (bool)value;
            if (cutmode)
            {
                return new BitmapImage(new Uri("/SDBS3000;component/Utils/Image/Measure/去重0.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                return new BitmapImage(new Uri("/SDBS3000;component/Utils/Image/Measure/加重0.png", UriKind.RelativeOrAbsolute));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
