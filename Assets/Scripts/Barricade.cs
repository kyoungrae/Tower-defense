using UnityEngine;
using System;

public class Barricade : MonoBehaviour, IHealth
{
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth;

    public static event Action OnBarricadeDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnBarricadeDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
