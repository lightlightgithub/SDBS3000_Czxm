using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SDBS3000.Resources;
using SDBS3000.Utils.AppSettings;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace SDBS3000.ViewModel
{
    public class UserManagementViewModel : ViewModelBase
    {
        public UserManagementViewModel()
        {
            AddFlag = false;
            ModifyFlag = false;
            using (CodeFirstDbContext Entity = new CodeFirstDbContext())
            {
                Users = new ObservableCollection<T_USER>(Entity.T_Users);
                User = Entity.T_Users.First(p => p.NAME == GlobalVar.user.NAME);
                //currentUser = CloneExtend.DeepClone(Entity.T_Users.First(p => p.NAME == GlobalVariable.username));
            }
            UserPermissionEnum = Enum.GetNames(typeof(SDBSEntity.UserPermissionEnum)).ToArray();
        }

        private string[] userPermissionEnum;

        public string[] UserPermissionEnum
        {
            get { return userPermissionEnum; }
            set { Set(ref userPermissionEnum, value); }
        }

        private ObservableCollection<T_USER> users;

        public ObservableCollection<T_USER> Users
        {
            get { return users; }
            set
            {
                Set(ref users, value);
            }
        }

        private T_USER user;

        public T_USER User
        {
            get { return user; }
            set
            {
                if (isEditing)
                {
                    if (user.ID != value.ID)
                    {
                        AddFlag = false;
                        ModifyFlag = false;
                        using (CodeFirstDbContext Entity = new CodeFirstDbContext())
                        {
                            Users = new ObservableCollection<T_USER>(Entity.T_Users);
                            User = Entity.T_Users.First(p => p.ID == value.ID);
                        }
                    }
                    else
                    {
                        Set(ref user, value);
                    }
                }
                else
                {
                    Set(ref user, value);
                }
            }
        }

        private bool addFlag;

        public bool AddFlag
        {
            get { return addFlag; }
            set
            {
                addFlag = value;
                IsEditing = value || ModifyFlag;
            }
        }

        private bool modifyFlag;

        public bool ModifyFlag
        {
            get { return modifyFlag; }
            set
            {
                modifyFlag = value;
                IsEditing = value /*|| AddFlag*/;
            }
        }

        private bool isEditing;

        public bool IsEditing
        {
            get { return isEditing; }
            set { Set(ref isEditing, value); }
        }


        public ICommand Add
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    User = new T_USER();
                    AddFlag = true;
                });
        }

        public ICommand Modify
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    ModifyFlag = true;
                });
        }

        public ICommand Delete
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    if (User.ID < 4)
                    {
                        NewMessageBox.Show(LanguageManager.Instance["Presetdata"]);
                        return;
                    }
                    var back = NewMessageBox.Show(LanguageManager.Instance["DelCurrentUser"], LanguageManager.Instance["Alert"], CMessageBoxButton.YesNO, CMessageBoxImage.None).ToString();
                    if (back == "Yes")
                    {
                        CodeFirstDbContext Entity = new CodeFirstDbContext();
                        try
                        {
                            var userDelete = Entity.T_Users.FirstOrDefault(p => p.ID == User.ID);
                            if (userDelete != null)
                            {
                                Entity.T_Users.Remove(userDelete);
                                int i = Entity.SaveChanges();
                                if (i > 0)
                                {
                                    Users = new ObservableCollection<T_USER>(Entity.T_Users);
                                    User = null;
                                    //NewMessageBox.Show("删除成功", "用户删除", CMessageBoxButton.OK, CMessageBoxImage.None).ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            NewMessageBox.Show(e.InnerException.Message, "用户保存", CMessageBoxButton.OK, CMessageBoxImage.None).ToString();
                        }
                    }
                });
        }

        public ICommand Save
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    if (string.IsNullOrEmpty(User.NAME) || string.IsNullOrEmpty(User.PSD) || string.IsNullOrEmpty(User.PERMISSION.ToString()))
                    {
                        NewMessageBox.Show(LanguageManager.Instance["TheUsername"]);
                        return;
                    }
                    CodeFirstDbContext Entity = new CodeFirstDbContext();
                    try
                    {
                        int i;
                        if (ModifyFlag)
                        {
                            Entity.Entry(User).State = EntityState.Modified;
                            Entity.Entry(User).CurrentValues.SetValues(User);
                            i = Entity.SaveChanges();
                            if (i == 1)
                            {
                                ModifyFlag = false;
                            }
                        }
                        else if (AddFlag)
                        {
                            Entity.T_Users.Add(User);
                            i = Entity.SaveChanges();
                            if (i == 1)
                            {
                                AddFlag = false;
                                Users = new ObservableCollection<T_USER>(Entity.T_Users);
                            }
                        };
                    }
                    catch (Exception e)
                    {
                        NewMessageBox.Show(e.InnerException.Message, "用户保存", CMessageBoxButton.OK, CMessageBoxImage.None).ToString();
                    }
                    finally
                    {
                        Entity.Dispose();
                    }
                }
                );
        }
    }
}
