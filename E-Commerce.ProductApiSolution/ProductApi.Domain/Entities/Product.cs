namespace ProductApi.Domain.Entities
{
	public class Product
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid CategoryId { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
		public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }

		// Many To One
		public ICollection<ProductColor>? ProductColors { get; set; }
		public ICollection<ProductDiscount>? ProductDiscounts { get; set; }
		public ICollection<ProductImage>? productImages { get; set; }
		public ICollection<ProductReview>? productReviews { get; set; }
		public ICollection<ProductSize>? productSizes { get; set; }
		public ICollection<ProductTag>? productTags { get; set; }

		// One To Many 
		public Category? Category { get; set; }
	}
}
