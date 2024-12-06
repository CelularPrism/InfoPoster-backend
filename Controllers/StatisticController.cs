﻿using InfoPoster_backend.Handlers.Statistics;
using InfoPoster_backend.Models.Posters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize(AuthenticationSchemes = "Asymmetric")]
    public class StatisticController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatisticController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetPublished([FromQuery] DateTime dateStart, [FromQuery] DateTime dateEnd, [FromQuery] Guid? userId)
        {
            var result = await _mediator.Send(new GetPublishedRequest() { DateStart = dateStart, DateEnd = dateEnd, UserId = userId, Status = (int)POSTER_STATUS.PUBLISHED });
            return Ok(result);
        }

        [HttpGet("draft")]
        public async Task<IActionResult> GetCreated([FromQuery] DateTime dateStart, [FromQuery] DateTime dateEnd, [FromQuery] Guid? userId)
        {
            var result = await _mediator.Send(new GetPublishedRequest() { DateStart = dateStart, DateEnd = dateEnd, UserId = userId, Status = (int)POSTER_STATUS.DRAFT });
            return Ok(result);
        }

        [HttpGet("common")]
        public async Task<IActionResult> GetCommonStat([FromQuery] DateTime dateStart, [FromQuery] DateTime dateEnd)
        {
            var result = await _mediator.Send(new GetCommonStatRequest() { DateStart = dateStart, DateEnd = dateEnd });
            return Ok(result);
        }

        [HttpGet("best")]
        public async Task<IActionResult> GetBestEditorStat([FromQuery] DateTime dateStart, [FromQuery] DateTime dateEnd)
        {
            var result = await _mediator.Send(new GetBestEditorStatRequest() { DateStart = dateStart, DateEnd = dateEnd });
            return Ok(result);
        }
    }
}