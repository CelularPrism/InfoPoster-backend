using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Handlers.Offers;
using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Handlers.Posters;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Models.Posters;
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

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersRequest());
            return Ok(result);
        }

        [HttpPost("users/update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("categories/get")]
        public async Task<IActionResult> GetCategories(CategoryType type)
        {
            var result = await _mediator.Send(new GetCategoriesRequest() { type = type, IsAdmin = true });
            return Ok(result);
        }

        [HttpGet("cities/get")]
        public async Task<IActionResult> GetCities()
        {
            var result = await _mediator.Send(new GetCitiesRequest());
            return Ok(result);
        }

        [HttpGet("subcategories/get")]
        public async Task<IActionResult> GetSubcategories([FromQuery] GetSubcategoriesRequest request)
        {
            request.IsAdmin = true;
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("poster/available")]
        public async Task<IActionResult> GetAvailablePosters(
            [FromQuery] int sort, 
            [FromQuery] Guid? categoryId, 
            [FromQuery] Guid? cityId, 
            [FromQuery] int? status, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new AdministrationGetPostersRequest() { Status = status, StartDate = startDate, EndDate = endDate, CategoryId = categoryId, CityId = cityId, Sort = sort, Page = page - 1, CountPerPage = countPerPage });
            return Ok(result);
        }

        [HttpGet("poster/rejected")]
        public async Task<IActionResult> GetRejectedPosters(
            [FromQuery] int sort,
            [FromQuery] Guid? categoryId,
            [FromQuery] Guid? cityId,
            [FromQuery] int? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new AdministrationGetRejectedPostersRequest() { Status = status, StartDate = startDate, EndDate = endDate, CategoryId = categoryId, CityId = cityId, Sort = sort, Page = page - 1, CountPerPage = countPerPage });
            return Ok(result);
        }

        [HttpGet("poster/get")]
        public async Task<IActionResult> GetPosterById([FromQuery] Guid id, [FromQuery] string lang)
        {
            var result = await _mediator.Send(new AdministrationGetPosterByIdRequest() { Id = id, Lang = lang });
            return Ok(result);
        }

        [HttpGet("poster/all")]
        public async Task<IActionResult> GetAllPosters(
            [FromQuery] int sort, 
            [FromQuery] Guid? categoryId, 
            [FromQuery] Guid? cityId, 
            [FromQuery] int? status, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate, 
            [FromQuery] Guid? editorId,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new GetAllPostersRequest() { Status = status, StartDate = startDate, EndDate = endDate, CategoryId = categoryId, CityId = cityId, Sort = sort, UserId = editorId, Page = page - 1, CountPerPage = countPerPage });
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
            try
            {
                var result = await _mediator.Send(request);
                if (result == null)
                {
                    ModelState.AddModelError("Error", "Can't find Poster with this Identifier");
                    return BadRequest(ModelState);
                }

                return Ok(result);
            } catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("poster/delete")]
        public async Task<IActionResult> DeletePoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.DELETED });
            return Ok(result);
        }

        [HttpPost("poster/reject")]
        public async Task<IActionResult> RejectPoster([FromForm] Guid posterId, [FromForm] string comment)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.REJECTED, Comment = comment });
            return Ok(result);
        }

        [HttpPost("poster/disable")]
        public async Task<IActionResult> DisablePoster([FromForm] Guid posterId, [FromForm] string comment)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.REJECTED, Comment = comment });
            return Ok(result);
        }

        [HttpPost("poster/draft")]
        public async Task<IActionResult> DraftPoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.DRAFT });
            return Ok(result);
        }

        [Authorize(Roles = "4657c003-ab5a-4553-ad0a-7e8d5ec3dbba,c7d65315-0ad4-486f-9bc1-88f86cc1d45b")]
        [HttpPost("poster/enable")]
        public async Task<IActionResult> EnablePoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.PUBLISHED });

            if (result != null && !result.IsSuccess)
            {
                ModelState.AddModelError("Error", result.ErrorMessage);
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost("poster/active")]
        public async Task<IActionResult> ActivatePoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.PENDING });
            return Ok(result);
        }

        [Authorize(Roles = "4657c003-ab5a-4553-ad0a-7e8d5ec3dbba,c7d65315-0ad4-486f-9bc1-88f86cc1d45b")]
        [HttpPost("poster/review")]
        public async Task<IActionResult> ReviewPoster([FromForm] Guid posterId)
        {
            var result = await _mediator.Send(new ChangePosterStatusRequest() { Id = posterId, Status = Models.Posters.POSTER_STATUS.REVIEWING });
            return Ok(result);
        }

        [HttpGet("poster/history/get")]
        public async Task<IActionResult> GetPosterHistory([FromQuery] Guid posterId)
        {
            var result = await _mediator.Send(new GetPosterHistoryRequest() { Id = posterId });
            if (result == null)
                return NoContent();

            return Ok(result);
        }

        [HttpGet("poster/history/fields")]
        public async Task<IActionResult> GetPosterFieldsHistory([FromQuery] Guid articleId)
        {
            var result = await _mediator.Send(new GetPosterChangeHistoryRequest() { Id = articleId });
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetMenuList()
        {
            var result = await _mediator.Send(new GetMenusRequest());
            return Ok(result);
        }

        [HttpGet("organization/get")]
        public async Task<IActionResult> GetOrganizationById([FromQuery] Guid id, [FromQuery] string lang)
        {
            var result = await _mediator.Send(new GetOrganizationRequest() { Id = id, Lang = lang });
            return Ok(result);
        }

        [HttpGet("organization/search")]
        public async Task<IActionResult> SearchOrganizations([FromQuery] string searchText, [FromQuery] Guid cityId)
        {
            var result = await _mediator.Send(new SearchOrganizationRequest() { SearchText = searchText, CityId = cityId });
            return Ok(result);
        }

        [Authorize(Roles = "4657c003-ab5a-4553-ad0a-7e8d5ec3dbba,c7d65315-0ad4-486f-9bc1-88f86cc1d45b")]
        [HttpGet("search/editors")]
        public async Task<IActionResult> SearchEditor([FromQuery] string searchText)
        {
            var result = await _mediator.Send(new SearchEditorRequest() { SearchText = searchText });
            return Ok(result);
        } 

        [HttpGet("organization/all")]
        public async Task<IActionResult> GetAllOrganizations(
            [FromQuery] int sort, 
            [FromQuery] Guid? categoryId, 
            [FromQuery] Guid? cityId, 
            [FromQuery] int? status, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate, 
            [FromQuery] Guid? editorId,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new GetAllOrganizationRequest() { Status = status, StartDate = startDate, EndDate = endDate, CategoryId = categoryId, CityId = cityId, Sort = sort, UserId = editorId, Page = page - 1, CountPerPage = countPerPage });
            return Ok(result);
        }

        [HttpGet("organization/available")]
        public async Task<IActionResult> GetAvailableOrganizations(
            [FromQuery] int sort, 
            [FromQuery] Guid? categoryId, 
            [FromQuery] Guid? cityId, 
            [FromQuery] int? status, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new GetOrganizationListRequest() { Sort = sort, CategoryId = categoryId, CityId = cityId, EndDate = endDate, StartDate = startDate, Status = status, Page = page - 1, CountPerPage = countPerPage });
            return Ok(result);
        }

        [HttpGet("organization/rejected")]
        public async Task<IActionResult> GetRejectedOrganizations(
            [FromQuery] int sort,
            [FromQuery] Guid? categoryId,
            [FromQuery] Guid? cityId,
            [FromQuery] int? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new GetRejectedOrganizationListRequest() { Sort = sort, CategoryId = categoryId, CityId = cityId, EndDate = endDate, StartDate = startDate, Status = status, Page = page - 1, CountPerPage = countPerPage });
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

        [HttpPost("organization/delete")]
        public async Task<IActionResult> DeleteOrganization([FromForm] Guid organizationId)
        {
            var result = await _mediator.Send(new ChangeOrganizationStatusRequest() { Id = organizationId, Status = Models.Posters.POSTER_STATUS.DELETED });
            return Ok(result);
        }

        [HttpPost("organization/draft")]
        public async Task<IActionResult> DraftOrganization([FromForm] Guid organizationId)
        {
            var result = await _mediator.Send(new ChangeOrganizationStatusRequest() { Id = organizationId, Status = Models.Posters.POSTER_STATUS.DRAFT });
            return Ok(result);
        }

        [HttpPost("organization/reject")]
        public async Task<IActionResult> DisableOrganization([FromForm] Guid organizationId, [FromForm] string comment)
        {
            var result = await _mediator.Send(new ChangeOrganizationStatusRequest() { Id = organizationId, Status = Models.Posters.POSTER_STATUS.REJECTED, Comment = comment });
            return Ok(result);
        }

        [HttpPost("organization/disable")]
        public async Task<IActionResult> RejectOrganization([FromForm] Guid organizationId, [FromForm] string comment)
        {
            var result = await _mediator.Send(new ChangeOrganizationStatusRequest() { Id = organizationId, Status = Models.Posters.POSTER_STATUS.REJECTED, Comment = comment });
            return Ok(result);
        }

        [Authorize(Roles = "4657c003-ab5a-4553-ad0a-7e8d5ec3dbba,c7d65315-0ad4-486f-9bc1-88f86cc1d45b")]
        [HttpPost("organization/enable")]
        public async Task<IActionResult> EnableOrganization([FromForm] Guid organizationId)
        {
            var result = await _mediator.Send(new ChangeOrganizationStatusRequest() { Id = organizationId, Status = Models.Posters.POSTER_STATUS.PUBLISHED });

            if (result != null && !result.IsSuccess)
            {
                ModelState.AddModelError("Error", result.ErrorMessage);
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost("organization/active")]
        public async Task<IActionResult> ActivateOrganization([FromForm] Guid organizationId)
        {
            var result = await _mediator.Send(new ChangeOrganizationStatusRequest() { Id = organizationId, Status = Models.Posters.POSTER_STATUS.PENDING });
            return Ok(result);
        }

        [Authorize(Roles = "4657c003-ab5a-4553-ad0a-7e8d5ec3dbba,c7d65315-0ad4-486f-9bc1-88f86cc1d45b")]
        [HttpPost("organization/review")]
        public async Task<IActionResult> ReviewOrganization([FromForm] Guid organizationId)
        {
            var result = await _mediator.Send(new ChangeOrganizationStatusRequest() { Id = organizationId, Status = Models.Posters.POSTER_STATUS.REVIEWING });
            return Ok(result);
        }

        [HttpGet("organization/history/get")]
        public async Task<IActionResult> GetOrganizationHistory([FromQuery] Guid organizationId)
        {
            var result = await _mediator.Send(new GetOrganizationHistoryRequest() { Id = organizationId });
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("organization/history/fields")]
        public async Task<IActionResult> GetOrganizationFieldsHistory([FromQuery] Guid articleId)
        {
            var result = await _mediator.Send(new GetOrganizationChangeHistoryRequest() { Id = articleId });
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("file/upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("file/primary/set")]
        public async Task<IActionResult> FileSetPrimary([FromForm] FileSetPrimaryRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("file/get")]
        public async Task<IActionResult> GetFiles([FromQuery] Guid applicationId, [FromQuery] int place)
        {
            var result = await _mediator.Send(new GetFileRequest() { ApplicationId = applicationId, Place = place });
            return Ok(result);
        }

        [HttpDelete("file/delete")]
        public async Task<IActionResult> DeleteFile([FromQuery] Guid fileId, Guid applicationId)
        {
            var result = await _mediator.Send(new DeleteFileRequest() { FileId = fileId, ApplicationId = applicationId });
            return Ok(result);
        }

        [HttpGet("offer/all")]
        public async Task<IActionResult> GetAllOffers(
            [FromQuery] int sort,
            [FromQuery] OFFER_TYPES? type,
            [FromQuery] Guid? cityId,
            [FromQuery] POSTER_STATUS? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] Guid? editorId,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new GetAllOffersRequest() { Status = status, StartDate = startDate, EndDate = endDate, CityId = cityId, Sort = sort, UserId = editorId, Page = page - 1, CountPerPage = countPerPage, Type = type });
            return Ok(result);
        }

        [HttpGet("offer/available")]
        public async Task<IActionResult> GetAvailableOffers(
            [FromQuery] int sort,
            [FromQuery] OFFER_TYPES? type,
            [FromQuery] Guid? cityId,
            [FromQuery] POSTER_STATUS? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] Guid? editorId,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new GetAvailableOffersRequest() { Status = status, StartDate = startDate, EndDate = endDate, CityId = cityId, Sort = sort, UserId = editorId, Page = page - 1, CountPerPage = countPerPage, Type = type });
            return Ok(result);
        }

        [HttpGet("offer/rejected")]
        public async Task<IActionResult> GetRejectedOffers(
            [FromQuery] int sort,
            [FromQuery] OFFER_TYPES? type,
            [FromQuery] Guid? cityId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] Guid? editorId,
            [FromQuery] int page = 0,
            [FromQuery] int countPerPage = 10)
        {
            var result = await _mediator.Send(new GetRejectedOffersRequest() { StartDate = startDate, EndDate = endDate, CityId = cityId, Sort = sort, UserId = editorId, Page = page - 1, CountPerPage = countPerPage, Type = type });
            return Ok(result);
        }

        [HttpPost("offer/create")]
        public async Task<IActionResult> CreateOffer([FromForm] CreateOfferRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("offer/full-info/save")]
        public async Task<IActionResult> SaveFullInfoOffer([FromForm] SaveFullInfoOfferRequest request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }
    }
}
