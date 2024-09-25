using Sirenix.OdinInspector;
using UnityEngine;

[InlineEditor]
public abstract class EntityCharacterData : ScriptableObject
{
    [Title("Info")]
    public string Name;
    public int Level;
    public LayerMask TargetLayer;

    [Title("Movement")]
    public float MoveSpeed;


    [Title("Health Point")]
    public float Health;


    [Title("Attack Point")]
    public float Attack;

}