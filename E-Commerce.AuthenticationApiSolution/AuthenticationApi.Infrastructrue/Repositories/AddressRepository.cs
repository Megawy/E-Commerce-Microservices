using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entites;
using AuthenticationApi.Infrastructrue.Data;
using E_Commerce.SharedLibray.Logs;
using E_Commerce.SharedLibray.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace AuthenticationApi.Infrastructrue.Repositories
{
	public class AddressRepository(AuthenticationDbContext context, IHttpContextAccessor httpContextAccessor) : IAddress
	{
		public async Task<Response> CreateAsync(UserAddress entity)
		{
			try
			{
				// check if the Address already exist
				var getAddress = await GetByAsync(_ => _.street!.ToUpper().Equals(entity.street!.ToUpper()));

				if (getAddress is not null && !string.IsNullOrEmpty(getAddress.street))
					return new Response(false, $"{entity.street} already added");

				var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (string.IsNullOrEmpty(userId))
					return new Response(false, "User not found");

				entity.UserId = Guid.Parse(userId);
				entity.created_at = DateTime.UtcNow;
				entity.updated_at = DateTime.UtcNow;

				var result = context.Address.Add(entity).Entity;
				await context.SaveChangesAsync();

				if (result is not null && result.Id != Guid.Empty)
					return new Response(true, $"{entity.street} Address Created successfully");
				else
					return new Response(false, $"Error occurred while adding {entity.street} ");
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while Create Address");
			}
		}

		public async Task<Response> DeleteAsync(UserAddress entity)
		{
			try
			{
				var address = await context.Address.FindAsync(entity.Id);

				if (address is null)
					return new Response(false, $"{entity.Id} not found");

				context.Address.Remove(address);
				await context.SaveChangesAsync();
				return new Response(true, $"{entity.Id} is deleted successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred Deleting address");
			}
		}

		public async Task<UserAddress> FindByIdAsync(Guid id)
		{
			try
			{
				var getAddress = await context.Address!.FindAsync(id);
				return getAddress is not null ? getAddress : null!;
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while retrieving address");
			}
		}

		public async Task<IEnumerable<UserAddress>> GetAddressByUserId()
		{
			try
			{
				var userIdStr = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userIdStr))
					return Enumerable.Empty<UserAddress>();

				if (!Guid.TryParse(userIdStr, out var userId))
					return Enumerable.Empty<UserAddress>();

				var addresses = await context.Address
											 .AsNoTracking()
											 .Where(a => a.UserId == userId)
											 .ToListAsync();

				return addresses.Any() ? addresses : Enumerable.Empty<UserAddress>();
			}
			catch (Exception ex)
			{
				LogException.LogExceptions(ex);
				throw new Exception("Error Occurred while retrieving Addresses");
			}
		}

		public async Task<IEnumerable<UserAddress>> GetAllAsync()
		{
			try
			{
				var Addresses = await context.Address.AsNoTracking().ToListAsync();
				return Addresses is not null ? Addresses : null!;
			}
			catch (Exception ex)
			{
				//Log Original Exception
				LogException.LogExceptions(ex);

				// Display Scary-Free message to client 
				throw new Exception("Error Occured while retrieving Addresses");
			}
		}

		public async Task<UserAddress> GetByAsync(Expression<Func<UserAddress, bool>> predicate)
		{
			try
			{
				var Address = await context.Address.Where(predicate).FirstOrDefaultAsync()!;
				return Address is not null ? Address : null!;
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				throw new InvalidOperationException("Error occurred retieving Address");
			}
		}

		public async Task<Response> UpdateAsync(UserAddress entity)
		{
			try
			{
				// check if the Address already exist
				var getAddress = await GetByAsync(_ => _.street!.ToUpper().Equals(entity.street!.ToUpper()));

				if (getAddress is not null && !string.IsNullOrEmpty(getAddress.street))
					return new Response(false, $"{entity.street} already added");

				var address = await FindByIdAsync(entity.Id);
				if (address is null)
					return new Response(false, $"{entity.Id} not found");

				var userIdStr = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userIdStr))
					return new Response(false, $"userIdStr not found");

				entity.UserId = Guid.Parse(userIdStr);
				entity.updated_at = DateTime.UtcNow;

				context.Entry(address).State = EntityState.Detached;
				context.Address.Update(entity);
				context.Entry(entity).Property(r => r.created_at).IsModified = false;
				await context.SaveChangesAsync();

				return new Response(true, $"{entity.street} is updated successfully");
			}
			catch (Exception ex)
			{
				//Log the original exception
				LogException.LogExceptions(ex);

				// display scary-free message to the client
				return new Response(false, "Error occurred updating existing address");
			}
		}
	}
}
