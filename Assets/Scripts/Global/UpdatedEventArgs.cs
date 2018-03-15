using System;

public class UpdateEventArgs<T> : EventArgs
{
    public T OldValue { get; }
    public T NewValue { get; }

    public UpdateEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
