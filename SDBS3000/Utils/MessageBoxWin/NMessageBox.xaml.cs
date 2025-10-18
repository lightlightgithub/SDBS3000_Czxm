using SDBS3000.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SDBS3000
{
    /// <summary>
    /// 消息对话框按钮样式
    /// </summary>
    public enum ButtonStyle
    {
        NormalButtonStyle = 0,
        NotNormalButtonStyle = 1
    }
    /// <summary>
    /// NewMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class NMessageBox : Window
    {        
        #region 成员
        private Style normalButtonStyle;

        private Style notNormalButtonStyle;
        #endregion

        #region 属性
        public string MessageBoxTitle
        {
            get;
            set;
        }

        public string MessageBoxText
        {
            get;
            set;
        }

        public string ImagePath
        {
            get;
            set;
        }

        public Visibility OKButtonVisibility
        {
            get;
            set;
        }

        public Visibility CancelButtonVisibility
        {
            get;
            set;
        }

        public Visibility YesButtonVisibility
        {
            get;
            set;
        }

        public Visibility NoButtonVisibility
        {
            get;
            set;
        }

        public ButtonStyle OKButtonStyle
        {
            set
            {
                if (value == ButtonStyle.NormalButtonStyle)
                {
                    OKButton.Style = normalButtonStyle;
                }
                else if (value == ButtonStyle.NotNormalButtonStyle)
                {
                    OKButton.Style = notNormalButtonStyle;
                }
            }
        }

        public ButtonStyle CancelButtonStyle
        {
            set
            {
                if (value == ButtonStyle.NormalButtonStyle)
                {
                    CancelButton.Style = normalButtonStyle;
                }
                else if (value == ButtonStyle.NotNormalButtonStyle)
                {
                    CancelButton.Style = notNormalButtonStyle;
                }
            }
        }

        public ButtonStyle YesButtonStyle
        {
            set
            {
                if (value == ButtonStyle.NormalButtonStyle)
                {
                    YesButton.Style = normalButtonStyle;
                }
                else if (value == ButtonStyle.NotNormalButtonStyle)
                {
                    YesButton.Style = notNormalButtonStyle;
                }
            }
        }

        public ButtonStyle NoButtonStyle
        {
            set
            {
                if (value == ButtonStyle.NormalButtonStyle)
                {
                    
                    NoButton.Style = normalButtonStyle;
                }
                else if (value == ButtonStyle.NotNormalButtonStyle)
                {
                    NoButton.Style = notNormalButtonStyle;
                }
            }
        }

        public CMessageBoxResult Result;
        #endregion

        #region 单例模式
        private NMessageBox() // 必须是私有的构造函数，这样就可以确保该类无法通过new来创建该类的实例。
        {
            InitializeComponent();
            this.DataContext = this;
            // 消息提示 MessageBoxTitle = "消息提示";
            MessageBoxTitle = LanguageManager.Instance["NewTip"];
            OKButtonVisibility = System.Windows.Visibility.Collapsed;
            CancelButtonVisibility = System.Windows.Visibility.Collapsed;
            YesButtonVisibility = System.Windows.Visibility.Collapsed;
            NoButtonVisibility = System.Windows.Visibility.Collapsed;

            normalButtonStyle = this.FindResource("NormalButtonStyle") as Style;
            notNormalButtonStyle = this.FindResource("NotNormalButtonStyle") as Style;

            Result = CMessageBoxResult.None;
        }

        static NMessageBox boxsin;  // 定义一个私有的静态全局变量来保存该类的唯一实例。

        public static NMessageBox Getbox() // 全局访问点，设置为静态方法则可在外边无需创建该类的实例就可调用该方法。
        {
            boxsin = boxsin == null ? new NMessageBox() : boxsin;
            return boxsin;
        }
        #endregion

        #region 事件
        private void OnMouseLeftButtonDownAtTitlee(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {            
            Result = CMessageBoxResult.OK;
            this.Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = CMessageBoxResult.Yes;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = CMessageBoxResult.No;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = CMessageBoxResult.Cancel;
            this.Close();
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Result = CMessageBoxResult.None;
            this.Close();
        }

        private void MinWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void Win_Loaded(object sender, RoutedEventArgs e)
        {
            BlurEffect effect = new BlurEffect();
            effect.Radius = 3;
            effect.KernelType = KernelType.Gaussian;
            Application.Current.Windows[0].Effect = effect;
        }

        private void Win_Closed(object sender, EventArgs e)
        {
            if (Application.Current.Windows.Count > 0)
            {
                Application.Current.Windows[0].Effect = null;
            }
            boxsin = null;
            this.Close();
        }
        #endregion

        private void CmdBinding_Enter(object sender, ExecutedRoutedEventArgs e)
        {
            OKButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Y(object sender, ExecutedRoutedEventArgs e)
        {
            YesButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_N(object sender, ExecutedRoutedEventArgs e)
        {
            NoButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Esc(object sender, ExecutedRoutedEventArgs e)
        {
            CancelButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void CmdBinding_Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            CloseWindowBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}
