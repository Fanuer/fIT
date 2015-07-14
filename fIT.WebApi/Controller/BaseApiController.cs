using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using fIT.WebApi.Manager;
using fIT.WebApi.Models;
using fIT.WebApi.Repository.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace fIT.WebApi.Controller
{
    public class BaseApiController : ApiController
    {
        #region Field
        private ModelFactory _modelFactory;
        private ApplicationUserManager _AppUserManager = null;
        private ApplicationRoleManager _AppRoleManager = null;
        private IRepository _rep = null;
        #endregion

        #region Ctor
        public BaseApiController()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Wraps the modelerrror to a request
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Singleton Modelfactory
        /// </summary>
        protected ModelFactory TheModelFactory
        {
            get { return _modelFactory ?? (_modelFactory = new ModelFactory(this.Request)); }
        }

        /// <summary>
        /// Returns a single User-Manager per Request
        /// </summary>
        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return _AppUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        /// <summary>
        /// Manages User Roles
        /// </summary>
        protected ApplicationRoleManager AppRoleManager
        {
            get { return _AppRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>(); }
        }

        /// <summary>
        /// Repository
        /// </summary>
        protected IRepository AppRepository
        {
            get { return this._rep ?? Request.GetOwinContext().Get<IRepository>(); }
        }

        /// <summary>
        /// Current User Id
        /// </summary>
        protected string CurrentUserId
        {
            get { return this.User.Identity.GetUserId(); }
        }
        #endregion
    }
}
