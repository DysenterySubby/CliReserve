﻿using Android.App;
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
    public class TokenDto
    {
        public User User { get; set; }
        public string token { get; set; }
    }
}