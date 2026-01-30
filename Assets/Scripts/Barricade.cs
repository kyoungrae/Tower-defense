using UnityEngine;
using System;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class Barricade : MonoBehaviour, IHealth
{
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth;

    [SerializeField] private TextMeshProUGUI healthText; // 이 줄을 추가

    public static event Action OnBarricadeDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // 초기 체력 UI 업데이트
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI(); // 체력 변경 시 UI 업데이트
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI() // 이 메서드를 추가
    {
        if (healthText != null)
        {
            healthText.text = Mathf.CeilToInt(currentHealth).ToString();
        }
    }

    private void Die()
    {
        OnBarricadeDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
