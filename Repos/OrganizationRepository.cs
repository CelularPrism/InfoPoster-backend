﻿using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Organizations.Menu;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Collections.Generic;

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

        public async Task<List<GetAllOrganizationResponse>> GetOrganizationList(string lang)
        {
            var organizations = await _organization.Organizations.Where(o => o.Status == (int)POSTER_STATUS.ACTIVE || o.Status == (int)POSTER_STATUS.VERIFIED)
                                                                 .Join(_organization.OrganizationsMultilang,
                                                                       o => o.Id,
                                                                       m => m.OrganizationId,
                                                                       (o, m) => new { Organization = o, Multilang = m })
                                                                 .Where(m => m.Multilang.Lang == "en")
                                                                 .Join(_organization.Users,
                                                                       o => o.Organization.UserId,
                                                                       user => user.Id,
                                                                       (o, user) => new { o.Organization, o.Multilang, UserName = user.FirstName + " " + user.LastName })
                                                                 .ToListAsync();
            var result = organizations.Select(m => new GetAllOrganizationResponse(m.Organization, m.Multilang, m.UserName)).OrderByDescending(o => o.CreatedAt).ToList();
            return result;
        }

        public async Task<List<OrganizationModel>> GetOrganizationList(Guid userId) =>
            await _organization.Organizations.Where(o => o.UserId == userId)
                                             .Join(_organization.OrganizationsMultilang,
                                                   o => o.Id,
                                                   m => m.OrganizationId,
                                                   (o, m) => new { Organization = o, Multilang = m })
                                             .Where(m => m.Multilang.Lang == "en")
                                             .Select(o => new OrganizationModel() {
                                                 Id = o.Organization.Id,
                                                 Name = o.Multilang.Name,
                                                 CategoryId = o.Organization.CategoryId,
                                                 CreatedAt = o.Organization.CreatedAt,
                                                 Status = o.Organization.Status,
                                                 SubcategoryId = o.Organization.SubcategoryId,
                                                 UserId = o.Organization.UserId
                                             })
                                             .OrderByDescending(o => o.CreatedAt).ToListAsync();

        public async Task<OrganizationFullInfoModel> GetOrganizationFullInfo(Guid organizationId) =>
            await _organization.OrganizationsFullInfo.FirstOrDefaultAsync(f => f.OrganizationId == organizationId);

        public async Task<OrganizationMultilangModel> GetOrganizationMultilang(Guid organizationId, string lang) =>
            await _organization.OrganizationsMultilang.FirstOrDefaultAsync(f => f.OrganizationId == organizationId && f.Lang == lang);
        public async Task<List<OrganizationFileURLModel>> GetFileUrls(Guid organizationId) =>
            await _organization.OrganizationFileUrls.Where(f => f.OrganizationId == organizationId).ToListAsync();

        public async Task<List<PlaceModel>> GetPlaces(Guid organizationId) =>
            await _organization.Places.Where(p => p.ApplicationId == organizationId).ToListAsync();

        public async Task<List<CityModel>> GetCities(string lang) =>
            await _organization.CitiesMultilang.Where(c => c.Lang == lang).Select(c => new CityModel() { Id = c.CityId, Name = c.Name }).ToListAsync();

        public async Task<SelectelFileURLModel> GetSelectelFile(Guid organizationId) =>
            await _organization.FileToApplication.Where(f => f.ApplicationId == organizationId)
                                                 .Join(_organization.SelectelFileURLs,
                                                       f => f.FileId,
                                                       s => s.Id,
                                                       (f, s) => s)
                                                 .FirstOrDefaultAsync();

        public async Task<List<MenuModel>> GetMenuList(string lang) =>
            await _organization.MenusMultilang.Where(m => m.Lang == lang)
                                              .OrderBy(m => m.Name)
                                              .Select(m => new MenuModel()
                                              {
                                                  Id = m.MenuId,
                                                  Name = m.Name
                                              }).ToListAsync();

        public async Task<List<MenuModel>> GetMenuList(Guid organizationId, string lang) =>
            await _organization.MenusToOrganization.Where(org => org.OrganizationId == organizationId)
                                                   .Join(_organization.MenusMultilang,
                                                         org => org.MenuId,
                                                         menu => menu.MenuId,
                                                         (org, menu) => menu)
                                                   .Where(m => m.Lang == lang)
                                                   .OrderBy(m => m.Name)
                                                   .Select(m => new MenuModel()
                                                   {
                                                        Id = m.MenuId,
                                                        Name = m.Name
                                                   }).ToListAsync();

        public async Task<OrganizationContactModel> GetContact(Guid organizationId) =>
            await _organization.OrganizationContacts.FirstOrDefaultAsync(o => o.OrganizationId == organizationId);

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

        public async Task SavePlaces(List<PlaceModel> places, Guid organizationId)
        {
            var old = await _organization.Places.Where(p => p.ApplicationId == organizationId).ToListAsync();
            if (old.Count > 0)
                _organization.Places.RemoveRange(old);

            if (places.Count > 0)
                await _organization.Places.AddRangeAsync(places);
            await _organization.SaveChangesAsync();
        }

        public async Task SaveFiles(List<OrganizationFileURLModel> list, Guid organizationId)
        {
            var old = await _organization.OrganizationFileUrls.Where(f => f.OrganizationId == organizationId).ToListAsync();
            if (old.Count > 0)
                _organization.OrganizationFileUrls.RemoveRange(old);

            if (list.Count > 0)
                await _organization.OrganizationFileUrls.AddRangeAsync(list);
            await _organization.SaveChangesAsync();
        }

        public async Task SaveMenus(List<MenuToOrganizationModel> list, Guid organizationId)
        {
            var old = await _organization.MenusToOrganization.Where(f => f.OrganizationId == organizationId).ToListAsync();
            if (old.Count > 0)
                _organization.MenusToOrganization.RemoveRange(old);

            if (list.Count > 0)
                await _organization.MenusToOrganization.AddRangeAsync(list);
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

        public async Task AddFilePoster(FileToApplication file)
        {
            await _organization.FileToApplication.AddAsync(file);
            await _organization.SaveChangesAsync();
        }

        public async Task AddSelectelFile(SelectelFileURLModel file)
        {
            await _organization.SelectelFileURLs.AddAsync(file);
            await _organization.SaveChangesAsync();
        }

        public async Task AddContact(OrganizationContactModel contact)
        {
            await _organization.OrganizationContacts.AddAsync(contact);
            await _organization.SaveChangesAsync();
        }

        public async Task UpdateContact(OrganizationContactModel contact)
        {
            _organization.OrganizationContacts.Update(contact);
            await _organization.SaveChangesAsync();
        }
    }
}
