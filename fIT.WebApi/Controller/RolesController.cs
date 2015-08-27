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
using Swashbuckle.Swagger.Annotations;

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// Controler to manage User roles
    /// </summary>
    [SwaggerResponse(HttpStatusCode.Unauthorized, "You are not allowed to receive this resource")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "An internal Server error has occured")]
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RoleModel))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetRole(string id)
        {
            var role = await this.AppRoleManager.FindByIdAsync(id);

            if (role != null)
            {
                return Ok(TheModelFactory.CreateViewModel(role));
            }
            return NotFound();
        }

        /// <summary> 
        /// Get a single Role by its name
        /// </summary>
        /// <param name="name">name of the role</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [Route("{name}", Name = "GetRoleByName")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RoleModel))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetRoleByName(string name)
        {
          var role = await this.AppRoleManager.FindByNameAsync(name);

          if (role != null)
          {
            return Ok(TheModelFactory.CreateViewModel(role));
          }
          return NotFound();
        }

        /// <summary>
        /// Returns a list of roles
        /// </summary>
        /// <response code="500">Internal Server Error</response>
        [Route("", Name = "GetAllRoles")]
        [EnableQuery]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<RoleModel>))]
        public IQueryable<RoleModel> GetAllRoles()
        {
            return this.AppRoleManager.Roles.ToList().Select(role => TheModelFactory.CreateViewModel(role)).AsQueryable();
        }

        /// <summary>
        /// Creates a new Role
        /// </summary>
        /// <param name="model">role data</param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(RoleModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IHttpActionResult> Create(CreateRoleModel model)
        {
            var currentRole = await AppRoleManager.FindByNameAsync(model.Name);
            if (currentRole != null)
            {
                ModelState.AddModelError("Name", "A role with this name already exists");
            }
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

            return Created(locationHeader, TheModelFactory.CreateViewModel(role));

        }

        /// <summary>
        /// Deletes a Role
        /// </summary>
        /// <param name="id">Id of the role to delete</param>
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
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
        [Route("ManageUsersInRole")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpPut]
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
