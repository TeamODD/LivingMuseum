using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class RoomCameraController : MonoBehaviour
{    
    [SerializeField] AudioSource moveroom;
    public RoomZoneLayout zoneLayout;
    public RectTransform contentRoot;
    public int startZoneIndex = 0;
    public float moveSpeed = 8f;


    [Header("Y축 시점 이동 설정")]
    public float yShiftAmount = 500f;  // 이동할 Y 거리 (기본 500)
    private float currentYOffset = 0f; // 현재 반영된 Y 오프셋

    [SerializeField] GameManager gameManager;

    Camera cam;
    RectTransform roomRect;
    int currentZone;

    public int CurrentZone
    {
        get { return currentZone; }
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

        // A, D 키로 좌우 방 이동
        if (Keyboard.current.dKey.wasPressedThisFrame)
            currentZone = Mathf.Min(currentZone + 1, zoneLayout.zoneCount - 1);

        if (Keyboard.current.aKey.wasPressedThisFrame)
            currentZone = Mathf.Max(currentZone - 1, 0);
    }

    // ==========================================
    // [UI 버튼에 연결할 함수 2개] - 이름 및 기능 맞교환
    // ==========================================

    /// <summary>
    /// MoveUp: Y축 -500 위치로 즉시 순간이동
    /// </summary>
    public void MoveUp()
    {
        moveroom.Play();
        ApplyYShiftInstant(-yShiftAmount);
        gameManager.now += 11;
    }

    /// <summary>
    /// MoveDown: 원래 Y축 위치(0)로 즉시 순간이동
    /// </summary>
    public void MoveDown()
    {
        moveroom.Play();
        ApplyYShiftInstant(0f);
        gameManager.now -= 11;
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
}