using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity
{
    /// <summary>
    /// 转子类型
    /// </summary>
    public enum RotorType
    {
        双风扇 = 1,
        三风扇 = 2,
    }    

    /// <summary>
    /// 用户权限
    /// </summary>
    public enum UserPermissionEnum
    {
        Administrator = 0,
        Inspector,
        Operator
    }

    public enum FormulaResolvingModePageEnum
    {
        PageHorTwo,
        PageOne,
        PageHorThree,
        PageVerTwo,
        PageVerThree
    }
    public enum ListSelectType
    {
        [Description("查询")]
        Select,
        [Description("当日")]
        Today,
        [Description("昨日")]
        Yesterday,
        [Description("当月")]
        Month,
        [Description("当年")]
        Year
    }
    //public enum ShowUnit
    //{
    //    g,
    //    mg,
    //    g.mm,
    //    g.cm,       
    //}
}
