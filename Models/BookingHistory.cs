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
    internal class BookingHistory
    {
        public DateTime BookingDate { get; set; }
        public List<BookingHistoryData> Bookings { get; set; }
    }

    public class BookingHistoryData
    {
        public string BookingId { get; set; } = null!;
        public string SeatId { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

    }
}