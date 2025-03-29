namespace ProductApi.Domain.Entities
{
	public class ProductSize
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string? SizeName { get; set; }
		public int? StockQuantity { get; set; }

		// Navigation Property
		public Product? Product { get; set; }
	}

}
