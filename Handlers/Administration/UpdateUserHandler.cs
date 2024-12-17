using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class UpdateUserRequest : IRequest<UpdateUserResponse>
    {
        public Guid Id { get; set; }
        public bool IsBlocked { get; set; }
    }

    public class UpdateUserResponse { }

    public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, UpdateUserResponse>
    {
        private readonly AccountRepository _repository;
        public UpdateUserHandler(AccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetUser(request.Id);
            if (user == null)
                return null;

            user.IsBlocked = request.IsBlocked;
            await _repository.UpdateUser(user);
            return new UpdateUserResponse();
        }
    }
}
