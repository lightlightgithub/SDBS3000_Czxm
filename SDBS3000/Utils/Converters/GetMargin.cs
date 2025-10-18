using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SDBS3000.Utils.Converters
{

    public class GetMargin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            try
            {
                int zcfs = System.Convert.ToInt32(value);
                int ctr = System.Convert.ToInt32(parameter);
                Thickness[,] ts = new Thickness[11, 6] {
                {new Thickness(746, 264, 894, 386),new Thickness(989,264,651,386),new Thickness(1228,264,412,386),new Thickness(567,400,1073,243),new Thickness(1400,416,240,232) ,new Thickness(873,460,575,175)}
                ,{new Thickness(802,314,838,337),new Thickness(987,264,653,386),new Thickness(1168,313,472,337),new Thickness(617,422,1023,222),new Thickness(1350,418,290,232),new Thickness(881,476,567,159) }
                ,{new Thickness(854,268,786,381),new Thickness(0),new Thickness(1140,268,500,381),new Thickness(567,390,1073,260),new Thickness(0),new Thickness(897,446,551,177) }
                ,{new Thickness(858,304,782,344),new Thickness(1009,260,631,390),new Thickness(1164,305,476,345),new Thickness(613,414,1027,229),new Thickness(1358,416,282,234),new Thickness(905,472,543,163) }
               //模式5
                    ,{new Thickness(804,304,836,344),new Thickness(959,260,681,390),new Thickness(1110,305,530,345),new Thickness(613,414,1027,229),new Thickness(1358,416,282,228),new Thickness(873,472,575,163) }
                ,{new Thickness(0),new Thickness(0),new Thickness(0),new Thickness(904,274,736,376),new Thickness(0),new Thickness(918,363,592,267) }
                ,{new Thickness(951,566,689,84),new Thickness(887,258,753,392),new Thickness(1141,258,499,391),new Thickness(625,363,1015,282),new Thickness(1344,368,296,261),new Thickness(845,412,603,215) }
                ,{new Thickness(828,258,812,392),new Thickness(1081,258,559,392),new Thickness(1017,564,623,84),new Thickness(625,375,1015,268),new Thickness(1344,362,296,288),new Thickness(931,412,517,218) }
                //模式9
                    ,{new Thickness(934,323,706,328),new Thickness(0),new Thickness(993,258,647,390),new Thickness(655,423,985,227),new Thickness(0),new Thickness(951,470,497,160) }
                ,{new Thickness(943,259,697,391),new Thickness(0),new Thickness(999,324,641,326),new Thickness(1281,423,359,227),new Thickness(0),new Thickness(795,470,653,160) }
                ,{new Thickness(752,432,888,216),new Thickness(1217,392,423,258),new Thickness(1215,474,425,176),new Thickness(899,262,741,388),new Thickness(912,431,728,219),new Thickness(920,341,590,289) }
            };
                return ts[zcfs, ctr];
            }
            catch
            {
                return new Thickness(0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}