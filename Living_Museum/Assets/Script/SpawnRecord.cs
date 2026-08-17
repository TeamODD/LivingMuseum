using UnityEngine;

[System.Serializable]
public class SpawnRecord
{
    public string id;
    public Vector2 position;
    public Vector2 size;

    public SpawnRecord(string id, Vector2 position, Vector2 size)
    {
        this.id = id;
        this.position = position;
        this.size = size;
    }
}
