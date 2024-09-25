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

    public EnemyMoveToPlayerCommand(Transform playerTransform, NavMeshAgent agent, float duration)
    {
        this.duration = duration;
        this.playerTransform = playerTransform;
        this.agent = agent;
    }

    public override void Execute()
    {
        if (!playerTransform) return;

        // Set the destination to the player's position
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);

        CameraManager.Instance.CloseUpEnemy(agent.transform);

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(duration)
            .OnUpdate(() =>
            {
                // Check if the agent has arrived at the destination
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        StopMove();
                        sequence.Kill();
                    }
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