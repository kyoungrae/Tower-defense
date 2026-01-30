using UnityEngine;
using System.Collections.Generic;
using System;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("업그레이드 설정")]
    // 에디터에서 할당할 업그레이드 데이터 ScriptableObject 목록
    [SerializeField] public List<UpgradeData> allUpgradeDatas; 

    // 각 업그레이드 타입별 현재 레벨을 저장하는 딕셔너리
    private Dictionary<UpgradeData.UpgradeType, int> currentUpgradeLevels = new Dictionary<UpgradeData.UpgradeType, int>();

    // UI 업데이트용 이벤트 (업그레이드 타입, 현재 레벨, 다음 레벨 비용)
    public static event Action<UpgradeData.UpgradeType, int, int> OnUpgradeUIUpdated;

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

        // 모든 업그레이드 타입의 초기 레벨을 0으로 설정
        foreach (UpgradeData data in allUpgradeDatas)
        {
            if (!currentUpgradeLevels.ContainsKey(data.upgradeType))
            {
                currentUpgradeLevels.Add(data.upgradeType, 0);
            }
        }
    }

    void OnEnable()
    {
        // 게임 상태가 업그레이드 시간으로 변경될 때 UI 업데이트 트리거
        GameManager.OnUpgradeTimeStarted += InitializeUpgradeUI;
    }

    void OnDisable()
    {
        GameManager.OnUpgradeTimeStarted -= InitializeUpgradeUI;
    }

    // 게임 시작 시 또는 업그레이드 시간이 시작될 때 UI 초기화 및 업데이트를 위해 호출
    private void InitializeUpgradeUI()
    {
        foreach (UpgradeData data in allUpgradeDatas)
        {
            int currentLevel = GetCurrentLevel(data.upgradeType);
            int nextCost = data.GetUpgradeCost(currentLevel);
            OnUpgradeUIUpdated?.Invoke(data.upgradeType, currentLevel, nextCost);
        }
    }

    // 특정 업그레이드 타입의 현재 레벨을 반환
    public int GetCurrentLevel(UpgradeData.UpgradeType type)
    {
        return currentUpgradeLevels.ContainsKey(type) ? currentUpgradeLevels[type] : 0;
    }

    // 특정 업그레이드 타입을 시도하고 성공 여부 반환
    public bool TryUpgrade(UpgradeData.UpgradeType type, PlayerController playerController)
    {
        UpgradeData dataToUpgrade = allUpgradeDatas.Find(data => data.upgradeType == type);

        if (dataToUpgrade == null)
        {
            Debug.LogError($"업그레이드 데이터를 찾을 수 없음: {type}");
            return false;
        }

        int currentLevel = GetCurrentLevel(type);

        // 최대 레벨인지 확인
        if (dataToUpgrade.IsMaxLevel(currentLevel))
        {
            Debug.Log($"{dataToUpgrade.upgradeName}은(는) 이미 최대 레벨입니다.");
            return false;
        }

        int upgradeCost = dataToUpgrade.GetUpgradeCost(currentLevel);

        // 코인이 충분한지 확인하고 사용
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.TrySpendCoins(upgradeCost))
        {
            // 업그레이드 레벨 증가
            currentUpgradeLevels[type]++;
            int newLevel = currentUpgradeLevels[type];

            // 플레이어 컨트롤러에 업그레이드 효과 적용
            float upgradeValue = dataToUpgrade.GetLevelValue(newLevel);
            ApplyUpgradeEffect(type, upgradeValue, playerController);

            Debug.Log($"{dataToUpgrade.upgradeName}을(를) 레벨 {newLevel}로 업그레이드했습니다! 비용: {upgradeCost}");

            // UI 업데이트 이벤트 발생
            OnUpgradeUIUpdated?.Invoke(type, newLevel, dataToUpgrade.GetUpgradeCost(newLevel));
            return true;
        }
        else if (CurrencyManager.Instance == null)
        {
            Debug.LogError("CurrencyManager 인스턴스를 찾을 수 없습니다.");
        }

        Debug.Log("코인이 부족하여 업그레이드할 수 없습니다.");
        return false;
    }

    // 업그레이드 종류에 따라 PlayerController에 효과 적용
    private void ApplyUpgradeEffect(UpgradeData.UpgradeType type, float value, PlayerController playerController)
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController가 null입니다. 업그레이드 효과를 적용할 수 없습니다.");
            return;
        }

        switch (type)
        {
            case UpgradeData.UpgradeType.FireRate:
                playerController.IncreaseFireRate(value);
                break;
            case UpgradeData.UpgradeType.Damage:
                playerController.IncreaseDamage(value);
                break;
            case UpgradeData.UpgradeType.MoveSpeed:
                playerController.IncreaseMoveSpeed(value);
                break;
        }
    }
}
