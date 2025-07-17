namespace WorkflowEngine.ApiModels;

/// <summary>Definition returned to clients.</summary>
public sealed record WorkflowDefinitionResponse(
    string Id,
    string Name,
    List<StateDto> States,
    List<ActionDto> Actions
);
