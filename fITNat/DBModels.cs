using SQLite;
using System;

namespace fITNat.DBModels
{
    [Table("User")]
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int LocalId { get; set; }
        public bool wasOffline { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public override string ToString()
        {
            return string.Format("[User: LocalId={0}, UserId={1}, Username={2}, Password={3}]", LocalId, UserId, Username, Password);
        }
    }

    [Table("Schedule")]
    public class Schedule
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int LocalId { get; set; }
        public bool WasOffline { get; set; }
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public override string ToString()
        {
            return string.Format("[Schedule: LocalId={0}, WasOffline={1}, Id={2}, UserId={3}, Name={4}, Url={5}]", LocalId, WasOffline, Id, UserId, Name, Url);
        }
    }

    [Table("ScheduleHasExercises")]
    public class ScheduleHasExercises
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int LocalId { get; set; }
        public bool WasOffline { get; set; }
        public int ScheduleId { get; set; }
        public int ExerciseId { get; set; }
        public override string ToString()
        {
            return string.Format("[ScheduleHasExercises: LocalId={0}, WasOffline={1}, ScheduleId={2}, ExerciseId={3}]", LocalId, WasOffline, ScheduleId, ExerciseId);
        }
    }

    [Table("Exercise")]
    public class Exercise
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int LocalId { get; set; }
        public bool WasOffline { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public override string ToString()
        {
            return string.Format("[Exercise: LocalId={0}, WasOffline={1}, Id={2}, Description={3}, Name={4}, Url={5}", LocalId, WasOffline, Id, Description, Name, Url);
        }
    }

    [Table("Practice")]
    public class Practice
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int LocalId { get; set; }
        public bool WasOffline { get; set; }
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
        public override string ToString()
        {
            return string.Format("[Exercise: LocalId={0}, WasOffline={1}, Id={2}, Url={3}, Timestamp={4}, Weight={5}, NumberOfRepetitions={6}, Repetitions={7}, ScheduleId={8}, ExerciseId={9}, UserId={10}", LocalId, WasOffline, Id, Url, Timestamp, Weight, NumberOfRepetitions, Repetitions, ScheduleId, ExerciseId, UserId);
        }
    }
}