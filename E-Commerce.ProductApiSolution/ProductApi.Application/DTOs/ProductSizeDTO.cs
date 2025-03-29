using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
	public record ProductSizeDTO(Guid Id, [Required] Guid ProductId, [Required] string SizeName, int? StockQuantity);
}
