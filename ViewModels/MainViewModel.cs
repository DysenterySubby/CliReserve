using Android.Accounts;
using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using CliReserve.Models;
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
    public class MainViewModel : ViewModel
    {
        public List<Clir> Clirs { get; set; }
        public List<SeatType> RizalSeatTypes { get; set; }
        public List<SeatType> EinsteinSeatTypes { get; set; }

        MainViewModel _mainViewModel;

        public MainViewModel()
        {
            
        }

    }
}