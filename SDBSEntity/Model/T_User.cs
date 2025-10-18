using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity.Model
{
    [Serializable]
    public class T_USER : ICloneable
    {
        [Key]
        public int ID { get; set; }
        [StringLength(32)]
        [Required]
        [Index(IsUnique = true)]
        public string NAME { get; set; }
        [StringLength(12)]
        [Required]
        public string PSD { get; set; }
        [Range(0, 2)]
        public int? PERMISSION { get; set; }
        [StringLength(12)]
        public string NOTE { get; set; }
        [StringLength(20)]
        public string MESNAME { get; set; }
        [StringLength(20)]
        public string MESPSD { get; set; }
        public DateTime MODIFYTIME { get; set; }

        [Timestamp]
        public byte[] timestamp { get; set; }

        //CodeFirst中体现两张表的关系
        //相当于EF模型中的导航属性  
        //一个班级 可以对应 多个学生
        //public virtual ICollection<StudentInfo> StudentInfo { get; set; }
        //ALTER TABLE SDBS3000.t_user MODIFY COLUMN Name varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL;
        //ALTER TABLE SDBS3000.t_user MODIFY COLUMN Psd varchar(12) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL;
        //ALTER TABLE SDBS3000.t_user MODIFY COLUMN Note varchar(12) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL;
        public T_USER()
        {
            MODIFYTIME = DateTime.Now;
        }

        public override string ToString()
        {
            return this.NAME;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
