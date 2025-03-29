using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
	public record class OtpRequestDTO([Required] string Email);
}
