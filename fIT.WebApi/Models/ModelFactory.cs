﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using fIT.WebApi.Entities;
using fIT.WebApi.Manager;
using Microsoft.AspNet.Identity.EntityFramework;

namespace fIT.WebApi.Models
{
  public class ModelFactory
  {
    private UrlHelper _UrlHelper;
    private ApplicationUserManager _AppUserManager;

    public ModelFactory(HttpRequestMessage request, ApplicationUserManager appUserManager)
    {
      _UrlHelper = new UrlHelper(request);
      _AppUserManager = appUserManager;
    }

    public UserModel Create(ApplicationUser appUser)
    {
      return new UserModel
      {
        Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
        Id = appUser.Id,
        UserName = appUser.UserName,
        Email = appUser.Email,
        EmailConfirmed = appUser.EmailConfirmed,
        Roles = _AppUserManager.GetRolesAsync(appUser.Id).Result,
        Claims = _AppUserManager.GetClaimsAsync(appUser.Id).Result,
        Age = appUser.Age,
        Job = appUser.Job,
        Gender = appUser.Gender,
        Fitness = appUser.Fitness
      };
    }

    public RoleModel Create(IdentityRole appRole)
    {
      return new RoleModel
      {
        Url = _UrlHelper.Link("GetRoleById", new { id = appRole.Id }),
        Id = appRole.Id,
        Name = appRole.Name
      };
    }
  }
}
