namespace WorkflowEngine.ApiModels;

public sealed record StateDto(
    string Id,
    string Name,
    bool IsInitial,
    bool IsFinal,
    bool Enabled,
    string? Description
);
