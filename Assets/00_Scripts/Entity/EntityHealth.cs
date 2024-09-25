using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.WSA;

public abstract class EntityHealth : MonoBehaviour
{
    public Action<float> OnTakeDamage;

    EntityCharacterData _entityCharacterData;

    [SerializeField] protected float currentHp;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] protected EntityHealth_UI entityHealth_UI;


    public float CurrentHp
    {
        get
        {
            return currentHp;
        }
        set
        {
            currentHp = value;
        }
    }

    public float MaxHp => _entityCharacterData.Health;
    public bool IsDead => CurrentHp <= 0;

    private void Awake()
    {
        EnemyController enemyController = transform.GetComponentInParent<EnemyController>();
        if (enemyController)
        {
            _entityCharacterData = enemyController.EnemyCharacterData;
        }
        else
        {
            _entityCharacterData = transform.GetComponentInParent<CanonController>().CanonCharacterData;
        }

        currentHp = MaxHp;
    }

    protected virtual void OnEnable()
    {
        if (entityHealth_UI)
        {
            entityHealth_UI.gameObject.SetActive(true);
        }

        OnTakeDamage += (damage) =>
        {
            entityHealth_UI.SetHpBar(CurrentHp / MaxHp);
        };


    }

    public virtual void TakeDamage(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp < 0)
        {
            CurrentHp = 0;
        }
        OnTakeDamage?.Invoke(damage);
    }

    public virtual EntityHealth_UI GetEnemyHealth_UI()
    {
        return entityHealth_UI;
    }


    public void ShowHpUI()
    {
        entityHealth_UI.gameObject.SetActive(true);
    }

    public void HideHpUI()
    {
        entityHealth_UI.gameObject.SetActive(false);

    }
}
