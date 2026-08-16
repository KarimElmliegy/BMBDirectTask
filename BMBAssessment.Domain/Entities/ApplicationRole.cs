using Microsoft.AspNetCore.Identity;

namespace BMBAssessment.Domain.Entities;

public sealed class ApplicationRole : IdentityRole<int>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}

public static class RoleNames
{
    public const string Customer = "Customer";
    public const string Admin = "Admin";
}
