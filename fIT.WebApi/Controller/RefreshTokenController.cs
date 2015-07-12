using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// Grants access to refreshtoken data
    /// </summary>
    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokensController : BaseApiController
    {
        /// <summary>
        /// Gets all refresh tokens
        /// </summary>
        /// <response code="500">Internal Server Error</response>
        [Authorize(Roles = "Admin")]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            return Ok(await this.AppRepository.RefreshTokens.GetAllAsync());
        }

        /// <summary>
        /// Deletes a Refreshtoken
        /// </summary>
        /// <param name="tokenId">tokenID</param>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal Server Error</response>
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
