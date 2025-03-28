using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Android.Widget;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Text;
using CliReserve.Activities;
using Android.Content;
using CliReserve.RestAPI_Services;
using CliReserve.RestAPI_Services.Dtos;
using static System.Net.Mime.MediaTypeNames;
using CliReserve.ViewModels;
using AndroidX.Lifecycle;
namespace CliReserve.Fragments
{
    public class RegisterFragment : Fragment
    {
        private Button btnRegister;
        private EditText txtFname, txtLname, txtStudNum, txtEmail, txtPassword;

        LandingViewModel _landingViewModel;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _landingViewModel = new ViewModelProvider(Activity).Get(Java.Lang.Class.FromType(typeof(LandingViewModel))) as LandingViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_register, container, false);

            txtFname = view.FindViewById<EditText>(Resource.Id.txtFname);
            txtLname = view.FindViewById<EditText>(Resource.Id.txtLname);
            txtStudNum = view.FindViewById<EditText>(Resource.Id.txtStudNum);

            txtEmail = view.FindViewById<EditText>(Resource.Id.txtRegEmail);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtRegPass);

            btnRegister = view.FindViewById<Button>(Resource.Id.btnRegister);

            btnRegister.Click += async delegate
            {
                try
                {
                    await _landingViewModel.RegisterAsync(new RegisterDto
                    {
                        StudentNumber = int.Parse(txtStudNum.Text),
                        FirstName = txtFname.Text,
                        LastName = txtLname.Text,
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