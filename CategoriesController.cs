using InfoPoster_backend.Handlers.Posters;
using InfoPoster_backend.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryType type)
        {
            var result = await _mediator.Send(new GetCategoriesRequest() { type = type });
            return Ok(result);
        }

        [HttpGet("subcategories")]
        public async Task<IActionResult> GetSubcategories([FromQuery] GetSubcategoriesRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
