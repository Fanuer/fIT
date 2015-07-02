using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models
{
    public class ApplicationUserDBContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationUserDBContext() : base("name=DBUserContext", false)
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public static ApplicationUserDBContext Create()
        {
            return new ApplicationUserDBContext();
        }
    }
}
