using E_Commerce.SharedLibray.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
	public static class ServiceContainer
	{
		public static IServiceCollection AddInfrastructureService
			(this IServiceCollection services, IConfiguration config)
		{
			// Add database connectivity
			//Add authentication scheme
			SharedServiceContainer.AddSharedServices<ProductDbContext>
							(services, config, config["MySerilog:FineName"]!);

			// Create Dependency Injection (DI)
			services.AddScoped<ICategory, CategoryRepository>();

			services.AddScoped<IProduct, ProductRepository>();

			services.AddScoped<IProductColor, ProductColorRepository>();

			services.AddScoped<IProductDiscount, ProductDiscountRepository>();

			services.AddScoped<IProductImage, ProductImageRepository>();

			services.AddScoped<IProductReview, ProductReviewRepository>();

			services.AddScoped<IProductSize, ProductSizeRepository>();

			services.AddScoped<IProductTag, ProductTagRepository>();

			services.AddHttpContextAccessor();

			return services;
		}
		public static IApplicationBuilder UseInfrastruePolicy
					(this IApplicationBuilder app)
		{
			//Register middleware such as:
			// Global Excpetion: handles external error
			//Listen to Only Api Geteway: blocks all outsilder calls;
			SharedServiceContainer.UseSharedPolicies(app);

			return app;
		}
	}
}
