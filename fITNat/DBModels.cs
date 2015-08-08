using SQLite;
using System;

namespace fITNat.DBModels
{
    [Table("User")]
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Table("Schedule")]
    public class Schedule
    {
        [PrimaryKey, Column("_id")]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    [Table("ScheduleHasExercises")]
    public class ScheduleHasExercises
    {
        public int ScheduleId { get; set; }
        public int ExerciseId { get; set; }
    }

    [Table("Exercise")]
    public class Exercise
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    [Table("Practice")]
    public class Practice
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime Timestamp { get; set; }
        public double Weight { get; set; }
        public int NumberOfRepetitions { get; set; }
        public int Repetitions { get; set; }
        //Foreign Key
        public int ScheduleId { get; set; }
        //Foreign Key
        public int ExerciseId { get; set; }
        //Foreign Key
        public string UserId { get; set; }
    }
}