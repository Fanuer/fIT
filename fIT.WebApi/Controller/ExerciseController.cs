using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using fIT.WebApi.Entities;
using fIT.WebApi.Models;

namespace fIT.WebApi.Controller
{
    public class ExerciseController : BaseApiController
    {
        public IQueryable<ExerciseModel> GetExercises()
        {
            return this.AppRepository.Excercies.GetAllAsync().Select(this.TheModelFactory.Create).AsQueryable();
        }

        // GET: api/Exercises/5
        [ResponseType(typeof(ExerciseModel))]
        public async Task<IHttpActionResult> GetExercise(int id)
        {
            var exercise = await this.AppRepository.Excercies.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            return Ok(exercise);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutExercise(int id, Exercise exercise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != exercise.Id)
            {
                return BadRequest();
            }

          var exists = await this.AppRepository.Excercies.ExistsAsync(id); 

            try
            {
                await this.AppRepository.Excercies.UpdateAsync(id, exercise);
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

        // POST: api/Exercises
        [ResponseType(typeof(Exercise))]
        public async Task<IHttpActionResult> PostExercise(Exercise exercise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await this.AppRepository.Excercies.AddAsync(exercise);

            return CreatedAtRoute("DefaultApi", new { id = exercise.Id}, exercise);
        }

        // DELETE: api/Exercises/5
        [ResponseType(typeof(Exercise))]
        public async Task<IHttpActionResult> DeleteExercise(int id)
        {
            var exercise = await this.AppRepository.Excercies.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await this.AppRepository.Excercies.RemoveAsync(exercise);
            return Ok(exercise);
        }
    }
}
