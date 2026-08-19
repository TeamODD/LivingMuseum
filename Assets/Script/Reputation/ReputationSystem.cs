using System;
using System.Collections.Generic;
using UnityEngine;

public enum EndingType
{
    GameOver,
    Bad,
    Good
}

public class ReputationSystem : MonoBehaviour
{
    public static ReputationSystem Instance;

    public AudienceSystem audienceSystem;
    public RandomSpriteSpawner spawner;

    public int maxReputation = 100;
    public int startReputation = 100;

    public float losePerAnomalyPerSecond = 5f;
    public float gainPerSecond = 2f;
    public float gainDelay = 3f;

    public event Action<int> OnReputationChanged;
    public event Action OnGameOver;

    float current;
    float gainTimer;
    bool isGameOver;

    public int Current
    {
        get { return Mathf.CeilToInt(current); }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    void Awake()
    {
        Instance = this;
        current = Mathf.Clamp(startReputation, 0, maxReputation);
    }

    void Update()
    {
        if (isGameOver)
            return;

        // only work while at least one audience group is watching
        if (audienceSystem == null || !audienceSystem.IsWatching)
            return;

        int exposed = CountExposedAnomalies();

        if (exposed > 0)
        {
            gainTimer = 0f;
            ChangeBy(-losePerAnomalyPerSecond * exposed * Time.deltaTime);
            return;
        }

        gainTimer += Time.deltaTime;
        if (gainTimer >= gainDelay)
            ChangeBy(gainPerSecond * Time.deltaTime);
    }

    // called by other systems
    public void Add(int amount)
    {
        if (isGameOver)
            return;

        ChangeBy(amount);
    }

    public EndingType GetEnding()
    {
        if (Current <= 0)
            return EndingType.GameOver;

        if (Current <= 50)
            return EndingType.Bad;

        return EndingType.Good;
    }

    void ChangeBy(float amount)
    {
        int before = Current;
        current = Mathf.Clamp(current + amount, 0f, maxReputation);

        if (Current != before && OnReputationChanged != null)
            OnReputationChanged(Current);

        if (current <= 0f)
            TriggerGameOver();
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game over: reputation reached 0");

        if (OnGameOver != null)
            OnGameOver();
    }

    int CountExposedAnomalies()
    {
        if (spawner == null)
            return 0;

        List<SpawnRecord> records = spawner.GetRecords();
        int count = 0;

        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].isHidden)
                continue;

            if (audienceSystem.IsZoneWatched(records[i].zoneIndex))
                count++;
        }

        return count;
    }
}
