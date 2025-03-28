using Android.App;
using Android.Content;
using Android.Graphics;
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
    public class SeatType
    {
        public string SeatTypeId { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public string ClirName { get; set; } = null!;
        public int SeatCount { get; set; }
        public Bitmap Image;
    }
}