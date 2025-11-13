using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100;
    public float health { get; set; } // GameManager와 동기화될 체력
    public bool dead { get; set; }
    public event Action onDeath;

    public virtual void OnEnable()
    {
        dead = false;

        // GameManager가 있으면 GameManager의 HP 사용
        if (GameManager.Instance != null)
        {
            health = GameManager.Instance.GetCurrentHP();
        }
        else
        {
            health = startingHealth;
        }
    }

    public void Update()
    {
        // GameManager와 항상 동기화
        if (GameManager.Instance != null)
        {
            health = GameManager.Instance.GetCurrentHP();

            // GameManager에서 HP가 0이면 사망 처리
            if (health <= 0 && !dead)
            {
                Die();
            }
        }
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        // GameManager를 통해 데미지 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damage);
        }
        else
        {
            // GameManager가 없으면 로컬 처리
            health -= damage;

            if (health <= 0 && !dead)
            {
                Die();
            }
        }
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            return;
        }

        // GameManager를 통해 회복 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Heal(newHealth);
        }
        else
        {
            // GameManager가 없으면 로컬 처리
            health += newHealth;
        }
    }

    public virtual void Die()
    {
        if (dead) return; // 중복 사망 방지

        dead = true;

        onDeath?.Invoke();

        Debug.Log($"{gameObject.name} 사망");
    }
}