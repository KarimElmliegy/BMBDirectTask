using BMBAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMBAssessment.Infrastructure.Persistence.Configurations;
public sealed class OrderDeletionConfiguration : IEntityTypeConfiguration<OrderDeletion>
{
    public void Configure(EntityTypeBuilder<OrderDeletion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.OrderCreatedAt).IsRequired();
        builder.Property(x => x.DeletedAt).IsRequired();
        builder.HasOne(x => x.Customer).WithMany(x => x.OrderDeletions).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CustomerId, x.DeletedAt });
    }
}
