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
    // X 스폰 위치는 화면 경계에 따라 자동으로 설정됩니다.
    private float minXSpawn; // 최소 X 스폰 위치 (자동 계산)
    private float maxXSpawn; // 최대 X 스폰 위치 (자동 계산)

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
        // 화면의 좌우 경계를 기준으로 minXSpawn과 maxXSpawn을 자동으로 설정
        // ViewportToWorldPoint는 뷰포트 좌표(0,0 ~ 1,1)를 월드 좌표로 변환합니다.
        // x=0은 화면의 왼쪽, x=1은 화면의 오른쪽입니다.
        Vector3 screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));

        minXSpawn = screenLeft.x;
        maxXSpawn = screenRight.x;

        // 적 프리팹의 절반 너비를 고려하여 스폰 범위 조정 (선택 사항)
        // enemyPrefab.GetComponent<Collider2D>() 또는 Renderer의 bounds.extents.x 사용
        // 현재는 Enemy 프리팹의 크기를 알 수 없으므로, 필요하다면 여기에 추가 로직을 구현하세요.
        // 예: float enemyHalfWidth = enemyPrefab.GetComponent<SpriteRenderer>().bounds.extents.x;
        // minXSpawn += enemyHalfWidth;
        // maxXSpawn -= enemyHalfWidth;


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
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(minXSpawn, maxXSpawn), spawnPoint.position.y, spawnPoint.position.z);
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
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
