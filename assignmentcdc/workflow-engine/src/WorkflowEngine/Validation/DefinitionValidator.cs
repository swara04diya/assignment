using WorkflowEngine.Domain;

namespace WorkflowEngine.Validation;

public static class DefinitionValidator
{
    public static void Validate(WorkflowDefinition def)
    {
        var errors = new List<ValidationError>();

        // exactly one initial
        var initials = def.States.Values.Where(s => s.IsInitial).ToList();
        if (initials.Count == 0)
            errors.Add(new("MissingInitial", "Definition must have exactly one initial state."));
        else if (initials.Count > 1)
            errors.Add(new("MultipleInitial", "Definition has more than one initial state."));

        // validate state references in actions
        foreach (var action in def.Actions.Values)
        {
            foreach (var fs in action.FromStates)
            {
                if (!def.States.ContainsKey(fs))
                    errors.Add(new("UnknownFromState", $"Action '{action.Id}' references unknown fromState '{fs}'."));
            }
            if (!def.States.ContainsKey(action.ToState))
                errors.Add(new("UnknownToState", $"Action '{action.Id}' references unknown toState '{action.ToState}'."));
        }

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
