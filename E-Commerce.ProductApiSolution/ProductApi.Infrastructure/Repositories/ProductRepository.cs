using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
	public class ProductRepository(ProductDbContext context) : IProduct
	{
		public async Task<Response> CreateAsync(Product entity)
		{
			try
			{
				// check if the product already exist
				var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
				if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
					return new Response(false, $"{entity.Name} already added");

				entity.CreatedAt = DateTime.UtcNow;
				entity.UpdatedAt = DateTime.UtcNow;
				var currentEntity = context.Products.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (currentEntity is not null && currentEntity.Id != Guid.Empty)
					return new Response(true, $"{entity.Name} added to database successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.Name} ");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred adding new product");
			}
		}

		public async Task<Response> DeleteAsync(Product entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.Name} not found");

				context.Products.Remove(product);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.Name} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting product");
			}
		}

		public async Task<Product> FindByIdAsync(Guid id)
		{
			try
			{
				var product = await context.Products
			.Include(p => p.ProductColors)
			.Include(p => p.ProductDiscounts)
			.Include(p => p.productImages)
			.Include(p => p.productReviews)
			.Include(p => p.productSizes)
			.Include(p => p.productTags)
			.FirstOrDefaultAsync(p => p.Id == id);

				return product is not null ? product : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new Exception("Error occurred retieving product");
			}
		}

		public async Task<IEnumerable<Product>> GetAllAsync()
		{
			try
			{
				var products = await context.Products
					.Include(p => p.ProductColors)
					.Include(p => p.ProductDiscounts)
					.Include(p => p.productImages)
					.Include(p => p.productReviews)
					.Include(p => p.productSizes)
					.Include(p => p.productTags)
					.AsNoTracking().ToListAsync();
				return products is not null ? products : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving products");
			}
		}

		public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
		{
			try
			{
				var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
				return product is not null ? product : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving product");
			}
		}

		public async Task<Response> UpdateAsync(Product entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.Name} not found");

				context.Entry(product).State = EntityState.Detached;
				entity.UpdatedAt = DateTime.UtcNow;
				context.Products.Update(entity);
				context.Entry(entity).Property(r => r.CreatedAt).IsModified = false;
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.Name} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing product");
			}
		}
	}
}
