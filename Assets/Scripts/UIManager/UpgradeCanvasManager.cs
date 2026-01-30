using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 필요
using System.Collections.Generic;
using System;

// 업그레이드 Canvas의 UI 요소들을 관리하는 스크립트
public class UpgradeCanvasManager : MonoBehaviour
{
    [Header("업그레이드 UI 패널")]
    [SerializeField] private GameObject upgradePanel; // 업그레이드 패널 전체 (활성화/비활성화용)

    [Header("업그레이드 버튼 및 텍스트")]
    [SerializeField] private Button fireRateUpgradeButton; // 발사 속도 업그레이드 버튼
    [SerializeField] private Text fireRateLevelText; // 발사 속도 현재 레벨 텍스트
    [SerializeField] private Text fireRateCostText; // 발사 속도 다음 레벨 비용 텍스트

    [SerializeField] private Button damageUpgradeButton; // 데미지 업그레이드 버튼
    [SerializeField] private Text damageLevelText; // 데미지 현재 레벨 텍스트
    [SerializeField] private Text damageCostText; // 데미지 다음 레벨 비용 텍스트

    [SerializeField] private Button moveSpeedUpgradeButton; // 이동 속도 업그레이드 버튼
    [SerializeField] private Text moveSpeedLevelText; // 이동 속도 현재 레벨 텍스트
    [SerializeField] private Text moveSpeedCostText; // 이동 속도 다음 레벨 비용 텍스트

    [Header("웨이브 시작 버튼")]
    [SerializeField] private Button startWaveButton; // 다음 웨이브 시작 버튼

    void Awake()
    {
        // 초기에는 업그레이드 패널 비활성화
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        // 게임 상태 변경 이벤트 구독
        GameManager.OnGameStateChanged += OnGameStateChangedHandler;
        // 업그레이드 UI 업데이트 이벤트 구독
        UpgradeManager.OnUpgradeUIUpdated += UpdateUpgradeUI;

        // 각 업그레이드 버튼에 클릭 리스너 추가
        fireRateUpgradeButton?.onClick.AddListener(() => OnUpgradeButtonClicked(UpgradeData.UpgradeType.FireRate));
        damageUpgradeButton?.onClick.AddListener(() => OnUpgradeButtonClicked(UpgradeData.UpgradeType.Damage));
        moveSpeedUpgradeButton?.onClick.AddListener(() => OnUpgradeButtonClicked(UpgradeData.UpgradeType.MoveSpeed));
        startWaveButton?.onClick.AddListener(OnStartWaveButtonClicked);
    }

    void OnDisable()
    {
        // 스크립트 비활성화 시 이벤트 구독 및 리스너 해제
        GameManager.OnGameStateChanged -= OnGameStateChangedHandler;
        UpgradeManager.OnUpgradeUIUpdated -= UpdateUpgradeUI;

        fireRateUpgradeButton?.onClick.RemoveAllListeners();
        damageUpgradeButton?.onClick.RemoveAllListeners();
        moveSpeedUpgradeButton?.onClick.RemoveAllListeners();
        startWaveButton?.onClick.RemoveAllListeners();
    }

    // 게임 상태 변경 시 호출되는 핸들러
    private void OnGameStateChangedHandler(GameManager.GameState newState)
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(newState == GameManager.GameState.UpgradeTime);
            if (newState == GameManager.GameState.UpgradeTime)
            {
                // 업그레이드 시간이 시작되면 현재 업그레이드 상태를 UI에 반영 (UpgradeManager에서 이벤트 발생시킴)
                Debug.Log("업그레이드 시간: Upgrade Canvas 활성화");
            }
            else
            {
                Debug.Log("업그레이드 시간 종료: Upgrade Canvas 비활성화");
            }
        }
    }

    // 업그레이드 버튼 클릭 시 호출되는 메서드
    private void OnUpgradeButtonClicked(UpgradeData.UpgradeType type)
    {
        // PlayerController 인스턴스는 게임 내에 하나만 존재하므로 FindObjectOfType으로 찾음
        // 실제 게임에서는 PlayerController를 UpgradeManager에 캐싱해두거나 다른 방식으로 전달하는 것이 좋음
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PlayerController를 찾을 수 없습니다. 업그레이드할 수 없습니다.");
            return;
        }

        UpgradeManager.Instance?.TryUpgrade(type, player);
    }

    // 업그레이드 UI를 업데이트하는 메서드
    private void UpdateUpgradeUI(UpgradeData.UpgradeType type, int currentLevel, int nextCost)
    {
        // 현재 업그레이드 데이터의 최대 레벨을 가져옵니다.
        UpgradeData data = UpgradeManager.Instance.allUpgradeDatas.Find(d => d.upgradeType == type);
        int maxLevel = (data != null) ? data.MaxLevel : 0;

        switch (type)
        {
            case UpgradeData.UpgradeType.FireRate:
                if (fireRateLevelText != null) fireRateLevelText.text = $"Lv. {currentLevel}";
                if (fireRateCostText != null)
                {
                    if (currentLevel >= maxLevel) fireRateCostText.text = "MAX";
                    else fireRateCostText.text = $"비용: {nextCost}";
                }
                if (fireRateUpgradeButton != null) fireRateUpgradeButton.interactable = (currentLevel < maxLevel && CurrencyManager.Instance.CurrentCoins >= nextCost);
                break;
            case UpgradeData.UpgradeType.Damage:
                if (damageLevelText != null) damageLevelText.text = $"Lv. {currentLevel}";
                if (damageCostText != null)
                {
                    if (currentLevel >= maxLevel) damageCostText.text = "MAX";
                    else damageCostText.text = $"비용: {nextCost}";
                }
                if (damageUpgradeButton != null) damageUpgradeButton.interactable = (currentLevel < maxLevel && CurrencyManager.Instance.CurrentCoins >= nextCost);
                break;
            case UpgradeData.UpgradeType.MoveSpeed:
                if (moveSpeedLevelText != null) moveSpeedLevelText.text = $"Lv. {currentLevel}";
                if (moveSpeedCostText != null)
                {
                    if (currentLevel >= maxLevel) moveSpeedCostText.text = "MAX";
                    else moveSpeedCostText.text = $"비용: {nextCost}";
                }
                if (moveSpeedUpgradeButton != null) moveSpeedUpgradeButton.interactable = (currentLevel < maxLevel && CurrencyManager.Instance.CurrentCoins >= nextCost);
                break;
        }
    }

    // 다음 웨이브 시작 버튼 클릭 시 호출
    private void OnStartWaveButtonClicked()
    {
        GameManager.Instance?.StartNextWave();
    }
}
