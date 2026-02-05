using UnityEngine;
using System;
using TMPro; // TextMeshPro를 사용하기 위해 추가
using UnityEngine.UI; // UI 요소를 사용하기 위해 추가

public class Barricade : MonoBehaviour, IHealth
{
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth;

    // [SerializeField] private TextMeshProUGUI healthText; // 이제 Slider 사용
    [SerializeField] private Slider healthSlider; // 체력 게이지 Slider
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 1.5f, 0); // 체력바 상대 위치 오프셋

    public static event Action OnBarricadeDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // 초기 체력 UI 업데이트
        Debug.Log($"Barricade: {gameObject.name} initialized with HP: {currentHealth}"); // 초기 HP 값 로그
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

    void LateUpdate()
    {
        if (healthSlider != null)
        {
            healthSlider.transform.position = transform.position + healthBarOffset;
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
        // if (healthText != null)
        // {
        //     healthText.text = Mathf.CeilToInt(currentHealth).ToString();
        // }
    }
    private void Die()
    {
        OnBarricadeDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
