namespace Application.UseCases.Authentication.Dto
{
    public class LoginResponse
    {
        public string AccessToken { get; init; } = string.Empty;

        public string UserType { get; init; } = string.Empty;

        public string name { get; init; } = string.Empty;

        public string AgentName { get; init; } = string.Empty;

        public string PhoneNumber { get; set; }

        public int? CountryId { get; init; } 
    }
}
