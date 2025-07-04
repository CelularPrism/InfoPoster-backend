﻿using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class AdministrationGetRejectedPostersRequest : IRequest<AdministrationGetPostersResponse>
    {
        public int Sort { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SubcategoryId { get; set; }
        public Guid? CityId { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class AdministrationGetRejectedPostersHandler : IRequestHandler<AdministrationGetRejectedPostersRequest, AdministrationGetPostersResponse>
    {
        private readonly LoginService _loginService;
        private readonly PosterRepository _repository;
        private readonly string _lang;

        public AdministrationGetRejectedPostersHandler(LoginService loginService, PosterRepository repository, IHttpContextAccessor accessor)
        {
            _loginService = loginService;
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<AdministrationGetPostersResponse> Handle(AdministrationGetRejectedPostersRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _loginService.GetUserId();
            if (userId == Guid.Empty)
                return null;

            var posters = await _repository.GetRejectedListNoTracking(_lang, userId, request.CategoryId, request.SubcategoryId, request.Status, request.StartDate, request.EndDate, userId, request.CityId);
            var cities = await _repository.GetCities();
            var categories = await _repository.GetCategories();
            var subcategories = await _repository.GetSubcategories();
            var result = new AdministrationGetPostersResponse()
            {
                Count = posters.Count,
                CountPerPage = request.CountPerPage,
                Page = request.Page + 1
            };

            if (request.Sort == 0)
            {
                posters = posters.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else if (request.Sort == 1)
            {
                posters = posters.OrderByDescending(x => x.UpdatedAt).ToList();
            }
            else
            {
                posters = posters.OrderBy(x => x.Status).ToList();
            }

            posters = posters.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            var idEnum = posters.Select(x => x.Id);
            var multilang = await _repository.GetMultilang(idEnum);
            var fullInfo = await _repository.GetFullInfo(idEnum);

            var posterList = posters.Select(o => new AdministrationPostersResponse()
            {
                Id = o.Id,
                CategoryId = o.CategoryId != null ? (Guid)o.CategoryId : Guid.Empty,
                CategoryName = categories.Where(c => c.Id == o.CategoryId).Select(c => c.Name).FirstOrDefault(),
                Name = multilang.Where(m => m.PosterId == o.Id).Select(m => m.Name).FirstOrDefault(),
                CityId = fullInfo.Where(f => f.OrganizationId == o.Id).Select(f => f.City).FirstOrDefault(),
                CityName = fullInfo.Where(f => f.OrganizationId == o.Id)
                                   .Join(cities,
                                         f => f.City,
                                         c => c.Id,
                                         (f, c) => c.Name)
                                   .FirstOrDefault(),
                SubcategoryId = o.SubcategoryId != null ? (Guid)o.SubcategoryId : Guid.Empty,
                SubcategoryName = subcategories.Where(s => s.Id == o.SubcategoryId).Select(s => s.Name).FirstOrDefault(),
                CreatedAt = o.CreatedAt,
                Status = o.Status,
                UpdatedAt = o.UpdatedAt,
                ReleaseDate = o.ReleaseDate,
                ReleaseDateEnd = o.ReleaseDateEnd
            }).ToList();

            if (request.Sort == 0)
            {
                posterList = posterList.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else if (request.Sort == 1)
            {
                posterList = posterList.OrderByDescending(x => x.UpdatedAt).ToList();
            }
            else
            {
                posterList = posterList.OrderBy(x => x.Status).ToList();
            }

            posterList = posterList.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            result.Posters = posterList;

            return result;
        }
    }
}
