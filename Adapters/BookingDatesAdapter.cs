using AndroidX;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;
using Android.App;
using CliReserve.Activities;
using Android.Content;
using Newtonsoft.Json;
using CliReserve.RestAPI_Services.Dtos;
using CliReserve.RestAPI_Services;
using CliReserve.Models;
namespace CliReserve
{
    internal class BookingDatesAdapter : RecyclerView.Adapter
    {
        List<BookingHistory> items;

        public BookingDatesAdapter(List<BookingHistory> data)
        {
            items = data;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_booking_history_date, parent, false);
            var vh = new BookingDatesViewHolder(itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            BookingHistory item = items[position];

            var holder = viewHolder as BookingDatesViewHolder;
            var adapter = new BookingHistoryAdapter(item.Bookings);

            holder.tvDate.Text = item.BookingDate.ToString("MMMM dd, yyyy");
            holder.rvBookingHistory.SetAdapter(adapter);
        }
        public override int ItemCount => items.Count;

    }

    public class BookingDatesViewHolder : RecyclerView.ViewHolder
    {
        public TextView tvDate;
        public RecyclerView rvBookingHistory;

        public BookingDatesViewHolder(View itemView) : base(itemView)
        {
            tvDate = itemView.FindViewById<TextView>(Resource.Id.tvDate);
            rvBookingHistory = itemView.FindViewById<RecyclerView>(Resource.Id.rvBookingHistory);
            rvBookingHistory.SetLayoutManager(new LinearLayoutManager(itemView.Context, LinearLayoutManager.Vertical, false));
        }
    }
}