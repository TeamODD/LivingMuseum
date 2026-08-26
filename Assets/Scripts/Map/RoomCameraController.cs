using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

[RequireComponent(typeof(Camera))]
public class RoomCameraController : MonoBehaviour
{    
    [SerializeField] AudioSource moveroom;
    [SerializeField] AudioSource footstep;
    public RoomZoneLayout zoneLayout;
    public RectTransform contentRoot;
    public int startZoneIndex = 0;
    public float moveSpeed = 8f;
    public Image fadeImage;//문 이동시 페이드효과 이미지
    public TextMeshProUGUI roomtxt;
    private bool canMove=true;

    [Header("Y축 시점 이동 설정")]
    float yShiftAmount = 1072f;  // 이동할 Y 거리 (기본 500)
    private float currentYOffset = 0f; // 현재 반영된 Y 오프셋

    [SerializeField] GameManager gameManager;

    Camera cam;
    RectTransform roomRect;
    int currentZone;
    int currentFloor;

    public int CurrentZone
    {
        get { return currentZone; }
    }

    public int CurrentFloor
    {
        get { return currentFloor; }
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    void Start()
    {
        roomRect = zoneLayout.GetComponent<RectTransform>();
        currentZone = Mathf.Clamp(startZoneIndex, 0, zoneLayout.zoneCount - 1);

        ApplyAspectRatio();
        FitRoomHeightToScreen();
        MoveContent(GetDistanceToZone(), 0f);
        SyncGameManagerZone();
    }

    void Update()
    {
        HandleInput();

        float xDistance = GetDistanceToZone();

        // X축 이동 완료 체크 (화면 떨림 방지)
        if (Mathf.Abs(xDistance) < 0.1f)
        {
            if (Mathf.Abs(xDistance) > 0.001f)
            {
                MoveContent(xDistance, 0f);
            }
            return;
        }

        ApplyAspectRatio();
        FitRoomHeightToScreen();

        // X축(좌우 방 이동)만 부드럽게 이동
        float xAmount = xDistance * Mathf.Clamp01(moveSpeed * Time.deltaTime);
        MoveContent(xAmount, 0f);
    }

    void HandleInput()
    {
        if (Keyboard.current == null)
            return;

        int before = currentZone;

        // A, D 키로 좌우 방 이동
        if (Keyboard.current.dKey.wasPressedThisFrame&&canMove && gameManager.now != 10 && gameManager.now != 21)
        {
            currentZone = Mathf.Min(currentZone + 1, zoneLayout.zoneCount - 1);
            gameManager.now++;
            footstep.Play();
        }
           
        if (Keyboard.current.aKey.wasPressedThisFrame&&canMove && gameManager.now != 0 && gameManager.now != 11)
        {
            currentZone = Mathf.Max(currentZone - 1, 0);
            gameManager.now--;
            footstep.Play();
        }
            
        if (currentZone != before)
            SyncGameManagerZone();
    }

    public void MoveUp()
    {
        StartCoroutine("MoveUpCo");      
    }

    public void MoveDown()
    {      
        StartCoroutine("MoveDownCo");
    }

    // GameManager.now = 층 * 11 + 현재 구역
    void SyncGameManagerZone()
    {
        if (gameManager == null)
            return;

        gameManager.now = currentFloor * GameManager.ZonesPerFloor + currentZone;
    }

    // Y축 좌표를 즉시 변경하는 내부 함수
    private void ApplyYShiftInstant(float targetY)
    {
        float deltaY = targetY - currentYOffset;
        currentYOffset = targetY;
        MoveContent(0f, deltaY); // 대기 없이 바로 좌표 이동
    }

    // ==========================================

    float GetDistanceToZone()
    {
        return -(roomRect.anchoredPosition.x + zoneLayout.GetZoneCenterX(currentZone));
    }

    void MoveContent(float xAmount, float yAmount)
    {
        for (int i = 0; i < contentRoot.childCount; i++)
        {
            RectTransform child = contentRoot.GetChild(i) as RectTransform;
            if (child == null)
                continue;

            child.anchoredPosition += new Vector2(xAmount, yAmount);
        }
    }

    void FitRoomHeightToScreen()
    {
        CanvasScaler scaler = contentRoot.GetComponent<CanvasScaler>();
        if (scaler == null)
            return;

        float viewHeight = Screen.height * cam.rect.height;
        scaler.scaleFactor = viewHeight / roomRect.rect.height;
    }

    void ApplyAspectRatio()
    {
        float targetAspect = 4f / 3f;
        float windowAspect = (float)Screen.width / Screen.height;
        Rect rect = new Rect(0f, 0f, 1f, 1f);

        if (windowAspect > targetAspect)
        {
            rect.width = targetAspect / windowAspect;
            rect.x = (1f - rect.width) * 0.5f;
        }
        else
        {
            rect.height = windowAspect / targetAspect;
            rect.y = (1f - rect.height) * 0.5f;
        }

        cam.rect = rect;
    }

    public void PlayFadeInOut(System.Action onBlackout, float duration = 0.9f)
    {
        fadeImage.gameObject.SetActive(true);

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        Sequence fadeSequence = DOTween.Sequence();

        fadeSequence
            .Append(fadeImage.DOFade(1f, duration)) // 1. 서서히 어두워짐 (0.9초)
            .AppendCallback(() =>
            {
                // 2. 화면이 완전히 어두워진 순간(알파값 1) 층 이동 처리
                onBlackout?.Invoke();
            })
            .Append(fadeImage.DOFade(0f, duration)) // 3. 다시 서서히 밝아짐 (0.9초)
            .OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false); // 연출 끝
            });
    }

    IEnumerator MoveUpCo()
    {
        canMove = false;
        moveroom.Play();

        // 어두워진 순간(중간 지점)에 실행할 층 이동 로직을 람다식으로 전달
        PlayFadeInOut(() =>
        {
            roomtxt.text = "Room2";
            currentFloor = 1;
            ApplyYShiftInstant(-yShiftAmount);
            SyncGameManagerZone();
        }, 0.9f);

        // 전체 페이드 연출(0.9s + 0.9s = 1.8s)이 마칠 때까지 대기 후 이동 제한 해제
        yield return new WaitForSeconds(1.8f);
        canMove = true;
    }

    IEnumerator MoveDownCo()
    {
        canMove = false;
        moveroom.Play();

        PlayFadeInOut(() =>
        {
            roomtxt.text = "Room1";
            currentFloor = 0;
            ApplyYShiftInstant(0f);
            SyncGameManagerZone();
        }, 0.9f);

        yield return new WaitForSeconds(1.8f);
        canMove = true;
    }
}