using System;
using DG.Tweening;
using UnityEngine;

public class EnemyAttackCommand : Command
{
    float duration;
    Action attackAction;
    Func<bool> condition;


    public EnemyAttackCommand(Action attackAction, float duration, Func<bool> condition)
    {
        this.duration = duration;
        this.attackAction = attackAction;
        this.condition = condition;
    }

    public override void Execute()
    {
        if (condition())
        {
            attackAction.Invoke();
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(duration).OnComplete(() =>
            {
                Debug.Log("Finish Attack");
                base.Execute();
            });
        }
        else
        {
            Debug.Log("Cant Attack");
            base.Execute();
        }

    }

}
