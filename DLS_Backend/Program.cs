using DLS_Backend.utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tilføj JwtService til containeren
builder.Services.AddSingleton<JwtService>(new JwtService("2b13d563f605b3bb6b5f43ec95a2aaeef1d780049d91d62e0d7c04d70d46de44"));


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


// Opret en ny endpoint til at generere JWT-token
app.MapGet("/generate-token", (JwtService jwtService) => 
    {
        // Generer et JWT-token med en fiktiv bruger-id
        string token = jwtService.GenerateToken("f79330ab-c0bd-4bf8-97f2-37718917f2c9");

        // Log token til konsollen
        Console.WriteLine($"Generated JWT token: {token}");

        // Returner det genererede token som en HTTP-respons
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