using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fIT.WebApi.Models
{
    /// <summary>
    /// Defines one Exercise
    /// </summary>
    public class ExerciseModel : EntryModel<int>
    {
        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Zugehoerige Trainingsplaene
        /// </summary>
        public IEnumerable<EntryModel<int>> Schedules { get; set; }
    }
}
