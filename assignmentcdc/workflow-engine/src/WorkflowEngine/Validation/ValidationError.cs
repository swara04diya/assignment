namespace WorkflowEngine.Validation;

/// <summary>Structured validation failure.</summary>
public sealed record ValidationError(string Code, string Message);
