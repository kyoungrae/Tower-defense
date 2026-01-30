using UnityEngine;

// UpgradeData ScriptableObject를 생성하기 위한 메뉴 추가
[CreateAssetMenu(fileName = "NewUpgradeData", menuName = "Upgrade System/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    // 업그레이드 항목의 종류를 나타내는 열거형
    public enum UpgradeType
    {
        FireRate,
        Damage,
        MoveSpeed
    }

    [Header("업그레이드 정보")]
    public UpgradeType upgradeType; // 업그레이드 종류
    public string upgradeName; // 업그레이드 이름 (예: "발사 속도 증가")
    public string description; // 업그레이드 설명

    [Header("레벨 및 비용")]
    [SerializeField] private int maxLevel = 5; // 최대 업그레이드 레벨
    [SerializeField] private int basePrice = 10; // 기본 업그레이드 비용

    // 각 레벨별로 적용될 값 (예: 발사 속도 감소량, 데미지 증가량, 이동 속도 증가량)
    // 인덱스는 레벨 - 1 에 해당하며, 0번 인덱스는 레벨 1의 값을 의미
    [SerializeField] private float[] levelValues; 

    // 현재 레벨은 런타임에 관리되므로 ScriptableObject 자체에는 저장하지 않음
    // 이 데이터는 주로 UpgradeManager나 PlayerData 등에서 관리될 것임

    // 특정 레벨의 업그레이드 값을 반환
    public float GetLevelValue(int level)
    {
        if (level > 0 && level <= maxLevel && level - 1 < levelValues.Length)
        {
            return levelValues[level - 1];
        }
        Debug.LogWarning($"유효하지 않은 업그레이드 레벨 요청: {upgradeName}, Level: {level}");
        return 0f;
    }

    // 다음 레벨로 업그레이드하는 데 필요한 비용 계산
    public int GetUpgradeCost(int currentLevel)
    {
        // BasePrice * (CurrentLevel * 1.5) 공식 적용
        // currentLevel은 0부터 시작하는 레벨 (0레벨에서 1레벨로 업그레이드할 때)로 가정
        // 실제 게임에서는 1레벨부터 시작하므로 currentLevel + 1을 사용하거나, 0레벨을 기본 상태로 간주
        if (currentLevel < maxLevel)
        {
            return Mathf.RoundToInt(basePrice * (currentLevel + 1) * 1.5f);
        }
        return -1; // 최대 레벨에 도달했음을 의미
    }

    // 최대 레벨에 도달했는지 확인
    public bool IsMaxLevel(int currentLevel)
    {
        return currentLevel >= maxLevel;
    }

    public int MaxLevel => maxLevel;
    public int BasePrice => basePrice;
}
