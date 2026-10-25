using Application.UseCases.Authentication.Dto;
using Application.UseCases.Authentication.Request;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

        public Task<bool> VerifyDevice(VerifyDeviceRequest request);

        public Task ClearDeviceToken(int id);

        public IQueryable<UsersDtos> GetAllUsers(int countryId);

        public IQueryable<UsersDtos> GetAllUsersAgent(int countryId);

        public IQueryable<UsersDtos> GetAllUsersAdmin(int countryId);

        public Task CreateUser(CreateUserRequest request);

        public Task UpdateUsers(int id, CreateUserRequest request);

        public Task DeleteUser(int id);
    }
}
