using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
	public record UserAddressDTO
	(Guid Id,
		Guid UserId,
		[Required] string FullName,
		[Required] string street,
		[Required] string apartment,
		[Required] string city,
		[Required] string country,
		[Required] string phone_number,
		string? floor,
		string? building,
		 string? state,
		 string? postal_code,
		DateTime? created_at,
		DateTime? updated_at);
}
