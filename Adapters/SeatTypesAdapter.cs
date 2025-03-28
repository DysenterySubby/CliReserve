using AndroidX;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;
using CliReserve.Models;
using Google.Android.Material.ImageView;
using Android.Graphics;
using Android.Content;
using CliReserve.RestAPI_Services;
using Android.OS;
using Android.Views.Animations;
using System.Linq;
using static Android.App.DownloadManager;
using Java.IO;

namespace CliReserve
{
    public class SeatTypesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<SeatTypesAdapterClickEventArgs> ItemClick;
        List<SeatType> seatTypes;
        List<SeatType> initialList;

        public string ClirName;

        string _stringQuery;
        string[] _stringFilters;
        public SeatTypesAdapter(List<SeatType> data)
        {
            seatTypes = data;
            initialList = data;
        }
        public SeatTypesAdapter(List<SeatType> data, string clirName)
        {
            seatTypes = data;
            initialList = data;

            ClirName = clirName;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = ClirName == null ? LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_seat_type, parent, false) : LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_seat_type_booking, parent, false);
            var vh = ClirName == null ? new SeatTypesAdapterViewHolder(itemView, OnClick) : new SeatTypesAdapterViewHolder(itemView, OnClick, ClirName);
            
            return vh;
        }
        public async override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            var item = seatTypes[position];
            var holder = viewHolder as SeatTypesAdapterViewHolder;

            holder.tvSeatType.Text = item.TypeName;
            holder.tvSeatCount.Text = item.SeatCount > 1 ? $"{item.SeatCount} seats available" : $"{item.SeatCount} Seat Available";

            if (ClirName != null)
                holder.ItemView.FindViewById<TextView>(Resource.Id.tvSeatTypeId).Text = item.SeatTypeId;

            try
            {
                if (item.Image == null)
                {
                    new Handler().PostDelayed(() =>
                    {
                        holder.ItemView.Visibility = ViewStates.Visible;
                        SetAnimation(holder.ItemView, position);
                    }, 800);
                    var context = holder.ItemView.Context;
                    var imgData = await ClirServices.GetImageAsync(AccountService.GetToken(context), item);
                    item.Image = await BitmapFactory.DecodeByteArrayAsync(imgData, 0, imgData.Length);
                }
                holder.imgSeatType.SetImageBitmap(item.Image);
            }
            catch
            {
                Toast.MakeText(holder.ItemView.Context, "Failed fetching image from server", ToastLength.Short).Show();
            }
            if(ClirName!= null)
            {
                holder.ItemView.FindViewById<TextView>(Resource.Id.tvSeatTypeId).Text = item.SeatTypeId;
            }
        }
        private void SetAnimation(View view, int position)
        {
            Animation animation = AnimationUtils.LoadAnimation(view.Context, Android.Resource.Animation.FadeIn);
            view.StartAnimation(animation);
        }

        public void FilterDataQuery(string query)
        {

            if (!string.IsNullOrEmpty(query))
            {
                var filteredList = initialList.Where(st => st.TypeName.ToLower().Contains(query.ToLower()));
                filteredList = _stringFilters == null ? filteredList : filteredList.Where(st => _stringFilters.Any(f => st.TypeName.ToLower().Contains(f.ToLower())));

                _stringQuery = query;
                seatTypes = filteredList.ToList();
            }
            else
            {
                seatTypes = _stringFilters == null ? initialList : initialList.Where(st => _stringFilters.Any(f => st.TypeName.ToLower().Contains(f.ToLower()))).ToList();
                _stringQuery = null;
            }
                

            NotifyDataSetChanged();
        }
        public void FilterDataArray(string[] filters)
        {
            if(filters.Length >= 1)
            {
                
                var filteredList = initialList.Where(st => filters.Any(f => st.TypeName.ToLower().Contains(f.ToLower())));
                filteredList = string.IsNullOrEmpty(_stringQuery) ? filteredList : filteredList.Where(st => st.TypeName.ToLower().Contains(_stringQuery.ToLower()));

                _stringFilters = filters;
                seatTypes = filteredList.ToList();
            }
            else
            {
                seatTypes = string.IsNullOrEmpty(_stringQuery) ? initialList : initialList.Where(st => st.TypeName.ToLower().Contains(_stringQuery.ToLower())).ToList();
                _stringFilters = null;
            }
            
            NotifyDataSetChanged();
        }

        public override int ItemCount => seatTypes.Count;
        void OnClick(SeatTypesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);

    }

    public class SeatTypesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ImageView imgSeatType;
        public TextView tvSeatType, tvSeatCount, tvSeatTypeId;
        public List<Seat> SeatList;

        public Button btnCheck;
        public SeatTypesAdapterViewHolder(View itemView, Action<SeatTypesAdapterClickEventArgs> clickListener) : base(itemView)
        {                                                                               
            
            itemView.Visibility = ViewStates.Invisible;
            tvSeatType = itemView.FindViewById<TextView>(Resource.Id.tvSeatType);
            tvSeatCount = itemView.FindViewById<TextView>(Resource.Id.tvSeatCount);
            imgSeatType = itemView.FindViewById<ImageView>(Resource.Id.imgSeatType);

            itemView.Click += (sender, e) => clickListener(new SeatTypesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        public SeatTypesAdapterViewHolder(View itemView, Action<SeatTypesAdapterClickEventArgs> clickListener, string clirName) : base(itemView)
        {
            itemView.Visibility = ViewStates.Invisible;
            itemView.FindViewById<TextView>(Resource.Id.tvClir).Text = clirName;

            tvSeatType = itemView.FindViewById<TextView>(Resource.Id.tvSeatType);
            tvSeatCount = itemView.FindViewById<TextView>(Resource.Id.tvSeatCount);
            imgSeatType = itemView.FindViewById<ImageView>(Resource.Id.imgSeatType);
            tvSeatTypeId = itemView.FindViewById<TextView>(Resource.Id.tvSeatTypeId);
            btnCheck = itemView.FindViewById<Button>(Resource.Id.btnCheck);

            btnCheck.Click += (sender, e) => clickListener(new SeatTypesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class SeatTypesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}