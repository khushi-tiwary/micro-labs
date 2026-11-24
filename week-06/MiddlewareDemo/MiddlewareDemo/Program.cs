
using MiddlewareDemo.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register controller services
builder.Services.AddControllers();

var app = builder.Build();

// Built-in middleware
app.UseHttpsRedirection();

// Register custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// Map controller endpoints
app.MapControllers();

app.Run();
