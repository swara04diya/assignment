using WorkflowEngine.Domain;
using WorkflowEngine.ApiModels;

namespace WorkflowEngine.Services;

public static class MappingExtensions
{
    // Domain -> DTO
    public static StateDto ToDto(this State s) => new(
        s.Id, s.Name, s.IsInitial, s.IsFinal, s.Enabled, s.Description);

    public static ActionDto ToDto(this WorkflowAction a) => new(
        a.Id, a.Name, a.FromStates.ToList(), a.ToState, a.Enabled, a.Description);

    public static WorkflowDefinitionResponse ToDto(this WorkflowDefinition d) => new(
        d.Id,
        d.Name,
        d.States.Values.Select(ToDto).ToList(),
        d.Actions.Values.Select(ToDto).ToList());

    public static WorkflowInstanceResponse ToDto(this WorkflowInstance i) => new(
        i.Id,
        i.DefinitionId,
        i.CurrentStateId,
        i.Completed,
        i.History.Select(h =>
            new InstanceHistoryItemDto(h.Timestamp, h.ActionId, h.FromStateId, h.ToStateId)).ToList());

    // DTO -> Domain
    public static State ToDomain(this StateDto dto) => new(
        dto.Id,
        dto.Name,
        dto.IsInitial,
        dto.IsFinal,
        dto.Enabled,
        dto.Description);

    public static WorkflowAction ToDomain(this ActionDto dto) => new(
        dto.Id,
        dto.Name,
        dto.FromStates,
        dto.ToState,
        dto.Enabled,
        dto.Description);
}
