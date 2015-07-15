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
    /// <summary>
    /// Grants access to practice data
    /// </summary>
    [Authorize]
    [RoutePrefix("api/practice")]
    public class PracticeController : BaseApiController
    {
        /// <summary>
        /// Get all PracticeEntries
        /// </summary>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [EnableQuery]
        [Route("")]
        [ResponseType(typeof(IQueryable<PracticeModel>))]
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

        /// <summary>
        /// Get one practice by id
        /// </summary>
        /// <param name="id">id of the practice</param>
        /// <response code="400">Bad request</response>  
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
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

        /// <summary>
        /// Updates an existing practice
        /// </summary>
        /// <param name="practice">New practice data</param>
        /// <param name="id">Practice id</param>
        /// <response code="400">Bad request</response>  
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
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

        /// <summary>
        /// Create new practice for the logged in user
        /// </summary>
        /// <param name="practice">Practice data to create</param>
        /// <response code="201">Created</response>  
        /// <response code="400">Bad request</response> 
        /// <response code="500">Internal Server Error</response>
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

        /// <summary>
        /// Removes a schedule
        /// </summary>
        /// <param name="id">Id of the schedule to delete</param>
        /// <response code="400">Bad request</response>  
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
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
