using System;
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

        public ExerciseModel Create(Exercise exercise)
        {
            return new ExerciseModel()
            {
                Id = exercise.Id,
                Description = exercise.Description,
                Name = exercise.Name,
                Url = _UrlHelper.Link("GetExcerciseById", new {id = exercise.Id}),
                Schedules = exercise.Schedules.Select(x => new EntryModel<int>()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = _UrlHelper.Link("GetScheduleById", new {id = x.ID})
                })
            };
        }

      public ScheduleModel Create(Schedule schedule)
      {
        return new ScheduleModel()
        {
          Id = schedule.Id,
          Name = schedule.Name,
          Url = _UrlHelper.Link("GetScheduleById", new {id = schedule.Id}),
          Exercises = schedule.Exercises.Select(x => new EntryModel<int>()
          {
            Id = x.Id,
            Name = x.Name,
            Url = _UrlHelper.Link("GetExcerciseById", new {id = x.Id}),
          })
        };
      }
    }
}
