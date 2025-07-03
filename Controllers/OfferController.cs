using InfoPoster_backend.Handlers.Offers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OfferController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetOfferList()
        {
            var result = await _mediator.Send(new GetOfferListRequest());
            return Ok(result);
        }

        [HttpGet("full-info")]
        public async Task<IActionResult> GetOfferFullInfo([FromQuery] Guid id)
        {
            var result = await _mediator.Send(new GetOfferFullInfoRequest() { Id = id });
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
