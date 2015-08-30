private async Task checkSync()
{
    List<Practice> offPractices = db.GetOfflinePractice();
    int result;
    if(offPractices.Count != 0)
    {
        foreach (var item in offPractices)
        {
            try
            {
                User u = db.findUser(item.UserId);
                result = await mgnService.recordPractice(item.ScheduleId, item.ExerciseId, item.UserId, item.Timestamp, item.Weight, item.Repetitions, item.NumberOfRepetitions, u.Username, u.Password);
                if (result != 0)
                    item.Id = result;
            }
            catch(Exception ex){[...] break;}
        }
    }
}