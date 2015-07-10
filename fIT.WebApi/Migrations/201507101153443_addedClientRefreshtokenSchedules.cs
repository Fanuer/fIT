namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedClientRefreshtokenSchedules : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
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
                .PrimaryKey(t => new { t.ID, t.ScheduleID, t.ExerciseID });
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            DropForeignKey("dbo.Schedules", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ScheduleExercises", "Exercise_ID", "dbo.Exercises");
            DropForeignKey("dbo.ScheduleExercises", "Schedule_ID", "dbo.Schedules");
            DropIndex("dbo.ScheduleExercises", new[] { "Exercise_ID" });
            DropIndex("dbo.ScheduleExercises", new[] { "Schedule_ID" });
            DropIndex("dbo.Schedules", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ScheduleExercises");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Practices");
            DropTable("dbo.Schedules");
            DropTable("dbo.Exercises");
            DropTable("dbo.Clients");
        }
    }
}
