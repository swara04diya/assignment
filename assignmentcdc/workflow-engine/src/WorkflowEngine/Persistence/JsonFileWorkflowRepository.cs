using System.Text.Json;
using WorkflowEngine.Domain;

namespace WorkflowEngine.Persistence;

/// <summary>
/// In-memory repo + JSON dump to disk on each save. Demo durability only.
/// Not optimized for large data.
/// </summary>
public sealed class JsonFileWorkflowRepository : IWorkflowRepository
{
    private readonly InMemoryWorkflowRepository _inner = new();
    private readonly string _path;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private sealed record RepoSnapshot(
        List<WorkflowDefinition> Definitions,
        List<WorkflowInstance> Instances);

    public JsonFileWorkflowRepository(string path)
    {
        _path = path;
        if (File.Exists(path))
        {
            try
            {
                var txt = File.ReadAllText(path);
                var data = JsonSerializer.Deserialize<RepoSnapshot>(txt, _jsonOptions);
                if (data is not null)
                {
                    foreach (var d in data.Definitions)
                        _ = _inner.SaveDefinitionAsync(d);
                    foreach (var i in data.Instances)
                        _ = _inner.SaveInstanceAsync(i);
                }
            }
            catch
            {
                // swallow for demo — in prod log + handle schema upgrade
            }
        }
    }

    private async Task PersistAsync()
    {
        var snap = new RepoSnapshot(
            (await _inner.ListDefinitionsAsync()).ToList(),
            (await _inner.ListInstancesAsync()).ToList());

        var json = JsonSerializer.Serialize(snap, _jsonOptions);
        await File.WriteAllTextAsync(_path, json);
    }

    public Task<WorkflowDefinition?> GetDefinitionAsync(string id, CancellationToken ct = default) =>
        _inner.GetDefinitionAsync(id, ct);

    public Task<IReadOnlyCollection<WorkflowDefinition>> ListDefinitionsAsync(CancellationToken ct = default) =>
        _inner.ListDefinitionsAsync(ct);

    public async Task SaveDefinitionAsync(WorkflowDefinition def, CancellationToken ct = default)
    {
        await _inner.SaveDefinitionAsync(def, ct);
        await PersistAsync();
    }

    public Task<WorkflowInstance?> GetInstanceAsync(string id, CancellationToken ct = default) =>
        _inner.GetInstanceAsync(id, ct);

    public Task<IReadOnlyCollection<WorkflowInstance>> ListInstancesAsync(CancellationToken ct = default) =>
        _inner.ListInstancesAsync(ct);

    public async Task SaveInstanceAsync(WorkflowInstance instance, CancellationToken ct = default)
    {
        await _inner.SaveInstanceAsync(instance, ct);
        await PersistAsync();
    }
}
