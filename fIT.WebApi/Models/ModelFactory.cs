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
        #region Field
        private UrlHelper _UrlHelper;
        #endregion

        #region Ctor
        public ModelFactory(HttpRequestMessage request)
        {
            _UrlHelper = new UrlHelper(request);
        }

        #endregion

        #region Method
        public UserModel Create(ApplicationUser datamodel)
        {
          if (datamodel == null) { throw new ArgumentNullException("datamodel"); }
            return new UserModel
            {
                Url = _UrlHelper.Link("GetUserById", new { id = datamodel.Id }),
                Id = datamodel.Id,
                UserName = datamodel.UserName,
                Email = datamodel.Email,
                DateOfBirth = datamodel.DateOfBirth,
                Job = datamodel.Job,
                Gender = datamodel.Gender,
                Fitness = datamodel.Fitness
            };
        }

        public RoleModel Create(IdentityRole datamodel)
        {
          if (datamodel == null) { throw new ArgumentNullException("datamodel"); }
            return new RoleModel
            {
                Url = _UrlHelper.Link("GetRoleById", new { id = datamodel.Id }),
                Id = datamodel.Id,
                Name = datamodel.Name
            };
        }

        public ExerciseModel Create(Exercise datamodel)
        {
          if (datamodel == null) { throw new ArgumentNullException("datamodel"); }
            return new ExerciseModel()
            {
                Id = datamodel.Id,
                Description = datamodel.Description,
                Name = datamodel.Name,
                Url = _UrlHelper.Link("GetExcerciseById", new { id = datamodel.Id }),
                Schedules = datamodel.Schedules.Select(x => new EntryModel<int>()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = _UrlHelper.Link("GetScheduleById", new { id = x.Id })
                })
            };
        }

        public ScheduleModel Create(Schedule datamodel)
        {
            if (datamodel == null) { throw new ArgumentNullException("datamodel"); }

            return new ScheduleModel()
            {
                Id = datamodel.Id,
                Name = datamodel.Name,
                Url = _UrlHelper.Link("GetScheduleById", new { id = datamodel.Id }),
                Exercises = datamodel.Exercises.Select(x => new EntryModel<int>()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = _UrlHelper.Link("GetExcerciseById", new { id = x.Id }),
                })
            };
        }

        public PracticeModel Create(Practice datamodel)
        {
            if (datamodel == null) { throw new ArgumentNullException("datamodel"); }

            return new PracticeModel()
            {
                Id = datamodel.Id,
                
                ExerciseId = datamodel.ExerciseId,
                NumberOfRepetitions = datamodel.NumberOfRepetitions,
                Repetitions = datamodel.Repetitions,
                ScheduleId = datamodel.ScheduleId,
                Timestamp = datamodel.Timestamp,
                Weight = datamodel.Weight
            };
        }

        public Schedule Update(ScheduleModel model, Schedule datamodel = null)
        {
            var result = datamodel ?? new Schedule();
            result.UserID = model.UserId;
            result.Name = model.Name;
            result.Id = model.Id;
            return result;
        }

        public Exercise Update(ExerciseModel model, Exercise datamodel = null)
        {
            var result = datamodel ?? new Exercise();
            result.Description = model.Description;
            result.Name = model.Name;
            result.Id = model.Id;
            return result;
        }

        public Practice Update(PracticeModel model, string userId, Practice datamodel = null)
        {
            var result = datamodel ?? new Practice();
            result.Timestamp = model.Timestamp;
            result.Repetitions = model.Repetitions;
            result.ExerciseId = model.ExerciseId;
            result.ScheduleId = model.ScheduleId;
            result.UserId = userId;
            result.NumberOfRepetitions = model.NumberOfRepetitions;
            result.Id = model.Id;
            return result;
        }
        #endregion
    }
}
