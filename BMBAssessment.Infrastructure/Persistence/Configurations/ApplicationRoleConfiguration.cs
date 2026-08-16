using BMBAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMBAssessment.Infrastructure.Persistence.Configurations;

public sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
            new ApplicationRole(RoleNames.Customer)
            {
                Id = 1,
                NormalizedName = RoleNames.Customer.ToUpperInvariant(),
                ConcurrencyStamp = "f1dfd850-754c-4b8a-a0c7-0c938fd3d8ef"
            },
            new ApplicationRole(RoleNames.Admin)
            {
                Id = 2,
                NormalizedName = RoleNames.Admin.ToUpperInvariant(),
                ConcurrencyStamp = "2636af86-dfed-4941-a623-100246dc955a"
            });
    }
}
