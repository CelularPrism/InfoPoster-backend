using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Statistics
{
    public class GetStatisticTableByEditorRequest : IRequest<List<GetStatisticTableByEditorResponse>>
    {
        public int Method { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetStatisticTableByEditorResponse
    {
        public DateTime Date { get; set; }
        public int Deleted { get; set; }
        public int Drafted { get; set; }
        public int Pending { get; set; }
        public int Published { get; set; }
    }

    public class GetStatisticTableByEditorHandler : IRequestHandler<GetStatisticTableByEditorRequest, List<GetStatisticTableByEditorResponse>>
    {
        private readonly StatisticRepository _repository;

        public GetStatisticTableByEditorHandler(StatisticRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetStatisticTableByEditorResponse>> Handle(GetStatisticTableByEditorRequest request, CancellationToken cancellationToken = default)
        {
            List<ApplicationChangeHistory> statistic = null;
            var result = new List<GetStatisticTableByEditorResponse>();
            if (request.Method == 0)
            {

                statistic = await _repository.GetHistoryList(request.StartDate, request.EndDate);
                result = statistic.GroupBy(s => s.ChangedAt.Date)
                                  .Select(s => new GetStatisticTableByEditorResponse()
                                  {
                                      Date = s.Key,
                                      Pending = s.Count(s => s.NewValue == POSTER_STATUS.PENDING.ToString() || s.OldValue == "0"),
                                      Drafted = s.Count(s => s.NewValue == POSTER_STATUS.DRAFT.ToString() || s.OldValue == "1"),
                                      Deleted = s.Count(s => s.NewValue == POSTER_STATUS.DELETED.ToString() || s.OldValue == "2"),
                                      Published = s.Count(s => s.NewValue == POSTER_STATUS.PUBLISHED.ToString() || s.OldValue == "3")
                                  }).ToList();
            } else if (request.Method == 1)
            {
                statistic = await _repository.GetHistoryList();
                result = statistic.GroupBy(s => new { s.ChangedAt.Year, s.ChangedAt.Month })
                                  .Select(s => new GetStatisticTableByEditorResponse()
                                  {
                                      Date = new DateTime(s.Key.Year, s.Key.Month, 1),
                                      Pending = s.Count(s => s.NewValue == POSTER_STATUS.PENDING.ToString() || s.OldValue == "0"),
                                      Drafted = s.Count(s => s.NewValue == POSTER_STATUS.DRAFT.ToString() || s.OldValue == "1"),
                                      Deleted = s.Count(s => s.NewValue == POSTER_STATUS.DELETED.ToString() || s.OldValue == "2"),
                                      Published = s.Count(s => s.NewValue == POSTER_STATUS.PUBLISHED.ToString() || s.OldValue == "3")
                                  }).ToList();
            }

            return result;
        }
    }
}
