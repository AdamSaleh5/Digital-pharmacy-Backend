using Application.Interfaces;
using Application.UseCases.Authentication.Dto;
using Domain.Enums;
using System.Security.Claims;

namespace digital_pharmacy_backend.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public CurrentUser GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return new CurrentUser();
            }

            var role = Enum.TryParse<UserRole>(user.FindFirstValue(ClaimTypes.Role), out var parsedRole)
                ? parsedRole
                : UserRole.User;

            var countryId = int.TryParse(user.FindFirstValue("country_id"), out var parsedCountryId)
                ? parsedCountryId
                : (int?)null;

            return new CurrentUser
            {
                Id = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
                Name = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                Email = user.FindFirstValue(ClaimTypes.Email),
                Role = role,
                CountryId = countryId,
                AgentName = user.FindFirstValue("agent_name"),
                PhoneNumber = user.FindFirstValue("phone_number")
            };
        }
    }
}
