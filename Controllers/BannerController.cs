using InfoPoster_backend.Handlers.Banner;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BannerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("main/get")]
        public async Task<IActionResult> GetPopularOnMain()
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.MAIN });
            return Ok(result);
        }


        [HttpGet("place/get")]
        public async Task<IActionResult> GetPopularOnCategoryPlace()
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.CATEGORY, Type = Models.CategoryType.PLACE });
            return Ok(result);
        }


        [HttpGet("event/get")]
        public async Task<IActionResult> GetPopularOnCategoryEvent()
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.CATEGORY, Type = Models.CategoryType.EVENT });
            return Ok(result);
        }

        [HttpGet("category/get")]
        public async Task<IActionResult> GetPopularOnSubcategory([FromQuery] Guid categoryId)
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.CATEGORY, PlaceId = categoryId });
            return Ok(result);
        }

        [HttpGet("subcategory/get")]
        public async Task<IActionResult> GetPopularOnApplicationList([FromQuery] Guid subcategoryId)
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.SUBCATEGORY, PlaceId = subcategoryId });
            return Ok(result);
        }
    }
}
