using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
namespace ProductApi.Infrastructure.Data
{
	public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
	{
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductColor> productColors { get; set; }
		public DbSet<ProductDiscount> ProductDiscounts { get; set; }
		public DbSet<ProductImage> ProductImages { get; set; }
		public DbSet<ProductReview> ProductReviews { get; set; }
		public DbSet<ProductSize> ProductSizes { get; set; }
		public DbSet<ProductTag> ProductTags { get; set; }
	}
}
