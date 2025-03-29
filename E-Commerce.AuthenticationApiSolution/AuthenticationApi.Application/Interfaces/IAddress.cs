using AuthenticationApi.Domain.Entites;
using E_Commerce.SharedLibray.Interface;

namespace AuthenticationApi.Application.Interfaces
{
	public interface IAddress : IGenericInterface<UserAddress>
	{
		Task<IEnumerable<UserAddress>> GetAddressByUserId();
	}
}
