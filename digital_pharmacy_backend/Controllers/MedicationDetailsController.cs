using Application.Constants;
using Application.Interfaces;
using Application.Models;
using Application.UseCases.Medications.Dto;
using Application.UseCases.Medications.Request;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace digital_pharmacy_backend.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MedicationDetailsController : ControllerBase
    {
        private readonly IMedicationDetailsService medicationDetailsService;
        private readonly ICurrentUserService currentUserService;

        public MedicationDetailsController(
            IMedicationDetailsService medicationDetailsService,
            ICurrentUserService currentUserService)
        {
            this.medicationDetailsService = medicationDetailsService;
            this.currentUserService = currentUserService;
        }

        [Route("GetMedicationSelect")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MedicationSelectRequest request)
        {
            try
            {
                var data = await this.medicationDetailsService.GetAll(request);
                return Ok(ApiResponse<PaginatedList<MedicationSelectDto>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetMedicationDetails")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int id, [FromQuery] SearchType searchType, [FromQuery] PaginationRequest request)
        {
            try
            {
                var data = await this.medicationDetailsService.GetAllMedication(id, searchType, request);
                return Ok(ApiResponse<PaginatedList<MedicationDetailsDto>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetCompanySeletc")]
        [HttpGet]
        public async Task<IActionResult> GetCompanySeletc([FromQuery] int? CountryId, [FromQuery] PaginationRequest request)
        {
            try
            {
                var data = await this.medicationDetailsService.GetCompanySeletc(CountryId, request);
                return Ok(ApiResponse<PaginatedList<CompanySelectDto>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetMedicationDetailsByCompany")]
        [HttpGet]
        public async Task<IActionResult> GetMedicationDetailsByCompany([FromQuery] int id, [FromQuery] int? CountryId, [FromQuery] PaginationRequest request)
        {
            try
            {
                var data = await this.medicationDetailsService.GetMedicationDetailsByCompany(id, CountryId, request);
                return Ok(ApiResponse<PaginatedList<MedicationDetailsDto>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetAgentSeletc")]
        [HttpGet]
        public async Task<IActionResult> GetAgentSeletc([FromQuery] int? CountryId, [FromQuery] PaginationRequest request)
        {
            try
            {
                var data = await this.medicationDetailsService.GetAgentSeletc(CountryId, request);
                return Ok(ApiResponse<PaginatedList<MedicationSelectDto>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetMedicationDetailsByAgent")]
        [HttpGet]
        public async Task<IActionResult> GetMedicationDetailsByAgent([FromQuery] int id, [FromQuery] int? CountryId, [FromQuery] PaginationRequest request)
        {
            try
            {
                var data = await this.medicationDetailsService.GetMedicationDetailsByAgent(id, CountryId, request);
                return Ok(ApiResponse<PaginatedList<MedicationDetailsDto>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetPharmaceuticalForm")]
        [HttpGet]
        public async Task<IActionResult> GetPharmaceuticalFormById([FromQuery] int id, [FromQuery] PaginationRequest request)
        {
            try
            {
                var data = await this.medicationDetailsService.GetPharmaceuticalFormById(id,request);
                return Ok(ApiResponse<PaginatedList<MedicationDetailsDto>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("MedicationDetails")]
        [HttpPost]
        [Authorize(Roles = "Administrator,Admin,Agent")]
        public async Task<IActionResult> CreateMedicationDetails([FromBody] MedicationDetailsRequest request)
        {
            try
            {
                await medicationDetailsService.CreateMedicationDetails(request);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("MedicationDetails/{id}")]
        [HttpPut]
        [Authorize(Roles = "Administrator,Admin,Agent")]
        public async Task<IActionResult> UpdateMedicationDetails(
            [FromRoute] int id,
            [FromBody] MedicationDetailsRequest request)
        {
            try
            {
                await medicationDetailsService.UpdateMedicationDetails(id, request);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("MedicationDetails/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Administrator,Admin,Agent")]
        public async Task<IActionResult> DeleteMedicationDetails(
            [FromRoute] int id)
        {
            try
            {
                await medicationDetailsService.DeleteMedicationDetails(id);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("MedicationDetails/import")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Administrator,Admin,Agent")]
        public async Task<IActionResult> importMedicationDetails(
            [FromForm] ImportMedicationDetailsForm request)
        {
            try
            {
                var res = await medicationDetailsService.ImportMedicationDetailsAsync(request.File, request.CountryId);
                return Ok(res);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public sealed class ImportMedicationDetailsForm
    {
        public int CountryId { get; set; }
        public IFormFile File { get; set; } = default!;
    }
}
