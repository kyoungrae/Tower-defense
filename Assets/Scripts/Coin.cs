using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    // Tags
    public const string ENEMY_TAG = "Enemy";
    public const string PLAYER_TAG = "Player";
    public const string BULLET_TAG = "Bullet";
    public const string COIN_TAG = "Coin";

    [SerializeField] private int value = 1;
    [SerializeField] private float lifetime = 5f; // 코인이 사라지는 시간
    [SerializeField] private float fadeStartTime = 2f; // 점멸 시작 시간

    private SpriteRenderer spriteRenderer;
    private bool fading = false;

    public int Value => value;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Coin object is missing a SpriteRenderer component.");
            enabled = false; // Disable script if no SpriteRenderer
        }
    }

    void Start()
    {
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(fadeStartTime);

        fading = true;
        float timer = lifetime - fadeStartTime;
        Color startColor = spriteRenderer.color;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * 5f)); // 간단한 점멸 효과
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    // PlayerController에서 코인을 습득하므로, 여기서는 충돌 처리 안 함
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag(PLAYER_TAG))
    //     {
    //         // 코인 습득 로직 (PlayerController에서 처리)
    //         Destroy(gameObject);
    //     }
    // }
}
