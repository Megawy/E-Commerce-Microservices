namespace ProductApi.Domain.Entities
{
	public class ProductImage
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string? ImageUrl { get; set; }
		public bool IsMain { get; set; } = false;
		public DateTime UploadedAt { get; set; }

		// Navigation Property
		public Product? Product { get; set; }
	}
}
