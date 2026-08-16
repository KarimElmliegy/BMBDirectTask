using Microsoft.AspNetCore.Identity;

namespace BMBAssessment.Domain.Entities;

public sealed class ApplicationUser : IdentityUser<int>
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? BannedUntil { get; set; }
    public bool IsBanned => BannedUntil.HasValue && BannedUntil.Value > DateTime.UtcNow;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<OrderDeletion> OrderDeletions { get; set; } = new List<OrderDeletion>();
}
