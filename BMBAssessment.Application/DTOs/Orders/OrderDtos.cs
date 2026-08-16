using BMBAssessment.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace BMBAssessment.Application.DTOs.Orders;
public sealed record CreateOrderItemDto(
    [param: Range(1, int.MaxValue)] int ProductVariantId,
    [param: Range(1, 5)] int Quantity);

public sealed record UpdateOrderItemDto(
    [param: Range(1, 5)] int Quantity,
    [param: Required] string Version);

public sealed record OrderItemDto(
    int Id,
    int ProductVariantId,
    int Quantity,
    decimal UnitPrice,
    string ProductName,
    string Sku,
    string MemorySize,
    string StorageSize,
    string Color,
    string OtherDetails,
    string Version);

public sealed record CreateOrderDto(
    [param: Required, StringLength(1000)] string Description,
    [param: Required, MinLength(1)] IReadOnlyCollection<CreateOrderItemDto> Items);

public sealed record OrderDto(int Id, int CustomerId, DateTime CreatedAt, string Description, OrderStatus Status,
    IReadOnlyCollection<OrderItemDto> Items);

public sealed record DeleteOrderResultDto(DateTime? BannedUntil);
