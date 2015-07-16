namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlteredModels_WeightTimeStamp : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ScheduleExercises", new[] { "Schedule_ID" });
            DropIndex("dbo.ScheduleExercises", new[] { "Exercise_ID" });
            AddColumn("dbo.Practices", "UserId", c => c.String());
            AddColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Schedules", "UserID", c => c.String());
            AlterColumn("dbo.Practices", "Timestamp", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Practices", "Weight", c => c.Double(nullable: false));
            CreateIndex("dbo.ScheduleExercises", "Schedule_Id");
            CreateIndex("dbo.ScheduleExercises", "Exercise_Id");
            DropColumn("dbo.AspNetUsers", "Age");
            DropColumn("dbo.AspNetUsers", "Level");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Level", c => c.Byte(nullable: false));
            AddColumn("dbo.AspNetUsers", "Age", c => c.Int(nullable: false));
            DropIndex("dbo.ScheduleExercises", new[] { "Exercise_Id" });
            DropIndex("dbo.ScheduleExercises", new[] { "Schedule_Id" });
            AlterColumn("dbo.Practices", "Weight", c => c.Int(nullable: false));
            AlterColumn("dbo.Practices", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AlterColumn("dbo.Schedules", "UserID", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "DateOfBirth");
            DropColumn("dbo.Practices", "UserId");
            CreateIndex("dbo.ScheduleExercises", "Exercise_ID");
            CreateIndex("dbo.ScheduleExercises", "Schedule_ID");
        }
    }
}
