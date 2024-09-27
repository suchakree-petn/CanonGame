using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection : MonoBehaviour
{
    CanonBallController canonBallController;

    Collider _collider;

    private void Awake()
    {
        canonBallController = transform.GetComponentInParent<CanonBallController>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger) return;

        RaycastHit[] raycastHit = Physics.SphereCastAll(transform.position, canonBallController.AoeRange, Vector3.up,canonBallController.AoeRange, canonBallController.DamageMask);

        if(raycastHit.Length == 0) return;

        foreach (var hit in raycastHit)
        {
            IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                canonBallController.DealDamage(damageable);
            }
        }
        _collider.enabled = false;

    }
}
