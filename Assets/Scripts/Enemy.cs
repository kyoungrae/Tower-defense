using UnityEngine;
using System; // For Action event

public interface IHealth
{
    void TakeDamage(float amount);
}

public abstract class Enemy : MonoBehaviour, IHealth
{
    // Tags
    public const string ENEMY_TAG = "Enemy";
    public const string PLAYER_TAG = "Player";
    public const string BULLET_TAG = "Bullet";
    public const string COIN_TAG = "Coin";

    [Header("Enemy Stats")]
    [SerializeField] public float maxHealth = 10f; // 최대 체력
    [SerializeField] protected float moveSpeed = 1f; // 이동 속도
    [SerializeField] protected int dropCoinAmount = 1; // 드랍 코인 양
    [SerializeField] protected GameObject coinPrefab; // 스폰할 코인 프리팹

    public float currentHealth; // 현재 체력

    // Event for enemy death (can be used by GameManager/WaveManager)
    public static event Action OnEnemyDied;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        MoveDown();
    }

    protected virtual void MoveDown()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        DropCoin();
        OnEnemyDied?.Invoke();
        Destroy(gameObject);
    }

    protected virtual void DropCoin()
    {
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    // Optional: Implement other behaviors like attacking player, reaching bottom wall etc.
}
