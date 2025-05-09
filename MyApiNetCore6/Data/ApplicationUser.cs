using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace MyApiNetCore6.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string ID_NHOMQUYEN { get; set; } = null!; 
        public string FullName { get; set; } = null!;
        public string PasswordDecrypt { get; set; } = null!;

        public string URL_IMAGE { get; set; } = null!;
    }
}
