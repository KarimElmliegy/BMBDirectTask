namespace BMBAssessment.Application.DTOs.Customers;
public sealed record CustomerDto(int Id, string Name, string Email, DateTime CreatedAt, DateTime? BannedUntil);
