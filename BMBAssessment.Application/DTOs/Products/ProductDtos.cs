using System.ComponentModel.DataAnnotations;
using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Application.DTOs.Products;

public sealed record CreateProductVariantDto(
    [param: Required, StringLength(100)] string Sku,
    [param: StringLength(50)] string MemorySize,
    [param: StringLength(50)] string StorageSize,
    [param: Required, StringLength(50)] string Color,
    [param: StringLength(1000)] string OtherDetails,
    [param: Range(typeof(decimal), "0.01", "999999999999.99")] decimal Price,
    [param: Range(0, int.MaxValue)] int Quantity);

public sealed record CreateProductDto(
    [param: Required, StringLength(200)] string Name,
    [param: StringLength(2000)] string Description,
    ProductType Type,
    [param: Required, MinLength(1)] IReadOnlyCollection<CreateProductVariantDto> Variants);

public sealed record ProductVariantDto(int Id, string Sku, string MemorySize, string StorageSize,
    string Color, string OtherDetails, decimal Price, int Quantity, bool IsActive);

public sealed record ProductDto(int Id, string Name, string Description, ProductType Type,
    bool IsActive, IReadOnlyCollection<ProductVariantDto> Variants);
