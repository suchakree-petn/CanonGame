using Sirenix.OdinInspector;
using UnityEngine;

public class Projection : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;


    [FoldoutGroup("Config", true), Range(10, 100), SerializeField]
    private int LinePoints = 25;

    [FoldoutGroup("Config", true), Range(0.01f, 0.25f), SerializeField]
    float TimeBetweenPoints = 0.1f;

    [FoldoutGroup("Config", true), ReadOnly] public Vector3 CanonBallDropPoint;
    [FoldoutGroup("Config", true), ReadOnly] public EnemyController HitEnemy;


    [SerializeField] private LayerMask layerMask;

    private void Awake()
    {

    }

    private void Update()
    {

    }

    public void DrawProjection(Vector3 startPosition, Vector3 startVelocity, float canonBallGravityMultiplier)
    {
        lineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;

        int i = 0;
        lineRenderer.SetPosition(i, startPosition);
        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (-canonBallGravityMultiplier / 2 * time * time);

            lineRenderer.SetPosition(i, point);

            Vector3 lastPosition = lineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude,
                layerMask))
            {
                lineRenderer.SetPosition(i, hit.point);
                lineRenderer.positionCount = i + 1;
                CanonBallDropPoint = hit.point;
                HitEnemy = hit.transform.GetComponentInParent<EnemyController>();
                return;
            }
        }
    }

    public void HideProjectionLine()
    {
        lineRenderer.enabled = false;
    }

    public void ShowProjectionLine()
    {
        lineRenderer.enabled = true;
    }

    private void OnDrawGizmos()
    {
    }

}