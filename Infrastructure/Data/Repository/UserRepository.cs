using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email != null && user.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public Task<User?> GetByActivationCode(string activationCode)
        {
            return dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Role == UserRole.User && user.ActivationCode == activationCode);
        }
    }
}
