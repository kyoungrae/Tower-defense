using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Ready,
        WaveInProgress,
        UpgradeTime,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    // Game State Events
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnWaveStarted;
    public static event Action OnWaveCleared;
    public static event Action OnUpgradeTimeStarted;
    public static event Action OnGameOver;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        SetState(GameState.WaveInProgress);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Ready:
                Debug.Log("Game State: Ready");
                break;
            case GameState.WaveInProgress:
                Debug.Log("Game State: Wave In Progress");
                OnWaveStarted?.Invoke();
                break;
            case GameState.UpgradeTime:
                Debug.Log("Game State: Upgrade Time");
                OnWaveCleared?.Invoke(); // Wave cleared when upgrade time starts
                OnUpgradeTimeStarted?.Invoke();
                break;
            case GameState.GameOver:
                Debug.Log("Game State: Game Over");
                OnGameOver?.Invoke();
                break;
        }
    }

    // Example methods for game flow control
    public void StartGame()
    {
        SetState(GameState.WaveInProgress);
    }

    public void EndWave()
    {
        SetState(GameState.UpgradeTime);
    }

    public void StartNextWave()
    {
        SetState(GameState.WaveInProgress);
    }

    public void TriggerGameOver()
    {
        SetState(GameState.GameOver);
    }
}
