using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SDBS3000.Ucs
{
    public class PasswordConverter : IMultiValueConverter
    {
        string realPassword = "";
        string realPasswordChar = "*";
        bool encryptPawssword = true;

        /// <summary>
        /// 由返回值可以看出这是将绑定的值转化为最终显示的Text
        /// </summary>
        /// <param name="value">value的值顺序就和绑定的顺序一样</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        object IMultiValueConverter.Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder();
            if (value != null && value.Length >= 1&& value[0]!=null)
            {
                string sValue = value[0].ToString();
                bool encrypt = (bool)value.Where(v => v.GetType() == typeof(bool)).FirstOrDefault();
                encryptPawssword = encrypt;

                realPassword = sValue;

                if (value.Length > 1 && encrypt)
                {
                    char passChar = value[1].ToString()[0];
                    realPasswordChar = passChar.ToString();
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        for (int i = 0; i < sValue.Length; i++)
                        {
                            sb.Append(passChar);
                        }
                    }

                    return sb.ToString();
                }
                else
                {
                    return sValue;
                }
            }

            return null;
        }

        /// <summary>
        /// 将Text的值传进来，转化为绑定数值
        /// 这里需要注意，如果PasswordChar为*，那value的值就是*****x
        /// 假设已经输入了五位数，第六个数量入了g，那么value就是*****g
        /// 所以在此类需要保存真正的密码
        /// 举个例子 前五个你输入了12345，第六个输入了g
        /// 那么value的值就是*****g，全局realPassword值为12345
        /// 此时需要将g交给realPassword
        /// 如果你是在12后面插入的g，那valu就是**g***
        /// 所以需要将**g***和realPassword的位置做对比，将g插在第三位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string backValue = "";
            if (value != null)
            {
                string strValue = value.ToString();

                if (encryptPawssword)
                {
                    char replaceChar = realPasswordChar[0];
                    //这里需要注意一下，原作者这里用的是realPassword索引backValue+=realPassword[index]
                    //如果这么做的话，正常输入密码没问题，但是在密码中间插入密码，就会出现数组越界
                    IEnumerator<char> realChars = realPassword.GetEnumerator();
                    for (int index = 0; index < strValue.Length; ++index)
                    {
                        if (strValue[index] == replaceChar)
                        {
                            if (realChars.MoveNext()) backValue += realChars.Current;
                        }
                        else
                        {
                            backValue += strValue[index];
                        }
                    }

                    strValue = backValue;
                }

                return new object[] { strValue, realPasswordChar, encryptPawssword };
            }

            return null;
        }
    }


}
