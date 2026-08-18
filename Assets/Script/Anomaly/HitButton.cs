using System.Collections;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Kino;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HitButton : MonoBehaviour, IPointerClickHandler
{
    [Header("피격 연출 설정")]
    [SerializeField] private Color hitColor = Color.red; // 피격 시 변경할 색상
    [SerializeField] private float duration = 0.25f;     // 피격 효과 지속 시간
    [SerializeField] private float shakeStrength = 15f;  // 흔들림 강도
    [SerializeField] private int vibrato = 20;            // 진동 횟수
    [SerializeField] int hp;
    [SerializeField] AudioSource hitsound;
    [SerializeField] private Transform targetCamera;
    [SerializeField] AnalogGlitch analogGlitch;
    [SerializeField] bool isApproach;
    [SerializeField] GameManager gameManager;

    private Image targetImage;
    private RectTransform rectTransform;
    private Color originalColor;

    // 피격 연출 전용 트윈 참조 (DOScale 방해 금지용)
    private Tween colorTween;
    private Tween shakeTween;

    private void Awake()
    {
        targetImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (targetImage != null)
        {
            originalColor = targetImage.color;
        }
    }
    private void OnEnable()
    {
        if(isApproach) StartCoroutine("Glitch");
    }
    private void OnDisable()
    {
        analogGlitch.horizontalShake = 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayHitEffect();
    }

    public void PlayHitEffect()
    {
        if(gameManager.mode==3)
        {
            hp--;           
            hitsound.Play();
            colorTween?.Kill();
            shakeTween?.Kill();

            if (targetImage != null)
            {
                targetImage.color = hitColor;
                colorTween = targetImage.DOColor(originalColor, duration);
            }

            shakeTween = rectTransform.DOShakeAnchorPos(duration, shakeStrength, vibrato);

            if (hp <= 0)
            {
                gameObject.SetActive(false);
                StopCoroutine("Glitch");//화면 이상효과들 다 제거
                analogGlitch.scanLineJitter = 0;
                analogGlitch.colorDrift = 0;
                analogGlitch.horizontalShake = 0;
                gameManager.WalkMode();//순찰 모드로 진입
            }
        }   
    }

    public void ShakeCamera()
    {
        targetCamera.DOKill(true);
    }

    IEnumerator Glitch()
    {
        if (analogGlitch.scanLineJitter <= 0.5)
        {
            analogGlitch.scanLineJitter += 0.01f;
            analogGlitch.colorDrift += 0.01f;
        }
        else
        {
            analogGlitch.scanLineJitter += 0.01f;
            analogGlitch.colorDrift += 0.01f;
            analogGlitch.horizontalShake += 0.02f;
        }
        yield return new WaitForSeconds(0.05f);
        StartCoroutine("Glitch");
    }
}
