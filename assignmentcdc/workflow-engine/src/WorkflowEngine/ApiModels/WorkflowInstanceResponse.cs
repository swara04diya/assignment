namespace WorkflowEngine.ApiModels;

public sealed record InstanceHistoryItemDto(
    DateTimeOffset Timestamp,
    string ActionId,
    string FromStateId,
    string ToStateId
);

public sealed record WorkflowInstanceResponse(
    string Id,
    string DefinitionId,
    string CurrentStateId,
    bool Completed,
    List<InstanceHistoryItemDto> History
);
