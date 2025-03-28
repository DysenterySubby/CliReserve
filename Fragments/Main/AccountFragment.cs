using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Android.Widget;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using Android.Nfc.Tech;
using System.Collections.Generic;
using System.Threading.Tasks;
using AndroidX.RecyclerView.Widget;
using Android.Content;
using CliReserve.Activities;
using CliReserve.ViewModels;
using AndroidX.Lifecycle;
using CliReserve.RestAPI_Services;

namespace CliReserve.Fragments
{
    public class AccountFragment : Fragment
    {
        LinearLayout btnBookings, btnLogout;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        TextView tvName, tvEmail;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_account, container, false);

            tvName = view.FindViewById<TextView>(Resource.Id.tvName);
            tvEmail = view.FindViewById<TextView>(Resource.Id.tvEmail);
            btnBookings = view.FindViewById<LinearLayout>(Resource.Id.btnBookings);
            btnBookings.Click += delegate
            {
                Intent intent = new Intent(Context, typeof(PastBookingActivity));
                StartActivity(intent);
            };

            btnLogout = view.FindViewById<LinearLayout>(Resource.Id.btnLogout);
            btnLogout.Click += delegate
            {
                Intent intent = new Intent(Context, typeof(LandingActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(intent);
            };
            return view;
        }
        public async override void OnResume()
        {
            try
            {
                base.OnResume();
                var user = await AccountService.GetUser(AccountService.GetToken(this.Activity));

                tvName.Text = $"{user.FirstName} {user.LastName}";
                tvEmail.Text = user.Email;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, ex.Message, ToastLength.Long).Show();
            }

        }

    }
}