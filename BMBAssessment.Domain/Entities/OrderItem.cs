namespace BMBAssessment.Domain.Entities;

public sealed class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string MemorySize { get; set; } = string.Empty;
    public string StorageSize { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string OtherDetails { get; set; } = string.Empty;
    public byte[] Version { get; set; } = Array.Empty<byte>();
}
