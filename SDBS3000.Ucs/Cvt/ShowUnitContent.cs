using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SDBS3000.Ucs.Cvt
{
    /// <summary>
    /// 量值显示单位
    /// </summary>
    public class ShowUnitContent : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int show = (int)value;
            string showunit = "";
            switch (show)
            {
                case 0:
                    showunit = "量值[g]";break;
                case 1:
                    showunit = "量值[mg]"; break;
                case 2:
                    showunit = "量值[g.mm]"; break;
                case 3:
                    showunit = "量值[g.cm]"; break;
                default:
                    showunit = "量值[g]"; break;

            }
            return showunit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
