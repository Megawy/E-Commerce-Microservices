using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
	public class ProductColorRepository(ProductDbContext context) : IProductColor
	{
		public async Task<Response> CreateAsync(ProductColor entity)
		{
			try
			{
				var currentEntity = context.productColors.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (currentEntity is not null && currentEntity.Id != Guid.Empty)
					return new Response(true, $"{entity.ColorName} added to database successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.ColorName} ");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred adding new ProductColor");
			}
		}

		public async Task<Response> DeleteAsync(ProductColor entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.ColorName} not found");

				context.productColors.Remove(product);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.ColorName} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting product");
			}
		}

		public async Task<ProductColor> FindByIdAsync(Guid id)
		{
			try
			{
				var productColor = await context.productColors.FindAsync(id);
				return productColor is not null ? productColor : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new Exception("Error occurred retieving productColor");
			}
		}

		public async Task<IEnumerable<ProductColor>> GetAllAsync()
		{
			try
			{
				var products = await context.productColors.AsNoTracking().ToListAsync();
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

		public async Task<ProductColor> GetByAsync(Expression<Func<ProductColor, bool>> predicate)
		{
			try
			{
				var product = await context.productColors.Where(predicate).FirstOrDefaultAsync()!;
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

		public async Task<Response> UpdateAsync(ProductColor entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.ColorName} not found");

				context.Entry(product).State = EntityState.Detached;
				context.productColors.Update(entity);
				await context.SaveChangesAsync();

				return new Response(true, $"{entity.ColorName} is updated successfully");
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
