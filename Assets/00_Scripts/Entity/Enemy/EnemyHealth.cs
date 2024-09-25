using System;
using UnityEngine;

public class EnemyHealth : EntityHealth
{
    [Header("Enemy Reference")]
    [SerializeField] protected EnemyController enemyController;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnTakeDamage += (damage) =>
        {
            Vector3 screenSpacePos = Camera.main.WorldToScreenPoint(enemyController.transform.position);
            PlayerUIManager.Instance.ShowDamage((int)damage, screenSpacePos);
        };
    }

    private void OnDisable()
    {

    }
}
