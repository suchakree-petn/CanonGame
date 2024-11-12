using System;
using System.Collections.Generic;

public abstract class Command : ICommand
{
    public delegate void OnCompleteCallback();
    private List<OnCompleteCallback> callbacksList = new();
    public int Piority;

    public Command(int piority)
    {
        this.Piority = piority;
    }

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

}
