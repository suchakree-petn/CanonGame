using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    CanonBallController canonBallController;
    Rigidbody _rigidbody;

    private void Awake()
    {
        canonBallController = transform.GetComponentInParent<CanonBallController>();
        _rigidbody = transform.GetComponentInParent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!canonBallController.IsInit) return;

        if ((canonBallController.CollisionMask.value & (1 << other.gameObject.layer)) != 0)
        {
            _rigidbody.drag = 10;

            canonBallController.DestroyCanonBall();
        }
    }

}
