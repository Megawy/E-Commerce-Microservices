using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entites;
using AutoMapper;
using E_Commerce.SharedLibray.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class AuthenticationController(IUser userInterface, IMapper mapper) : ControllerBase
	{
		[HttpGet("{id:Guid}")]
		public async Task<ActionResult<GetUserDTO>> GetUser(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Invalid user Id");

			var user = await userInterface.FindUser(id);

			if (user is null) return NotFound($"User id  {id} not Found");

			var GetUserDTOs = mapper.Map<GetUserDTO>(user);

			return GetUserDTOs.Id != Guid.Empty ? Ok(GetUserDTOs) : NotFound($"User id  {id} not Found");
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<GetUserDTO>>> GetUsers()
		{
			var Users = await userInterface.GetAllAsync();
			if (!Users.Any())
				return NotFound("No Users detected in the database");


			var finalUsers = mapper.Map<IEnumerable<GetUserDTO>>(Users);

			return !finalUsers!.Any() ? NotFound() : Ok(finalUsers);
		}

		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<ActionResult<Response>> Register(AppUserDTO appUserDTO)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var createUser = mapper.Map<AppUser>(appUserDTO);
			var result = await userInterface.Resister(createUser);
			return result.Flag ? Ok(result) : BadRequest(result);
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var getEntity = mapper.Map<AppUser>(loginDTO);
			var result = await userInterface.Login(getEntity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}

		[HttpPost("change-password")]
		public async Task<ActionResult<Response>> ChangePassword(ChangePasswordDTO entiy)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await userInterface.ChangePassword(entiy);
			return result.Flag ? Ok(result) : BadRequest(result);
		}

		[HttpPost("reset-password/{token}")]
		[AllowAnonymous]
		public async Task<ActionResult<Response>> ResetPassword(string token, ResetPasswordDTO entity)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var settoken = token;
			var result = await userInterface.ResetPassword(settoken, entity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}


		[HttpPost("forget-password")]
		[AllowAnonymous]
		public async Task<ActionResult<Response>> ForgetPassword(ForgetPasswordDTO entity)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await userInterface.ForgetPassword(entity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}


		[Authorize(Roles = "Admin")]
		[HttpDelete]
		public async Task<ActionResult<Response>> DeleteUser(GetUserDTO entity)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var getEntity = mapper.Map<AppUser>(entity);
			var result = await userInterface.DeleteUser(getEntity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}

		[HttpPut]
		public async Task<ActionResult<Response>> UpdateUser(UpdateUserDTO entity)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var getEntity = mapper.Map<AppUser>(entity);
			var result = await userInterface.UpdateUser(getEntity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}


		[Authorize(Roles = "Admin")]
		[HttpPost("ban")]
		public async Task<ActionResult<Response>> BanUser(BanUserDTO entity)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await userInterface.BanUser(entity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("update-role")]
		public async Task<ActionResult<Response>> UpdateRoleUser(UpdateRoleDTO entity)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await userInterface.UpdateRole(entity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}

		[HttpPost("otp-generate")]
		public async Task<ActionResult<Response>> GenerateOtp(OtpRequestDTO entity)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await userInterface.GenOTPMail(entity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}

		[HttpPost("otp-verify")]
		public async Task<ActionResult<Response>> VerifyeOtp(VerifyOtpDTO entity)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await userInterface.VerOTPMail(entity);
			return result.Flag ? Ok(result) : BadRequest(result);
		}

	}
}
