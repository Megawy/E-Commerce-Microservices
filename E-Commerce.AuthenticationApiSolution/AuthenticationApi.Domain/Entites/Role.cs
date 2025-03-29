namespace AuthenticationApi.Domain.Entites
{
	public class Role
	{
		public Guid Id { get; set; }
		public string? RoleName { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }

		// Many To One
		public ICollection<AppUser>? RoleUsers { get; set; }
	}
}
