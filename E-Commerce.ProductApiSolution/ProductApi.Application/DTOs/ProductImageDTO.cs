using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
	public record ProductImageDTO(Guid Id, [Required] Guid ProductId, [Required] string ImageUrl, bool? IsMain, DateTime? UploadedAt);
}
