using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
	public class CategoryRepository(ProductDbContext context) : ICategory
	{
		public async Task<Response> CreateAsync(Category entity)
		{
			try
			{
				// check if the product already exist
				var getCategory = await GetByAsync(_ => _.Name!.Equals(entity.Name));
				if (getCategory is not null && !string.IsNullOrEmpty(getCategory.Name))
					return new Response(false, $"{entity.Name} already added");


				entity.CreatedAt = DateTime.UtcNow;
				entity.UpdatedAt = DateTime.UtcNow;
				var currentEntity = context.Categories.Add(entity).Entity;
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
				return new Response(false, "Error occurred adding new category");
			}
		}

		public async Task<Response> DeleteAsync(Category entity)
		{
			try
			{
				var category = await FindByIdAsync(entity.Id);
				if (category is null)
					return new Response(false, $"{entity.Name} not found");

				context.Categories.Remove(category);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.Name} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting category");
			}
		}

		public async Task<Category> FindByIdAsync(Guid id)
		{
			try
			{
				var category = await context.Categories
					.Include(P => P.Products!)
					.ThenInclude(p => p.productImages)
					.Include(P => P.Products!)
					.ThenInclude(p => p.ProductDiscounts)
					.FirstOrDefaultAsync(p => p.Id == id);
				return category is not null ? category : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new Exception("Error occurred retieving category");
			}
		}

		public async Task<IEnumerable<Category>> GetAllAsync()
		{
			try
			{
				var categories = await context.Categories.AsNoTracking().ToListAsync();
				return categories is not null ? categories : null!;

			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving categories");
			}
		}

		public async Task<Category> GetByAsync(Expression<Func<Category, bool>> predicate)
		{
			try
			{
				var category = await context.Categories.Where(predicate).FirstOrDefaultAsync()!;
				return category is not null ? category : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving category");
			}
		}

		public async Task<Response> UpdateAsync(Category entity)
		{
			try
			{
				var category = await FindByIdAsync(entity.Id);
				if (category is null)
					return new Response(false, $"{entity.Name} not found");

				context.Entry(category).State = EntityState.Detached;
				entity.UpdatedAt = DateTime.UtcNow;
				context.Categories.Update(entity);
				context.Entry(entity).Property(r => r.CreatedAt).IsModified = false;
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.Name} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing category");
			}
		}
	}
}
