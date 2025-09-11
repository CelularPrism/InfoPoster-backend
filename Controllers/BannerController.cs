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
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.CATEGORY_PLACE });
            return Ok(result);
        }


        [HttpGet("event/get")]
        public async Task<IActionResult> GetPopularOnCategoryEvent()
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.CATEGORY_EVENT });
            return Ok(result);
        }

        [HttpGet("place/category/get")]
        public async Task<IActionResult> GetPopularOnPlaceSubcategory([FromQuery] Guid categoryId)
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.SUBCATEGORY_PLACE, PlaceId = categoryId });
            return Ok(result);
        }

        [HttpGet("event/category/get")]
        public async Task<IActionResult> GetPopularOnSubcategory([FromQuery] Guid categoryId)
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.SUBCATEGORY_EVENT, PlaceId = categoryId });
            return Ok(result);
        }

        [HttpGet("place/subcategory/get")]
        public async Task<IActionResult> GetPopularOnPlaceList([FromQuery] Guid subcategoryId)
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.LIST_APPLICATION_PLACE, PlaceId = subcategoryId });
            return Ok(result);
        }

        [HttpGet("event/subcategory/get")]
        public async Task<IActionResult> GetPopularOnEventList([FromQuery] Guid subcategoryId)
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.LIST_APPLICATION_EVENT, PlaceId = subcategoryId });
            return Ok(result);
        }

        [HttpGet("article/get")]
        public async Task<IActionResult> GetPopularOnArticleList()
        {
            var result = await _mediator.Send(new GetPublishedBannerListRequest() { Place = Models.Administration.POPULARITY_PLACE.LIST_APPLICATION_ARTICLE, LimitBanner = 1 });
            return Ok(result);
        }
    }
}
