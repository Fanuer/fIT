using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace fIT.WebApi.Repository
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Ctor
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }
        #endregion

        #region Methods
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        #endregion
        
    }
}
