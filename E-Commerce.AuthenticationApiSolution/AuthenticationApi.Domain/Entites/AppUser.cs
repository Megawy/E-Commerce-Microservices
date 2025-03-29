namespace AuthenticationApi.Domain.Entites
{
	public class AppUser
	{
		public Guid Id { get; set; }
		public Guid? RoleID { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? TelephoneNumber { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public bool IsBanned { get; set; } = false;
		public bool IsVerfiyMail { get; set; } = false;
		public DateTime? DateRegistered { get; set; } = DateTime.UtcNow;

		// Many To one
		public ICollection<UserAddress>? UserAddresses { get; set; }
		public ICollection<ResetPassword>? ResetPassword { get; set; }
		// One To Many 
		public Role? Role { get; set; }
	}
}
