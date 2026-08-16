using BMBAssessment.Application.DTOs.Products;
using BMBAssessment.Application.Services;
using BMBAssessment.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMBAssessment.API.Controllers;

[ApiController, Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyCollection<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto request, CancellationToken cancellationToken)
    {
        return Ok(await _productService.CreateAsync(request, cancellationToken));
    }
}
