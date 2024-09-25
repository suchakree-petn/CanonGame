using System;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Action OnFinishFollowCamera;

    [FoldoutGroup("Follow Camera Config", true), SerializeField] float distanceDetachFollowCam = 10;
    [FoldoutGroup("Follow Camera Config", true), SerializeField] float delayAfterDetach = 2;
    [FoldoutGroup("Follow Camera Config", true), SerializeField] float detachOffset_Y = 20;
    [FoldoutGroup("Follow Camera Config", true), SerializeField, ReadOnly] Vector3 canonBallDropPoint;


    [FoldoutGroup("Config", true), ReadOnly]
    public bool IsMainCamActive, IsBirdEyeViewCamActive, IsFollowCamActive, IsShowAllEnemyCamActive;



    [FoldoutGroup("Reference"), Required]
    public CinemachineVirtualCamera MainCam, BirdEyeView, FollowCam, EnemyCloseUpCam;
    CinemachineBrain cinemachineBrain;

    Sequence followCamSequence;

    Vector3 _mainCamOriginPos;
    Quaternion _showAllEnemyCamOriginRotation;

    protected override void InitAfterAwake()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

        _mainCamOriginPos = MainCam.transform.position;
        _showAllEnemyCamOriginRotation = EnemyCloseUpCam.transform.rotation;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (IsFollowCamActive)
        {

            if (FollowCam.Follow)
            {
                float distance = Vector3.Distance(FollowCam.Follow.position, canonBallDropPoint);
                if (distance <= distanceDetachFollowCam)
                {
                    FollowCam.Follow = null;
                }
            }
            else
            {
                FollowCam.transform.LookAt(canonBallDropPoint);

            }

            if (!FollowCam.LookAt)
            {
                StopFollowCanonBall();
            }

        }
    }

    private void OnEnable()
    {

        CanonController.Instance.OnFireCanon += FollowCanonBall;


    }
    private void OnDisable()
    {
        if (CanonController.Instance)
            CanonController.Instance.OnFireCanon -= FollowCanonBall;

    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(FollowCam.transform.position, canonBallDropPoint);
    }

    void FollowCanonBall(CanonBallController canonBall)
    {
        if (!canonBall) return;
        if (!IsFollowCamActive)
        {
            canonBallDropPoint = CanonController.Instance.CanonBallDropPoint;
            Transform firePoint = CanonController.Instance.transform;
            FollowCam.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
            FollowCam.Follow = canonBall.transform;
            FollowCam.LookAt = canonBall.transform;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(0.1f).OnComplete(() =>
            {
                ActiveCamera(CameraType.FollowCam);
            });
        }
    }

    void StopFollowCanonBall()
    {
        FollowCam.Follow = null;
        FollowCam.LookAt = null;

        Vector3 newPos = new(canonBallDropPoint.x, canonBallDropPoint.y + detachOffset_Y, canonBallDropPoint.z - 10);

        followCamSequence = DOTween.Sequence();

        FollowCam.transform.DOMove(newPos, 0.5f).SetUpdate(UpdateType.Late);
        followCamSequence.AppendInterval(delayAfterDetach);
        followCamSequence.AppendCallback(() =>
        {
            CanonController.Instance.Projection.ShowProjectionLine();

            DeactiveCamera(CameraType.FollowCam);
            OnFinishFollowCamera?.Invoke();
        });

        followCamSequence.Play();
    }

    public void ActiveCamera(CameraType cameraType, int priority = int.MaxValue)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetCamera(cameraType);
        cinemachineVirtualCamera.Priority = priority;

        switch (cameraType)
        {
            case CameraType.MainCam:
                IsMainCamActive = true;
                break;
            case CameraType.BirdEyeView:
                IsBirdEyeViewCamActive = true;
                break;
            case CameraType.FollowCam:
                IsFollowCamActive = true;
                break;
            case CameraType.EnemyCloseUpCam:
                IsShowAllEnemyCamActive = true;
                break;
            default:
                Debug.LogWarning("Undefined Camera Type");
                break;
        }
    }

    public void DeactiveCamera(CameraType cameraType, int priority = int.MinValue)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetCamera(cameraType);
        cinemachineVirtualCamera.Priority = priority;
        switch (cameraType)
        {
            case CameraType.MainCam:
                IsMainCamActive = false;
                break;
            case CameraType.BirdEyeView:
                IsBirdEyeViewCamActive = false;
                break;
            case CameraType.FollowCam:
                IsFollowCamActive = false;
                break;
            case CameraType.EnemyCloseUpCam:
                IsShowAllEnemyCamActive = false;
                break;
            default:
                Debug.LogWarning("Undefined Camera Type");
                break;
        }

    }

    public void CloseUpEnemy(Transform enemy)
    {
        EnemyCloseUpCam.Follow = enemy;
        EnemyCloseUpCam.LookAt = CanonController.Instance.transform;

        ActiveCamera(CameraType.EnemyCloseUpCam);

    }

    public void StopCloseUpEnemy()
    {
        DeactiveCamera(CameraType.EnemyCloseUpCam);

        EnemyCloseUpCam.Follow = null;
        EnemyCloseUpCam.LookAt = null;
    }

    [Button]
    public Tween ShakeCamera(ShakeCameraConfig shakeCameraConfig)
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = GetCamera(shakeCameraConfig.shakeCamera);
        cinemachineVirtualCamera.transform.DOKill(true);
        Vector3 origin = cinemachineVirtualCamera.transform.localPosition;
        return cinemachineVirtualCamera.transform.DOShakePosition(shakeCameraConfig.duration, shakeCameraConfig.strength, shakeCameraConfig.vibrato, fadeOut: false)
            .OnComplete(() => cinemachineVirtualCamera.transform.localPosition = origin)
            .SetRelative(false);

    }

    public CinemachineVirtualCamera GetCamera(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.MainCam:
                return MainCam;
            case CameraType.BirdEyeView:
                return BirdEyeView;
            case CameraType.FollowCam:
                return FollowCam;
            case CameraType.EnemyCloseUpCam:
                return EnemyCloseUpCam;
            default:
                Debug.LogWarning("Undefined Camera Type");
                return MainCam;
        }
    }

}


public enum CameraType
{
    MainCam,
    BirdEyeView,
    FollowCam,
    EnemyCloseUpCam
}
