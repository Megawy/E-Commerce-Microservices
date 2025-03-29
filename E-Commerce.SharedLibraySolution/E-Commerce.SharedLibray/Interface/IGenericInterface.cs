using System.Linq.Expressions;
using E_Commerce.SharedLibray.Responses;
namespace E_Commerce.SharedLibray.Interface
{
	public interface IGenericInterface<T> where T : class
	{
		Task<Response> CreateAsync(T entity);
		Task<Response> UpdateAsync(T entity);
		Task<Response> DeleteAsync(T entity);
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> FindByIdAsync(Guid id);
		Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
	}

}
