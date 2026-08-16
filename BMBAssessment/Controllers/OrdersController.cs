using BMBAssessment.Application.DTOs.Orders;
using BMBAssessment.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMBAssessment.API.Controllers;

[ApiController, Authorize, Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create( CreateOrderDto request, CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateAsync(request, cancellationToken);
        return Ok(order);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<DeleteOrderResultDto>> Delete(int id, CancellationToken cancellationToken)
    {
        return Ok(await _orderService.DeleteAsync(id, cancellationToken));
    }

    [HttpPatch("{orderId:int}/items/{itemId:int}")]
    public async Task<ActionResult<OrderItemDto>> UpdateItem(int orderId, int itemId, UpdateOrderItemDto request, CancellationToken cancellationToken)
    {
        return Ok(await _orderService.UpdateItemAsync(orderId, itemId, request, cancellationToken));
    }
}
