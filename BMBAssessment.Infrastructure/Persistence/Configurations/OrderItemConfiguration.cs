using BMBAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMBAssessment.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Sku).HasMaxLength(100).IsRequired();
        builder.Property(x => x.MemorySize).HasMaxLength(50).IsRequired();
        builder.Property(x => x.StorageSize).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(50).IsRequired();
        builder.Property(x => x.OtherDetails).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Version).IsRowVersion().IsConcurrencyToken();
        builder.HasIndex(x => new { x.OrderId, x.ProductVariantId }).IsUnique();
        builder.HasQueryFilter(x => x.Order.DeletedAt == null);
    }
}
