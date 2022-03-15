using AutoMapper;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.Customer;
using Dipterv.Shared.Dto.Location;
using Dipterv.Shared.Dto.Order;
using Dipterv.Shared.Dto.ProductInventory;
using Dipterv.Shared.Dto.SpecialOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Bll.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddProductCommand, Product>()
                .ForMember(dest => dest.SafetyStockLevel, opt => opt.MapFrom(_ => 10))
                .ForMember(dest => dest.ReorderPoint, opt => opt.MapFrom(_ => 20))
                .ForMember(dest => dest.SellStartDate, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<Product, ProductDto>();
            //.ForMember(dest => dest.LocationIds, opt => opt.MapFrom(src => src.ProductInventories.Select(p => p.LocationId).ToHashSet()))
            //.ForMember(dest => dest.ProductReviewIds, opt => opt.MapFrom(src => src.ProductReviews.Select(p => p.ProductReviewId).ToHashSet()));

            CreateMap<ProductDto, ProductWithReviewsDto>();

            CreateMap<UpdateProductCommand, Product>();

            CreateMap<AddProductReviewCommand, ProductReview>();

            CreateMap<UpdateProductReviewCommand, ProductReview>();

            CreateMap<ProductReview, ProductReviewDto>();

            CreateMap<SpecialOffer, SpecialOfferDto>();

            CreateMap<AddSpecialOfferCommand, SpecialOffer>();

            CreateMap<ProductInventory, ProductInventoryDto>();

            CreateMap<AddProductInventoryCommand, ProductInventory>()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<EditProductInventoryCommand, ProductInventory>()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<Location, LocationDto>();

            CreateMap<ProductInventory, ProductInventoryDto>();

            CreateMap<Customer, CustomerDto>();

            CreateMap<SalesOrderHeader, OrderHeaderDto>();
        }
    }
}
