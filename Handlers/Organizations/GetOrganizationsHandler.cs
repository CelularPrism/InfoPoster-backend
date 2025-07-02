using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;
using System.Diagnostics;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationsRequest : IRequest<GetOrganizationsResponse>
    {
        public DateTime startDate { get; set; } = DateTime.MinValue;
        public DateTime endDate { get; set; } = DateTime.UtcNow;
        public Guid? categoryId { get; set; }
        public Guid? subcategoryId { get; set; }
        public int Limit { get; set; } = 9;
        public int Offset { get; set; } = 0;
    }

    public class GetOrganizationsResponse
    {
        public List<InfoOrganization> data { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }

    public class InfoOrganization
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<IdNameModel> Category { get; set; }
        public List<IdNameModel> Subcategory { get; set; }
        public string Place { get; set; }
        public Guid? FileId { get; set; }
        public string FileURL { get; set; }
        public string PriceLevel { get; set; }
    }

    public class GetOrganizationsHandler : IRequestHandler<GetOrganizationsRequest, GetOrganizationsResponse> 
    {
        private readonly OrganizationRepository _repository;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectelAuth;
        private readonly Guid _city;

        public GetOrganizationsHandler(OrganizationRepository organizationRepository, FileRepository file, SelectelAuthService selectelAuth, IHttpContextAccessor accessor)
        {
            _repository = organizationRepository;
            _file = file;
            _selectelAuth = selectelAuth;
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<GetOrganizationsResponse> Handle(GetOrganizationsRequest request, CancellationToken cancellationToken = default)
        {
            var categsTask = _repository.GetApplicationCategories();
            var organizationList = await _repository.GetOrganizationList(string.Empty, Guid.Empty, new List<int>() { (int)POSTER_STATUS.PUBLISHED }, request.categoryId, request.startDate, request.endDate, null, _city);
            if (request.subcategoryId != null)
            {
                organizationList = organizationList.Where(o => o.SubcategoryId == request.subcategoryId).ToList();
            }

            organizationList = organizationList.OrderByDescending(o => o.CreatedAt).Skip(request.Offset).Take(request.Limit).ToList();

            var organizationIds = organizationList.Select(org => org.Id).AsEnumerable();
            var multilang = await _repository.GetMultilang(organizationIds);

            var data = new List<InfoOrganization>();
            if (request.categoryId != null && request.subcategoryId != null) 
            {
                var categories = await _repository.GetCategories();
                var subcategories = await _repository.GetSubcategories();

                var categs = await categsTask;
                var orgs = categs.Where(c => c.CategoryId == request.categoryId && c.SubcategoryId == request.subcategoryId).Select(c => c.ApplicationId).ToList();
 
                data = organizationList.Where(o => orgs.Contains(o.Id))
                                             .Select(o => new InfoOrganization()
                                             {
                                                 Category = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.CategoryId).Select(c => new IdNameModel()
                                                 {
                                                     Id = c.Key,
                                                     Name = categories.Where(cat => cat.Id == c.Key).Select(cat => cat.Name).FirstOrDefault()
                                                 }).OrderBy(c => c.Name).ToList(),
                                                 Subcategory = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.SubcategoryId).Select(c => new IdNameModel()
                                                 {
                                                     Id = c.Key,
                                                     Name = subcategories.Where(cat => cat.Id == c.Key).Select(cat => cat.Name).FirstOrDefault()
                                                 }).OrderBy(c => c.Name).ToList(),
                                                 Id = o.Id,
                                                 Name = multilang.Where(m => m.OrganizationId == o.Id).Select(o => o.Name).FirstOrDefault(),
                                                 CreatedAt = o.CreatedAt,
                                                 PriceLevel = _repository.GetPriceLevel(o.Id),
                                                 Description = _repository.GetDescription(o.Id),
                                             }).ToList();
            } else if (request.subcategoryId != null)
            {
                var categories = await _repository.GetCategories();
                var subcategories = await _repository.GetSubcategories();

                var categs = await categsTask;
                var orgs = categs.Where(c => c.SubcategoryId == request.subcategoryId).Select(c => c.ApplicationId).ToList();

                data = organizationList.Where(o => orgs.Contains(o.Id))
                                             .Select(o => new InfoOrganization()
                                             {
                                                 Category = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.CategoryId).Select(c => new IdNameModel()
                                                 {
                                                     Id = c.Key,
                                                     Name = categories.Where(cat => cat.Id == c.Key).Select(cat => cat.Name).FirstOrDefault()
                                                 }).OrderBy(c => c.Name).ToList(),
                                                 Subcategory = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.SubcategoryId).Select(c => new IdNameModel()
                                                 {
                                                     Id = c.Key,
                                                     Name = subcategories.Where(cat => cat.Id == c.Key).Select(cat => cat.Name).FirstOrDefault()
                                                 }).OrderBy(c => c.Name).ToList(),
                                                 Id = o.Id,
                                                 Name = multilang.Where(m => m.OrganizationId == o.Id).Select(o => o.Name).FirstOrDefault(),
                                                 CreatedAt = o.CreatedAt,
                                                 PriceLevel = _repository.GetPriceLevel(o.Id),
                                                 Description = _repository.GetDescription(o.Id),
                                             }).ToList();
            } else if (request.categoryId != null)
            {
                var categories = await _repository.GetCategories();
                var subcategories = await _repository.GetSubcategories();

                var categs = await categsTask;
                var orgs = categs.Where(c => c.CategoryId == request.categoryId).Select(c => c.ApplicationId).ToList();

                data = organizationList.Where(o => orgs.Contains(o.Id))
                                             .Select(o => new InfoOrganization()
                                             {
                                                 Category = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.CategoryId).Select(c => new IdNameModel()
                                                 {
                                                     Id = c.Key,
                                                     Name = categories.Where(cat => cat.Id == c.Key).Select(cat => cat.Name).FirstOrDefault()
                                                 }).OrderBy(c => c.Name).ToList(),
                                                 Subcategory = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.SubcategoryId).Select(c => new IdNameModel()
                                                 {
                                                     Id = c.Key,
                                                     Name = subcategories.Where(cat => cat.Id == c.Key).Select(cat => cat.Name).FirstOrDefault()
                                                 }).OrderBy(c => c.Name).ToList(),
                                                 Id = o.Id,
                                                 Name = multilang.Where(m => m.OrganizationId == o.Id).Select(o => o.Name).FirstOrDefault(),
                                                 CreatedAt = o.CreatedAt,
                                                 PriceLevel = _repository.GetPriceLevel(o.Id),
                                                 Description = _repository.GetDescription(o.Id),
                                             }).ToList();
            } else
            {
                var categories = await _repository.GetCategories();
                var subcategories = await _repository.GetSubcategories();

                var categs = await categsTask;

                data = organizationList.Select(o => new InfoOrganization()
                                         {
                                                 Category = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.CategoryId).Select(c => new IdNameModel()
                                                 {
                                                     Id = c.Key,
                                                     Name = categories.Where(cat => cat.Id == c.Key).Select(cat => cat.Name).FirstOrDefault()
                                                 }).OrderBy(c => c.Name).ToList(),
                                                 Subcategory = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.SubcategoryId).Select(c => new IdNameModel()
                                                 {
                                                     Id = c.Key,
                                                     Name = subcategories.Where(cat => cat.Id == c.Key).Select(cat => cat.Name).FirstOrDefault()
                                                 }).OrderBy(c => c.Name).ToList(),
                                                Id = o.Id,
                                                Name = multilang.Where(m => m.OrganizationId == o.Id).Select(o => o.Name).FirstOrDefault(),
                                                CreatedAt = o.CreatedAt,
                                                PriceLevel = _repository.GetPriceLevel(o.Id),
                                                Description = _repository.GetDescription(o.Id),
                                         }).ToList();
            }

            var loggedIn = await _selectelAuth.Login();
            if (loggedIn)
            {
                var selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var item in data)
                {
                    var file = await _file.GetPrimaryFile(item.Id, (int)FILE_PLACES.GALLERY);
                    if (file == null)
                        file = await _file.GetApplicationFileByApplication(item.Id);

                    if (file != null)
                    {
                        item.FileId = file.FileId;
                        item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", item.FileId);
                    }
                }
            }
            var total = data.Count;

            var result = new GetOrganizationsResponse()
            {
                data = data,
                Limit = request.Limit,
                Offset = request.Offset,
                Total = total
            };

            return result;
        }

    }
}
