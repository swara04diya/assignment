namespace WorkflowEngine.Validation;

/// <summary>Thrown when one or more validation errors occur.</summary>
public sealed class ValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationError> errors)
        : base("Validation failed")
    {
        Errors = errors.ToList();
    }
}
