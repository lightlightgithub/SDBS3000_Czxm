using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity.Model
{
    public class T_Caldata
    {
        [Key]
        public int ID { get; set; }

        public double v0 { get; set; }
        public double v1 { get; set; }
        public double v2 { get; set; }
        public double v3 { get; set; }

        public double h0 { get; set; }
        public double h1 { get; set; }
        public double h2 { get; set; }
        public double h3 { get; set; }

        public double ar0 { get; set; }
        public double ar1 { get; set; }
        public double ar2 { get; set; }
        public double ar3 { get; set; }

        public double ai0 { get; set; }
        public double ai1 { get; set; }
        public double ai2 { get; set; }
        public double ai3 { get; set; }

        public T_Caldata()
        {

        }

        public int rotorid { get; set; }
        public virtual T_RotorSet RotorSet { get; set; }

        public double speed { get; set; }

        public static implicit operator ObservableCollection<object>(T_Caldata v)
        {
            throw new NotImplementedException();
        }
    }

    public class T_Clampdata
    {
        [Key]
        public int ID { get; set; }

        public int test_times { get; set; }//设置夹具翻转次数
        public double cps_val_1 { get; set; }//补偿值
        public double cps_val_2 { get; set; }
        public double cps_val_3 { get; set; }
        public double cps_val_4 { get; set; }


        public T_Clampdata()
        {

        }
        [Index(IsUnique = true)]
        public int rotorid { get; set; }

        public virtual T_RotorSet RotorSet { get; set; }

        public static implicit operator ObservableCollection<object>(T_Clampdata v)
        {
            throw new NotImplementedException();
        }
    }
}
