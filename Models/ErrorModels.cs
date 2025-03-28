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
    public class RegisterErrorResponse
    {
        public string code {  get; set; }
        public string description { get; set; }
    }
    public class LoginErrorResponse
    {
        public Dictionary<string, List<string>> Errors { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
    }
}