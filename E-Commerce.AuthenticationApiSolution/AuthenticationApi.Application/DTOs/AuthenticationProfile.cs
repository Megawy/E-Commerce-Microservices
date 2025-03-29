using AuthenticationApi.Domain.Entites;
using AutoMapper;

namespace AuthenticationApi.Application.DTOs
{
	public class AuthenticationProfile : Profile
	{
		public AuthenticationProfile()
		{
			CreateMap<AppUser, GetUserDTO>()
			.ConstructUsing(src => new GetUserDTO(
				src.Id,
				src.Role != null ? src.Role.Id : (Guid?)null,
				src.Role != null ? src.Role.RoleName : null,
				src.FirstName,
				src.LastName,
				src.TelephoneNumber,
				src.Email,
				src.DateRegistered,
				src.IsBanned,
				src.IsVerfiyMail
			))
			.ReverseMap();

			CreateMap<AppUser, AppUserDTO>().ReverseMap();

			CreateMap<AppUser, LoginDTO>().ReverseMap();

			CreateMap<ResetPassword, ForgetPasswordDTO>().ReverseMap();

			CreateMap<AppUser, UpdateUserDTO>().ReverseMap();

			CreateMap<UserAddress, UserAddressDTO>().ReverseMap();

			CreateMap<Role, AppRoleDTO>().ReverseMap();
		}
	}
}
