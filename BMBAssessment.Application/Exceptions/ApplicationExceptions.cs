namespace BMBAssessment.Application.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) 
    { 
    }
}
public sealed class CustomerBannedException : Exception
{
    public CustomerBannedException(DateTime bannedUntil) : base($"Customer is banned until {bannedUntil:u}.")
    {
        BannedUntil = bannedUntil;
    }
    public DateTime BannedUntil { get; }
}
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
public sealed class RequestValidationException : Exception
{
    public RequestValidationException(string message) : base(message)
    {
    }
}
