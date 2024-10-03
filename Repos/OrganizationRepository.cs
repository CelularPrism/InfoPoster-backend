using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class OrganizationRepository
    {
        private readonly AccountContext _account;
        private readonly OrganizationContext _organization;

        public OrganizationRepository(AccountContext account, OrganizationContext organization)
        {
            _account = account;
            _organization = organization;
        }

        public async Task<OrganizationModel> GetOrganization(Guid id) =>
            await _organization.Organizations.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<OrganizationModel>> GetOrganizationList(Guid userId) =>
            await _organization.Organizations.Where(o => o.UserId == userId).ToListAsync();

        public async Task<OrganizationFullInfoModel> GetOrganizationFullInfo(Guid organizationId) =>
            await _organization.OrganizationsFullInfo.FirstOrDefaultAsync(f => f.OrganizationId == organizationId);

        public async Task<OrganizationMultilangModel> GetOrganizationMultilang(Guid organizationId, string lang) =>
            await _organization.OrganizationsMultilang.FirstOrDefaultAsync(f => f.OrganizationId == organizationId && f.Lang == lang);
        public async Task<List<OrganizationFileURLModel>> GetFileUrls(Guid organizationId) =>
            await _organization.OrganizationFileUrls.Where(f => f.OrganizationId == organizationId).ToListAsync();

        public async Task AddOrganization(OrganizationModel model)
        {
            await _organization.Organizations.AddAsync(model);
            await _organization.SaveChangesAsync();
        }

        public async Task AddFullInfo(OrganizationFullInfoModel model)
        {
            await _organization.OrganizationsFullInfo.AddAsync(model);
            await _organization.SaveChangesAsync();
        }

        public async Task AddMultilang(OrganizationMultilangModel model)
        {
            await _organization.OrganizationsMultilang.AddAsync(model);
            await _organization.SaveChangesAsync();
        }

        public async Task SaveFiles(List<OrganizationFileURLModel> list, Guid organizationId)
        {
            var old = await _organization.OrganizationFileUrls.Where(f => f.OrganizationId == organizationId).ToListAsync();
            if (old.Count > 0)
                _organization.OrganizationFileUrls.RemoveRange(old);
            await _organization.OrganizationFileUrls.AddRangeAsync(list);
            await _organization.SaveChangesAsync();
        }

        public async Task UpdateOrganization(OrganizationModel model)
        {
            _organization.Organizations.Update(model);
            await _organization.SaveChangesAsync();
        }

        public async Task UpdateFullInfo(OrganizationFullInfoModel model)
        {
            _organization.OrganizationsFullInfo.Update(model);
            await _organization.SaveChangesAsync();
        }

        public async Task UpdateMultilang(OrganizationMultilangModel model)
        {
            _organization.OrganizationsMultilang.Update(model);
            await _organization.SaveChangesAsync();
        }
    }
}
