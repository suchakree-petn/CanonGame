using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SSVEPManager : SerializedSingleton<SSVEPManager>
{
    [FoldoutGroup("Config")][SerializeField] Dictionary<int, SSVEP_Flickering> flickeringMap = new();
    [FoldoutGroup("Config")][SerializeField] List<int> flickeringHz = new();
    [FoldoutGroup("Config")][SerializeField] float offset = 5f;


    [FoldoutGroup("Reference"), Required][SerializeField] SSVEP_Flickering flickering_prf;
    [FoldoutGroup("Reference"), Required][SerializeField] Canvas uiParent;

    List<SSVEP_Flickering> _flickering = new();




    protected override void InitAfterAwake()
    {
        foreach (int hz in flickeringHz)
        {
            SSVEP_Flickering flickering = Instantiate(flickering_prf, uiParent.transform);
            flickering.frequencyImg = hz;
            flickering.gameObject.SetActive(false);
            _flickering.Add(flickering);
        }
    }

    private void Start()
    {
        // CameraManager.Instance.OnFinishFollowCamera += MapFlickeringWithEnemies;
        // CameraManager.Instance.OnFinishFollowCamera += UpdateFlickeringUIAtEnemies;

        // GameManager.Instance.OnStartPlayerTurn += MapFlickeringWithEnemies;
        // GameManager.Instance.OnStartPlayerTurn += UpdateFlickeringUIAtEnemies;
        // GameManager.Instance.OnStartPlayerTurn += ShowFlickerings;

        // GameManager.Instance.OnStartEnemyTurn += HideFlickerings;

        // CanonController.Instance.OnMoving += UpdateFlickeringUIAtEnemies;


    }

    private void MapFlickeringWithEnemies()
    {
        flickeringMap.Clear();

        Dictionary<int, EnemyController> enemies = EnemyManager.Instance.AliveEnemy;
        for (int i = 0; i < flickeringHz.Count; i++)
        {
            flickeringMap.Add(enemies.ElementAt(i).Key, _flickering[i]);
        }
    }

    public void UpdateFlickeringUIAtEnemies()
    {
        for (int i = 0; i < flickeringHz.Count; i++)
        {
            int enemyInstanceId = flickeringMap.ElementAt(i).Key;
            EnemyController enemyController = EnemyManager.Instance.AliveEnemy[enemyInstanceId];
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemyController.transform.position);
            screenPos.y += offset;
            
            flickeringMap[enemyInstanceId].transform.position = screenPos;
        }

    }

    public void ShowFlickerings()
    {
        foreach (var flickering in flickeringMap)
        {
            flickering.Value.gameObject.SetActive(true);
        }
    }

    public void HideFlickerings()
    {
        foreach (var flickering in flickeringMap)
        {
            flickering.Value.gameObject.SetActive(false);
        }
    }


}
