using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyManager : SerializedSingleton<EnemyManager>
{
    public Dictionary<ulong, Transform> EnemyCharacterPrefab;


    [FoldoutGroup("Config", true)] public float MoveToPlayerDuration = 2;
    [FoldoutGroup("Alive Enemy", true)] public Dictionary<int, EnemyController> AliveEnemy = new();
    [FoldoutGroup("Alive Enemy", true)]
    public List<EnemyController> OrderedEnemy
    {
        get
        {
            return ReorderEnemies();
        }
    }

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


    [Button]
    private List<EnemyController> ReorderEnemies()
    {
        if (AliveEnemy == null || AliveEnemy.Count == 0) return null;

        List<EnemyController> enemyControllers = AliveEnemy.Values.ToList();
        enemyControllers.Sort((a, b) => a.DistanceToTarget.CompareTo(b.DistanceToTarget));

        return enemyControllers;

    }

    public int GetPiority(EnemyController enemyController)
    {
        if(!enemyController) return int.MaxValue;

        return OrderedEnemy.IndexOf(enemyController);
    }


}
