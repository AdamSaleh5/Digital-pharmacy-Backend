using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        public Task<User?> GetByActivationCode(string activationCode);
    }
}
