private async void bt_ItemClick(object sender, EventArgs e)
{
    try
    {
        scheduleId = Intent.GetIntExtra("Schedule", 0);
        exerciseId = Intent.GetIntExtra("Exercise", 0);
        userId = Intent.GetStringExtra("User");
        double weight = Double.Parse(txtWeight.Text);
        int repetitions = Java.Lang.Integer.ParseInt(txtRepetitions.Text);
        int numberOfRepetitions = Java.Lang.Integer.ParseInt(txtNumberOfRepetitions.Text);

        bool result = await ooService.createPracticeAsync(scheduleId, exerciseId, userId, DateTime.Now, weight, repetitions, numberOfRepetitions);
        if(result)
        { 
            //Zurueck zu der Uebungsseite
            OnBackPressed();
        }
        else
        {
            new AlertDialog.Builder(this)
                .SetMessage("Anlegen ist fehlgeschlagen")
                .SetTitle("Error")
                .Show();
        }
    }
    catch (ServerException ex){[...]}
    catch(FormatException exc){[...]}
    catch (Exception exce){[...]}
} 