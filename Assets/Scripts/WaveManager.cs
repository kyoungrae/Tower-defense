using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 웨이브 데이터 구조체
[System.Serializable]
public class WaveData
{
    public int enemyCount; // 해당 웨이브 총 좀비 수
    public float spawnRate; // 스폰 간격
    public float enemyHealthMultiplier; // 웨이브 진행에 따른 체력 증가분
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waveDatas; // 1~10단계 웨이브 데이터
    [SerializeField] private GameObject enemyPrefab; // 스폰할 좀비 프리팹
    [SerializeField] private Transform spawnPoint; // 좀비 스폰 위치

    private int currentWaveIndex = -1; // 현재 웨이브 인덱스 (0부터 시작)
    private int enemiesRemainingInWave; // 현재 웨이브에 남은 좀비 수
    private int enemiesSpawnedThisWave; // 현재 웨이브에서 스폰된 좀비 수
    private Coroutine spawnCoroutine;

    // UI 업데이트용 이벤트
    public static event System.Action<int, int> OnWaveUpdated; // currentWave, totalWaves
    public static event System.Action<int> OnEnemiesRemainingUpdated; // enemiesRemaining

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void OnEnable()
    {
        GameManager.OnWaveStarted += StartNextWave;
        Enemy.OnEnemyDied += OnEnemyDiedHandler;
    }

    void OnDisable()
    {
        GameManager.OnWaveStarted -= StartNextWave;
        Enemy.OnEnemyDied -= OnEnemyDiedHandler;
    }

    void Start()
    {
        // 초기 웨이브 UI 업데이트
        OnWaveUpdated?.Invoke(currentWaveIndex + 1, waveDatas.Count);
        OnEnemiesRemainingUpdated?.Invoke(0);
    }

    private void StartNextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex < waveDatas.Count)
        {
            WaveData currentWaveData = waveDatas[currentWaveIndex];
            enemiesRemainingInWave = currentWaveData.enemyCount;
            enemiesSpawnedThisWave = 0;

            OnWaveUpdated?.Invoke(currentWaveIndex + 1, waveDatas.Count);
            OnEnemiesRemainingUpdated?.Invoke(enemiesRemainingInWave);

            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
            spawnCoroutine = StartCoroutine(SpawnEnemies(currentWaveData));
            Debug.Log($"Starting Wave {currentWaveIndex + 1}");
        }
        else
        {
            Debug.Log("All waves completed!");
            GameManager.Instance.TriggerGameOver(); // 모든 웨이브 완료 시 게임 오버
        }
    }

    private IEnumerator SpawnEnemies(WaveData waveData)
    {
        for (int i = 0; i < waveData.enemyCount; i++)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 웨이브 진행에 따른 적 체력 보정
                enemy.maxHealth *= waveData.enemyHealthMultiplier;
                enemy.currentHealth = enemy.maxHealth; // 현재 체력도 조정
            }
            enemiesSpawnedThisWave++;
            yield return new WaitForSeconds(waveData.spawnRate);
        }
    }

    private void OnEnemyDiedHandler()
    {
        enemiesRemainingInWave--;
        OnEnemiesRemainingUpdated?.Invoke(enemiesRemainingInWave);

        if (enemiesRemainingInWave <= 0 && enemiesSpawnedThisWave >= waveDatas[currentWaveIndex].enemyCount)
        {
            // 모든 좀비가 스폰되고 화면에 남은 좀비가 0이 되면 WaveClear
            Debug.Log($"Wave {currentWaveIndex + 1} Cleared!");
            GameManager.Instance.EndWave();
        }
    }
}
