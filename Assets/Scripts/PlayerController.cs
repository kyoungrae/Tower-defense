using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    // Tags
    public const string ENEMY_TAG = "Enemy";
    public const string PLAYER_TAG = "Player";
    public const string BULLET_TAG = "Bullet";
    public const string COIN_TAG = "Coin";

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movementInput;

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;
    [SerializeField] private float bulletDamage = 1f; // Initial bullet damage

    // Event for coin collection
    public static event Action<int> OnCoinCollected;

    void Update()
    {
        HandleMovementInput();
        HandleShootingInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
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

    private void HandleShootingInput()
    {
        // PC Shooting (Mouse Left Click)
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Mobile Touch Shooting (Placeholder, assuming touch anywhere for shooting if not moving)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // If not moving via touch, assume it's a tap to shoot
            if (touch.phase == TouchPhase.Began && movementInput == Vector2.zero && Time.time >= nextFireTime)
            {
                 Shoot();
                 nextFireTime = Time.time + fireRate;
            }
        }
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
