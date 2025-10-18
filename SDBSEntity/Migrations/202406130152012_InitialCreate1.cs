namespace SDBSEntity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.T_Caldata", "v0", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.T_Caldata", "v0", c => c.Single());
        }
    }
}
