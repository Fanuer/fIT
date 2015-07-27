using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using fIT.WebApi.Models;
using Microsoft.AspNet.Identity;
using Swashbuckle.Swagger.Annotations;

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// Grants access to schedule data
    /// </summary>
    [SwaggerResponse(HttpStatusCode.Unauthorized, "You are not allowed to receive this resource")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "An internal Server error has occured")]
    [Authorize]
    [RoutePrefix("api/schedule")]
    public class SchedulesController : BaseApiController
    {
        /// <summary>
        /// Gets all schedules of a user or all, if logged in as an admin
        /// </summary>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<PracticeModel>))]
        [Route("")]
        [HttpGet]
        [EnableQuery]
        public IQueryable<ScheduleModel> GetSchedules()
        {
            return this.AppRepository
                       .Schedules
                       .GetAllAsync()
                       .Where(x => IsValidSchedule(x.UserID))
                       .Select(this.TheModelFactory.Create)
                       .AsQueryable();
        }

        /// <summary>
        /// Get one Schedule
        /// </summary>
        /// <param name="id">Schedule id</param>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ScheduleModel))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpGet]
        [Route("{id:int}", Name = "GetScheduleById")]
        public async Task<IHttpActionResult> GetSchedule(int id)
        {
            var schedule = await this.AppRepository.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            if (!this.IsValidSchedule(schedule.UserID))
            {
                return BadRequest();
            }
            return Ok(schedule);
        }

        /// <summary>
        /// Updates an existing schedule
        /// </summary>
        /// <param name="schedule">New schedule Data</param>
        /// <param name="id">Schedule id</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("{id:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateExercise([FromUri]int id, [FromBody]ScheduleModel schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != schedule.Id || !IsValidSchedule(schedule.UserId))
            {
                return BadRequest();
            }

            var exists = await this.AppRepository.Schedules.ExistsAsync(id);

            try
            {
                var orig = await this.AppRepository.Schedules.FindAsync(id);
                orig = this.TheModelFactory.Update(schedule, orig);
                await this.AppRepository.Schedules.UpdateAsync(id, orig);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!exists)
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Create new Schedule for the logged in user
        /// </summary>
        /// <param name="schedule">Schedule data to create</param>
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(PracticeModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [ResponseType(typeof(ScheduleModel))]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateExercise(ScheduleModel schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datamodel = this.TheModelFactory.Update(schedule);
            await this.AppRepository.Schedules.AddAsync(datamodel);
            return CreatedAtRoute("DefaultApi", new { id = schedule.Id }, schedule);
        }

        /// <summary>
        /// Removes a schedule
        /// </summary>
        /// <param name="id">Id of the schedule to delete</param>
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteExercise(int id)
        {
            var schedule = await this.AppRepository.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            else if (!IsValidSchedule(schedule.UserID))
            {
                return BadRequest();
            }

            await this.AppRepository.Schedules.RemoveAsync(schedule);
            return Ok();
        }

        private bool IsValidSchedule(string schedulesUserId)
        {
            return schedulesUserId != User.Identity.GetUserId();
        }
    }
}
