using System;
using UnityEngine;

public class Player : LivingEntity
{
    private static Player instance;

    private void Awake()
    {
        // 싱글톤은 GameManager에서 관리하므로 제거
        // 씬마다 플레이어가 있으므로 DontDestroyOnLoad 제거
    }

    void Start()
    {
        // GameManager와 HP 동기화
        if (GameManager.Instance != null)
        {
            health = GameManager.Instance.GetCurrentHP();
            startingHealth = GameManager.Instance.GetMaxHP();
        }
    }

    void Update()
    {
        // 부모 클래스의 Update 호출 (GameManager 동기화)
        base.Update();
    }

    public override void Die()
    {
        if (dead) return; // 중복 사망 방지

        base.Die();
        Debug.Log("플레이어 사망 처리");

        // GameManager를 통해 사망 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // GameManager를 통해 데미지 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damage);
        }
        else
        {
            base.OnDamage(damage, hitPoint, hitNormal);
        }

        Debug.Log("플레이어가 데미지를 받음. 현재 체력: " + health);
    }
}