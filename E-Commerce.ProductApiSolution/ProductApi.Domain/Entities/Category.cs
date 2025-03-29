﻿namespace ProductApi.Domain.Entities
{
	public class Category
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string? Name { get; set; }
		public string? Description { get; set; }
		public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }

		// Many To One
		public ICollection<Product>? Products { get; set; }
	}
}
