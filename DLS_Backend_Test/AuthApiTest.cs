using System.Text;
using DLS_Backend_Test.Models;
using DLS_Backend.Controller;
using dotenv.net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace DLS_Backend_Test;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>> 
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly IConfiguration _configuration;

    public ApiTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _output = output;
        
        if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") != "true")
        {
            // Only load the .env file when not running on GitHub Actions
            DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../../../.env" }, ignoreExceptions: false));
        }
        // Set up configuration to read from the environment variables
        _configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        SetupEnvironmentVariables();
        InitializeTestDatabase(_configuration["DB_SERVER"], 
                                _configuration["DB_BACKEND"], 
                                _configuration["DB_USER"], 
                                "hej");
        
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri("http://localhost:5012/v1/");
    }
    
    [Fact]
    public async Task RegisterUserTest()
    {
        // Arrange
        var content = new StringContent(JsonConvert.SerializeObject(_registerRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("register", content);
        var responseData = await response.Content.ReadAsStringAsync();
        var actualData = JsonConvert.DeserializeObject<User>(responseData);
        _output.WriteLine("Received response: " + responseData);
        
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(actualData?.email == _registerRequest.email && 
                    actualData.first_name == _registerRequest.first_name && 
                    actualData.last_name == _registerRequest.last_name &&
                    actualData.password == null);
    }

    [Fact]
    public async Task LoginUserTest()
    {
        //Arrangte
        var content = new StringContent(JsonConvert.SerializeObject(_loginRequest), Encoding.UTF8, "application/json");
        
        //Act
        var response = await _client.PostAsync("login", content);
        var responseData = await response.Content.ReadAsStringAsync();
        var actualData = JsonConvert.DeserializeObject(responseData);
        _output.WriteLine("Received response: " + responseData);
        
        //Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(actualData != null);
    }
    
    [Fact]
    public async Task GenerateTokenTest()
    {
        //Act
        var response = await _client.GetAsync("generate-token");
        var responseData = await response.Content.ReadAsStringAsync();
        var actualData = JsonConvert.DeserializeObject(responseData);
        _output.WriteLine("Received response: " + responseData);
        
        //Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(actualData != null);
    }
    
    [Fact]
    public async Task GetUsersTest()
    {
        string email = "zo@pyra.dk";
        //Act
        var response = await _client.GetAsync($"get-user/{email}");
        var responseData = await response.Content.ReadAsStringAsync();
        var actualData = JsonConvert.DeserializeObject<User>(responseData);
        
        _output.WriteLine("Received response: " + responseData);
        
        //Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(actualData.first_name == "Zack" && 
                    actualData.last_name == "Ottesen" && 
                    actualData.email == email);
    }
    
    private void SetupEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("DB_SERVER", _configuration["DB_SERVER"]);
        Environment.SetEnvironmentVariable("DB_BACKEND", _configuration["DB_BACKEND"]);
        Environment.SetEnvironmentVariable("DB_USER", _configuration["DB_USER"]);
        Environment.SetEnvironmentVariable("DB_PASSWORD", "hej3");
        Environment.SetEnvironmentVariable("JWT_SECRET", _configuration["JWT_SECRET"]);
        Environment.SetEnvironmentVariable("FRONTENDURL", _configuration["FRONTENDURL"]);
        Environment.SetEnvironmentVariable("RABBITUSER", _configuration["RABBITUSER"]);
        Environment.SetEnvironmentVariable("RABBITPW", _configuration["RABBITPW"]);
        Environment.SetEnvironmentVariable("RABBITURL", _configuration["RABBITURL"]);
        
        Console.WriteLine(_configuration["DB_SERVER"]);
        Console.WriteLine(_configuration["DB_BACKEND"]);
        Console.WriteLine(_configuration["DB_USER"]);
        Console.WriteLine(_configuration["DB_PASSWORD"]);
        Console.WriteLine(_configuration["JWT_SECRET"]);
        Console.WriteLine(_configuration["FRONTENDURL"]);
        Console.WriteLine(_configuration["RABBITUSER"]);
        Console.WriteLine(_configuration["RABBITPW"]);
        Console.WriteLine(_configuration["RABBITURL"]);
        
    }
    
    private void InitializeTestDatabase(string server, string database, string user, string password)
    {
        var connectionString = $"Server={server};Database=master;User={user};Password={password};TrustServerCertificate=True;";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = $@"IF DB_ID('{database}') IS NOT NULL BEGIN ALTER DATABASE {database} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE {database};END CREATE DATABASE {database};"; 
        command.ExecuteNonQuery();
        connection.Close();
        var options = new DbContextOptionsBuilder<DlsUsersContext>()
            .UseSqlServer(connectionString.Replace("master", database), b => b.MigrationsAssembly("DLS_Backend"))
            .Options;
        using var context = new DlsUsersContext(options);
        context.Database.Migrate();
    }

    private readonly RegisterRequest _registerRequest = new()
    {
        first_name = "test",
        last_name = "test-test",
        email = "test132@test.dk",
        password = "test123", // Store the hashed password
        guid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 
    };

    private readonly LoginRequest _loginRequest = new()
    {
        email = "zo@pyra.dk",
        password = "Test123"
    };
}