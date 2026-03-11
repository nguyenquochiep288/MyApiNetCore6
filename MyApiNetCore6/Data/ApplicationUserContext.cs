using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DatabaseTHP;
using Microsoft.AspNetCore.Identity;

namespace MyApiNetCore6.Data
{
    public class dbApplicationUserContext : IdentityDbContext<ApplicationUser>
    {
        public dbApplicationUserContext(DbContextOptions<dbApplicationUserContext> opt): base(opt)
        {

        }
    }
}
