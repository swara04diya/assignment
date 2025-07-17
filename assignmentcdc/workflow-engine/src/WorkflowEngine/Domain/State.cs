namespace WorkflowEngine.Domain;

/// <summary>
/// A named step in a workflow. Exactly one state in a definition must be marked IsInitial.
/// Multiple IsFinal states are allowed.
/// </summary>
public sealed record State(
    string Id,
    string Name,
    bool IsInitial = false,
    bool IsFinal = false,
    bool Enabled = true,
    string? Description = null
);
