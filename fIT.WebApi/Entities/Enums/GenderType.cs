using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Entities.Enums
{
    /// <summary>
    /// Geschlecht
    /// </summary>
    public enum GenderType
    {
        /// <summary>
        /// männlich
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Gender_Male")]
        Male,
        /// <summary>
        /// weiblich
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Gender_Female")]
        Female,
        /// <summary>
        /// Rest
        /// </summary>
        [Display(ResourceType = typeof (Resources), Name = "Enum_Gender_Others")]
        Other
    }
}
