using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Tools;
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

        public async Task<List<UserModel>> SearchUsers(string text) => 
            await _context.Users.Where(u => u.FirstName.Equals(text) || u.LastName.Equals(text) || u.Email.Equals(text)).ToListAsync();

        public async Task<List<UserModel>> GetUsersForAdmin() =>
            await _context.User_To_Roles.Where(r => r.RoleId != Constants.ROLE_ADMIN)
                                        .Join(_context.Users,
                                              r => r.UserId,
                                              user => user.Id,
                                              (r, user) => user)
                                        .ToListAsync();

        public async Task<List<UserModel>> GetEditors() =>
            await _context.User_To_Roles.Where(r => r.RoleId == Constants.ROLE_EDITOR)
                                        .Join(_context.Users,
                                              r => r.UserId,
                                              user => user.Id,
                                              (r, user) => user)
                                        .ToListAsync();

        public async Task<UserModel> GetUser(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<UserModel> GetUser(Guid userId) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<List<Guid>> GetUserRoles(Guid userId) =>
            await _context.User_To_Roles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync();

        public async Task<Guid> GetUserRole(Guid userId) =>
            await _context.User_To_Roles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).FirstOrDefaultAsync();

        public async Task UpdateUser(UserModel user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
