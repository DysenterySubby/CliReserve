using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.Activity;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using CliReserve.Fragments;
using CliReserve.Fragments.Reservation;
using CliReserve.Models;
using CliReserve.RestAPI_Services;
using Google.Android.Material.Chip;
using Java.Lang;
using Kotlin.Jvm.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static Android.Content.ClipData;
using static Android.Views.View;
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace CliReserve.Activities
{
    [Activity(Label = "PastBookingActivity", Theme = "@style/AppTheme")]
    public class PastBookingActivity : AppCompatActivity
    {
        public RecyclerView rvBookingDates;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_past_bookings);
            //Get seat types data from the intent
            var bookingHistory = await BookingService.GetBookingHistoryAsync(authToken: RestAPI_Services.AccountService.GetToken(this));
            var adapter = new BookingDatesAdapter(bookingHistory);
            rvBookingDates = FindViewById<RecyclerView>(Resource.Id.rvBookingDates);
            rvBookingDates.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
            rvBookingDates.SetAdapter(adapter);

            FindViewById<Button>(Resource.Id.btnBack).Click += delegate
            {
                Finish();
            };
        }
    }
}