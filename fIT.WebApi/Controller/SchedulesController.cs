using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using fIT.WebApi.Models;

namespace fIT.WebApi.Controller
{
    public class SchedulesController : ApiController
    {
        private DBContext db = new DBContext();

        // GET: api/Schedules
        public IQueryable<Schedule> GetSchedules()
        {
            return db.Schedules;
        }

        // GET: api/Schedules/5
        [ResponseType(typeof(Schedule))]
        public async Task<IHttpActionResult> GetSchedule(int id)
        {
            Schedule schedule = await db.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

        // PUT: api/Schedules/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSchedule(int id, Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != schedule.ID)
            {
                return BadRequest();
            }

            db.Entry(schedule).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Schedules
        [ResponseType(typeof(Schedule))]
        public async Task<IHttpActionResult> PostSchedule(Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Schedules.Add(schedule);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = schedule.ID }, schedule);
        }

        // DELETE: api/Schedules/5
        [ResponseType(typeof(Schedule))]
        public async Task<IHttpActionResult> DeleteSchedule(int id)
        {
            Schedule schedule = await db.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            db.Schedules.Remove(schedule);
            await db.SaveChangesAsync();

            return Ok(schedule);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ScheduleExists(int id)
        {
            return db.Schedules.Count(e => e.ID == id) > 0;
        }
    }
}