using UnityEngine;
using System; // For Action event
using TMPro; // For TextMeshPro

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
    [SerializeField] private TextMeshProUGUI healthText; // 체력을 표시할 TextMeshPro UI

    [Header("Attack Stats")]
    [SerializeField] protected float attackDamage = 10f; // 공격력
    [SerializeField] protected float attackSpeed = 1f; // 공격 속도 (초당 공격 횟수)
    protected float attackCooldown = 0f; // 다음 공격까지 남은 시간

    protected bool isAttacking = false; // 공격 중인지 여부
    protected IHealth targetBarricade; // 공격 대상 바리케이드

    public float currentHealth; // 현재 체력

    // Event for enemy death (can be used by GameManager/WaveManager)
    public static event Action OnEnemyDied;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // 초기 체력 UI 업데이트
    }

    protected virtual void Update()
    {
        if (!isAttacking)
        {
            MoveDown();
        }
        else
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                Attack();
                attackCooldown = 1f / attackSpeed;
            }
        }
    }

    protected virtual void MoveDown()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
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

    protected void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = Mathf.CeilToInt(currentHealth).ToString(); // 정수형으로 반올림하여 표시
        }
    }

    protected virtual void Attack()
    {
        if (targetBarricade != null)
        {
            targetBarricade.TakeDamage(attackDamage);
        }
        else
        {
            isAttacking = false; // 타겟이 사라지면 공격 중지
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Barricade")) // "Barricade" 태그를 가진 오브젝트와 충돌 시
        {
            isAttacking = true;
            targetBarricade = other.GetComponent<IHealth>();
            attackCooldown = 0f; // 즉시 공격 시작
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Barricade"))
        {
            isAttacking = false;
            targetBarricade = null;
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
