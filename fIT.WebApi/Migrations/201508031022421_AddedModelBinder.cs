namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedModelBinder : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ScheduleExercises", newName: "ScheduleExercise");
            RenameColumn(table: "dbo.ScheduleExercises", name: "Schedule_Id", newName: "SchaduleRefId");
            RenameColumn(table: "dbo.ScheduleExercises", name: "Exercise_Id", newName: "ExerciseRefId");
            RenameIndex(table: "dbo.ScheduleExercises", name: "IX_Exercise_Id", newName: "IX_ExerciseRefId");
            RenameIndex(table: "dbo.ScheduleExercises", name: "IX_Schedule_Id", newName: "IX_SchaduleRefId");
            DropPrimaryKey("dbo.ScheduleExercises");
            AddPrimaryKey("dbo.ScheduleExercises", new[] { "ExerciseRefId", "SchaduleRefId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ScheduleExercise");
            AddPrimaryKey("dbo.ScheduleExercises", new[] { "Schedule_Id", "Exercise_Id" });
            RenameIndex(table: "dbo.ScheduleExercise", name: "IX_SchaduleRefId", newName: "IX_Schedule_Id");
            RenameIndex(table: "dbo.ScheduleExercise", name: "IX_ExerciseRefId", newName: "IX_Exercise_Id");
            RenameColumn(table: "dbo.ScheduleExercise", name: "ExerciseRefId", newName: "Exercise_Id");
            RenameColumn(table: "dbo.ScheduleExercise", name: "SchaduleRefId", newName: "Schedule_Id");
            RenameTable(name: "dbo.ScheduleExercise", newName: "ScheduleExercises");
        }
    }
}
