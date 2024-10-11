using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveToPlayerCommand : Command
{
    Transform playerTransform;
    NavMeshAgent agent;
    float duration;
    Func<bool> stopCondition;

    public EnemyMoveToPlayerCommand(Transform playerTransform, NavMeshAgent agent, float duration, Func<bool> stopCondition, int piority)
    {
        this.duration = duration;
        this.playerTransform = playerTransform;
        this.agent = agent;
        this.stopCondition = stopCondition;
    }

    public override void Execute()
    {
        if (!playerTransform || !agent)
        {
            base.Execute();
            return;
        }

        agent.isStopped = false;
        NavMeshPath path = new();
        agent.CalculatePath(playerTransform.position, path);
        agent.SetPath(path);
        CameraManager.Instance.CloseUpEnemy(agent.transform);

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(duration)
            .OnUpdate(() =>
            {
                if (stopCondition())
                {
                    StopMove();
                    sequence.Kill();
                }
            })
            .OnComplete(() =>
            {
                StopMove();
            });

        sequence.Play();
    }

    void StopMove()
    {
        Debug.Log("stop move");
        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        base.Execute();
    }
}