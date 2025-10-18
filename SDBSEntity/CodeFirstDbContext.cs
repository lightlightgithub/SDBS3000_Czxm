using SDBSEntity.Migrations;
using SDBSEntity.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSEntity
{
    [BaseEfConfiguration]
    //[DbConfigurationType("MySql.Data.Entity.MySqlEFConfiguration, MySql.Data.Entity.EF6")]
    public class CodeFirstDbContext : DbContext
    {
        //1.需要一个构造方法, 调用父类的构造方法, 传入 数据库连接字符串 的名字作为参数        
        public CodeFirstDbContext() : base("sql")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<CodeFirstDbContext, Configuration>());
            //Database.SetInitializer(new sdbs3000initializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //2.去掉 将表名设置为实体类型名称的复数版本 的约定(如 对应ClassInfo 在数据库生成 ClassInfos表)
            //1>、在程序包管理器控制台，执行语句：
            //PM> Enable-Migrations -EnableAutomaticMigrations -Force
            // 执行成功后，在应用程序代码结构中，添加Migrations文件夹，并生成类文件Configuration.cs。
            //2>、在程序包管理器控制台，执行语句：
            //PM> Add-Migration InitialCreate
            //3>、在程序包管理器控制台，执行语句：
            //PM> Update-Database -Verbose
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<T_Caldata>().HasRequired(l => l.RotorSet).WithMany(d => d.caldatas).HasForeignKey(t => t.rotorid);

            modelBuilder.Entity<T_Clampdata>().HasRequired(l => l.RotorSet).WithMany(d => d.clampdatas).HasForeignKey(t => t.rotorid);

            modelBuilder.Entity<Drilling>().HasRequired(l => l.RotorSet).WithMany(d => d.drillings).HasForeignKey(t => t.rotorid).WillCascadeOnDelete(true);

            modelBuilder.Entity<Division>().HasRequired(l => l.RotorSet).WithMany(d => d.divisions).HasForeignKey(t => t.rotorid).WillCascadeOnDelete(true);           
        }

        //3.
        public DbSet<T_USER> T_Users { get; set; }
        public DbSet<T_RotorSet> T_RotorSet { get; set; }
        public DbSet<T_MeasureData> T_MeasureData { get; set; }
        public DbSet<T_Caldata> T_Caldatas { get; set; }
        public DbSet<T_Clampdata> T_Clampdatas { get; set; }
        public DbSet<T_AxisSet> T_AxisSet { get; set; }
        public DbSet<T_Set> T_Set { get; set; }

        public DbSet<T_Alarm> T_Alarms { get; set; }

        public DbSet<Division> Divisions { get; set; }
        public DbSet<Drilling> Drillings { get; set; }

        public DbSet<T_Dictionary> Dictionarys { get; set; }
    }

    public class BaseEfConfiguration : DbConfigurationTypeAttribute
    {
        public BaseEfConfiguration() : base(SqlConfiguration)
        {

        }

        public static Type SqlConfiguration
        {
            get
            {
                DbConfigurationTypeAttribute a;
                //switch (GlobalVariable.dbcon)
                switch ("sql")
                {
                    case "sql":
                        a = new DbConfigurationTypeAttribute("System.Data.Entity.DbConfiguration, EntityFramework");
                        break;
                }
                return a.ConfigurationType;
            }
        }
    }
}
