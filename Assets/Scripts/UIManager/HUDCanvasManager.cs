using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 필요
using System;

// HUD Canvas의 UI 요소들을 관리하는 스크립트
public class HUDCanvasManager : MonoBehaviour
{
    [Header("UI 텍스트 요소")]
    [SerializeField] private Text coinText; // 현재 코인을 표시할 Text 컴포넌트
    [SerializeField] private Text waveText; // 현재 웨이브를 표시할 Text 컴포넌트
    [SerializeField] private Text enemiesRemainingText; // 남은 적 수를 표시할 Text 컴포넌트

    void OnEnable()
    {
        // CurrencyManager의 코인 변경 이벤트를 구독하여 UI 업데이트
        CurrencyManager.OnCurrencyChanged += UpdateCoinText;
        // WaveManager의 웨이브 업데이트 이벤트를 구독하여 UI 업데이트
        WaveManager.OnWaveUpdated += UpdateWaveText;
        // WaveManager의 남은 적 수 업데이트 이벤트를 구독하여 UI 업데이트
        WaveManager.OnEnemiesRemainingUpdated += UpdateEnemiesRemainingText;

        // 게임 관리자의 상태 변경 이벤트를 구독하여 업그레이드 UI 활성화/비활성화
        GameManager.OnGameStateChanged += OnGameStateChangedHandler;
    }

    void OnDisable()
    {
        // 스크립트 비활성화 시 이벤트 구독 해제 (메모리 누수 방지)
        CurrencyManager.OnCurrencyChanged -= UpdateCoinText;
        WaveManager.OnWaveUpdated -= UpdateWaveText;
        WaveManager.OnEnemiesRemainingUpdated -= UpdateEnemiesRemainingText;
        GameManager.OnGameStateChanged -= OnGameStateChangedHandler;
    }

    void Start()
    {
        // 초기 UI 설정 (매니저들이 시작하기 전에 이 값이 0일 수 있으므로 Start에서 한번 더 호출)
        if (CurrencyManager.Instance != null)
        {
            UpdateCoinText(CurrencyManager.Instance.CurrentCoins);
        }
        // WaveManager의 초기값을 가져와서 설정하는 부분은 WaveManager에서 Start 시 이벤트를 발생시켜주므로 필요 없음
    }

    // 코인 텍스트 업데이트 메서드
    private void UpdateCoinText(int coins)
    {
        if (coinText != null)
        {
            coinText.text = $"코인: {coins}";
        }
    }

    // 웨이브 텍스트 업데이트 메서드
    private void UpdateWaveText(int currentWave, int totalWaves)
    {
        if (waveText != null)
        {
            waveText.text = $"웨이브: {currentWave} / {totalWaves}";
        }
    }

    // 남은 적 수 텍스트 업데이트 메서드
    private void UpdateEnemiesRemainingText(int enemiesRemaining)
    {
        if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = $"남은 적: {enemiesRemaining}";
        }
    }

    // 게임 상태 변경 핸들러
    private void OnGameStateChangedHandler(GameManager.GameState newState)
    {
        // 필요하다면 HUD UI의 특정 요소 활성화/비활성화를 여기서 처리할 수 있음
        // 예를 들어, 게임 오버 시 특정 패널을 보여주는 등
        switch (newState)
        {
            case GameManager.GameState.Ready:
            case GameManager.GameState.WaveInProgress:
            case GameManager.GameState.UpgradeTime:
                gameObject.SetActive(true); // 게임 플레이 중에는 HUD 활성화
                break;
            case GameManager.GameState.GameOver:
                // 게임 오버 시 HUD를 비활성화하거나 특정 게임 오버 UI를 띄울 수 있음
                // gameObject.SetActive(false);
                break;
        }
    }
}
