using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class RoomCameraController : MonoBehaviour
{
    const float TargetAspect = 4f / 3f;

    [Header("References")]
    public RoomZoneLayout zoneLayout;

    [Header("Camera")]
    public int startZoneIndex = 0;
    public float moveSmoothTime = 0.25f;

    Camera cam;
    int currentZoneIndex;
    float targetCameraX;
    float velocityX;

    public int CurrentZoneIndex => currentZoneIndex;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    void Start()
    {
        ApplyAspectRatio();
        FitCameraToSingleZone();
        MoveToZone(startZoneIndex, immediate: true);
    }

    void Update()
    {
        ApplyAspectRatio();
        HandleInput();
        SmoothFollowTarget();
    }

    void HandleInput()
    {
        if (Keyboard.current == null || zoneLayout == null)
            return;

        if (Keyboard.current.dKey.wasPressedThisFrame)
            MoveToZone(currentZoneIndex + 1);

        if (Keyboard.current.aKey.wasPressedThisFrame)
            MoveToZone(currentZoneIndex - 1);
    }

    void SmoothFollowTarget()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.SmoothDamp(pos.x, targetCameraX, ref velocityX, moveSmoothTime);
        transform.position = pos;
    }

    public void MoveToZone(int zoneIndex, bool immediate = false)
    {
        if (zoneLayout == null)
            return;

        zoneIndex = Mathf.Clamp(zoneIndex, 0, zoneLayout.ZoneCount - 1);
        currentZoneIndex = zoneIndex;
        targetCameraX = zoneLayout.GetZoneCenter(zoneIndex).x;

        if (immediate)
        {
            Vector3 pos = transform.position;
            pos.x = targetCameraX;
            transform.position = pos;
            velocityX = 0f;
        }
    }

    void FitCameraToSingleZone()
    {
        if (zoneLayout == null)
            return;

        float zoneWidth = zoneLayout.GetZoneWidth();
        cam.orthographicSize = zoneWidth * 3f / 8f;

        Bounds total = zoneLayout.GetTotalBounds();
        Vector3 pos = transform.position;
        pos.y = total.center.y;
        transform.position = pos;
    }

    void ApplyAspectRatio()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        Rect rect = new Rect(0f, 0f, 1f, 1f);

        if (windowAspect > TargetAspect)
        {
            float width = TargetAspect / windowAspect;
            rect.width = width;
            rect.x = (1f - width) * 0.5f;
        }
        else
        {
            float height = windowAspect / TargetAspect;
            rect.height = height;
            rect.y = (1f - height) * 0.5f;
        }

        cam.rect = rect;
    }
}
