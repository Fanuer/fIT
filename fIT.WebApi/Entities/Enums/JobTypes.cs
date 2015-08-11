using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Entities.Enums
{
    /// <summary>
    /// Art der Aktivität des Nutzers
    /// </summary>
    public enum JobTypes
    {
        /// <summary>
        /// z.B. Schüler, Student, Büroarbeiten
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Job_Easy")]
        Easy,
        /// <summary>
        /// z.B. Handwerker, Verkäufer
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Job_Middle")]
        Middle,
        /// <summary>
        /// z.B. Bauarbeiter, Landwirt
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Job_Hard")]
        Hard,
    }
}
