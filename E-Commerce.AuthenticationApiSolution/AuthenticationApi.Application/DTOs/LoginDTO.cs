using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
	public record LoginDTO([Required] string Email, [Required] string Password);
}
