using AutoMapper;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.Customer;
using Dipterv.Shared.Dto.Location;
using Dipterv.Shared.Dto.Order;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ProductCategory;
using Dipterv.Shared.Dto.ProductInventory;
using Dipterv.Shared.Dto.ProductPhoto;
using Dipterv.Shared.Dto.ShoppingCart;
using Dipterv.Shared.Dto.SpecialOffer;

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

            CreateMap<ProductDto, ProductDetailsDto>();

            CreateMap<ProductDto, ListProductDto>();

            CreateMap<ProductDto, ProductWithSpecialOffersDto>();

            CreateMap<UpdateProductCommand, Product>();

            CreateMap<AddProductReviewCommand, ProductReview>();

            CreateMap<UpdateProductReviewCommand, ProductReview>();

            CreateMap<ProductReview, ProductReviewDto>();

            CreateMap<SpecialOffer, SpecialOfferDto>()
                .ForMember(dest => dest.ValidForProductIds, opt => opt.MapFrom(src => src.SpecialOfferProducts.Select(sop => sop.ProductId).ToList()));

            CreateMap<AddSpecialOfferCommand, SpecialOffer>();

            CreateMap<AddProductInventoryCommand, ProductInventory>()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<EditProductInventoryCommand, ProductInventory>()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<Location, LocationDto>();

            CreateMap<ProductInventory, ProductInventoryDto>();

            CreateMap<Customer, CustomerDto>();

            CreateMap<SalesOrderHeader, OrderHeaderDto>();

            CreateMap<ShoppingCartItem, ShoppingCartItemDto>();


            CreateMap<ShoppingCartItemDto, ShoppingCartItemDetailsDto>();


            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductPhotoIds, opt => opt.MapFrom(src => src.ProductProductPhotos.Select(ppp => ppp.ProductPhotoId).ToList()))
                .ForMember(dest => dest.PrimaryProductPhotoId, opt => opt.MapFrom(src =>
                    src.ProductProductPhotos.FirstOrDefault(p => p.Primary) != null 
                        ? (int?) src.ProductProductPhotos.FirstOrDefault(p => p.Primary).ProductPhotoId
                        : null)
                    )
                .ForMember(dest => dest.ProductReviewIds, opt => opt.MapFrom(src => src.ProductReviews.Select(pr => pr.ProductReviewId).ToList()))
                .ForMember(dest => dest.ShoppingCartItemIds, opt => opt.MapFrom(src => src.ShoppingCartItems.Select(sci => sci.ShoppingCartItemId).ToList()))
                .ForMember(dest => dest.SpecialOfferIds, opt => opt.MapFrom(src => src.SpecialOfferProducts.Select(sop => sop.SpecialOfferId).ToList()))
                .ForMember(dest => dest.ProductCategoryId, opt => opt.MapFrom(src => src.ProductSubcategoryId.HasValue ? (int?) src.ProductSubcategory.ProductCategoryId : null))
                .ForMember(dest => dest.ProductSubcategoryId, opt => opt.MapFrom(src => src.ProductSubcategoryId));

            CreateMap<ProductPhoto, ProductPhotoDto>();

            CreateMap<ProductCategory, ProductCategoryDto>()
                .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.ProductSubcategories));

            CreateMap<ProductSubcategory, ProductSubcategoryDto>();
        }
    }
}
