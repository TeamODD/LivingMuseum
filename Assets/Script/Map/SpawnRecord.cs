using UnityEngine;

[System.Serializable]
public class SpawnRecord
{
    public string id;
    public int zoneIndex;
    public Vector2 position;
    public Vector2 size;

    public SpawnRecord(string id, int zoneIndex, Vector2 position, Vector2 size)
    {
        this.id = id;
        this.zoneIndex = zoneIndex;
        this.position = position;
        this.size = size;
    }
}
