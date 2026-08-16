namespace BMBAssessment.Domain.Entities;

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProductType Type { get; set; } = ProductType.Phone;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}

public enum ProductType
{
    Phone,
    Other
}
