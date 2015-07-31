using System;
using fIT.WebApi.Client.Data.Models.Shared;

namespace fIT.WebApi.Client.Data.Models.Practice
{
    /// <summary>
    /// Training
    /// </summary>
    public class PracticeModel
    {
        /// <summary>
        /// Url zu dieser Resource
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// UTC Datum
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Gewicht
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// Sets
        /// </summary>
        public int NumberOfRepetitions { get; set; }
        /// <summary>
        /// Wiederholungen pro Set
        /// </summary>
        public int Repetitions { get; set; }
        /// <summary>
        /// Trainingsplan
        /// </summary>
        public int ScheduleId { get; set; }
        /// <summary>
        /// Uebung
        /// </summary>
        public int ExerciseId { get; set; }
    }
}