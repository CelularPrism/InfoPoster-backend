using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Organizations.Menu;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Tools;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class OrganizationRepository
    {
        private readonly AccountContext _account;
        private readonly OrganizationContext _organization;
        private readonly string _lang;
        private readonly Guid _city;

        public OrganizationRepository(AccountContext account, OrganizationContext organization, IHttpContextAccessor accessor)
        {
            _account = account;
            _organization = organization;
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<OrganizationModel> GetOrganization(Guid id) =>
            await _organization.Organizations.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<CategoryModel>> GetCategories() => await _organization.Categories.Join(_organization.CategoriesMultilang,
                                                                                                      c => c.Id,
                                                                                                      m => m.CategoryId,
                                                                                                      (c, m) => new { Category = c, Multilang = m })
                                                                                                .Where(c => c.Multilang.lang == _lang && c.Category.Type == (int)CategoryType.PLACE)
                                                                                                .Select(c => new CategoryModel()
                                                                                                {
                                                                                                    Id = c.Category.Id,
                                                                                                    Name = c.Multilang.Name,
                                                                                                    ImageSrc = c.Category.ImageSrc,
                                                                                                    Type = c.Category.Type,
                                                                                                }).ToListAsync();

        public async Task<CategoryModel> GetCategory(Guid categoryId) => await _organization.Categories.Join(_organization.CategoriesMultilang,
                                                                                                      c => c.Id,
                                                                                                      m => m.CategoryId,
                                                                                                      (c, m) => new { Category = c, Multilang = m })
                                                                                                .Where(c => c.Multilang.lang == _lang && c.Category.Id == categoryId)
                                                                                                .Select(c => new CategoryModel()
                                                                                                {
                                                                                                    Id = c.Category.Id,
                                                                                                    Name = c.Multilang.Name,
                                                                                                    ImageSrc = c.Category.ImageSrc,
                                                                                                    Type = c.Category.Type,
                                                                                                }).FirstOrDefaultAsync();

        public async Task<List<SubcategoryModel>> GetSubcategories() => await _organization.Subcategories.Join(_organization.SubcategoriesMultilang,
                                                                                                      c => c.Id,
                                                                                                      m => m.SubcategoryId,
                                                                                                      (c, m) => new { Subcategory = c, Multilang = m })
                                                                                                .Where(c => c.Multilang.lang == _lang)
                                                                                                .Select(c => new SubcategoryModel()
                                                                                                {
                                                                                                    Id = c.Subcategory.Id,
                                                                                                    Name = c.Multilang.Name,
                                                                                                    ImageSrc = c.Subcategory.ImageSrc
                                                                                                }).ToListAsync();

        public async Task<SubcategoryModel> GetSubcategory(Guid subcategoryId) => await _organization.Subcategories.Join(_organization.SubcategoriesMultilang,
                                                                                                      c => c.Id,
                                                                                                      m => m.SubcategoryId,
                                                                                                      (c, m) => new { Subcategory = c, Multilang = m })
                                                                                                .Where(c => c.Multilang.lang == _lang && c.Subcategory.Id == subcategoryId)
                                                                                                .Select(c => new SubcategoryModel()
                                                                                                {
                                                                                                    Id = c.Subcategory.Id,
                                                                                                    Name = c.Multilang.Name,
                                                                                                    ImageSrc = c.Subcategory.ImageSrc,
                                                                                                    CategoryId = c.Subcategory.CategoryId
                                                                                                }).FirstOrDefaultAsync();

        public async Task<List<OrganizationModel>> GetOrganizationList() => await _organization.Organizations.Where(o => o.Status == (int)POSTER_STATUS.PUBLISHED)
                                                                 .Join(_organization.OrganizationsMultilang,
                                                                       o => o.Id,
                                                                       m => m.OrganizationId,
                                                                       (o, m) => new { Organization = o, Multilang = m })
                                                                 .Where(m => m.Multilang.Lang == _lang)
                                                                 .Select(o => new OrganizationModel()
                                                                 {
                                                                     Id = o.Organization.Id,
                                                                     CategoryId = o.Organization.CategoryId,
                                                                     CreatedAt = o.Organization.CreatedAt,
                                                                     Name = o.Multilang.Name,
                                                                     Status = o.Organization.Status,
                                                                     SubcategoryId = o.Organization.SubcategoryId,
                                                                     UserId = o.Organization.UserId
                                                                 }).ToListAsync();

        public async Task<List<GetAllOrganizationResponse>> GetOrganizationList(string lang)
        {
            var organizations = await _organization.Organizations.Where(o => o.Status == (int)POSTER_STATUS.PENDING || o.Status == (int)POSTER_STATUS.PUBLISHED)
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

        public async Task<List<OrganizationMultilangModel>> GetOrganizationMultilangList(Guid organizationId) =>
            await _organization.OrganizationsMultilang.Where(f => f.OrganizationId == organizationId).ToListAsync();

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

        public async Task<List<Guid>> GetMenuList(Guid organizationId, string lang) =>
            await _organization.MenusToOrganization.Where(org => org.OrganizationId == organizationId)
                                                   .Join(_organization.MenusMultilang,
                                                         org => org.MenuId,
                                                         menu => menu.MenuId,
                                                         (org, menu) => menu)
                                                   .Where(m => m.Lang == lang)
                                                   .OrderBy(m => m.Name)
                                                   .Select(m => m.MenuId).ToListAsync();

        public async Task<ContactModel> GetContact(Guid organizationId) =>
            await _organization.Contacts.FirstOrDefaultAsync(o => o.ApplicationId == organizationId);

        public async Task<GetFullInfoOrganizationResponse> GetFullInfo(Guid id)
        {
            var model = await _organization.Organizations.Where(o => o.Id == id)
                                               .Join(_organization.OrganizationsFullInfo,
                                                     o => o.Id,
                                                     f => f.OrganizationId,
                                                     (o, f) => new { o, f })
                                               .Join(_organization.OrganizationsMultilang,
                                                    o => o.f.OrganizationId,
                                                    m => m.OrganizationId,
                                                    (o, m) => new { o, m })
                                               .Where(o => o.m.Lang == _lang)
                                               .Select(o => new { Organization = o.o.o, FullInfo = o.o.f, Multilang = o.m })
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync();

            var poster = new GetFullInfoOrganizationResponse(model.Organization, model.FullInfo, model.Multilang);
            poster.CategoryName = await _organization.CategoriesMultilang.Where(c => c.CategoryId == poster.CategoryId && c.lang == _lang)
                                                                    .AsNoTracking()
                                                                    .Select(c => c.Name)
                                                                    .FirstOrDefaultAsync();

            poster.SubcategoryName = await _organization.SubcategoriesMultilang.Where(c => c.SubcategoryId == poster.SubcategoryId && c.lang == _lang)
                                                                    .AsNoTracking()
                                                                    .Select(c => c.Name)
                                                                    .FirstOrDefaultAsync();

            poster.City = await _organization.CitiesMultilang.Where(c => c.CityId == poster.CityId && c.Lang == _lang)
                                                                    .AsNoTracking()
                                                                    .Select(c => c.Name)
                                                                    .FirstOrDefaultAsync();

            var files = await _organization.FileUrls.Where(f => f.PosterId == id)
                                               .AsNoTracking()
                                               .ToListAsync();

            poster.SocialLinks = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.SOCIAL_LINKS).Select(f => f.URL).ToList();
            poster.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();

            var places = await _organization.Places.Where(p => p.ApplicationId == id)
                                              .AsNoTracking()
                                              .ToListAsync();

            poster.Parking = places;
            poster.Lang = _lang;

            poster.MenuCategories = await _organization.MenusToOrganization.Where(m => m.OrganizationId == id)
                                                                           .Join(_organization.MenusMultilang,
                                                                                 org => org.MenuId,
                                                                                 ml => ml.MenuId,
                                                                                 (org, ml) => ml)
                                                                           .Where(ml => ml.Lang == _lang)
                                                                           .Select(ml => ml.Name)
                                                                           .ToListAsync();
            return poster;
        }

        public async Task<List<PlaceModel>> GetPlaceList(Guid organizationId) =>
            await _organization.Places.Where(p => p.ApplicationId == organizationId).ToListAsync();

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

        public async Task AddMultilang(List<OrganizationMultilangModel> model)
        {
            await _organization.OrganizationsMultilang.AddRangeAsync(model);
            await _organization.SaveChangesAsync();
        }

        public async Task AddPlaces(List<PlaceModel> places)
        {
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

        public async Task UpdateMultilang(List<OrganizationMultilangModel> model)
        {
            _organization.OrganizationsMultilang.UpdateRange(model);
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

        public async Task AddContact(ContactModel contact)
        {
            await _organization.Contacts.AddAsync(contact);
            await _organization.SaveChangesAsync();
        }

        public async Task UpdateContact(ContactModel contact)
        {
            _organization.Contacts.Update(contact);
            await _organization.SaveChangesAsync();
        }

        public async Task RemovePlaceList(List<PlaceModel> model)
        {
            _organization.Places.RemoveRange(model);
            await _organization.SaveChangesAsync();
        }
    }
}
