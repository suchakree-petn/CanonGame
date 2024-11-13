using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projection : MonoBehaviour
{
    [FoldoutGroup("Config", true)][SerializeField] private LayerMask layerMask;

    [FoldoutGroup("Config", true), Range(10, 100), SerializeField]
    private int LinePoints = 25;

    [FoldoutGroup("Config", true), Range(0.01f, 0.25f), SerializeField]
    float TimeBetweenPoints = 0.1f;

    [FoldoutGroup("Config", true), ReadOnly] public Vector3 CanonBallDropPoint;
    [FoldoutGroup("Config", true), ReadOnly] public EnemyController HitEnemy;



    [FoldoutGroup("Reference"), SerializeField, Required] DecalProjector targetDestination_prf;
    DecalProjector _targetDestination;

    [FoldoutGroup("Reference"), SerializeField, Required] LineRenderer lineRenderer;


    private void Awake()
    {
        if (targetDestination_prf)
        {
            _targetDestination = Instantiate(targetDestination_prf);
        }
    }

    private void Update()
    {

    }

    private void OnDrawGizmos()
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
                UpdateTargetDestinationDecal();
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

    public void HideTargetDestination()
    {
        _targetDestination.gameObject.SetActive(false);
    }

    public void ShowTargetDestination()
    {
        _targetDestination.gameObject.SetActive(true);
    }

    private void UpdateTargetDestinationDecal()
    {
        _targetDestination.transform.position = CanonBallDropPoint;
    }

}