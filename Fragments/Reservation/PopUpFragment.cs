using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using AndroidX.ViewPager2.Widget;
using CliReserve.Activities;
using CliReserve.Models;
using CliReserve.RestAPI_Services;
using Java.Interop;
using Java.Lang;
using Me.Relex.CircleIndicatorLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ZXing.Multi.QrCode;

namespace CliReserve.Fragments.Reservation
{
    public class PopUpFragment : Fragment
    {
        FragmentContainerView fcPopup;

        public SeatsFragment seatsFragment;
        public QrFragment qrFragment;

        List<Seat> _seats;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_popup, container, false);

            seatsFragment = new SeatsFragment();
            qrFragment = new QrFragment();

            fcPopup = view.FindViewById<FragmentContainerView>(Resource.Id.fcPopupChild);

            ChildFragmentManager.BeginTransaction()
                .Add(fcPopup.Id, qrFragment, "QRFragment")
                .Hide(qrFragment)
                .Add(fcPopup.Id, seatsFragment, "SeatsFragment")
                .Commit();
            return view;
        }


        public void UpdateSeatsAdapter(List<Seat> seats, int selectedIndex = 0)
        {
            var viewPager = seatsFragment.vpSeats;
            var indicator = seatsFragment.indicator;
            var newAdapter = new SeatsAdapter(seats);
            _seats = seats;

            
            viewPager.Adapter = newAdapter;
    
            seatsFragment.seatsAdapter = newAdapter;
            indicator.CreateIndicators(newAdapter.ItemCount, 0);

            ChildFragmentManager.BeginTransaction()
            .Show(seatsFragment)
            .Hide(qrFragment)
            .Commit();

            viewPager.Invalidate();
            seatsFragment.seatsAdapter.ItemClick += SeatAdapter_ItemClick;
            viewPager.SetCurrentItem(selectedIndex, true);
        }

        //BOOK BUTTON CLICKED
        private async void SeatAdapter_ItemClick(object sender, SeatsAdapterClickEventArgs e)
        {
            Seat seat = _seats[e.Position];
            var stayDuration = e.ViewHolder.StayDuration;
            try
            {
                var bookingId = await BookingService.BookSeatAsync(AccountService.GetToken(Context), seat, stayDuration);
                if (bookingId != null)
                {
                    qrFragment.WriteQr(bookingId);
                    ChildFragmentManager.BeginTransaction()
                    .Show(qrFragment)
                    .Hide(seatsFragment)
                    .Commit();
                }
            }
            catch
            {
                Intent intent = new Intent(Context, typeof(LandingActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(intent);
                Toast.MakeText(Context, "Connection Error", ToastLength.Short).Show();
            }
            
        }
    }

    public class SeatsFragment : Fragment
    {
        public ViewPager2 vpSeats;
        public CircleIndicator3 indicator;   
        public SeatsAdapter seatsAdapter;
        public  override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_popup_seats, container, false);

            indicator = view.FindViewById<CircleIndicator3>(Resource.Id.circleIndicator1);
            
            vpSeats = view.FindViewById<ViewPager2>(Resource.Id.vpSeats);

            vpSeats.SetClipToPadding(false);
            vpSeats.SetClipChildren(false);
            vpSeats.OffscreenPageLimit = 3;
            vpSeats.GetChildAt(0).OverScrollMode = OverScrollMode.Never;
            vpSeats.RegisterOnPageChangeCallback(new SeatsViewPagerOnPageChangeCallback(indicator));
            CompositePageTransformer compositePageTransformer = new CompositePageTransformer();
            compositePageTransformer.AddTransformer(new MarginPageTransformer(10));
            compositePageTransformer.AddTransformer(new PageTransformer());

            vpSeats.SetPageTransformer(compositePageTransformer);
            return view;
        }
        internal class PageTransformer : Java.Lang.Object, ViewPager2.IPageTransformer
        {
            public void TransformPage(View page, float position)
            {
                float r = 1 - System.Math.Abs(position);
                page.ScaleY = (0.85f + r * 0.15f);
            }
        }
        internal class SeatsViewPagerOnPageChangeCallback : ViewPager2.OnPageChangeCallback
        {
            CircleIndicator3 _indicator;
            public SeatsViewPagerOnPageChangeCallback(CircleIndicator3 indicator)
            {
                _indicator = indicator; 
            }
            public override void OnPageSelected(int position)
            {
                base.OnPageSelected(position);
                _indicator.AnimatePageSelected(position);

            }
        }
    }
    
    public class QrFragment : Fragment
    {
        ImageView imgQr;
        TextView tvBookingId;
        string _bookingId;

        Runnable runnable;
        Handler handler;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_popup_qr, container, false);
            imgQr = view.FindViewById<ImageView>(Resource.Id.imgQr);
            tvBookingId = view.FindViewById<TextView>(Resource.Id.tvBookingId);
            view.FindViewById<TextView>(Resource.Id.btnCancelBooking).Click += async delegate
            {
                var response = await BookingService.UnbookAsync(RestAPI_Services.AccountService.GetToken(Context));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Toast.MakeText(Context, "Booking Successfully deleted", ToastLength.Long).Show();
                }
                else
                    Toast.MakeText(Context, response.Content.ToString(), ToastLength.Long).Show();
            };
            handler = new Handler();
            return view;
        }
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (hidden)
            {
                handler.RemoveCallbacks(runnable);
            }
            else
            {
                handler.PostDelayed(runnable = new Runnable(async () =>
                {
                    handler.PostDelayed(runnable, 1000);
                    var response = await BookingService.CheckBookingAsync(AccountService.GetToken(Context), _bookingId);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        handler.RemoveCallbacks(runnable);
                        Toast.MakeText(Context, "APPROVED", ToastLength.Short).Show();

                        var booking = await BookingService.GetBookingAsync(AccountService.GetToken(Context));
                        Intent intent = new Intent(Context, typeof(BookingActivity));
                        intent.PutExtra("Booking", JsonConvert.SerializeObject(booking));
                        StartActivity(intent);
                        Activity.Finish();
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.Conflict)
                        Toast.MakeText(Context, response.Content.ToString(), ToastLength.Short).Show();
                }), 1000);
            }
        }

        public void WriteQr(string bookingid)
        {
            _bookingId = bookingid;
            var barcodeWriter = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 250,
                    Height = 250
                }
            };
            imgQr.SetImageBitmap(barcodeWriter.Write(bookingid));
            tvBookingId.Text = $"Booking ID: {bookingid}";
        }
    }

    
}