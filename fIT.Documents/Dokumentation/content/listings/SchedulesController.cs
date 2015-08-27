// Grants access to schedule data
[SwaggerResponse(HttpStatusCode.Unauthorized, "You are not allowed to receive this resource")]
[SwaggerResponse(HttpStatusCode.InternalServerError, "An internal Server error has occured")]
[Authorize] |\label{line:SchedulesController_Autorize}|
[RoutePrefix("api/schedule")] |\label{line:SchedulesController_RoutePrefix}|
public class SchedulesController : BaseApiController
{
	// Create new Schedule for the logged in user
	[SwaggerResponse(HttpStatusCode.Created, Type = typeof(ScheduleModel))]
	[SwaggerResponse(HttpStatusCode.BadRequest)]
	[Route("")] |\label{line:SchedulesController_Route}|
	[HttpPost] |\label{line:SchedulesController_HTTPVerb}|
	public async Task<IHttpActionResult> CreateSchedule(ScheduleModel schedule)
	{
		if (ModelState.IsValid && !schedule.UserId.Equals(this.CurrentUserId))
		{
			ModelState.AddModelError("UserId", "You can only create schedules for yourself");
		}
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var datamodel = this.TheModelFactory.CreateModel(schedule);
		await this.AppRepository.Schedules.AddAsync(datamodel);
		var result = this.TheModelFactory.CreateViewModel(datamodel);
		return CreatedAtRoute("GetScheduleById", new { id = schedule.Id }, result);
	}
}
