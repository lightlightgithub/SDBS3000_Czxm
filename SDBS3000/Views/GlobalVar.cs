using GalaSoft.MvvmLight.Messaging;
using SDBS3000.Log;
using SDBS3000.ViewModel;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using static SDBS3000.Log.Log;

namespace SDBS3000.Views
{
    public static class GlobalVar
    {
        public static bool iskey;

        public static double doublehei;
        public static double doublewit;
        public static double scale;

        public static Main mainWindow;

        public static int? FormulaResolvingMode;

        public static T_USER user;

        public static int jsms;
        public static string portcjb = "COM1";
        public static string portkzb = "COM2";

        public static string Str = "";

        public static ushort runmode;

        public static ViewModel.MainViewModel main;
        public static ViewModel.MeasureViewModel meaVM;


        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


        private static int flagofSFSpeed;
        public static int FlagofSFSpeed
        {
            get { return flagofSFSpeed; }
            set
            {
                if (flagofSFSpeed != value)
                {
                    flagofSFSpeed = value;
                }
            }
        }
        private static string rtname;

        public static string Rtname
        {
            get { return rtname; }
            set
            {
                if (rtname != value)
                {
                    rtname = value;  }                
            }
        }

        private static RotorStruct1 rotorStruct1;

        public static RotorStruct1 RotorStruct1
        {
            get { return rotorStruct1; }
            set
            {
                if (!rotorStruct1.Equals(value))
                {
                    rotorStruct1 = value;
                }
            }
        }

        private static RotorStruct2 rotorStruct2;

        public static RotorStruct2 RotorStruct2
        {
            get { return rotorStruct2; }
            set
            {
                if (!rotorStruct2.Equals(value))
                {
                    rotorStruct2 = value;
                }
            }
        }


        private static Set set;
        /// <summary>
        /// 设置界面对应数据
        /// </summary>
        public static Set Set
        {
            get { return set; }
            set
            {
                if (!set.Equals(value))
                {
                    set = value;
                }
            }
        }

        private static Clamp_Key clamp_Key;

        public static Clamp_Key Clamp_Key
        {
            get { return clamp_Key; }
            set { clamp_Key = value; }
        }
        /// <summary>
        /// 定标系数
        /// </summary>
        private static SetCal1 setCal1;

        public static SetCal1 SetCal1
        {
            get { return setCal1; }
            set { setCal1 = value; }
        }

        private static Setcal setcal;

        public static Setcal Setcal
        {
            get { return setcal; }
            set
            {
                setcal = value;
                try
                {
                    byte[] bt1 = new byte[48];
                    byte[] bt2 = new byte[48];
                    for (int i = 0; i < 12; i++)
                    {
                        byte[] bytedata = BitConverter.GetBytes(value.caldata[i]);
                        Array.Reverse(bytedata);
                        bt1[i * 4] = bytedata[0];
                        bt1[i * 4 + 1] = bytedata[1];
                        bt1[i * 4 + 2] = bytedata[2];
                        bt1[i * 4 + 3] = bytedata[3];
                        bt2[i * 4] = bytedata[4];
                        bt2[i * 4 + 1] = bytedata[5];
                        bt2[i * 4 + 2] = bytedata[6];
                        bt2[i * 4 + 3] = bytedata[7];
                    }
                    //回写两组感量角度
                    Set0_val set0struct = new Set0_val();
                    set0struct.val0 = value.set0[0];
                    set0struct.val1 = value.set0[1];
                    set0struct.val2 = value.set0[2];
                    set0struct.val3 = value.set0[3];
                }
                catch (Exception ex)
                {
                    Write(LogType.ERROR, "标定数据传输至PLC异常：" + ex.Message);
                };
            }
        }

        public static Dictionary<string, bool> setItemActive;
        public static Dictionary<string, int> setItemKey;

        public static ushort Getushort(string setname)
        {
            try
            {
                return Convert.ToUInt16(config.AppSettings.Settings[setname].Value);
            }
            catch
            {
                return 0;
            }
        }

        public static Single GetSingle(string setname)
        {
            try
            {
                return Convert.ToSingle(config.AppSettings.Settings[setname].Value);
            }
            catch
            {
                return 0;
            }
        }

        public static string GetStr(string setname)
        {
            try
            {
                return config.AppSettings.Settings[setname].Value.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
