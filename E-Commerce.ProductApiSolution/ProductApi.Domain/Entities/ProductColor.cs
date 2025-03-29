namespace ProductApi.Domain.Entities
{
	public class ProductColor
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string? ColorName { get; set; }
		public string? HexCode { get; set; }

		// Navigation Property
		public Product? Product { get; set; }
	}

}
