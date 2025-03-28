using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CliReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CliReserve
{
    public class Clir
    {
        public string ClirName { get; set; } = null!;
        public string ClirLocation { get; set; } = null!;
        public Bitmap Image;
    }
}