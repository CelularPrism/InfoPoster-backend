using InfoPoster_backend.Handlers.Posters;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosterController : ControllerBase
    {
        private readonly IMediator _mediator;

        [HttpGet]
        public async Task<IActionResult> GetPosters([FromQuery] GetPostersRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostersByCategory([FromQuery] GetPostersByCategoryRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesRequest());
            return Ok(result);
        }

        [HttpGet("full-info")]
        public async Task<IActionResult> GetFullInfoPoster([FromQuery] GetFullInfoPosterRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
