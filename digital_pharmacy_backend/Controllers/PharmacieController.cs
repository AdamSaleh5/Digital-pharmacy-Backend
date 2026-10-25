using Application.Constants;
using Application.Interfaces;
using Application.Models;
using Application.UseCases.Medications.Dto;
using Application.UseCases.Medications.Request;
using Application.UseCases.Pharmacies.Dto;
using Application.UseCases.Pharmacies.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace digital_pharmacy_backend.Controllers
{
    [Route("api/")]
    [ApiController]
    public class PharmacieController : ControllerBase
    {
        private readonly IPharmaciesService pharmaciesService;
        private readonly ICurrentUserService currentUserService;

        public PharmacieController(
            IPharmaciesService pharmaciesService,
            ICurrentUserService currentUserService)
        {
            this.pharmaciesService = pharmaciesService;
            this.currentUserService = currentUserService;
        }

        [Route("GetPharmacies")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PharmaciesSelectRequest request)
        {
            try
            {
                var data = await this.pharmaciesService.GetAll(request);
                return Ok(ApiResponse<PaginatedList<PharmaciesDto>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Pharmacies")]
        [HttpPost]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> Create([FromBody] PharmaciesRequest request)
        {
            try
            {
                await this.pharmaciesService.CreatePharmacie(request);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Pharmacies/{id}")]
        [HttpPut]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PharmaciesRequest request)
        {
            try
            {
                await this.pharmaciesService.UpdatePharmacie(id,request);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Pharmacies/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await this.pharmaciesService.DeletePharmacie(id);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Pharmacies/import")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> ImportPharmacies([FromForm] ImportMedicationDetailsForms request)
        {
            var result = await pharmaciesService.ImportPharmaciesAsync(request.File, request.CountryId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}

public sealed class ImportMedicationDetailsForms
{
    public int CountryId { get; set; }
    public IFormFile File { get; set; } = default!;
}
