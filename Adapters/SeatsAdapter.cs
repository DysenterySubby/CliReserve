using AndroidX;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;
using CliReserve.Models;
using System.Linq;
using Google.Android.Material.TextView;
using Android.App;
using CliReserve.Activities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;

namespace CliReserve
{
    public class SeatsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<SeatsAdapterClickEventArgs> ItemClick;
        public int StayDuration;
        List<Seat> seats;
        public SeatsAdapter(List<Seat> data)
        {
            seats = data;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_seat, parent, false);
            return new SeatsAdapterViewHolder(itemView, OnClick); ;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            Seat seat = seats[position];

            var holder = viewHolder as SeatsAdapterViewHolder;
            if (!seat.IsAvailable)
                holder.btnBook.Enabled = false;

            holder.tvSeatType.Text = seat.TypeName;
            holder.tvSeatId.Text = seat.SeatId;
            holder.tvDescription.Text = seat.Description;
            holder.tvCapacity.Text = $"{seat.Capacity} Seater |";
            holder.tvUsed.Text = $"{seat.Capacity - seat.BookedCount} Available";
            holder.btnBook.Enabled = seat.IsAvailable ? true : false;
            holder.btnDecrease.Click += (sender, e) =>
            {
                var btn = sender as Button;
                holder.StayDuration -= 30;
                btn.Enabled = holder.StayDuration > 30 ? true : false;
                holder.btnIncrease.Enabled = holder.StayDuration/60 < 3 ? true : false;

                holder.tvDuration.Text = holder.StayDuration % 60 != 0 ? (holder.StayDuration == 30 ? $"{holder.StayDuration} Minutes" : $"{((float)holder.StayDuration / 60).ToString("0.0")} Hours") : (holder.StayDuration / 60 == 1 ? $"{holder.StayDuration / 60} Hour" : $"{holder.StayDuration / 60} Hours");
            };
            holder.btnIncrease.Click += (sender, e) =>
            {
                var btn = sender as Button;
                holder.StayDuration += 30;
                btn.Enabled = holder.StayDuration/60 >= 3 ? false : true;
                holder.btnDecrease.Enabled = holder.StayDuration >= 30 ? true : false;

                
                holder.tvDuration.Text = holder.StayDuration % 60 != 0 ? (holder.StayDuration == 30 ? $"{holder.StayDuration} Minutes" : $"{ ((float)holder.StayDuration / 60).ToString("0.0")} Hours") : (holder.StayDuration / 60 == 1 ? $"{holder.StayDuration/60} Hour" : $"{holder.StayDuration / 60} Hours");
            };
        }

        public override int ItemCount => seats.Count;

        void OnClick(SeatsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
    }

    public class SeatsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView tvSeatType, tvSeatId,tvDescription, tvUsed, tvCapacity, tvDuration;
        public Button btnBook, btnIncrease, btnDecrease;
        public int StayDuration = 30;

        public SeatsAdapterViewHolder(View itemView, Action<SeatsAdapterClickEventArgs> clickListener) : base(itemView)
        {
            tvSeatType = itemView.FindViewById<TextView>(Resource.Id.tvSeatType);
            tvSeatId = itemView.FindViewById<TextView>(Resource.Id.tvSeatId);
            tvDescription = itemView.FindViewById<TextView>(Resource.Id.tvDescription);
            tvUsed = itemView.FindViewById<TextView>(Resource.Id.tvUsed);
            tvCapacity = itemView.FindViewById<TextView>(Resource.Id.tvCapacity);

            btnBook = itemView.FindViewById<Button>(Resource.Id.btnBook);

            tvDuration = itemView.FindViewById<TextView>(Resource.Id.tvDuration);
            btnIncrease = itemView.FindViewById<Button>(Resource.Id.btnIncrease);
            btnDecrease = itemView.FindViewById<Button>(Resource.Id.btnDecrease);

            btnBook.Click += (sender, e) => clickListener(new SeatsAdapterClickEventArgs { View = itemView, Position = AdapterPosition, ViewHolder = this});
        }
    }

    public class SeatsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public SeatsAdapterViewHolder ViewHolder { get; set; }

    }
}