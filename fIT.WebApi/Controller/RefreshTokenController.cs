using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using fIT.WebApi.Entities;
using fIT.WebApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// Grants access to refreshtoken data
    /// </summary>
    [SwaggerResponse(HttpStatusCode.InternalServerError, "An internal Server error has occured")]
    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokensController : BaseApiController
    {
        /// <summary>
        /// Gets all refresh tokens
        /// </summary>
        /// <response code="500">Internal Server Error</response>
        [Authorize(Roles = "Admin")]
        [Route("")]
        [EnableQuery]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<RefreshTokenModel>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "You are not allowed to receive this resource")]
      public async Task<IQueryable<RefreshTokenModel>> Get()
        {
            var all = await this
                            .AppRepository
                            .RefreshTokens
                            .GetAllAsync();

            return all.Select(x => this.TheModelFactory.Create(x)).AsQueryable();
        }

        /// <summary>
        /// Deletes a Refreshtoken
        /// </summary>
        /// <param name="tokenId">tokenID</param>
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            var result = await this.AppRepository.RefreshTokens.RemoveAsync(tokenId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.AppRepository.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
