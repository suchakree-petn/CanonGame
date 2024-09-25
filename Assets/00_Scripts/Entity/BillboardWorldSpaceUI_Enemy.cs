using UnityEngine;

public class BillboardWorldSpaceUI_Enemy : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        // transform.Rotate(0, 180, 0);
    }
}
