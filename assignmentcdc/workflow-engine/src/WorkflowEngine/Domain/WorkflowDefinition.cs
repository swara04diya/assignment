namespace WorkflowEngine.Domain;

/// <summary>
/// Immutable (post-construction) workflow definition: the template from which instances start.
/// </summary>
public sealed class WorkflowDefinition
{
    public string Id { get; }
    public string Name { get; }
    public IReadOnlyDictionary<string, State> States => _states;
    public IReadOnlyDictionary<string, WorkflowAction> Actions => _actions;

    private readonly Dictionary<string, State> _states;
    private readonly Dictionary<string, WorkflowAction> _actions;

    public WorkflowDefinition(
        string id,
        string name,
        IEnumerable<State> states,
        IEnumerable<WorkflowAction> actions)
    {
        Id = id;
        Name = name;
        _states  = states.ToDictionary(s => s.Id, StringComparer.OrdinalIgnoreCase);
        _actions = actions.ToDictionary(a => a.Id, StringComparer.OrdinalIgnoreCase);
    }

    public State? TryGetState(string id) =>
        _states.TryGetValue(id, out var s) ? s : null;

    public WorkflowAction? TryGetAction(string id) =>
        _actions.TryGetValue(id, out var a) ? a : null;

    /// <summary>Returns the single initial state (if valid); null if 0 or >1 flagged.</summary>
    public State? InitialState => _states.Values.SingleOrDefault(s => s.IsInitial);
}
