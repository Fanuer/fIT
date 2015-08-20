using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using fIT.WebApi.Entities.Enums;
using fIT.WebApi.Helpers;
using fIT.WebApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// Returns all Values and Displaynames of an enumeration
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Enums")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "An internal Server error has occured")]
    public class EnumsController : BaseApiController
    {
        /// <summary>
        /// Returns all Jobs
        /// </summary>
        [ResponseType(typeof(IEnumerable<EnumValueModel<JobTypes>>))]
        [Route("JobTypes")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetJobValues()
        {
            return Ok(GetEnumValues<JobTypes>());
        }

        /// <summary>
        /// Returns all Genders
        /// </summary>
        [ResponseType(typeof(IEnumerable<EnumValueModel<GenderType>>))]
        [Route("GenderTypes")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetGenderValues()
        {
            return Ok(GetEnumValues<GenderType>());
        }

        /// <summary>
        /// Returns all Fitness Types
        /// </summary>
        [ResponseType(typeof(IEnumerable<EnumValueModel<FitnessType>>))]
        [Route("FitnessTypes")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetFitnessValues()
        {
            return Ok(GetEnumValues<FitnessType>());
        }

        private IEnumerable<EnumValueModel<T>> GetEnumValues<T>()
        {
            var enumType = typeof (T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Only enum tyües are allowed");
            }
            var values = Enum.GetValues(enumType).Cast<T>();
            return values.Select(x => new EnumValueModel<T>()
            {
                Value = x,
                DisplayName = x.GetDisplayName()
            });
        } 
    }
}
