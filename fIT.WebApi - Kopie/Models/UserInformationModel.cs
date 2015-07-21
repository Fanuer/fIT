using fIT.WebApi.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models
{
    public class UserInformationModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Nutzer ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Geschlecht
        /// </summary>
        public GenderType? Gender { get; set; }

        /// <summary>
        /// Berufsfeld
        /// </summary>
        public JobTypes? Job { get; set; }

        /// <summary>
        /// Fittness
        /// </summary>
        public FitnessType? Fitness { get; set; }

        /// <summary>
        /// Alter
        /// </summary>
        public int Age { get; set; }
    }
}
