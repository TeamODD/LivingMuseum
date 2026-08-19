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

    [Header("관객")]
    public AudienceSystem[] audienceSystems;

    [Header("이상현상")]
    public GameManager gameManager;
    public RandomSpriteSpawner spawner;

    public int maxReputation = 100;
    public int startReputation = 100;

    public float losePerAnomalyPerSecond = 5f;
    public float gainPerSecond = 2f;
    public float gainDelay = 3f;

    public float gameDuration = 200f;
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

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
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
        if (!IsAnyAudienceWatching())
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
        if (isGameOver || endingLoaded)
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    bool IsAnyAudienceWatching()
    {
        if (audienceSystems == null)
            return false;

        for (int i = 0; i < audienceSystems.Length; i++)
        {
            if (audienceSystems[i] != null && audienceSystems[i].IsWatching)
                return true;
        }

        return false;
    }

    bool IsZoneWatched(int globalZone)
    {
        if (audienceSystems == null)
            return false;

        for (int i = 0; i < audienceSystems.Length; i++)
        {
            if (audienceSystems[i] != null && audienceSystems[i].IsGlobalZoneWatched(globalZone))
                return true;
        }

        return false;
    }

    int CountExposedAnomalies()
    {
        return CountFromGameManager() + CountFromSpawner();
    }

    int CountFromGameManager()
    {
        if (gameManager == null)
            return 0;

        int count = 0;

        for (int zone = 0; zone < gameManager.ZoneCount; zone++)
        {
            if (gameManager.IsAnomalyExposed(zone) && IsZoneWatched(zone))
                count++;
        }

        return count;
    }

    int CountFromSpawner()
    {
        if (spawner == null)
            return 0;

        List<SpawnRecord> records = spawner.GetRecords();
        int count = 0;

        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].isHidden)
                continue;

            if (IsZoneWatched(records[i].zoneIndex))
                count++;
        }

        return count;
    }
}
