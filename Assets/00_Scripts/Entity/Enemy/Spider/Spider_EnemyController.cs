using System.Collections;
using FIMSpace.FProceduralAnimation;
using Sirenix.OdinInspector;
using UnityEngine;
public class Spider_EnemyController : EnemyController
{

    [SerializeField] private float attackPower;
    [SerializeField] private float attackPower_Multiplier;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackTimeInterval;
    [SerializeField] private bool isReadyToAttack;
    [SerializeField] private bool isFinishAttack;


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
        // if (Vector3.Distance(transform.position, Target.position) > attackRange + 2 && isFinishAttack && CanMove)
        // {
        //     agent.isStopped = false;
        //     animator.SetFloat("VelocityZ", Mathf.Lerp(animator.GetFloat("VelocityZ"), 1, Time.deltaTime * 5));
        // }
        // else
        // {
        //     agent.isStopped = true;

        //     if (!CanMove) return;
        //     // Attack
        //     animator.SetFloat("VelocityZ", Mathf.Lerp(animator.GetFloat("VelocityZ"), 0, Time.deltaTime * 10));
        //     if (!isReadyToAttack || IsStun) return;
        //     animator.SetTrigger("Attack");

        // }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.OnStartEnemyTurn += AttackCommand;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (GameManager.Instance)
        {
            GameManager.Instance.OnStartEnemyTurn -= AttackCommand;

        }

    }

    private void NormalAttack()
    {
        RaycastHit[] hits = Physics.SphereCastAll(attackPointTransform.position, attackRange, attackPointTransform.forward, attackRange, EnemyCharacterData.TargetLayer);
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
        animator.SetTrigger("Attack");

    }

    // public override void MoveToPlayer()
    // {
    //     base.MoveToPlayer();

    // }

    public void AttackCommand()
    {

        Command attackCommand = new EnemyAttackCommand(PlayAttackAnimation, attackClip.length, CanAttack);
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
