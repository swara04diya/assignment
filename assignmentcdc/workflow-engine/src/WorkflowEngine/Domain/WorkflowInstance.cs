namespace WorkflowEngine.Domain;

/// <summary>
/// A runtime instance of a workflow definition. Holds current state + history.
/// </summary>
public sealed class WorkflowInstance
{
    public string Id { get; }
    public string DefinitionId { get; }
    public string CurrentStateId { get; private set; }
    public bool Completed { get; private set; }
    public List<InstanceHistoryRecord> History { get; } = new();

    public WorkflowInstance(string id, string definitionId, string initialStateId)
    {
        Id = id;
        DefinitionId = definitionId;
        CurrentStateId = initialStateId;
        Completed = false;
    }

    public void ApplyTransition(string actionId, string fromStateId, string toStateId)
    {
        History.Add(new InstanceHistoryRecord(DateTimeOffset.UtcNow, actionId, fromStateId, toStateId));
        CurrentStateId = toStateId;
    }

    public void MarkCompleted() => Completed = true;
}
