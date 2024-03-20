using System.Text.Json;
using DLS_Backend.Controller;
using DLS_Backend.Models;
using DLS_Backend.utility;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Hashing hash = new Hashing();

builder.Services.AddDbContext<DbContextSetup>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Load environment variables from .env file
Env.Load();


// Get JWT secret from environment variables
var jwtSecret = Env.GetString("JWT_SECRET");


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<JwtService>(new JwtService(jwtSecret));

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", corsbuilder =>
    {
        corsbuilder.WithOrigins("http://localhost:8080") // Tilpas domænet til din frontend
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/register", async (RegisterRequest request, DbContextSetup context) =>
{
    var hashedPassword = hash.Hash(request.password); // Hash the password before storing
    var user = new User
    {
        first_name = request.first_name,
        last_name = request.last_name,
        email = request.email,
        password = hashedPassword, // Store the hashed password
        guid = request.guid, 
        created_at = DateTime.UtcNow // Set the created time
    };
    context.Users.Add(user);
    await context.SaveChangesAsync(); // Save changes in the DB context
    
    user.password = null; // Remove the password from the user object
    
    string userJson = JsonSerializer.Serialize(user);
    
    var rabbitMQService = new RabbitMQService();
    rabbitMQService.SendMessage(userJson);
    rabbitMQService.Close(); 

    return Results.Created($"/users/{user.id}", user); // Return the created user object and the location header
}).WithName("Encryption")
.WithOpenApi();

app.MapPost("/login", async (LoginRequest request, DbContextSetup context, JwtService jwtService) =>
{
    var user = await context.Users.FirstOrDefaultAsync(u => u.email == request.email);
    if (user == null)
    {
        return Results.NotFound("User not found");
    }
    if (hash.Verify(request.password, user.password))
    {
        var token = jwtService.GenerateToken(user.guid);
        return Results.Ok(new { token });
    }
    return Results.Unauthorized();
}).WithName("Login")
.WithOpenApi();

// Endpoint for testing generating a token - only for testing purposes
app.MapGet("/generate-token", (JwtService jwtService) => 
    {
        // Generating a token with a fictive GUID
        string token = jwtService.GenerateToken("f79330ab-c0bd-4bf8-97f2-37718917f2c9");

        // Console log the token
        Console.WriteLine($"Generated JWT token: {token}");

        // Returns the generated token as a HTTP-respons
        return Results.Ok(new { token });
    }).WithName("GenerateToken")
    .WithOpenApi();

app.Run();