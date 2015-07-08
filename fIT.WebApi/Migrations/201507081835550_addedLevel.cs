namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedLevel : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", new[] { "UserID" });
            AddColumn("dbo.AspNetUsers", "Level", c => c.Byte(nullable: false));
            DropColumn("dbo.AspNetUsers", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserID", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "Level");
            CreateIndex("dbo.AspNetUsers", "UserID", unique: true);
        }
    }
}
