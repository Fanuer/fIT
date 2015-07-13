using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities.Enums;

namespace fIT.WebApi.Models
{
    public class UserModel
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Geschlecht
        /// </summary>
        public GenderType Gender { get; set; }
        /// <summary>
        /// Berufsfeld
        /// </summary>
        public JobTypes Job { get; set; }
        /// <summary>
        /// Fittness
        /// </summary>
        public FitnessType Fitness { get; set; }
        /// <summary>
        /// Alter
        /// </summary>
        public int Age { get; set; }
    }
}
