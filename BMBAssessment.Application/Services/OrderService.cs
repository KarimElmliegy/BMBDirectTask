using AutoMapper;
using BMBAssessment.Application.DTOs.Orders;
using BMBAssessment.Application.Exceptions;
using BMBAssessment.Application.Interfaces;
using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Application.Services;
public sealed class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var orders = await _unitOfWork.Orders.GetByCustomerId(customer.Id, cancellationToken);
        return _mapper.Map<IReadOnlyCollection<OrderDto>>(orders);
    }

    public async Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        return _mapper.Map<OrderDto>(await GetOwnedOrderAsync(id, customer.Id, cancellationToken));
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto request, CancellationToken cancellationToken = default)
    {
        var variantIds = request.Items.Select(x => x.ProductVariantId).ToArray();
        if (variantIds.Distinct().Count() != variantIds.Length)
            throw new ConflictException("A product variant can only appear once in an order.");

        var variants = await _unitOfWork.Products.GetActiveVariants(variantIds, cancellationToken);
        if (variants.Count != variantIds.Length)
            throw new NotFoundException("One or more product variants were not found or are inactive.");

        var customer = await GetCurrentCustomerAsync(cancellationToken);
        if (customer.IsBanned) throw new CustomerBannedException(customer.BannedUntil!.Value);
        var variantsById = variants.ToDictionary(x => x.Id);
        foreach (var requestItem in request.Items)
        {
            var variant = variantsById[requestItem.ProductVariantId];
            if (requestItem.Quantity > variant.Quantity)
                throw new ConflictException($"Only {variant.Quantity} unit(s) of {variant.Product.Name} ({variant.Color}) are available.");
        }

        foreach (var requestItem in request.Items)
            variantsById[requestItem.ProductVariantId].Quantity -= requestItem.Quantity;

        var order = new Order
        {
            CustomerId = customer.Id,
            Description = request.Description.Trim(),
            CreatedAt = DateTime.UtcNow,
            Items = request.Items.Select(item =>
            {
                var variant = variantsById[item.ProductVariantId];
                return new OrderItem
                {
                    ProductVariantId = variant.Id,
                    Quantity = item.Quantity,
                    UnitPrice = variant.Price,
                    ProductName = variant.Product.Name,
                    Sku = variant.Sku,
                    MemorySize = variant.MemorySize,
                    StorageSize = variant.StorageSize,
                    Color = variant.Color,
                    OtherDetails = variant.OtherDetails
                };
            }).ToList()
        };
        await _unitOfWork.Orders.Add(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<DeleteOrderResultDto> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var order = await GetOwnedOrderAsync(id, customer.Id, cancellationToken);
        var variants = await _unitOfWork.Products.GetVariants(order.Items.Select(item => item.ProductVariantId), cancellationToken);
        var variantsById = variants.ToDictionary(variant => variant.Id);
        foreach (var item in order.Items)
        {
            if (variantsById.TryGetValue(item.ProductVariantId, out var variant))
                variant.Quantity += item.Quantity;
        }
        var deletedAt = DateTime.UtcNow;
        var deletion = new OrderDeletion
        {
            OrderId = order.Id,
            CustomerId = customer.Id,
            OrderCreatedAt = order.CreatedAt,
            DeletedAt = deletedAt
        };
        await _unitOfWork.OrderDeletions.Add(deletion, cancellationToken);

        if (order.CreatedAt.Date == deletedAt.Date)
        {
            var priorCount = await _unitOfWork.OrderDeletions.CountCustomerDeletionsOnDateAsync(customer.Id, deletedAt.Date, cancellationToken);
            if (priorCount + 1 >= 3)
            {
                customer.BannedUntil = deletedAt.AddHours(6);
                _unitOfWork.Customers.Update(customer);
            }
        }

        order.DeletedAt = deletedAt;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new DeleteOrderResultDto(customer.IsBanned ? customer.BannedUntil : null);
    }

    public async Task<OrderItemDto> UpdateItemAsync(int orderId, int itemId, UpdateOrderItemDto request, CancellationToken cancellationToken = default)
    {
        var customer = await GetCurrentCustomerAsync(cancellationToken);
        var item = await _unitOfWork.Orders.GetCustomerOrderItem(orderId, itemId, customer.Id, cancellationToken)
            ?? throw new NotFoundException($"Order item {itemId} was not found.");

        var quantityDifference = request.Quantity - item.Quantity;
        if (quantityDifference > item.ProductVariant.Quantity)
            throw new ConflictException($"Only {item.ProductVariant.Quantity} additional unit(s) are available.");

        byte[] version;
        try
        {
            version = Convert.FromBase64String(request.Version);
        }
        catch (FormatException)
        {
            throw new RequestValidationException("The order item version is invalid.");
        }

        _unitOfWork.Orders.SetOriginalVersion(item, version);
        item.ProductVariant.Quantity -= quantityDifference;
        item.Quantity = request.Quantity;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrderItemDto>(item);
    }

    private async Task<ApplicationUser> GetCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated) throw new UnauthorizedException("Authentication is required.");
        var customer = await _unitOfWork.Customers.GetById(_currentUser.CustomerId, cancellationToken)
            ?? throw new UnauthorizedException("Authenticated customer no longer exists.");
        return customer;
    }

    private async Task<Order> GetOwnedOrderAsync(int id, int customerId, CancellationToken cancellationToken) =>
        await _unitOfWork.Orders.GetCustomerOrder(id, customerId, cancellationToken)
        ?? throw new NotFoundException($"Order {id} was not found.");
}
