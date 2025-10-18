namespace SDBSEntity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.T_Dictionary",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        code = c.Int(nullable: false),
                        name = c.String(nullable: false, maxLength: 32),
                        value = c.Single(nullable: false),
                        note = c.String(),
                        plcaddr = c.String(),
                        active = c.Boolean(nullable: false),
                        time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.code, unique: true)
                .Index(t => t.name, unique: true);
            
            CreateTable(
                "dbo.Division",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        rotorid = c.Int(nullable: false),
                        whichp = c.Int(nullable: false),
                        posnum = c.Int(nullable: false),
                        pos0 = c.Single(nullable: false),
                        pos1 = c.Single(nullable: false),
                        pos2 = c.Single(nullable: false),
                        pos3 = c.Single(nullable: false),
                        pos4 = c.Single(nullable: false),
                        pos5 = c.Single(nullable: false),
                        pos6 = c.Single(nullable: false),
                        pos7 = c.Single(nullable: false),
                        pos8 = c.Single(nullable: false),
                        pos9 = c.Single(nullable: false),
                        pos10 = c.Single(nullable: false),
                        pos11 = c.Single(nullable: false),
                        pos12 = c.Single(nullable: false),
                        pos13 = c.Single(nullable: false),
                        pos14 = c.Single(nullable: false),
                        pos15 = c.Single(nullable: false),
                        pos16 = c.Single(nullable: false),
                        pos17 = c.Single(nullable: false),
                        pos18 = c.Single(nullable: false),
                        pos19 = c.Single(nullable: false),
                        pos20 = c.Single(nullable: false),
                        pos21 = c.Single(nullable: false),
                        pos22 = c.Single(nullable: false),
                        pos23 = c.Single(nullable: false),
                        pos24 = c.Single(nullable: false),
                        pos25 = c.Single(nullable: false),
                        pos26 = c.Single(nullable: false),
                        pos27 = c.Single(nullable: false),
                        pos28 = c.Single(nullable: false),
                        pos29 = c.Single(nullable: false),
                        pos30 = c.Single(nullable: false),
                        pos31 = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.T_RotorSet", t => t.rotorid, cascadeDelete: true)
                .Index(t => t.rotorid);
            
            CreateTable(
                "dbo.T_RotorSet",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NAME = c.String(nullable: false, maxLength: 32),
                        Zcfs = c.Int(nullable: false),
                        Jsms = c.Int(nullable: false),
                        Clms = c.Int(nullable: false),
                        Ccdw = c.Int(nullable: false),
                        Yxldw = c.Int(nullable: false),
                        Pmyyxl = c.Single(),
                        Pmeyxl = c.Single(),
                        Jyxl = c.Single(),
                        A = c.Single(),
                        B = c.Single(),
                        C = c.Single(),
                        R1 = c.Single(),
                        R2 = c.Single(),
                        Speed = c.Single(),
                        MODIFYTIME = c.DateTime(nullable: false),
                        timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        jjcs = c.Int(nullable: false),
                        jj1 = c.Double(nullable: false),
                        jj2 = c.Double(nullable: false),
                        jj3 = c.Double(nullable: false),
                        jj4 = c.Double(nullable: false),
                        key1 = c.Double(nullable: false),
                        key2 = c.Double(nullable: false),
                        key3 = c.Double(nullable: false),
                        key4 = c.Double(nullable: false),
                        axis1 = c.Single(nullable: false),
                        axis2 = c.Single(nullable: false),
                        axis3 = c.Single(nullable: false),
                        axis4 = c.Single(nullable: false),
                        axis5 = c.Single(nullable: false),
                        axis6 = c.Single(nullable: false),
                        axis7 = c.Single(nullable: false),
                        axis8 = c.Single(nullable: false),
                        axis9 = c.Single(nullable: false),
                        axis10 = c.Single(nullable: false),
                        pmy = c.Boolean(nullable: false),
                        pme = c.Boolean(nullable: false),
                        noweight = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.NAME, unique: true);
            
            CreateTable(
                "dbo.T_Caldata",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        v0 = c.Single(nullable: false),
                        v1 = c.Single(nullable: false),
                        v2 = c.Single(nullable: false),
                        v3 = c.Single(nullable: false),
                        h0 = c.Double(nullable: false),
                        h1 = c.Double(nullable: false),
                        h2 = c.Double(nullable: false),
                        h3 = c.Double(nullable: false),
                        ar0 = c.Double(nullable: false),
                        ar1 = c.Double(nullable: false),
                        ar2 = c.Double(nullable: false),
                        ar3 = c.Double(nullable: false),
                        ai0 = c.Double(nullable: false),
                        ai1 = c.Double(nullable: false),
                        ai2 = c.Double(nullable: false),
                        ai3 = c.Double(nullable: false),
                        rotorid = c.Int(nullable: false),
                        speed = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.T_RotorSet", t => t.rotorid, cascadeDelete: true)
                .Index(t => t.rotorid);
            
            CreateTable(
                "dbo.Drilling",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        rotorid = c.Int(nullable: false),
                        whichp = c.Int(nullable: false),
                        rT = c.Single(nullable: false),
                        angT = c.Single(nullable: false),
                        depMax = c.Single(nullable: false),
                        R = c.Single(nullable: false),
                        density = c.Single(nullable: false),
                        remnant = c.Single(nullable: false),
                        factor = c.Single(nullable: false),
                        angDis = c.Single(nullable: false),
                        calMode = c.Int(nullable: false),
                        nHoleMax = c.Int(nullable: false),
                        rangeMax = c.Single(nullable: false),
                        errMea = c.Single(nullable: false),
                        allow = c.Single(nullable: false),
                        b = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.T_RotorSet", t => t.rotorid, cascadeDelete: true)
                .Index(t => t.rotorid);
            
            CreateTable(
                "dbo.T_AxisSet",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        jg_yxsd = c.Single(nullable: false),
                        jg_zlsd = c.Single(nullable: false),
                        axis1 = c.Single(nullable: false),
                        jg_sxw = c.Single(nullable: false),
                        jg_xxw = c.Single(nullable: false),
                        axis2 = c.Single(nullable: false),
                        tds_sxw = c.Single(nullable: false),
                        tds_xxw = c.Single(nullable: false),
                        axis3 = c.Single(nullable: false),
                        tdx_sxw = c.Single(nullable: false),
                        tdx_xxw = c.Single(nullable: false),
                        tdsx_yxsd = c.Single(nullable: false),
                        tdsx_zlsd = c.Single(nullable: false),
                        tdsx_jiassj = c.Single(nullable: false),
                        tdsx_jianssj = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.T_MeasureData",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RotorID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        rpm = c.Int(nullable: false),
                        singleL = c.Int(nullable: false),
                        singleR = c.Int(nullable: false),
                        fl = c.Double(nullable: false),
                        ql = c.Double(nullable: false),
                        fr = c.Double(nullable: false),
                        qr = c.Double(nullable: false),
                        fm = c.Double(nullable: false),
                        qm = c.Double(nullable: false),
                        Ispass = c.String(),
                        NAME = c.String(),
                        Zcfs = c.Int(nullable: false),
                        Jsms = c.String(),
                        Clms = c.String(),
                        Ccdw = c.String(),
                        Yxldw = c.String(),
                        Pmyyxl = c.Single(),
                        Pmeyxl = c.Single(),
                        Jyxl = c.Single(),
                        A = c.Single(),
                        B = c.Single(),
                        C = c.Single(),
                        R1 = c.Single(),
                        R2 = c.Single(),
                        Speed = c.Single(),
                        Jjjz = c.Int(nullable: false),
                        Pmyjjz = c.Int(nullable: false),
                        Pmejjz = c.Int(nullable: false),
                        isclear = c.Int(nullable: false),
                        MODIFYTIME = c.DateTime(nullable: false),
                        timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.T_Set",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        dbsd = c.Single(nullable: false),
                        dbxc = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.T_USER",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NAME = c.String(nullable: false, maxLength: 32),
                        PSD = c.String(nullable: false, maxLength: 12),
                        PERMISSION = c.Int(),
                        NOTE = c.String(maxLength: 12),
                        MESNAME = c.String(maxLength: 20),
                        MESPSD = c.String(maxLength: 20),
                        MODIFYTIME = c.DateTime(nullable: false),
                        timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.NAME, unique: true);
            Sql(@"SET IDENTITY_INSERT [dbo].[T_RotorSet] ON 
                    INSERT [dbo].[T_RotorSet] ([ID], [NAME], [Jsms], [Clms], [Zcfs], [Ccdw], [Yxldw], [Pmyyxl], [Pmeyxl], [Jyxl], [MODIFYTIME]
                    , [A], [B], [C], [R1], [R2], [Speed],  [jj1], [jj2], [jj3], [jj4], [key1], [key2], [key3], [key4]
                    , [axis1], [axis2], [axis3], [axis4], [axis5], [axis6], [axis7], [axis8], [axis9], [axis10], [pmy], [pme], [noweight], [jjcs]) VALUES (1, N'jP1', 0, 0, 1, 0, 0, 4, 4, 22, CAST(N'2024-01-26T13:52:26.403' AS DateTime), 21, 22, 10, 30, 30, 700,  -105888.39197665083, 22962.334581728803, -200294.01077311218, 77993.540645217668, 186.7159455736255, 176.51201406589826, -701.42292359450948, -1255.030195290441, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 2)
                    SET IDENTITY_INSERT [dbo].[T_RotorSet] OFF
                    GO");
            Sql(@"SET IDENTITY_INSERT [dbo].[T_USER] ON 
                    INSERT [dbo].[T_USER] ([ID], [NAME], [PSD], [PERMISSION], [NOTE], [MESNAME], [MESPSD], [MODIFYTIME]) VALUES (1, N'Administrator', N'3', 0, NULL, NULL, NULL, CAST(N'2024-01-20T11:27:56.087' AS DateTime))
                    INSERT [dbo].[T_USER] ([ID], [NAME], [PSD], [PERMISSION], [NOTE], [MESNAME], [MESPSD], [MODIFYTIME]) VALUES (2, N'Inspector', N'2', 1, NULL, NULL, NULL, CAST(N'2024-01-20T11:27:56.087' AS DateTime))
                    INSERT [dbo].[T_USER] ([ID], [NAME], [PSD], [PERMISSION], [NOTE], [MESNAME], [MESPSD], [MODIFYTIME]) VALUES (3, N'Operator', N'1', 2, NULL, NULL, NULL, CAST(N'2024-01-20T11:27:56.087' AS DateTime))
                    SET IDENTITY_INSERT [dbo].[T_USER] OFF
                    GO
                    SET IDENTITY_INSERT [dbo].[T_Dictionary] ON 
                    INSERT[dbo].[T_Dictionary]([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(1, N'算法选择', 1, N'0=无 1=分度算法 2=钻孔算法', CAST(N'2024-04-23T09:02:17.857' AS DateTime), 1, 1, N'DB21.DBW500')
                    INSERT[dbo].[T_Dictionary]([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(2, N'工件驱动模式', 0, N'0=伺服 1=普通电机 2=自驱动  工件运转的方式', CAST(N'2024-04-23T09:02:17.870' AS DateTime), 1, 2, N'D20W340')
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(3, N'测量次数', 5, N'设置范围1~50', CAST(N'2024-04-23T09:02:17.870' AS DateTime), 1, 3, N'D20W312')
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(4, N'刷新频率', 5, N'设置范围2~64', CAST(N'2024-04-23T09:02:17.873' AS DateTime), 1, 4, N'D20D308')
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(5, N'转差范围', 10, N'测量时的转速范围差 单位%', CAST(N'2024-04-23T09:02:17.873' AS DateTime), 1, 5, N'D20D314')
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(6, N'工作方式', 1, N'0=连续普通测量 1=普通测量停止 2=测量定位', CAST(N'2024-04-23T09:02:17.873' AS DateTime), 1, 6, N'D20W318')
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(7, N'显示位数', 3, N'0=不显示小数点 1=显示1位小数点 2=显示2位小数点 3=显示3位小数点 ', CAST(N'2024-04-23T09:02:17.873' AS DateTime), 1, 7, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(8, N'光针模式', 0, N'0=不使用光针 1=使用手动检测每转脉冲数 2=使用自动检测每转脉冲数', CAST(N'2024-04-23T09:02:17.873' AS DateTime), 1, 8, N'D20W330')
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(9, N'安全门模式', 1, N'0=不使用 1=使用', CAST(N'2024-04-23T09:02:17.873' AS DateTime), 1, 9, N'D20W328')
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(10, N'数据保存方式', 1, N'0=不记录,1=每次记录 2=合格记录,3=不合格记录', CAST(N'2024-04-23T09:02:17.873' AS DateTime), 1, 10, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(11, N'打印模式', 0, N'0=不使用打印机 1=A4手动打印平衡报告 2=A4自动打印平衡报告 3=手动标签打印 4=自动标签打印', CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 11, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(12, N'矢量图方向', 0, N'0=顺时针 1=逆时针', CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 12, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(13, N'定位模式', 1, N'0=先左后右 1=只左面 2=只右面', CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 13, N'D20W320')
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(14, N'扫码功能', 0, NULL, CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 14, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(16, N'三色报警灯', 0, NULL, CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 15, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(17, N'双手启动', 0, NULL, CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 16, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(18, N'气压保护', 0, NULL, CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 17, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(19, N'避让算法', 0, NULL, CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 18, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(20, N'固定偏量', 0, NULL, CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 19, NULL)
                    INSERT[dbo].[T_Dictionary] ([id], [name], [value], [note], [time], [active], [code], [plcaddr]) VALUES(21, N'多段标定', 0, N'0不使用 1使用', CAST(N'2024-04-23T09:02:17.877' AS DateTime), 1, 20, NULL)
                    SET IDENTITY_INSERT[dbo].[T_Dictionary]   OFF 
                    GO");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Division", "rotorid", "dbo.T_RotorSet");
            DropForeignKey("dbo.Drilling", "rotorid", "dbo.T_RotorSet");
            DropForeignKey("dbo.T_Caldata", "rotorid", "dbo.T_RotorSet");
            DropIndex("dbo.T_USER", new[] { "NAME" });
            DropIndex("dbo.Drilling", new[] { "rotorid" });
            DropIndex("dbo.T_Caldata", new[] { "rotorid" });
            DropIndex("dbo.T_RotorSet", new[] { "NAME" });
            DropIndex("dbo.Division", new[] { "rotorid" });
            DropIndex("dbo.T_Dictionary", new[] { "name" });
            DropIndex("dbo.T_Dictionary", new[] { "code" });
            DropTable("dbo.T_USER");
            DropTable("dbo.T_Set");
            DropTable("dbo.T_MeasureData");
            DropTable("dbo.T_AxisSet");
            DropTable("dbo.Drilling");
            DropTable("dbo.T_Caldata");
            DropTable("dbo.T_RotorSet");
            DropTable("dbo.Division");
            DropTable("dbo.T_Dictionary");
        }
    }
}
