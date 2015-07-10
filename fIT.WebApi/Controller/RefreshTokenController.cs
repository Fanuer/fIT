﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace fIT.WebApi.Controller
{
  [RoutePrefix("api/RefreshTokens")]
  public class RefreshTokensController : BaseApiController
  {
    [Authorize(Roles = "Admin")]
    [Route("")]
    public async Task<IHttpActionResult> Get()
    {
      return Ok(await this.AppRepository.RefreshTokens.GetAllAsync());
    }

    //[Authorize(Users = "Admin")]
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
