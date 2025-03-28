using Android.Accounts;
using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using CliReserve.RestAPI_Services;
using CliReserve.RestAPI_Services.Dtos;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CliReserve.LandingActivity;

namespace CliReserve.ViewModels
{
    public class LandingViewModel : ViewModel
    {
        private MutableLiveData _user;
        public LiveData User => _user;


        public AccountService AccountService { get; set; }

        public LandingViewModel()
        {
            _user = new MutableLiveData();
        }

        public async Task LoginAsync(LoginDto loginDto)
        {
            var user = await AccountService.LoginAsync(loginDto);

            if(user != null)
            {
                _user.SetValue(user);
            }
        }

        public async Task RegisterAsync(RegisterDto registerDto)
        {
            var user = await AccountService.RegisterAsync(registerDto);

            if (user != null)
            {
                _user.SetValue(user);
            }
        }
    }
}