using System;
using System.Collections.Generic;
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
        Easy,
        /// <summary>
        /// z.B. Handwerker, Verkäufer
        /// </summary>
        Middle,
        /// <summary>
        /// z.B. Bauarbeiter, Landwirt
        /// </summary>
        Hard,
    }
}
