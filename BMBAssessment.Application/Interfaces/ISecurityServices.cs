using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Application.Interfaces;
public interface IJwtService { string GenerateToken(ApplicationUser user, IEnumerable<string> roles); }
public interface ICurrentUserService { int CustomerId { get; } bool IsAuthenticated { get; } }
