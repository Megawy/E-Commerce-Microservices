namespace AuthenticationApi.Application.DTOs
{
	public record GetUserDTO(Guid Id, Guid? RoleID, string? RoleName, string? Email,
											string? FirstName, string? LastName, string? TelephoneNumber,
												DateTime? DateRegistered, bool? IsBanned, bool? IsVerfiyMail);
}

