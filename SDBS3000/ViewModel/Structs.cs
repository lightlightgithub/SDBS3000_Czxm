using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModel
{
    class Structs
    {
       
    }
    
    /// <summary>
    /// 刷新频率refl,测量次数kimax,转差范围rev,工作方式workmode,定位模式posmode
    /// </summary>
    public struct Set1
    {
        /// <summary>
        /// 刷新频率
        /// </summary>
        public float refl;
        /// <summary>
        /// 测量次数
        /// </summary>
        public ushort kimax;
        /// <summary>
        /// 转差范围
        /// </summary>
        public float rev;
        /// <summary>
        /// 工作方式
        /// </summary>
        public ushort workmode;
        /// <summary>
        /// 定位模式
        /// </summary>
        public ushort posmode;
    }

    public struct Set2
    {
        public ushort safeDoor;
        public ushort needle;
    }

    /// <summary>
    /// 机器设置
    /// </summary>
    public struct Set
    {
        public Set1 set1;
        public Set2 set2;

        public ushort Decimalnum;
        public ushort Direction;
        public ushort Algorithm;
        public ushort DriverMode;
        public ushort DataSaveMode;
        public ushort PrintMode;
        public ushort MultSpeedCal;
    }

    /// <summary>
    /// 116-140  加去重 定位参数 dwzs定位转速 ,jssj1加速时间,jssj2减速时间,mcs每转脉冲数,dwbc定位补偿,zcbc转差补偿
    /// </summary>
    public struct Set4
    {
        public bool zjqz;
        public bool yjqz;
        public bool jjqz;
        //15个预留
        public bool yl1;
        public bool yl2;
        public bool yl3;
        public bool yl4;
        public bool yl5;
        public bool yl6;
        public bool yl7;
        public bool yl8;
        public bool yl9;
        public bool yl10;
        public bool yl11;
        public bool yl12;
        public bool yl13;

        public ushort yl14;
        //Real	120.0	0.0	60.0	True True    True True    False 定位转速
        //Real	124.0	0.0	2.2	True True    True True    False 加速时间
        //Real	128.0	0.0	2.2	True True    True True    False 减速时间
        //Real	132.0	0.0	2278.5	True True    True True    False 每转脉冲数
        //Real	136.0	0.0	0.0	True True    True True    False 定位补偿
        //Real	140.0	0.0	0.0	True True    True True    False 转差补偿

        public float dwzs;
        public float jssj1;
        public float jssj2;
        public float mcs;
        public float dwbc;
        public float zcbc;
    }


    public struct WarnStruct
    {
        public bool w1;
        public bool w2;
        public bool w3;
        public bool w4;
        public bool w5;
        public bool w6;
        public bool w7;
        public bool w8;
        public bool w9;
        public bool w10;
        public bool w11;
        public bool w12;
        public bool w13;
        public bool w14;
        public bool w15;
        public bool w16;
        public bool w17;
        public bool w18;
        public bool w19;
        public bool w20;
        public bool w21;
        public bool w22;
        public bool w23;
        public bool w24;
        public bool w25;
        public bool w26;
        public bool w27;
        public bool w28;
        public bool w29;
        public bool w30;
        public bool w31;
        public bool w32;
        public bool w33;
        public bool w34;
        public bool w35;
        public bool w36;
        public bool w37;
        public bool w38;
        public bool w39;
        public bool w40;
        public bool w41;
        public bool w42;
        public bool w43;
        public bool w44;
        public bool w45;
        public bool w46;
        public bool w47;
        public bool w48;
        public bool w49;
        public bool w50;
        public bool w51;
        public bool w52;
        public bool w53;
        public bool w54;
        public bool w55;
        public bool w56;

    }

    public struct RotorStruct1 : IStructuralEquatable
    {
        public float speed;
        public ushort jsms;
        public ushort zcfs;
        public ushort clms;
        public ushort ccdw;
        public ushort xsdw;
        //夹具补偿次数
        public ushort jjcs;

        public bool Equals(object other, IEqualityComparer comparer)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            throw new NotImplementedException();
        }
    }

    public struct RotorStruct2
    {
        public float pmy;
        public float pme;
        public float j;
        public float r1;
        public float r2;
        public float r0;
        public float a;
        public float b;
        public float c;
    }

    //470.0	false	FALSE True    True True    True False   开机界面  1
    //470.1	false	FALSE True    True True    True False   双面动平衡 2
    //470.2	false	FALSE True    True True    True False   静平衡3
    //470.3	false	FALSE True    True True    True False   动静平衡4
    //470.4	false	FALSE True    True True    True False   参数5
    //470.5	false	FALSE True    True True    True False   定位6
    //470.6	false	FALSE True    True True    True False   夹具7
    //470.7	false	FALSE True    True True    True False   标定8
    //471.0	false	FALSE True    True True    True False   单步9
    //471.1	false	FALSE True    True True    True False   报警10
    //471.2	false	FALSE True    True True    True False   记录11
    //471.3	false	FALSE True    True True    True False   设置12
    public struct WhichPage
    {
        public bool p1;
        public bool p2;
        public bool p3;
        public bool p4;
        public bool p5;
        public bool p6;
        public bool p7;
        public bool p8;
        public bool p9;
        public bool p10;
        public bool p11;
        public bool p12;
        public bool p13;
        public bool p14;
        public bool p15;
        public bool p16;
        //public WhichPage(int? input)
        //{
        //    p1 = false;
        //    p2 = false;
        //    p3 = false;
        //    p4 = false;
        //    p5 = false;
        //    p6 = false;
        //    p7 = false;
        //    p8 = true;
        //    p9 = false;
        //    p10 = false;
        //    p11 = false;
        //    p12 = false;
        //    p13 = false;
        //    p14 = false;
        //    p15 = false;
        //    p16 = false;
        //}
    }
    /// <summary>
    /// 钻削参数结构体
    /// </summary>
    public struct DrillingStruct
    {
        private float _rT;

        public float rT;
        public float angT;
        public float depMax;
        public float R;
        public float density;
        public float remnant;
        public float factor;
        public float angDis;
        public ushort calMode;
        public ushort nHoleMax;
        public float rangeMax;
        public float errMea;
        public float allow;
        public float b;
    }


    public class JsData
    {
        public string whichp { get; set; }
        public float data1 { get; set; }
        public float data2 { get; set; }
        public float data3 { get; set; }
        public float data4 { get; set; }
        public float data5 { get; set; }
        public float data6 { get; set; }
        public float data7 { get; set; }
        public float data8 { get; set; }
        //public float data1 { get; set; }
        //public float data1 { get; set; }
        //public float data1 { get; set; }
        //public float data1 { get; set; }
        //public float data1 { get; set; }
        //public float data1 { get; set; }
        //public float data1 { get; set; }
    }
    /// <summary>
    /// 钻削解算数据结构体
    /// </summary>
    public struct DrillingDataStruct
    {
        public float data1;
        public float dat1;
        public float da1;
        public float data2;
        public float dat2;
        public float da2;
        public float data3;
        public float dat3;
        public float da3;
        public float data4;
        public float dat4;
        public float da4;
        public float data5;
        public float dat5;
        public float da5;

        public float data6;
        public float dat6;
        public float da6;
        public float data7;
        public float dat7;
        public float da7;
        public float data8;
        public float dat8;
        public float da8;
        public float data9;
        public float dat9;
        public float da9;
        public float data10;
        public float dat10;
        public float da10;

        public float data11;
        public float dat11;
        public float da11;
        public float data12;
        public float dat12;
        public float da12;
        public float data13;
        public float dat13;
        public float da13;
        public float data14;
        public float dat14;
        public float da14;
        public float data15;
        public float dat15;
        public float da15;

        public float data16;
        public float dat16;
        public float da16;
        public float data17;
        public float dat17;
        public float da17;
        public float data18;
        public float dat18;
        public float da18;
        public float data19;
        public float dat19;
        public float da19;
        public float data20;
        public float dat20;
        public float da20;

        public float data21;
        public float dat21;
        public float da21;
        public float data22;
        public float dat22;
        public float da22;
        public float data23;
        public float dat23;
        public float da23;
        public float data24;
        public float dat24;
        public float da24;
        public float data25;
        public float dat25;
        public float da25;

        public float data26;
        public float dat26;
        public float da26;
        public float data27;
        public float dat27;
        public float da27;
        public float data28;
        public float dat28;
        public float da28;
        public float data29;
        public float dat29;
        public float da29;
        public float data30;
        public float dat30;
        public float da30;
        public float data31;
        public float dat31;
        public float da31;
        public float data32;
        public float dat32;
        public float da32;
    }

    public class DivisionJsData
    {
        public ushort xh { get; set; }
        public float data1 { get; set; }
        public float data2 { get; set; }
    }

    public struct DivisionItem
    {
        //public float a { get; set; }
        //public float b { get; set; }
        //public ushort c { get; set; }

        public float a;

        public float A
        {
            get { return a; }
            set { a = value; }
        }

        public float b;

        public float B
        {
            get { return b; }
            set { b = value; }
        }

        public ushort c;

        public ushort C
        {
            get { return c; }
            set { c = value; }
        }
    }

    public struct DivisionItem2
    {
        public float a;
        public float A
        {
            get { return a; }
            set { a = value; }
        }

        public float b;
        public float B
        {
            get { return b; }
            set { b = value; }
        }

        public ushort c;
        public ushort C
        {
            get { return c; }
            set { c = value; }
        }
        public float a2;
        public float A2
        {
            get { return a2; }
            set { a2 = value; }
        }

        public float b2;
        public float B2
        {
            get { return b2; }
            set { b2 = value; }
        }

        public ushort c2;
        public ushort C2
        {
            get { return c; }
            set { c = value; }
        }
    }

    public struct DivisionSingleItem
    {
        public DivisionItem item0;
        public DivisionItem item1;
        public DivisionItem item2;
        public DivisionItem item3;
        public DivisionItem item4;
        public DivisionItem item5;
        public DivisionItem item6;
        public DivisionItem item7;
        public DivisionItem item8;
        public DivisionItem item9;
        public DivisionItem Item0 { get { return item0; } set { item0 = value; } }
        public DivisionItem Item1 { get { return item1; } set { item1 = value; } }
        public DivisionItem Item2 { get { return item2; } set { item2 = value; } }
        public DivisionItem Item3 { get { return item3; } set { item3 = value; } }
        public DivisionItem Item4 { get { return item4; } set { item4 = value; } }
        public DivisionItem Item5 { get { return item5; } set { item5 = value; } }
        public DivisionItem Item6 { get { return item6; } set { item6 = value; } }
        public DivisionItem Item7 { get { return item7; } set { item7 = value; } }
        public DivisionItem Item8 { get { return item8; } set { item8 = value; } }
        public DivisionItem Item9 { get { return item9; } set { item9 = value; } }


        public DivisionItem item10;
        public DivisionItem item11;
        public DivisionItem item12;
        public DivisionItem item13;
        public DivisionItem item14;
        public DivisionItem item15;
        public DivisionItem item16;
        public DivisionItem item17;
        public DivisionItem item18;
        public DivisionItem item19;

        public DivisionItem item20;
        public DivisionItem item21;
        public DivisionItem item22;
        public DivisionItem item23;
        public DivisionItem item24;
        public DivisionItem item25;
        public DivisionItem item26;
        public DivisionItem item27;
        public DivisionItem item28;
        public DivisionItem item29;

        public DivisionItem item30;
        public DivisionItem item31;
    }

    public struct DivisionItems
    {
        public ushort lnum;
        public ushort rnum;
        public ushort Lnum { get { return lnum; } set { lnum = value; } }
        public ushort Rnum { get { return rnum; } set { rnum = value; } }

        public DivisionSingleItem litems;
        public DivisionSingleItem ritems;
        public DivisionSingleItem Litems { get { return litems; } set { litems = value; } }
        public DivisionSingleItem Ritems { get { return ritems; } set { ritems = value; } }
    }



    /// <summary>
    /// 设置写给plc
    /// </summary>
    public struct DivisionSet
    {
        public ushort num;
        public float pos0;
        public float pos1;
        public float pos2;
        public float pos3;
        public float pos4;
        public float pos5;
        public float pos6;
        public float pos7;
        public float pos8;
        public float pos9;
        public float pos10;
        public float pos11;
        public float pos12;
        public float pos13;
        public float pos14;
        public float pos15;
        public float pos16;
        public float pos17;
        public float pos18;
        public float pos19;
        public float pos20;
        public float pos21;
        public float pos22;
        public float pos23;
        public float pos24;
        public float pos25;
        public float pos26;
        public float pos27;
        public float pos28;
        public float pos29;
        public float pos30;
        public float pos31;
    }

    public struct DivisionDataStruct
    {
        public ushort lnum;
        public ushort rnum;
        public float data1;
        public float dat1;
        public float da1;
        public float data2;
        public float dat2;
        public float da2;
        public float data3;
        public float dat3;
        public float da3;
        public float data4;
        public float dat4;
        public float da4;
        public float data5;
        public float dat5;
        public float da5;

        public float data6;
        public float dat6;
        public float da6;
        public float data7;
        public float dat7;
        public float da7;
        public float data8;
        public float dat8;
        public float da8;
        public float data9;
        public float dat9;
        public float da9;
        public float data10;
        public float dat10;
        public float da10;

        public float data11;
        public float dat11;
        public float da11;
        public float data12;
        public float dat12;
        public float da12;
        public float data13;
        public float dat13;
        public float da13;
        public float data14;
        public float dat14;
        public float da14;
        public float data15;
        public float dat15;
        public float da15;

        public float data16;
        public float dat16;
        public float da16;
        public float data17;
        public float dat17;
        public float da17;
        public float data18;
        public float dat18;
        public float da18;
        public float data19;
        public float dat19;
        public float da19;
        public float data20;
        public float dat20;
        public float da20;

        public float data21;
        public float dat21;
        public float da21;
        public float data22;
        public float dat22;
        public float da22;
        public float data23;
        public float dat23;
        public float da23;
        public float data24;
        public float dat24;
        public float da24;
        public float data25;
        public float dat25;
        public float da25;

        public float data26;
        public float dat26;
        public float da26;
        public float data27;
        public float dat27;
        public float da27;
        public float data28;
        public float dat28;
        public float da28;
        public float data29;
        public float dat29;
        public float da29;
        public float data30;
        public float dat30;
        public float da30;
        public float data31;
        public float dat31;
        public float da31;
        public float data32;
        public float dat32;
        public float da32;

        public float rdata1;
        public float rdat1;
        public float rda1;
        public float rdata2;
        public float rdat2;
        public float rda2;
        public float rdata3;
        public float rdat3;
        public float rda3;
        public float rdata4;
        public float rdat4;
        public float rda4;
        public float rdata5;
        public float rdat5;
        public float rda5;

        public float rdata6;
        public float rdat6;
        public float rda6;
        public float rdata7;
        public float rdat7;
        public float rda7;
        public float rdata8;
        public float rdat8;
        public float rda8;
        public float rdata9;
        public float rdat9;
        public float rda9;
        public float rdata10;
        public float rdat10;
        public float rda10;

        public float rdata11;
        public float rdat11;
        public float rda11;
        public float rdata12;
        public float rdat12;
        public float rda12;
        public float rdata13;
        public float rdat13;
        public float rda13;
        public float rdata14;
        public float rdat14;
        public float rda14;
        public float rdata15;
        public float rdat15;
        public float rda15;

        public float rdata16;
        public float rdat16;
        public float rda16;
        public float rdata17;
        public float rdat17;
        public float rda17;
        public float rdata18;
        public float rdat18;
        public float rda18;
        public float rdata19;
        public float rdat19;
        public float rda19;
        public float rdata20;
        public float rdat20;
        public float rda20;

        public float rdata21;
        public float rdat21;
        public float rda21;
        public float rdata22;
        public float rdat22;
        public float rda22;
        public float rdata23;
        public float rdat23;
        public float rda23;
        public float rdata24;
        public float rdat24;
        public float rda24;
        public float rdata25;
        public float rdat25;
        public float rda25;

        public float rdata26;
        public float rdat26;
        public float rda26;
        public float rdata27;
        public float rdat27;
        public float rda27;
        public float rdata28;
        public float rdat28;
        public float rda28;
        public float rdata29;
        public float rdat29;
        public float rda29;
        public float rdata30;
        public float rdat30;
        public float rda30;
        public float rdata31;
        public float rdat31;
        public float rda31;
        public float rdata32;
        public float rdat32;
        public float rda32;
    }

    /// <summary>
    /// 标定设置
    /// </summary>
    public struct Setcal
    {
        public double[] caldata;
        public float[] set0;
    }
    public struct SetCal1
    {
        public double[] cal_ar;
        public double[] cal_ai;
        public float[] set0;
    }

    //夹具结构体
    public struct Clamp_Key
    {
        public bool start_test;//检测夹具补偿
        public bool compesation;//夹具补偿模式启动结束后清零
        public int test_times;//设置夹具翻转次数
        public double[] cps_val;//补偿值
    }
}
