namespace TodoList.Domain.Exceptions;

public class ItemLimitExceededException : DomainException
{
    public int MaxLimit { get; }

    public ItemLimitExceededException(int maxLimit)
        : base($"Cannot add item. List has reached maximum limit of {maxLimit} items.")
    {
        MaxLimit = maxLimit;
    }
}
