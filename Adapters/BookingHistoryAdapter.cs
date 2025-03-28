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
    internal class BookingHistoryAdapter : RecyclerView.Adapter
    {
        List<BookingHistoryData> items;

        public BookingHistoryAdapter(List<BookingHistoryData> data)
        {
            items = data;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_booking_history, parent, false);
            var vh = new BookingHistoryViewHolder(itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            BookingHistoryData item = items[position];

            var holder = viewHolder as BookingHistoryViewHolder;
            var startTime = DateTime.Today.Add((TimeSpan)item.StartTime);
            var endTime = DateTime.Today.Add((TimeSpan)item.EndTime);
            holder.tvTypeName.Text = item.TypeName;
            holder.tvSeatId.Text = item.SeatId;
            holder.tvBookingId.Text = item.BookingId;
            holder.tvStartTime.Text = $"{startTime.ToString("hh:mm")}{startTime.ToString("tt")}";
            holder.tvEndTime.Text = $"{endTime.ToString("hh:mm")}{startTime.ToString("tt")}";
        }
        public override int ItemCount => items.Count;

    }

    public class BookingHistoryViewHolder : RecyclerView.ViewHolder
    {
        public TextView tvTypeName, tvSeatId, tvBookingId, tvStartTime, tvEndTime;

        public BookingHistoryViewHolder(View itemView) : base(itemView)
        {
            tvTypeName = itemView.FindViewById<TextView>(Resource.Id.tvTypeName);
            tvSeatId = itemView.FindViewById<TextView>(Resource.Id.tvSeatId);
            tvBookingId = itemView.FindViewById<TextView>(Resource.Id.tvBookingId);
            tvStartTime = itemView.FindViewById<TextView>(Resource.Id.tvStartTime);
            tvEndTime = itemView.FindViewById<TextView>(Resource.Id.tvEndTime);
        }
    }
}