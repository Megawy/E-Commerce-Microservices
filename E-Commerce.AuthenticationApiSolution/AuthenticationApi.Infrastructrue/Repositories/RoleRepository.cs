using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entites;
using AuthenticationApi.Infrastructrue.Data;
using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthenticationApi.Infrastructrue.Repositories
{
	public class RoleRepository(AuthenticationDbContext context) : IRole
	{
		public async Task<Response> CreateAsync(Role entity)
		{
			try
			{
				// check if the Role already exist
				var getRole = await GetByAsync(_ => _.RoleName!.ToUpper().Equals(entity.RoleName!.ToUpper()));
				if (getRole is not null && !string.IsNullOrEmpty(getRole.RoleName))
					return new Response(false, $"{entity.RoleName} already added");

				entity.created_at = DateTime.UtcNow;
				entity.updated_at = DateTime.UtcNow;
				var result = context.Roles.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (result is not null && result.Id != Guid.Empty)
					return new Response(true, $"{entity.RoleName} Role Created successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.RoleName} ");
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while Create Role");
			}
		}

		public async Task<Response> DeleteAsync(Role entity)
		{
			try
			{
				var role = await context.Roles.FindAsync(entity.Id);
				if (role is null)
					return new Response(false, $"{entity.RoleName} not found");

				context.Roles.Remove(role);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.RoleName} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting role");
			}
		}

		public async Task<Role> FindByIdAsync(Guid id)
		{
			try
			{
				var getRole = await context.Roles!.FindAsync(id);
				return getRole is not null ? getRole : null!;
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while retrieving role");
			}
		}

		public async Task<IEnumerable<Role>> GetAllAsync()
		{
			try
			{
				var Roles = await context.Roles.AsNoTracking().ToListAsync();
				return Roles is not null ? Roles : null!;
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while retrieving Roles");
			}
		}

		public async Task<Role> GetByAsync(Expression<Func<Role, bool>> predicate)
		{
			try
			{
				var Role = await context.Roles.Where(predicate).FirstOrDefaultAsync()!;
				return Role is not null ? Role : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving role");
			}
		}

		public async Task<Response> UpdateAsync(Role entity)
		{
			try
			{
				// check if the Role already exist
				var getRole = await GetByAsync(_ => _.RoleName!.ToUpper().Equals(entity.RoleName!.ToUpper()));
				if (getRole is not null && !string.IsNullOrEmpty(getRole.RoleName))
					return new Response(false, $"{entity.RoleName} already added");

				var role = await FindByIdAsync(entity.Id);
				if (role is null)
					return new Response(false, $"{entity.Id} not found");

				entity.updated_at = DateTime.UtcNow;
				context.Entry(role).State = EntityState.Detached;

				context.Roles.Update(entity);
				context.Entry(entity).Property(r => r.created_at).IsModified = false;
				await context.SaveChangesAsync();

				return new Response(true, $"{entity.RoleName} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing role");
			}
		}
	}
}
