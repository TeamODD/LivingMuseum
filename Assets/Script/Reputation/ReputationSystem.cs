using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public float gameDuration = 300f;
    public string gameOverSceneName = "GameOver";
    public string badEndingSceneName = "BadEnding";
    public string goodEndingSceneName = "GoodEnding";

    public event Action<int> OnReputationChanged;
    public event Action OnGameOver;

    float current;
    float gainTimer;
    float remainingTime;
    bool isGameOver;
    bool endingLoaded;

    public int Current
    {
        get { return Mathf.CeilToInt(current); }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    public float RemainingTime
    {
        get { return Mathf.Max(remainingTime, 0f); }
    }

    void Awake()
    {
        Instance = this;
        current = Mathf.Clamp(startReputation, 0, maxReputation);
        remainingTime = gameDuration;
    }

    void Update()
    {
        if (isGameOver || endingLoaded)
            return;

        UpdateReputation();

        if (isGameOver)
            return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
            LoadEnding(GetEnding());
    }

    void UpdateReputation()
    {
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
        Debug.Log("Game over");

        if (OnGameOver != null)
            OnGameOver();

        LoadEnding(EndingType.GameOver);
    }

    void LoadEnding(EndingType ending)
    {
        string sceneName = gameOverSceneName;

        if (ending == EndingType.Bad)
            sceneName = badEndingSceneName;
        else if (ending == EndingType.Good)
            sceneName = goodEndingSceneName;



        endingLoaded = true;
        SceneManager.LoadScene(sceneName);
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
