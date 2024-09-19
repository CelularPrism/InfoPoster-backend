using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class AccountRepository
    {
        private readonly AccountContext _context;

        public AccountRepository(AccountContext context)
        {
            _context = context;
        }

        public async Task<UserModel> GetUser(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<List<Guid>> GetUserRoles(Guid userId) =>
            await _context.User_To_Roles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync();
    }
}
