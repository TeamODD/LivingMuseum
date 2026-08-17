using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RoomZoneLayout : MonoBehaviour
{
    [SerializeField] int zoneCount = 10;

    SpriteRenderer spriteRenderer;

    public int ZoneCount => zoneCount;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnValidate()
    {
        zoneCount = Mathf.Max(1, zoneCount);
    }

    public Bounds GetTotalBounds()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        return spriteRenderer.bounds;
    }

    public Bounds GetZoneBounds(int zoneIndex)
    {
        zoneIndex = Mathf.Clamp(zoneIndex, 0, zoneCount - 1);

        Bounds total = GetTotalBounds();
        float zoneWidth = total.size.x / zoneCount;
        float minX = total.min.x + zoneIndex * zoneWidth;

        return new Bounds(
            new Vector3(minX + zoneWidth * 0.5f, total.center.y, total.center.z),
            new Vector3(zoneWidth, total.size.y, total.size.z)
        );
    }

    public Vector3 GetZoneCenter(int zoneIndex)
    {
        return GetZoneBounds(zoneIndex).center;
    }

    public float GetZoneWidth()
    {
        return GetTotalBounds().size.x / zoneCount;
    }

    public List<int> GetSpawnableZoneIndices(bool excludeEnds = true)
    {
        var indices = new List<int>();

        int start = excludeEnds ? 1 : 0;
        int end = excludeEnds ? zoneCount - 2 : zoneCount - 1;

        for (int i = start; i <= end; i++)
            indices.Add(i);

        return indices;
    }

    /// <summary>
    /// 오브젝트 전체가 한 구역 안에 들어가도록 랜덤 위치를 구합니다.
    /// </summary>
    public bool TryGetRandomPositionInZone(int zoneIndex, Vector2 worldSize, out Vector2 position)
    {
        Bounds zone = GetZoneBounds(zoneIndex);
        float halfW = worldSize.x * 0.5f;
        float halfH = worldSize.y * 0.5f;

        float minX = zone.min.x + halfW;
        float maxX = zone.max.x - halfW;
        float minY = zone.min.y + halfH;
        float maxY = zone.max.y - halfH;

        if (minX > maxX || minY > maxY)
        {
            position = default;
            return false;
        }

        position = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
        return true;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (zoneCount <= 0)
            return;

        for (int i = 0; i < zoneCount; i++)
        {
            Bounds zone = GetZoneBounds(i);
            Gizmos.color = (i == 0 || i == zoneCount - 1)
                ? new Color(1f, 0.3f, 0.3f, 0.35f)
                : new Color(0.3f, 1f, 0.3f, 0.35f);
            Gizmos.DrawCube(zone.center, zone.size);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(zone.center, zone.size);
        }
    }
#endif
}
