using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CliReserve.Models;
using CliReserve.RestAPI_Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliReserve.Activities
{
    [Android.App.Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class BookingActivity : Activity
    {
        Booking userBooking;

        TextView tvStatus, tvStartTime, tvEndTime, tvSeatId, tvDate, tvBookingId;
        TextView btnCancelBooking;
        ImageView imgQr;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_booking);

            userBooking = JsonConvert.DeserializeObject<Booking>(Intent.GetStringExtra("Booking"));
            
            tvStatus = FindViewById<TextView>(Resource.Id.tvStatus);
            tvStartTime = FindViewById<TextView>(Resource.Id.tvStartTime);
            tvEndTime = FindViewById<TextView>(Resource.Id.tvEndTime);
            tvSeatId = FindViewById<TextView>(Resource.Id.tvSeatId);
            tvDate = FindViewById<TextView>(Resource.Id.tvDate);
            imgQr = FindViewById<ImageView>(Resource.Id.imgQr);
            tvBookingId = FindViewById<TextView>(Resource.Id.tvBookingId);

            FindViewById<TextView>(Resource.Id.btnCancelBooking).Click += async delegate
            {
                var response = await BookingService.UnbookAsync(RestAPI_Services.AccountService.GetToken(this));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Toast.MakeText(this, "Booking Successfully deleted", ToastLength.Long).Show();
                    Finish();
                }
                else
                    Toast.MakeText(this, response.Content.ToString(), ToastLength.Long).Show();
            };

            FindViewById<Button>(Resource.Id.btnBack).Click += delegate
            {
                Finish();
            };
            var startTime = DateTime.Today.Add((TimeSpan)userBooking.StartTime);
            var endTime = DateTime.Today.Add((TimeSpan)userBooking.EndTime);
            tvStartTime.Text = $"{startTime.ToString("hh:mm")}{startTime.ToString("tt")}";
            tvEndTime.Text = $"{endTime.ToString("hh:mm")}{endTime.ToString("tt")}";
            tvSeatId.Text = userBooking.SeatId;
            tvDate.Text = userBooking.BookingDate.ToString("MM/dd/yyyy");
            tvBookingId.Text = userBooking.BookingId;

            var barcodeWriter = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 250,
                    Height = 250
                }
            };
            imgQr.SetImageBitmap(barcodeWriter.Write(userBooking.BookingId));
        }

        
    }
}