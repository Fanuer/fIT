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
using BarChart;
using fITNat.DBModels;

namespace fITNat.Activities
{
    [Activity(Label = "fITNat")]
    class StatisticActivity : Activity
    {
        private int exerciseId;
        private int scheduleId;
        private string userId;
        private OnOffService ooService;

        protected async override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                ooService = new OnOffService();

                scheduleId = Intent.GetIntExtra("Schedule", 0);
                exerciseId = Intent.GetIntExtra("Exercise", 0);
                userId = Intent.GetStringExtra("User");
                List<Practice> practices = new List<Practice>();

                practices = await ooService.getAllPracticesAsync(userId, scheduleId, exerciseId);

                List<float> zahlen = new List<float>();
                //Dictionary<DateTime, double> daten = new Dictionary<DateTime, double>();
                foreach (var item in practices)
                {
                    zahlen.Add(Convert.ToSingle(item.Repetitions * item.NumberOfRepetitions * item.Weight));
                    //daten.Add(item.Timestamp, item.Repetitions * item.NumberOfRepetitions * item.Weight);
                }
                
                var data = zahlen.ToArray();
                var chart = new BarChartView(this)
                {
                    ItemsSource = Array.ConvertAll(data, v => new BarModel { Value = v })
                };
                AddContentView(chart, new ViewGroup.LayoutParams(
                  ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Fehler in der StatisticActivity: " + ex.StackTrace);
            }
        }
    }
}