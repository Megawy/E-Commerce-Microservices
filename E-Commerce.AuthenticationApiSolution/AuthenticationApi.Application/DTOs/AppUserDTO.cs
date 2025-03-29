using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
	public record AppUserDTO
	(
		Guid Id,
		[Required] Guid RoleId,
		[Required] string FirstName,
		[Required] string LastName,
		[Required] string TelephoneNumber,
		[Required, EmailAddress] string Email,
		[Required, RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$",
		ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one digit.")] string Password);
}