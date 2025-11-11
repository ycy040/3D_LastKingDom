using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamageable 
{

    public float startingHealth = 100;
    public float health { get; protected set; } // 현재 체력
    public bool dead { get; protected set; } // 사망
    public event Action onDeath;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) 
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    
    // 아이템과 연결, 체력 회복
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            return; // 회복 불가
        }

        health += newHealth;
    }

    public virtual void Die()
    {
        if (onDeath != null)
        {
            onDeath();
        }

        dead = true;
    }
}
