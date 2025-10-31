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
    /// <summary>
    /// 转子测量模式
    /// </summary>
    public enum MeasureMode
    {
        [Description("双面动平衡")]
        TwoPlaneDynamicBalance = 0,
        [Description("静平衡")]
        StaticBalance = 1,
        [Description("动静平衡")]
        DynamicStaticBalance = 2,
        [Description("立式双面动平衡")]
        VerticalTwoPlaneDynamicBalance = 3,
        [Description("立式动静平衡")]
        VerticalDynamicStaticBalance = 4
    }
    /// <summary>
    /// 运行模式  1测量中  3停止  6 测量准备 7预热期过滤 死区滤波 8夹具补偿完成  
    /// </summary>
    public enum RunMode
    {
        [Description("测量中")]
        Measuring = 1,
        [Description("停止")]
        Stop = 3,
        [Description("测量准备")]
        Ready = 6,
        [Description("预热期过滤")]
        PreFilter = 7,
        [Description("夹具补偿")]
        ClampComp = 8
    }
}
