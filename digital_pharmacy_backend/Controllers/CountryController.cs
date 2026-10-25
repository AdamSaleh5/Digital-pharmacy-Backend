using Application.Constants;
using Application.Interfaces;
using Application.Models;
using Application.UseCases.Medications.Dto;
using Application.UseCases.Medications.Request;
using Application.UseCases.Pharmacies.Request;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace digital_pharmacy_backend.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService countryService;

        public CountryController(ICountryService countryService)
        {
            this.countryService = countryService;
        }

        [Route("GetCountries")]
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = this.countryService.GetAllCountries();
                return Ok(ApiResponse<IQueryable<Country>>.Success(data, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Country")]
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] Country request)
        {
            try
            {
                await this.countryService.CreateCountry(request);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Country/{id}")]
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update([FromRoute] int id , [FromBody] Country request)
        {
            try
            {
                await this.countryService.UpdateCountry(id,request);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Country/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await this.countryService.DeleteCountry(id);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
