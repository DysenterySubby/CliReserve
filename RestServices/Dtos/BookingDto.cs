using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CliReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliReserve.RestServices.Dtos
{
    public class BookingDto
    {
        public string SeatId;
        public int Duration;
    }
}