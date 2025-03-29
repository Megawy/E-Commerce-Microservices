using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entites;
using AuthenticationApi.Infrastructrue.Data;
using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_Commerce.SharedLibray.Services;
using Hangfire;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using File = System.IO.File;
using AuthenticationApi.Application.DTOs;



namespace AuthenticationApi.Infrastructrue.Repositories
{
	public class UserRepository(AuthenticationDbContext context, IConfiguration config, IEmailService emailService, IMemoryCache cache, IMapper mapper) : IUser
	{
		public async Task<IEnumerable<AppUser>> GetAllAsync()
		{
			try
			{
				var Users = await context.Users
					.Include(u => u.Role)
					.AsNoTracking()
					.ToListAsync();
				return Users is not null ? Users : null!;
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while retrieving Users");
			}
		}

		public async Task<AppUser> FindUser(Guid id)
		{
			try
			{
				var getUser = await context.Users
					.Include(r => r.Role)
					.FirstOrDefaultAsync(u => u.Id == id);

				return getUser is not null ? getUser : null!;
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while retrieving user");
			}
		}

		public async Task<Response> Resister(AppUser entity)
		{
			try
			{
				var getUser = await GetUserByEmail(entity.Email!.ToUpper());
				if (getUser is not null) return new Response(false, $"you cannot use this email {entity.Email} for registration");

				var getNum = await GetUserByNum(entity.TelephoneNumber!);
				if (getNum is not null) return new Response(false, $"you cannot use this TelephoneNumber {entity.TelephoneNumber} for registration");

				var role = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Customer");
				if (role is null) return new Response(false, "Role not found");

				entity.Password = BCrypt.Net.BCrypt.HashPassword(entity.Password);
				entity.RoleID = role.Id;
				entity.DateRegistered = DateTime.UtcNow;

				var result = await context.Users.AddAsync(entity);
				await context.SaveChangesAsync();

				await GenOTPMail(new OtpRequestDTO(entity.Email));

				// EmailBody
				string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/WelcomeEmail.html");
				if (!File.Exists(templatePath))
					return new Response(false, "Email template not found");

				string htmlTemplate = await File.ReadAllTextAsync(templatePath);
				// Replace token placeholder with actual reset link
				string emailBody = htmlTemplate
				.Replace("{{USERNAME}}", entity.Email)
				.Replace("{{LOGIN_URL}}", $"{config["WebLink:URL"]}/api/authentication/login");
				// Send email
				BackgroundJob.Enqueue(() => emailService.SendEmailAsync(entity.Email!, "Welcome", emailBody));

				return result.Entity.Id != Guid.Empty ?
					new Response(true, "User registered successfully") :
					new Response(false, "Invalid data provided");
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while Create User");
			}
		}
		private async Task<AppUser> GetUserByEmail(string email)
		{
			var user = await context.Users.FirstOrDefaultAsync(x => x.Email!.ToUpper() == email.ToUpper());
			return user is null ? null! : user!;
		}
		private async Task<AppUser> GetUserByNum(string Num)
		{
			var user = await context.Users.FirstOrDefaultAsync(x => x.TelephoneNumber!.ToUpper() == Num.ToUpper());
			return user is null ? null! : user!;
		}

