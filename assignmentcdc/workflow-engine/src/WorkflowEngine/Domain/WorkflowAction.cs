namespace WorkflowEngine.Domain;

/// <summary>
/// Transition that can move an instance from one of a set of allowed states to a target state.
/// </summary>
public sealed record WorkflowAction(
    string Id,
    string Name,
    IReadOnlyCollection<string> FromStates,
    string ToState,
    bool Enabled = true,
    string? Description = null
);
