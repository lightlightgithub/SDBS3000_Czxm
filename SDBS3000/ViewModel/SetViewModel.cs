using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SDBS3000.Communicate.Servo;
using SDBS3000.Resources;
using SDBS3000.Views;
using SDBSEntity;
using System;
using System.Configuration;
using System.IO;
using System.Windows.Input;
using static SDBS3000.Log.Log;

namespace SDBS3000.ViewModel
{
    public class SetViewModel : ViewModelBase
    {
        ServoTrans svTrans = new ServoTrans();
        public SetViewModel()
        {
            
            Ki_max =MainViewModel.bal._runDB.set_test.ki_max;
            Refalsh = MainViewModel.bal._runDB.set_test.refalsh;
            Rev_difference = MainViewModel.bal._runDB.set_test.Rev_difference;
            WorkMode = MainViewModel.bal._runDB.set_test.WorkMode;
            PosMode = MainViewModel.bal._runDB.set_test.PosMode;

            Needle = MainViewModel.bal._runDB.set_test.Needle;
            SafeDoor = MainViewModel.bal._runDB.set_test.SafeDoor;

            Decimalnum = MainViewModel.bal._runDB.set_test.Decimalnum;
            Direction = MainViewModel.bal._runDB.set_test.Direction;
            Algorithm = MainViewModel.bal._runDB.set_test.Algorithm;
            DriverMode = MainViewModel.bal._runDB.set_run.drive_mode;
            DataSaveMode = MainViewModel.bal._runDB.set_test.DataSaveMode;
            PrintMode = MainViewModel.bal._runDB.set_test.PrintMode;
            MultSpeedCal = MainViewModel.bal._runDB.set_test.MultSpeedCal;
        }

        public int Ki_max { get; set; }
        public double Rev_difference { get; set; }
        public double Refalsh { get; set; }
        public double WorkMode { get; set; }
        public double PosMode { get; set; }

        public double Needle { get; set; }
        public double SafeDoor { get; set; }

        public double Decimalnum { get; set; }
        public double Direction { get; set; }

        public double Algorithm { get; set; }
        public double DriverMode { get; set; }
        public double DataSaveMode { get; set; }
        public double PrintMode { get; set; }
        public double MultSpeedCal { get; set; }

        public ICommand SetSave
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    try
                    {
                        Updatesqldata();
                        NewMessageBox.Show(LanguageManager.Instance["Saved"]);
                    }
                    catch (Exception e)
                    {
                        Write(LogType.ERROR, "转子信息下载至PLC异常：" + e.ToString());
                    }
                });
        }

        /// <summary>
        /// 校验是否保存后基于客户体验考虑不弹窗
        /// </summary>
        public ICommand SetSave1
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    try
                    {
                        Updatesqldata();                        
                    }
                    catch (Exception e)
                    {
                        Write(LogType.ERROR, "转子信息下载至PLC异常：" + e.ToString());
                    }
                });
        }

        /// <summary>
        /// 保存至配置文件
        /// </summary>
        public void Updatesqldata()
        {
            using (CodeFirstDbContext entity = new CodeFirstDbContext())
            {
                entity.Dictionarys.Find(GlobalVar.setItemKey["测量次数"]).value = Ki_max;
                entity.Dictionarys.Find(GlobalVar.setItemKey["刷新频率"]).value = Refalsh;
                entity.Dictionarys.Find(GlobalVar.setItemKey["转差范围"]).value = Rev_difference;
                entity.Dictionarys.Find(GlobalVar.setItemKey["工作方式"]).value = WorkMode;
                entity.Dictionarys.Find(GlobalVar.setItemKey["定位模式"]).value = PosMode;
                                           
                entity.Dictionarys.Find(GlobalVar.setItemKey["光针模式"]).value = Needle;
                entity.Dictionarys.Find(GlobalVar.setItemKey["安全门模式"]).value = SafeDoor;
                                           
                entity.Dictionarys.Find(GlobalVar.setItemKey["显示位数"]).value = Decimalnum;
                entity.Dictionarys.Find(GlobalVar.setItemKey["矢量图方向"]).value = Direction;
                                           
                entity.Dictionarys.Find(GlobalVar.setItemKey["算法选择"]).value = Algorithm;
                entity.Dictionarys.Find(GlobalVar.setItemKey["工件驱动模式"]).value = DriverMode;
                entity.Dictionarys.Find(GlobalVar.setItemKey["数据保存方式"]).value = DataSaveMode;
                entity.Dictionarys.Find(GlobalVar.setItemKey["打印模式"]).value = PrintMode;
                int i = entity.SaveChanges();
                MainViewModel.bal._runDB.set_test.ki_max = Ki_max;
                MainViewModel.bal._runDB.set_test.refalsh = Refalsh;
                MainViewModel.bal._runDB.set_test.Rev_difference = Rev_difference;
                MainViewModel.bal._runDB.set_run.drive_mode = (int)DriverMode;
                MainViewModel.bal._runDB.set_test.WorkMode = (int)WorkMode;

                MainViewModel.bal._runDB.set_test.PosMode = PosMode;

                MainViewModel.bal._runDB.set_test.Needle = Needle;
                MainViewModel.bal._runDB.set_test.SafeDoor = SafeDoor;
                MainViewModel.bal._runDB.set_test.Decimalnum = Decimalnum;
                MainViewModel.bal._runDB.set_test.Direction = Direction;
                MainViewModel.bal._runDB.set_test.Algorithm = Algorithm;
                MainViewModel.bal._runDB.set_test.DataSaveMode = DataSaveMode;
                MainViewModel.bal._runDB.set_test.PrintMode = PrintMode;
                MainViewModel.bal._runDB.set_test.MultSpeedCal = MultSpeedCal;
                //MainViewModel.svTrans.Balstart(1);

                Messenger.Default.Send(Decimalnum, "decnum");
                Messenger.Default.Send(Direction, "direction");
                Messenger.Default.Send(WorkMode, "workmode");
        
                
            }
        }


    }

}
