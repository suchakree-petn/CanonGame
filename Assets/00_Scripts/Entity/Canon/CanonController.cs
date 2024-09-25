using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CanonController : SerializedSingleton<CanonController>, IDamageable
{
    public Action<CanonBallController> OnFireCanon;
    public Action OnTakeDamage;
    public Action OnMoving;



    [FoldoutGroup("Canon Config", true)] public CanonCharacterData CanonCharacterData;
    [FoldoutGroup("Canon Config", true), SerializeField] float minTurn, maxTurn, maxTilt, minTilt;
    [FoldoutGroup("Canon Config", true), SerializeField] float currentTurn, currentTilt;
    [FoldoutGroup("Canon Config", true), SerializeField] float turnSpeed, tiltSpeed;
    [FoldoutGroup("Canon Config", true), SerializeField] bool isReadyToFire;
    bool IsReadyToFire => isReadyToFire;


    [FoldoutGroup("Canon Ball Config", true), SerializeField]
    float canonBallSpeed = 1;

    public Vector3 CanonBallDropPoint => Projection.CanonBallDropPoint;

    [FoldoutGroup("Reference"), Required, SerializeField] Transform canonRotaterTransform;
    [FoldoutGroup("Reference"), Required, SerializeField] Transform canonTiltTransform;
    [FoldoutGroup("Reference"), Required, SerializeField] Transform firePointTransform;
    public Transform FirePointTransform => firePointTransform;
    [FoldoutGroup("Reference"), Required] public Projection Projection;
    [FoldoutGroup("Reference"), Required] public PlayerHealth PlayerHealth;


    protected override void InitAfterAwake()
    {

    }

    void Start()
    {
        PlayerHealth.OnTakeDamage += (damage) => PlayerUIManager.Instance.FullScreen_Player_Hit();
        PlayerHealth.OnTakeDamage += (damage) => PlayerUIManager.Instance.ShakeCameraOnHit();

        CameraManager.Instance.ActiveCamera(CameraType.MainCam, 100);

        OnMoving += SimulateProjection;


        GameManager.Instance.OnStartPlayerTurn += Projection.ShowProjectionLine;
        GameManager.Instance.OnStartEnemyTurn += Projection.HideProjectionLine;


    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    void LateUpdate()
    {
        if (!GameManager.Instance.IsPlayerTurn) return;

        if (Input.GetKey(KeyCode.W))
        {
            OnMoving?.Invoke();
            TiltUp();
        }

        if (Input.GetKey(KeyCode.S))
        {
            OnMoving?.Invoke();

            TiltDown();
        }

        if (Input.GetKey(KeyCode.A))
        {
            OnMoving?.Invoke();

            TurnLeft();
        }

        if (Input.GetKey(KeyCode.D))
        {
            OnMoving?.Invoke();

            TurnRight();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!IsReadyToFire) return;

            FireCanonBall();

        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (CameraManager.Instance.IsBirdEyeViewCamActive)
            {
                CameraManager.Instance.DeactiveCamera(CameraType.BirdEyeView);
            }
            else
            {
                CameraManager.Instance.ActiveCamera(CameraType.BirdEyeView, int.MaxValue - 1);
            }
        }


    }


    private void OnEnable()
    {
    }

    private void SimulateProjection()
    {
        CanonBallData ghostCanonBall = PlayerManager.Instance.PeekCanonBall();
        if (ghostCanonBall)
        {
            Projection.ShowProjectionLine();
            Transform canonBall = ghostCanonBall.CanonBall_prf;
            Projection.DrawProjection(
                firePointTransform.position,
                firePointTransform.forward * canonBallSpeed,
                canonBall.GetComponent<CanonBallController>().CanonBallGravityMultiplier);
        }
        else
        {
            Projection.HideProjectionLine();
        }
    }

    private CanonBallController FireCanonBall()
    {

        CanonBallData currentCanonBall = PlayerManager.Instance.GetCanonBall();

        if (currentCanonBall == null) return null;

        Projection.HideProjectionLine();

        Transform canonballTransform = Instantiate(currentCanonBall.CanonBall_prf, firePointTransform.position, Quaternion.identity);
        CanonBallController canonBall = canonballTransform.GetComponent<CanonBallController>();
        canonballTransform.gameObject.SetActive(false);

        bool isBirdEyeViewActive = CameraManager.Instance.IsBirdEyeViewCamActive;

        if (isBirdEyeViewActive)
        {
            CameraManager.Instance.DeactiveCamera(CameraType.BirdEyeView);
        }
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f);
        sequence.Append(PlayerUIManager.Instance.ShakeCameraOnFireCanon().SetEase(Ease.InQuad));
        sequence.OnComplete(() =>
        {
            if (isBirdEyeViewActive)
            {
                CameraManager.Instance.ActiveCamera(CameraType.BirdEyeView, int.MaxValue - 1);
            }

            canonballTransform.gameObject.SetActive(true);

            Rigidbody canonRb = canonballTransform.GetComponent<Rigidbody>();
            canonRb.AddForce(firePointTransform.forward * canonBallSpeed, ForceMode.Impulse);
            OnFireCanon?.Invoke(canonBall);


        });

        return canonBall;


    }

    void TurnLeft()
    {
        currentTurn -= Time.deltaTime * turnSpeed;
        currentTurn = Mathf.Clamp(currentTurn, minTurn, maxTurn);
        canonRotaterTransform.rotation = Quaternion.Euler(0, currentTurn, 0);
    }

    void TurnRight()
    {
        currentTurn += Time.deltaTime * turnSpeed;
        currentTurn = Mathf.Clamp(currentTurn, minTurn, maxTurn);
        canonRotaterTransform.rotation = Quaternion.Euler(0, currentTurn, 0);
    }

    void TiltUp()
    {
        currentTilt -= Time.deltaTime * tiltSpeed;
        currentTilt = Mathf.Clamp(currentTilt, minTilt, maxTilt);
        canonTiltTransform.localRotation = Quaternion.Euler(currentTilt, 0, 0);
    }

    void TiltDown()
    {
        currentTilt += Time.deltaTime * tiltSpeed;
        currentTilt = Mathf.Clamp(currentTilt, minTilt, maxTilt);
        canonTiltTransform.localRotation = Quaternion.Euler(currentTilt, 0, 0);
    }

    public void TakeDamage(float damage)
    {
        PlayerHealth.TakeDamage(damage);
    }
}
