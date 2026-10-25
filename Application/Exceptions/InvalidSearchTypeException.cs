using Application.Constants;

namespace Application.Exceptions;

public sealed class InvalidSearchTypeException : ArgumentException
{
    public InvalidSearchTypeException()
        : base(AppMessages.InvalidSearchType)
    {
    }
}
