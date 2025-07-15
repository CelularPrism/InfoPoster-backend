using InfoPoster_backend.Handlers.Articles;
using InfoPoster_backend.Handlers.Articles.Popularity;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Contexts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpGet]
        public async Task<IActionResult> GetArticleList([FromQuery] int page = 1, [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new GetArticleListRequest() { Page = page - 1, CountPerPage = countPerPage });
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateArticle()
        {
            var result = await _mediator.Send(new CreateArticleRequest());
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPost("save")]
        public async Task<IActionResult> SaveArticle([FromBody] SaveArticleRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpGet("full-info")]
        public async Task<IActionResult> GetArticle([FromQuery] Guid id)
        {
            var result = await _mediator.Send(new GetArticleRequest() { Id = id });
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPost("draft")]
        public async Task<IActionResult> DraftArticle([FromForm] Guid id)
        {
            var result = await _mediator.Send(new ChangeArticleStatusRequest() { Id = id, Status = Models.Posters.POSTER_STATUS.DRAFT });
            if (result == null)
                return BadRequest();
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteArticle([FromForm] Guid id)
        {
            var result = await _mediator.Send(new ChangeArticleStatusRequest() { Id = id, Status = Models.Posters.POSTER_STATUS.DELETED });
            if (result == null)
                return BadRequest();
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Asymmetric")]
        [HttpPost("publish")]
        public async Task<IActionResult> PublishArticle([FromForm] Guid id)
        {
            var result = await _mediator.Send(new ChangeArticleStatusRequest() { Id = id, Status = Models.Posters.POSTER_STATUS.PUBLISHED });
            if (result == null)
                return BadRequest();
            return Ok(result);
        }

        [HttpGet("main/get")]
        public async Task<IActionResult> GetPublishedArticleList()
        {
            var result = await _mediator.Send(new GetPublishedArticlesListRequest() { Place = Models.Administration.POPULARITY_PLACE.MAIN });
            return Ok(result);
        }

        [HttpPost("popularity/add")]
        public async Task<IActionResult> AddPopularityPoster([FromBody] AddPopularityArticleRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("popularity/get")]
        public async Task<IActionResult> GetPopularityPoster([FromQuery] Guid cityId)
        {
            var result = await _mediator.Send(new GetPopularityArticleRequest() { Place = Models.Administration.POPULARITY_PLACE.MAIN, CityId = cityId });
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("published/get")]
        public async Task<IActionResult> GetPublishedPoster([FromQuery] string SearchText)
        {
            var result = await _mediator.Send(new GetPublishedArticleRequest() { SearchText = SearchText });
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
