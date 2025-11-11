using System;
using UnityEngine;

public class Player : LivingEntity
{
    public override void Die()
    {
        base.Die();
        Debug.Log("플레이어 사망 처리 (예: GameOver UI)");
        // 사망 애니메이션, 게임오버 UI 띄우기 등
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        Debug.Log("플레이어가 피해를 받음. 현재 체력: " + health);
    }
}
