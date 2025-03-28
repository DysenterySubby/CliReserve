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
using Kotlin.Coroutines.Jvm.Internal;
using Kotlin.Jvm.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static Android.Views.View;

namespace CliReserve.Activities
{
    [Activity(Label = "ReservationActivity", Theme = "@style/AppTheme")]
    public class ReservationActivity : AppCompatActivity, View.IOnTouchListener
    {
        //General U.I views
        PopUpFragment popUpFragment;
        RecyclerView recyclerView;
        SeatTypesAdapter seatTypesAdapter;

        //Filter views
        AndroidX.AppCompat.Widget.SearchView searchView;
        ChipGroup cgSeatTypes;

        //Background and animator for the pop up
        FrameLayout bg_Popup;
        TransitionDrawable transitionDrawable;

        //Fragment Container for the popup
        FragmentContainerView fcPopup;

        //Animation variables - IGNORE
        float lastPosY;
        float containerHiddenTransY;

        List<SeatType> seatTypes;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_reservation);
            //Get seat types data from the intent
            seatTypes = JsonConvert.DeserializeObject<List<SeatType>>(Intent.GetStringExtra("SeatTypes"));

            //Set views
            recyclerView = FindViewById<RecyclerView>(Resource.Id.rvSeats);
            fcPopup = FindViewById<FragmentContainerView>(Resource.Id.fcPop);
            bg_Popup = FindViewById<FrameLayout>(Resource.Id.bg_fcPop);
            FindViewById<TextView>(Resource.Id.tvClir).Text = Intent.GetStringExtra("ClirName");
            transitionDrawable = new TransitionDrawable(new ColorDrawable[] { new ColorDrawable(Color.Transparent), new ColorDrawable(Resources.GetColor(Resource.Color.blur)) });
            bg_Popup.Background = transitionDrawable;


            //Set views for filtering
            searchView = FindViewById<AndroidX.AppCompat.Widget.SearchView>(Resource.Id.svSeatTypes);
            cgSeatTypes = FindViewById<ChipGroup>(Resource.Id.cgSeatTypes);

            //Initializes the pop fragment
            popUpFragment = new PopUpFragment();
            SupportFragmentManager.BeginTransaction()
                .Add(fcPopup.Id, popUpFragment, "PopUpFragment")
                .Commit();

            //Set pop up animation variables
            SetContainerTranslateY();

            //Initializes the adapter for the list of seat types
            seatTypesAdapter = new SeatTypesAdapter(seatTypes);
            recyclerView.SetLayoutManager(new GridLayoutManager(this, 2));
            recyclerView.SetAdapter(seatTypesAdapter);

