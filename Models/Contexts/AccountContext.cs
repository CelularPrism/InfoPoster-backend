using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Posters;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Models.Contexts
{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                var exc = ex;
            }
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<UserToRolesModel> User_To_Roles { get; set; }
    }
}
