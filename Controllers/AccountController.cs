using InfoPoster_backend.Handlers.Account;
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

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
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
    }
}
