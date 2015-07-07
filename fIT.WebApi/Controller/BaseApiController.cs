using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using fIT.WebApi.Manager;
using fIT.WebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace fIT.WebApi.Controller
{
    public class BaseApiController : ApiController
    {
        #region Field
        private ModelFactory _modelFactory;
        private ApplicationUserManager _AppUserManager = null;
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
                    foreach (string error in result.Errors)
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
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request, this.AppUserManager);
                }
                return _modelFactory;
            }
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

        #endregion
    }
}
