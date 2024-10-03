using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Handlers.Posters;
using InfoPoster_backend.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Asymmetric")]
    public class AdministrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdministrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("categories/get")]
        public async Task<IActionResult> GetCategories(CategoryType type)
        {
            var result = await _mediator.Send(new GetCategoriesRequest() { type = type });
            return Ok(result);
        }

        [HttpGet("subcategories/get")]
        public async Task<IActionResult> GetSubcategories([FromQuery] GetSubcategoriesRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("poster/available")]
        public async Task<IActionResult> GetAvailablePosters()
        {
            var result = await _mediator.Send(new AdministrationGetPostersRequest());
            return Ok(result);
        }

        [HttpGet("poster/get")]
        public async Task<IActionResult> GetPosterById([FromQuery] Guid id, [FromQuery] string lang)
        {
            var result = await _mediator.Send(new AdministrationGetPosterByIdRequest() { Id = id, Lang = lang });
            return Ok(result);
        }

        [HttpGet("poster/all")]
        public async Task<IActionResult> GetAllPosters()
        {
            var result = await _mediator.Send(new GetAllPostersRequest());
            return Ok(result);
        }

        [HttpPost("poster/create")]
        public async Task<IActionResult> CreatePoster([FromForm] AddPosterRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
            {
                ModelState.AddModelError("Error", "Can't find user");
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost("poster/full-info/save")]
        public async Task<IActionResult> SaveFullInfoPoster([FromForm] SaveFullInfoPosterRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
            {
                ModelState.AddModelError("Error", "Can't find Poster with this Identifier");
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost("poster/delete")]
        public async Task<IActionResult> DeletePoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.DELETED });
            return Ok(result);
        }

        [HttpPost("poster/disable")]
        public async Task<IActionResult> DisablePoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.DISABLED });
            return Ok(result);
        }

        [Authorize(Roles = "4657c003-ab5a-4553-ad0a-7e8d5ec3dbba")]
        [HttpPost("poster/enable")]
        public async Task<IActionResult> EnablePoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.ARCHIVED });
            return Ok(result);
        }

        [HttpPost("poster/active")]
        public async Task<IActionResult> ActivatePoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.ACTIVE });
            return Ok(result);
        }

        [HttpGet("organization/get")]
        public async Task<IActionResult> GetOrganizationById([FromQuery] Guid id, [FromQuery] string lang)
        {
            var result = await _mediator.Send(new GetOrganizationRequest() { Id = id, Lang = lang });
            return Ok(result);
        }

        [HttpGet("organization/all")]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var result = await _mediator.Send(new GetOrganizationListRequest());
            return Ok(result);
        }

        [HttpPost("organization/create")]
        public async Task<IActionResult> CreateOrganization([FromForm] CreateOrganizationRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
            {
                ModelState.AddModelError("Error", "Can't find user");
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost("organization/full-info/save")]
        public async Task<IActionResult> SaveFullInfoOrganization([FromForm] SaveOrganizationRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
            {
                ModelState.AddModelError("Error", "Can't find Organization with this Identifier");
                return BadRequest(ModelState);
            }

            return Ok(result);
        }
    }
}
