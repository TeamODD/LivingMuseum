using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteSpawner : MonoBehaviour
{
    [Header("References")]
    public RoomZoneLayout zoneLayout;
    public GameObject spritePrefab;

    [Header("Spawn Rules")]
    public int maxTotalSpawns = 3;
    public bool excludeEndZones = true;

    [Header("Size")]
    public Vector2 spriteSize = new Vector2(0.5f, 0.5f);

    [Header("Record")]
    public List<SpawnRecord> spawnRecords = new List<SpawnRecord>();

    readonly List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        ClearSpawned();

        List<int> candidates = zoneLayout.GetSpawnableZoneIndices(excludeEndZones);
        Shuffle(candidates);

        int spawnCount = Mathf.Min(maxTotalSpawns, candidates.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            int zoneIndex = candidates[i];

            if (!zoneLayout.TryGetRandomPositionInZone(zoneIndex, spriteSize, out Vector2 pos))
                continue;

            string id = $"sprite_{i}";
            GameObject obj = Instantiate(spritePrefab, transform);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.localScale = new Vector3(spriteSize.x, spriteSize.y, 1f);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = 2;

            spawnedObjects.Add(obj);

            SpawnRecord record = new SpawnRecord(id, zoneIndex, pos, spriteSize);
            spawnRecords.Add(record);

            Debug.Log($"[Spawn] {id} zone={zoneIndex} pos=({pos.x:F2}, {pos.y:F2})");
        }
    }

    public void ClearSpawned()
    {
        spawnRecords.Clear();

        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] != null)
                Destroy(spawnedObjects[i]);
        }

        spawnedObjects.Clear();
    }

    public IReadOnlyList<SpawnRecord> GetRecords()
    {
        return spawnRecords;
    }

    static void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