		public async Task<Response> Login(AppUser entity)
		{
			try
			{
				var getUser = await GetUserByEmail(entity.Email!.ToUpper());
				if (getUser is null)
					return new Response(false, $"Invalid credentials");

				if (getUser.IsBanned)
					return new Response(false, "Your account has been banned.");

				bool verifyPassword = BCrypt.Net.BCrypt.Verify(entity.Password, getUser.Password);
				if (!verifyPassword)
					return new Response(false, $"Invalid credentials");

				string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/LoginAlert.html");
				if (!File.Exists(templatePath))
					return new Response(false, "Email template not found");

				string htmlTemplate = await File.ReadAllTextAsync(templatePath);
				string emailBody = htmlTemplate
				.Replace("{{UserEmail}}", getUser.Email!.ToString())
				.Replace("{{LoginTime}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
				.Replace("{{ResetPasswordLink}}", $"{config["WebLink:URL"]}/api/authentication/forget-password");
				//.Replace("{{UserIP}}", userIp)

				BackgroundJob.Enqueue(() => emailService.SendEmailAsync(getUser.Email!, "LoginAlert", emailBody));

				string token = await GenerateToken(getUser);
				return new Response(true, token);
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while login");
			}
		}
		private async Task<string> GenerateToken(AppUser user)
		{
			var key = Encoding.UTF8.GetBytes(config.GetValue<string>("Authentication:Key")!);
			var securityKey = new SymmetricSecurityKey(key);
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var userRole = await context.Roles.FindAsync(user.RoleID);

			var claims = new List<Claim>
	{
		 new (ClaimTypes.Email, user.Email!),
		 new (ClaimTypes.NameIdentifier, user.Id.ToString()),
		 new(ClaimTypes.Role, userRole!.RoleName!)
	};
			var token = new JwtSecurityToken(
				issuer: config["Authentication:Issuer"],
				audience: config["Authentication:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<Response> ChangePassword(ChangePasswordDTO entity)
		{
			try
			{
				var findUser = await context.Users.FindAsync(entity.Id);
				if (findUser is null)
					return new Response(false, $"User {entity.Id} not found");

				if (findUser.IsBanned)
					return new Response(false, "Your account has been banned.");

				var isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(entity.OldPassword, findUser.Password);
				if (!isOldPasswordCorrect)
					return new Response(false, "Old password is incorrect");

				bool isSamePassword = BCrypt.Net.BCrypt.Verify(entity.NewPassword, findUser.Password);
				if (isSamePassword)
					return new Response(false, "The same password cannot be repeated.");

				findUser.Password = BCrypt.Net.BCrypt.HashPassword(entity.NewPassword);

				context.Users.Update(findUser);
				await context.SaveChangesAsync();

				string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/ChangePasswordAlert.html");
				if (!File.Exists(templatePath))
					return new Response(false, "Email template not found");

				string htmlTemplate = await File.ReadAllTextAsync(templatePath);
				string emailBody = htmlTemplate
				.Replace("{{UserEmail}}", findUser.Email!.ToString())
				.Replace("{{ChangeTime}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
				.Replace("{{ResetPasswordLink}}", $"{config["WebLink:URL"]}/api/authentication/forget-password");

				BackgroundJob.Enqueue(() => emailService.SendEmailAsync(findUser.Email!, "ChangePasswordAlert", emailBody));

				return new Response(true, "Password updated successfully");
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				return new Response(false, "An error occurred while updating the password");
			}
		}

		public async Task<Response> ResetPassword(string token, ResetPasswordDTO entity)
		{
			try
			{
				var geToken = await context.ResetPassword
			.FirstOrDefaultAsync(t => t.ResetToken == token);

				if (geToken is null)
					return new Response(false, "Invalid token.");

				if (geToken.IsUsed)
					return new Response(false, "This token has already been used.");

				if (geToken.created_at.GetValueOrDefault().AddHours(1) < DateTime.UtcNow)
					return new Response(false, "This token has expired.");

				var getUser = await context.Users.FindAsync(geToken.UserId);

				if (getUser is null) return new Response(false, "user not found");

				if (getUser.IsBanned) return new Response(false, "Your account has been banned.");

				bool isSamePassword = BCrypt.Net.BCrypt.Verify(entity.password, getUser.Password);
				if (isSamePassword)
					return new Response(false, "The same password cannot be repeated.");

				getUser.Password = BCrypt.Net.BCrypt.HashPassword(entity.password);

				geToken.IsUsed = true;

				context.Users.Update(getUser);
				context.ResetPassword.Update(geToken);
				await context.SaveChangesAsync();

				string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/ChangePasswordAlert.html");
				if (!File.Exists(templatePath))
					return new Response(false, "Email template not found");

				string htmlTemplate = await File.ReadAllTextAsync(templatePath);
				string emailBody = htmlTemplate
				.Replace("{{UserEmail}}", getUser.Email!.ToString())
				.Replace("{{ChangeTime}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
				.Replace("{{ResetPasswordLink}}", $"{config["WebLink:URL"]}/api/authentication/forget-password");

				BackgroundJob.Enqueue(() => emailService.SendEmailAsync(getUser.Email!, "ChangePasswordAlert", emailBody));

				return new Response(true, "Password has been reset successfully.");
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				return new Response(false, "An error occurred while resetting the password.");
			}
		}

		public async Task<Response> ForgetPassword(ForgetPasswordDTO entity)
		{
			try
			{
				var getUser = await GetUserByEmail(entity.Email.ToUpper());

				if (getUser is null) return new Response(false, "Email not valid");

				if (getUser.IsBanned) return new Response(false, "Your account has been banned.");

				// Generate reset token
				var token = GenerateResetToken(getUser.Id.ToString(), getUser.Email!);

				// Create reset link
				var resetLink = $"{config["WebLink:URL"]}/api/authentication/reset-password/{Uri.EscapeDataString(token)}";

				var tokendb = mapper.Map<ResetPassword>(entity);
				tokendb.ResetToken = token;
				tokendb.UserId = getUser.Id;
				context.ResetPassword.Add(tokendb);
				await context.SaveChangesAsync();

				// EmailBody
				string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/ForgetPassword.html");
				if (!File.Exists(templatePath))
					return new Response(false, "Email template not found");

				string htmlTemplate = await File.ReadAllTextAsync(templatePath);
				// Replace token placeholder with actual reset link
				string emailBody = htmlTemplate
				.Replace("{{reset_link}}", resetLink)
				.Replace("{{entity_ID}}", getUser.Id.ToString());
				// Send email
				BackgroundJob.Enqueue(() => emailService.SendEmailAsync(getUser.Email!, "Password Reset", emailBody));

				return new Response(true, "Password reset link sent.");
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while cannot Create reset password token");
			}
		}
		private string GenerateResetToken(string userId, string email)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(config["Authentication:Key"]!);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
				new Claim("userId", userId),
				new Claim("email", email),
				new Claim("type", "password_reset")
			}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public async Task<Response> DeleteUser(AppUser entity)
		{
			try
			{
				var getUser = await context.Users.FindAsync(entity.Id);

				if (getUser is null)
					return new Response(false, "user not found");

				context.Users.Remove(getUser);
				await context.SaveChangesAsync();
				return new Response(true, "User successfully deleted");
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				return new Response(false, "Error Occured while deleting user");
			}
		}

		public async Task<Response> UpdateUser(AppUser entity)
		{
			try
			{
				var getUser = await FindUser(entity.Id);
				if (getUser is null)
					return new Response(false, $"User {entity.Id} not found");
				if (getUser.Email != entity.Email)
				{
					var exitmail = await GetUserByEmail(entity.Email!.ToUpper());
					if (exitmail is not null) return new Response(false, $"you cannot use this email {entity.Email}");
				}
				if (getUser.TelephoneNumber != entity.TelephoneNumber)
				{
					var getNum = await GetUserByNum(entity.TelephoneNumber!);
					if (getNum is not null) return new Response(false, $"you cannot use this TelephoneNumber {entity.TelephoneNumber}");
				}

				if (getUser.Email != entity.Email) _ = getUser.IsVerfiyMail = false;
				context.Entry(getUser).CurrentValues.SetValues(entity);
				context.Entry(getUser).Property(r => r.Password).IsModified = false;
				context.Entry(getUser).Property(r => r.RoleID).IsModified = false;
				await context.SaveChangesAsync();


				string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/UpdateUser.html");
				if (!File.Exists(templatePath))
					return new Response(false, "Email template not found");

				string htmlTemplate = await File.ReadAllTextAsync(templatePath);


				string emailBody = htmlTemplate
					.Replace("{{UserName}}", getUser.FirstName)
					.Replace("{{UserEmail}}", getUser.Email)
					.Replace("{{UpdateTime}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
					.Replace("{{SupportLink}}", "https://yourwebsite.com/support");

				// Send email in the background
				BackgroundJob.Enqueue(() => emailService.SendEmailAsync(getUser.Email!, "Profile Update Notification", emailBody));

				return new Response(true, "User updated successfully.");
			}
			catch (Exception ex)
			{
				// Log error
				LogException.LogExceptions(ex);
				return new Response(false, "Error occurred while updating user.");
			}
		}

		public async Task<Response> BanUser(BanUserDTO entity)
		{
			try
			{
				var getUser = await FindUser(entity.Id);
				if (getUser is null)
					return new Response(false, $"User {entity.Id} not found");

				getUser.IsBanned = entity.banStatus;
				await context.SaveChangesAsync();


				if (string.IsNullOrEmpty(getUser.Email))
					return new Response(false, "User email not found");

				// EmailBody
				string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/BanUserMail.html");
				if (!File.Exists(templatePath))
					return new Response(false, "Email template not found");

				string htmlTemplate = await File.ReadAllTextAsync(templatePath);
				string emailBody = htmlTemplate.Replace("{{ban_status}}", entity.banStatus ? "Banned" : "Unbanned");

				BackgroundJob.Enqueue(() => emailService.SendEmailAsync(getUser.Email!, "Account Status Update", emailBody));

				string message = entity.banStatus ? "User has been banned." : "User has been unbanned.";
				return new Response(true, message);
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				return new Response(false, "An error occurred while updating the user ban status.");
			}
		}

		public async Task<Response> UpdateRole(UpdateRoleDTO entity)
		{
			try
			{
				var getUser = await FindUser(entity.Id);
				if (getUser is null) return new Response(false, $"User id {entity.Id} not found");

				var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == entity.RoleId);
				if (role is null) return new Response(false, $"Role {entity.RoleId} not found");

				context.Entry(getUser).State = EntityState.Detached;
				getUser.RoleID = role.Id;
				context.Users.Update(getUser);
				await context.SaveChangesAsync();

				if (role.RoleName == "Admin")
				{
					// EmailBody
					string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/AdminRoleAssigned.html");
					if (!File.Exists(templatePath))
						return new Response(false, "Email template not found");

					string htmlTemplate = await File.ReadAllTextAsync(templatePath);
					// Replace token placeholder with actual reset link
					string emailBody = htmlTemplate
					.Replace("{{UserEmail}}", getUser.Email)
					.Replace("{{AssignedTime}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
					.Replace("{{AdminDashboardLink}}", "https://yourwebsite.com/admin-dashboard");
					// Send email
					BackgroundJob.Enqueue(() => emailService.SendEmailAsync(getUser.Email!, "AdminRoleAssigned", emailBody));
				}

				return new Response(true, $"User update role ({role.RoleName})");
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				return new Response(false, "Error Occured while updateing role user");
			}
		}

		public async Task<Response> GenOTPMail(OtpRequestDTO entity)
		{
			try
			{
				var otp = new Random().Next(100000, 999999).ToString();

				cache.Set(entity.Email, otp, TimeSpan.FromMinutes(5));

				string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/GenOTPMail.html");
				if (!File.Exists(templatePath))
					return new Response(false, "Email template not found");

				string htmlTemplate = await File.ReadAllTextAsync(templatePath);
				string emailBody = htmlTemplate.Replace("{{OTP_CODE}}", otp);

				BackgroundJob.Enqueue(() => emailService.SendEmailAsync(entity.Email, "Your OTP Code", emailBody));

				return new Response(true, "OTP sent successfully.");
			}
			catch (Exception ex)
			{
				// Log the original exception
				LogException.LogExceptions(ex);

				// Return a user-friendly error message
				return new Response(false, "Error occurred while generating OTP.");
			}
		}

		public async Task<Response> VerOTPMail(VerifyOtpDTO entity)
		{
			try
			{
				if (cache.TryGetValue(entity.Email, out string storedOtp) && storedOtp == entity.Otp)
				{
					cache.Remove(entity.Email);

					var getmail = await GetUserByEmail(entity.Email);

					getmail.IsVerfiyMail = true;
					await context.SaveChangesAsync();

					string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "../AuthenticationApi.Application/Templates/VerOTPMail.html");
					if (!File.Exists(templatePath))
						return new Response(false, "Email template not found");

					string htmlTemplate = await File.ReadAllTextAsync(templatePath);

					BackgroundJob.Enqueue(() => emailService.SendEmailAsync(entity.Email, "OTP verified successfully", htmlTemplate));
					return new Response(true, "OTP verified successfully.");
				}
				return new Response(true, "Invalid or expired OTP.");
			}
			catch (Exception ex)
			{
				// Log the original exception
				LogException.LogExceptions(ex);

				// Return a user-friendly error message
				return new Response(false, "Error occurred while Invalid or expired OTP");
			}
		}
	}
}