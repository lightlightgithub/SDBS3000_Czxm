using SDBS3000.Resources;
using SDBS3000.Utils.AppSettings;
using SDBS3000.ViewModel;
using SDBSEntity;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SDBS3000.Views
{
    /// <summary>
    /// WindowDrilling.xaml 的交互逻辑
    /// </summary>
    public partial class WindowDrilling : Window
    {
        public WindowDrilling()
        {
            InitializeComponent();
            CodeFirstDbContext Entity = new CodeFirstDbContext();
            try
            {
                T_RotorSet Rotor = Entity.T_RotorSet.Find(GlobalVar.main.Rotor.ID);
                var drillings = Entity.Drillings.Where(p => p.rotorid == Rotor.ID).ToList();
                if (drillings.Count() == 0)
                {
                    List<Drilling> dlist = new List<Drilling>();
                    Drilling d1 = new Drilling() { whichp = 1, rotorid = Rotor.ID, RotorSet = Rotor };
                    Drilling d2 = new Drilling() { whichp = 2, rotorid = Rotor.ID, RotorSet = Rotor };
                    dlist.Add(d1);
                    dlist.Add(d2);
                    Entity.Drillings.AddRange(dlist);
                    int rt = Entity.SaveChanges();
                    drillings = Entity.Drillings.Where(p => p.rotorid == Rotor.ID).ToList();
                }
                datagrid.ItemsSource = drillings;

                var divisions = Entity.Divisions.Where(p => p.rotorid == Rotor.ID).ToList();
                if (divisions.Count() == 0)
                {
                    List<Division> dlist = new List<Division>();
                    Division d1 = new Division() { whichp = 1, rotorid = Rotor.ID, RotorSet = Rotor };
                    Division d2 = new Division() { whichp = 2, rotorid = Rotor.ID, RotorSet = Rotor };
                    dlist.Add(d1);
                    dlist.Add(d2);
                    Entity.Divisions.AddRange(dlist);
                    int rt = Entity.SaveChanges();
                    divisions = Entity.Divisions.Where(p => p.rotorid == Rotor.ID).ToList();
                }
                datagrid2.ItemsSource = divisions;
            }
            catch (Exception ex) { }
            finally
            {
                Entity.Dispose();
            }
            datagrid.Loaded += Datagrid_Loaded;
        }

        private void Datagrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridRow r1 = datagrid.ItemContainerGenerator.ContainerFromIndex(1) as DataGridRow;
            DataGridCell cell = datagrid.Columns[14].GetCellContent(r1).Parent as DataGridCell;
            cell.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CodeFirstDbContext Entity = new CodeFirstDbContext();
            try
            {
                foreach (Drilling data in datagrid.ItemsSource)
                {
                    Drilling drilling = Entity.Drillings.FirstOrDefault(p => p.id == data.id);
                    Entity.Entry(drilling).State = System.Data.Entity.EntityState.Modified;
                    Entity.Entry(drilling).CurrentValues.SetValues(data);
                    int save2 = Entity.SaveChanges();

                    DrillingStruct drillingStruct = new DrillingStruct
                    {
                        rT = data.rT,
                        angT = data.angT,
                        depMax = data.depMax,
                        R = data.R,
                        density = data.density,
                        remnant = data.remnant,
                        factor = data.factor,
                        angDis = data.angDis,
                        calMode = Convert.ToUInt16(data.calMode),
                        nHoleMax = Convert.ToUInt16(data.nHoleMax),
                        rangeMax = data.rangeMax,
                        errMea = data.errMea,
                        allow = data.allow,
                        b = data.b
                    };
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Entity.Dispose();
                NewMessageBox.Show(LanguageManager.Instance["Saved"]);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CodeFirstDbContext Entity = new CodeFirstDbContext();
            try
            {
                foreach (Division data in datagrid2.ItemsSource)
                {
                    Division div = Entity.Divisions.FirstOrDefault(p => p.id == data.id);
                    Entity.Entry(div).State = System.Data.Entity.EntityState.Modified;
                    Entity.Entry(div).CurrentValues.SetValues(data);
                    //通讯异常则不保存数据库，不执行上传数据
                    int save2 = Entity.SaveChanges();
                    DivisionSet divisionSet = new DivisionSet()
                    {
                        num = Convert.ToUInt16(data.posnum),
                        pos0 = data.pos0,
                        pos1 = data.pos1,
                        pos2 = data.pos2,
                        pos3 = data.pos3,
                        pos4 = data.pos4,
                        pos5 = data.pos5,
                        pos6 = data.pos6,
                        pos7 = data.pos7,
                        pos8 = data.pos8,
                        pos9 = data.pos9,
                        pos10 = data.pos10,
                        pos11 = data.pos11,
                        pos12 = data.pos12,
                        pos13 = data.pos13,
                        pos14 = data.pos14,
                        pos15 = data.pos15,
                        pos16 = data.pos16,
                        pos17 = data.pos17,
                        pos18 = data.pos18,
                        pos19 = data.pos19,
                        pos20 = data.pos20,
                        pos21 = data.pos21,
                        pos22 = data.pos22,
                        pos23 = data.pos23,
                        pos24 = data.pos24,
                        pos25 = data.pos25,
                        pos26 = data.pos26,
                        pos27 = data.pos27,
                        pos28 = data.pos28,
                        pos29 = data.pos29,
                        pos30 = data.pos30,
                        pos31 = data.pos31,
                    };
                }
            }
            catch (Exception ex)
            {
            }
            finally { Entity.Dispose(); NewMessageBox.Show(LanguageManager.Instance["Saved"]); }
        }

        //使用平面一数据
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                Division d1 = new Division(), d2 = new Division();
                foreach (Division division in datagrid2.ItemsSource)
                {
                    if (division.whichp == 1)
                        d1 = division;
                    else
                        d2 = division;
                }
                var Types = typeof(Division);//获得类型  

                foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段  
                {
                    if (sp.Name != "whichp" && sp.Name != "id")//判断属性名是否相同  
                    {
                        sp.SetValue(d2, sp.GetValue(d1, null), null);//获得s对象属性的值复制给d对象的属性  
                    }
                }
                List<Division> divisions = new List<Division>();
                divisions.Add(d1);
                divisions.Add(d2);
                datagrid2.ItemsSource = divisions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
