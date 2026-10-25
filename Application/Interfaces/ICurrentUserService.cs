using Application.UseCases.Authentication.Dto;

namespace Application.Interfaces
{
    public interface ICurrentUserService
    {
        CurrentUser GetCurrentUser();
    }
}
