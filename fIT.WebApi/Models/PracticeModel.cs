using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using fIT.WebApi.Repository.Interfaces.CRUD;

namespace fIT.WebApi.Models
{
    public class PracticeModel
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double Weight { get; set; }
        public int NumberOfRepetitions { get; set; }
        public int Repetitions { get; set; }
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        public int ScheduleId { get; set; }
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        public int ExerciseId { get; set; }
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        public string UserId { get; set; }
    }
}