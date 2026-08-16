using System.ComponentModel.DataAnnotations;

namespace BMBAssessment.Application.DTOs.Auth;
public sealed record RegisterRequestDto(
    [param: Required, StringLength(100)] string Name,
    [param: Required, EmailAddress, StringLength(320)] string Email,
    [param: Required, StringLength(128, MinimumLength = 8)] string Password);

public sealed record LoginRequestDto(
    [param: Required, EmailAddress] string Email,
    [param: Required] string Password);

public sealed record AuthResponseDto(int CustomerId, string Name, string Email, string Token, DateTime? BannedUntil);
