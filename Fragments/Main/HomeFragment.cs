using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Newtonsoft.Json;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;
using CliReserve.Activities;
using Android.Content;
using CliReserve.RestAPI_Services;
using Android.Widget;
using System;
using AndroidX.ViewPager2.Widget;
using static CliReserve.Fragments.Reservation.SeatsFragment;
using AndroidX.AppCompat.Widget;
using CliReserve.Models;
using CliReserve.Fragments.Reservation;
using System.Threading.Tasks;
using AndroidX.Lifecycle;
using Kotlin.Coroutines.Jvm.Internal;
using CliReserve.ViewModels;

namespace CliReserve.Fragments
{
    public class HomeFragment : Fragment
    {

        RecyclerView rvClir;
        ClirsAdapter clirAdapter;

        RecyclerView rvRizal;
        SeatTypesAdapter rizalAdapter;

        RecyclerView rvEinstein;
        SeatTypesAdapter einsteinAdapter;

        Button btnHeader;
        ImageView imgHeader;

        Booking booking;
        MainViewModel _mainViewModel;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _mainViewModel = new ViewModelProvider(Activity).Get(Java.Lang.Class.FromType(typeof(MainViewModel))) as MainViewModel;
        }
        

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_home, container, false);

            rvClir = view.FindViewById<RecyclerView>(Resource.Id.rvClir);
            rvClir.SetLayoutManager(new LinearLayoutManager(this.Context, LinearLayoutManager.Horizontal, false));

            rvRizal = view.FindViewById<RecyclerView>(Resource.Id.rvRizal);
            rvRizal.SetLayoutManager(new LinearLayoutManager(this.Context, LinearLayoutManager.Horizontal, false));

            rvEinstein = view.FindViewById<RecyclerView>(Resource.Id.rvEinstein);
            rvEinstein.SetLayoutManager(new LinearLayoutManager(this.Context, LinearLayoutManager.Horizontal, false));

            btnHeader = view.FindViewById<Button>(Resource.Id.btnHeader);
            imgHeader = view.FindViewById<ImageView>(Resource.Id.imgHeader);

            btnHeader.Click += delegate
            {
                Intent intent = new Intent(Context, typeof(BookingActivity));
                intent.PutExtra("Booking", JsonConvert.SerializeObject(booking));
                StartActivity(intent);
            };

            try
            {

                clirAdapter = new ClirsAdapter(data: _mainViewModel.Clirs);
                rvClir.SetAdapter(clirAdapter);

                clirAdapter.ItemClick += async (sender, e) =>
                {
                    try
                    {
                        var clir = _mainViewModel.Clirs[e.Position];
                        var seatTypes = await ClirServices.GetSeatTypesAsync(authToken: AccountService.GetToken(Context), clir);
                        var json = JsonConvert.SerializeObject(seatTypes);

                        Intent intent = new Intent(Context, typeof(ReservationActivity));
                        intent.PutExtra("SeatTypes", json);
                        intent.PutExtra("ClirName", clir.ClirName);
                        Context.StartActivity(intent);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Context, ex.Message, ToastLength.Long).Show();
                    }
                };

                rizalAdapter = new SeatTypesAdapter(data: _mainViewModel.RizalSeatTypes, "Rizal");
                einsteinAdapter = new SeatTypesAdapter(data: _mainViewModel.EinsteinSeatTypes, "Einstein");

                rvRizal.SetAdapter(rizalAdapter);

                rvEinstein.SetAdapter(einsteinAdapter);

                einsteinAdapter.ItemClick += SeatTypeAdapter_ItemClick;
                rizalAdapter.ItemClick += SeatTypeAdapter_ItemClick;
            }
            catch
            {
                Intent intent = new Intent(Context, typeof(LandingActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(intent);
                Toast.MakeText(Context, "Connection Error", ToastLength.Short).Show();
            }

            return view;
        }

        public async override void OnStart()
        {
            base.OnStart();
            await SetHeader();
        }
        private async void SeatTypeAdapter_ItemClick(object sender, SeatTypesAdapterClickEventArgs e)
        {
            try
            {
                var booking = await BookingService.GetBookingAsync(CliReserve.RestAPI_Services.AccountService.GetToken(Context));

                if (booking == null || !booking.Approved)
                {
                    var adapter = sender as SeatTypesAdapter;
                    var list = adapter.ClirName == "Rizal" ? _mainViewModel.RizalSeatTypes : _mainViewModel.EinsteinSeatTypes;

                    string authToken = AccountService.GetToken(Context);


                    var seatsJson = JsonConvert.SerializeObject(await ClirServices.GetSeatsAsync(authToken: AccountService.GetToken(Context), list[e.Position]));
                    var seatTypesJson = JsonConvert.SerializeObject(await ClirServices.GetSeatTypesAsync(authToken: AccountService.GetToken(Context), new Clir { ClirName = adapter.ClirName }));

                    Intent intent = new Intent(Context, typeof(ReservationActivity));
                    intent.PutExtra("SeatTypes", seatTypesJson);
                    intent.PutExtra("Seats", seatsJson);
                    StartActivity(intent);
                }
                else
                {
                    if (booking.Approved)
                    {
                        Intent intent = new Intent(Context, typeof(BookingActivity));
                        intent.PutExtra("Booking", JsonConvert.SerializeObject(booking));
                        StartActivity(intent);
                    }
                }
            }
            catch
            {
                Toast.MakeText(Context, "Connection Error", ToastLength.Short).Show();
            }
        }

        private async Task SetHeader()
        {
            btnHeader.Visibility = ViewStates.Invisible;
            imgHeader.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.img_header));
            try
            {
                booking = await BookingService.GetBookingAsync(AccountService.GetToken(Context));
                if (booking != null && booking.Approved)
                {
                    imgHeader.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.img_header_booked));
                    btnHeader.Text = "CHECK";
                    btnHeader.Visibility = ViewStates.Visible;
                }
            }
            catch
            {
                Toast.MakeText(Context, "Connection Error", ToastLength.Short).Show();
            }
            
        }
    }
}