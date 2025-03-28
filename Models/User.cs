using AndroidX.Lifecycle;
using CliReserve.Models;
using System.Collections.Generic;

namespace CliReserve
{
    public class User : Java.Lang.Object
    {
        public int StudentNumber { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}