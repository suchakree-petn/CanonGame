using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    public class DEMO_Legsanim_Translate : MonoBehaviour
    {
        public Vector3 LocalOffset = Vector3.zero;
        Rigidbody rig;

        public float TurnCd = 10;
        float turnCd;

        private void Start()
        {
            rig = GetComponent<Rigidbody>();
            turnCd = TurnCd;
        }

        void Update()
        {
            turnCd -= Time.deltaTime;

            if (turnCd <= 0)
            {
                turnCd = TurnCd;
                transform.Rotate(new(0,90,0));
            }
            if (rig != null) return;
            transform.position += transform.TransformVector(LocalOffset * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (rig == null) return;
            Vector3 newVelo = transform.TransformVector(LocalOffset);
            newVelo.y = rig.velocity.y;
            rig.velocity = newVelo;
        }
    }
}