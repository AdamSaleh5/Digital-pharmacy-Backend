using Application.Constants;
using Application.Interfaces;
using Application.UseCases.Authentication.Dto;
using Application.UseCases.Authentication.Request;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using System.Threading;

namespace Application.UseCases.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtTokenGenerator jwtTokenGenerator;
        private readonly ICurrentUserService currentUserService;
        private readonly IGenericRepository genericRepository;

        public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, ICurrentUserService currentUserService, IGenericRepository genericRepository)
        {
            this.userRepository = userRepository;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.currentUserService = currentUserService;
            this.genericRepository = genericRepository;

        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);

            if (user?.Password is null || user.Password != request.Password)
            {
                return null;
            }

            return new LoginResponse
            {
                AccessToken = jwtTokenGenerator.GenerateToken(user),
                UserType = user.Role.ToString(),
                name = user.Name,
                AgentName = user.AgentName,
                CountryId = user.CountryId,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> VerifyDevice(VerifyDeviceRequest request)
        {
            var user = await userRepository.GetByActivationCode(request.ActivationCode);
            if (user == null)
            {
                return false;
            }

            if (request.IsNewUser)
            {
                if(user.DeviceIp != null)
                {
                    return user.DeviceIp == request.DeviceToken;
                }else
                {
                    user.DeviceIp = request.DeviceToken;
                    this.genericRepository.Update<Domain.Entities.User>(user);
                    return true;
                }
                    
            }
            else
            {
                return user.DeviceIp == request.DeviceToken;
            }
        }

        public Task ClearDeviceToken(int id)
        {
            var currentUser = currentUserService.GetCurrentUser();
            var user = this.genericRepository.GetById<Domain.Entities.User>(id);
            if (user == null)
            {
                throw new KeyNotFoundException(AppMessages.InvalidInput);
            }

            if (currentUser.Role == UserRole.Admin)
            {
                if ((user.Role != UserRole.Agent && user.Role != UserRole.User) || user.CountryId != currentUser.CountryId)
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }
            }
            else if (currentUser.Role == UserRole.Administrator)
            {
                if (user.Role != UserRole.Admin && user.Role != UserRole.Agent && user.Role != UserRole.User)
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }
            }
            else
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
            }

            user.DeviceIp = null;
            this.genericRepository.Update<Domain.Entities.User>(user);
            return Task.CompletedTask;
        }

        public IQueryable<UsersDtos> GetAllUsers(int countryId)
        {
            var currentUser = currentUserService.GetCurrentUser();
            var users = this.genericRepository.GetAll<Domain.Entities.User>().Where(u => u.Role == UserRole.User);
            if (currentUser.Role == UserRole.Admin)
            {
                users = users.Where(u => u.CountryId == currentUser.CountryId);
            }
            else
            {
                users = users.Where(u => u.CountryId == countryId);
            }

            return users.Select(u => new UsersDtos
            {
                Id = u.Id,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber,
                ActivationCode = u.ActivationCode,
                CountryId = u.CountryId
            });
        }

        public IQueryable<UsersDtos> GetAllUsersAgent(int countryId)
        {
            var currentUser = currentUserService.GetCurrentUser();
            var users = this.genericRepository.GetAll<Domain.Entities.User>().Where(u => u.Role == UserRole.Agent);
            if (currentUser.Role == UserRole.Admin)
            {
                users = users.Where(u => u.CountryId == currentUser.CountryId);
            }
            else
            {
                users = users.Where(u => u.CountryId == countryId);
            }
            
           return users.Select(u => new UsersDtos
           {
                Id = u.Id,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                AgentName = u.AgentName,
                Password = u.Password,
                CountryId = u.CountryId
            });
        }

        public IQueryable<UsersDtos> GetAllUsersAdmin(int countryId)
        {
            var currentUser = currentUserService.GetCurrentUser();
            var users = this.genericRepository.GetAll<Domain.Entities.User>().Where(u => u.Role == UserRole.Admin);
            if (currentUser.Role != UserRole.Administrator)
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
            }
            else
            {
                users = users.Where(u => u.CountryId == countryId);
            }

            return users.Select(u => new UsersDtos
            {
                Id = u.Id,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                Password = u.Password,
                CountryId = u.CountryId
            });
        }
        public Task CreateUser(CreateUserRequest request)
        {
            var currentUser = currentUserService.GetCurrentUser();

            var user = new Domain.Entities.User();
            if(currentUser.Role == UserRole.Admin)
            {
                if (request.Role == UserRole.Agent )
                {
                    this.ValidateAgentRequest(request);

                        user = new Domain.Entities.User
                        {
                            Name = request.Name,
                            PhoneNumber = request.PhoneNumber,
                            Role = request.Role,
                            Email = NormalizeOptionalEmail(request.Email),
                            Password = request.Password,
                            AgentName = request.AgentName,
                            CountryId = currentUser.CountryId
                        };

                }
                else if (request.Role == UserRole.User)
                {
                    this.ValidateUserRequest(request);

                    user = new Domain.Entities.User
                    {
                        Name = request.Name,
                        PhoneNumber = request.PhoneNumber,
                        Role = request.Role,
                        ActivationCode = request.ActivationCode,
                        CountryId = currentUser.CountryId
                    };
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }
            }
            else if (currentUser.Role == UserRole.Administrator)
            {
                if (request.Role == UserRole.Admin)
                {
                    this.ValidateAdminRequest(request);

                    user = new Domain.Entities.User
                    {
                        Name = request.Name,
                        PhoneNumber = request.PhoneNumber,
                        Role = request.Role,
                        Email = NormalizeOptionalEmail(request.Email),
                        Password = request.Password,
                        CountryId = request.CountryId
                    };
                }
                else if(request.Role == UserRole.Agent)
                {
                    this.ValidateAgentRequest(request);

                    user = new Domain.Entities.User
                    {
                        Name = request.Name,
                        PhoneNumber = request.PhoneNumber,
                        Role = request.Role,
                        Email = NormalizeOptionalEmail(request.Email),
                        Password = request.Password,
                        AgentName = request.AgentName,
                        CountryId = request.CountryId
                    };

                }
                else if (request.Role == UserRole.User)
                {
                    this.ValidateUserRequest(request);

                    user = new Domain.Entities.User
                    {
                        Name = request.Name,
                        PhoneNumber = request.PhoneNumber,
                        Role = request.Role,
                        ActivationCode = request.ActivationCode,
                        CountryId = currentUser.CountryId
                    };
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }
            }
            else
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);

            }

            if (user != null )
            {
                this.genericRepository.Create<Domain.Entities.User>(user);
            }

            return Task.CompletedTask;
        }

        public Task UpdateUsers(int id ,CreateUserRequest request)
        {
            var currentUser = currentUserService.GetCurrentUser();

            var user = this.genericRepository.GetById<Domain.Entities.User>(id);

            if (currentUser.Role == UserRole.Admin)
            {
                if (user.Role == UserRole.Agent)
                {
                    this.ValidateAgentRequest(request);
                    user.Name = request.Name;
                    user.PhoneNumber = request.PhoneNumber;
                    user.Email = NormalizeOptionalEmail(request.Email);
                    user.Password = request.Password;
                    user.AgentName = request.AgentName;
                    user.CountryId = currentUser.CountryId;

                }
                else if (user.Role == UserRole.User)
                {
                    this.ValidateUserRequest(request);

                    user.Name = request.Name;
                    user.PhoneNumber = request.PhoneNumber;
                    user.ActivationCode = request.ActivationCode;
                    user.CountryId = currentUser.CountryId;

                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }
            }
            else if (currentUser.Role == UserRole.Administrator)
            {
                if (user.Role == UserRole.Admin)
                {
                    this.ValidateAdminRequest(request);

                    user.Name = request.Name;
                    user.PhoneNumber = request.PhoneNumber;
                    user.Email = NormalizeOptionalEmail(request.Email);
                    user.Password = request.Password;
                    user. CountryId = request.CountryId;
                }
                else if (request.Role == UserRole.Agent)
                {
                    this.ValidateAgentRequest(request);

                    user.Name = request.Name;
                    user.PhoneNumber = request.PhoneNumber;
                    user.Email = NormalizeOptionalEmail(request.Email);
                    user.Password = request.Password;
                    user.AgentName = request.AgentName;
                    user.CountryId = currentUser.CountryId;

                }
                else if (request.Role == UserRole.User)
                {
                    this.ValidateUserRequest(request);

                    user.Name = request.Name;
                    user.PhoneNumber = request.PhoneNumber;
                    user.ActivationCode = request.ActivationCode;
                    user.CountryId = currentUser.CountryId;
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }
            }
            else
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);

            }

            if (user != null)
            {
                this.genericRepository.Update<Domain.Entities.User>(user);
            }

            return Task.CompletedTask;
        }

        public Task DeleteUser(int id)
        {
            var currentUser = currentUserService.GetCurrentUser();
            var user = this.genericRepository.GetById<Domain.Entities.User>(id);
            if (currentUser.Role == UserRole.Admin)
            {
                if ((user.Role == UserRole.Agent || user.Role == UserRole.User) && user.CountryId == currentUser.CountryId )
                {
                    this.genericRepository.Delete<Domain.Entities.User>(user.Id);
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }
            }
            else if (currentUser.Role == UserRole.Administrator)
            {
                if (user.Role == UserRole.Admin || user.Role == UserRole.Agent || user.Role == UserRole.User)
                {
                    this.genericRepository.Delete<Domain.Entities.User>(user.Id);
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }
            }
            else
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
            }
            return Task.CompletedTask;
        }

        private void ValidateAgentRequest(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.AgentName))
            {
                throw new ArgumentException("Agent name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password is required.");
            }
        }

        private void ValidateUserRequest(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ActivationCode))
            {
                throw new ArgumentException("Activation Code is required.");
            }
        }

        private void ValidateAdminRequest(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password is required.");
            }

            if (request.CountryId is null)
            {
                throw new ArgumentException("Country ID must be empty for an agent.");
            }
        }

        private static string? NormalizeOptionalEmail(string? email) =>
            string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }
}
