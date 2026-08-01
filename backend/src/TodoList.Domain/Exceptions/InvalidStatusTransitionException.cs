using TodoList.Domain.Enums;

namespace TodoList.Domain.Exceptions;

public class InvalidStatusTransitionException : DomainException
{
    public Status CurrentStatus { get; }
    public Status TargetStatus { get; }

    public InvalidStatusTransitionException(Status currentStatus, Status targetStatus)
        : base($"Cannot transition status from '{currentStatus}' to '{targetStatus}'.")
    {
        CurrentStatus = currentStatus;
        TargetStatus = targetStatus;
    }
}
