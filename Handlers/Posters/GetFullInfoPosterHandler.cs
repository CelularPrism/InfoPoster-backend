using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetFullInfoPosterRequest : IRequest<PosterFullInfoResponseModel>
    {
        public Guid Id { get; set; }
    }

    public class GetFullInfoPosterHandler : IRequestHandler<GetFullInfoPosterRequest, PosterFullInfoResponseModel>
    {
        private readonly PosterRepository _repository;
        private readonly string _lang;

        public GetFullInfoPosterHandler(PosterRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<PosterFullInfoResponseModel> Handle(GetFullInfoPosterRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetFullInfo(request.Id, _lang);
            var viewLog = new PosterViewLogModel()
            {
                Id = Guid.NewGuid(),
                PosterId = request.Id,
                Date = DateTime.UtcNow
            };

            await _repository.AddViewLog(viewLog);
            return result;
        }
    }
}
