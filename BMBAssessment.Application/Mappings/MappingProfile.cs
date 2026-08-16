using AutoMapper;
using BMBAssessment.Application.DTOs.Customers;
using BMBAssessment.Application.DTOs.Orders;
using BMBAssessment.Application.DTOs.Products;
using BMBAssessment.Domain.Entities;

namespace BMBAssessment.Application.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, CustomerDto>().MaxDepth(1);
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>()
            .ForCtorParam(nameof(OrderItemDto.Version), options => options.MapFrom(item => Convert.ToBase64String(item.Version)));
        CreateMap<Product, ProductDto>();
        CreateMap<ProductVariant, ProductVariantDto>();
    }
}
