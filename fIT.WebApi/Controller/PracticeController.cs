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

namespace fIT.WebApi.Controller
{
    [Authorize]
    [RoutePrefix("api/practice")]
    public class PracticeController : BaseApiController
    {
        /// <summary>
        /// Get all PracticeEntries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        [Route("")]
        public IQueryable<PracticeModel> GetPractices()
        {
            return this
                   .AppRepository
                   .Practices
                   .GetAllAsync()
                   .Where(x=>x.UserId.Equals(CurrentUserId))
                   .Select(this.TheModelFactory.Create)
                   .AsQueryable();
        }

        [ResponseType(typeof(PracticeModel))]
        [HttpGet]
        [Route("{id:int}", Name = "GetPracticeById")]
        public async Task<IHttpActionResult> GetPractice(int id)
        {
            var practice = await this.AppRepository.Practices.FindAsync(id);
            if (practice == null)
            {
                return NotFound();
            }
            if (!IsValidRequest(practice.UserId))
            {
                return BadRequest();
            }

            return Ok(practice);
        }

        [ResponseType(typeof(void))]
        [Route("{id:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdatePractice([FromUri]int id, [FromBody] PracticeModel practice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != practice.Id)
            {
                return BadRequest();
            }

            var exists = await this.AppRepository.Practices.ExistsAsync(id);

            try
            {
                var orig = await this.AppRepository.Practices.FindAsync(id);
                if (!IsValidRequest(orig.UserId))
                {
                    return BadRequest();
                }
                orig = this.TheModelFactory.Update(practice, this.CurrentUserId, orig);
                await this.AppRepository.Practices.UpdateAsync(id, orig);
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

        // POST: api/Practices
        [ResponseType(typeof(PracticeModel))]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> CreatePractice(PracticeModel practice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datamodel = this.TheModelFactory.Update(practice, CurrentUserId);
            await this.AppRepository.Practices.AddAsync(datamodel);
            return CreatedAtRoute("DefaultApi", new { id = practice.Id }, practice);
        }

        // DELETE: api/Practices/5
        [ResponseType(typeof(void))]
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeletePractice(int id)
        {
            var practice = await this.AppRepository.Practices.FindAsync(id);
            if (practice == null)
            {
                return NotFound();
            }
            else if (!IsValidRequest(practice.UserId))
            {
                return BadRequest();
            }
            await this.AppRepository.Practices.RemoveAsync(practice);
            return Ok();
        }

        private bool IsValidRequest(string practiceUserId)
        {
            return User.IsInRole("Admin") || practiceUserId == CurrentUserId;
        }
    }
}
