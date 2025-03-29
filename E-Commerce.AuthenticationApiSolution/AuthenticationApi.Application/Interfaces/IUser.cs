using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entites;
using E_Commerce.SharedLibray.Responses;
namespace AuthenticationApi.Application.Interfaces
{
	public interface IUser
	{
		Task<AppUser> FindUser(Guid id);
		Task<IEnumerable<AppUser>> GetAllAsync();
		Task<Response> Resister(AppUser entity);
		Task<Response> Login(AppUser entity);
		Task<Response> ChangePassword(ChangePasswordDTO changePasswordDTO);
		Task<Response> ForgetPassword(ForgetPasswordDTO entity);
		Task<Response> ResetPassword(string token, ResetPasswordDTO entity);
		Task<Response> DeleteUser(AppUser entity);
		Task<Response> UpdateUser(AppUser entity);
		Task<Response> BanUser(BanUserDTO entity);
		Task<Response> UpdateRole(UpdateRoleDTO entity);
		Task<Response> GenOTPMail(OtpRequestDTO entity);
		Task<Response> VerOTPMail(VerifyOtpDTO entity);
	}
}
