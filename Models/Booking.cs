using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliReserve.Models
{
    public class Booking
    {
        public string BookingId { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool Approved { get; set; }
        public string SeatId { get; set; } = null!;
    }
}