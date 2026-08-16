namespace BMBAssessment.Domain.Entities;

public sealed class OrderDeletion
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public ApplicationUser Customer { get; set; } = null!;
    public DateTime OrderCreatedAt { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
}
