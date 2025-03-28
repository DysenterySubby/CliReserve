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

namespace CliReserve.RestAPI_Services.Dtos
{
    internal class ImageRequestDto
    {
        public string SeatTypeId { get; set; } = null!;
        public string ClirName { get; set; } = null!;
    }
}