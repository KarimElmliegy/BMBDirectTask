using System.Security.Cryptography;
using BMBAssessment.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BMBAssessment.Infrastructure.Security;

public sealed class LegacyCompatiblePasswordHasher : IPasswordHasher<ApplicationUser>
{
    private const string LegacyPrefix = "PBKDF2-SHA256";
    private readonly PasswordHasher<ApplicationUser> _identityHasher;

    public LegacyCompatiblePasswordHasher(IOptions<PasswordHasherOptions> options)
    {
        _identityHasher = new PasswordHasher<ApplicationUser>(options);
    }

    public string HashPassword(ApplicationUser user, string password) =>
        _identityHasher.HashPassword(user, password);

    public PasswordVerificationResult VerifyHashedPassword(
        ApplicationUser user,
        string hashedPassword,
        string providedPassword)
    {
        if (!hashedPassword.StartsWith($"{LegacyPrefix}:", StringComparison.Ordinal))
            return _identityHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);

        return VerifyLegacyHash(providedPassword, hashedPassword)
            ? PasswordVerificationResult.SuccessRehashNeeded
            : PasswordVerificationResult.Failed;
    }

    private static bool VerifyLegacyHash(string password, string passwordHash)
    {
        var parts = passwordHash.Split(':');
        if (parts.Length != 4 || parts[0] != LegacyPrefix ||
            !int.TryParse(parts[1], out var iterations) || iterations <= 0)
            return false;

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
