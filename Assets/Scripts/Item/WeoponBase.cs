using UnityEngine;

// ========== 기본 무기 클래스 ==========
public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float attackRange = 4f;
    public float attackCooldown = 0.5f;

    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Animator weaponAnimator;

    protected float lastAttackTime = 0f;

    protected virtual void Start()
    {
        // attackPoint가 없으면 무기 자신의 위치를 사용
        if (attackPoint == null)
            attackPoint = transform;

        // 플레이어 찾기 (부모 오브젝트)
        Transform parent = transform.parent;
        if (parent != null)
        {
            Debug.Log("무기가 " + parent.name + "에 장착되었습니다.");
        }
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
        {
            TryAttack();
        }
    }

    protected virtual void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            Debug.Log("아직 공격할 수 없습니다!");
            return;
        }

        lastAttackTime = Time.time;
        Attack();
    }

    // 자식 클래스에서 구현
    protected abstract void Attack();

    // 공격 범위 시각화
    protected virtual void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            attackPoint = transform;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}