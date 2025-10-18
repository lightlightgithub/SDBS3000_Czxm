using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SDBS3000.Utils.Converters
{
    public class GetEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            int ms =  System.Convert.ToInt32( parameter);
            int v = System.Convert.ToInt32(value);
            if (ms == 1)//软支撑1不能编辑  0硬支撑可
            {
                if (v == 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            else if (ms == 2)//当为静平衡时不能编辑，其他可以
            {
                if (v == 1)
                {
                    return Visibility.Hidden;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}