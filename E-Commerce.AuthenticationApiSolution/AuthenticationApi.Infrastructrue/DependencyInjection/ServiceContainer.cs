using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructrue.Data;
using AuthenticationApi.Infrastructrue.Repositories;
using E_Commerce.SharedLibray.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructrue.DependencyInjection
{
	public static class ServiceContainer
	{
		public static IServiceCollection AddInfrastructrueService(this IServiceCollection services, IConfiguration config)
		{
			// Add database connectivity 
			// JWT Add Authentication Scheme
			SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

			// Create Dependency Injection
			services.AddScoped<IUser, UserRepository>();

			services.AddScoped<IRole, RoleRepository>();

			services.AddScoped<IAddress, AddressRepository>();

			services.AddHttpContextAccessor();

			services.AddMemoryCache();

			return services;
		}

		public static IApplicationBuilder UserInfrastructruePolicy(this IApplicationBuilder app)
		{
			// Register middleware such as:
			// Global Exception : Handle external errors 
			// Listen Only To Api Gateway : block all outsiders call.
			SharedServiceContainer.UseSharedPolicies(app);

			return app;
		}
	}
}


