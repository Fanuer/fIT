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
using fIT.WebApi.Entities;
using fIT.WebApi.Models;
using Swashbuckle.Swagger.Annotations;

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// Grants access to exercise data
    /// </summary>
    [Authorize]
    [RoutePrefix("api/exercise")]
    [SwaggerResponse(HttpStatusCode.Unauthorized, "You are not allowed to receive this resource")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "An internal Server error has occured")]
    public class ExerciseController : BaseApiController
    {
        /// <summary>
        /// Get all Exercises
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ExerciseModel>))]
        [HttpGet]
        [EnableQuery]
        [Route("")]
        public async Task<IQueryable<ExerciseModel>> GetExercises()
        {
            var all = await this.AppRepository.Exercise.GetAllAsync();
            return all.Select(this.TheModelFactory.CreateViewModel).AsQueryable();
        }

        /// <summary>
        /// Get one Exercise
        /// </summary>
        /// <param name="id">exercise id</param>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ExerciseModel))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpGet]
        [Route("{id:int}", Name = "GetExerciseById")]
        public async Task<IHttpActionResult> GetExercise(int id)
        {
            var exercise = await this.AppRepository.Exercise.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            return Ok(this.TheModelFactory.CreateViewModel(exercise));
        }

        /// <summary>
        /// Updates an existing exercise
        /// </summary>
        /// <param name="id">Id of the exercise to update</param>
        /// <param name="exercise">new exercise data</param>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateExercise([FromUri]int id, [FromBody] ExerciseModel exercise)
        {
            if (id != exercise.Id)
            {
                ModelState.AddModelError("id", "The given id have to be the same as in the model");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exists = await this.AppRepository.Exercise.ExistsAsync(id);

            try
            {
                var orig = await this.AppRepository.Exercise.FindAsync(id);
                orig = this.TheModelFactory.CreateModel(exercise, orig);
                await this.AppRepository.Exercise.UpdateAsync(orig);
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
        /// Creates a new Excercise
        /// </summary>
        /// <param name="exercise">new exercise data</param>
        /// <response code="201">Created</response>  
        /// <response code="400">Bad request</response> 
        /// <response code="500">Internal Server Error</response>
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(ExerciseModel))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [Route("")]
        [Authorize(Roles="Admin")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateExercise(ExerciseModel exercise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datamodel = this.TheModelFactory.CreateModel(exercise);
            await this.AppRepository.Exercise.AddAsync(datamodel);
            exercise = this.TheModelFactory.CreateViewModel(datamodel);
            return CreatedAtRoute("GetExerciseById", new { id = exercise.Id }, exercise);
        }

        /// <summary>
        /// Removes a exercise
        /// </summary>
        /// <param name="id">Id of the exercise to delete</param>
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteExercise(int id)
        {
            var exercise = await this.AppRepository.Exercise.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await this.AppRepository.Exercise.RemoveAsync(exercise);
            return Ok();
        }
    }
}
