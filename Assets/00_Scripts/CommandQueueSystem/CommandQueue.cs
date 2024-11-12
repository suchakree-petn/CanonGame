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
    public List<Command> commandsQueueList = new();

    [BoxGroup("Command")]
    [SerializeField, ReadOnly] private bool isExecuting;

    public bool IsExecuting => isExecuting;

    public CommandQueue()
    {
        commandsQueueList = new();
    }

    public void AddCommand(Command command)
    {
        command.OnComplete(OnFinishExecute);

        commandsQueueList.Add(command);
        // TryExecuteCommands();
        // Debug.Log($"Command queue size: {commandsQueue.Count}");
    }

    public void TryExecuteCommands()
    {
        if (isExecuting) return;
        if (commandsQueueList.Count > 0)
        {
            Debug.Log("Sorting");
            commandsQueueList.Sort((a, b) => a.Piority.CompareTo(b.Piority));

            isExecuting = true;
            // Debug.Log($"Command queue size: {commandsQueue.Count}");
            Command command = commandsQueueList[0];
            commandsQueueList.Remove(command);

            if (commandsQueueList.Count > 0)
            {
                command.OnComplete(TryExecuteCommands);
            }
            else
            {

            }
            if (commandsQueueList.Count == 0)
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
        return commandsQueueList[^1];
    }

    private void LastCommandExecuted()
    {
        Debug.Log("Last Command");
        OnLastCommandExecuted?.Invoke();
    }
}
