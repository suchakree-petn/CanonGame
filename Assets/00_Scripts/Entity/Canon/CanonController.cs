using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CanonController : SerializedSingleton<CanonController>, IDamageable
{
    public Action<CanonBallController> OnFireCanon;
    public Action OnTakeDamage;
    public Action OnMoving;

    public CommandUI commandUI;
    [SerializeField] float cooldown = 2;
    float _cooldown = 0;
    [FoldoutGroup("Frequency Config", true)] public string frequencyDown;
    [FoldoutGroup("Frequency Config", true)] public string frequencyLeft;
    [FoldoutGroup("Frequency Config", true)] public string frequencyUp;
    [FoldoutGroup("Frequency Config", true)] public string frequencyRight;

    [FoldoutGroup("Canon Config", true)] public CanonCharacterData CanonCharacterData;
    [FoldoutGroup("Canon Config", true), SerializeField] float minTurn, maxTurn, maxTilt, minTilt;
    [FoldoutGroup("Canon Config", true), SerializeField] float currentTurn, currentTilt;
    [FoldoutGroup("Canon Config", true), SerializeField] float turnSpeed, tiltSpeed;
    [FoldoutGroup("Canon Config", true), SerializeField] bool isReadyToFire;
    bool IsReadyToFire => isReadyToFire;
    [FoldoutGroup("Canon Config", true), SerializeField] float fireRate = 1;
    float _fireRate;


    public Vector3 CanonBallDropPoint => Projection.CanonBallDropPoint;
    public EnemyController HitEnemy => Projection.HitEnemy;

    [FoldoutGroup("Reference"), Required, SerializeField] Transform canonRotaterTransform;
    [FoldoutGroup("Reference"), Required, SerializeField] Transform canonTiltTransform;
    [FoldoutGroup("Reference"), Required, SerializeField] Transform firePointTransform;
    public Transform FirePointTransform => firePointTransform;
    [FoldoutGroup("Reference"), Required] public Projection Projection;
    [FoldoutGroup("Reference"), Required] public PlayerHealth PlayerHealth;
    [FoldoutGroup("Reference"), Required, SerializeField] ParticleSystem onFireExplosion;
    [FoldoutGroup("Reference"), Required, SerializeField] ParticleSystem onFireSmoke;



    protected override void InitAfterAwake()
    {

    }

    void Start()
    {
        PlayerHealth.OnTakeDamage += (damage) => PlayerUIManager.Instance.FullScreen_Player_Hit();
        PlayerHealth.OnTakeDamage += (damage) => PlayerUIManager.Instance.ShakeCameraOnHit();

        CameraManager.Instance.ActiveCamera(CameraType.MainCam, 100);

        OnMoving += SimulateProjection;
        // CameraManager.Instance.OnFinishFollowCamera += LockToTarget;
        OnFireCanon += (canonball) => SimulateProjection();
        // OnFireCanon += (canonball) => commandUI.Hide();


        // GameManager.Instance.OnStartPlayerTurn += LockToTarget;



        CameraManager.Instance.TargetGroup.AddMember(transform, 1, 0);




    }

    private void OnEnable()
    {
        GameManager.Instance.OnStartPlayerTurn += Projection.ShowProjectionLine;
        GameManager.Instance.OnStartPlayerTurn += Projection.ShowTargetDestination;
        GameManager.Instance.OnStartPlayerTurn += SimulateProjection;
        GameManager.Instance.OnStartPlayerTurn += commandUI.Show;

        GameManager.Instance.OnStartEnemyTurn += Projection.HideProjectionLine;
        GameManager.Instance.OnStartEnemyTurn += Projection.HideTargetDestination;
        GameManager.Instance.OnStartEnemyTurn += commandUI.Hide;

        // CameraManager.Instance.OnFinishFollowCamera += commandUI.Show;

    }

    private void OnDisable()
    {
        GameManager.Instance.OnStartPlayerTurn -= Projection.ShowProjectionLine;
        GameManager.Instance.OnStartPlayerTurn -= Projection.ShowTargetDestination;
        GameManager.Instance.OnStartPlayerTurn -= SimulateProjection;
        GameManager.Instance.OnStartPlayerTurn -= commandUI.Show;

        GameManager.Instance.OnStartEnemyTurn -= Projection.HideProjectionLine;
        GameManager.Instance.OnStartEnemyTurn -= Projection.HideTargetDestination;
        GameManager.Instance.OnStartEnemyTurn -= commandUI.Hide;

        // CameraManager.Instance.OnFinishFollowCamera -= commandUI.Show;

    }

    private void Update()
    {
        // if (GameState.PlayerTurn == GameManager.Instance.CurrentGameState)
        // {
        //     RotateTowardTarget(EnemyManager.Instance.ClosestAliveEnemy);
        //     LockToTarget();
        //     PlayerManager.Instance.CheckEndTurn();
        // }
        if (Input.GetKeyUp(KeyCode.Space)
        || EEGReceiver.Instance.Data == frequencyDown + "Hz"
   )
        {
            if (IsReadyToFire && !CameraManager.Instance.IsFollowCamActive)
            {
                isReadyToFire = false;

                FireCanonBall();
                _fireRate = 0;
            }



        }
        if (_fireRate >= fireRate)
        {
            isReadyToFire = true;
        }
        else
        {
            isReadyToFire = false;
            _fireRate += Time.deltaTime;
        }
    }



    private void FixedUpdate()
    {

    }
    string inputCanon = "Non";
    void LateUpdate()
    {
        // string inputEEG = EEGReceiver.Instance.Data;
        Debug.Log("IncanonControll " + EEGReceiver.Instance.Data);
        // if(inputCanon != inputEEG){
        //     inputCanon = inputEEG;
        //     Debug.Log("inputCanon : "+inputCanon);
        // }

        if (!GameManager.Instance.IsPlayerTurn) return;

        if (Input.GetKey(KeyCode.W)
         || EEGReceiver.Instance.Data == frequencyLeft + "Hz" && CameraManager.Instance.IsBirdEyeViewCamActive
         )
        {
            TiltUp();
            OnMoving?.Invoke();
        }

        if (Input.GetKey(KeyCode.S)
         || EEGReceiver.Instance.Data == frequencyRight + "Hz" && CameraManager.Instance.IsBirdEyeViewCamActive
         )
        {

            TiltDown();
            OnMoving?.Invoke();
        }

        if (Input.GetKey(KeyCode.A)
         || EEGReceiver.Instance.Data == frequencyLeft + "Hz" && !CameraManager.Instance.IsBirdEyeViewCamActive
         )
        {

            TurnLeft();
            OnMoving?.Invoke();
        }

        if (Input.GetKey(KeyCode.D)
          || EEGReceiver.Instance.Data == frequencyRight + "Hz" && !CameraManager.Instance.IsBirdEyeViewCamActive
         )
        {

            TurnRight();
            OnMoving?.Invoke();
        }





        // if (IsReadyToFire)
        // {
        //     FireCanonBall();
        // }

        if (Input.GetKeyUp(KeyCode.LeftShift)
         || EEGReceiver.Instance.Data == frequencyUp + "Hz"
        && _cooldown <= 0
         )
        {
            _cooldown = cooldown;
            if (CameraManager.Instance.IsBirdEyeViewCamActive)
            {
                CameraManager.Instance.DeactiveCamera(CameraType.BirdEyeView);
            }
            else
            {
                CameraManager.Instance.ActiveCamera(CameraType.BirdEyeView, int.MaxValue - 1);
            }
            commandUI.ChangeUIOnView();

        }

        _cooldown -= Time.deltaTime;
    }


    private void RotateTowardTarget(EnemyController enemyController)
    {
        if (!enemyController) return;
        Vector3 dir = (enemyController.transform.position - firePointTransform.position).normalized;
        dir = Vector3.ProjectOnPlane(dir, Vector3.up);
        canonRotaterTransform.forward = dir;
        canonTiltTransform.localRotation = Quaternion.Euler(currentTilt, 0, 0);
    }

    private void SimulateProjection()
    {
        CanonBallData ghostCanonBall = PlayerManager.Instance.PeekCanonBall();
        if (ghostCanonBall)
        {
            Projection.DrawProjection(
                firePointTransform.position,
                firePointTransform.forward * ghostCanonBall.CanonBallSpeed,
                CanonBallData.kGravityMultiplier);

        }
        else
        {
            Projection.HideProjectionLine();
            Projection.HideTargetDestination();
        }
    }

    [Button]
    private void LockToTarget()
    {
        for (currentTilt = maxTilt; currentTilt >= minTilt; currentTilt -= 0.3f)
        {
            canonTiltTransform.localRotation = Quaternion.Euler(currentTilt, 0, 0);
            SimulateProjection();

            bool isInside = HitEnemy;

            if (isInside)
            {
                if (currentTilt > -45)
                {
                    currentTilt -= 3f;
                    canonTiltTransform.localRotation = Quaternion.Euler(currentTilt, 0, 0);
                }
                break;
            }
        }
    }


    private CanonBallController FireCanonBall()
    {

        CanonBallData currentCanonBall = PlayerManager.Instance.GetCanonBall();

        if (currentCanonBall == null) return null;

        Projection.HideProjectionLine();
        Projection.HideTargetDestination();

        CameraManager cameraManager = CameraManager.Instance;
        Transform canonballTransform = Instantiate(currentCanonBall.CanonBall_prf, firePointTransform.position, Quaternion.identity);
        CanonBallController canonBall = canonballTransform.GetComponent<CanonBallController>();
        canonballTransform.gameObject.SetActive(false);

        bool isBirdEyeViewActive = cameraManager.IsBirdEyeViewCamActive;

        if (isBirdEyeViewActive)
        {

            cameraManager.DeactiveCamera(CameraType.BirdEyeView);
            cameraManager.OnFinishFollowCamera += ActiveBirdEyeViewCam;

        }
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f);
        sequence.AppendCallback(() =>
        {
            onFireExplosion.Play();
            onFireSmoke.Play();
            PlayerUIManager.Instance.ShakeCameraOnFireCanon().SetEase(Ease.InQuad);
        });
        sequence.OnComplete(() =>
        {
            canonballTransform.gameObject.SetActive(true);

            Rigidbody canonRb = canonballTransform.GetComponent<Rigidbody>();
            canonRb.AddForce(firePointTransform.forward * currentCanonBall.CanonBallSpeed, ForceMode.Impulse);
            OnFireCanon?.Invoke(canonBall);


        });

        return canonBall;


    }

    void ActiveBirdEyeViewCam()
    {
        CameraManager.Instance.ActiveCamera(CameraType.BirdEyeView, int.MaxValue - 1);
        CameraManager.Instance.OnFinishFollowCamera -= ActiveBirdEyeViewCam;
    }
    void TurnLeft()
    {
        currentTurn -= Time.deltaTime * turnSpeed;
        currentTurn = Mathf.Clamp(currentTurn, minTurn, maxTurn);
        canonRotaterTransform.localRotation = Quaternion.Euler(0, currentTurn, 0);
    }

    void TurnRight()
    {
        currentTurn += Time.deltaTime * turnSpeed;
        currentTurn = Mathf.Clamp(currentTurn, minTurn, maxTurn);
        canonRotaterTransform.localRotation = Quaternion.Euler(0, currentTurn, 0);
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
