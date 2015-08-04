namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePracticeKeyDefinition2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Practices", "ScheduleId");
            CreateIndex("dbo.Practices", "ExerciseId");
            AddForeignKey("dbo.Practices", "ExerciseId", "dbo.Exercises", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Practices", "ScheduleId", "dbo.Schedules", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Practices", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Practices", "ExerciseId", "dbo.Exercises");
            DropIndex("dbo.Practices", new[] { "ExerciseId" });
            DropIndex("dbo.Practices", new[] { "ScheduleId" });
        }
    }
}
