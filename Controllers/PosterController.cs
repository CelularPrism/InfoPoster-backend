﻿using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Handlers.Posters;
using InfoPoster_backend.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PosterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosters([FromQuery] GetPostersRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("by-category")]
        public async Task<IActionResult> GetPostersByCategory([FromQuery] GetPostersByCategoryRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("by-subcategory")]
        public async Task<IActionResult> GetPostersBySubcategory([FromQuery] GetPostersBySubcategoryRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("full-info")]
        public async Task<IActionResult> GetFullInfoPoster([FromQuery] GetFullInfoPosterRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("cities/get")]
        public async Task<IActionResult> GetCities()
        {
            var result = await _mediator.Send(new GetCitiesRequest());
            return Ok(result);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming()
        {
            var result = await _mediator.Send(new GetUpcomingPostersRequest());
            return Ok(result);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular()
        {
            var result = await _mediator.Send(new GetPopularPostersRequest());
            return Ok(result);
        }

        [HttpGet("popular/by-subcategory")]
        public async Task<IActionResult> GetPopularBySubcategory([FromQuery] Guid subcategoryId)
        {
            var response = await _mediator.Send(new GetPostersBySubcategoryRequest() { Limit = 9, Offset = 0, startDate = DateTime.UtcNow, endDate = DateTime.UtcNow.AddYears(1), subcategoryId = subcategoryId });
            return Ok(response.data);
        }

        [HttpGet("recently-added")]
        public async Task<IActionResult> GetRecentlyAdded()
        {
            var result = await _mediator.Send(new GetRecentlyAddedPostersRequest());
            return Ok(result);
        }
    }
}
