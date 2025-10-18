using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SDBS3000.Utils.Converters
{
    /// <summary>
    /// 功能栏按钮修改
    /// </summary>
    [ValueConversion(typeof(String), typeof(String))]
    public class ToolCheckState : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string v = value.ToString();
            return v.Replace("未", "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
