using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SDBS3000.Utils.Converters
{
    public class GetSptUrl : IValueConverter
    {
        #region 
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (value is null)
                return "/SDBS3000;component/Utils/Image/SupMet/stm1.png";
            int i = (int)value + 1;
            //if (Views.GlobalVar.jsms != 1)
            return "/SDBS3000;component/Utils/Image/SupMet/stm" + i.ToString() + ".png";
            //else
            //    return "/SDBS3000;component/Utils/Image/SupMet/stmr" + i.ToString() + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
