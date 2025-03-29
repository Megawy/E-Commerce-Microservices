using AuthenticationApi.Infrastructrue.Data;
using AuthenticationApi.Infrastructrue.DependencyInjection;
using AuthenticationApi.Infrastructure.Data;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructrueService(builder.Configuration);
var app = builder.Build();
// Seed Data in DB
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
	var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

	var seedData = new SeedData(dbContext, config);
	await seedData.SeedRolesAsync();
}
app.UserInfrastructruePolicy();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard("/hangfire");
app.MapHangfireDashboard("/hangfire");
app.Run();
