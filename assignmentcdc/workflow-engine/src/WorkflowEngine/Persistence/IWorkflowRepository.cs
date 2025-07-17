using WorkflowEngine.Domain;

namespace WorkflowEngine.Persistence;

/// <summary>
/// Abstraction over storage so we can swap in-memory, file, database later.
/// </summary>
public interface IWorkflowRepository
{
    // Definitions
    Task<WorkflowDefinition?> GetDefinitionAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyCollection<WorkflowDefinition>> ListDefinitionsAsync(CancellationToken ct = default);
    Task SaveDefinitionAsync(WorkflowDefinition def, CancellationToken ct = default);

    // Instances
    Task<WorkflowInstance?> GetInstanceAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyCollection<WorkflowInstance>> ListInstancesAsync(CancellationToken ct = default);
    Task SaveInstanceAsync(WorkflowInstance instance, CancellationToken ct = default);
}
