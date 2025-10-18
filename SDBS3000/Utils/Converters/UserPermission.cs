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
    /// 用户权限转换
    /// </summary>
    [ValueConversion(typeof(String), typeof(String))]
    public class UserPermission : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var userPermission = (SDBSEntity.UserPermissionEnum)System.Enum.Parse(typeof(SDBSEntity.UserPermissionEnum), value.ToString());
            return LanguageManager.Instance._resourceManager.GetString(userPermission.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
