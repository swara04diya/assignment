namespace WorkflowEngine.ApiModels;

public sealed record ActionDto(
    string Id,
    string Name,
    List<string> FromStates,
    string ToState,
    bool Enabled,
    string? Description
);
