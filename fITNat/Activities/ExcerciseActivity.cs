using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace fITNat
{
    [Activity(Label = "ExcerciseActivity")]
    public class ExcerciseActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ExerciseLayout);

            List<string> exercises = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                exercises.Add(i + ". Exercise");
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.ExerciseView, Resource.Id.txtExerciseViewDescription, exercises);
            ListView lv = (ListView)FindViewById(Resource.Id.lvExercise);
            lv.Adapter = adapter;
        }
    }
}