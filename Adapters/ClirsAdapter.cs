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
using Android.Graphics;
using Android.OS;
using Android.Views.Animations;
namespace CliReserve
{
    internal class ClirsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ClirsAdapterClickEventArgs> ItemClick;
        List<Clir> items;

        public ClirsAdapter(List<Clir> data)
        {
            items = data;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_clir, parent, false);
            var vh = new ClirsAdapterViewHolder(itemView, OnClick);
            return vh;
        }

        public async override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            Clir item = items[position];

            var holder = viewHolder as ClirsAdapterViewHolder;

            holder.tvName.Text = item.ClirName;
            holder.tvDescription.Text = item.ClirLocation;
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
                holder.imgClir.SetImageBitmap(item.Image);
            }
            catch
            {
                Toast.MakeText(holder.ItemView.Context, "Failed fetching image from server", ToastLength.Short).Show();
            }
            
        }
        private void SetAnimation(View view, int position)
        {
            Animation animation = AnimationUtils.LoadAnimation(view.Context, Android.Resource.Animation.FadeIn);
            view.StartAnimation(animation);
        }
        public override int ItemCount => items.Count;
        void OnClick(ClirsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);

    }

    public class ClirsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public View mainView;
        public TextView tvName, tvDescription;
        public ImageView imgClir;
        public Button btnVisit;

        public ClirsAdapterViewHolder(View itemView, Action<ClirsAdapterClickEventArgs> clickListener) : base(itemView)
        {
            ItemView.Visibility = ViewStates.Invisible;
            tvName = itemView.FindViewById<TextView>(Resource.Id.tvName);
            imgClir = itemView.FindViewById<ImageView>(Resource.Id.imgClir);
            tvDescription = itemView.FindViewById<TextView>(Resource.Id.tvDescription);
            btnVisit = itemView.FindViewById<Button>(Resource.Id.btnVisit);

            btnVisit.Click += (sender, e) => clickListener(new ClirsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }
    public class ClirsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}