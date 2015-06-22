namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exercises",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Gender = c.Int(nullable: false),
                        Password = c.String(nullable: false),
                        Job = c.Int(nullable: false),
                        Fitness = c.Int(nullable: false),
                        Age = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Practices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        Weight = c.Int(nullable: false),
                        NumberOfRepetitions = c.Int(nullable: false),
                        Repetitions = c.Int(nullable: false),
                        ScheduleID = c.Int(nullable: false),
                        ExerciseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Exercises", t => t.ExerciseID, cascadeDelete: true)
                .ForeignKey("dbo.Schedules", t => t.ScheduleID, cascadeDelete: true)
                .Index(t => t.ScheduleID)
                .Index(t => t.ExerciseID);
            
            CreateTable(
                "dbo.ScheduleExercises",
                c => new
                    {
                        Schedule_ID = c.Int(nullable: false),
                        Exercise_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Schedule_ID, t.Exercise_ID })
                .ForeignKey("dbo.Schedules", t => t.Schedule_ID, cascadeDelete: true)
                .ForeignKey("dbo.Exercises", t => t.Exercise_ID, cascadeDelete: true)
                .Index(t => t.Schedule_ID)
                .Index(t => t.Exercise_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Practices", "ScheduleID", "dbo.Schedules");
            DropForeignKey("dbo.Practices", "ExerciseID", "dbo.Exercises");
            DropForeignKey("dbo.Schedules", "UserID", "dbo.Users");
            DropForeignKey("dbo.ScheduleExercises", "Exercise_ID", "dbo.Exercises");
            DropForeignKey("dbo.ScheduleExercises", "Schedule_ID", "dbo.Schedules");
            DropIndex("dbo.ScheduleExercises", new[] { "Exercise_ID" });
            DropIndex("dbo.ScheduleExercises", new[] { "Schedule_ID" });
            DropIndex("dbo.Practices", new[] { "ExerciseID" });
            DropIndex("dbo.Practices", new[] { "ScheduleID" });
            DropIndex("dbo.Schedules", new[] { "UserID" });
            DropTable("dbo.ScheduleExercises");
            DropTable("dbo.Practices");
            DropTable("dbo.Users");
            DropTable("dbo.Schedules");
            DropTable("dbo.Exercises");
        }
    }
}
