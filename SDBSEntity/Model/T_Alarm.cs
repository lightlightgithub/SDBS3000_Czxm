using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity.Model
{
    public class T_Alarm
    {
        [Key]
        public int ID { get; set; }
        public int Alarm_ID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string WarnDescription { get; set; }
    }
}
