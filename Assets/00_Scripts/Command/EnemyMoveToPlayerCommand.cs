// using System;
// using DG.Tweening;
// using UnityEngine;
// using UnityEngine.AI;

// public class EnemyMoveToPlayerCommand : Command
// {
//     Transform playerTransform;
//     NavMeshAgent agent;
//     float duration;
//     public EnemyMoveToPlayerCommand(Transform playerTransform, NavMeshAgent agent, float duration)
//     {
//         this.duration = duration;
//         this.playerTransform = playerTransform;
//         this.agent = agent;
//     }

//     public override void Execute()
//     {
//         if (!playerTransform) return;


//         if (agent.remainingDistance > 0)
//         {

//             Sequence sequence = DOTween.Sequence();
//             sequence.AppendCallback(() =>
//             {
//                 agent.isStopped = false;

//                 agent.SetDestination(playerTransform.position);

//             });
//             sequence.AppendInterval(duration)
//                 .OnUpdate(() =>
//                 {
//                     if (agent.remainingDistance <= 0)
//                     {
//                         agent.isStopped = true;
//                         agent.ResetPath();
//                         base.Execute();
//                         Debug.Log("Arrive");
//                         sequence.Kill();
//                     }
//                 })
//                 .OnComplete(() =>
//                 {
//                     base.Execute();
//                     agent.isStopped = true;
//                     agent.ResetPath();
//                 });
//             sequence.Play();
//         }

//     }

// }
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

    public EnemyMoveToPlayerCommand(Transform playerTransform, NavMeshAgent agent, float duration,Func<bool> stopCondition)
    {
        this.duration = duration;
        this.playerTransform = playerTransform;
        this.agent = agent;
        this.stopCondition = stopCondition;
    }

    public override void Execute()
    {
        if (!playerTransform) return;

        // Set the destination to the player's position
        agent.isStopped = false;
        NavMeshPath path = new();
        agent.CalculatePath(playerTransform.position,path);
        agent.SetPath(path);
        // agent.SetDestination(playerTransform.position);
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