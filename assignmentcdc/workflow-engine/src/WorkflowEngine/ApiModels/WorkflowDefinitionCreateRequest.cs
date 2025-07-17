namespace WorkflowEngine.ApiModels;

/// <summary>Payload used to create/replace a workflow definition.</summary>
public sealed record WorkflowDefinitionCreateRequest(
    string Id,
    string Name,
    List<StateDto> States,
    List<ActionDto> Actions
);
