using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using fIT.WebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// Controler to manage User roles
    /// </summary>
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/Roles")]
    public class RolesController : BaseApiController
    {
        /// <summary> 
        /// Get a single Role
        /// </summary>
        /// <param name="id">Guid of the role</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRole(string id)
        {
            var role = await this.AppRoleManager.FindByIdAsync(id);

            if (role != null)
            {
                return Ok(TheModelFactory.Create(role));
            }
            return NotFound();
        }

        /// <summary>
        /// Returns a list of roles
        /// </summary>
        /// <response code="500">Internal Server Error</response>
        [Route("", Name = "GetAllRoles")]
        [EnableQuery]
        public IQueryable<IdentityRole> GetAllRoles()
        {
            return this.AppRoleManager.Roles;
        }

        /// <summary>
        /// Creates a new Role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Create")]
        [ResponseType(typeof(CreateRoleModel))]
        public async Task<IHttpActionResult> Create(CreateRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new IdentityRole { Name = model.Name };

            var result = await this.AppRoleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            Uri locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Created(locationHeader, TheModelFactory.Create(role));

        }

        /// <summary>
        /// Deletes a Role
        /// </summary>
        /// <param name="id">Id of the role to delete</param>
        /// <response code="400">Bad request</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteRole(string id)
        {
            var role = await this.AppRoleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await this.AppRoleManager.DeleteAsync(role);
                return !result.Succeeded ? GetErrorResult(result) : Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// Alters the settings between users and roles
        /// </summary>
        /// <param name="model">new settings</param>
        /// <returns></returns>
        [Route("ManageUsersInRole")]
        [ResponseType(typeof(UsersInRoleModel))]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleModel model)
        {
            var role = await this.AppRoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            foreach (string user in model.EnrolledUsers)
            {
                var appUser = await this.AppUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", String.Format("User: {0} does not exists", user));
                    continue;
                }

                if (!this.AppUserManager.IsInRole(user, role.Name))
                {
                    IdentityResult result = await this.AppUserManager.AddToRoleAsync(user, role.Name);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", String.Format("User: {0} could not be added to role", user));
                    }

                }
            }

            foreach (string user in model.RemovedUsers)
            {
                var appUser = await this.AppUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", String.Format("User: {0} does not exists", user));
                    continue;
                }

                IdentityResult result = await this.AppUserManager.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", String.Format("User: {0} could not be removed from role", user));
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}
