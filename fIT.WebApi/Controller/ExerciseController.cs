﻿using System;
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

namespace fIT.WebApi.Controller
{
    /// <summary>
    /// Grants access to exercise data
    /// </summary>
    [Authorize]
    [RoutePrefix("api/exercise")]
    public class ExerciseController : BaseApiController
    {

        /// <summary>
        /// Get all Exercises
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        [Route("")]
        public IQueryable<ExerciseModel> GetExercises()
        {
            return this.AppRepository.Excercies.GetAllAsync().Select(this.TheModelFactory.Create).AsQueryable();
        }

        /// <summary>
        /// Get one Exercise
        /// </summary>
        /// <param name="id">exercise id</param>
        /// <response code="400">Bad request</response>  
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [ResponseType(typeof(ExerciseModel))]
        [HttpGet]
        [Route("{id:int}", Name = "GetExerciseById")]
        public async Task<IHttpActionResult> GetExercise(int id)
        {
            var exercise = await this.AppRepository.Excercies.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            return Ok(exercise);
        }

        /// <summary>
        /// Updates an existing exercise
        /// </summary>
        /// <param name="id">Id of the exercise to update</param>
        /// <param name="exercise">new exercise data</param>
        /// <response code="400">Bad request</response>  
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [ResponseType(typeof(void))]
        [Route("{id:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateExercise([FromUri]int id, [FromBody] ExerciseModel exercise)
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
                var orig = await this.AppRepository.Excercies.FindAsync(id);
                orig = this.TheModelFactory.Update(exercise, orig);
                await this.AppRepository.Excercies.UpdateAsync(id, orig);
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
        /// Creates a new Excercise
        /// </summary>
        /// <param name="exercise">new exercise data</param>
        /// <response code="201">Created</response>  
        /// <response code="400">Bad request</response> 
        /// <response code="500">Internal Server Error</response>
        [ResponseType(typeof(ExerciseModel))]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateExercise(ExerciseModel exercise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datamodel = this.TheModelFactory.Update(exercise);
            await this.AppRepository.Excercies.AddAsync(datamodel);
            return CreatedAtRoute("DefaultApi", new { id = exercise.Id }, exercise);
        }

        /// <summary>
        /// Removes a exercise
        /// </summary>
        /// <param name="id">Id of the exercise to delete</param>
        /// <response code="400">Bad request</response>  
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [ResponseType(typeof(void))]
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteExercise(int id)
        {
            var exercise = await this.AppRepository.Excercies.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            await this.AppRepository.Excercies.RemoveAsync(exercise);
            return Ok();
        }
    }
}
