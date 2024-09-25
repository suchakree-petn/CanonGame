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
    [FoldoutGroup("Alive Enemy", true)] public List<Transform> AliveEnemy = new();
    [FoldoutGroup("Alive Enemy", true), ReadOnly]
    public Transform ClosestAliveEnemy
    {
        get
        {
            float closestDistance = Mathf.Infinity;
            Transform closestAliveEnemy = AliveEnemy[0];
            Vector3 canonPos = CanonController.Instance.transform.position;
            foreach (var enemy in AliveEnemy)
            {
                float distance = Vector3.Distance(enemy.position, canonPos);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestAliveEnemy = enemy;
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
        if(EditorApplication.isPlaying) return;

        // GameManager.Instance.OnStartEnemyTurn -= CameraManager.Instance.ShowAllEnemy;

    }


    public void Spawn(ulong id, Vector3 position = default)
    {

    }




}
