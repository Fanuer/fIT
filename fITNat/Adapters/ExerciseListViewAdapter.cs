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
using fIT.WebApi.Client.Data.Models.Exercise;

namespace fITNat
{
    class ExerciseListViewAdapter : BaseAdapter<ExerciseModel>
    {
        private List<ExerciseModel> mItems;
        private Context mContext;
        private Guid mUserId;
        public int scheduleID { get; set; }

        public ExerciseListViewAdapter(Context context, List<ExerciseModel> items, Guid userId)
        {
            mItems = items;
            mContext = context;
            mUserId = userId;
        }

        public override int Count
        {
            get { return mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override ExerciseModel this[int position]
        {
            get { return mItems[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ExerciseView, null, false);
            }

            TextView txtExerciseViewName = row.FindViewById<TextView>(Resource.Id.txtExerciseViewName);
            TextView txtID = row.FindViewById<TextView>(Resource.Id.txtExerciseViewID);
            TextView txtExerciseViewDescription = row.FindViewById<TextView>(Resource.Id.txtExerciseViewDescription);
            TextView txtExerciseViewScheduleID = row.FindViewById<TextView>(Resource.Id.txtExerciseViewScheduleID);
            TextView txtExerciseViewUserID = row.FindViewById<TextView>(Resource.Id.txtExerciseViewUserID);
            txtExerciseViewName.Text = mItems[position].Name;
            txtID.Text = mItems[position].Id.ToString();
            txtExerciseViewDescription.Text = mItems[position].Description;
            txtExerciseViewScheduleID.Text = scheduleID.ToString();
            txtExerciseViewUserID.Text = mUserId.ToString();

            return row;
        }
    }
}