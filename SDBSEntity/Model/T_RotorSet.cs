using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity.Model
{
    public class T_RotorSet
    {
        [Key]
        public int ID { get; set; }
        [StringLength(32)]
        [Required]
        [Index(IsUnique = true)]
        public string NAME { get; set; }

        public int Zcfs { get; set; }
        //解算模式
        public int Jsms { get; set; }

        public int Clms { get; set; }

        public int Ccdw { get; set; }
        public int Yxldw { get; set; }
        public float? Pmyyxl { get; set; }
        public float? Pmeyxl { get; set; }
        public float? Jyxl { get; set; }

        public float? A { get; set; }
        public float? B { get; set; }
        public float? C { get; set; }
        public float? R1 { get; set; }
        public float? R2 { get; set; }
        public float? Speed { get; set; }
        public DateTime MODIFYTIME { get; set; }

        [Timestamp]
        public byte[] timestamp { get; set; }
        public virtual ICollection<T_Caldata> caldatas { get; set; }
        public virtual ICollection<T_Clampdata> clampdatas { get; set; }
        public virtual ICollection<Division> divisions { get; set; }
        public virtual ICollection<Drilling> drillings { get; set; }

        public bool start_test { get; set; }//检测夹具补偿
        public bool compesation { get; set; }//夹具补偿模式启动结束后清零
        //夹具补偿次数
        //public int jjcs { get; set; }
        //public double jj1 { get; set; }
        //public double jj2 { get; set; }
        //public double jj3 { get; set; }
        //public double jj4 { get; set; }
        //public double key1 { get; set; }
        //public double key2 { get; set; }
        //public double key3 { get; set; }
        //public double key4 { get; set; }

        public float axis1 { get; set; }
        public float axis2 { get; set; }
        public float axis3 { get; set; }
        public float axis4 { get; set; }
        public float axis5 { get; set; }
        public float axis6 { get; set; }
        public float axis7 { get; set; }
        public float axis8 { get; set; }
        public float axis9 { get; set; }
        public float axis10 { get; set; }

        //记录平面是否做过标定
        public bool pmy { get; set; }
        public bool pme { get; set; }
        public bool noweight { get; set; }

        public T_RotorSet()
        {
            Zcfs = 0;
            Ccdw = 0;
            Yxldw = 0;
            Jsms = 0;
            Clms = 0;
            pmy = true;
            pme = true;
            noweight = true;
            //jjcs = 2;
            MODIFYTIME = DateTime.Now;
        }
    }

    /// <summary>
    /// 轴参数
    /// </summary>
    public class T_AxisSet
    {
        [Key]
        public int id { get; set; }

        public float jg_yxsd { get; set; }
        public float jg_zlsd { get; set; }
        public float axis1 { get; set; }
        public float jg_sxw { get; set; }
        public float jg_xxw { get; set; }

        public float axis2 { get; set; }
        public float tds_sxw { get; set; }
        public float tds_xxw { get; set; }

        public float axis3 { get; set; }
        public float tdx_sxw { get; set; }
        public float tdx_xxw { get; set; }

        public float tdsx_yxsd { get; set; }
        public float tdsx_zlsd { get; set; }
        public float tdsx_jiassj { get; set; }
        public float tdsx_jianssj { get; set; }
    }

    public class T_Set
    {
        [Key]
        public int id { get; set; }
        public float dbsd { get; set; }
        public float dbxc { get; set; }
    }

    /// <summary>
    /// 自定义字典
    /// </summary>
    public class T_Dictionary
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public int code { get; set; }
        [Required]
        [StringLength(32)]
        [Index(IsUnique = true)]
        public string name { get; set; }
        public double value { get; set; }
        public string note { get; set; }
        public string plcaddr { get; set; }

        public bool active { get; set; }
        public DateTime time { get; set; }
    }

    /// <summary>
    /// 分度
    /// </summary>
    public class Division
    {
        [Key]
        public int id { get; set; }

        public int rotorid { get; set; }

        public int whichp { get; set; }

        public int posnum { get; set; }
        public float pos0 { get; set; }
        public float pos1 { get; set; }
        public float pos2 { get; set; }
        public float pos3 { get; set; }
        public float pos4 { get; set; }
        public float pos5 { get; set; }
        public float pos6 { get; set; }
        public float pos7 { get; set; }
        public float pos8 { get; set; }
        public float pos9 { get; set; }
               
        public float pos10 { get; set; }
        public float pos11 { get; set; }
        public float pos12 { get; set; }
        public float pos13 { get; set; }
        public float pos14 { get; set; }
        public float pos15 { get; set; }
        public float pos16 { get; set; }
        public float pos17 { get; set; }
        public float pos18 { get; set; }
        public float pos19 { get; set; }
               
        public float pos20 { get; set; }
        public float pos21 { get; set; }
        public float pos22 { get; set; }
        public float pos23 { get; set; }
        public float pos24 { get; set; }
        public float pos25 { get; set; }
        public float pos26 { get; set; }
        public float pos27 { get; set; }
        public float pos28 { get; set; }
        public float pos29 { get; set; }
               
        public float pos30 { get; set; }
        public float pos31 { get; set; }

        [Required]
        //实体关系在此处建立联系
        public T_RotorSet RotorSet { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class S7StringAttribute : Attribute
    {
        // You can add properties or other members here if needed
    }


    /// <summary>
    /// 钻孔
    /// </summary>
    public class Drilling
    {
        [Key]
        public int id { get; set; }

        public int rotorid { get; set; }

        public int whichp { get; set; }


        private float _rT;
       
        public float rT { get=> _rT; set=> _rT=value; }
        public float angT { get; set; }
        public float depMax { get; set; }
        public float R { get; set; }
        public float density { get; set; }
        public float remnant { get; set; }
        public float factor { get; set; }
        public float angDis { get; set; }
        public int calMode { get; set; }
        public int nHoleMax { get; set; }
        public float rangeMax { get; set; }
        public float errMea { get; set; }
        public float allow { get; set; }
        public float b { get; set; }

        [Required]
        //实体关系在此处建立联系
        public T_RotorSet RotorSet { get; set; }
    }
}
