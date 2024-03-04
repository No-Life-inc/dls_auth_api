using DLS_Backend.utility;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();


// Get JWT secret from environment variables
var jwtSecret = Env.GetString("JWT_SECRET");


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<JwtService>(new JwtService(jwtSecret));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


// Endpoint for testing generating a token - only for testing purposes
app.MapGet("/generate-token", (JwtService jwtService) => 
    {
        // Generating a token with a fictive GUID
        string token = jwtService.GenerateToken("f79330ab-c0bd-4bf8-97f2-37718917f2c9");

        // Console log the token
        Console.WriteLine($"Generated JWT token: {token}");

        // Returns the generated token as a HTTP-respons
        return Results.Ok(new { token });
    })
    .WithName("GenerateToken")
    .WithOpenApi();





app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}