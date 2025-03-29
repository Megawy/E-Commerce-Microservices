using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record VerifyOtpDTO([Required] string Email , [Required] string Otp);
}
