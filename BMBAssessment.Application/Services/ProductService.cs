using AutoMapper;
using BMBAssessment.Application.DTOs.Products;
using BMBAssessment.Application.Exceptions;
using BMBAssessment.Application.Interfaces;
using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _mapper.Map<IReadOnlyCollection<ProductDto>>(
            await _unitOfWork.Products.GetActiveProducts(cancellationToken));
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto request, CancellationToken cancellationToken = default)
    {
        if (request.Variants.Select(x => x.Sku.Trim().ToUpperInvariant()).Distinct().Count() != request.Variants.Count)
            throw new ConflictException("Product variant SKUs must be unique.");

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Type = request.Type,
            Variants = request.Variants.Select(variant => new ProductVariant
            {
                Sku = variant.Sku.Trim(),
                MemorySize = variant.MemorySize?.Trim() ?? string.Empty,
                StorageSize = variant.StorageSize?.Trim() ?? string.Empty,
                Color = variant.Color.Trim(),
                OtherDetails = variant.OtherDetails?.Trim() ?? string.Empty,
                Price = variant.Price,
                Quantity = variant.Quantity
            }).ToList()
        };

        await _unitOfWork.Products.Add(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ProductDto>(product);
    }
}
