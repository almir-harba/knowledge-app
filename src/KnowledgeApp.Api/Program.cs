using KnowledgeApp.Api.Data;
using KnowledgeApp.Api.Models;
using KnowledgeApp.Api.Services;
using KnowledgeApp.Api.Skills;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<SkillAcademyDbContext>(options =>
{
    if (builder.Environment.EnvironmentName == "Testing")
    {
        options.UseInMemoryDatabase(builder.Configuration["Testing:DatabaseName"] ?? "SkillAcademyTests");
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

if (builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddSingleton<IClaudeSkillFileStore, NullClaudeSkillFileStore>();
}
else
{
    builder.Services.AddSingleton<IClaudeSkillFileStore, ClaudeSkillFileStore>();
}
builder.Services.AddScoped<ISkillService, EfSkillService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SkillAcademyDbContext>();
    var skillFiles = scope.ServiceProvider.GetRequiredService<IClaudeSkillFileStore>();

    if (db.Database.IsRelational())
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
    }

    await DbSeeder.SeedAsync(db, skillFiles);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var skills = app.MapGroup("/api/skills").WithTags("Skills");

skills.MapGet("/", async (ISkillService service) =>
    Results.Ok(await service.GetAllAsync()))
    .WithName("GetSkills");

skills.MapGet("/{id:int}", async (int id, ISkillService service) =>
    await service.GetByIdAsync(id) is { } skill ? Results.Ok(skill) : Results.NotFound())
    .WithName("GetSkillById");

skills.MapPost("/", async (CreateSkillRequest request, ISkillService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Purpose) || string.IsNullOrWhiteSpace(request.HowBuilt))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["Name"] = ["Name, Purpose, and HowBuilt are required."]
        });
    }

    var created = await service.CreateAsync(request);
    return Results.Created($"/api/skills/{created.Id}", created);
})
    .WithName("CreateSkill");

skills.MapPut("/{id:int}", async (int id, UpdateSkillRequest request, ISkillService service) =>
    await service.UpdateAsync(id, request) ? Results.NoContent() : Results.NotFound())
    .WithName("UpdateSkill");

skills.MapDelete("/{id:int}", async (int id, ISkillService service) =>
    await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound())
    .WithName("DeleteSkill");

skills.MapPost("/sync", async (ISkillService service) =>
    Results.Ok(await service.SyncAsync()))
    .WithName("SyncSkills");

app.Run();

public partial class Program { }
