using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
	public record AppRoleDTO(
		Guid id, [Required] string RoleName, DateTime? created_at, DateTime? updated_at);
}
