using UnityEngine;

public class BillboardWorldSpaceUI_Player : MonoBehaviour
{

    private void LateUpdate()
    {


        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
