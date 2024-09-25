// using System;
// using System.Collections;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.AI;

// [SelectionBase]
// public class RedDragon_Fly_EnemyController : EnemyController
// {
//     public Action OnActiveBoss;

//     [Header("Attack Config")]
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private float attackPower_Multiplier;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private float attackRange;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private float attackCD = 1f;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private float fireBreathAttackDuration = 3f;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private AnimationClip screamClip;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private Transform attackPointTransform;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private ParticleSystem fireBreath_ps;

//     [Header("Moving Config")]
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private Transform takeOffDestinationTransform;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private float takeOffDuration;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private float steeringDuration;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private Transform provokeTarget;
//     [FoldoutGroup("RedDragon_Fly Config")][SerializeField] private bool isReadyToMove = false;


//     protected override void Awake()
//     {
//         base.Awake();

//         agent.enabled = false;
//         enemyRb.useGravity = false;
//         collideHitBox.enabled = false;
//     }

//     protected override void Start()
//     {
//         base.Start();

//         gameObject.SetActive(false);
//         outlineController.HideOutline();

//         OnActiveBoss += () => isReadyToMove = true;
//     }

//     public override void OnNetworkSpawn()
//     {
//         if (!IsOwner || !IsServer) return;
//     }

//     protected override void Update()
//     {
//         if (attackCD > 0)
//         {
//             attackCD -= Time.deltaTime;
//         }
//         else
//         {
//             attackCD = 0;
//         }
//     }

//     protected override void LateUpdate()
//     {
//         if (agent.enabled) return;
//         if (!IsOwner || !IsServer || !IsSpawned) return;

//         if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < 0.1f)
//         {
//             if (provokeTarget.TryGetComponent(out HornController hornController))
//             {
//                 AttackTarget(hornController);

//             }
//             if (provokeTarget.TryGetComponent(out Broken_BalistaController broken_BalistaController))
//             {
//                 AttackTarget(broken_BalistaController);
//             }
//         }
//     }

//     private void OnValidate()
//     {
// #if UNITY_EDITOR


// #endif
//     }

//     public void NormalAttack()
//     {
//         RaycastHit[] hits = Physics.SphereCastAll(attackPointTransform.position, attackRange, attackPointTransform.forward, attackRange, EnemyCharacterData.TargetLayer);
//         foreach (RaycastHit hit in hits)
//         {
//             if (hit.collider.transform.root.TryGetComponent(out PlayerController playerController) && hit.collider.isTrigger)
//             {
//                 AttackDamage attackDamage = new(attackPower_Multiplier, EnemyCharacterData.AttackBase, DamageType.Melee, false);
//                 playerController.GetComponent<IDamageable>().TakeDamage_ClientRpc(attackDamage);
//             }
//         }

//     }

//     public Tween TakeOffTo(Vector3 destination, float duration)
//     {
//         animator.SetBool("TakeOff", true);
//         return transform.DOMove(destination, duration)
//             .SetUpdate(UpdateType.Fixed)
//             .OnComplete(() => animator.SetBool("TakeOff", false));
//     }

//     public Tween LandingTo(Vector3 destination, float duration)
//     {
//         animator.SetBool("Landing", true);
//         return transform.DOMove(destination, duration)
//             .SetUpdate(UpdateType.Fixed)
//             .OnComplete(() => animator.SetBool("Landing", false));
//     }

//     public void MovingTo(Transform provokeTarget)
//     {
//         if (!isReadyToMove) return;

//         Vector3 destination = new(provokeTarget.position.x, transform.localPosition.y, provokeTarget.position.z);
//         SetAgentDestination(destination);

//         animator.SetBool("Moving", true);

//         this.provokeTarget = provokeTarget;
//     }

//     public void AttackTarget(HornController hornController)
//     {
//         if (attackCD > 0) return;

//         Sequence sequence = DOTween.Sequence();
//         sequence.Append(transform.DOLookAt(hornController.transform.position, steeringDuration));

//         sequence.AppendCallback(() =>
//         {
//             animator.SetBool("FlyFlameAttack", true);
//             StartFireBreathParticle();
//             fireBreath_ps.Play();

//         });

//         sequence.AppendInterval(fireBreathAttackDuration);

//         sequence.AppendCallback(() =>
//         {
//             animator.SetBool("FlyFlameAttack", false);
//             fireBreath_ps.Stop();
//         });

//         sequence.Play();


//     }

//     public void AttackTarget(Broken_BalistaController broken_BalistaController)
//     {
//     }

//     public void StartFireBreathParticle()
//     {
//         // fireBreath_ps.main
//     }

//     private void SetAgentDestination(Vector3 destination)
//     {
//         if(agent.pathStatus == NavMeshPathStatus.PathInvalid) return;
//         agent.ResetPath();

//         path = new();
//         agent.CalculatePath(destination, path);
//         agent.SetPath(path);
//     }


//     public void ActiveBoss()
//     {
//         gameObject.SetActive(true);

//         Sequence sequence = DOTween.Sequence();
//         sequence.AppendInterval(screamClip.length);
//         sequence.AppendCallback(async () =>
//         {
//             legsAnimator.enabled = false;

//             await TakeOffTo(takeOffDestinationTransform.position, takeOffDuration).AsyncWaitForCompletion();
//             OnActiveBoss?.Invoke();
//             agent.enabled = true;
//             enemyRb.useGravity = true;
//             outlineController.ShowOutline();
//             collideHitBox.enabled = true;
//         });
//         sequence.Play();

//     }



//     private void OnDrawGizmos()
//     {
//         if (provokeTarget)
//         {
//             Vector3 dir = (provokeTarget.position - attackPointTransform.position).normalized;
//             Debug.DrawLine(attackPointTransform.position, dir * attackRange);

//         }
//     }

//     protected override void OnEnable()
//     {
//         base.OnEnable();
//     }

//     [ClientRpc]
//     public override void TakeDamage_ClientRpc(AttackDamage damage)
//     {

//     }

//     [ServerRpc(RequireOwnership = false)]
//     public override void TakeDamage_ServerRpc(AttackDamage damage)
//     {

//     }

// }
