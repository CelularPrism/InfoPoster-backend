﻿using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationListRequest : IRequest<List<GetOrganizationListResponse>>
    {
        public int Sort { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? CityId { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetOrganizationListResponse
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public Guid? CityId { get; set; }
        public string CityName { get; set; }
    }

    public class GetOrganizationListHandler : IRequestHandler<GetOrganizationListRequest, List<GetOrganizationListResponse>>
    {
        private readonly OrganizationRepository _repository;
        private readonly LoginService _loginService;

        public GetOrganizationListHandler(OrganizationRepository repository, LoginService loginService)
        {
            _repository = repository;
            _loginService = loginService;
        }

        public async Task<List<GetOrganizationListResponse>> Handle(GetOrganizationListRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _loginService.GetUserId();
            var organizations = await _repository.GetOrganizationList(userId);

            if (request.CategoryId != null)
            {
                organizations = organizations.Where(x => x.CategoryId == request.CategoryId).ToList();
            }

            if (request.Status != null)
            {
                organizations = organizations.Where(x => x.Status == request.Status).ToList();
            }

            if (request.StartDate != null)
            {
                organizations = organizations.Where(x => x.CreatedAt >= request.StartDate).ToList();
            }

            if (request.EndDate != null)
            {
                organizations = organizations.Where(x => x.CreatedAt <= request.EndDate).ToList();
            }

            if (request.CityId != null)
            {
                organizations = organizations.Where(x => x.CityId == request.CityId).ToList();
            }

            var result = organizations.Select(o => new GetOrganizationListResponse()
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                Name = o.Name,
                Status = o.Status
            }).ToList();

            if (request.Sort == 0)
            {
                result = result.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else
            {
                result = result.OrderBy(x => x.Status).ToList();
            }

            return result;
        }
    }
}
