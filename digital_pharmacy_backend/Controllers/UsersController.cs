using Application.Constants;
using Application.Interfaces;
using Application.Models;
using Application.UseCases.Authentication.Dto;
using Application.UseCases.Authentication.Request;
using Application.UseCases.Pharmacies.Dto;
using Application.UseCases.Pharmacies.Request;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace digital_pharmacy_backend.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IAuthService authService;

        public UsersController(IAuthService authService)
        {
            this.authService = authService;
        }

        [Route("/api/users/verify-device")]
        [HttpPost]
        public async Task<IActionResult> GetUsersCode([FromBody] VerifyDeviceRequest request)
        {
            try
            {
                var res = await this.authService.VerifyDevice(request);
                return Ok(ApiResponse<bool>.Success(res, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("/api/users/{id}/clear-device")]
        [HttpPost]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> ClearDevice([FromRoute] int id)
        {
            try
            {
                await this.authService.ClearDeviceToken(id);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetAllUsers")]
        [HttpGet]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int countryId)
        {
            try
            {
                var res =  this.authService.GetAllUsers(countryId);
                return Ok(ApiResponse<IQueryable<UsersDtos>>.Success(res, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetAllUsers/Agent")]
        [HttpGet]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> GetAllAgent([FromQuery] int countryId)
        {
            try
            {
                var res = this.authService.GetAllUsersAgent(countryId);
                return Ok(ApiResponse<IQueryable<UsersDtos>>.Success(res, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("GetAllUsers/Admin")]
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllAdmin([FromQuery] int countryId)
        {
            try
            {
                var res = this.authService.GetAllUsersAdmin(countryId);
                return Ok(ApiResponse<IQueryable<UsersDtos>>.Success(res, AppMessages.Success));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Users")]
        [HttpPost]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                this.authService.CreateUser(request);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Users/{id}")]
        [HttpPut]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateUserRequest request)
        {
            try
            {
                this.authService.UpdateUsers(id, request);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("Users/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Administrator,Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                this.authService.DeleteUser(id);
                return Ok(ApiResponse<object?>.Success(null, AppMessages.MedicationUpdated));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
