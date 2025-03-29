using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
	public record CategoryDTO(Guid Id, [Required] string Name, string? Description,
		DateTime CreatedAt,
		DateTime UpdatedAt,
		List<ProductDTO>? Products);
}
