using AuthenticationApi.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructrue.Data
{
	public class AuthenticationDbContext(DbContextOptions options) : DbContext(options)
	{
		public DbSet<AppUser> Users { get; set; }
		public DbSet<UserAddress> Address { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<ResetPassword> ResetPassword { get; set; }
	}
}
