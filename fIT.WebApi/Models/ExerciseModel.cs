using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace fIT.WebApi.Models
{
    public class ExerciseModel: EntryModel
    {
        public string Description { get; set; }
        public IEnumerable<EntryModel> Schedules { get; set; }
    }
}
