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

namespace CliReserve.RestAPI_Services.Dtos
{
    public class QrScanDto
    {
        public string SeatId { get; set; } = null!;
        public IEnumerable<Seat> Seats { get; set; }
        public IEnumerable<SeatType> SeatTypes { get; set; }
    }
}