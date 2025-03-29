using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
	public record UpdateRoleDTO([Required] Guid Id, [Required] Guid RoleId);
}
