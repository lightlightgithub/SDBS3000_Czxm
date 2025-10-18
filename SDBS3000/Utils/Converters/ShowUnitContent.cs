using SDBS3000.Resources;
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
    /// 量值显示单位    string[] yxldwdic = { "g", "mg", "g.mm", "g.cm" };
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
                    showunit = "[g]"; break;
                case 1:
                    showunit = "[mg]"; break;
                case 2:
                    showunit = "[g.mm]"; break;
                case 3:
                    showunit = "[g.cm]"; break;
                default:
                    showunit = "[g]"; break;

            }
            return showunit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    /// <summary>
    /// 尺寸单位  string[] ccdwdic = { "mm", "cm", "m", "inch", "foot" };
    /// </summary>
    public class UnitContent : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int show = System.Convert.ToInt32(value);
            string showunit = "";
            switch (show)
            {
                case 0:
                    showunit = "[mm]"; break;
                case 1:
                    showunit = "[cm]"; break;
                case 2:
                    showunit = "[m]"; break;
                case 3:
                    showunit = "[inch]"; break;
                default:
                    showunit = "[foot]"; break;

            }
            return showunit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
    /// <summary>
    /// 打印界面判断是否合格
    /// </summary>
    public class IsQualified : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string IsPass = System.Convert.ToString(value);
            string IsQualified = "";
            switch (IsPass)
            {
                case "OK":
                    IsQualified = "Qualified"; break;
                case "NG":
                    IsQualified = "Unqualified"; break;
                default:
                    IsQualified = ""; break;
            }
            return IsQualified;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
