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
                        OwnerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.OwnerID, cascadeDelete: true)
                .Index(t => t.OwnerID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Gender = c.Int(),
                        Password = c.String(nullable: false),
                        Job = c.Int(),
                        Fitness = c.Int(),
                        Age = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Practices",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ScheduleID = c.Int(nullable: false),
                        ExerciseID = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Weight = c.Int(nullable: false),
                        NumberOfRepetitions = c.Int(nullable: false),
                        Repetitions = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ID, t.ScheduleID, t.ExerciseID })
                .ForeignKey("dbo.Exercises", t => t.ExerciseID, cascadeDelete: true)
                .ForeignKey("dbo.Schedules", t => t.ScheduleID, cascadeDelete: true)
                .Index(t => t.ScheduleID)
                .Index(t => t.ExerciseID);
            
            CreateTable(
                "dbo.ScheduleExercise",
                c => new
                    {
                        ExerciseRefId = c.Int(nullable: false),
                        SchaduleRefId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ExerciseRefId, t.SchaduleRefId })
                .ForeignKey("dbo.Exercises", t => t.ExerciseRefId, cascadeDelete: true)
                .ForeignKey("dbo.Schedules", t => t.SchaduleRefId, cascadeDelete: true)
                .Index(t => t.ExerciseRefId)
                .Index(t => t.SchaduleRefId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Practices", "ScheduleID", "dbo.Schedules");
            DropForeignKey("dbo.Practices", "ExerciseID", "dbo.Exercises");
            DropForeignKey("dbo.ScheduleExercise", "SchaduleRefId", "dbo.Schedules");
            DropForeignKey("dbo.ScheduleExercise", "ExerciseRefId", "dbo.Exercises");
            DropForeignKey("dbo.Schedules", "OwnerID", "dbo.Users");
            DropIndex("dbo.ScheduleExercise", new[] { "SchaduleRefId" });
            DropIndex("dbo.ScheduleExercise", new[] { "ExerciseRefId" });
            DropIndex("dbo.Practices", new[] { "ExerciseID" });
            DropIndex("dbo.Practices", new[] { "ScheduleID" });
            DropIndex("dbo.Schedules", new[] { "OwnerID" });
            DropTable("dbo.ScheduleExercise");
            DropTable("dbo.Practices");
            DropTable("dbo.Users");
            DropTable("dbo.Schedules");
            DropTable("dbo.Exercises");
        }
    }
}
