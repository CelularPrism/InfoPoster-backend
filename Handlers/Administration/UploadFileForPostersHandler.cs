using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;
using System;
using System.Text.Json;

namespace InfoPoster_backend.Handlers.Administration
{
    public class UploadFileForPostersRequest : IRequest<UploadFileForPostersResponse>
    {
        public IFormFile File { get; set; }
    }

    public class UploadFileForPostersResponse { }

    public class UploadFileForPostersHandler : IRequestHandler<UploadFileForPostersRequest, UploadFileForPostersResponse>
    {
        private readonly PosterRepository _repository;
        private Guid _user;

        public UploadFileForPostersHandler(PosterRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<UploadFileForPostersResponse> Handle(UploadFileForPostersRequest request, CancellationToken cancellationToken = default)
        {
            string content = string.Empty;
            if (request.File != null)
            {
                content = FileConverter.ReadIFormFileContent(request.File);
                var deserializedResponse = JsonSerializer.Deserialize<PosterExportFileModel>(content);

                PosterModel poster;
                PosterFullInfoModel fullInfo;
                List<PosterMultilangModel> availableMultilang;

                var posterList = new List<PosterModel>();
                var multilang = new List<PosterMultilangModel>();
                var fullInfoList = new List<PosterFullInfoModel>();
                var files = new List<FileURLModel>();

                var countMl = 0;
                var successLang = false;

                foreach (var element in deserializedResponse.Array)
                {
                    poster = new PosterModel()
                    {
                        Id = Guid.NewGuid(),
                        ReleaseDateEnd = element.ReleaseDateEnd,
                        ReleaseDate = element.ReleaseDateStart,
                        CategoryId = element.CategoryId == null ? Guid.Empty : (Guid)element.CategoryId,
                        CreatedAt = DateTime.UtcNow,
                        Name = element.Name,
                        Status = (int)POSTER_STATUS.DRAFT,
                        UpdatedAt = DateTime.UtcNow,
                        UserId = _user
                    };

                    fullInfo = new PosterFullInfoModel()
                    {
                        Id = Guid.NewGuid(),
                        AgeRestriction = element.AgeRestriction,
                        CategoryId = element.CategoryId,
                        City = element.City,
                        PlaceLink = element.PlaceLink,
                        PosterId = poster.Id,
                        Price = element.Price,
                        TimeStart = element.TimeStart
                    };

                    countMl = 0;
                    availableMultilang = new List<PosterMultilangModel>();
                    successLang = true;

                    foreach (var lang in Constants.SystemLangs)
                    {
                        var ml = element.Langs.Where(e => e.Lang == lang).FirstOrDefault();
                        if (ml == null)
                        {
                            ml = element.Langs.Where(e => e.Lang == Constants.DefaultLang).FirstOrDefault();
                        }

                        if (ml != null)
                        {
                            var mlModel = new PosterMultilangModel()
                            {
                                Id = Guid.NewGuid(),
                                Adress = ml.Adress,
                                Description = ml.Description,
                                Lang = lang,
                                Name = ml.Name,
                                Phone = ml.Phone,
                                Place = ml.Place,
                                PosterId = poster.Id,
                                SiteLink = ml.SiteLink,
                                Tickets = ml.Tickets
                            };

                            availableMultilang.Add(mlModel);
                            countMl++;
                        }
                        else
                        {
                            successLang = false;
                        }
                    }

                    if (countMl == Constants.SystemLangs.Count && successLang && (element.SocialLinks.Split(',').Length < 2 || element.SocialLinks.Split(' ').Length < 2))
                    {
                        posterList.Add(poster);
                        fullInfoList.Add(fullInfo);
                        multilang.AddRange(availableMultilang);
                        if (!string.IsNullOrEmpty(element.SocialLinks))
                        {
                            var fileUrl = $"<p><a href=\"{element.SocialLinks}\" rel=\"noopener noreferrer\" target=\"_blank\"> {element.SocialLinks}</a></p>";
                            files.Add(new FileURLModel(poster.Id, fileUrl, (int)FILE_CATEGORIES.SOCIAL_LINKS));
                        }
                    }
                }

                await _repository.AddPoster(posterList, _user);
                await _repository.AddPosterFullInfo(fullInfoList);
                await _repository.AddPosterMultilang(multilang);
                await _repository.SaveFiles(files, Guid.Empty);
                return new UploadFileForPostersResponse();
            }

            return null;
        }
    }
}
