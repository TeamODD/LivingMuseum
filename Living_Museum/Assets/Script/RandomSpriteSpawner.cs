using System.Collections.Generic;
using UnityEngine;


public class RandomSpriteSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject spritePrefab;

    [Header("How many")]
    public int spawnCount = 5;

    [Header("Where")]
    public float minX = -4f;
    public float maxX = 4f;
    public float minY = -3f;
    public float maxY = 3f;

    [Header("Size")]
    public Vector2 spriteSize = new Vector2(0.5f, 0.5f);

    [Header("Record")]
    public List<SpawnRecord> spawnRecords = new List<SpawnRecord>();

    void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        spawnRecords.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            string id = $"sprite_{i}";
            GameObject obj = Instantiate(spritePrefab, transform);
            obj.transform.position = new Vector3(pos.x, pos.y, 0f);
            obj.transform.localScale = new Vector3(spriteSize.x, spriteSize.y, 1f);

            SpawnRecord record = new SpawnRecord(id, pos, spriteSize);
            spawnRecords.Add(record);

            Debug.Log($"[Spawn] {id} → ({pos.x:F2}, {pos.y:F2})");
        }
    }

    public IReadOnlyList<SpawnRecord> GetRecords()
    {
        return spawnRecords;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float centerX = (minX + maxX) * 0.5f;
        float centerY = (minY + maxY) * 0.5f;
        float width = maxX - minX;
        float height = maxY - minY;
        Gizmos.DrawWireCube(new Vector3(centerX, centerY, 0f), new Vector3(width, height, 0.1f));
    }
#endif
}
