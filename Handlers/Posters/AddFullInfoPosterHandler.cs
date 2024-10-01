//using InfoPoster_backend.Models.Posters;
//using InfoPoster_backend.Repos;
//using MediatR;

//namespace InfoPoster_backend.Handlers.Posters
//{
//    public class AddFullInfoPosterRequest : IRequest<AddFullInfoPosterResponse>
//    {
//        public Guid PosterId { get; set; }
//        public string Lang { get; set; }
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public DateTime ReleaseDate { get; set; }
//        public Guid CategoryId { get; set; }
//        public string Place { get; set; }
//        public string City { get; set; }
//        public string TimeStart { get; set; }
//        public double Price { get; set; }
//        public string Adress { get; set; }
//        public string latitude { get; set; }
//        public string longitude { get; set; }
//        public string Parking { get; set; }
//        public string ParkingPlace { get; set; }
//        public string Tags { get; set; }
//        public string SocialLinks { get; set; }
//        public string Phone { get; set; }
//        public string SiteLink { get; set; }
//        public string AgeRestriction { get; set; }
//        public List<string> GaleryUrls { get; set; }
//        public List<string> VideoUrls { get; set; }
//    }

//    public class AddFullInfoPosterResponse
//    {
//        public Guid Id { get; set; }
//    }

//    public class AddFullInfoPosterHandler : IRequestHandler<AddFullInfoPosterRequest, AddFullInfoPosterResponse>
//    {
//        private readonly PosterRepository _repository;
//        public AddFullInfoPosterHandler(PosterRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<AddFullInfoPosterResponse> Handle(AddFullInfoPosterRequest request, CancellationToken cancellationToken = default)
//        {
//            var poster = await _repository.GetPoster(request.PosterId);
//            if (poster == null)
//                return null;

//            var fullInfo = new PosterFullInfoModel()
//            {
//                PosterId = request.PosterId,
//                AgeRestriction = request.AgeRestriction,
//                CategoryId = request.CategoryId,
//                Latitude = request.latitude,
//                Longitude = request.longitude,
//                Price = request.Price,
//                TimeStart = request.TimeStart
//            };

//            var multilang = new PosterMultilangModel(request);

//            poster.ReleaseDate = request.ReleaseDate;
//            poster.CategoryId = request.CategoryId;
//            if (string.IsNullOrEmpty(poster.Name))
//                poster.Name = request.Name;

//            await _repository.AddPosterFullInfo(fullInfo);
//            await _repository.AddPosterMultilang(multilang);
//            await _repository.UpdatePoster(poster);

//            var result = new AddFullInfoPosterResponse()
//            {
//                Id = poster.Id
//            };
//            return result;
//        }
//    }
//}
