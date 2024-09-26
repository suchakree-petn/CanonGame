using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChainCanonBallController : CanonBallController
{
    [FoldoutGroup("Canon Ball Config/Animation", true), SerializeField]
    float spinSpeed = 2f;


    protected override void Start()
    {
        base.Start();
    }


    public override void Init()
    {
        canonBallModel.DORotate(new(0, 360, 0), 1/spinSpeed,RotateMode.FastBeyond360)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }


    public override void DealDamage(IDamageable damageable)
    {
        if (IsDestroy) return;

        materials.DOKill(true);
        materials.DOKill(true);

        damageable.TakeDamage(CanonBallData.Damage);
    }

    public override void DestroyCanonBall()
    {
        base.DestroyCanonBall();

    }
}
