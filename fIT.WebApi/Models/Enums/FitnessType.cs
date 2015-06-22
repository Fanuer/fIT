using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models.Enums
{
    /// <summary>
    /// Art der Fittness eines Nutzers
    /// </summary>
    public enum FitnessType
    {
        /// <summary>
        /// Nutzer macht keinen Sport
        /// </summary>
        NoSport,
        /// <summary>
        /// Nutzer macht ein mal die Woche Sport
        /// </summary>
        OnceAWeek,
        /// <summary>
        /// Nutzer macht mehr als einmal die Woche Sport
        /// </summary>
        MoreThanOnceAWeek,
        /// <summary>
        /// Hochleistungssportler
        /// </summary>
        HighPerformanceAthletes
    }
}
