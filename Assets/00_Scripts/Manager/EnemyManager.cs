using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEditor;

public class EnemyManager : SerializedSingleton<EnemyManager>
{
    public Dictionary<ulong, Transform> EnemyCharacterPrefab;


    [FoldoutGroup("Config", true)] public float MoveToPlayerDuration = 2;
    [FoldoutGroup("Alive Enemy", true)] public Dictionary<int, EnemyController> AliveEnemy = new();
    [FoldoutGroup("Alive Enemy", true), ReadOnly]
    public EnemyController ClosestAliveEnemy
    {
        get
        {
            float closestDistance = Mathf.Infinity;
            EnemyController closestAliveEnemy = null;
            Vector3 canonPos = CanonController.Instance.transform.position;
            foreach (var enemy in AliveEnemy)
            {
                float distance = Vector3.Distance(enemy.Value.transform.position, canonPos);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestAliveEnemy = enemy.Value;
                }
            }


            return closestAliveEnemy;
        }
    }


    protected override void InitAfterAwake()
    {
    }

    private void OnEnable()
    {
        // GameManager.Instance.OnStartEnemyTurn += CameraManager.Instance.ShowAllEnemy;
    }

    private void OnDisable()
    {

        // GameManager.Instance.OnStartEnemyTurn -= CameraManager.Instance.ShowAllEnemy;

    }


    public void Spawn(ulong id, Vector3 position = default)
    {

    }

    public int GetPiority()
    {
        return 0;
    }


}
