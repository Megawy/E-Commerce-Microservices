using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
	public record ProductTagDTO(Guid Id, [Required] Guid ProductId, [Required] string Tag);
}
