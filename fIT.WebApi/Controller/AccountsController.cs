using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using fIT.WebApi.Entities;
using fIT.WebApi.Models;
using Microsoft.AspNet.Identity;

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts")]
    public class AccountsController : BaseApiController
    {
        #region Users
        /// <summary>
        /// GET: api/Accounts/Users
        /// Gets all application Users
        /// </summary>
        /// <returns></returns>
        [Route("User")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetUsers()
        {
            //aktueller Workaround: Wenn der EF-Fehler entfernt ist, kann das ToList entfernt werden
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        /// <summary>
        /// GET: api/Accounts/Users/{id:guid}
        /// Get a user by its guid
        /// </summary>
        /// <param name="Id">user's guid</param>
        /// <returns></returns>
        [Route("User/{id:guid}", Name = "GetUserById")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        /// <summary>
        /// GET: api/Accounts/Users/{username}
        /// Get User by Username
        /// </summary>
        /// <param name="username">username to search for</param>
        /// <returns></returns>
        [Route("User/{username}")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }
        
        /// <summary>
        /// DELETE: api/Accounts/Users/{id:guid}
        /// Admin can delete User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("User/{id:guid}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var appUser = await this.AppUserManager.FindByIdAsync(id);
            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                return Ok();
            }
            return NotFound();
        }

        /// <summary>
        /// Returns the current users Information
        /// </summary>
        /// <returns></returns>
        [Route("User")]
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            var currentUserId = User.Identity.GetUserId();
            var user = await this.AppUserManager.FindByIdAsync(currentUserId);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }
            return NotFound();
        }

        /// <summary>
        /// POST: api/Accounts/register
        /// User can register to the Application
        /// </summary>
        /// <param name="createUserModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(RegisterUserModel createUserModel)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                Gender = createUserModel.Gender,
                Fitness = createUserModel.Fitness,
                Job = createUserModel.Job,
                Age = createUserModel.Age

            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        /// <summary>
        /// POST: api/Accounts/ChangePassword
        /// User can change its password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        #endregion

        #region Roles
        /// <summary>
        /// PUT: User/{id:guid}/roles
        /// Assigned the user to the given roles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rolesToAssign"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {
            var appUser = await this.AppUserManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            var currentRoles = await this.AppUserManager.GetRolesAsync(appUser.Id);
            var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Any())
            {
                ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return BadRequest(ModelState);
            }

            IdentityResult removeResult = await this.AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            IdentityResult addResult = await this.AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();
        }
        #endregion
    }
}
