using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
	public record class ProductColorDTO(Guid Id, [Required] Guid ProductId, [Required] string ColorName, [Required] string HexCode);
}
