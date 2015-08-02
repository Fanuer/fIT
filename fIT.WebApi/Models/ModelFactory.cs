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
            var result = new ExerciseModel()
            {
                Id = datamodel.Id,
                Description = datamodel.Description,
                Name = datamodel.Name,
                Url = _UrlHelper.Link("GetExerciseById", new { id = datamodel.Id })
            };
            if (datamodel.Schedules != null)
            {
                result.Schedules = datamodel.Schedules.Select(x => new EntryModel<int>()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = _UrlHelper.Link("GetScheduleById", new { id = x.Id })
                });
            }
            return result;
        }

        public ScheduleModel Create(Schedule datamodel)
        {
            if (datamodel == null) { throw new ArgumentNullException("datamodel"); }

            var result = new ScheduleModel()
            {
                Id = datamodel.Id,
                Name = datamodel.Name,
                UserId = datamodel.UserID,
                Url = _UrlHelper.Link("GetScheduleById", new { id = datamodel.Id })
            };

            if (datamodel.Exercises != null)
            {
                result.Exercises = datamodel.Exercises.Select(x => new EntryModel<int>()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = _UrlHelper.Link("GetExcerciseById", new { id = x.Id }),
                });
            }

            return result;
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
                Weight = datamodel.Weight,
                Url = _UrlHelper.Link("GetPracticeById", new { id = datamodel.Id })
            };
        }

        public RefreshTokenModel Create(RefreshToken datamodel)
        {
            if (datamodel == null) { throw new ArgumentNullException("datamodel"); }

            return new RefreshTokenModel()
            {
                Id = datamodel.Id,
                ClientId = datamodel.ClientId,
                Subject = datamodel.Subject,
                ExpiresUtc = datamodel.ExpiresUtc,
                IssuedUtc = datamodel.IssuedUtc
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

        public Practice Update(PracticeModel model, Practice datamodel = null)
        {
            var result = datamodel ?? new Practice();
            result.Timestamp = model.Timestamp;
            result.Repetitions = model.Repetitions;
            result.ExerciseId = model.ExerciseId;
            result.ScheduleId = model.ScheduleId;
            result.UserId = model.UserId;
            result.Weight = model.Weight;
            result.NumberOfRepetitions = model.NumberOfRepetitions;
            result.Id = model.Id;
            return result;
        }

        internal ApplicationUser Update(UserModel model, ApplicationUser datamodel = null)
        {
            var result = datamodel ?? new ApplicationUser();
            result.DateOfBirth = model.DateOfBirth;
            result.Fitness = model.Fitness;
            result.Gender = model.Gender;
            result.Job = model.Job;
            result.UserName = model.UserName;
            result.Email = model.Email;
            return result;
        }
        #endregion
    }
}
