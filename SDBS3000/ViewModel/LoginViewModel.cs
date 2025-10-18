using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SDBS3000.Resources;
using SDBS3000.Views;
using SDBSEntity;
using SDBSEntity.Model;

namespace SDBS3000.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {

        CodeFirstDbContext db;
        Configuration config;
        List<T_USER> t_Users;
        public List<T_USER> Userlist
        {
            get => t_Users;
            set => t_Users = value;
        }

        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                Set(ref index, value);
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                Set(ref name, value);
            }
        }

        private string psd;

        public string Psd
        {
            get { return psd; }
            set
            {
                Set(ref psd, value);
            }
        }

        private string remember;

        public string Remember
        {
            get { return remember; }
            set
            {
                Set(ref remember, value);
            }
        }

        public LoginViewModel()
        {
            db = new CodeFirstDbContext();
            try
            {
                if (db.T_Users.Where(p => p.NAME == "Administrator").Count() == 0)
                {
                    T_USER u = new T_USER
                    {
                        NAME = "Administrator",
                        PSD = "3",
                        PERMISSION = 0,
                        NOTE = "备注",
                        MODIFYTIME = DateTime.Now
                    };
                    db.T_Users.Add(u);
                    db.SaveChanges();
                }
                Userlist = db.T_Users.ToList();

                // 集合排序
                // Userlist = db.T_Users.OrderBy(p => p.ModifyTime).ToList();
                string rememberpsd = ConfigurationManager.AppSettings["rememberpsd"];
                if (rememberpsd != null)
                {
                    Name = ConfigurationManager.AppSettings["lastuser"];
                    GlobalVar.portcjb = ConfigurationManager.AppSettings["COM1"];
                    GlobalVar.portkzb = ConfigurationManager.AppSettings["COM2"];                    

                    Index = Userlist.FindIndex(p => p.NAME == Name);
                    Remember = rememberpsd;
                    Psd = "";
                    if (Remember.ToLower() == "true")
                    {
                        if (Index > -1 && Index < Userlist.Count)
                        {
                            Psd = Userlist[Index].PSD;
                        }
                    }
                }

                Name = "Administrator";

                // 添加授权验证
                //AppLicense();
            }
            catch (Exception e)
            {
                NewMessageBox.Show(LanguageManager.Instance["Exception"] + e.Message);
            }
        }



        public ICommand LoginBtn
        {
            get => new RelayCommand<object>(
               obj =>
               {
                   Login l = obj as Login;
                   if (Name == "000")
                   {
                       GlobalVar.user = new T_USER() { NAME = "000", PERMISSION = 2 };

                       Main main = new Main();
                       main.Show();
                       l.Close();
                       return;
                   }
                   else if (Userlist.Find(p => p.NAME == Name && p.PSD == this.Psd) != null)
                   {
                       config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                       config.AppSettings.Settings["rememberpsd"].Value = Remember;
                       config.Save(ConfigurationSaveMode.Modified);
                       GlobalVar.user = Userlist.Find(p => p.NAME == Name && p.PSD == this.Psd);
                       Main main = new Main();
                       main.Show();
                       l.Close();
                   }
                   else if (Name is null)//调试用
                   {
                       GlobalVar.user = Userlist.Find(p => p.NAME == "Administrator");
                       Main main = new Main();
                       main.Show();
                       l.Close();
                   }
                   else
                   {
                       NewMessageBox.Show(LanguageManager.Instance["FailedToLogin"], LanguageManager.Instance["Error"], CMessageBoxButton.OK, CMessageBoxImage.Warning);
                   }
               });
        }

        public ICommand CloseBtn
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    Login l = obj as Login;
                    l?.Close();
                });
        }
    }
}