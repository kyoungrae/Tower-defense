using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Tags
    public const string ENEMY_TAG = "Enemy";
    public const string PLAYER_TAG = "Player";
    public const string BULLET_TAG = "Bullet";
    public const string COIN_TAG = "Coin";

    [SerializeField] private float bulletSpeed = 10f;
    private float damage;

    void Update()
    {
        // 직진 이동
        transform.Translate(Vector2.up * bulletSpeed * Time.deltaTime);
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy와 충돌 시 데미지 전달 후 소멸
        if (other.CompareTag(ENEMY_TAG))
        {
            IHealth enemyHealth = other.GetComponent<IHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // 총알 소멸
        }
    }

    // 일정 시간 뒤 자동 소멸 (화면 밖으로 나가는 경우 등)
    // 필요에 따라 Collider나 Rigidbody 설정 후 AddForce로 발사하거나, 카메라 범위 밖에서 Destroy 하는 로직 추가
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
