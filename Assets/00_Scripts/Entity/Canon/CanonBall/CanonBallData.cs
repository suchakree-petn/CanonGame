using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "_CanonBallData", menuName = "Canon Ball Data")]
public class CanonBallData : SerializedScriptableObject
{
    public CanonBallType CanonBallType;
    public float Damage;
    public Transform CanonBall_prf;

    [ShowIf("CanonBallType", CanonBallType.Explosive)]
    public float ExplosionRange;

    [ShowIf("CanonBallType", CanonBallType.Explosive)]
    public float ExplosionKnockBack;

    [ShowIf("CanonBallType", CanonBallType.Explosive)]
    public float ExplosionDamage;
}
