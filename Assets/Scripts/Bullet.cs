using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // 총알 이동 속도
    [SerializeField] private float damage = 10f; // 총알 공격력

    void Update()
    {
        // 총알을 위로 이동
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void SetDamage(float newDamage) // 이 메서드를 추가합니다
    {
        damage = newDamage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy 태그를 가진 오브젝트와 충돌 시
        if (other.CompareTag("Enemy"))
        {
            IHealth enemyHealth = other.GetComponent<IHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // 적에게 데미지 적용
            }
            Destroy(gameObject); // 총알 제거
        }
        // 화면 밖으로 나가면 총알 제거 (Optional: Bounds Check)
        // ... (필요하다면 화면 경계 체크 로직 추가)
    }
}
