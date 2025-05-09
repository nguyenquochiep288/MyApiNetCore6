using System.ComponentModel.DataAnnotations;

namespace MyApiNetCore6.Models
{
    public class SignUpModel
    {
        public string? ID { get; set; }
        public string? ID_NHOMQUYEN { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? URL_IMAGE { get; set; }
    }
}
