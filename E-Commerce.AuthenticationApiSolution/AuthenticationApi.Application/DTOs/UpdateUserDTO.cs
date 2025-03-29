using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
	public record UpdateUserDTO(
		Guid Id,
		[Required] string FirstName,
		[Required] string LastName,
		[Required] string TelephoneNumber,
		[Required, EmailAddress] string Email
		);
}
