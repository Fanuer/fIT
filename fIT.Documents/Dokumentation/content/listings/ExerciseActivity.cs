/// <summary>
/// Clickevent auf ein Element des ListViews
/// Geht zu dem ausgewaehlten Training
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
{
    string selectedExerciseName = exercises[e.Position].Name.ToString();
    int exerciseId = Integer.ParseInt(exercises[e.Position].Id.ToString());
    string selectedExerciseDescription = exercises[e.Position].Description.ToString();

    var practiceActivity = new Intent(this, typeof(PracticeActivity));
    practiceActivity.PutExtra("Exercise", exerciseId);
    practiceActivity.PutExtra("Schedule", scheduleId);
    practiceActivity.PutExtra("User", userId);
    StartActivity(practiceActivity);
}