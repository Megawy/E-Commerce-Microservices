namespace ProductApi.Domain.Entities
{
	public class ProductTag
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string? Tag { get; set; }

		// Navigation Property
		public Product? Product { get; set; }
	}

}
