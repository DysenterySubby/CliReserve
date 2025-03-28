using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Kotlin.Coroutines.Jvm.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliReserve.Models
{
    public class Seat
    {
        public string SeatId { get; set; } = null!;
        public string SeatTypeId { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public bool IsAvailable { get; set; }
    }
}