            seatTypesAdapter.ItemClick += async (sender, e) =>
            {
                try
                {
                    var booking = await BookingService.GetBookingAsync(CliReserve.RestAPI_Services.AccountService.GetToken(this));
                    if (booking == null)
                    {
                        var seatType = seatTypes[e.Position];
                        var seats = await ClirServices.GetSeatsAsync(authToken: RestAPI_Services.AccountService.GetToken(BaseContext), seatType);
                        AnimatePopUp();

                        popUpFragment.UpdateSeatsAdapter(seats);
                    }
                    else
                    {
                        if (!booking.Approved)
                        {
                            popUpFragment.qrFragment.WriteQr(booking.BookingId);
                            popUpFragment.ChildFragmentManager.BeginTransaction()
                            .Show(popUpFragment.qrFragment)
                            .Hide(popUpFragment.seatsFragment)
                            .Commit();
                            AnimatePopUp();
                        }
                        else
                        {
                            Intent intent = new Intent(this, typeof(BookingActivity));
                            intent.PutExtra("Booking", JsonConvert.SerializeObject(booking));
                            StartActivity(intent);
                        }
                    }
                }
                catch
                {
                    Intent intent = new Intent(this, typeof(LandingActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    Toast.MakeText(this, "Connection Error", ToastLength.Short).Show();
                }
                
            };

            //Set Event Listeners
            searchView.SetOnQueryTextListener(new SeatTypeOnQueryTextListener(seatTypesAdapter));
            cgSeatTypes.SetOnCheckedStateChangeListener(new SeatTypeOnCheckedStateChangeListener(seatTypesAdapter));
            fcPopup.SetOnTouchListener(this);


            OnBackPressedDispatcher.AddCallback(this, new ReservationActivityOnBackPressedCallback(true, this));
        }


        protected async override void OnStart()
        {
            base.OnStart();
            

            var seatsJson = Intent.GetStringExtra("Seats");
            var seatId = Intent.GetStringExtra("SeatId");
            if (!string.IsNullOrEmpty(seatsJson))
            {
                var booking = await BookingService.GetBookingAsync(CliReserve.RestAPI_Services.AccountService.GetToken(this));

                if (booking == null)
                {
                    var seats = JsonConvert.DeserializeObject<List<Seat>>(seatsJson);

                    if (!string.IsNullOrEmpty(seatId))
                        popUpFragment.UpdateSeatsAdapter(seats, seats.FindIndex(s => s.SeatId == seatId));
                    else
                        popUpFragment.UpdateSeatsAdapter(seats);
                    AnimatePopUp();
                }
                else
                {
                    popUpFragment.qrFragment.WriteQr(booking.BookingId);
                    popUpFragment.ChildFragmentManager.BeginTransaction()
                    .Show(popUpFragment.qrFragment)
                    .Hide(popUpFragment.seatsFragment)
                    .Commit();
                    AnimatePopUp();
                }
            }
        }

        #region U.I ANIMATION (IGNORE)
        //Primarly for U.I Design, sets the offset of the pop-up container so its hidden
        public void BackPressed()
        {
            if(fcPopup.TranslationY <= fcPopup.Height)
            {
                transitionDrawable.ReverseTransition(500);

                var interpolator = new Android.Views.Animations.LinearInterpolator();
                bg_Popup.Background.Alpha = 0;
                fcPopup.Animate()
                .SetInterpolator(interpolator).TranslationY(containerHiddenTransY)
                .SetDuration(250);
            }
            else
            {
                if(fcPopup.TranslationY == containerHiddenTransY)
                {
                    popUpFragment.ChildFragmentManager.BeginTransaction()
                    .Hide(popUpFragment.seatsFragment)
                    .Hide(popUpFragment.qrFragment)
                    .Commit();
                    Finish();
                }
                
            }
                
        }

        void SetContainerTranslateY()
        {
            containerHiddenTransY = Resources.DisplayMetrics.HeightPixels + fcPopup.Height;
            fcPopup.TranslationY = containerHiddenTransY;
        }
        
        void AnimatePopUp()
        {
            transitionDrawable.StartTransition(500);
            var interpolator = new OvershootInterpolator((float)0.1);
            if (fcPopup.TranslationY <= fcPopup.Height)
                fcPopup.TranslationY = containerHiddenTransY;

            fcPopup.Animate()
            .SetInterpolator(interpolator).TranslationYBy(-containerHiddenTransY)
            .SetDuration(500);
        }

        //Drag Animation
        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    lastPosY = e.GetY();
                    return true;
                case MotionEventActions.Move:
                    var curPosY = e.GetY();
                    var deltaY = lastPosY - curPosY;

                    var transY = v.TranslationY;
                    transY = transY - deltaY;
                    if (transY < 0)
                        transY = 0;
                    v.TranslationY= transY;
                    bg_Popup.Background.Alpha = (int)(((transY - containerHiddenTransY) / (0 - containerHiddenTransY)) * 220);
                    return true;
                case MotionEventActions.Up:
                    var interpolator = new Android.Views.Animations.LinearInterpolator();
                    if (v.TranslationY <= v.Height / 2)
                    {
                        bg_Popup.Background.Alpha = 220;
                        v.Animate()
                        .SetInterpolator(interpolator).TranslationYBy(v.TranslationY * -1)
                        .SetDuration(250);
                    }
                    else
                    {
                        bg_Popup.Background.Alpha = 0;
                        v.Animate()
                        .SetInterpolator(interpolator).TranslationY(containerHiddenTransY)
                        .SetDuration(250);
                        popUpFragment.ChildFragmentManager.BeginTransaction()
                        .Hide(popUpFragment.qrFragment)
                        .Hide(popUpFragment.seatsFragment)
                        .Commit();
                    }
                    return true;
                default:
                    return v.OnTouchEvent(e);
            }
        }
        #endregion
    }
    public class ReservationActivityOnBackPressedCallback : OnBackPressedCallback
    {
        ReservationActivity _activity;
        public ReservationActivityOnBackPressedCallback(bool enabled, Activity activity) : base(enabled)
        {
            _activity = activity as ReservationActivity;
        }

        public override void HandleOnBackPressed()
        {
            _activity.BackPressed();
        }
    }
    public class SeatTypeOnQueryTextListener : Java.Lang.Object, AndroidX.AppCompat.Widget.SearchView.IOnQueryTextListener
    {
        private SeatTypesAdapter _adapter;
        public SeatTypeOnQueryTextListener(SeatTypesAdapter adapter)
        {
            _adapter = adapter;
        }
        public bool OnQueryTextChange(string newText)
        {
            _adapter.FilterDataQuery(newText);
            return true;
        }

        public bool OnQueryTextSubmit(string newText)
        {
            return false;
        }
    }
    public class SeatTypeOnCheckedStateChangeListener : Java.Lang.Object, ChipGroup.IOnCheckedStateChangeListener
    {
        private SeatTypesAdapter _adapter;
        public SeatTypeOnCheckedStateChangeListener(SeatTypesAdapter adapter)
        {
            _adapter = adapter;
        }
        public void OnCheckedChanged(ChipGroup p0, IList<Integer> p1)
        {
            Activity context = p0.Context as Activity;

            var filters = p1.Select(x => context.FindViewById<Chip>(x.IntValue()).Text).ToArray();
            _adapter.FilterDataArray(filters);
        }
    }
}