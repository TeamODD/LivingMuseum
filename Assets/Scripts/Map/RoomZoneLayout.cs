using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RoomZoneLayout : MonoBehaviour
{
    public int zoneCount = 10;

    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public float GetZoneWidth()
    {
        return GetRoomSize().x / zoneCount;
    }

    public float GetZoneCenterX(int zoneIndex)
    {
        float zoneWidth = GetZoneWidth();
        return -GetRoomSize().x * 0.5f + zoneWidth * (zoneIndex + 0.5f);
    }

    public List<int> GetSpawnableZones(bool excludeEnds)
    {
        List<int> zones = new List<int>();

        int start = excludeEnds ? 1 : 0;
        int end = excludeEnds ? zoneCount - 2 : zoneCount - 1;

        for (int i = start; i <= end; i++)
            zones.Add(i);

        return zones;
    }

    public Vector2 GetRandomPositionInZone(int zoneIndex, Vector2 size)
    {
        Vector2 roomSize = GetRoomSize();
        float zoneWidth = GetZoneWidth();
        float zoneMinX = -roomSize.x * 0.5f + zoneWidth * zoneIndex;

        float minX = zoneMinX + size.x * 0.5f;
        float maxX = zoneMinX + zoneWidth - size.x * 0.5f;
        float minY = -roomSize.y * 0.5f + size.y * 0.5f;
        float maxY = roomSize.y * 0.5f - size.y * 0.5f;

        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    Vector2 GetRoomSize()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        return rect.rect.size;
    }
}
