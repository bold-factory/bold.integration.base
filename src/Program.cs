using Bold.Integration.Base.Configuration;
using Bold.Integration.Base.Endpoints;
using Bold.Integration.Base.Middlewares;
using Bold.Integration.Base.Persistence;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureServices();

var app = builder.Build();

app.MapHealthChecks("/health");
app.UseMiddleware<HmacValidationMiddleware>(builder.Configuration["HMACSecret"]);
app.UseCors(app.Environment.IsDevelopment() ? CorsConfigurator.DevelopmentOrigins : CorsConfigurator.ProductionOrigins);
app.ApplyMigrations();
app.MapAllEndpoints();

app.Logger.LogInformation("Application is booting...");
app.Run();