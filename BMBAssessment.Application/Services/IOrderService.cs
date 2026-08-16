using BMBAssessment.Application.DTOs.Orders;

namespace BMBAssessment.Application.Services;
public interface IOrderService
{
    Task<IReadOnlyCollection<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderDto request, CancellationToken cancellationToken = default);
    Task<DeleteOrderResultDto> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderItemDto> UpdateItemAsync(int orderId, int itemId, UpdateOrderItemDto request, CancellationToken cancellationToken = default);
}
