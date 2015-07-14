using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using fIT.WebApi.Repository.Interfaces.CRUD;

namespace fIT.WebApi.Models
{
    public class PracticeModel: IEntity<int>
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double Weight { get; set; }
        public int NumberOfRepetitions { get; set; }
        public int Repetitions { get; set; }
        public int ScheduleId { get; set; }
        public int ExerciseId { get; set; }
    }
}