namespace AuthenticationApi.Domain.Entites
{
	public class ResetPassword
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string? ResetToken { get; set; }
		public bool IsUsed { get; set; }
		public DateTime? created_at { get; set; } = DateTime.UtcNow;

		// One To Many 
		public AppUser? Users { get; set; }
	}
}
