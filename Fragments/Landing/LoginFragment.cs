using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Android.Widget;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using Android.Nfc.Tech;
using CliReserve.Activities;
using Android.Content;
using CliReserve.RestAPI_Services;
using CliReserve.RestAPI_Services.Dtos;
using static System.Net.Mime.MediaTypeNames;
using AndroidX.Lifecycle;
using CliReserve.ViewModels;
using static CliReserve.LandingActivity;
using System.Runtime;
namespace CliReserve.Fragments
{
    public class LoginFragment : Fragment
    {
        public TextView btnNavRegister;

        private Button btnLogin;
        private EditText txtEmail, txtPassword;

        LandingViewModel _landingViewModel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            _landingViewModel = new ViewModelProvider(Activity).Get(Java.Lang.Class.FromType(typeof(LandingViewModel))) as LandingViewModel;

        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_login, container, false);
            txtEmail = view.FindViewById<EditText>(Resource.Id.txtEmail);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtPassword);

            btnLogin = view.FindViewById<Button>(Resource.Id.btnLogin);
            btnNavRegister = view.FindViewById<TextView>(Resource.Id.btnNavRegister);

            btnLogin.Click += async delegate
            {
                try
                {
                    await _landingViewModel.LoginAsync(new LoginDto
                    {
                        Email = txtEmail.Text,
                        Password = txtPassword.Text
                    });
                }
                catch
                {
                    Toast.MakeText(Context, "Connection Error", ToastLength.Short).Show();
                }
                
            };

            
            return view;
        }
    }
}