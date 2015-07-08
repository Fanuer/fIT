using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities;
using fIT.WebApi.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace fIT.WebApi.Manager
{
    /// <summary>
    /// Gives access to Users
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        #region Ctor
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a new UserManager for each request
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var appUserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            //Configure validation logic for usernames
            appUserManager.UserValidator = new UserValidator<ApplicationUser>(appUserManager)
            {
                RequireUniqueEmail = true
            };

            //Configure validation logic for passwords
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                /*RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = true,*/
            };

            return appUserManager;
        } 
        #endregion
    }
}
