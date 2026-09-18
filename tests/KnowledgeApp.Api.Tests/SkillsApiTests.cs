using System.Net;
using System.Net.Http.Json;
using KnowledgeApp.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace KnowledgeApp.Api.Tests;

public class SkillsApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Testing:DatabaseName"] = _dbName
            });
        });
    }
}

public class SkillsApiTests : IClassFixture<SkillsApiFactory>
{
    private readonly HttpClient _client;

    public SkillsApiTests(SkillsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededSkills()
    {
        var skills = await _client.GetFromJsonAsync<List<Skill>>("/api/skills/");

        Assert.NotNull(skills);
        Assert.Contains(skills!, s => s.Name == "Skill Academy API");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForUnknownId()
    {
        var response = await _client.GetAsync("/api/skills/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesSkill_AndGetByIdReturnsIt()
    {
        var request = new CreateSkillRequest("Test Skill", "Testing", "Purpose", "How built", ["test"]);

        var postResponse = await _client.PostAsJsonAsync("/api/skills/", request);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<Skill>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync(postResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsValidationProblem_WhenNameMissing()
    {
        var request = new CreateSkillRequest("", "Category", "Purpose", "How built", null);

        var response = await _client.PostAsJsonAsync("/api/skills/", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesSkill()
    {
        var created = await _client.PostAsJsonAsync("/api/skills/",
            new CreateSkillRequest("To Delete", "Category", "Purpose", "How built", null));
        var location = created.Headers.Location;

        var deleteResponse = await _client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
