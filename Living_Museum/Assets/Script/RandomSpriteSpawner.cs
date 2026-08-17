using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteSpawner : MonoBehaviour
{
    [Header("References")]
    public RoomZoneLayout zoneLayout;
    public GameObject anomalyPrefab;

    [Header("Convert")]
    public float interval = 8f;
    public int typeCount = 10;
    public Vector2 anomalySize = new Vector2(0.5f, 0.5f);

    [Header("Record")]
    public List<SpawnRecord> spawnRecords = new List<SpawnRecord>();

    float timer;
    List<string> remainingTypes = new List<string>();

    void Start()
    {
        remainingTypes.Clear();
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

        NormalArt art = arts[Random.Range(0, arts.Length)];
        int zoneIndex = zoneLayout.GetZoneIndex(art.transform.position);

        Destroy(art.gameObject);

        if (!zoneLayout.TryGetRandomPositionInZone(zoneIndex, anomalySize, out Vector2 pos))
            return;

        int typePick = Random.Range(0, remainingTypes.Count);
        string typeId = remainingTypes[typePick];
        remainingTypes.RemoveAt(typePick);

        GameObject obj = Instantiate(anomalyPrefab, transform);
        obj.transform.position = new Vector3(pos.x, pos.y, 0f);
        obj.transform.localScale = new Vector3(anomalySize.x, anomalySize.y, 1f);

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 2;

        spawnRecords.Add(new SpawnRecord(typeId, zoneIndex, pos, anomalySize));
        Debug.Log("[Convert] " + typeId + " zone=" + zoneIndex + " pos=(" + pos.x.ToString("F2") + ", " + pos.y.ToString("F2") + ")");
    }

    public IReadOnlyList<SpawnRecord> GetRecords()
    {
        return spawnRecords;
    }
}
