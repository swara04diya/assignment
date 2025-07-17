/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
*/
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.ApiModels;
using WorkflowEngine.Domain;
using WorkflowEngine.Persistence;
using WorkflowEngine.Services;
using WorkflowEngine.Validation;

var builder = WebApplication.CreateBuilder(args);

// --- persistence choice ---
// To persist to disk, uncomment below & comment out InMemory line.
// var dataPath = Path.Combine(AppContext.BaseDirectory, "workflow-data.json");
// IWorkflowRepository repo = new JsonFileWorkflowRepository(dataPath);
IWorkflowRepository repo = new InMemoryWorkflowRepository();

builder.Services.AddSingleton(repo);
builder.Services.AddSingleton<WorkflowDefinitionService>();
builder.Services.AddSingleton<WorkflowRuntimeService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// --- Definitions ---
app.MapPost("/definitions", async (WorkflowDefinitionCreateRequest req, WorkflowDefinitionService svc) =>
{
    try
    {
        var def = new WorkflowDefinition(
            req.Id,
            req.Name,
            req.States.Select(s => s.ToDomain()),
            req.Actions.Select(a => a.ToDomain()));

        await svc.CreateOrReplaceAsync(def);
        return Results.Created($"/definitions/{def.Id}", def.ToDto());
    }
    catch (ValidationException vex)
    {
        return ProblemFromValidation(vex);
    }
});

app.MapGet("/definitions", async (WorkflowDefinitionService svc) =>
{
    var defs = await svc.ListAsync();
    return Results.Ok(defs.Select(d => d.ToDto()));
});

app.MapGet("/definitions/{id}", async (string id, WorkflowDefinitionService svc) =>
{
    var def = await svc.GetAsync(id);
    return def is null ? Results.NotFound() : Results.Ok(def.ToDto());
});

// --- Instances ---
app.MapPost("/definitions/{definitionId}/instances", async (string definitionId, WorkflowRuntimeService svc) =>
{
    try
    {
        var inst = await svc.StartInstanceAsync(definitionId);
        return Results.Created($"/instances/{inst.Id}", inst.ToDto());
    }
    catch (ValidationException vex)
    {
        return ProblemFromValidation(vex);
    }
});

app.MapGet("/instances", async (WorkflowRuntimeService svc) =>
{
    var insts = await svc.ListInstancesAsync();
    return Results.Ok(insts.Select(i => i.ToDto()));
});

app.MapGet("/instances/{id}", async (string id, WorkflowRuntimeService svc) =>
{
    var inst = await svc.GetInstanceAsync(id);
    return inst is null ? Results.NotFound() : Results.Ok(inst.ToDto());
});

app.MapPost("/instances/{id}/actions", async (string id, ExecuteActionRequest req, WorkflowRuntimeService svc) =>
{
    try
    {
        var inst = await svc.ExecuteActionAsync(id, req.ActionId);
        return Results.Ok(inst.ToDto());
    }
    catch (ValidationException vex)
    {
        return ProblemFromValidation(vex);
    }
});

app.Run();

// --- helper: central error formatting ---
static IResult ProblemFromValidation(ValidationException vex)
{
    var pd = new ProblemDetails
    {
        Status = (int)HttpStatusCode.BadRequest,
        Title = vex.Message,
        Detail = string.Join("; ", vex.Errors.Select(e => $"{e.Code}:{e.Message}"))
    };
    pd.Extensions["errors"] = vex.Errors;
    return Results.Problem(pd.Detail, statusCode: pd.Status, title: pd.Title, extensions: pd.Extensions);
}

