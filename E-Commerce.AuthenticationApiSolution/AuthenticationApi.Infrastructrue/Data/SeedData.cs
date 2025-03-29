using AuthenticationApi.Domain.Entites;
using AuthenticationApi.Infrastructrue.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Infrastructure.Data
{
	public class SeedData
	{
		private readonly AuthenticationDbContext _dbContext;
		private readonly IConfiguration _config;

		public SeedData(AuthenticationDbContext dbContext, IConfiguration config)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_config = config ?? throw new ArgumentNullException(nameof(config));
		}

		public async Task SeedRolesAsync()
		{
			await _dbContext.Database.MigrateAsync();

			var roles = new List<Role>
			{
				new Role { RoleName = "Admin", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow },
				new Role { RoleName = "Customer", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow }
			};

			var existingRoles = await _dbContext.Roles.Select(r => r.RoleName).ToListAsync();
			var newRoles = roles.Where(r => !existingRoles.Contains(r.RoleName)).ToList();

			if (newRoles.Any())
			{
				await _dbContext.Roles.AddRangeAsync(newRoles);
				await _dbContext.SaveChangesAsync();
			}

			string adminEmail = _config["UserAdmin:Email"]!;
			if (string.IsNullOrEmpty(adminEmail))
			{
				throw new Exception("Admin email is not configured.");
			}

			var existingAdmin = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
			if (existingAdmin == null)
			{
				var adminRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
				if (adminRole != null)
				{
					var adminUser = new AppUser
					{
						Email = adminEmail,
						DateRegistered = DateTime.UtcNow,
						Password = BCrypt.Net.BCrypt.HashPassword(_config["UserAdmin:Password"]),
						RoleID = adminRole.Id
					};

					await _dbContext.Users.AddAsync(adminUser);
					await _dbContext.SaveChangesAsync();
				}
			}
		}
	}
}
