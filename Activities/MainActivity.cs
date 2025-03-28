using CliReserve.Fragments;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using Google.Android.Material.BottomNavigation;
using Android.Views;
using AndroidX.Lifecycle;
using AndroidX.AppCompat.Widget;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.Navigation;
using Google.Android.Material.FloatingActionButton;
using AndroidX.Fragment.App;
using CliReserve.ViewModels;
using Newtonsoft.Json;
using CliReserve.RestAPI_Services;
using Android.Widget;
namespace CliReserve.Activities
{
    [Android.App.Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener, NavigationView.IOnNavigationItemSelectedListener
    {
        private string _currentStringFragment;
        private Fragment _currentFragment;

        private HomeFragment _homeFragment;
        private QRFragment _qrFragment;
        private AccountFragment _accountFragment;
        private FloatingActionButton fab;

        MainViewModel _mainViewModel;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_main);
            _mainViewModel = new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof(MainViewModel))) as MainViewModel;
            try
            {
                _mainViewModel.Clirs = await ClirServices.GetClirsAsync(authToken: RestAPI_Services.AccountService.GetToken(this));
                _mainViewModel.RizalSeatTypes = await ClirServices.GetSeatTypesAsync(authToken: RestAPI_Services.AccountService.GetToken(this), new Clir { ClirName = "Rizal" });
                _mainViewModel.EinsteinSeatTypes = await ClirServices.GetSeatTypesAsync(authToken: RestAPI_Services.AccountService.GetToken(this), new Clir { ClirName = "Einstein" });
            }
            catch
            {
                Toast.MakeText(this, "Connection Error", ToastLength.Short).Show();
            }
            
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navbar_main);
            fab = FindViewById<FloatingActionButton>(Resource.Id.btnQr);
            navigation.SetOnNavigationItemSelectedListener(this);
            fab.Click += delegate
            {
                SupportFragmentManager.BeginTransaction()
                    .Hide(_currentFragment)
                    .Show(_qrFragment)
                    .AddToBackStack(_currentStringFragment)
                .Commit();
                _currentFragment = _qrFragment;
                _currentStringFragment = "QRFragment";
            };
            _homeFragment = new HomeFragment();
            _qrFragment = new QRFragment();
            _accountFragment = new AccountFragment();

            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.mainFragmentContainer, _accountFragment, "AccountFragment")
                    .Hide(_accountFragment)
                .Add(Resource.Id.mainFragmentContainer, _qrFragment, "QRFragment")
                    .Hide(_qrFragment)
                .Add(Resource.Id.mainFragmentContainer, _homeFragment, "HomeFragment")
                    .Commit();
            _currentStringFragment = "HomeFragment";
            _currentFragment = _homeFragment;
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.nav_home:
                    SupportFragmentManager.BeginTransaction()
                            .Hide(_currentFragment)
                            .Show(_homeFragment)
                            .AddToBackStack(_currentStringFragment)
                        .Commit();
                    _currentFragment = _homeFragment;
                    _currentStringFragment = "HomeFragment";
                    return true;
                case Resource.Id.nav_account:
                    SupportFragmentManager.BeginTransaction()
                        .Hide(_currentFragment)
                        .Show(_accountFragment)
                        .AddToBackStack(_currentStringFragment)
                        .Commit();
                    _currentFragment = _accountFragment;
                    _currentStringFragment = "AccountFragment";
                    return true;
            }
            return false;
        }
    }
}