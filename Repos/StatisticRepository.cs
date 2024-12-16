using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Services.Login;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class StatisticRepository
    {
        private readonly OrganizationContext _organization;
        private readonly PostersContext _posters;
        private readonly Guid _user;

        public StatisticRepository(OrganizationContext organization, PostersContext posters, LoginService loginService)
        {
            _organization = organization;
            _posters = posters;
            _user = loginService.GetUserId();
        }

        public async Task<bool> AnyUser(Guid? userId) =>
            await _organization.Users.AnyAsync(us => us.Id == userId);

        public async Task<List<UserModel>> GetUserList() =>
            await _organization.Users.AsNoTracking().ToListAsync();

        public async Task<List<OrganizationModel>> GetOrganizationList(DateTime dateStart, DateTime dateEnd) =>
            await _organization.Organizations.Where(org => org.CreatedAt >= dateStart.Date && org.CreatedAt < dateEnd.Date).AsNoTracking().ToListAsync();

        public async Task<List<PosterModel>> GetPosterList(DateTime dateStart, DateTime dateEnd) =>
            await _posters.Posters.Where(org => org.CreatedAt >= dateStart.Date && org.CreatedAt < dateEnd.Date).AsNoTracking().ToListAsync();

        public async Task<List<ApplicationChangeHistory>> GetHistoryList() =>
            await _posters.ApplicationChangeHistory.Where(c => c.ChangedBy == _user && c.FieldName == "Status").AsNoTracking().ToListAsync();

        public async Task<List<ApplicationChangeHistory>> GetHistoryList(DateTime startDate, DateTime endDate) =>
            await _posters.ApplicationChangeHistory.Where(c => c.ChangedBy == _user && c.FieldName == "Status" && c.ChangedAt >= startDate.Date && c.ChangedAt <= endDate.Date).AsNoTracking().ToListAsync();
    }
}
