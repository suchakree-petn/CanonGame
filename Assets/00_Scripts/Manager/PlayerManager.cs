using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Tutor;
using System.Linq;
using UnityEditor;

public class PlayerManager : SerializedSingleton<PlayerManager>
{
    [FoldoutGroup("Canon Ball Config", true), Required, SerializeField]
    Dictionary<CanonBallType, CanonBallData> canonBallDict;
    public Dictionary<CanonBallType, CanonBallData> CanonBallDict => canonBallDict;

    [FoldoutGroup("Canon Ball Config", true), Required, SerializeField]
    int canonBallAmount = 3;

    [FoldoutGroup("Canon Ball Config", true), Required, SerializeField]
    Queue<CanonBallType> canonBallPool = new();


    protected override void InitAfterAwake()
    {
    }
    private void Start()
    {
    }


    private void OnEnable()
    {
        GameManager.Instance.OnStartEnemyTurn += ReloadCanonBallPool_Random;
        CameraManager.Instance.OnFinishFollowCamera += CheckEndTurn;

    }

    private void OnDisable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnStartPlayerTurn -= ReloadCanonBallPool_Random;

        }

        if (CameraManager.Instance)
        {
            CameraManager.Instance.OnFinishFollowCamera -= CheckEndTurn;
        }

    }

    [Button]
    public void ReloadCanonBallPool_Random()
    {
        canonBallPool.Clear();

        RandomEnumSelector<CanonBallType> selector = new();

        for (int i = 0; i < canonBallAmount; i++)
        {
            canonBallPool.Enqueue(selector.GetRandomEnumValue());
        }
    }

    [Button]
    public void ReloadCanonBallPool_ByType(CanonBallType canonBallType)
    {
        canonBallPool.Clear();


        for (int i = 0; i < canonBallAmount; i++)
        {
            canonBallPool.Enqueue(canonBallType);
        }
    }

    public CanonBallData GetCanonBall()
    {
        if (canonBallPool.Count <= 0) return null;

        return canonBallDict[canonBallPool.Dequeue()];
    }

    public CanonBallData PeekCanonBall()
    {
        if (canonBallPool.Count <= 0) return null;

        return canonBallDict[canonBallPool.Peek()];
    }

    public void CheckEndTurn()
    {
        if (canonBallPool.Count <= 0)
        {
            GameManager.Instance.StartEnemyTurn();
        }
    }

}
