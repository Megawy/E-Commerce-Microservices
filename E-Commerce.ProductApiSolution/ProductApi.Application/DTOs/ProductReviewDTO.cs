using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
	public record ProductReviewDTO(Guid Id, [Required] Guid ProductId, [Required] Guid CustomerId, [Required] int Rating, string? ReviewText, DateTime? CreatedAt);
}
