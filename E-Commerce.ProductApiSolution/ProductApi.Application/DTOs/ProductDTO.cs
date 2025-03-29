using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
	public record ProductDTO(Guid Id,
		Guid CategoryId,
		[Required] string Name,
		string? Description,
		[Required, DataType(DataType.Currency)] decimal Price,
		[Required, Range(1, int.MaxValue)] int StockQuantity,
		DateTime? CreatedAt,
		DateTime? UpdatedAt,
		List<ProductColorDTO>? productColors,
		List<ProductDiscountDTO>? ProductDiscounts,
		List<ProductImageDTO>? ProductImages,
		List<ProductReviewDTO>? ProductReviews,
		List<ProductSizeDTO>? ProductSizes,
		List<ProductTagDTO>? ProductTags);
}
