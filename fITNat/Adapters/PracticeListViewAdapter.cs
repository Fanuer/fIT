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
using fIT.WebApi.Client.Data.Models.Practice;

namespace fITNat
{
    class PracticeListViewAdapter : BaseAdapter<PracticeModel>
    {
        private List<PracticeModel> mItems;
        private Context mContext;

        public PracticeListViewAdapter(Context context, List<PracticeModel> items)
        {
            mItems = items;
            mContext = context;
        }

        public override int Count
        {
            get { return mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override PracticeModel this[int position]
        {
            get { return mItems[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ScheduleView, null, false);
            }

            TextView txtPracticeViewDescription = row.FindViewById<TextView>(Resource.Id.txtPracticeViewDescription);
            TextView txtID = row.FindViewById<TextView>(Resource.Id.txtPracticeViewID);
            txtPracticeViewDescription.Text = mItems[position].Id.ToString();
            txtID.Text = mItems[position].Id.ToString();

            return row;
        }
    }
}