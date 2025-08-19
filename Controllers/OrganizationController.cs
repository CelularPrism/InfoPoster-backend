using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Handlers.Posters;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Administration;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrganizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganizations([FromQuery] GetOrganizationsRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetOrganizationsCount()
        {
            var result = await _mediator.Send(new GetOrganizationsCountRequest());
            return Ok(result);
        }

        [HttpGet("actual")]
        public async Task<IActionResult> GetActualOrganizations()
        {
            var result = await _mediator.Send(new GetOrganizationsRequest() { startDate = DateTime.MinValue, endDate = DateTime.MaxValue, Limit = 10, Offset = 0 });
            return Ok(result);
        }

        [HttpGet("by-category")]
        public async Task<IActionResult> GetOrganizationsByCategory([FromQuery] GetOrganizationsRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("by-subcategory")]
        public async Task<IActionResult> GetOrganizationsBySubcategory([FromQuery] GetOrganizationsRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        //[HttpGet("categories")]
        //public async Task<IActionResult> GetCategories(CategoryType type)
        //{
        //    var result = await _mediator.Send(new GetCategoriesRequest() { type = type });
        //    return Ok(result);
        //}

        //[HttpGet("subcategories")]
        //public async Task<IActionResult> GetSubcategories([FromQuery] GetSubcategoriesRequest request)
        //{
        //    var result = await _mediator.Send(request);
        //    return Ok(result);
        //}

        [HttpGet("full-info")]
        public async Task<IActionResult> GetFullInfoOrganization([FromQuery] GetFullInfoOrganizationRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularOrganizations([FromQuery] Guid? categoryId, [FromQuery] POPULARITY_PLACE place = POPULARITY_PLACE.MAIN)
        {
            var result = await _mediator.Send(new GetPopularOrganizationsRequest() { Place = place, CategoryId = categoryId });
            return Ok(result);
        }

        [HttpGet("popular/by-subcategory")]
        public async Task<IActionResult> GetPopularBySubcategory([FromQuery] Guid subcategoryId)
        {
            var response = await _mediator.Send(new GetOrganizationsRequest() { subcategoryId = subcategoryId });
            return Ok(response.data);
        }
    }
}
