using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SDBS3000.Resources;
using SDBS3000.Utils.AppSettings;
using SDBS3000.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SDBS3000.ViewModel
{
    public class SingleStepViewModel:ViewModelBase
    {

        public SingleStepViewModel()
        {
          

        }


        /// <summary>
        /// 电机急停
        /// </summary>
        public ICommand mStop
        {
            get => new RelayCommand<object>(async obj =>
            {
                try
                {
                    var (success, code) = await MainViewModel.macControl.ServoStopAsync(MainViewModel.bal._runDB.set_run.drive_mode, 1, 0);
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（电机急停）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（电机急停）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（电机急停）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（电机急停）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（电机急停）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（电机急停）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（电机急停）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("电机急停命令执行异常: " + ex.Message);
                }
                
            });
        }

        /// <summary>
        /// 电机复位
        /// </summary>
        public ICommand mReset
        {
            get => new RelayCommand<object>(async obj =>
            {
                try
                {
                    var (success, code) = await MainViewModel.macControl.AlarmResetAsync();
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（电机复位）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（电机复位）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（电机复位）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（电机复位）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（电机复位）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（电机复位）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（电机复位）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("电机复位命令执行异常: " + ex.Message);
                }
              
            });
        }

        /// <summary>
        /// 气缸松开
        /// </summary>
        public ICommand cRelease
        {
            get => new RelayCommand<object>(async obj =>
            {
                try
                {
                    var (success, code) = await MainViewModel.macControl.CylinderCRAsync(false);
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（气缸松开）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（气缸松开）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（气缸松开）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（气缸松开）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（气缸松开）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（气缸松开）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（气缸松开）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("气缸松开命令执行异常: " + ex.Message);
                }
            });
        }

        /// <summary>
        /// 气缸夹紧
        /// </summary>
        public ICommand cClamp
        {
            get => new RelayCommand<object>(async obj =>
            {
                try
                {
                    var (success, code) = await MainViewModel.macControl.CylinderCRAsync(true);
                    string resultStr;
                    if (!success)
                    {
                        switch (code)
                        {
                            case 0x02:
                                resultStr = "气缸异常（气缸夹紧）";
                                break;
                            case 0x03:
                                resultStr = "伺服异常（气缸夹紧）";
                                break;
                            case 0x04:
                                resultStr = "伺服报警（气缸夹紧）";
                                break;
                            case 0x05:
                                resultStr = "伺服忙（气缸夹紧）";
                                break;
                            case 0x06:
                                resultStr = "伺服未启动（气缸夹紧）";
                                break;
                            case 0xFF:
                                resultStr = "等待响应超时（气缸夹紧）";
                                break;
                            default:
                                resultStr = $"未知错误 (Code: {code:X2})（气缸夹紧）";
                                break;
                        }
                        GlobalVar.Str = resultStr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("气缸夹紧命令执行异常: " + ex.Message);
                }
            });
        }

     

    }
}
