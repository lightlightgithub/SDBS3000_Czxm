using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SDBS3000.Ucs
{
    public class PasswordTextBox : TextBox
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        /// <summary>
        /// 密码修饰符
        /// </summary>
        public string PasswordChar
        {
            get { return (string)GetValue(PasswordCharProperty); }
            set { SetValue(PasswordCharProperty, value); }
        }

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool Encrypt
        {
            get { return (bool)GetValue(EncryptProperty); }
            set
            {
                SetValue(EncryptProperty, value);
            }
        }

        /// <summary>
        /// 水印
        /// </summary>
        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set
            {
                SetValue(PlaceHolderProperty, value);
            }
        }

        /// <summary>
        /// 记住切换明文和密文之前，上一次插入符所在的位置
        /// </summary>
        int lastSelectionStart;

        public static DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordTextBox), new PropertyMetadata(""));
        public static DependencyProperty PasswordCharProperty = DependencyProperty.Register("PasswordChar", typeof(string), typeof(PasswordTextBox), new PropertyMetadata("*"));
        public static DependencyProperty EncryptProperty = DependencyProperty.Register("Encrypt", typeof(bool), typeof(PasswordTextBox), new PropertyMetadata(true, Changed));
        public static DependencyProperty PlaceHolderProperty = DependencyProperty.Register("PlaceHolder", typeof(string), typeof(PasswordTextBox), new PropertyMetadata(""));

        /// <summary>
        /// 当Encrypt改变时记住插入符在的位置
        /// 在Encrypt改变后，肯定会触发下一次textchanged，因为text的内容改变了
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordTextBox box = d as PasswordTextBox;
            box.lastSelectionStart = box.SelectionStart;
            box.TextChanged += Box_TextChanged;
        }

        /// <summary>
        /// 在文字改变之后，将插入符位置复位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordTextBox box = sender as PasswordTextBox;
            box.SelectionStart = box.lastSelectionStart == 0 ? 1 : box.lastSelectionStart;
            box.TextChanged -= Box_TextChanged;
        }

        public PasswordTextBox()
        {
            this.PreviewKeyDown += PasswordTextBox_PreviewKeyDown;
            //this.InputMethod.PreferredImeState = "Off";
            //this.IsInputMethodEnabled = false;
            //this.input
            MultiBinding mutiBuilding = new MultiBinding();
            mutiBuilding.Converter = new PasswordConverter();
            mutiBuilding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            mutiBuilding.Bindings.Add(new Binding() { Path = new PropertyPath("Password"), Source = this });
            mutiBuilding.Bindings.Add(new Binding() { Path = new PropertyPath("PasswordChar"), Source = this });
            mutiBuilding.Bindings.Add(new Binding() { Path = new PropertyPath("Encrypt"), Source = this });
            BindingOperations.SetBinding(this, TextProperty, mutiBuilding);
        }

        private void PasswordTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
