using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private int currentCoins = 0;

    // UI 업데이트용 이벤트
    public static event Action<int> OnCurrencyChanged;

    public int CurrentCoins
    {
        get { return currentCoins; }
        private set
        {
            if (currentCoins != value)
            {
                currentCoins = value;
                OnCurrencyChanged?.Invoke(currentCoins);
                Debug.Log($"Current Coins: {currentCoins}");
            }
        }
    }

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
        PlayerController.OnCoinCollected += AddCoins;
    }

    void OnDisable()
    {
        PlayerController.OnCoinCollected -= AddCoins;
    }

    void Start()
    {
        // 게임 시작 시 초기 코인 값으로 UI 업데이트 이벤트 발생
        OnCurrencyChanged?.Invoke(currentCoins);
    }

    public void AddCoins(int amount)
    {
        if (amount < 0) return; // 음수 값 방지
        CurrentCoins += amount;
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount < 0) return false; // 음수 값 방지

        if (CurrentCoins >= amount)
        {
            CurrentCoins -= amount;
            return true;
        }
        Debug.Log("Not enough coins.");
        return false;
    }
}
