using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ProductApi.Infrastructure.Repositories
{
	public class ProductReviewRepository(ProductDbContext context, IHttpContextAccessor httpContextAccessor) : IProductReview
	{
		public async Task<Response> CreateAsync(ProductReview entity)
		{
			try
			{
				var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId))
					return new Response(false, "User not found");

				entity.CustomerId = Guid.Parse(userId);
				entity.CreatedAt = DateTime.Now;
				var currentEntity = context.ProductReviews.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (currentEntity is not null && currentEntity.Id != Guid.Empty)
					return new Response(true, $"{entity.Rating} added to database successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.Rating} ");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred adding new ProductReview");
			}
		}

		public async Task<Response> DeleteAsync(ProductReview entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.Rating} not found");

				context.ProductReviews.Remove(product);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.Rating} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting ProductReviews");
			}
		}

		public async Task<ProductReview> FindByIdAsync(Guid id)
		{
			try
			{
				var productColor = await context.ProductReviews.FindAsync(id);
				return productColor is not null ? productColor : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new Exception("Error occurred retieving ProductReviews");
			}
		}

		public async Task<IEnumerable<ProductReview>> GetAllAsync()
		{
			try
			{
				var products = await context.ProductReviews.AsNoTracking().ToListAsync();
				return products is not null ? products : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving ProductReviews");
			}
		}

		public async Task<ProductReview> GetByAsync(Expression<Func<ProductReview, bool>> predicate)
		{
			try
			{
				var product = await context.ProductReviews.Where(predicate).FirstOrDefaultAsync()!;
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

		public async Task<Response> UpdateAsync(ProductReview entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.Rating} not found");

				context.Entry(product).State = EntityState.Detached;
				entity.CreatedAt = DateTime.UtcNow;
				context.ProductReviews.Update(entity);
				await context.SaveChangesAsync();

				return new Response(true, $"{entity.Rating} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing ProductReviews");
			}
		}
	}
}
