using System.Collections.Concurrent;
using WorkflowEngine.Domain;

namespace WorkflowEngine.Persistence;

/// <summary>
/// Simple thread-safe in-memory storage. Data lost on app restart.
/// </summary>
public sealed class InMemoryWorkflowRepository : IWorkflowRepository
{
    private readonly ConcurrentDictionary<string, WorkflowDefinition> _defs =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, WorkflowInstance> _inst =
        new(StringComparer.OrdinalIgnoreCase);

    public Task<WorkflowDefinition?> GetDefinitionAsync(string id, CancellationToken ct = default) =>
        Task.FromResult(_defs.TryGetValue(id, out var d) ? d : null);

    public Task<IReadOnlyCollection<WorkflowDefinition>> ListDefinitionsAsync(CancellationToken ct = default) =>
        Task.FromResult((IReadOnlyCollection<WorkflowDefinition>)_defs.Values.ToList());

    public Task SaveDefinitionAsync(WorkflowDefinition def, CancellationToken ct = default)
    {
        _defs[def.Id] = def;
        return Task.CompletedTask;
    }

    public Task<WorkflowInstance?> GetInstanceAsync(string id, CancellationToken ct = default) =>
        Task.FromResult(_inst.TryGetValue(id, out var i) ? i : null);

    public Task<IReadOnlyCollection<WorkflowInstance>> ListInstancesAsync(CancellationToken ct = default) =>
        Task.FromResult((IReadOnlyCollection<WorkflowInstance>)_inst.Values.ToList());

    public Task SaveInstanceAsync(WorkflowInstance instance, CancellationToken ct = default)
    {
        _inst[instance.Id] = instance;
        return Task.CompletedTask;
    }
}
