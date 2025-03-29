using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
	public class ProductDiscountRepository(ProductDbContext context) : IProductDiscount
	{
		public async Task<Response> CreateAsync(ProductDiscount entity)
		{
			try
			{
				var currentEntity = context.ProductDiscounts.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (currentEntity is not null && currentEntity.Id != Guid.Empty)
					return new Response(true, $"{entity.DiscountPercentage} added to database successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.DiscountPercentage} ");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred adding new DiscountPercentage");
			}
		}

		public async Task<Response> DeleteAsync(ProductDiscount entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.DiscountPercentage} not found");

				context.ProductDiscounts.Remove(product);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.DiscountPercentage} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting DiscountPercentage");
			}
		}

		public async Task<ProductDiscount> FindByIdAsync(Guid id)
		{
			try
			{
				var productColor = await context.ProductDiscounts.FindAsync(id);
				return productColor is not null ? productColor : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new Exception("Error occurred retieving ProductDiscounts");
			}
		}

		public async Task<IEnumerable<ProductDiscount>> GetAllAsync()
		{
			try
			{
				var products = await context.ProductDiscounts.AsNoTracking().ToListAsync();
				return products is not null ? products : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving ProductDiscounts");
			}
		}

		public async Task<ProductDiscount> GetByAsync(Expression<Func<ProductDiscount, bool>> predicate)
		{
			try
			{
				var product = await context.ProductDiscounts.Where(predicate).FirstOrDefaultAsync()!;
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

		public async Task<Response> UpdateAsync(ProductDiscount entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.DiscountPercentage} not found");

				context.Entry(product).State = EntityState.Detached;
				context.ProductDiscounts.Update(entity);
				await context.SaveChangesAsync();

				return new Response(true, $"{entity.DiscountPercentage} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing DiscountPercentage");
			}
		}
	}
}
