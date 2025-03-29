namespace ProductApi.Domain.Entities
{
	public class ProductDiscount
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public decimal DiscountPercentage { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		// Navigation Property
		public Product? Product { get; set; }
	}

}
