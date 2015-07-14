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

namespace fIT.WebApi.Controller
{
    [Authorize]
    [RoutePrefix("api/schedule")]
    public class SchedulesController : BaseApiController
    {
        /// <summary>
        /// Gets all application Users
        /// </summary>
        /// <response code="500">Internal Server Error</response>
        [Route("")]
        [HttpGet]
        [EnableQuery]
        public IQueryable<ScheduleModel> GetUsers()
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
        /// <param name="id">exercise id</param>
        /// <returns></returns>
        [ResponseType(typeof(ScheduleModel))]
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
        /// <param name="id"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("{id:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateExercise(int id, ScheduleModel schedule)
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
            catch (DbUpdateConcurrencyException e)
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
        /// Create new Schedule
        /// </summary>
        /// <param name="schedule">schedule data to create</param>
        /// <returns></returns>
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
        /// <param name="id">id of the schedule to delete</param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
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
