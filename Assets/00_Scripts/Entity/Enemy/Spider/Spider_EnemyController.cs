using System.Collections;
using DG.Tweening;
using FIMSpace.FProceduralAnimation;
using Sirenix.OdinInspector;
using UnityEngine;
public class Spider_EnemyController : EnemyController
{

    [FoldoutGroup("Spider Reference"), Required]
    [SerializeField] private Transform attackPointTransform;
    [FoldoutGroup("Spider Reference"), Required]
    [SerializeField] private AnimationClip attackClip;

    protected override void Start()
    {
        base.Start();
        // OnEnemyAttack_Local += NormalAttack;
        OnEnemyHit_Local += OnEnemyHit_HitAnimation;


    }

    protected override void Update()
    {
        base.Update();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.OnStartEnemyTurn += AttackCommand;
        OnEnemyDead_Local += () => GameManager.Instance.OnStartEnemyTurn -= AttackCommand;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (GameManager.Instance)
        {

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPointTransform.position, EnemyCharacterData.AttackRange + agent.stoppingDistance);
    }

    private void NormalAttack()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            attackPointTransform.position,
            EnemyCharacterData.AttackRange + agent.stoppingDistance,
            attackPointTransform.forward,
            EnemyCharacterData.AttackRange + agent.stoppingDistance,
            EnemyCharacterData.TargetLayer);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.root.TryGetComponent(out IDamageable damageable) && hit.collider.isTrigger)
            {
                Debug.Log("Spider hit " + hit.collider.transform.root.name);
                damageable.TakeDamage(EnemyCharacterData.Attack);
            }
        }
    }

    private void PlayAttackAnimation()
    {
        transform.DOLookAt(Target.position, 0.3f);
        animator.SetTrigger("Attack");

    }

    // public override void MoveToPlayer()
    // {
    //     base.MoveToPlayer();

    // }

    public override bool CanAttack()
    {
        return base.CanAttack();
    }

    public void AttackCommand()
    {

        Command attackCommand = new EnemyAttackCommand(PlayAttackAnimation, attackClip.length, CanAttack, Piority);
        GameManager.Instance.AddCommand(attackCommand);
    }


    private void OnEnemyHit_HitAnimation()
    {
        // animator.SetTrigger("Hit");
    }

    public override void OnEnemyHit_Shaking()
    {
        LegsAnimator.PelvisImpulseSettings pelvisImpulseSettings = new(new(0, -1f, -1f), 0.2f, 0.4f);
        legsAnimator.User_AddImpulse(pelvisImpulseSettings);
    }
}
