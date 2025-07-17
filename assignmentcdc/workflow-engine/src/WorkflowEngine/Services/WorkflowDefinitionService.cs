using WorkflowEngine.Domain;
using WorkflowEngine.Persistence;
using WorkflowEngine.Validation;

namespace WorkflowEngine.Services;

public sealed class WorkflowDefinitionService
{
    private readonly IWorkflowRepository _repo;
    public WorkflowDefinitionService(IWorkflowRepository repo) => _repo = repo;

    public async Task<WorkflowDefinition> CreateOrReplaceAsync(WorkflowDefinition def, CancellationToken ct = default)
    {
        DefinitionValidator.Validate(def); // throws if invalid
        await _repo.SaveDefinitionAsync(def, ct);
        return def;
    }

    public Task<WorkflowDefinition?> GetAsync(string id, CancellationToken ct = default) =>
        _repo.GetDefinitionAsync(id, ct);

    public Task<IReadOnlyCollection<WorkflowDefinition>> ListAsync(CancellationToken ct = default) =>
        _repo.ListDefinitionsAsync(ct);
}
