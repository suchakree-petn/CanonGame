using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Sirenix.OdinInspector;
using FIMSpace.FProceduralAnimation;

public class EnemyController : MonoBehaviour, IDamageable
{
    public Action OnEnemyDead_Local;
    public Action OnEnemyHit_Local;
    public Action OnEnemyAttack_Local;

    [FoldoutGroup("Config")] public EnemyCharacterData EnemyCharacterData;
    [FoldoutGroup("Config")] public Transform Target;
    [FoldoutGroup("Config")] public bool CanMove = true;
    [FoldoutGroup("Config")] public bool IsTaunted = false;
    [FoldoutGroup("Config")] public bool IsStun = false;
    [FoldoutGroup("Config")] public bool IsDead => enemyHealth.IsDead;

    [FoldoutGroup("AI")][InlineEditor, SerializeField] protected NavMeshAgent agent;
    [FoldoutGroup("AI")][SerializeField] protected NavMeshPath path;

    [FoldoutGroup("Reference")][InlineEditor, SerializeField] protected EnemyHealth enemyHealth;
    [FoldoutGroup("Reference")][InlineEditor, SerializeField] protected Rigidbody enemyRb;
    [FoldoutGroup("Reference")][InlineEditor, SerializeField] protected Animator animator;
    [FoldoutGroup("Reference")][InlineEditor, SerializeField] protected LegsAnimator legsAnimator;

    [FoldoutGroup("Collider")][InlineEditor, SerializeField] protected Collider hitBox;
    [FoldoutGroup("Collider")][InlineEditor, SerializeField] protected Collider collideHitBox;

    [FoldoutGroup("Mesh & Material")][InlineEditor, SerializeField] protected Renderer mesh;
    [FoldoutGroup("Mesh & Material")][InlineEditor, SerializeField] protected Transform mesh_parent;
    [FoldoutGroup("Mesh & Material")][ReadOnly, SerializeField] protected Material dissolveMaterial;
    protected OutlineController outlineController;

    protected virtual void Awake()
    {
        dissolveMaterial = mesh.material;
        outlineController = GetComponent<OutlineController>();
    }

    protected virtual void Start()
    {
        // OnEnemyDead_Local += () => enemyHealth.GetEnemyHealth_UI().gameObject.SetActive(false);
        OnEnemyDead_Local += () => collideHitBox.enabled = false;
        OnEnemyDead_Local += outlineController.HideOutline;
        OnEnemyHit_Local += OnEnemyHit_Shaking;

        outlineController.HideOutline();

        Target = CanonController.Instance.transform;

    }




    protected virtual void Update()
    {


    }

    protected virtual void FixedUpdate()
    {



    }


    protected virtual void LateUpdate()
    {
        // Vector3 direction = agent.velocity.normalized;
        // if (agent.isStopped)
        // {
        //     direction = (Target.position - transform.position).normalized;
        // }
        // else if (direction != Vector3.zero && CanMove)
        // {
        //     Vector3 forward = enemyRb.transform.forward;
        //     enemyRb.transform.forward = new(forward.x, direction.y, forward.z);
        // }
    }

    protected virtual void OnEnable()
    {
        GameManager.Instance.OnStartEnemyTurn += MoveToPlayer;
    }
    protected virtual void OnDisable()
    {
        if (GameManager.Instance)
        {

            GameManager.Instance.OnStartEnemyTurn -= MoveToPlayer;
        }

    }

    public void AnimationEvent_OnEnemyAttackHandler()
    {
        OnEnemyAttack_Local?.Invoke();
    }

    public void OnEnemyDead_Dissolve()
    {
        mesh.gameObject.layer = 0;
        dissolveMaterial.DOFloat(1, "_Dissolve", 2).SetEase(Ease.OutSine);
    }


    public virtual void OnEnemyHit_Shaking()
    {
        // Sequence sequence = DOTween.Sequence();
        // sequence.Append(mesh_parent.DOShakePosition(0.2f, strength: 0.5f, vibrato: 50));
        // sequence.Play();

        LegsAnimator.PelvisImpulseSettings pelvisImpulseSettings = new(new(0, -1f, -1f), 0.2f, 0.1f);
        legsAnimator.User_AddImpulse(pelvisImpulseSettings);
    }

    public void TakeDamage(float damage)
    {

        if (enemyHealth.IsDead)
        {
            StopMoving();
            hitBox.enabled = false;
            OnEnemyDead_Local?.Invoke();
            OnEnemyDead_Local = null;
        }
        else
        {
            enemyHealth.TakeDamage(damage);
            OnEnemyHit_Local?.Invoke();
        }
    }


    public virtual void MoveToPlayer()
    {
        Command moveToPlayerCommand = new EnemyMoveToPlayerCommand(Target, agent, EnemyManager.Instance.MoveToPlayerDuration);
        GameManager.Instance.AddCommand(moveToPlayerCommand);
    }



    public void StopMoving()
    {
        CanMove = false;
        agent.isStopped = true;
        animator.SetFloat("VelocityZ", 0);

    }

    public void Moving()
    {
        CanMove = true;
        agent.isStopped = false;
    }

    public virtual bool CanAttack()
    {
        float distanceToPlayer = Vector3.Distance(Target.position, transform.position);
        return distanceToPlayer <= EnemyCharacterData.AttackRange + agent.stoppingDistance;
    }

    public IEnumerator DelayDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
