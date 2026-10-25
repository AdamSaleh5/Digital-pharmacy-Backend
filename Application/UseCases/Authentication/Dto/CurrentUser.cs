using Domain.Enums;

namespace Application.UseCases.Authentication.Dto
{
    public class CurrentUser
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Email { get; init; }

        public string? Password { get; init; }

        public string? ActivationCode { get; init; }

        public UserRole Role { get; init; } = UserRole.User;

        public int? CountryId { get; init; }

        public string? AgentName { get; init; }

        public string? PhoneNumber { get; init; }
    }
}
