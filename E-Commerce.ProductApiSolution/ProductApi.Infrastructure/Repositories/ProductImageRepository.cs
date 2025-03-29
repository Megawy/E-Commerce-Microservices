using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
	public class ProductImageRepository(ProductDbContext context) : IProductImage
	{
		public async Task<Response> CreateAsync(ProductImage entity)
		{
			try
			{
				entity.UploadedAt = DateTime.UtcNow;
				var currentEntity = context.ProductImages.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (currentEntity is not null && currentEntity.Id != Guid.Empty)
					return new Response(true, $"{entity.ImageUrl} added to database successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.ImageUrl} ");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred adding new ImageUrl");
			}
		}

		public async Task<Response> DeleteAsync(ProductImage entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.ImageUrl} not found");

				context.ProductImages.Remove(product);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.ImageUrl} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting ImageUrl");
			}
		}

		public async Task<ProductImage> FindByIdAsync(Guid id)
		{
			try
			{
				var productColor = await context.ProductImages.FindAsync(id);
				return productColor is not null ? productColor : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new Exception("Error occurred retieving ProductImages");
			}
		}

		public async Task<IEnumerable<ProductImage>> GetAllAsync()
		{
			try
			{
				var products = await context.ProductImages.AsNoTracking().ToListAsync();
				return products is not null ? products : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving ProductImages");
			}
		}

		public async Task<ProductImage> GetByAsync(Expression<Func<ProductImage, bool>> predicate)
		{
			try
			{
				var product = await context.ProductImages.Where(predicate).FirstOrDefaultAsync()!;
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

		public async Task<Response> UpdateAsync(ProductImage entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.ImageUrl} not found");

				context.Entry(product).State = EntityState.Detached;
				entity.UploadedAt = DateTime.UtcNow;
				context.ProductImages.Update(entity);
				await context.SaveChangesAsync();

				return new Response(true, $"{entity.ImageUrl} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing ImageUrl");
			}
		}
	}
}
