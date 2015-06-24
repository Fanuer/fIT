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
    public class PracticesController : ApiController
    {
        private DBContext db = new DBContext();

        // GET: api/Practices
        public IQueryable<Practice> GetPractices()
        {
            return db.Practices;
        }

        // GET: api/Practices/5
        [ResponseType(typeof(Practice))]
        public async Task<IHttpActionResult> GetPractice(int id)
        {
            Practice practice = await db.Practices.FindAsync(id);
            if (practice == null)
            {
                return NotFound();
            }

            return Ok(practice);
        }

        // PUT: api/Practices/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPractice(int id, Practice practice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != practice.ID)
            {
                return BadRequest();
            }

            db.Entry(practice).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PracticeExists(id))
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

        // POST: api/Practices
        [ResponseType(typeof(Practice))]
        public async Task<IHttpActionResult> PostPractice(Practice practice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Practices.Add(practice);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PracticeExists(practice.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = practice.ID }, practice);
        }

        // DELETE: api/Practices/5
        [ResponseType(typeof(Practice))]
        public async Task<IHttpActionResult> DeletePractice(int id)
        {
            Practice practice = await db.Practices.FindAsync(id);
            if (practice == null)
            {
                return NotFound();
            }

            db.Practices.Remove(practice);
            await db.SaveChangesAsync();

            return Ok(practice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PracticeExists(int id)
        {
            return db.Practices.Count(e => e.ID == id) > 0;
        }
    }
}