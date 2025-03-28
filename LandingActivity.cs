using System;
using System.Net.Http;
using CliReserve.Fragments;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Newtonsoft.Json;
using AndroidX.Fragment.App;
using CliReserve.Activities;
using CliReserve.RestAPI_Services;
using Android.Media;
using System.Runtime.Remoting.Contexts;
using Android.Animation;
using Android.Preferences;
using AndroidX.Lifecycle;
using CliReserve.ViewModels;
using Android.App;

namespace CliReserve
{
    [Android.App.Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class LandingActivity : AppCompatActivity
    {
        private LoginFragment _loginFragment;
        private RegisterFragment _registerFragment;

        LandingViewModel _landingViewModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_landing);

            var accountService = new AccountService(this);

            _landingViewModel = new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof(LandingViewModel))) as LandingViewModel;
            _landingViewModel.AccountService = accountService;
            _landingViewModel.User.Observe(this, new LoginObserver(this, _landingViewModel));

            _registerFragment = new RegisterFragment();
            _loginFragment = new LoginFragment();

            //Intializes fragments (LOGIN AND REGISTER)
            SupportFragmentManager.BeginTransaction()
            .Add(Resource.Id.landingFragmentContainer, _registerFragment, "RegisterFragment")
            .Hide(_registerFragment)
            .Add(Resource.Id.landingFragmentContainer, _loginFragment, "LoginFragment")
            .Commit();

            
        }
        protected override void OnStart()
        {
            base.OnStart();
            _loginFragment.btnNavRegister.Click += delegate
            {
                SupportFragmentManager.BeginTransaction()
                .Show(_registerFragment)
                .Hide(_loginFragment)
                .AddToBackStack("LoginFragment")
                .Commit();
            };

           
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public class LoginObserver : Java.Lang.Object, IObserver
        {
            Android.Content.Context _context;
            LandingViewModel _viewModel;
            public LoginObserver(Android.Content.Context context, LandingViewModel viewmodel){ _context = context; _viewModel = viewmodel; }
            public void OnChanged(Java.Lang.Object p0)
            {
                Intent intent = new Intent(_context, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                _context.StartActivity(intent);
            }
        }
    }
}