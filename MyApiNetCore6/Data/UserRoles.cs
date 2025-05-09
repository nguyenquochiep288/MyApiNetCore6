using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiNetCore6.Data
{
    [Table("UserRoles")]
    public class UserRoles
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string UserAdmin = "User,Admin";
    }
}
