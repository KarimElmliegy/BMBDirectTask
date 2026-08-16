using BMBAssessment.Application.DTOs.Orders;
using BMBAssessment.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMBAssessment.API.Controllers;

[ApiController, Authorize, Route("api/customers/me")]
public sealed class CustomersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public CustomersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpGet("orders")]
    public async Task<ActionResult<IReadOnlyCollection<OrderDto>>> GetOrders(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllAsync(cancellationToken);
        return Ok(result);
    }
}
