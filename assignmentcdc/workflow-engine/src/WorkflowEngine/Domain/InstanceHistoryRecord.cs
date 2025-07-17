namespace WorkflowEngine.Domain;

/// <summary>
/// One applied transition in an instance's history.
/// </summary>
public sealed record InstanceHistoryRecord(
    DateTimeOffset Timestamp,
    string ActionId,
    string FromStateId,
    string ToStateId
);
