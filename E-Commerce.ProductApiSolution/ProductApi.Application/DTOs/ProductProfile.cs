using AutoMapper;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs
{
	class ProductProfile : Profile
	{
		public ProductProfile()
		{
			CreateMap<Product, ProductDTO>().ReverseMap();

			CreateMap<ProductColor, ProductColorDTO>().ReverseMap();

			CreateMap<ProductDiscount, ProductDiscountDTO>().ReverseMap();

			CreateMap<ProductImage, ProductImageDTO>().ReverseMap();

			CreateMap<ProductReview, ProductReviewDTO>().ReverseMap();

			CreateMap<ProductSize, ProductSizeDTO>().ReverseMap();

			CreateMap<ProductTag, ProductTagDTO>().ReverseMap();
		}
	}
}
