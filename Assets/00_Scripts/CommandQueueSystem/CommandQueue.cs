using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class CommandQueue
{
    [BoxGroup("Command")]
    public Action OnLastCommandExecuted;

    [BoxGroup("Command")]
    public Queue<Command> commandsQueue;

    [BoxGroup("Command")]
    public List<Command> commandsQueueList = new();

    [BoxGroup("Command")]
    [SerializeField, ReadOnly] private bool isExecuting;

    public bool IsExecuting => isExecuting;

    public CommandQueue()
    {
        commandsQueue = new();
        commandsQueueList = new();
    }

    public void AddCommand(Command command)
    {
        command.OnComplete(OnFinishExecute);
        commandsQueue.Enqueue(command);

        commandsQueueList.Add(command);
        // TryExecuteCommands();
        // Debug.Log($"Command queue size: {commandsQueue.Count}");
    }

    public void TryExecuteCommands()
    {
        if (isExecuting) return;
        if (commandsQueue.Count > 0)
        {
            isExecuting = true;
            // Debug.Log($"Command queue size: {commandsQueue.Count}");
            Command command = commandsQueue.Dequeue();
            commandsQueueList.Remove(command);

            if (commandsQueue.Count > 0)
            {
                command.OnComplete(TryExecuteCommands);
            }
            else
            {

            }
            if (commandsQueue.Count == 0)
            {
                command.OnComplete(LastCommandExecuted);
            }
            command.Execute();
        }
    }

    public void OnFinishExecute()
    {
        isExecuting = false;
        TryExecuteCommands();
    }

    public Command GetLastCommand()
    {
        return commandsQueue.Peek();
    }

    private void LastCommandExecuted()
    {
        Debug.Log("Last Command");
        OnLastCommandExecuted?.Invoke();
    }
}
