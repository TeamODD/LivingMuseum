using System.Collections.Generic;
using UnityEngine;

public class AudienceSystem : MonoBehaviour
{
    public RoomZoneLayout zoneLayout;
    public RectTransform contentRoot;
    public RadioWarning radio;

    public int occupiedZoneCount = 3;
    public float moveDuration = 5f;

    public float firstEntryDelay = 15f;
    public float entryInterval = 65f;
    public float[] warningTimes = { 40f, 10f };

    public int minPeoplePerZone = 2;
    public int maxPeoplePerZone = 3;
    public float personSpacing = 90f;
    public float personY = -60f;
    public Vector2 personSize = new Vector2(140f, 260f);
    public int crowdSpriteCount = 8;

    List<AudienceGroup> groups = new List<AudienceGroup>();
    float entryTimer;
    bool[] warningShown;
    bool waitingForEntry;

    public bool IsWatching
    {
        get { return groups.Count > 0; }
    }

    void Start()
    {
        StartEntryCountdown(firstEntryDelay);
    }

    void Update()
    {
        RemoveFinishedGroups();

        if (groups.Count > 0)
            return;

        if (!waitingForEntry)
            StartEntryCountdown(entryInterval);

        entryTimer -= Time.deltaTime;
        CheckWarnings();

        if (entryTimer > 0f)
            return;

        SpawnGroup();
        waitingForEntry = false;
    }

    public bool IsZoneWatched(int zoneIndex)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].IsZoneOccupied(zoneIndex))
                return true;
        }

        return false;
    }

    public Sprite GetRandomCrowdSprite()
    {
        return Resources.Load<Sprite>("Crowd" + Random.Range(1, crowdSpriteCount + 1));
    }

    void CheckWarnings()
    {
        if (radio == null)
            return;

        for (int i = 0; i < warningTimes.Length; i++)
        {
            if (warningShown[i] || entryTimer > warningTimes[i])
                continue;

            radio.Show("Audience entering in " + Mathf.RoundToInt(warningTimes[i]) + " seconds");
            warningShown[i] = true;
        }
    }

    void StartEntryCountdown(float delay)
    {
        entryTimer = delay;
        waitingForEntry = true;
        warningShown = new bool[warningTimes.Length];

        for (int i = 0; i < warningTimes.Length; i++)
        {
            if (warningTimes[i] > delay)
                warningShown[i] = true;
        }
    }

    void SpawnGroup()
    {
        GameObject obj = new GameObject("AudienceGroup");
        obj.transform.SetParent(contentRoot, false);
        obj.AddComponent<RectTransform>();

        AudienceGroup group = obj.AddComponent<AudienceGroup>();
        group.Setup(this, zoneLayout);

        groups.Add(group);
    }

    void RemoveFinishedGroups()
    {
        for (int i = groups.Count - 1; i >= 0; i--)
        {
            if (groups[i] == null || groups[i].IsFinished)
                groups.RemoveAt(i);
        }
    }
}
