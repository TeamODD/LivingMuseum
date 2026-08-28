using System.Collections;
using UnityEngine;
using DG.Tweening;
using Kino;

public class HitButton : MonoBehaviour
{
    [Header("피격 연출 설정")]
    [SerializeField] private Color hitColor = Color.red; // 피격 시 변경할 색상
    [SerializeField] private float duration = 0.25f;      // 피격 효과 지속 시간
    private float shakeStrength = 0.3f;   // [수정] 월드 좌표계에 맞춘 흔들림 강도 (기존 15f는 너무 큼)
    [SerializeField] private int vibrato = 20;             // 진동 횟수
    [SerializeField] int hp;
    [SerializeField] AudioSource hitsound;
    [SerializeField] private Transform targetCamera;
    [SerializeField] AnalogGlitch analogGlitch;
    [SerializeField] bool isApproach;
    [SerializeField] GameManager gameManager;

    // [수정] Image -> SpriteRenderer
    private SpriteRenderer targetSprite;
    private Vector3 originalPosition;
    private Color originalColor;

    private Tween colorTween;
    private Tween shakeTween;

    private void Awake()
    {
        // [수정] SpriteRenderer 가져오기
        targetSprite = GetComponent<SpriteRenderer>();
        if (targetSprite == null)
        {
            // 자식 오브젝트에 SpriteRenderer가 있는 경우 대응
            targetSprite = GetComponentInChildren<SpriteRenderer>();
        }

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        analogGlitch = GameObject.Find("Main Camera").GetComponent<AnalogGlitch>();

        if (targetSprite != null)
        {
            originalColor = targetSprite.color;
        }
    }

    private void OnDisable()
    {
        if (analogGlitch != null)
        {
            analogGlitch.horizontalShake = 0;
        }
        hp = 30;
    }

    private void OnMouseDown() // 이 이상현상을 클릭 시 실행되는 함수
    {
        if (gameManager.mode == 3)
        {
            hp--;
            if (hitsound != null) hitsound.Play();

            colorTween?.Kill();
            shakeTween?.Kill();

            // [수정] SpriteRenderer 색상 변경 연출
            if (targetSprite != null)
            {
                targetSprite.color = hitColor;
                colorTween = targetSprite.DOColor(originalColor, duration);
            }

            // [수정] UI(AnchorPos)가 아닌 월드 Transform 흔들기
            shakeTween = transform.DOShakePosition(duration, shakeStrength, vibrato);

            if (hp <= 0)
            {
                gameObject.SetActive(false);
                StopCoroutine("Glitch"); // 화면 이상효과들 다 제거
                if (analogGlitch != null)
                {
                    analogGlitch.scanLineJitter = 0;
                    analogGlitch.colorDrift = 0;
                    analogGlitch.horizontalShake = 0;
                }
                gameManager.YachaWin(); // 순찰 모드로 진입
                gameObject.GetComponent<MoveAno>()?.fixPosition();
            }
        }
    }

    public void GlitchStart()
    {
        StartCoroutine("Glitch");
    }
    public void GlitchStop()
    {
        StopCoroutine("Glitch");
        analogGlitch.scanLineJitter = 0;
        analogGlitch.colorDrift = 0;
        analogGlitch.horizontalShake = 0;
    }
    public void ShakeCamera()
    {
        targetCamera.DOKill(true);
    }

    IEnumerator Glitch()
    {
        if (analogGlitch == null) yield break;

        if (analogGlitch.scanLineJitter <= 0.5f)
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
        yield return new WaitForSeconds(0.2f);
        StartCoroutine("Glitch");
    }
}