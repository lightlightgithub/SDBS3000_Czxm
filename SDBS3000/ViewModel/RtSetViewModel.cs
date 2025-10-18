using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NPOI.POIFS.FileSystem;
using SDBS3000.Resources;
using SDBS3000.Ucs.Controls;
using SDBS3000.Views;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace SDBS3000.ViewModel
{
    public class RtSetViewModel : ViewModelBase
    {
        string[] jsmsdic = { "硬支撑", "软支撑" };
        string[] clmsdic = { "双面动平衡", "静平衡", "动静平衡", "立式双面动平衡", "立式动静平衡" };
        string[] ccdwdic = { "mm", "cm", "m", "inch", "foot" };
        string[] yxldwdic = { "g", "mg", "g.mm", "g.cm" };
        public RtSetViewModel()
        {
            GlobalVar.config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            using (CodeFirstDbContext Entity = new CodeFirstDbContext())
            {
                RotorSets = new ObservableCollection<T_RotorSet>(Entity.T_RotorSet);
                if (GlobalVar.main.Rotor is null || RotorSets.Count == 0)
                    return;
                Rotor = Entity.T_RotorSet.First(p => p.ID == GlobalVar.main.Rotor.ID);
                Zcfs = Rotor.Zcfs;
            }
        }

        #region 支撑方式界面绑定
        public ICommand CloseSupportMethod
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    SupportMethod supportMethod = obj as SupportMethod;
                    supportMethod.Close();
                });
        }

        private int zcfs;

        public int Zcfs
        {
            get { return zcfs; }
            set
            {
                Set(ref zcfs, value);
            }
        }
        // 支撑界面保存
        public ICommand SaveSupportMethod
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    SupportMethod supportMethod = obj as SupportMethod;
                    RadioButton[] uIElements = new RadioButton[11];
                    supportMethod.gropu.Children.CopyTo(uIElements, 0);
                    //RadioButton特性是不能取消所以这边无需判空
                    Zcfs = supportMethod.gropu.Children.IndexOf(uIElements.First(p => p.IsChecked == true));
                    supportMethod.Close();
                });
        }
        #endregion     

        public ICommand ChooseSupmet
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    SupportMethod supportMethod = new SupportMethod(Zcfs, Convert.ToInt32(obj));
                    supportMethod.ShowDialog();
                });
        }

        public double Phdj { get; set; }
        public double Gzzs { get; set; }
        public double Gjzl { get; set; }
        public ICommand ISO
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    try
                    {
                        Phdj = Convert.ToDouble(ConfigurationManager.AppSettings["Phdj"]);
                        Gzzs = Convert.ToDouble(ConfigurationManager.AppSettings["Gzzs"]);
                        Gjzl = Convert.ToDouble(ConfigurationManager.AppSettings["Gjzl"]);
                        ISO iso = new ISO();
                        iso.ShowDialog();
                    }
                    catch (Exception e)
                    {
                        NewMessageBox.Show(LanguageManager.Instance["LoadISO"] + e.ToString());
                    }
                });
        }

        public ICommand ISOCalculate
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    try
                    {
                        PageRtSet rtSet = GlobalVar.mainWindow.frame.Content as PageRtSet;
                        double r1 = Convert.ToDouble(rtSet.R1.Text);
                        double r2 = Convert.ToDouble(rtSet.R2.Text);
                        if (r1 != 0 && Gzzs != 0)
                            rtSet.pmyyxl.Text = Math.Round(Phdj * 1000 * 10 / Gzzs * Gjzl / 2 / r1, 3).ToString();
                        else
                            rtSet.pmyyxl.Text = "0";
                        if (r2 != 0 && Gzzs != 0)
                            rtSet.pmeyxl.Text = Math.Round(Phdj * 1000 * 10 / Gzzs * Gjzl / 2 / r2, 3).ToString();
                        else rtSet.pmeyxl.Text = "0";
                        rtSet.jyxl.Text = (Convert.ToDouble(rtSet.pmyyxl.Text) + Convert.ToDouble(rtSet.pmeyxl.Text)).ToString();
                    }
                    catch (Exception e)
                    {
                        NewMessageBox.Show(LanguageManager.Instance["ISOParaSetIsAbnormal"] + e.ToString());
                    }
                });
        }

        public ICommand ISOClose
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    try
                    {
                        GlobalVar.config.AppSettings.Settings["Phdj"].Value = Phdj.ToString();
                        GlobalVar.config.AppSettings.Settings["Gzzs"].Value = Gzzs.ToString();
                        GlobalVar.config.AppSettings.Settings["Gjzl"].Value = Gjzl.ToString();
                        GlobalVar.config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                    catch (Exception e)
                    {

                    }
                });
        }

        public ICommand RtSetSave => new RelayCommand<object>(
                obj =>
                {
                    PageRtSet rtSet = GlobalVar.mainWindow.frame.Content as PageRtSet;

                    rtSet.name.GetBindingExpression(SetItem.TextProperty).UpdateSource();
                    rtSet.jsms.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();
                    rtSet.ccdw.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();
                    rtSet.clms.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();
                    rtSet.yxldw.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();
                    rtSet.pmyyxl.GetBindingExpression(SetItem.TextProperty).UpdateSource();
                    rtSet.jyxl.GetBindingExpression(SetItem.TextProperty).UpdateSource();
                    rtSet.pmeyxl.GetBindingExpression(SetItem.TextProperty).UpdateSource();
                    rtSet.A.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    rtSet.B.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    rtSet.C.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    rtSet.R1.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    rtSet.R2.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    rtSet.Speed.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    if (Rotor.NAME is null)
                    {
                        //NewMessageBox.Show("请输入转子名称");Mode 6
                        NewMessageBox.Show(LanguageManager.Instance["InputRotorName"]);
                        return;
                    }
                    Rotor.Zcfs = Zcfs;
                    using (CodeFirstDbContext Entity = new CodeFirstDbContext())
                    {
                        int i = 0;
                        if (RotorSets.FirstOrDefault(p => p.ID == Rotor.ID) is null)
                        {
                            if (RotorSets.FirstOrDefault(p => p.NAME == Rotor.NAME) != null)
                            {
                                NewMessageBox.Show(LanguageManager.Instance["InputAnotherRotorName"]);
                                return;
                            }
                            Entity.T_RotorSet.Add(Rotor);
                            i = Entity.SaveChanges();
                            if (i == 1)
                            {
                                RotorSets.Add(Rotor);
                                NewMessageBox.Show(LanguageManager.Instance["Saved"]);
                                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                                config.AppSettings.Settings["lastrotor"].Value = Rotor.ID.ToString();
                                config.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection("appSettings");
                            }
                        }
                        else
                        {
                            Entity.Entry(Rotor).State = EntityState.Modified;
                            Entity.Entry(Rotor).CurrentValues.SetValues(Rotor);
                            i = Entity.SaveChanges();
                            if (i == 1)
                            {
                                NewMessageBox.Show(LanguageManager.Instance["Saved"]);
                                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                                config.AppSettings.Settings["lastrotor"].Value = Rotor.ID.ToString();
                                config.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection("appSettings");
                            }
                        }
                        //更新测量页面模式
                        if (i == 1)
                        {
                            if (GlobalVar.FormulaResolvingMode != Rotor.Clms)
                            {
                                GlobalVar.FormulaResolvingMode = Rotor.Clms;
                            }
                            //RtSet_SavetoPLC(Rotor);
                            GlobalVar.main.Rotor = Rotor;
                            MeasureViewModel s = SimpleIoc.Default.GetInstance<MeasureViewModel>();
                            s.Rotor = Rotor;
                        }
                        int enable = Rotor.Jsms;
                        int meaMode = Rotor.Clms;
                        if (enable == 1)
                        {
                            MainViewModel.bal._runDB.hard_run.enable = false;
                        }
                        if (enable == 2)
                        {
                            MainViewModel.bal._runDB.hard_run.enable = true;
                            MainViewModel.bal._runDB.hard_run.r1 = (double)Rotor.R1;
                            MainViewModel.bal._runDB.hard_run.r2 = (double)Rotor.R2;
                            MainViewModel.bal._runDB.hard_run.A = (double)Rotor.A;
                            MainViewModel.bal._runDB.hard_run.B = (double)Rotor.B;
                            MainViewModel.bal._runDB.hard_run.C = (double)Rotor.C;
                        }
                        if (meaMode == 0)
                        {
                            MainViewModel.bal._runDB.set_run.single_side = false;
                        }
                        if (meaMode == 1)
                        {
                            MainViewModel.bal._runDB.set_run.single_side = true;
                        }
                        MainViewModel.bal._runDB.set_run.set_rpm = (double)Rotor.Speed;
                        MainViewModel.bal._runDB.set_run.ccdw = (double)Rotor.Ccdw;
                        MainViewModel.bal._runDB.set_run.Yxldw = (double)Rotor.Yxldw;
                        T_Caldata caldata = Entity.T_Caldatas.OrderByDescending(o => o.ID).FirstOrDefault(p => p.rotorid == Rotor.ID && p.speed == Rotor.Speed);
                        T_Clampdata clampdata = Entity.T_Clampdatas.FirstOrDefault(p => p.rotorid == Rotor.ID);
                        SyncBalData syncBalData = new SyncBalData();
                        syncBalData.SyncCal(caldata);
                        syncBalData.SyncClamp(clampdata);
                    }
                    RaisePropertyChanged("RotorSets");
                });

        public ICommand Add
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    ComboBox cbxchoose = obj as ComboBox;
                    cbxchoose.SelectedIndex = -1;
                    Rotor = new T_RotorSet();
                });
        }

        public ICommand Delete
        {
            get => new RelayCommand<object>(
                obj =>
                {
                    if (Rotor != null)
                    {
                        CodeFirstDbContext Entity = new CodeFirstDbContext();
                        if (Entity.T_RotorSet.Count() == 1)
                        {
                            NewMessageBox.Show(LanguageManager.Instance["KeepOneRotor"]);
                            return;
                        }
                        var r = Entity.T_RotorSet.FirstOrDefault(p => p.ID == Rotor.ID);
                        if (r != null)
                        {
                            Entity.T_RotorSet.Remove(r);
                            int i = Entity.SaveChanges();
                            if (i > 0)
                            {
                                RotorSets = new ObservableCollection<T_RotorSet>(Entity.T_RotorSet);
                                Rotor = RotorSets.FirstOrDefault();
                            }
                        }
                    }

                });
        }

        private T_RotorSet rotor;

        public T_RotorSet Rotor
        {
            get { return rotor; }
            set
            {
                Set(ref rotor, value);
                // 支撑方式对应到不同的转子图片
                if (rotor != null)
                {
                    Zcfs = rotor.Zcfs;
                }
            }
        }

        private ObservableCollection<T_RotorSet> rotorSets;

        public ObservableCollection<T_RotorSet> RotorSets
        {
            get { return rotorSets; }
            set
            {
                Set(ref rotorSets, value);
            }
        }
    }
}
