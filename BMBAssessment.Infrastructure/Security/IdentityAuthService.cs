using BMBAssessment.Application.DTOs.Auth;
using BMBAssessment.Application.Exceptions;
using BMBAssessment.Application.Interfaces;
using BMBAssessment.Application.Services;
using BMBAssessment.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BMBAssessment.Infrastructure.Security;

public sealed class IdentityAuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public IdentityAuthService(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var email = NormalizeEmail(request.Email);
        if (await _userManager.FindByEmailAsync(email) is not null)
            throw new ConflictException("A customer with this email already exists.");

        var user = new ApplicationUser
        {
            Name = request.Name.Trim(),
            Email = email,
            UserName = email,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        ThrowIfFailed(createResult);

        var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Customer);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            ThrowIfFailed(roleResult);
        }

        return await CreateResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(NormalizeEmail(request.Email));
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("Invalid email or password.");

        if (user.BannedUntil.HasValue && !user.IsBanned)
        {
            user.BannedUntil = null;
            ThrowIfFailed(await _userManager.UpdateAsync(user));
        }

        return await CreateResponseAsync(user);
    }

    private async Task<AuthResponseDto> CreateResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new AuthResponseDto(
            user.Id,
            user.Name,
            user.Email!,
            _jwtService.GenerateToken(user, roles),
            user.IsBanned ? user.BannedUntil : null);
    }

    private static void ThrowIfFailed(IdentityResult result)
    {
        if (result.Succeeded)
            return;

        if (result.Errors.Any(error =>
                error.Code is "DuplicateEmail" or "DuplicateUserName"))
            throw new ConflictException("A customer with this email already exists.");

        throw new RequestValidationException(string.Join(" ",
            result.Errors.Select(error => error.Description).Distinct()));
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
