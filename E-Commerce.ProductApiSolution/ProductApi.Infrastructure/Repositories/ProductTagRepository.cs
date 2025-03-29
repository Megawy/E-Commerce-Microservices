using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
	public class ProductTagRepository(ProductDbContext context) : IProductTag
	{
		public async Task<Response> CreateAsync(ProductTag entity)
		{
			try
			{
				// check if the product already exist
				var getProduct = await GetByAsync(_ => _.Tag!.Equals(entity.Tag));
				if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Tag))
					return new Response(false, $"{entity.Tag} already added");

				var currentEntity = context.ProductTags.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (currentEntity is not null && currentEntity.Id != Guid.Empty)
					return new Response(true, $"{entity.Tag} added to database successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.Tag} ");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred adding new Tag");

			}
		}

		public async Task<Response> DeleteAsync(ProductTag entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.Tag} not found");

				context.ProductTags.Remove(product);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.Tag} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting Tag");
			}
		}

		public async Task<ProductTag> FindByIdAsync(Guid id)
		{
			try
			{
				var product = await context.ProductTags.FindAsync(id);
				return product is not null ? product : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new Exception("Error occurred retieving Tag");
			}
		}

		public async Task<IEnumerable<ProductTag>> GetAllAsync()
		{
			try
			{
				var products = await context.ProductTags.AsNoTracking().ToListAsync();
				return products is not null ? products : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving Tags");
			}
		}

		public async Task<ProductTag> GetByAsync(Expression<Func<ProductTag, bool>> predicate)
		{
			try
			{
				var product = await context.ProductTags.Where(predicate).FirstOrDefaultAsync()!;
				return product is not null ? product : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving Tag");
			}
		}

		public async Task<Response> UpdateAsync(ProductTag entity)
		{
			try
			{
				var product = await FindByIdAsync(entity.Id);
				if (product is null)
					return new Response(false, $"{entity.Tag} not found");

				context.Entry(product).State = EntityState.Detached;
				context.ProductTags.Update(entity);
				await context.SaveChangesAsync();

				return new Response(true, $"{entity.Tag} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing Tag");
			}
		}
	}
}
