using System.ComponentModel.DataAnnotations;

namespace fIT.WebApi.Entities.Enums
{
    /// <summary>
    /// Art der Fitness eines Nutzers
    /// </summary>
    public enum FitnessType
    {
        /// <summary>
        /// Nutzer macht keinen Sport
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Fitness_NoSport")]
        NoSport,
        /// <summary>
        /// Nutzer macht ein mal die Woche Sport
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Fitness_OnceAWeek")]
        OnceAWeek,
        /// <summary>
        /// Nutzer macht mehr als einmal die Woche Sport
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Fitness_MoreThanOnceAWeek")]
        MoreThanOnceAWeek,
        /// <summary>
        /// Hochleistungssportler
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Fitness_HighPerformanceAthletes")]
        HighPerformanceAthletes
    }
}
