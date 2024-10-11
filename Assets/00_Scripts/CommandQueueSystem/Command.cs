using System;
using System.Collections.Generic;

public abstract class Command : ICommand
{
    public delegate void OnCompleteCallback();
    private List<OnCompleteCallback> callbacksList = new();
    internal string name;

    public int Piority { get => Piority; set => Piority = value; }

    public virtual void Execute()
    {
        foreach (OnCompleteCallback callback in callbacksList)
        {
            callback?.Invoke();
        }
        callbacksList.Clear();
    }
    internal void OnComplete(OnCompleteCallback callback, int callbackOrderIndex = -1)
    {
        if (callbackOrderIndex < 0)
        {
            callbacksList.Add(callback);
        }
        else
        {
            callbacksList.Insert(callbackOrderIndex, callback);
        }
    }

    public int CallbackListCount()
    {
        return callbacksList.Count;
    }

    public virtual string GetCommandName()
    {
        return name;
    }
}
