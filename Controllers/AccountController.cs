using InfoPoster_backend.Handlers.Account;
using InfoPoster_backend.Services;
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

        public AccountController(IMediator mediator, EmailService email)
        {
            _mediator = mediator;
            _email = email;
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

        [HttpPost("email/send")]
        public async Task<IActionResult> SendEmail([FromForm] string firstName, [FromForm] string email, [FromForm] string phone, [FromForm] string message)
        {
            message = string.Concat("Firstname: ", firstName, "\\nEmail: ", email, "\\nPhone: ", phone, "\\nMessage: ", message);
            try
            {
                await _email.Send(message, "jack@cityguide.vn", "jack@cityguide.vn", "Email from " + firstName);
            } catch (Exception ex)
            {
                return BadRequest();
            }
            
            return Ok();
        }
    }
}
