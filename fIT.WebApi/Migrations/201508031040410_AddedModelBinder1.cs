namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedModelBinder1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ScheduleExercises"); 
            AddPrimaryKey("dbo.ScheduleExercises", new[] { "Exercise_Id", "Schedule_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ScheduleExercises");
            AddPrimaryKey("dbo.ScheduleExercises", new[] { "Schedule_Id", "Exercise_Id" });
        }
    }
}
