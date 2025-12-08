
using UnityEngine;

public class KnifeWeapon : WeaponBase
{
    protected override void Attack()
    {
        // 공격 범위 내의 모든 적 탐지
        Collider[] hitEnemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider enemy in hitEnemies)
        {
            IDamageable target = enemy.GetComponent<IDamageable>();
            if (target != null)
            {
                // 타격 위치 및 방향 설정
                Vector3 hitPoint = enemy.transform.position + Vector3.up * 1f;
                Vector3 hitNormal = transform.forward;

                // 데미지 전달
                target.OnDamage(damage, hitPoint, hitNormal);

                Debug.Log($"{enemy.name}에게 Knife로 {damage} 데미지!");
            }
        }
    }
}
