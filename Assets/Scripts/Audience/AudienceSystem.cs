using System.Collections.Generic;
using UnityEngine;

public class AudienceSystem : MonoBehaviour
{
    public RoomZoneLayout zoneLayout;
    public RadioWarning radio;

    [Header("층 설정")]
    public int floorIndex = 0;
    public GameManager gameManager;

    public int occupiedZoneCount = 3;
    public float moveDuration = 5f;

    public float firstEntryDelay = 15f;
    public float entryInterval = 65f;
    public float[] warningTimes = { 40f, 10f };
    public string warningLabel = "";

    public int minPeoplePerZone = 2;
    public int maxPeoplePerZone = 3;
    public float personSpacing = 90f;
    public float personY = -60f;
    public Vector2 personSize = new Vector2(280f, 520f);
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
        PushCrowdFlags();

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

    // 이 층 안에서의 구역 번호(0 ~ zoneCount-1)로 판정
    public bool IsZoneWatched(int zoneIndex)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].IsZoneOccupied(zoneIndex))
                return true;
        }

        return false;
    }

    // GameManager.now와 같은 전역 구역 번호(0~21)로 판정
    public bool IsGlobalZoneWatched(int globalZone)
    {
        int localZone = globalZone - FirstGlobalZone;

        if (localZone < 0 || localZone >= zoneLayout.zoneCount)
            return false;

        return IsZoneWatched(localZone);
    }

    public int FirstGlobalZone
    {
        get { return floorIndex * GameManager.ZonesPerFloor; }
    }

    public int ToGlobalZone(int localZone)
    {
        return FirstGlobalZone + localZone;
    }

    // 관객이 서 있는 구역을 GameManager에 반영. 관객 앞에서는 야차를 못 뜬다
    void PushCrowdFlags()
    {
        if (gameManager == null || zoneLayout == null)
            return;

        for (int i = 0; i < zoneLayout.zoneCount; i++)
            gameManager.SetCrowd(ToGlobalZone(i), IsZoneWatched(i));
    }

    void OnDisable()
    {
        if (gameManager == null || zoneLayout == null)
            return;

        for (int i = 0; i < zoneLayout.zoneCount; i++)
            gameManager.SetCrowd(ToGlobalZone(i), false);
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

            string prefix = string.IsNullOrEmpty(warningLabel) ? "" : "[" + warningLabel + "] ";
            radio.Show(prefix + "관객 입장까지 " + Mathf.RoundToInt(warningTimes[i]) + " 초!");
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
        obj.layer = zoneLayout.gameObject.layer;
        obj.transform.SetParent(zoneLayout.transform, false);
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
