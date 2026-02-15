using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexus.API.Data;
using Nexus.API.DTOs;
using Nexus.API.Models;
using Npgsql;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Nexus.API.Tests.Integration;

[Collection("PostgresIntegration")]
public sealed class WordControllerIntegrationTests
{
    private static readonly string AdminConnectionString = BuildAdminConnectionString();

    private static async Task<string> CreateTestDatabaseAsync()
    {
        var dbName = $"nexus_it_{Guid.NewGuid():N}";

        await using var adminConnection = new NpgsqlConnection(AdminConnectionString);
        await adminConnection.OpenAsync();

        await using var createCommand = adminConnection.CreateCommand();
        createCommand.CommandText = $"CREATE DATABASE \"{dbName}\"";
        await createCommand.ExecuteNonQueryAsync();

        return dbName;
    }

    private static async Task DropTestDatabaseAsync(string databaseName)
    {
        await using var adminConnection = new NpgsqlConnection(AdminConnectionString);
        await adminConnection.OpenAsync();

        await using var dropCommand = adminConnection.CreateCommand();
        dropCommand.CommandText = $"DROP DATABASE IF EXISTS \"{databaseName}\" WITH (FORCE)";
        await dropCommand.ExecuteNonQueryAsync();
    }

    private static string BuildTestConnectionString(string databaseName)
    {
        var builder = new NpgsqlConnectionStringBuilder(AdminConnectionString)
        {
            Database = databaseName,
            Pooling = false
        };

        return builder.ConnectionString;
    }

    private static string BuildAdminConnectionString()
    {
        var explicitConnection = Environment.GetEnvironmentVariable("NEXUS_TEST_DB_ADMIN_CONNECTION");
        if (!string.IsNullOrWhiteSpace(explicitConnection))
        {
            return explicitConnection;
        }

        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "127.0.0.1";
        var portText = Environment.GetEnvironmentVariable("POSTGRES_PORT");
        var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        var database = Environment.GetEnvironmentVariable("POSTGRES_ADMIN_DB") ?? "postgres";
        var port = int.TryParse(portText, out var parsedPort) ? parsedPort : 5432;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Set NEXUS_TEST_DB_ADMIN_CONNECTION or both POSTGRES_USER and POSTGRES_PASSWORD before running integration tests.");
        }

        return new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Username = username,
            Password = password,
            Database = database,
            Pooling = false
        }.ConnectionString;
    }

    private static async Task ApplyMigrationsAsync(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(BuildTestConnectionString(databaseName))
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();
    }

    private static async Task<(WebApplication app, HttpClient client)> StartApiAsync(string databaseName)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        builder.WebHost.UseKestrel();
        builder.WebHost.UseUrls("http://127.0.0.1:0");

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(Nexus.API.Controllers.WordController).Assembly);

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(BuildTestConnectionString(databaseName));
        });

        builder.Services.AddAutoMapper(typeof(Nexus.API.Profiles.UserProfile).Assembly);

        var app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();

        await app.StartAsync();

        var baseAddress = app.Urls.Single();
        var client = new HttpClient { BaseAddress = new Uri(baseAddress) };

        return (app, client);
    }

    private static async Task<(string dbName, WebApplication app, HttpClient client)> StartApiWithFreshDatabaseAsync()
    {
        var databaseName = await CreateTestDatabaseAsync();
        await ApplyMigrationsAsync(databaseName);
        var (app, client) = await StartApiAsync(databaseName);

        return (databaseName, app, client);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetWords_Returns200AndExpectedJsonShape()
    {
        var (databaseName, app, client) = await StartApiWithFreshDatabaseAsync();

        try
        {
            var response = await client.GetAsync("/api/words");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var root = JsonNode.Parse(json)?.AsArray();
            Assert.NotNull(root);
            Assert.NotEmpty(root);

            var first = root[0]!.AsObject();
            Assert.True(first.ContainsKey("id"));
            Assert.True(first.ContainsKey("term"));
            Assert.True(first.ContainsKey("languageCode"));
        }
        finally
        {
            client.Dispose();
            await app.DisposeAsync();
            await DropTestDatabaseAsync(databaseName);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateWord_UsesExpectedStatusCodesAndJsonPayload()
    {
        var (databaseName, app, client) = await StartApiWithFreshDatabaseAsync();

        try
        {
            var createdResponse = await client.PostAsJsonAsync("/api/words", new CreateWordDto
            {
                Term = "  Sample Term  ",
                LanguageCode = " EN "
            });

            Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
            var createdWord = await createdResponse.Content.ReadFromJsonAsync<WordDto>();
            Assert.NotNull(createdWord);
            Assert.Equal("Sample Term", createdWord.Term);
            Assert.Equal("en", createdWord.LanguageCode);

            var duplicateResponse = await client.PostAsJsonAsync("/api/words", new CreateWordDto
            {
                Term = "sample term",
                LanguageCode = "en"
            });

            Assert.Equal(HttpStatusCode.OK, duplicateResponse.StatusCode);
            var duplicateWord = await duplicateResponse.Content.ReadFromJsonAsync<WordDto>();
            Assert.NotNull(duplicateWord);
            Assert.Equal(createdWord.Id, duplicateWord.Id);
        }
        finally
        {
            client.Dispose();
            await app.DisposeAsync();
            await DropTestDatabaseAsync(databaseName);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task AddWordToUser_ReturnsConflictProblemDetailsJson_WhenDuplicate()
    {
        var (databaseName, app, client) = await StartApiWithFreshDatabaseAsync();

        try
        {
            var firstResponse = await client.PostAsJsonAsync("/api/users/1/words", new AddWordToUserDto
            {
                WordId = 1
            });
            Assert.Equal(HttpStatusCode.NoContent, firstResponse.StatusCode);

            var duplicateResponse = await client.PostAsJsonAsync("/api/users/1/words", new AddWordToUserDto
            {
                WordId = 1
            });

            Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
            var duplicateJson = await duplicateResponse.Content.ReadAsStringAsync();
            var problem = JsonNode.Parse(duplicateJson)!.AsObject();

            Assert.Equal(StatusCodes.Status409Conflict, problem["status"]!.GetValue<int>());
            Assert.Equal("Word already assigned", problem["title"]!.GetValue<string>());
            Assert.Equal("This word is already in the user's list.", problem["detail"]!.GetValue<string>());
        }
        finally
        {
            client.Dispose();
            await app.DisposeAsync();
            await DropTestDatabaseAsync(databaseName);
        }
    }
}
