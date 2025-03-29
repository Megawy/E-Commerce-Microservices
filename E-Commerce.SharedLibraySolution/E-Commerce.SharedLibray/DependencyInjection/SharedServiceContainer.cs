using E_Commerce.SharedLibray.Middleware;
using E_Commerce.SharedLibray.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Hangfire;
using Hangfire.SqlServer;
using Polly.Retry;
using Polly;
using E_Commerce.SharedLibray.Logs;

namespace E_Commerce.SharedLibray.DependencyInjection
{
	public static class SharedServiceContainer
	{
		public static IServiceCollection AddSharedServices<TContext>
			(this IServiceCollection services, IConfiguration config, string filename) where TContext : DbContext
		{
			// Add Generic Database context
			string connectionString = config.GetConnectionString("eCommerceConnection")!;
			services.AddDbContext<TContext>(option => option.UseSqlServer(
				connectionString, SqlServerOption =>
				SqlServerOption.EnableRetryOnFailure()));


			// configure hangfire
			services.AddHangfire(hangfireConfig =>
				hangfireConfig.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
							  .UseSimpleAssemblyNameTypeSerializer()
							  .UseRecommendedSerializerSettings()
							  .UseSqlServerStorage(
								  connectionString,
								  new SqlServerStorageOptions
								  {
									  CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
									  SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
									  QueuePollInterval = TimeSpan.Zero,
									  UseRecommendedIsolationLevel = true,
									  DisableGlobalLocks = true
								  }
							  ));

			services.AddHangfireServer();

			// configure Serilog logging
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.Debug()
				.WriteTo.Console()
				.WriteTo.File(path: $"{filename}-.text",
				restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
				outputTemplate: "{Timestamp:yyyy-MM-DD HH:mm:ss.fff zzz} [{Level:u3} {message:lj}{NewLine}{Exception}]",
				rollingInterval: RollingInterval.Day)
				.CreateLogger();


			// Create Retry Strategy 
			var retryStrategy = new RetryStrategyOptions()
			{
				ShouldHandle = new PredicateBuilder()
				.Handle<TaskCanceledException>()
				.Handle<HttpRequestException>()
				.Handle<TimeoutException>()
				.Handle<OperationCanceledException>(),

				BackoffType = DelayBackoffType.Exponential,
				UseJitter = true,
				MaxRetryAttempts = 3,
				Delay = TimeSpan.FromSeconds(2),
				OnRetry = args =>
				{
					string message = $"OnRetry, Attempt: {args.AttemptNumber} Outcome: {args.Outcome.Exception?.Message}";
					LogException.LogToConsole(message);
					LogException.LogToDebugger(message);
					return ValueTask.CompletedTask;
				}
			};

			// Use Retry strategy
			services.AddResiliencePipeline("my-retry-pipeline", builder =>
			{
				builder.AddRetry(retryStrategy);
			});


			// Add JWT authentication Scheme
			JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);

			// Create Dependency Injection
			services.AddScoped<IEmailService, EmailService>();

			// Run Services AutoMapper
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			return services;
		}

		public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
		{
			// Use global Exception
			app.UseMiddleware<GlobalExcption>();

			// Refister middleware to block all outsiders API calls 
			app.UseMiddleware<ListenToOnlyApiGateway>();

			return app;
		}
	}
}
