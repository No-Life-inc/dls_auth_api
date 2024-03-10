using DLS_Backend.Controller;
using DLS_Backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Hashing hash = new Hashing();

builder.Services.AddDbContext<DbContextSetup>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/register", async (string name, string email, string password, DbContextSetup context) =>
    {
    var hashedPassword = hash.Hash(password); // Hash the password before storing
    var user = new User
    {
        Name = name,
        Email = email,
        Password = hashedPassword, // Store the hashed password
        Guid = Guid.NewGuid(), // Assign a new Guid
        CreatedAt = DateTime.UtcNow // Set the created time
    };

    context.Users.Add(user);
    await context.SaveChangesAsync(); // Save changes in the DB context

    return Results.Created($"/users/{user.Id}", user); // Return the created user object and the location header
}).WithName("Encryption")
    .WithOpenApi();

app.Run();