using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using E_Commerce.SharedLibray.DependencyInjection;
using ApiGateway.Presentation.Middelware;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
JWTAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(op =>
	{
		op.AddDefaultPolicy(builder =>
		{
			builder.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowAnyOrigin();
		});
	});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<AttachSignatrueToRequset>();
app.UseOcelot().Wait();
app.Run();



