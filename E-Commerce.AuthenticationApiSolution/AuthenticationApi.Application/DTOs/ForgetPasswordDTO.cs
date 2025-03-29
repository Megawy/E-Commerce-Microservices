using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
	public record ForgetPasswordDTO([Required, EmailAddress] string Email);
}
