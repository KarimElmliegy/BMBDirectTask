namespace BMBAssessment.Domain.Entities;

public sealed class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string Sku { get; set; } = string.Empty;
    public string MemorySize { get; set; } = string.Empty;
    public string StorageSize { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string OtherDetails { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] Version { get; set; } = Array.Empty<byte>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
