using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ExplosiveCanonBallController : CanonBallController
{

    [FoldoutGroup("Canon Ball Config/Animation", true), SerializeField]
    float flickeringDuration = 0.2f;

    [FoldoutGroup("Canon Ball Config/Animation", true), SerializeField]
    float delayExplode = 1;
    [FoldoutGroup("Canon Ball Config/Animation", true), SerializeField]
    float chargeExplode = 2;


    [FoldoutGroup("Reference", true), Required, SerializeField]

    Transform explosion_prf;

    Sequence explosionSequence;
    Sequence immedieatlyExplosionSequence;

    ParticleSystem explodeParticle;

    bool _isExploded;


    protected override void Awake()
    {
        base.Awake();
        explodeParticle = explosion_prf.GetComponent<ParticleSystem>();
    }
    protected override void Start()
    {
        base.Start();


        explosionSequence = DOTween.Sequence();
        explosionSequence.AppendCallback(() =>
        {
            _isExploded = true;
        });
        explosionSequence.AppendInterval(delayExplode);
        explosionSequence.Append(materials.DOFloat(5, "_Power", 0.2f));
        explosionSequence.Append(materials.DOFloat(0, "_Power", chargeExplode));
        explosionSequence.AppendCallback(() =>
        {
            canonBallModel.gameObject.SetActive(false);
            Instantiate(explosion_prf, transform);

            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, CanonBallData.ExplosionRange, Vector3.up, CanonBallData.ExplosionRange, DamageMask);
            foreach (var hit in raycastHits)
            {
                IDamageable _damageable = hit.transform.GetComponentInParent<IDamageable>();
                if (_damageable != null)
                {
                    _damageable.TakeDamage(CanonBallData.ExplosionDamage);
                    KnockBack(hit.rigidbody);
                    IsDestroy = true;
                    explosionSequence.Kill();
                }
            }

        });

        float explosionParticleDuration = explodeParticle.main.duration;
        explosionSequence.AppendInterval(explosionParticleDuration);
        explosionSequence.OnComplete(DestroyCanonBall);
        explosionSequence.Pause();


        immedieatlyExplosionSequence = DOTween.Sequence();
        immedieatlyExplosionSequence.AppendCallback(() =>
        {
            _isExploded = true;
            Rigidbody.velocity = Vector3.zero;
            transform.up = Vector3.up;
            Rigidbody.freezeRotation = true;
        });
        immedieatlyExplosionSequence.Append(materials.DOFloat(5, "_Power", 0f));
        immedieatlyExplosionSequence.Append(materials.DOFloat(0, "_Power", 0.1f));
        immedieatlyExplosionSequence.AppendCallback(() =>
        {
            canonBallModel.gameObject.SetActive(false);
            Instantiate(explosion_prf, transform);

            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, CanonBallData.ExplosionRange, Vector3.up, CanonBallData.ExplosionRange, DamageMask);
            foreach (var hit in raycastHits)
            {
                IDamageable _damageable = hit.transform.GetComponentInParent<IDamageable>();
                if (_damageable != null)
                {
                    _damageable.TakeDamage(CanonBallData.ExplosionDamage);
                    KnockBack(hit.rigidbody);
                    IsDestroy = true;
                    immedieatlyExplosionSequence.Kill();
                }
            }

        });

        float immedieatlyExplosionDuration = explodeParticle.main.duration;
        immedieatlyExplosionSequence.AppendInterval(immedieatlyExplosionDuration);
        immedieatlyExplosionSequence.OnComplete(DestroyCanonBall);
        immedieatlyExplosionSequence.Pause();
    }

    public override void Init()
    {
        base.Init();

        materials.SetFloat("_Power", 5);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(materials.DOFloat(2, "_Power", flickeringDuration));
        sequence.Append(materials.DOFloat(5, "_Power", flickeringDuration));

        sequence.SetLoops(-1);

        sequence.Play();
    }

    public override void DealDamage(IDamageable damageable)
    {
        if (IsDestroy) return;

        materials.DOKill(true);

        if (!_isExploded)
            immedieatlyExplosionSequence.Play();
    }

    public override void DestroyCanonBall()
    {
        canonBallModel.DOKill();

        if (!_isExploded)
        {
            explosionSequence.Play();

        }
        else
        {
        }
            Destroy(gameObject, explosionSequence.Duration());

    }

    void KnockBack(Rigidbody target)
    {
        if (!target) return;
        Vector3 dir = (target.position - transform.position).normalized;
        target.AddForce(dir * CanonBallData.ExplosionKnockBack, ForceMode.Impulse);
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, CanonBallData.ExplosionRange);
    }
}
