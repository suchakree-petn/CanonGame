using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : SerializedSingleton<PlayerUIManager>
{


    [BoxGroup("Camera Shake Config")][SerializeField] ShakeCameraConfig hitShakeScreen;
    [BoxGroup("Camera Shake Config")][SerializeField] ShakeCameraConfig fireCanonShakeScreen;


    [FoldoutGroup("Hit FS Config")][SerializeField] float FS_Hit_Intensity = 0.1f;
    [FoldoutGroup("Hit FS Config")][SerializeField] float FS_Hit_Duration = 0.3f;

    [FoldoutGroup("Fade Screen Config")][SerializeField] float fadeDuration = 4;


    [FoldoutGroup("Damage PopUp Config"), SerializeField] float randomOffsetX = 0.5f;

    [FoldoutGroup("Damage PopUp Config"), SerializeField] float randomOffsetY = 0.5f;

    bool isOnEditorPlaying = false;

    [FoldoutGroup("Test Damage Pop Up"), SerializeField]
    int damageAmount;

    [FoldoutGroup("Test Damage Pop Up"), SerializeField]
    Vector3 screenPosition;

    [FoldoutGroup("Reference UI")][SerializeField] Canvas playerCanvas;
    [FoldoutGroup("Reference UI")][SerializeField] GameObject lockTarget;
    [FoldoutGroup("Reference UI")][SerializeField] GameObject respawnCountdownUI;
    [FoldoutGroup("Reference UI")][SerializeField] TextMeshProUGUI respawnCountdownUI_text;
    [FoldoutGroup("Reference UI")][SerializeField] GameObject resultUI;
    [FoldoutGroup("Reference UI")][SerializeField] Material fullScreen_Hit;
    [FoldoutGroup("Reference UI")][SerializeField] Image fadeScreen;
    [FoldoutGroup("Reference UI"), SerializeField, Required] Transform damagePopUp_prf;


    private void Update()
    {
    }

    protected override void InitAfterAwake()
    {
        playerCanvas = GameObject.FindWithTag("PlayerCanvasUI").GetComponent<Canvas>();

    }

    private void OnDestroy()
    {
        fullScreen_Hit.SetFloat("_Intensity", 0);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            isOnEditorPlaying = true;
        }
        else
        {
            isOnEditorPlaying = false;
        }
#endif
    }

    public void SetLockTargetPosition(Vector3 worldPos, bool isScreenPos = false)
    {
        if (isScreenPos)
        {
            lockTarget.transform.position = worldPos;
        }
        else
        {
            lockTarget.transform.position = Camera.main.WorldToScreenPoint(worldPos);
        }
    }

    public void ShowResultUI()
    {
        resultUI.SetActive(true);
    }
    public void HideResultUI()
    {
        resultUI.SetActive(false);
    }
    public void SetLockTargetState(bool isActive)
    {
        lockTarget.SetActive(isActive);
    }

    [FoldoutGroup("Test Full Screen Hit")]
    [EnableIf("isOnEditorPlaying")]
    [InfoBox("ใช้ใน Play Mode เท่านั้น")]
    [Button(name: "Simulate Full Screen Hit", ButtonSizes.Large, ButtonStyle.FoldoutButton)]
    public void FullScreen_Player_Hit()
    {
        fullScreen_Hit.DOKill();
        fullScreen_Hit.DOFloat(FS_Hit_Intensity, "_Intensity", FS_Hit_Duration).OnComplete(() =>
        {
            fullScreen_Hit.DOFloat(0, "_Intensity", FS_Hit_Duration);
        });
    }

    public Tween FadeScreenOut(Color fadeToColor = default)
    {
        fadeScreen.color = fadeToColor;
        return fadeScreen.DOFade(1, fadeDuration);
    }

    public Tween FadeScreenIn(Color fadeFromColor = default)
    {
        fadeScreen.color = fadeFromColor;
        return fadeScreen.DOFade(0, fadeDuration);
    }





    [FoldoutGroup("Test Damage Pop Up")]
    [EnableIf("isOnEditorPlaying")]
    [InfoBox("ใช้ใน Play Mode เท่านั้น")]
    [Button(name: "Simulate Damage Pop Up", ButtonSizes.Large, ButtonStyle.FoldoutButton)]
    private void ShowDamage()
    {
#if UNITY_EDITOR
        if (!isOnEditorPlaying)
        {
            Debug.LogWarning("Use in play mode only.");
            return;
        }
        else
        {
            ShowDamage(damageAmount, screenPosition);
        }

#endif

    }

    public void ShowDamage(int damageAmount, Vector3 screenPosition)
    {
        Transform damagePopUp = Instantiate(damagePopUp_prf, screenPosition, Quaternion.identity, playerCanvas.transform);
        TextMeshProUGUI damageText = damagePopUp.GetComponent<TextMeshProUGUI>();
        damageText.text = "-" + damageAmount.ToString();

        Vector3 randomOffset = new Vector3(
            Random.Range(-randomOffsetX, randomOffsetX),
            Random.Range(-randomOffsetY, randomOffsetY),
            0
        );
        damagePopUp.position += randomOffset;

        damagePopUp.localScale = Vector3.zero;
        damagePopUp.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack);

        damagePopUp.DOMoveY(damagePopUp.position.y + 1, 1f).SetEase(Ease.OutQuad);

        damageText.DOFade(0, 1f).SetEase(Ease.OutQuad).OnComplete(() => Destroy(damagePopUp.gameObject));


    }

    public Tween ShakeCameraOnHit()
    {
        return CameraManager.Instance.ShakeCamera(hitShakeScreen);
    }

    public Tween ShakeCameraOnFireCanon()
    {
        return CameraManager.Instance.ShakeCamera(fireCanonShakeScreen);
    }

}

[System.Serializable]
public struct ShakeCameraConfig
{
    public float strength;
    public float duration;
    public int vibrato;
    public CameraType shakeCamera;

    public ShakeCameraConfig(float strength, float duration, int vibrato, CameraType shakeCamera)
    {
        this.strength = strength;
        this.duration = duration;
        this.vibrato = vibrato;
        this.shakeCamera = shakeCamera;
    }
}
