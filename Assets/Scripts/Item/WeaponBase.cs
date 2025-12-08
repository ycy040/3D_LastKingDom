
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage;
    public float attackRange;
    public float attackCooldown;

    [Header("Attack Settings")]
    public LayerMask enemyLayer;
    public Transform attackPoint;

    protected float lastAttackTime = 0f;

    protected virtual void Start()
    {
        if (attackPoint == null)
        {
            Transform weaponSlot = transform.parent;

            if (weaponSlot != null)
                attackPoint = weaponSlot.Find("AttackPoint");

            if (attackPoint == null)
            {
                Debug.LogWarning("AttackPoint를 찾지 못했습니다. 무기 위치를 AttackPoint로 사용합니다.");
                attackPoint = transform;
            }
            else
            {
                Debug.Log("AttackPoint 자동 연결됨 → " + attackPoint.name);
            }
        }
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
        {
            TryAttack();
        }
    }

    public void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    protected abstract void Attack();

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
