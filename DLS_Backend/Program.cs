using DLS_Backend.Controller;
using DLS_Backend.utility;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);
Hashing hash = new Hashing();

// Continue with your service configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DLS Backend API", Version = "v1" });
});
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddHostedService<DeleteUsersService>();
builder.Services.AddSingleton(new JwtService(Env.GetString("JWT_SECRET")));
builder.Services.AddDbContext<DlsUsersContext>(options =>
    options.UseSqlServer(
        $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
        $"Database={Environment.GetEnvironmentVariable("DB_BACKEND")};" +
        $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
        $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
        "TrustServerCertificate=True;"
    )
);



// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", corsbuilder =>
    {
        corsbuilder.WithOrigins(Environment.GetEnvironmentVariable("FRONTENDURL")) // Tilpas domænet til din frontend
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Migrate any pending changes to the database before running the app
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DlsUsersContext>();
        context.Database.Migrate(); // This will apply pending migrations
    }
    catch (Exception ex)
    {
        // Log errors or handle them as needed
        throw;
    }
}

// Enable CORS
app.UseCors("AllowOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { }