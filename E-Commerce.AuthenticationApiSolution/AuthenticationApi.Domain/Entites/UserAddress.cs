namespace AuthenticationApi.Domain.Entites
{
	public class UserAddress
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string? FullName { get; set; }
		public string? street { get; set; }
		public string? building { get; set; }
		public string? floor { get; set; }
		public string? apartment { get; set; }
		public string? city { get; set; }
		public string? state { get; set; }
		public string? country { get; set; }
		public string? phone_number { get; set; }
		public string? postal_code { get; set; }
		public DateTime? created_at { get; set; } = DateTime.UtcNow;
		public DateTime? updated_at { get; set; } = DateTime.UtcNow;

		// One To Many 
		public AppUser? User { get; set; }
	}
}
