using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    // Tags
    public const string ENEMY_TAG = "Enemy";
    public const string PLAYER_TAG = "Player";
    public const string BULLET_TAG = "Bullet";
    public const string COIN_TAG = "Coin";

    private float screenMinX, screenMaxX, screenMinY, screenMaxY;
    private float playerHalfWidth, playerHalfHeight;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movementInput;

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;
    [SerializeField] private float bulletDamage = 1f; // Initial bullet damage

    // 자동 사격 타이머
    private float fireTimer;

    // Event for coin collection
    public static event Action<int> OnCoinCollected;

    void Update()
    {
        HandleMovementInput();

        // --- 자동 사격 로직 추가 ---
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Start()
    {
        // 화면 경계 계산
        Vector3 screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        screenMinX = screenBottomLeft.x;
        screenMaxX = screenTopRight.x;
        screenMinY = screenBottomLeft.y;
        screenMaxY = screenTopRight.y;

        // 플레이어 크기 계산 (SpriteRenderer 또는 Collider2D 사용)
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            playerHalfWidth = spriteRenderer.bounds.extents.x;
            playerHalfHeight = spriteRenderer.bounds.extents.y;
        }
        else
        {
            // SpriteRenderer가 없다면 Collider2D 사용 (있을 경우)
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                playerHalfWidth = collider.bounds.extents.x;
                playerHalfHeight = collider.bounds.extents.y;
            }
            else
            {
                // 기본값 설정 (수동 조정 필요)
                Debug.LogWarning("PlayerController: No SpriteRenderer or Collider2D found. Player bounds might be inaccurate.");
                playerHalfWidth = 0.5f; // 임시 기본값
                playerHalfHeight = 0.5f; // 임시 기본값
            }
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
        // 플레이어를 화면 경계 내로 고정
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, screenMinX + playerHalfWidth, screenMaxX - playerHalfWidth);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, screenMinY + playerHalfHeight, screenMaxY - playerHalfHeight);
        transform.position = clampedPosition;
    }

    private void HandleMovementInput()
    {
        // PC Input (WASD)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        // Mobile Touch (Placeholder for joystick, actual implementation would use UI joystick)
        // For now, if touch input is detected, override PC input for simplicity in base script
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                // Simple touch movement: move towards touch position (will need refinement for joystick)
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                movementInput = (touchPos - transform.position).normalized;
            } else if (touch.phase == TouchPhase.Ended) {
                movementInput = Vector2.zero;
            }
        }
    }

    private void MovePlayer()
    {
        transform.Translate(movementInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            // Assuming Bullet script has a SetDamage method
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(bulletDamage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(COIN_TAG))
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin != null)
            {
                OnCoinCollected?.Invoke(coin.Value);
                Destroy(other.gameObject);
            }
        }
        // Player should only collect coins, not collide with enemies or bullets
    }

    // Methods for upgrades
    public void IncreaseFireRate(float amount)
    {
        fireRate = Mathf.Max(0.1f, fireRate - amount); // Ensure fireRate doesn't go below a certain threshold
    }

    public void IncreaseDamage(float amount)
    {
        bulletDamage += amount;
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }
}
