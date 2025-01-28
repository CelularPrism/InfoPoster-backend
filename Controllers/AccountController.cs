using InfoPoster_backend.Handlers.Account;
using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Handlers.Posters;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Services;
using InfoPoster_backend.Tools;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly EmailService _email;
        private readonly Guid _city;

        public AccountController(IMediator mediator, EmailService email, IHttpContextAccessor accessor)
        {
            _mediator = mediator;
            _email = email;
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequestModel request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
            {
                ModelState.AddModelError("Error", "Incorrect email or password");
                return BadRequest(ModelState);
            }    

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutRequest());
            return Ok(result);
        }


        [Authorize(AuthenticationSchemes = "Asymmetric"), HttpPost("user/update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchText)
        {
            var result = await _mediator.Send(new SearchApplicationRequest() { SearchText = searchText });
            return Ok(result);
        }

        [HttpPost("email/send")]
        public async Task<IActionResult> SendEmail([FromForm] string firstName, [FromForm] string email, [FromForm] string phone, [FromForm] string message)
        {
            message = string.Concat("Firstname: ", firstName, "<br>Email: ", email, "<br>Phone: ", phone, "<br>Message: ", message);
            try
            {
                await _email.Send(message, "jack@cityguide.vn", "jack@cityguide.vn", "Email from " + firstName);
            } catch (Exception ex)
            {
                return BadRequest();
            }
            
            return Ok();
        }

        [HttpGet("recently/added")]
        public async Task<IActionResult> GetRecentlyAddedApplications([FromQuery] CategoryType type)
        {
            if (type == CategoryType.PLACE)
            {
                var organizations = await _mediator.Send(new GetOrganizationsRequest() { startDate = DateTime.MinValue, endDate = DateTime.MaxValue, categoryId = null, Limit = 3, Offset = 0, subcategoryId = null });

                var result = organizations.data.Select(o => new GetRecentlyAddedApplicationsResponse()
                {
                    ReleaseDate = null,
                    FileURL = o.FileURL,
                    Id = o.Id,
                    Name = o.Name
                }).ToList();

                return Ok(result);
            } else
            {
                var posters = await _mediator.Send(new GetPostersRequest() { startDate = DateTime.MinValue, endDate = DateTime.MaxValue });
                posters = posters.OrderByDescending(p => p.ReleaseDate).Take(3).ToList();

                var result = posters.Select(p => new GetRecentlyAddedApplicationsResponse()
                {
                    ReleaseDate = p.ReleaseDate,
                    FileURL = p.FileURL,
                    Id = p.Id,
                    Name = p.Name
                }).ToList();

                return Ok(posters);
            }
        }
    }
}
