using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SDBS3000.Utils.Converters
{
    public class GetVisFromBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            try
            {
                bool b = (bool)value;
                if (b)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
            catch
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    //两圆传1，静传2
    public class GetVisFromPrint : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            try
            {
                int b = (int)value;
                int p = System.Convert.ToInt32(parameter);
                if (p == 1 && b != 1)
                    return Visibility.Visible;
                if (p == 2 && b == 1)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
            catch
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
