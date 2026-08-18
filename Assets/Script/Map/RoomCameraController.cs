using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class RoomCameraController : MonoBehaviour
{
    public RoomZoneLayout zoneLayout;
    public RectTransform contentRoot;
    public int startZoneIndex = 0;
    public float moveSpeed = 8f;

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
        MoveContent(GetDistanceToZone());
    }

    void Update()
    {
        ApplyAspectRatio();
        FitRoomHeightToScreen();
        HandleInput();
        MoveContent(GetDistanceToZone() * Mathf.Clamp01(moveSpeed * Time.deltaTime));
    }

    void HandleInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.dKey.wasPressedThisFrame)
            currentZone = Mathf.Min(currentZone + 1, zoneLayout.zoneCount - 1);

        if (Keyboard.current.aKey.wasPressedThisFrame)
            currentZone = Mathf.Max(currentZone - 1, 0);
    }

    float GetDistanceToZone()
    {
        return -(roomRect.anchoredPosition.x + zoneLayout.GetZoneCenterX(currentZone));
    }

    void MoveContent(float amount)
    {
        for (int i = 0; i < contentRoot.childCount; i++)
        {
            RectTransform child = contentRoot.GetChild(i) as RectTransform;
            if (child == null)
                continue;

            child.anchoredPosition += new Vector2(amount, 0f);
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
