namespace ProductApi.Domain.Entities
{
	public class ProductReview
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public Guid CustomerId { get; set; }
		public int Rating { get; set; }
		public string? ReviewText { get; set; }
		public DateTime CreatedAt { get; set; }
		// Navigation Properties
		public Product? Product { get; set; }
	}

}
