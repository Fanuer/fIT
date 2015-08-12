using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models
{
    public class EnumValueModel<T>
    {
        public T Value { get; set; }
        public string DisplayName { get; set; }
    }
}
