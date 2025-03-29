using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
	public record ProductDiscountDTO(Guid Id,
		[Required] Guid ProductId,
		[Required] decimal DiscountPercentage,
		[Required] DateTime StartDate,
		[Required] DateTime EndDate);
}
