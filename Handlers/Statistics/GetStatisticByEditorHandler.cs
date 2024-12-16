using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Statistics
{
    public class GetStatisticByEditorRequest : IRequest<GetStatisticByEditorResponse>
    {
        public int Method { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }

    public class GetStatisticByEditorResponse
    {
        public List<StatisticModel> Draft {  get; set; }
        public List<StatisticModel> Pending { get; set; }
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
    }

    public class GetStatisticByEditorHandler : IRequestHandler<GetStatisticByEditorRequest, GetStatisticByEditorResponse> 
    {
        private readonly StatisticRepository _repository;

        public GetStatisticByEditorHandler(StatisticRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetStatisticByEditorResponse> Handle(GetStatisticByEditorRequest request, CancellationToken cancellationToken = default)
        {
            List<ApplicationChangeHistory> statistic = null;
            var result = new GetStatisticByEditorResponse();
            if (request.Method == 0)
            {
                statistic = await _repository.GetHistoryList(request.DateStart, request.DateEnd);
                result.Draft = statistic.Where(s => s.NewValue == POSTER_STATUS.DRAFT.ToString() || s.OldValue == "1")
                                        .GroupBy(s => s.ChangedAt.Date)
                                        .Select(s => new StatisticModel()
                                        {
                                            Date = s.Key,
                                            Count = s.Count()
                                        }).ToList();

                result.Pending = statistic.Where(s => s.NewValue == POSTER_STATUS.PENDING.ToString() || s.OldValue == "0")
                                        .GroupBy(s => s.ChangedAt.Date)
                                        .Select(s => new StatisticModel()
                                        {
                                            Date = s.Key,
                                            Count = s.Count()
                                        }).ToList();

                for (var day = request.DateStart.Date; day <= request.DateEnd.Date; day = day.AddDays(1))
                {
                    if (!result.Draft.Any(s => s.Date == day))
                        result.Draft.Add(new StatisticModel() { Date = day, Count = 0 });

                    if (!result.Pending.Any(s => s.Date == day))
                        result.Pending.Add(new StatisticModel() { Date = day, Count = 0 });

                    result.Dates.Add(day);
                }

                result.Draft = result.Draft.OrderBy(s => s.Date).ToList();
                result.Pending = result.Pending.OrderBy(s => s.Date).ToList();
            }
            else if (request.Method == 1)
            {
                statistic = await _repository.GetHistoryList();
                result.Draft = statistic.Where(s => s.NewValue == POSTER_STATUS.DRAFT.ToString() || s.OldValue == "1")
                                        .GroupBy(s => new { s.ChangedAt.Date.Year, s.ChangedAt.Date.Month })
                                        .Select(s => new StatisticModel()
                                        {
                                            Date = new DateTime(s.Key.Year, s.Key.Month, 1),
                                            Count = s.Count()
                                        }).OrderBy(s => s.Date)
                                        .ToList();

                result.Pending = statistic.Where(s => s.NewValue == POSTER_STATUS.PENDING.ToString() || s.OldValue == "0")
                                        .GroupBy(s => new { s.ChangedAt.Date.Year, s.ChangedAt.Date.Month })
                                        .Select(s => new StatisticModel()
                                        {
                                            Date = new DateTime(s.Key.Year, s.Key.Month, 1),
                                            Count = s.Count()
                                        }).OrderBy(s => s.Date)
                                        .ToList();

                for (var day = request.DateStart.Date; day <= request.DateEnd.Date; day = day.AddMonths(1))
                {
                    result.Dates.Add(new DateTime(day.Year, day.Month, 1));
                }
            }

            return result;
        }
    }
}
