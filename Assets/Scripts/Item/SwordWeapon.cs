
using UnityEngine;

public class SwordWeapon : WeaponBase
{
    protected override void Attack()
    {
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
                Vector3 hitPoint = enemy.transform.position + Vector3.up * 1f;
                Vector3 hitNormal = transform.forward;
                target.OnDamage(damage, hitPoint, hitNormal);

                Debug.Log($"{enemy.name}에게 {damage} 데미지!");
            }
        }
    }
}
