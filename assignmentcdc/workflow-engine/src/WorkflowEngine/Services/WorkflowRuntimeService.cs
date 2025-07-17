using WorkflowEngine.Domain;
using WorkflowEngine.Persistence;
using WorkflowEngine.Validation;

namespace WorkflowEngine.Services;

public sealed class WorkflowRuntimeService
{
    private readonly IWorkflowRepository _repo;
    public WorkflowRuntimeService(IWorkflowRepository repo) => _repo = repo;

    public async Task<WorkflowInstance> StartInstanceAsync(
        string definitionId,
        string? instanceId = null,
        CancellationToken ct = default)
    {
        var def = await _repo.GetDefinitionAsync(definitionId, ct) ??
            throw new ValidationException(new[] {
                new ValidationError("DefinitionNotFound", $"Definition '{definitionId}' not found.")
            });

        var init = def.InitialState ?? throw new ValidationException(new[] {
            new ValidationError("NoInitial", "Definition missing initial state.")
        });

        var id = instanceId ?? Guid.NewGuid().ToString("n");
        var inst = new WorkflowInstance(id, def.Id, init.Id);

        if (init.IsFinal) inst.MarkCompleted();

        await _repo.SaveInstanceAsync(inst, ct);
        return inst;
    }

    public Task<WorkflowInstance?> GetInstanceAsync(string id, CancellationToken ct = default) =>
        _repo.GetInstanceAsync(id, ct);

    public Task<IReadOnlyCollection<WorkflowInstance>> ListInstancesAsync(CancellationToken ct = default) =>
        _repo.ListInstancesAsync(ct);

    public async Task<WorkflowInstance> ExecuteActionAsync(
        string instanceId,
        string actionId,
        CancellationToken ct = default)
    {
        var inst = await _repo.GetInstanceAsync(instanceId, ct) ??
            throw new ValidationException(new[] {
                new ValidationError("InstanceNotFound", $"Instance '{instanceId}' not found.")
            });

        var def = await _repo.GetDefinitionAsync(inst.DefinitionId, ct) ??
            throw new ValidationException(new[] {
                new ValidationError("DefinitionNotFound", $"Definition '{inst.DefinitionId}' missing.")
            });

        var currentState = def.TryGetState(inst.CurrentStateId) ??
            throw new ValidationException(new[] {
                new ValidationError("StateNotFound", $"Current state '{inst.CurrentStateId}' missing in def '{def.Id}'.")
            });

        if (currentState.IsFinal)
            throw new ValidationException(new[] {
                new ValidationError("FinalState", "Cannot execute actions from a final state.")
            });

        var action = def.TryGetAction(actionId) ??
            throw new ValidationException(new[] {
                new ValidationError("ActionNotFound", $"Action '{actionId}' not in def '{def.Id}'.")
            });

        if (!action.Enabled)
            throw new ValidationException(new[] {
                new ValidationError("ActionDisabled", $"Action '{actionId}' is disabled.")
            });

        if (!action.FromStates.Contains(currentState.Id, StringComparer.OrdinalIgnoreCase))
            throw new ValidationException(new[] {
                new ValidationError("InvalidSource", $"Action '{actionId}' not valid from state '{currentState.Id}'.")
            });

        var toState = def.TryGetState(action.ToState) ??
            throw new ValidationException(new[] {
                new ValidationError("UnknownTarget", $"Action '{actionId}' targets unknown state '{action.ToState}'.")
            });

        var from = inst.CurrentStateId;
        inst.ApplyTransition(action.Id, from, toState.Id);
        if (toState.IsFinal) inst.MarkCompleted();

        await _repo.SaveInstanceAsync(inst, ct);
        return inst;
    }
}
