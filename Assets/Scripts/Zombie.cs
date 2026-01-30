using UnityEngine;

// Enemy 추상 클래스를 상속받는 구체적인 좀비 클래스
// 이 스크립트를 실제 좀비 GameObject에 컴포넌트로 추가합니다.
public class Zombie : Enemy
{
    // Zombie 고유의 추가적인 속성이나 행동이 필요하다면 여기에 정의합니다.
    // 예: 공격 타입, 특수 능력 등
    // [SerializeField] private float attackDamage = 1f;

    // Start 메서드를 오버라이드하여 좀비 고유의 초기화 로직을 추가할 수 있습니다.
    protected override void Start()
    {
        base.Start(); // 부모 클래스(Enemy)의 Start 메서드를 호출하여 기본 초기화를 수행합니다.
        // Debug.Log("Zombie Start");
        // 여기에 Zombie 고유의 초기화 로직을 추가할 수 있습니다.
    }

    // Update 메서드를 오버라이드하여 좀비 고유의 업데이트 로직을 추가할 수 있습니다.
    protected override void Update()
    {
        base.Update(); // 부모 클래스(Enemy)의 Update 메서드를 호출하여 기본 행동(아래로 이동)을 수행합니다.
        // Debug.Log("Zombie Update");
        // 여기에 Zombie 고유의 업데이트 로직을 추가할 수 있습니다.
    }

    // Die 메서드를 오버라이드하여 좀비 고유의 사망 로직을 추가할 수 있습니다.
    protected override void Die()
    {
        // Debug.Log("Zombie Die");
        // 여기에 Zombie 고유의 사망 효과(예: 애니메이션 재생, 특수 아이템 드랍)를 추가합니다.
        base.Die(); // 부모 클래스(Enemy)의 Die 메서드를 호출하여 코인 드랍 및 GameObject 파괴 등 기본 사망 처리를 수행합니다.
    }

    // 추가적인 좀비 고유의 메서드나 이벤트 핸들러를 정의할 수 있습니다.
}
