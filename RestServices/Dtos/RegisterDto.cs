namespace CliReserve.RestAPI_Services.Dtos
{
    public class RegisterDto
    {
        public int StudentNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}