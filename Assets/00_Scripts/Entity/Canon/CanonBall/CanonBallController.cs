using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CanonBallController : MonoBehaviour
{

    [FoldoutGroup("Canon Ball Config", true), Required, InlineEditor]
    public CanonBallData CanonBallData;

    [FoldoutGroup("Canon Ball Config", true)]
    public bool IsDestroy = false;
    [FoldoutGroup("Canon Ball Config", true)]
    public bool IsInit = false;


    [FoldoutGroup("Canon Ball Config/Physic", true)]
    public float CanonBallGravityMultiplier = 10;

    [FoldoutGroup("Canon Ball Config/Physic", true)]
    public LayerMask CollisionMask;


    [FoldoutGroup("Canon Ball Config/Damage", true)]
    public LayerMask DamageMask;

    [FoldoutGroup("Canon Ball Config/Damage", true)]
    [SerializeField] protected float minimumSpeedToDealDamage = 5;

    [FoldoutGroup("Canon Ball Config/Damage", true)]
    public float AoeRange = 1;


    [FoldoutGroup("Canon Ball Config/Animation", true)]
    [SerializeField] protected float rotateSpeed = 10;

    [FoldoutGroup("Canon Ball Config/Animation", true)]
    [SerializeField] protected float delayDissolve = 1;

    [FoldoutGroup("Canon Ball Config/Animation", true)]
    [SerializeField] protected float dissolveDuration = 3;


    [FoldoutGroup("Reference", true), Required]
    [SerializeField] protected Transform canonBallModel;

    [FoldoutGroup("Reference", true), Required]
    [SerializeField] protected Rigidbody _rigidbody;

    [FoldoutGroup("Reference", true), Required]
    [SerializeField] protected Renderer meshRenderer;
    protected Material _material;

    [FoldoutGroup("Reference", true)]
    [SerializeField] protected List<ParticleSystem> destroyParticle = new();

    [FoldoutGroup("Reference", true), Required]
    [SerializeField] protected Collider damageCollider;

    [FoldoutGroup("Reference", true), Required]
    [SerializeField] protected Collider collisionCollider;



    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _material = meshRenderer.material;
    }

    protected virtual void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        canonBallModel.DOLocalRotate(new(360, 0, 0), 1 / rotateSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
        IsInit = true;
    }

    void FixedUpdate()
    {
        _rigidbody.AddForce(CanonBallGravityMultiplier * Vector3.down, ForceMode.Force);

    }

    void LateUpdate()
    {

    }

    protected virtual void OnDestroy()
    {
        DOTween.Kill(transform);
    }

    public virtual void DealDamage(IDamageable damageable)
    {
        if (_rigidbody.velocity.magnitude < minimumSpeedToDealDamage)
        {
            damageable.TakeDamage(CanonBallData.Damage * _rigidbody.velocity.magnitude / minimumSpeedToDealDamage);
        }
        else
        {
            damageable.TakeDamage(CanonBallData.Damage);
        }
        damageCollider.enabled = false;
    }

    public virtual void DestroyCanonBall()
    {
        if (IsDestroy) return;

        IsDestroy = true;

        canonBallModel.DOKill();

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(delayDissolve);
        sequence.AppendCallback(() =>
        {
            foreach (ParticleSystem particle in destroyParticle)
            {
                particle.Play();
            }
        });
        sequence.Append(_material.DOFloat(1, "_Dissolve", dissolveDuration));

        sequence.Play();

        Destroy(gameObject, sequence.Duration());

    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, AoeRange);
    }

}
