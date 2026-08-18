using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteSpawner : MonoBehaviour
{
    public RoomZoneLayout zoneLayout;
    public RectTransform contentRoot;
    public GameObject anomalyPrefab;

    public float interval = 8f;
    public int typeCount = 10;
    public int maxPerZone = 2;
    public bool excludeEndZones = true;
    public Vector2 anomalySize = new Vector2(100f, 100f);

    public List<SpawnRecord> spawnRecords = new List<SpawnRecord>();

    float timer;
    List<string> remainingTypes = new List<string>();

    void Start()
    {
        for (int i = 0; i < typeCount; i++)
            remainingTypes.Add("anomaly_" + i);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < interval)
            return;

        timer = 0f;
        ConvertOne();
    }

    void ConvertOne()
    {
        if (remainingTypes.Count == 0)
            return;

        NormalArt[] arts = FindObjectsByType<NormalArt>(FindObjectsSortMode.None);
        if (arts.Length == 0)
            return;

        List<int> zones = GetOpenZones();
        if (zones.Count == 0)
            return;

        int zoneIndex = zones[Random.Range(0, zones.Count)];
        Vector2 roomPos = zoneLayout.GetRandomPositionInZone(zoneIndex, anomalySize);

        Destroy(arts[Random.Range(0, arts.Length)].gameObject);

        int pick = Random.Range(0, remainingTypes.Count);
        string typeId = remainingTypes[pick];
        remainingTypes.RemoveAt(pick);

        RectTransform roomRect = zoneLayout.GetComponent<RectTransform>();
        GameObject obj = Instantiate(anomalyPrefab, contentRoot);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = anomalySize;
        rect.anchoredPosition = roomPos + roomRect.anchoredPosition;

        spawnRecords.Add(new SpawnRecord(typeId, zoneIndex, roomPos, anomalySize));
    }

    List<int> GetOpenZones()
    {
        List<int> zones = zoneLayout.GetSpawnableZones(excludeEndZones);
        List<int> open = new List<int>();

        for (int i = 0; i < zones.Count; i++)
        {
            int count = 0;

            for (int r = 0; r < spawnRecords.Count; r++)
            {
                if (spawnRecords[r].zoneIndex == zones[i])
                    count++;
            }

            if (count < maxPerZone)
                open.Add(zones[i]);
        }

        return open;
    }

    public List<SpawnRecord> GetRecords()
    {
        return spawnRecords;
    }
}
