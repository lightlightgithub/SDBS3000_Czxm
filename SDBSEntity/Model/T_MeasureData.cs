using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity.Model
{
    /*这段代码是用C#编写的实体模型的一部分，这个模型类代表数据库中的一个表，
     * 其名称和字段映射到数据库中的对应表和列。*/
    [Description("测量数据")]
    // T_MeasureData为模型的一个实体，通常与数据库表相对应。
    public class T_MeasureData
    {
        [Key]
        [Description("ID号")]
        public int ID { get; set; }
        [Description("转子id")]
        public int RotorID { get; set; }
        [Description("操作者id")]
        public int UserID { get; set; }
        [Description("测量转速")]
        public int rpm { get; set; }
        [Description("左信号量")]
        public int singleL { get; set; }
        [Description("右信号量")]
        public int singleR { get; set; }
        [Description("左量值")]
        public double fl { get; set; }
        [Description("左相位")]
        public double ql { get; set; }
        [Description("右量值")]
        public double fr { get; set; }
        [Description("右相位")]
        public double qr { get; set; }
        [Description("静量值")]
        public double fm { get; set; }
        [Description("静相位")]
        public double qm { get; set; }
        [Description("是否合格")]
        public string Ispass { get; set; }

        [Description("转子名称")]
        public string NAME { get; set; }
        [Description("支撑方式")]
        public int Zcfs { get; set; }
        [Description("解算模式")]
        public string Jsms { get; set; }
        [Description("测量模式")]
        public string Clms { get; set; }
        [Description("测量单位")]
        public string Ccdw { get; set; }
        [Description("允许量单位")]
        public string Yxldw { get; set; }
        [Description("平面一允许量")]
        public float? Pmyyxl { get; set; }
        [Description("平面二允许量")]
        public float? Pmeyxl { get; set; }
        [Description("静允许量")]
        public float? Jyxl { get; set; }
        [Description("A")]
        public float? A { get; set; }
        [Description("B")]
        public float? B { get; set; }
        [Description("C")]
        public float? C { get; set; }
        [Description("R1")]
        public float? R1 { get; set; }
        [Description("R2")]
        public float? R2 { get; set; }

        [Description("设置转速")]
        public float Duringtime { get; set; }
        [Description("节拍")]
        public float? Speed { get; set; }
        [Description("静加去重")]
        public int Jjjz { get; set; }
        [Description("平面一加去重")]
        public int Pmyjjz { get; set; }
        [Description("平面二加去重")]
        public int Pmejjz { get; set; }

        public int isclear { get; set; }

        [Description("保存时间")]
        public DateTime MODIFYTIME { get; set; }

        [Timestamp]
        public byte[] timestamp { get; set; }

        public T_MeasureData()
        {
            MODIFYTIME = DateTime.Now;
        }
    }
}
