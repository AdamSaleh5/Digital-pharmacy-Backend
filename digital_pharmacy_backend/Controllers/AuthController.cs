using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Authentication.Dto;
using Application.UseCases.Authentication.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace digital_pharmacy_backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await authService.LoginAsync(request, cancellationToken);
            return response is null
                ? Unauthorized(new { message = AppMessages.InvalidCredentials })
                : Ok(response);
        }
    }
}
