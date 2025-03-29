using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
	public class ProductSizeRepository(ProductDbContext context) : IProductSize
	{
		public async Task<Response> CreateAsync(ProductSize entity)
		{
			try
			{
				// check if the product already exist
				var getProduct = await GetByAsync(_ => _.SizeName!.Equals(entity.SizeName));
				if (getProduct is not null && !string.IsNullOrEmpty(getProduct.SizeName))
					return new Response(false, $"{entity.SizeName} already added");

				var currentEntity = context.ProductSizes.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (currentEntity is not null && currentEntity.Id != Guid.Empty)
					return new Response(true, $"{entity.SizeName} added to database successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.SizeName} ");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred adding new SizeName");

			}
		}

		public async Task<Response> DeleteAsync(ProductSize entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.SizeName} not found");

				context.ProductSizes.Remove(product);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.SizeName} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting SizeName");
			}
		}

		public async Task<ProductSize> FindByIdAsync(Guid id)
		{
			try
			{
				var product = await context.ProductSizes.FindAsync(id);
				return product is not null ? product : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new Exception("Error occurred retieving SizeName");
			}
		}

		public async Task<IEnumerable<ProductSize>> GetAllAsync()
		{
			try
			{
				var products = await context.ProductSizes.AsNoTracking().ToListAsync();
				return products is not null ? products : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving SizeName");
			}
		}

		public async Task<ProductSize> GetByAsync(Expression<Func<ProductSize, bool>> predicate)
		{
			try
			{
				var product = await context.ProductSizes.Where(predicate).FirstOrDefaultAsync()!;
				return product is not null ? product : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving SizeName");
			}
		}

		public async Task<Response> UpdateAsync(ProductSize entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.SizeName} not found");

				context.Entry(product).State = EntityState.Detached;
				context.ProductSizes.Update(entity);
				await context.SaveChangesAsync();

				return new Response(true, $"{entity.SizeName} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing SizeName");
			}
		}
	}
}
