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
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        /// <summary>
        /// Gets all application Users
        /// </summary>
        /// <returns></returns>
        [Route("users")]
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            //aktueller Workaround: Wenn der EF-Fehler entfernt ist, kann das ToList entfernt werden
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        /// <summary>
        /// Get a user by its guid
        /// </summary>
        /// <param name="Id">user's guid</param>
        /// <returns></returns>
        [Route("user/{id:guid}", Name = "GetUserById")]
        [HttpGet]
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
        /// Get User by Username
        /// </summary>
        /// <param name="username">username to search for</param>
        /// <returns></returns>
        [Route("user/{username}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Route("register")]
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

        [Route("user/{id:guid}")]
        [HttpDelete]
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
    }
}
